local t = {}
local PREFAB_PATH = 'ui/tips/confirm_tips_view'

function t.Open(tipsString,clickOkBtnHandler)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.Tips,UIOpenMode.Overlay)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  t.InitComponent()
  
  t.clickOkFunc = clickOkBtnHandler
  --set tip
  t.textTips.text = tipsString
  
end

function t.InitComponent()
  t.textTips = t.transform:Find('core/img_frame/text_tips_description'):GetComponent(typeof(Text))
  t.btnCancel = t.transform:Find('core/img_frame/btn_cancel'):GetComponent(typeof(Button))
  t.btnOk = t.transform:Find('core/img_frame/btn_ok'):GetComponent(typeof(Button))
  
  t.btnCancel.onClick:RemoveAllListeners()
  t.btnOk.onClick:RemoveAllListeners()
  t.btnOk.onClick:AddListener(t.ClickOkBtnHandler)
  
  t.btnCancel.onClick:AddListener(t.ClickCancelHandler)
  t.toggleTip = t.transform:Find('core/img_frame/Toggle_tip'):GetComponent(typeof(Toggle))
  t.toggleTip.gameObject:SetActive(false)
  
end

function t.ClickOkBtnHandler()
  
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  if t.clickOkFunc then
    t.clickOkFunc()
  end
end

function t.ClickCancelHandler()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end

return t