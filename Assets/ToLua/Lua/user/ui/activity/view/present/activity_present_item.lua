local t={}
t.__index = t
local name='activity_present_item'

local activity_model = gamemanager.GetModel('activity_model')
local event_data = gamemanager.GetData('event_data')
local auto_destroy_tips_view = require('ui/tips/view/auto_destroy_tip_view')

t._TextValue = nil
t._BtnAward = nil
t._BtnAwardText = nil

t.transform = nil
t.info = nil

t.DelegateOnClick = void_delegate.New()

function t.New(transform)
    local o = {}
    setmetatable(o,t)
    
 if nil == o.transform then 
		o.transform = transform    
    o._TextValue = o.transform:FindChild('titletime'):GetComponent(typeof(Text))
    o._BtnAward = o.transform:FindChild('Button'):GetComponent(typeof(Button))
    o._BtnAwardText = o.transform:FindChild('Button/Text'):GetComponent(typeof(Text))
    o.transform.gameObject:SetActive(false)
  end
   return o
end

function t:Init(info)
  self.info = info 
  local itemData = event_data.GetDataBySmallId(self.info.type, self.info.id)
  if nil == itemData then do return end end

  local timeDurationHour = tonumber(gamemanager.GetData('global_data').daily_key_time_keep)/60/60
  
  local strFromTime = itemData.event_param .. ":00"
  local strToTime = tostring(tonumber(itemData.event_param) + timeDurationHour) .. ":00"
  
  self._TextValue.text = strFromTime .. "-"..strToTime  
  self:InitButtons()  
  self.transform.gameObject:SetActive(true)
end

function t:InitButtons()
  local state = self.info.state
  
  local itemData = event_data.GetDataBySmallId(self.info.type, self.info.id)
  if nil == itemData then do return end end  

  local IsPresentInTimeofDay = activity_model.IsPresentInTimeofDay(tonumber(itemData.event_param))
  
  self._BtnAward.onClick:RemoveAllListeners()
  
  if state == 0 then     	
      self._BtnAward.onClick:AddListener(function()	self:OnBtnOverTime() end)
      self._BtnAwardText.text = LocalizationController.instance:Get('event_public_OverTime2')
      ui_util.SetGrayChildren(self._BtnAward.transform, true, true)      
  elseif state == 1 then
      if IsPresentInTimeofDay then
        self._BtnAward.onClick:AddListener(function()	self:OnBtnGetAward() end)
        self._BtnAwardText.text = LocalizationController.instance:Get('event_1008_btn1')
        ui_util.SetGrayChildren(self._BtnAward.transform, false, true)
      else
        self._BtnAwardText.text = LocalizationController.instance:Get('event_1008_btn3')
        ui_util.SetGrayChildren(self._BtnAward.transform, true, true)
      end
  elseif state == 2 then 
      self._BtnAwardText.text = LocalizationController.instance:Get('event_1008_btn2')
      ui_util.SetGrayChildren(self._BtnAward.transform, true, true)
  elseif state == 3 then 
       if IsPresentInTimeofDay then
          self._BtnAward.onClick:AddListener(function()	self:OnBtnGetAward() end)
          self._BtnAwardText.text = LocalizationController.instance:Get('event_1008_btn1')
          ui_util.SetGrayChildren(self._BtnAward.transform, false, true)
      else
          self._BtnAwardText.text = LocalizationController.instance:Get('event_1008_btn3')
          ui_util.SetGrayChildren(self._BtnAward.transform, true, true)
      end
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