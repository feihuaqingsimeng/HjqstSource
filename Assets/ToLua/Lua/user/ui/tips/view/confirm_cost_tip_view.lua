local t = {}
local PREFAB_PATH = "ui/tips/confirm_cost_tips_view_lua"

local game_model = gamemanager.GetModel('game_model')
local global_data = gamemanager.GetData('global_data')
local consume_tip_model = gamemanager.GetModel('consume_tip_model')

--
function t.OpenByThreeParam(baseResType,cost,okCallback)
  t.Open(baseResType,cost,LocalizationController.instance:Get('ui.confirm_cost_tip_view.cost_des'),okCallback,ConsumeTipType.None)
end

function t.Open(baseResType,cost,desString,okCallback,consumeTipType)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.Tips, UIOpenMode.Overlay)
  t.transform = gameObject.transform
  t.baseResType = baseResType
  t.cost= cost
  t.okCallback = okCallback
  t.consumeTipType = consumeTipType
  
  local bg = t.transform:Find('core/common_tips_root/img_background')
  t.textCost = bg:Find('text_cost'):GetComponent(typeof(Text))
  t.textDes = bg:Find('text_content'):GetComponent(typeof(Text))
  t.toggleTip = bg:Find('Toggle_tip'):GetComponent(typeof(Toggle))
  
  
  t.btnCancel = bg:Find('btn_cancel'):GetComponent(typeof(Button))
  t.btnCancel.onClick:AddListener(t.Close)
  t.btnOk = bg:Find('btn_ok'):GetComponent(typeof(Button))
  t.btnOk.onClick:AddListener(t.ClickOkHandler)
  
  if game_model.GetBaseResourceValue(BaseResType.Diamond) < t.cost then
    t.textCost.text = ui_util.FormatToRedText(cost)
  else
    t.textCost.text = cost
  end
  t.textDes.text = desString
  
  if not consume_tip_model.HasConsumeTipKey(consumeTipType) then
    t.toggleTip.gameObject:SetActive(false)
  end
end

function t.Close()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end
function t.ClickOkHandler()
  consume_tip_model.SetConsumeTipEnable(t.consumeTipType, not t.toggleTip.isOn);
  if t.okCallback then
    t.okCallback()
  end
  t.Close()
end

return t