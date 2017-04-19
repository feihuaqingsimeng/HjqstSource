local t = activity_base_view.activity_view()
local name = 'activity_foundlevel_view'

local activity_ctrl = gamemanager.GetCtrl('activity_controller')
local activity_model = gamemanager.GetModel('activity_model')
local event_data = gamemanager.GetData('event_data')

local activity_fundlevel_item = require("ui/activity/view/fundlevel/activity_fundlevel_item")

t._Text1 = nil
t._Text2 = nil
t._Text3 = nil
t._Text4 = nil
t.scrollcontent = nil
t.generalwelfare = nil
t._ToggleBtn = nil
t.vipPanel = nil
t.ItemDataList = {}
t.m_aryScrollItem = {}

function t:Start(btype)
  activity_base_view.Start(self, btype)
  
  self._Text1 = self.transform:FindChild('right/found_level_gift/titletime'):GetComponent(typeof(Text))
  self._Text2 = self.transform:FindChild('right/found_level_gift/titledesc'):GetComponent(typeof(Text))
  self._Text3 = self.transform:FindChild('right/found_level_gift/titledesc/desc'):GetComponent(typeof(Text))
  self._Text4 = self.transform:FindChild('right/found_level_gift/titletime/time'):GetComponent(typeof(Text)) 
  
  self.scrollcontent = self.transform:FindChild('right/found_level_gift/scrollrect/viewport/content'):GetComponent(typeof(ScrollContent))
  self.scrollcontent.onResetItem:RemoveAllListeners()
	self.scrollcontent.onResetItem:AddListener(function(go, index) self:OnItemInit(go, index) end)
  
  self.generalwelfare = dofile("ui/activity/view/fundlevel/activity_generalwelfare")
  local generalwelfare = self.transform:FindChild('right/general_welfare')
  self.generalwelfare:Start(generalwelfare)
  
  self._ToggleBtn = self.transform:FindChild('right/tabs/1'):GetComponent(typeof(Toggle))
  self._ToggleBtn.onValueChanged:RemoveAllListeners()
  self._ToggleBtn.onValueChanged:AddListener(function(value) self:OnTogglePageClick(value) end)
  
  self.vipPanel = dofile("ui/activity/view/fundlevel/activity_fundlevelvip")
  local vipPanel = self.transform:FindChild('left')
  self.vipPanel:Start(vipPanel)
  
  self:Init()
end

function t:Open()
  activity_base_view.Open(self)

  self:RefreshScrollContent()
  self.generalwelfare:Open()
  self.vipPanel:Open()
end

function t:Init()
  activity_base_view.Init(self)
  
  local data = event_data.GetDataByType(self.type)
  if nil == data then do return end end
  
  self._Text1.text = LocalizationController.instance:Get('event_public_time')..':'
  self._Text2.text = LocalizationController.instance:Get('event_public_describe')..':'
  self._Text3.text = LocalizationController.instance:Get(data.event_describe)
  
  local time1, time2 = uimanager.DealWithTime(data.event_timestart, data.event_timeover)
	if time2 then self._Text4.text = time1..'~'..time2
  else	self._Text4.text = time1	end 

  self.ItemDataList = self:GetListData()
  self:InitItems()
  
  self.generalwelfare:Init()
  self.vipPanel:Init(self.type)
end

function t:InitItems()  
  self.scrollcontent:Init(#self.ItemDataList, true, 0)
end

function t:ActivityUpdate()
  activity_base_view.Refresh(self)
  self:RefreshScrollContent()  
end

function t:Refresh()
  self.generalwelfare:Refresh()
end
function t:RefreshScrollContent()
  self.ItemDataList = self:GetListData()
  self.scrollcontent:RefreshAllContentItems()
end

function t:OnItemInit(go, index)
  local itemdata = self:GetItemByIndex(index + 1)
  if nil == itemdata then do return end end
  
   if self.m_aryScrollItem[go] == nil then
      self.m_aryScrollItem[go]= activity_fundlevel_item.New(go.transform)
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

function t:OnTogglePageClick(value) 
  if not value then do return end end  
  activity_ctrl.RepActivityList()
end

function t:Close()   
  activity_base_view.Close(self) 
  self.vipPanel:Close()
end

return t