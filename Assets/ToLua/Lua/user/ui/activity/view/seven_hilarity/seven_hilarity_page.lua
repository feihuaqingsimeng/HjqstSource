local t={}
local name='seven_hilarity_page'

local activity_ctrl = gamemanager.GetCtrl('activity_controller')
local activity_model = gamemanager.GetModel('activity_model')
local seven_hilarity_data = gamemanager.GetData('seven_hilarity_data')
local seven_hilarity_item = require("ui/activity/view/seven_hilarity/seven_hilarity_item")

t.transform = nil
t.scrollcontent = nil
t.ItemDataList = {}
t.day = 0
t.type = 0

t.m_aryScrollItem = {}

function t:Start(transform)
  if nil == self.transform then 
		self.transform = transform 
    self.scrollcontent = self.transform:FindChild('viewport/content'):GetComponent(typeof(ScrollContent))
    self.scrollcontent.onResetItem:RemoveAllListeners()
    self.scrollcontent.onResetItem:AddListener(function(go, index) self:OnItemInit(go, index) end)
  end
end

function t:Init(bday, btype)
  self.day = bday
  self.type = btype
  self.ItemDataList = activity_model:GetSevenHilarityDataByDayAndType(bday, btype)
  self:InitItems()
end

function t:Open()
  self.transform.gameObject:SetActive(true) 
end

function t:InitItems()  
  self.scrollcontent:Init(#self.ItemDataList, false, 0)  
end

function t:Refresh()
  self:RefreshScrollContent()
end

--列表刷新
function t:RefreshScrollContent()
  self.ItemDataList = activity_model:GetSevenHilarityDataByDayAndType(self.day, self.type)
  self.scrollcontent:RefreshAllContentItems()
end

function t:OnItemInit(go, index)
   if self.m_aryScrollItem[go] == nil then
      self.m_aryScrollItem[go]= seven_hilarity_item.New(go.transform)
      self.m_aryScrollItem[go].DelegateOnClick:RemoveAllListener()
      self.m_aryScrollItem[go].DelegateOnClick:AddListener(function(info) self:OnGetRewardBtnClick(info) end) 
  end
  
  local itemdata = self:GetItemByIndex(index + 1)
  if nil == itemdata then do return end end
  
  self.m_aryScrollItem[go]:Init(itemdata)   
end

function t:GetItemByIndex(index)
  for i = 1, #self.ItemDataList do if i == index then return self.ItemDataList[i] end end
end

function t:OnGetRewardBtnClick(CsvId)
  activity_ctrl.ReqGetSevenDayTaskAwardResp(CsvId)
end

function t:Close()  
  self.transform.gameObject:SetActive(false) 
end

return t