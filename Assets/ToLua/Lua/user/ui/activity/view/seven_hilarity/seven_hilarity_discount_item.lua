local t={}
local name='seven_hilarity_discount_item'

local activity_model = gamemanager.GetModel('activity_model')
local activity_ctrl = gamemanager.GetCtrl('activity_controller')

local seven_hilarity_data = gamemanager.GetData('seven_hilarity_data')

local common_reward_icon = require('ui/common_icon/common_reward_icon')
local game_res_data = require('ui/game/model/game_res_data')
local auto_destroy_tips_view = require('ui/tips/view/auto_destroy_tip_view')

t._TextValue1 = nil
t._TextValue2 = nil
t._BtnAward = nil
t._BtnAwardText = nil
t._AwardRoot = nil

t.transform = nil
t.day = 1
t.type = 1
t.sevenHilarityData = nil

t.DelegateOnClick = void_delegate.New()

function t:Start(transform)
  if nil == self.transform then 
		self.transform = transform    
    self._TextValue1 = self.transform:FindChild('text_price1'):GetComponent(typeof(Text))
    self._TextValue2 = self.transform:FindChild('text_price2'):GetComponent(typeof(Text))
    self._AwardRoot = self.transform:FindChild('horizontal')
    self._BtnAward = self.transform:FindChild('Button'):GetComponent(typeof(Button))
    self._BtnAwardText = self.transform:FindChild('Button/Text'):GetComponent(typeof(Text))
  end
end

function t:Init(bday, btype, bindex)
  local itemList = seven_hilarity_data.GetDataByDayAndType(bday, btype)
  self.sevenHilarityData = itemList[bindex]
  self.day = bday
  self.type = btype
  
  if nil == self.sevenHilarityData then do return end end
  local StrPrices = string.split(self.sevenHilarityData.reward_data, ';')
  local strPrice1 = string.split(StrPrices[1], ":")
  local strPrice2 = string.split(StrPrices[2], ":")

  self._TextValue1.text = string.format(LocalizationController.instance:Get("event_1012_yuanjia"), strPrice1[3])
  self._TextValue2.text = string.format(LocalizationController.instance:Get("event_1012_xianjia"), strPrice2[3])
  
  self:InitAwards()
  self:InitButtons()
  
  self.transform.gameObject:SetActive(true)
end

function t:Refresh()
  self:InitButtons()
end

function t:InitAwards()
  ui_util.ClearChildren(self._AwardRoot, true)  
  local awards = string.split(self.sevenHilarityData.task, ';')
  for i = 1, #awards do    
    local bdata = game_res_data.NewByString(awards[i])   
    common_reward_icon.New(self._AwardRoot, bdata)
  end
end

function t:InitButtons()
  local IsBuy = activity_model:IsBuyGoods(self.sevenHilarityData.id)

  self._BtnAward.onClick:RemoveAllListeners()
  if not IsBuy then 
      self._BtnAward.onClick:AddListener(function()	self:OnBtnGetAward() end)
      self._BtnAwardText.text = LocalizationController.instance:Get('event_1007_buy_btn1')
      ui_util.SetGrayChildren(self._BtnAward.transform, false, true)
  else
      self._BtnAwardText.text = LocalizationController.instance:Get('event_1007_buy_btn2')
      ui_util.SetGrayChildren(self._BtnAward.transform, true, true)
  end  
  self._BtnAward.gameObject:SetActive(true)
end

function t:OnBtnGetAward()
   activity_ctrl.ReqBuySevenDayGoodsResp(self.day, self.sevenHilarityData.id, self.sevenHilarityData.id)
end

return t