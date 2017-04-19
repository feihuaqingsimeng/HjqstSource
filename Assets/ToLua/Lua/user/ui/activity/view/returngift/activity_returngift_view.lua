local t = activity_base_view.activity_view()
local name = 'activity_returngift_view'

local activity_ctrl = gamemanager.GetCtrl('activity_controller')
local activity_model = gamemanager.GetModel('activity_model')
local event_data = gamemanager.GetData('event_data')
local bitem = require("ui/activity/view/returngift/activity_returngift_type")

t._TextTitle = nil
t._Text1 = nil
t._Text2 = nil
t._Text3 = nil
t._Text4 = nil
t.scrollcontent = nil
t.ItemDataList = {}
t.Panel = nil
t.m_aryScrollItem = {}

function t:Start(btype)
  activity_base_view.Start(self, btype)
  self._TextTitle = self.transform:FindChild('right/title/Text'):GetComponent(typeof(Text))
  self._Text1 = self.transform:FindChild('right/titletime'):GetComponent(typeof(Text))
  self._Text2 = self.transform:FindChild('right/titledesc'):GetComponent(typeof(Text))
  self._Text3 = self.transform:FindChild('right/titledesc/desc'):GetComponent(typeof(Text))
  self._Text4 = self.transform:FindChild('right/titletime/time'):GetComponent(typeof(Text))  
  self.scrollcontent = self.transform:FindChild('right/scrollrect/viewport/content'):GetComponent(typeof(ScrollContent))
  self.scrollcontent.onResetItem:RemoveAllListeners()
	self.scrollcontent.onResetItem:AddListener(function(go, index) self:OnItemInit(go, index) end)
  
  self.Panel = dofile("ui/activity/view/returngift/activity_returngift_panel")
  local panalGift = self.transform:FindChild('panel')
  self.Panel:Start()
  
  self:Init()
end

function t:Open()
   activity_base_view.Open(self) 
   self:RefreshScrollContent()
end

function t:Init()
  activity_base_view.Init(self)
  
  local data = event_data.GetDataByType(self.type)
  if nil == data then do return end end
  self._TextTitle.text = LocalizationController.instance:Get(data.event_name)  
  self._Text1.text = LocalizationController.instance:Get('event_public_time')..':'
  self._Text2.text = LocalizationController.instance:Get('event_public_describe')..':'
  self._Text3.text = LocalizationController.instance:Get(data.event_describe)
  
  local time1, time2 = uimanager.DealWithTime(data.event_timestart, data.event_timeover)
	if time2 then self._Text4.text = time1..'~'..time2
  else	self._Text4.text = time1	end 
  
  self.ItemDataList = self:GetListData()
  self:InitItems()
end

function t:InitItems()  
  self.scrollcontent:Init(#self.ItemDataList, false, 0)
end

function t:GetListData()  
  local blist = {}
  local tevent_data = event_data.GetTypeDatasByType(self.type)
  if nil == tevent_data then do return end end  

  for i = 1, #tevent_data do
    local isAlredyContain = false
    for j = 1, #blist do if blist[j].subType == tonumber(tevent_data[i].event_small_type) then isAlredyContain = true end end
    
    if not isAlredyContain then
      local o = {}
      o.type = tonumber(tevent_data[i].event_type)
      o.subType = tonumber(tevent_data[i].event_small_type)     
      table.insert(blist, o)
    end
    
    for index = 1, #blist do blist[index].IsJoinOne = IsJoinOne end
  end
  
  table.sort(blist, function(A, B) return self:SortActivitys(A, B) end)
  return blist
end

function t:SortActivitys(activityDataA, activityDataB)
  if nil == activityDataA or nil == activityDataB then return false end  
  local A, B = 0, 0  
  A = A + activityDataA.subType
  B = B + activityDataB.subType  
  return A < B
end

function t:Refresh()
  activity_base_view.Refresh(self)
  self:RefreshScrollContent()
  self.Panel.OnMsgGetRewardResp()
end

function t:ActivityUpdate()
  self:RefreshScrollContent()
  self.Panel:ActivityUpdate()
end

function t:RefreshScrollContent()
  self.ItemDataList = self:GetListData()
  self.scrollcontent:RefreshAllContentItems()
end

function t:OnItemInit(go, index)
  local itemdata = self:GetItemByIndex(index + 1)
  if nil == itemdata then do return end end
  
   if self.m_aryScrollItem[go] == nil then
      self.m_aryScrollItem[go]= activity_returngift_type.New(go.transform)
      self.m_aryScrollItem[go].DelegateOnClick:RemoveAllListener()
      self.m_aryScrollItem[go].DelegateOnClick:AddListener(function(info) self:OnFundationClick(info) end) 
  end
  
  t.m_aryScrollItem[go]:Init(itemdata)
end

function t:GetItemByIndex(index)
  for i = 1, #self.ItemDataList do if i == index then return self.ItemDataList[i] end end
end

function t:OnFundationClick(info)
  self.Panel:Open()
  self.Panel:Init(info)
end

function t:Close()
  activity_base_view.Close(self)
  self.Panel:Close()
end

function t:Clear()
  self.transform = nil
  self.Panel:Clear()
end

return t