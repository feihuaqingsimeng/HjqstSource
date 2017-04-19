local t={}
t.__index = t
local name='seven_hilarity_item'

local activity_model = gamemanager.GetModel('activity_model')
local seven_hilarity_data = gamemanager.GetData('seven_hilarity_data')
local task_data = gamemanager.GetData('task_data')
local task_condition_data = gamemanager.GetData('task_condition_data')


local common_reward_icon = require('ui/common_icon/common_reward_icon')
local game_res_data = require('ui/game/model/game_res_data')
local auto_destroy_tips_view = require('ui/tips/view/auto_destroy_tip_view')

t._TextDesc = nil
t._BtnAward = nil
t._BtnAwardText = nil
t._ConditionText = nil
t._AwardRoot = nil

t.transform = nil
t.info = nil
t.CsvId = 0

t.DelegateOnClick = void_delegate.New()

function t.New(transform)
    local o = {}
    setmetatable(o,t)
    
 if nil == o.transform then
		o.transform = transform    
    o._TextDesc = o.transform:FindChild('descText'):GetComponent(typeof(Text))
    o._AwardRoot = o.transform:FindChild('horizontal')
    o._BtnAward = o.transform:FindChild('Button'):GetComponent(typeof(Button))
    o._BtnAwardText = o.transform:FindChild('Button/Text'):GetComponent(typeof(Text))
    o._ConditionText = o.transform:FindChild('text_condition'):GetComponent(typeof(Text))
  end
   return o
end

function t:Init(info)
  self.info = info
  
  local seven_hilarity_data = seven_hilarity_data.GetDataByTask(self.info.id)
  if nil == seven_hilarity_data then do return end end 
  
  self.CsvId = seven_hilarity_data.id
  
  local taskData = task_data.GetDataById(self.info.id)
  if nil == taskData then do return end end
  
  local taskConditionData = task_condition_data.GetDataById(self.info.id)
  if nil == taskConditionData then do return end end

  self._TextDesc.text = LocalizationController.instance:Get(taskData.des)
  
  if taskData.count == 0 then 
    if self.info.completed then self._ConditionText.text = "1/1"
    else self._ConditionText.text = "0/1" end 
  elseif taskData.count == 1 then 
    if self.info.completed then self._ConditionText.text = string.format("%d/%d", taskConditionData.parameter1, taskConditionData.parameter1)
    else self._ConditionText.text = string.format("%d/%d", self.info.taskConditionValue, taskConditionData.parameter1) end
  elseif taskData.count == 2 then  
    if self.info.completed then self._ConditionText.text = string.format("%d/%d", taskConditionData.parameter1, taskConditionData.parameter2)
    else self._ConditionText.text = string.format("%d/%d", self.info.taskConditionValue, taskConditionData.parameter2) end
  end
  self:InitAwards()
  self:InitButtons()
  
  self.transform.gameObject:SetActive(true)
end

function t:InitAwards()  
  local seven_hilarity_data = seven_hilarity_data.GetDataByTask(self.info.id)
  if nil == seven_hilarity_data then do return end end 

  ui_util.ClearChildren(self._AwardRoot, true)  
  local awards = string.split(seven_hilarity_data.reward_data, ';')
  for i = 1, #awards do    
    local bdata = game_res_data.NewByString(awards[i])   
    common_reward_icon.New(self._AwardRoot, bdata)
  end
end

function t:InitButtons()  
  self._BtnAward.onClick:RemoveAllListeners()
  if not self.info.InActivity then     	
      self._BtnAward.onClick:AddListener(function()	self:OnBtnOverTime() end)
      self._BtnAwardText.text = LocalizationController.instance:Get('event_public_OverTime2')
      ui_util.SetGrayChildren(self._BtnAward.transform, true, true)
  else
    if not self.info.completed then 
      self._BtnAwardText.text = LocalizationController.instance:Get('event_1008_btn3')
      ui_util.SetGrayChildren(self._BtnAward.transform, true, true)
    else
      if not self.info.getReward then 
        self._BtnAward.onClick:AddListener(function()	self:OnBtnGetAward() end)
        self._BtnAwardText.text = LocalizationController.instance:Get('event_1008_btn1')
        ui_util.SetGrayChildren(self._BtnAward.transform, false, true)
    else
      self._BtnAwardText.text = LocalizationController.instance:Get('event_1008_btn2')
      ui_util.SetGrayChildren(self._BtnAward.transform, true, true)
      end
    end
  end
  
  self._BtnAward.gameObject:SetActive(true)
end

function t:OnBtnOverTime()
  auto_destroy_tips_view.Open(LocalizationController.instance:Get('event_public_OverTime1'))
end

function t:OnBtnGetAward()
  self.DelegateOnClick:InvokeOneParam(self.CsvId)
end

return t