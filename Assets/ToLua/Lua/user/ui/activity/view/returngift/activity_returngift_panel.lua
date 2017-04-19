local t = {}
local name = 'activity_returngift_panel'

local activity_ctrl = gamemanager.GetCtrl('activity_controller')
local activity_model = gamemanager.GetModel('activity_model')
local event_data = gamemanager.GetData('event_data')
local activity_returngift_item = require("ui/activity/view/returngift/activity_returngift_item")
local activity_main_view = nil

t._TextTitle = nil
t._TextPrice = nil
t._BtnAward = nil
t._BtnAwardText = nil
t._BtnClose = nil

t.scrollcontent = nil
t.ItemDataList = {}
t.info = {}
t.m_aryScrollItem = {}

function t:Start()
  if nil == self.transform then
    local transform = Instantiate(ResMgr.instance:Load("ui/activity/returngift_panel")).transform
    local activity_main_view = uimanager.GetView('activity_main_view')
		transform:SetParent(activity_main_view.frontform)
		transform:SetSiblingIndex(0)
		transform.localPosition = Vector3.zero
		transform.localScale = Vector3.one
    
    self.transform = transform 
    self._TextTitle = self.transform:FindChild('title'):GetComponent(typeof(Text))
    self._TextPrice = self.transform:FindChild('price/Text'):GetComponent(typeof(Text))

    self._BtnAward = self.transform:FindChild('buy'):GetComponent(typeof(Button))
    self._BtnAwardText = self.transform:FindChild('buy/Text'):GetComponent(typeof(Text))
    
    self._BtnClose = self.transform:FindChild('close'):GetComponent(typeof(Button))
    self._BtnClose.onClick:RemoveAllListeners()
    self._BtnClose.onClick:AddListener(function()	self:OnBtnClose() end)
    
    self.scrollcontent = self.transform:FindChild('scrollrect/viewport/content'):GetComponent(typeof(ScrollContent))
    self.scrollcontent.onResetItem:RemoveAllListeners()
    self.scrollcontent.onResetItem:AddListener(function(go, index) self:OnItemInit(go, index) end)
    
    self.transform.gameObject:SetActive(false)
  end
end

function t:Open()  
  self.transform.gameObject:SetActive(true)
  activity_ctrl.OnDlegategActivityJoinResp:AddListener(self.OnMsgJoinActivityResp)
end

function t:Init(info)
  self.info = info
  
  local data = event_data.GetDataBySubType(self.info.type, self.info.subType)
  if nil == data then do return end end
  
  local strPrice = string.split(data.event_buymoney, ":")

  self._TextTitle.text = LocalizationController.instance:Get(self.info.TitleName)  
  self._TextPrice.text = strPrice[3]
  
  self.ItemDataList = self:GetListData()
  self:InitItems()
  self:InitButtons()
end

function t:InitItems()  
  self.scrollcontent:Init(#self.ItemDataList, true, 0)
end

function t:InitButtons()  
  self._BtnAward.onClick:RemoveAllListeners()
  
  local isCurJoin = activity_model:IsActivityjoinBySubType(self.info.type, self.info.subType)
  local isjoinOne = activity_model:IsActivityjoin(self.info.type)
  
  if isCurJoin then 
      self._BtnAwardText.text = LocalizationController.instance:Get('event_1006_buy_btn2')
      ui_util.SetGrayChildren(self._BtnAward.transform, true, true)
  else
    if isjoinOne then 
      self._BtnAwardText.text = LocalizationController.instance:Get('event_1006_buy_btn3')
      ui_util.SetGrayChildren(self._BtnAward.transform, true, true)
    else
      self._BtnAward.onClick:AddListener(function()	self:OnBtnJoinActivityClick() end)
      self._BtnAwardText.text = LocalizationController.instance:Get('event_1006_buy_btn1')
      ui_util.SetGrayChildren(self._BtnAward.transform, false, true)
    end
  end
  
  self._BtnAward.gameObject:SetActive(true)
end

function t:GetListData()
  local activityData = activity_model:GetActivityByType(self.info.type)
  if nil == activityData then do return end end
  
  local isCurJoin = activity_model:IsActivityjoinBySubType(self.info.type, self.info.subType)
  local isjoinOne = activity_model:IsActivityjoin(self.info.type)
  
  local blist = {}
  if isCurJoin then    
    for k, v in pairs(activityData.scheduleStus.data) do
      local o = {}
      o.type = activityData.type
      o.subType = activityData.subType
      o.conditionValue = activityData.conditionValue
      o.id = k
      o.state = v
      o.IsJoin = isCurJoin
      o.IsJoinOne = isjoinOne
      table.insert(blist, o)
    end
  end
  
  local tevent_data = event_data.GetDatasBySubType(self.info.type, self.info.subType)
  if nil == tevent_data then do return end end
  
  for i = 1, #tevent_data do
    local isAlredyContain = false
    for j = 1, #blist do if blist[j].id == tonumber(tevent_data[i].small_id) then isAlredyContain = true end end
    
    if not isAlredyContain then
      local o = {}
      o.type = activityData.type
      o.subType = tonumber(tevent_data[i].event_small_type)
      o.conditionValue = activityData.conditionValue
      o.id = tonumber(tevent_data[i].small_id)
      o.state = 3
      o.IsJoin = activity_model:IsActivityjoinBySubType(activityData.type, o.subType)
      o.IsJoinOne = isjoinOne
      table.insert(blist, o)
    end
  end 
  
  table.sort(blist, function(A, B) return self:SortActivitys(A, B) end)
  return blist
end

function t:SortActivitys(activityDataA, activityDataB)
  if nil == activityDataA or nil == activityDataB then return false end
  
  local A, B = 0, 0
  local dataA = event_data.GetDataBySmallId(activityDataA.type, activityDataA.id)
  if nil == dataA then do return false end end
  
  local dataB = event_data.GetDataBySmallId(activityDataB.type, activityDataB.id)
  if nil == dataB then do return false end end  
  
  if activityDataA.state == 2 then  A = A + 10000000 end
  if activityDataB.state == 2 then  B = B + 10000000 end  
  
  A = A + dataA.event_param  
  B = B + dataB.event_param 
  
  return A < B
end

--列表刷新
function t:RefreshScrollContent()
  self.ItemDataList = self:GetListData()
  self.scrollcontent:RefreshAllContentItems()
  self:InitButtons()
end

function t:ActivityUpdate()
  self:RefreshScrollContent()  
end

function t.OnMsgJoinActivityResp()
  t:RefreshScrollContent()
  t:InitButtons()
end

function t.OnMsgGetRewardResp()
  t:RefreshScrollContent()
end

function t:OnItemInit(go, index)
  local itemdata = self:GetItemByIndex(index + 1)
  if nil == itemdata then do return end end
  
   if self.m_aryScrollItem[go] == nil then
      self.m_aryScrollItem[go]= activity_returngift_item.New(go.transform)
      self.m_aryScrollItem[go].DelegateOnClick:RemoveAllListener()
      self.m_aryScrollItem[go].DelegateOnClick:AddListener(function(info) self:OnGetRewardBtnClick(info) end) 
  end
  
  t.m_aryScrollItem[go]:Init(itemdata)
end

function t:GetItemByIndex(index)
  for i = 1, #self.ItemDataList do if i == index then return self.ItemDataList[i] end end
end

function t:OnBtnJoinActivityClick(info)
  activity_ctrl.ReqActivityJoin(self.info.type, self.info.subType)
end

function t:OnGetRewardBtnClick(info)
  activity_ctrl.ReqActivityReward(info.type, info.id, info.subType)
end

function t:OnBtnClose()
  self:Close()
end

function t:Close() 
  self.transform.gameObject:SetActive(false) 
  activity_ctrl.OnDlegategActivityJoinResp:RemoveListener(self.OnMsgJoinActivityResp)
end

function t:Clear()
  self.transform = nil
end

return t