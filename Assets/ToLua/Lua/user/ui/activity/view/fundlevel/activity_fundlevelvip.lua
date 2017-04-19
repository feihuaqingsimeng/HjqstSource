local t={}
local name='activity_levelup_item'

local activity_ctrl = gamemanager.GetCtrl('activity_controller')
local activity_model = gamemanager.GetModel('activity_model')
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

t._BtnAward = nil
t._BtnAwardText = nil
t._AwardRoot = nil
t._TextFundValue = nil

t.transform = nil
t.curShowVip = 0
t.DelegateOnClick = void_delegate.New()
t.CurRewardList = {}
t.type = 0

function t:Start(transform)
  if nil == self.transform then 
		self.transform = transform    
    self._TextEXPValue = self.transform:FindChild('exp/Text'):GetComponent(typeof(Text))
    self._TextVipValue = self.transform:FindChild('lv/Text'):GetComponent(typeof(Text))
    self._TextTitle = self.transform:FindChild('Text'):GetComponent(typeof(Text))
    self._SliderExp =   self.transform:FindChild('exp'):GetComponent(typeof(Slider))
    self._TextFundValue = self.transform:FindChild('text Price'):GetComponent(typeof(Text))
    
    self._AwardRoot = self.transform:FindChild('grid')
    self._BtnAward = self.transform:FindChild('Button'):GetComponent(typeof(Button))
    self._BtnAwardText = self.transform:FindChild('Button/Text'):GetComponent(typeof(Text))
  end
end

function t:Init(type)
  self.type = type
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
  
  self:InitVip()
end

function t:Open()
  vipModel.OnVIPInfoUpdateDelegate:AddListener(self.OnMsgVipInfoRefresh)  
  activity_ctrl.OnDlegategActivityJoinResp:AddListener(self.OnMsgJoinActivityResp)
end

--VIP
function t:InitVip()
  local bVIPData = vipData.GetVIPData (self.curShowVip)
  if nil == bVIPData then do return end end
  
  self._TextTitle.text = string.format(LocalizationController.instance:Get("event_1011_VIPValue"), vipModel.GetFundLevelGift())
  self:InitPrice()
  self:InitAwardBtn()
end

function t:InitPrice()
  local data = event_data.GetDataByType(self.type)
  if nil == data then do return end end
  
  local strPrice = string.split(data.event_buymoney, ":")
  self._TextFundValue.text = strPrice[3]
end

function t:InitAwardBtn()
  local IsCanGetVip = vipModel.vipLevel >= vipModel.GetFundLevelGift()
  local IsBuyVip = activity_model:IsActivityjoin(self.type)
  
  self._BtnAward.onClick:RemoveAllListeners()
  if IsCanGetVip then    
    if IsBuyVip then
      self._BtnAwardText.text = LocalizationController.instance:Get('event_1011_VIPBtn3')
      ui_util.SetGrayChildren(self._BtnAward.transform, true, true)
    else
      self._BtnAward.onClick:AddListener(function()	self:OnBtnJoinActivityClick() end)
      self._BtnAwardText.text = LocalizationController.instance:Get('event_1011_VIPBtn2')
    end
  else    
     self._BtnAward.onClick:AddListener(function()	self:OnBtnNavagation() end)
     self._BtnAwardText.text = LocalizationController.instance:Get('event_1011_VIPBtn1')
     ui_util.SetGrayChildren(self._BtnAward.transform, false, true)
  end
end

function t:OnBtnJoinActivityClick()
  local data = event_data.GetDataByType(self.type)
  if nil == data then do return end end
  
  activity_ctrl.ReqActivityJoin(data.event_type, tonumber(data.event_small_type))
end

function t:OnBtnNavagation()
  gamemanager.GetCtrl('shop_controller').OpenShopView(3)
end

function t.OnMsgJoinActivityResp()
  t:InitAwardBtn()
end

function t.OnMsgVipInfoRefresh() 
  t:Init(t.type)
end

function t:Close()
  vipModel.OnVIPInfoUpdateDelegate:RemoveListener(self.OnMsgVipInfoRefresh)  
  activity_ctrl.OnDlegategActivityJoinResp:RemoveListener(self.OnMsgJoinActivityResp)
end

return t