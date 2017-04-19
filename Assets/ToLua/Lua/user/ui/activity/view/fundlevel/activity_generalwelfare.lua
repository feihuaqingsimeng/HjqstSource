local t = activity_base_view.activity_view()
local name = 'activity_everyday_view'

local activity_ctrl = gamemanager.GetCtrl('activity_controller')
local activity_model = gamemanager.GetModel('activity_model')
local event_data = gamemanager.GetData('event_data')
local activity_generalwelfare_item = require("ui/activity/view/fundlevel/activity_generalwelfare_item")

t._TextTitle = nil
t.scrollcontent = nil
t.ItemDataList = {}
t.m_aryScrollItem = {}

function t:Start(transform)
  if nil == self.transform then
		self.transform = transform    
    self._TextTitle = self.transform:FindChild('desc'):GetComponent(typeof(Text)) 
    self.scrollcontent = self.transform:FindChild('scrollrect/viewport/content'):GetComponent(typeof(ScrollContent))
    self.scrollcontent.onResetItem:RemoveAllListeners()
    self.scrollcontent.onResetItem:AddListener(function(go, index) self:OnItemInit(go, index) end)    
    self.type = 150
  end  
end

function t:Open()  
end

function t:Init()  
  local data = event_data.GetDataByType(self.type)
  if nil == data then do return end end
  
  local activityData = activity_model:GetActivityByType(self.type)
  if nil == activityData then do return end end
  
  self._TextTitle.text = string.format(LocalizationController.instance:Get("event_1011_generalWelfareTitle"), activityData.conditionValue)
  
  self.ItemDataList = self:GetListData()
  self:InitItems()
  
  self:RefreshScrollContent()
end

function t:InitItems()  
  self.scrollcontent:Init(#self.ItemDataList, true, 0)
end

function t:Refresh()
  activity_base_view.Refresh(self)
  self:Init()
end
function t:RefreshScrollContent()
  self.ItemDataList = self:GetListData()
  self.scrollcontent:RefreshAllContentItems()
end

function t:OnItemInit(go, index)
  local itemdata = self:GetItemByIndex(index + 1)
  if nil == itemdata then do return end end
  
   if self.m_aryScrollItem[go] == nil then
      self.m_aryScrollItem[go]= activity_generalwelfare_item.New(go.transform)
      self.m_aryScrollItem[go].DelegateOnClick:RemoveAllListener()
      self.m_aryScrollItem[go].DelegateOnClick:AddListener(function(info) self:OnGetRewardBtnClick(info) end) 
  end
  
  t.m_aryScrollItem[go]:Init(itemdata) 
end

function t:GetItemByIndex(index)
  for i = 1, #self.ItemDataList do if i == index then return self.ItemDataList[i] end end
end

function t:OnGetRewardBtnClick(info)
  activity_ctrl.ReqActivityReward(info.type, info.id, info.subType)
end

return t