local t={}
t.__index = t
local name='activity_levelup_item'

local activity_model = gamemanager.GetModel('activity_model')
local event_data = gamemanager.GetData('event_data')

local common_reward_icon = require('ui/common_icon/common_reward_icon')
local game_res_data = require('ui/game/model/game_res_data')
local auto_destroy_tips_view = require('ui/tips/view/auto_destroy_tip_view')

t._TextlevelValue = nil
t._BtnAward = nil
t._BtnAwardText = nil
t._AwardRoot = nil

t.transform = nil
t.info = nil

t.DelegateOnClick = void_delegate.New()

function t.New(transform)
    local o = {}
    setmetatable(o,t)
    
 if nil == o.transform then 
		o.transform = transform    
    o._TextlevelValue = o.transform:FindChild('descText'):GetComponent(typeof(Text))
    o._AwardRoot = o.transform:FindChild('horizontal')
    o._BtnAward = o.transform:FindChild('Button'):GetComponent(typeof(Button))
    o._BtnAwardText = o.transform:FindChild('Button/Text'):GetComponent(typeof(Text))
  end
   return o
end

function t:Init(info)
  self.info = info 
  local itemData = event_data.GetDataBySmallId(self.info.type, self.info.id)
  if nil == itemData then do return end end

  self._TextlevelValue.text = string.format(LocalizationController.instance:Get("event_1008_LevelValue"),
    self.info.conditionValue, tonumber(itemData.event_param))
    self:InitAwards()
  self:InitButtons()
  
  self.transform.gameObject:SetActive(true)
end

function t:InitAwards()
  local eventData = event_data.GetDataBySmallId(self.info.type, self.info.id)
  if nil == eventData then do return end end 
  
  ui_util.ClearChildren(self._AwardRoot, true)  
  local awards = string.split(eventData.event_award, ';')
  for i = 1, #awards do    
    local bdata = game_res_data.NewByString(awards[i])   
    common_reward_icon.New(self._AwardRoot, bdata)
  end
end

function t:InitButtons()
  local state = self.info.state
  self._BtnAward.onClick:RemoveAllListeners()
  if state == 0 then     	
      self._BtnAward.onClick:AddListener(function()	self:OnBtnOverTime() end)
      self._BtnAwardText.text = LocalizationController.instance:Get('event_public_OverTime2')
      ui_util.SetGrayChildren(self._BtnAward.transform, true, true)      
  elseif state == 1 then 
      self._BtnAward.onClick:AddListener(function()	self:OnBtnGetAward() end)
      self._BtnAwardText.text = LocalizationController.instance:Get('event_1008_btn1')
      ui_util.SetGrayChildren(self._BtnAward.transform, false, true)
  elseif state == 2 then 
      self._BtnAwardText.text = LocalizationController.instance:Get('event_1008_btn2')
      ui_util.SetGrayChildren(self._BtnAward.transform, true, true)
  elseif state == 3 then 
      self._BtnAwardText.text = LocalizationController.instance:Get('event_1008_btn3')
      ui_util.SetGrayChildren(self._BtnAward.transform, true, true)
  end
  
  self._BtnAward.gameObject:SetActive(true)
end

function t:OnBtnOverTime()
  auto_destroy_tips_view.Open(LocalizationController.instance:Get('event_public_OverTime1'))
end

function t:OnBtnGetAward()
  self.DelegateOnClick:InvokeOneParam(self.info) 
end

return t