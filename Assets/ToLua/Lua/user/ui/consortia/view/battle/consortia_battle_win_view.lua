local t = {}
local PREFAB_PATH = 'ui/consortia/battle/consortia_battle_win_view'

function t.Open ()
  local gameObject = UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay)
  t.transform = gameObject.transform
  
  t.InitComponent ()
  t.BindDelegate ()
end

function t.InitComponent ()
  t.congratulationsText = t.transform:Find('core/frame/text_congratulations'):GetComponent(typeof(Text))
  t.congratulationsText.text = LocalizationController.instance:Get('ui.consortia_view.battle.win_view.congratulation_tips')
  t.drawButton = t.transform:Find('core/frame/btn_draw'):GetComponent(typeof(Button))
  t.drawButton.onClick:AddListener(t.ClickDrawButtonHandler)
  t.drawText = t.transform:Find('core/frame/btn_draw/text'):GetComponent(typeof(Text))
  t.drawText.text = LocalizationController.instance:Get('ui.consortia_view.battle.win_view.draw')
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