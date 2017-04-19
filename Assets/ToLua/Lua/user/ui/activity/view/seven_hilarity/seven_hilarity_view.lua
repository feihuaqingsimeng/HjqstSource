local t = {}
local name='seven_hilarity_view'
local PREFAB_PATH = 'ui/activity/seven_hilarity_view'

local activity_ctrl = gamemanager.GetCtrl('activity_controller')
local activity_model = gamemanager.GetModel('activity_model')

local seven_hilarity_data = gamemanager.GetData('seven_hilarity_data')

local common_reward_icon = require('ui/common_icon/common_reward_icon')
local game_res_data = require('ui/game/model/game_res_data')

t.transform = nil
t.coretransform = nil
t.common_top_bar = nil

t._ListDayToggles = {}
t._ListPageToggles = {}

t.PagePanel1 = nil
t.PagePanel2 = nil
t.PagePanel3 = nil
t.PagePanel4 = nil
t.pagePanelList = {}

t.CurPageView = nil
t.PageList = {}

t.BoxPanel = nil
t.curSelectedDay = 1 --[1-7]
t.curSelectedPage = 1 --[1-4]
t.tempRewardInfo = {}

local function Start( ... )
	uimanager.RegisterView(name,t)
end

function t:Open()
  local asset = LuaInterface.LuaCsTransfer.UIMgrOpen(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay)	
	self.transform = asset:GetComponent(typeof(Transform))
  self.coretransform = self.transform:FindChild('core')
  
  local bar = dofile ('ui/common_top_bar/common_top_bar')
	self.common_top_bar = bar:Create(self.transform:Find('core'))
  self.common_top_bar:SetAsCommonStyle("", function() self:OnBtnBarBack() end, true, true, true, true, false, false, false) 
  
  self._ListDayToggles = {}
  local _TabDayToggle = self.coretransform:FindChild('toggle_days')
  for i = 1, _TabDayToggle.childCount do
    local toggle = _TabDayToggle:GetChild(i - 1):GetComponent(typeof(Toggle))
    toggle.name = tostring(i)--天数[1-7]    
    toggle.onValueChanged:RemoveAllListeners()
    toggle.onValueChanged:AddListener(function(value) self:OnToggleDayClick(value) end)
    table.insert(self._ListDayToggles, toggle) 
  end
  
  self._ListPageToggles = {}
  local _TabPageToggle = self.coretransform:FindChild('toggle_activity_type')
  for i = 1, _TabPageToggle.childCount do
    local toggle = _TabPageToggle:GetChild(i - 1):GetComponent(typeof(Toggle))
    toggle.name = tostring(i)--页签[1-4]
    toggle.onValueChanged:RemoveAllListeners()
    toggle.onValueChanged:AddListener(function(value) self:OnTogglePageClick(value) end)
    table.insert(self._ListPageToggles, toggle)
  end
  
  local page1Obj = self.coretransform:FindChild('pages/scrollrect_1')
  self.PagePanel1 = dofile("ui/activity/view/seven_hilarity/seven_hilarity_page")
  self.PagePanel1:Start(page1Obj)
  table.insert(self.pagePanelList, self.PagePanel1)
  
  local page2Obj = self.coretransform:FindChild('pages/scrollrect_2')
  self.PagePanel2 = dofile("ui/activity/view/seven_hilarity/seven_hilarity_page")
  self.PagePanel2:Start(page2Obj)
  table.insert(self.pagePanelList, self.PagePanel2)
  
  local page3Obj = self.coretransform:FindChild('pages/scrollrect_3')
  self.PagePanel3 = dofile("ui/activity/view/seven_hilarity/seven_hilarity_page")
  self.PagePanel3:Start(page3Obj)
  table.insert(self.pagePanelList, self.PagePanel3)
  
  local page4Obj = self.coretransform:FindChild('pages/page_discount')
  self.PagePanel4 = dofile("ui/activity/view/seven_hilarity/seven_hilarity_discountPanel")
  self.PagePanel4:Start(page4Obj)
  table.insert(self.pagePanelList, self.PagePanel4)
  
  local boxObj = self.coretransform:FindChild('box')
  self.BoxPanel = dofile("ui/activity/view/seven_hilarity/seven_hilarity_box")
  self.BoxPanel:Start(boxObj)
  
  self:Init()
  gamemanager.GetCtrl('activity_controller').ReqSevenDayInfoResp()  
end

function t:Init()
  self.curSelectedDay = activity_model.SevenHilarityDay
  self.curSelectedPage = seven_hilarity_data:GetDefaultTypeByday(self.curSelectedDay)
  
  self:InitDayToggles()
  self:InitBox()
  self:InitDefaultDay()
  self.BindDelegate() 
  self:SetCommomTopBarTitle(LocalizationController.instance:Get("event_name_1016"))
end

function t:InitDayToggles()
  for i = 1, #self._ListDayToggles do
    local toggle = self._ListDayToggles[i]
    local cDay = tonumber(toggle.name)
    local IsActive = cDay <= activity_model.SevenHilarityDay
    ui_util.SetGrayChildren(toggle.transform, not IsActive, true)
    toggle.enabled = IsActive
  end
end

function t:InitToggleDayRedPoint()
  for i = 1, #self._ListDayToggles do
    local toggle = self._ListDayToggles[i]
    local cDay = tonumber(toggle.name)    
    local IsCanReward = activity_model:IsHaveHilarityCanAwardByDay(cDay) 
    local IsActive = cDay <= activity_model.SevenHilarityDay
    local redPoint = toggle.transform:FindChild('red_point').gameObject
    redPoint:SetActive(IsCanReward and IsActive)
  end
end

function t:InitPageRedPoint()   
  for i = 1, #self._ListPageToggles do
    local toggle = self._ListPageToggles[i]
    local cPage = tonumber(toggle.name)
    local IsCanReward = activity_model:IsSevenHilarityCanAwardByDayAndType(self.curSelectedDay, cPage)
    local redPoint = toggle.transform:FindChild('red_point').gameObject
    redPoint:SetActive(IsCanReward)
  end 
end

function t:InitBox()
  self.BoxPanel:Open()
end

function t:InitDefaultDay()
  for i = 1, #self._ListDayToggles do
    self._ListDayToggles[i].isOn = i == self.curSelectedDay
  end
end

function t:RefeshDay()
  self.PageList = {}
  self:InitPage()
  self:InitPageRedPoint()
end

function t:InitPage() 
  for i = 1, #self._ListPageToggles do    
    self._ListPageToggles[i].isOn = i == self.curSelectedPage
    if self._ListPageToggles[i].isOn and i == self.curSelectedPage then self:RefeshPage() end
  end
end

function t:RefeshPage()
  local bday  = self.curSelectedDay
  local btype = self.curSelectedPage

  if nil ~= self.CurPageView then self.CurPageView:Close() end
  local bpage = nil
  if nil ~= self.PageList[tostring(btype)] then
    bpage = self.PageList[tostring(btype)]
    bpage:Open()
  else
    bpage = self.pagePanelList[btype]
    bpage:Open()
    bpage:Init(bday, btype)    
    self.PageList[tostring(btype)] = bpage
  end

  self.CurPageView = bpage
end

function t:OnToggleDayClick(value) 
  if not value then do return end end

  local cDay = 1
  for i = 1, #self._ListDayToggles do
    local toggle = self._ListDayToggles[i]
    if toggle.isOn then cDay = tonumber(toggle.name) end
  end 

  self.curSelectedDay = cDay
  self:RefeshDay()
end

function t:OnTogglePageClick(value) 
  if not value then do return end end  
  local cPage = 1
  for i = 1, #self._ListPageToggles do
    local toggle = self._ListPageToggles[i]
    if toggle.isOn then cPage = tonumber(toggle.name) end
  end 

  self.curSelectedPage = cPage
  self:RefeshPage()
end

function t:SetCommomTopBarTitle(title)
  self.common_top_bar:SetTitle(title)  
end

function t.BindDelegate()
  activity_ctrl.OnDlegateSevenDayInfoResp:AddListener(t.OnMsgSevenDayInfoList)
  activity_ctrl.OnDlegateBuySevenDayGoodsResp:AddListener(t.OnMsgBuySevenDayGoods)  
  activity_ctrl.OnDlegateGetSevenDayTaskAwardResp:AddListener(t.OnMsgSevenDayTaskAward)
end

function t.UnBindDelegate()
  activity_ctrl.OnDlegateSevenDayInfoResp:RemoveListener(t.OnMsgSevenDayInfoList)
  activity_ctrl.OnDlegateBuySevenDayGoodsResp:RemoveListener(t.OnMsgBuySevenDayGoods)  
  activity_ctrl.OnDlegateGetSevenDayTaskAwardResp:RemoveListener(t.OnMsgSevenDayTaskAward)
end

function t.OnMsgSevenDayTaskAward()
  t.ShowReward()  
end

function t.OnMsgSevenDayInfoList()
    if nil ~= t.CurPageView then t.CurPageView:Refresh() end
    if nil ~= t.BoxPanel then t.BoxPanel.Refresh() end
    
    t:InitToggleDayRedPoint()    
    t:InitPageRedPoint() 
end

function t.OnMsgBuySevenDayGoods()
  t.ShowSevenDayGoodsReward()
end

function t.ShowReward()
  if nil == t.tempRewardInfo.CsvId then do return end end
  local itemData = seven_hilarity_data.GetDataById(t.tempRewardInfo.CsvId)
  if nil == itemData then do return end end
  
  if nil == itemData.reward_data or itemData.reward_data == "" then do return end end 
  local list = string.split(itemData.reward_data, ";")
  
  uimanager.ShowTipsAward(list)
  t.tempRewardInfo = {}
end

function t.ShowSevenDayGoodsReward()
  if nil == t.tempRewardInfo.CsvId then do return end end
  local itemData = seven_hilarity_data.GetDataById(t.tempRewardInfo.CsvId)
  if nil == itemData then do return end end
  
  if nil == itemData.task or itemData.task == "" then do return end end 
  local list = string.split(itemData.task, ";")
  
  uimanager.ShowTipsAward(list)
  t.tempRewardInfo = {}
end

function t:RecordCurActivity(id)  
  self.tempRewardInfo.CsvId = id
end

function t:OnBtnBarBack()
  self.BoxPanel:Close()
  self:UnBindDelegate()
  UIMgr.instance:Close(PREFAB_PATH)
  uimanager.CloseView(name)
  
  activity_model.DelegateClose:Invoke()
end

function t.Close() end
Start()
return t