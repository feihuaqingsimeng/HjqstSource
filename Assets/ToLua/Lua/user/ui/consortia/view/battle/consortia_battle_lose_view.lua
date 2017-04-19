local t = {}
local PREFAB_PATH = 'ui/consortia/battle/consortia_battle_lose_view'

function t.Open ()
  local gameObject = UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay)
  t.transform = gameObject.transform
  
  t.InitComponent ()
  t.BindDelegate ()
end

function t.InitComponent ()
  t.loseTipText = t.transform:Find('core/frame/text_lose_tip'):GetComponent(typeof(Text))
  t.loseTipText.text = LocalizationController.instance:Get('ui.consortia_view.battle.lose_view.lose_tips')
  t.drawButton = t.transform:Find('core/frame/btn_draw'):GetComponent(typeof(Button))
  t.drawButton.onClick:AddListener(t.ClickDrawButtonHandler)
  t.drawText = t.transform:Find('core/frame/btn_draw/text'):GetComponent(typeof(Text))
  t.drawText.text = LocalizationController.instance:Get('ui.consortia_view.battle.lose_view.draw')
end

function t.BindDelegate ()
end

function t.UnbindDelegate ()
end

function t.Close ()
  t.UnbindDelegate ()
  UIMgr.instance:Close(PREFAB_PATH)
end

function t.ClickDrawButtonHandler ()
  t.Close()
end

return t