local t={}
local name='activity_levelup_item'

local activity_ctrl = gamemanager.GetCtrl('activity_controller')
local event_data = gamemanager.GetData('event_data')

local common_reward_icon = require('ui/common_icon/common_reward_icon')
local game_res_data = require('ui/game/model/game_res_data')
local error_tip_view = require('ui/tips/view/common_error_tips_view')

local vipData = gamemanager.GetData('vip_data')
local vipModel = gamemanager.GetModel('vip_model')

t._TextEXPValue = nil
t._TextVipValue = nil
t._TextTitle = nil
t._SliderExp = nil
t._BtnLeft = nil
t._BtnRight = nil

t._BtnAward = nil
t._BtnAwardText = nil
t._AwardRoot = nil

t.transform = nil
t.curShowVip = 0
t.DelegateOnClick = void_delegate.New()
t.CurRewardList = {}

function t:Start(transform)
  if nil == self.transform then 
		self.transform = transform    
    self._TextEXPValue = self.transform:FindChild('exp/Text'):GetComponent(typeof(Text))
    self._TextVipValue = self.transform:FindChild('lv/Text'):GetComponent(typeof(Text))
    self._TextTitle = self.transform:FindChild('Text'):GetComponent(typeof(Text))
    self._SliderExp =   self.transform:FindChild('exp'):GetComponent(typeof(Slider))
    
    self._BtnLeft = self.transform:FindChild('pagel'):GetComponent(typeof(Button))
    self._BtnLeft.onClick:RemoveAllListeners()
    self._BtnLeft.onClick:AddListener(function()	self:OnBtnShowLeft() end)
    
    self._BtnRight = self.transform:FindChild('pager'):GetComponent(typeof(Button))
    self._BtnRight.onClick:RemoveAllListeners()
    self._BtnRight.onClick:AddListener(function()	self:OnBtnShowRight() end)
    
    self._AwardRoot = self.transform:FindChild('grid')
    self._BtnAward = self.transform:FindChild('Button'):GetComponent(typeof(Button))
    self._BtnAwardText = self.transform:FindChild('Button/Text'):GetComponent(typeof(Text))
  end
end

function t:Init()
  self.curShowVip = vipModel.vipLevel
  self._TextVipValue.text = vipModel.vipLevel 
  
  local curVIPData = vipData.GetVIPData (vipModel.vipLevel)
  if nil == curVIPData then do return end end
  
  if not curVIPData:IsMaxLevel() then 
    local nextVipData = curVIPData:GetNextLevelVIPData()
    if nil == nextVipData then do return end end
    
    t._TextEXPValue.text = string.format("%s/%s", vipModel.totalRecharge * 10, nextVipData.exp * 10)
    t._SliderExp.value = vipModel.totalRecharge / nextVipData.exp    
    t._SliderExp.gameObject:SetActive(true)
  else
    t._TextEXPValue.text = vipModel.totalRecharge * 10    
    t._SliderExp.gameObject:SetActive(false)
  end
  
  self:InitCurShowVip()
end

function t:Open()
  vipModel.OnVIPInfoUpdateDelegate:AddListener(self.OnMsgVipInfoRefresh)
  activity_ctrl.OnDlegateActivityVipDailyGifReceive:AddListener(self.OnMsgVipDailyGifReceive)
  self:Init()
end

--初始化当前展示的VIP
function t:InitCurShowVip()
  local bVIPData = vipData.GetVIPData (self.curShowVip)
  if nil == bVIPData then do return end end
  
  self._TextTitle.text = string.format(LocalizationController.instance:Get("event_1007_title"), self.curShowVip)
  self:InitAwards()
  self:InitAwardBtn()
  
  local IsMaxLevel = bVIPData:IsMaxLevel()
  local IsMinLevel = bVIPData:IsMinLevel()
  
  self._BtnLeft.gameObject:SetActive(not IsMinLevel)
  self._BtnRight.gameObject:SetActive(not IsMaxLevel)
end

function t:InitAwards()
  local bVIPData = vipData.GetVIPData (self.curShowVip)
  if nil == bVIPData then do return end end 
  
  ui_util.ClearChildren(self._AwardRoot, true)  
  local awards = string.split(bVIPData.daily_welfare, ';')
  
  for i = 1, #awards do 
    local bdata = game_res_data.NewByString(awards[i])   
    common_reward_icon.New(self._AwardRoot, bdata)
  end
end

function t:OnBtnShowLeft()  
  if self.curShowVip > vipData.minLevel then self.curShowVip = self.curShowVip - 1 end
  self:InitCurShowVip()
end

function t:OnBtnShowRight()
  if self.curShowVip < vipData.maxLevel then self.curShowVip = self.curShowVip + 1 end
  self:InitCurShowVip()
end

function t:InitAwardBtn()
  local IsCurPlayerVipLevel = self.curShowVip == vipModel.vipLevel
  local IsHaveGetDailyGift = vipModel.IsVipDailyGiftGetted()
  
  local IsCanGetVip = IsCurPlayerVipLevel and not IsHaveGetDailyGift  
  if IsCanGetVip then
    self._BtnAward.onClick:RemoveAllListeners()
    self._BtnAward.onClick:AddListener(function()	self:OnBtnGetAward() end)
    self._BtnAwardText.text = LocalizationController.instance:Get('event_public_btn1')
    ui_util.SetGrayChildren(self._BtnAward.transform, false, true)
  else
     self._BtnAward.onClick:RemoveAllListeners()
     self._BtnAwardText.text = LocalizationController.instance:Get('event_public_btn2')
     ui_util.SetGrayChildren(self._BtnAward.transform, true, true)
  end
  
  self._BtnAward.gameObject:SetActive(IsCurPlayerVipLevel)
end

function t:OnBtnGetAward()
  local bVIPData = vipData.GetVIPData (self.curShowVip)
  if nil == bVIPData then do return end end 
  
  self.CurRewardList = string.split(bVIPData.daily_welfare, ';')
  activity_ctrl.ReqActivityVipDailyGift()
end

function t.OnMsgVipInfoRefresh()
  t:Init()
end

function t.OnMsgVipDailyGifReceive()
  uimanager.ShowTipsAward(t.CurRewardList)
  t.CurRewardList = {}
  
  t:InitAwardBtn()
end

function t:Close()  
   vipModel.OnVIPInfoUpdateDelegate:RemoveListener(self.OnMsgVipInfoRefresh)
   activity_ctrl.OnDlegateActivityVipDailyGifReceive:RemoveListener(self.OnMsgVipDailyGifReceive)
end

return t