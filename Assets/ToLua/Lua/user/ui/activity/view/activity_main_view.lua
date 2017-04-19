local t = {}
local name='activity_main_view'
local PREFAB_PATH = 'ui/activity/activity_main_view'
require 'ui/activity/view/activity_base_view'

local activity_ctrl = gamemanager.GetCtrl('activity_controller')
local activity_model = gamemanager.GetModel('activity_model')
local event_data = gamemanager.GetData('event_data')

local bottomitem = require('ui/activity/view/activity_bottom_icon')

t.common_top_bar = nil
t.transform = nil
t.coretransform = nil
t.frontform = nil
t.CurView = nil
t.viewList = {}
t.tempRewardInfo = {}
t.m_aryScrollItem = {}

t.DelegateBottomRedPointOnTimeTicker = void_delegate.New()

local function Start( ... )
	uimanager.RegisterView(name,t)
end

function t:Open()
  self.transform = Logic.UI.UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay).transform  
  local bar = dofile ('ui/common_top_bar/common_top_bar')
	self.common_top_bar = bar:Create(self.transform:Find('core'))
  self.common_top_bar:SetAsCommonStyle("", function() self:OnBtnBarBack() end,true,true,true,true,false,false,false)  
  
	self.coretransform = self.transform:FindChild('core')
  self.frontform = self.transform:FindChild('front')
	self.content = self.transform:FindChild('core/bottom/vp/content'):GetComponent(typeof(RectTransform))
	self.item= self.content:FindChild('item')
  
	self.scrollcontent = self.transform:FindChild('core/bottom/vp/content'):GetComponent(typeof(ScrollContent))
  self.scrollcontent.onResetItem:RemoveAllListeners()
	self.scrollcontent.onResetItem:AddListener(function(go, index) t:OnBottomIconsInit(go, index) end)	
  
	self.pagel = self.transform:FindChild('core/bottom/pageleft').gameObject
	self.pager = self.transform:FindChild('core/bottom/pageright').gameObject
  
  self.BindDelegate()
  self:InitActivity(activity_model:GetActivityListExcludeFirstCharge())  
  self:OpenDefaultView()  
  self:OpenTimer()
  
end

function t:OnBottomIconsInit(go, index)
  local active = activity_model:GetActivityExcludeFirstChargeByIndex(index + 1)
  if nil == active then do return end end
  
  if self.m_aryScrollItem[go] == nil then
      self.m_aryScrollItem[go]= bottomitem.New(go.transform)      
      self.m_aryScrollItem[go].onClick:RemoveAllListener()
      self.m_aryScrollItem[go].onClick:AddListener(function(bicon) self:OnBottomIconClick(bicon) end) 
  end
  
  t.m_aryScrollItem[go]:Init(active.type)
  
  self.DelegateBottomRedPointOnTimeTicker:RemoveListener(t.m_aryScrollItem[go].OnTimerTick)
  self.DelegateBottomRedPointOnTimeTicker:AddListener(t.m_aryScrollItem[go].OnTimerTick)
end

function t:OnBottomIconClick(icon)
  if nil == icon then do return end end
  self:OpenViewByType(icon.Type)
end

function t:SetCommomTopBarTitle(title)
  self.common_top_bar:SetTitle(title)  
end

function t:InitActivity(list)
  self.DelegateBottomRedPointOnTimeTicker:RemoveAllListener()
  self.scrollcontent:Init(#list, false, 0)
end

function t.OnMsgActivityList()
  if nil ~= t.CurView then t.CurView:Refresh() end
  t:InitActivity(activity_model:GetActivityListExcludeFirstCharge())
end

function t.OnMsgActivityUpdate()
  if nil ~= t.CurView then t.CurView:ActivityUpdate() end
  t.DelegateBottomRedPointOnTimeTicker:RemoveAllListener()
  t.scrollcontent:RefreshAllContentItems()  
end

function t.OnMsgActivityReward()
  if nil ~= t.CurView then t.CurView:Refresh() end
  
  local itemData = event_data.GetDataBySmallId(t.tempRewardInfo.type, t.tempRewardInfo.id)
  if nil == itemData then do return end end
  
  if nil == itemData.event_award or itemData.event_award == "" then do return end end 
  local list = string.split(itemData.event_award, ";")

  uimanager.ShowTipsAward(list)
  
  t.tempRewardInfo = {}
end

function t:OpenDefaultView()
  local activetyData = activity_model:GetFirstActivity()
  if nil == activetyData then do return end end
  self:OpenViewByType(activetyData.type)
end

function t:OpenViewByType(btype)  
  local activityView = activity_model:GetActivityConfigByType(btype)
  if nil == activityView then do return end end  
  
  if nil ~= self.CurView then self.CurView:Close() end
  local bview = nil
  
  if nil ~= self.viewList[activityView["name"] .. tostring(btype)] then
    bview = self.viewList[activityView["name"] .. tostring(btype)] 
    bview:Open()
  else
    
    bview = dofile(activityView["path"])
    bview:Start(btype)
    bview:Open()
    self.viewList[activityView["name"] .. tostring(btype)] = bview   
  end   
  self.CurView = bview
end

function t.BindDelegate()
  activity_ctrl.OnDlegateActivityListResp:AddListener(t.OnMsgActivityList)
  activity_ctrl.OnDlegateActivityUpdateResp:AddListener(t.OnMsgActivityUpdate)
  activity_ctrl.OnDlegateActivityRewardResp:AddListener(t.OnMsgActivityReward) 
end

function t.UnBindDelegate()
  activity_ctrl.OnDlegateActivityListResp:RemoveListener(t.OnMsgActivityList)
  activity_ctrl.OnDlegateActivityUpdateResp:RemoveListener(t.OnMsgActivityUpdate)
  activity_ctrl.OnDlegateActivityRewardResp:RemoveListener(t.OnMsgActivityReward)
end

function t:OpenTimer()
  activity_model.DelegateOnTimeTicker:AddListener(self.OnTimerTick)
end

function t.OnTimerTick()
  if nil ~= t.CurView then t.CurView:OnTimerTick() end
  t.DelegateBottomRedPointOnTimeTicker:Invoke()
end

function t:CloseTimer()
  activity_model.DelegateOnTimeTicker:RemoveListener(self.OnTimerTick)
end

function t:OnBtnBarBack()
  if nil ~= self.viewList then 
    for k, v in pairs(self.viewList) do
      v:Close()
      v:Clear()
    end 
  end
  
  self.viewList = {}
  
  self:UnBindDelegate() 
  self:CloseTimer()
  UIMgr.instance:Close(PREFAB_PATH)
  uimanager.CloseView(name)
end

function t:RecordCurActivity(type, id)  
  self.tempRewardInfo.type = type
  self.tempRewardInfo.id = id
end

function t.Close() end

Start()
return t