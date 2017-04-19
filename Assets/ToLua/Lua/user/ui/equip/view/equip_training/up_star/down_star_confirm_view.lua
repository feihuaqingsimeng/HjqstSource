local t = {}
local PREFAB_PATH = 'ui/equipments/up_star/down_star_confirm_view'

function t.Open(cost,tipString,okCallback)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  t.okCallback = okCallback
  
  t.textTip = t.transform:Find('core/frame/text_content'):GetComponent(typeof(Text))
  t.textCost = t.transform:Find('core/frame/cost/text_cost'):GetComponent(typeof(Text))
  t.btnCancel = t.transform:Find('core/frame/btn_cancel'):GetComponent(typeof(Button))
  t.btnCancel.onClick:AddListener(t.Close)
  t.btnOk = t.transform:Find('core/frame/btn_ok'):GetComponent(typeof(Button))
  t.btnOk.onClick:AddListener(t.ClickOkHanler)
  
  t.textTip.text = tipString
  local own = gamemanager.GetModel('game_model').GetBaseResourceValue(BaseResType.Diamond)
  if own >= cost then
    t.textCost.text = tostring(cost)
  else
    t.textCost.text = ui_util.FormatToRedText(tostring(cost))
  end
  
  
end

function t.Close()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end
function t.ClickOkHanler()
  t.Close()
  if t.okCallback ~= nil then
    t.okCallback()
  end
end

return t