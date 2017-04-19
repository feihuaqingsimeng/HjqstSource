local t = {}
local PREFAB_PATH = 'ui/tips/get_hero_piece_tip_view'
local delay = 0.2

function t.Open(heroPieceGameResData)
  local go = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.Tips,UIOpenMode.Overlay)
  t.transform = go:GetComponent(typeof(Transform))
  t.getHeroPieceTipsText = t.transform:Find('core/text_get_hero_piece_tips'):GetComponent(typeof(Text))
  
  local item_data = gamemanager.GetData('item_data')
  local heroPieceItemData = item_data.GetDataById(heroPieceGameResData.id)
  local hero_piece_name_with_color = ui_util.FormatStringWithinQualityColor (heroPieceItemData.quality, ui_util.GetQualityColorString(heroPieceItemData.quality)..LocalizationController.instance:Get(heroPieceItemData.name))
  t.getHeroPieceTipsText.text = string.format(LocalizationController.instance:Get('ui.get_hero_piece_tip_view.get_hero_piece_tips'), hero_piece_name_with_color, heroPieceGameResData.count)
  t.StartAction()
end

function t.StartAction()
  LeanTween.cancel(t.transform.gameObject)
  local fadeTo = CommonFadeToAnimation.Get(t.transform.gameObject)
  fadeTo:init(0,1,delay,0);
  coroutine.stop(t.destroyCorotine)
  t.destroyCorotine = coroutine.start(t.DestroyDelayCoroutine)
end

function t.DestroyDelayCoroutine()
  coroutine.wait(1.5)

  local fadeTo = CommonFadeToAnimation.Get(t.transform.gameObject)
  fadeTo:init(1,0,delay,0);
  coroutine.wait(delay)

  t.Close()
end

function t.Close()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end

return t