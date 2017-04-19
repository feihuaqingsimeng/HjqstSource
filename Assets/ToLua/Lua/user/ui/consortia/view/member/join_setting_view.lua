local t = {}
local PREFAB_PATH = 'ui/consortia/member/join_setting_view'
local name = PREFAB_PATH

local consortia_model = gamemanager.GetModel('consortia_model')
local consortia_controller = gamemanager.GetCtrl('consortia_controller')
local auto_destroy_tip_view = require('ui/tips/view/auto_destroy_tip_view')

function t.Open()
  local gameObject = UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  t.InitComponent()
  t.BindDelegate()
  consortia_controller.GuildAutoPassInfoReq()
end
function t.Close()
  UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
end
function t.BindDelegate()
  consortia_controller.GuildAutoPassDelegate:AddListener(t.GuildAutoPassByProtocol)
end

function t.UnbindDelegate()
  consortia_controller.GuildAutoPassDelegate:RemoveListener(t.GuildAutoPassByProtocol)
end

function t.InitComponent()
  local frame = t.transform:Find('core/frame')
  t.inputPower = frame:Find('power/InputField'):GetComponent(typeof(InputField))
  t.inputPower.onValueChange:AddListener(t.InputTextPowerChangeHandler)
  t.togglePowerNoLimit = frame:Find('power/Toggle'):GetComponent(typeof(Toggle))
  t.togglePowerNoLimit.transform:GetComponent(typeof(EventTriggerDelegate)).onClick:AddListener(t.ClickTogglePowerNoLimit)
  t.inputLevel = frame:Find('level/InputField'):GetComponent(typeof(InputField))
  t.inputLevel.onValueChange:AddListener(t.InputTextLevelChangeHandler)
  t.toggleLevelNoLimit = frame:Find('level/Toggle'):GetComponent(typeof(Toggle))
  t.toggleLevelNoLimit.transform:GetComponent(typeof(EventTriggerDelegate)).onClick:AddListener(t.ClickToggleLevelNoLimit)
  t.btnClose = frame:Find('btn_close'):GetComponent(typeof(Button))
  t.btnClose.onClick:AddListener(t.ClickCloseHandler)
  t.btnOpen = frame:Find('btn_open'):GetComponent(typeof(Button))
  t.btnOpen.onClick:AddListener(t.ClickOpenHandler)
  t.textOpen = t.btnOpen.transform:Find('Text'):GetComponent(typeof(Text))
end

function t.Refresh()
  local consortiaInfo = consortia_model.consortiaInfo
  t.inputPower.text = consortiaInfo.limitPower
  t.togglePowerNoLimit.isOn = consortiaInfo.limitPower <= 0
  if consortiaInfo.limitPower <= 0 then
    t.inputPower.textComponent.color = Color.gray
  else
    t.inputPower.textComponent.color = Color(3/255,176/255,240/255)
  end
  t.inputLevel.text = consortiaInfo.limitLevel
  t.toggleLevelNoLimit.isOn = consortiaInfo.limitLevel <= 0
  if consortiaInfo.limitLevel <= 0 then
    t.inputLevel.textComponent.color = Color.gray
  else
    t.inputLevel.textComponent.color = Color(3/255,176/255,240/255)
  end
  
  
  if consortiaInfo.limitIsOpen then
    t.textOpen.text = LocalizationController.instance:Get('ui.consortia_view.member.limitCloseTip')
  else
    t.textOpen.text = LocalizationController.instance:Get('ui.consortia_view.member.limitOpenTip')
  end
end
--关闭界面
function t.ClickCloseHandler()
  t.Close()
end
--打开或关闭设置
function t.ClickOpenHandler()
  local consortiaInfo = consortia_model.consortiaInfo
  consortiaInfo.limitIsOpen = not consortiaInfo.limitIsOpen
  local power = tonumber(t.inputPower.text)
  if not power then
    power = 0
  end
  local level = tonumber(t.inputLevel.text)
  if not level then
    level = 0
  end
  consortiaInfo.limitPower = power
  consortiaInfo.limitLevel = level
  t.Refresh()
  
  if consortiaInfo.limitIsOpen then
    auto_destroy_tip_view.Open(LocalizationController.instance:Get('ui.consortia_view.member.limitOpenSuc'))
  else
    auto_destroy_tip_view.Open(LocalizationController.instance:Get('ui.consortia_view.member.limitCloseSuc'))
  end
  consortia_controller.GuildAutoPassReq(consortiaInfo.limitIsOpen,consortiaInfo.limitPower,consortiaInfo.limitLevel)
end
--toggle 战力
function t.ClickTogglePowerNoLimit(value)
  if t.togglePowerNoLimit.isOn then
    consortia_model.consortiaInfo.limitPower = 0
  end
  consortia_model.consortiaInfo.limitIsOpen = false
  t.Refresh()
end
--toggle level
function t.ClickToggleLevelNoLimit(value)
  if t.toggleLevelNoLimit.isOn then
    consortia_model.consortiaInfo.limitLevel = 0
  end
  consortia_model.consortiaInfo.limitIsOpen = false
  t.Refresh()
end
-- 输入 战力
function t.InputTextPowerChangeHandler(str)
  
  str = string.sub(str,0,9)
  local num = tonumber(str)
  if not num then
    return
  end
  if consortia_model.consortiaInfo.limitPower ~= num then
    consortia_model.consortiaInfo.limitIsOpen = false
    consortia_model.consortiaInfo.limitPower = num
    t.Refresh()
  end
end
--输入 等级
function t.InputTextLevelChangeHandler(str)
  str = string.sub(str,0,9)
  local num = tonumber(str)
  if not num then
    return
  end
  if consortia_model.consortiaInfo.limitLevel ~= num then
    consortia_model.consortiaInfo.limitIsOpen = false
    consortia_model.consortiaInfo.limitLevel = num
    t.Refresh()
  end
  
end
-----------------------update by protocol----------------
function t.GuildAutoPassByProtocol()
  t.Refresh()
end
return t