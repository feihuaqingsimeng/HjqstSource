local t = {}
local PREFAB_PATH = 'ui/tips/common_error_tips_view'

function t.Open(tipsString)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.Tips,UIOpenMode.Overlay)
  t.transform = gameObject:GetComponent(typeof(Transform))
  t.textTips = t.transform:Find('core/img_frame/text_tips'):GetComponent(typeof(Text))
  t.btnOk = t.transform:Find('core/img_frame/btn_ok'):GetComponent(typeof(Button))
  t.btnOk.onClick:AddListener(t.ClickOkBtnHandler)
  
  --set tip
  t.textTips.text = tipsString
end

function t.ClickOkBtnHandler()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end

return t