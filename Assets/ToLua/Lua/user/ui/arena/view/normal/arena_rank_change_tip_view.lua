local t = {}
local PREFAB_PATH = 'ui/pvp/pvp_rank_change_tip_view'
local name = PREFAB_PATH

local delay = 0.2

function t.Open(curRank,lastRank)
  if not uimanager.GetView(name) then
    uimanager.RegisterView(name,t)
    local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.Tips,UIOpenMode.Overlay)
    t.transform = gameObject:GetComponent(typeof(Transform))
    
    t.InitComponent()
  end
  
  
  t.curRank = curRank
  t.lastRank = lastRank
  if lastRank < curRank then
    t.textName.text = string.format(LocalizationController.instance:Get('ui.pvp_rank_change_tip_view.fellTip'),curRank-lastRank)
  end
  t.StartAction()
end
function t.InitComponent()
  t.textName = t.transform:Find('core/text_name'):GetComponent(typeof(Text))
end

function t.StartAction()
  coroutine.stop(t.destroyCorotine)
  LeanTween.cancel(t.transform.gameObject)
  local fadeTo = CommonFadeToAnimation.Get(t.transform.gameObject)
  fadeTo:init(0,1,delay,0)
  t.destroyCorotine = coroutine.start(t.DelayDestroyCoroutine)
end

function t.DelayDestroyCoroutine()
  coroutine.wait(1.5)
  local fadeTo = CommonFadeToAnimation.Get(t.transform.gameObject)
  fadeTo:init(1,0,delay,0);
  coroutine.wait(delay)
  uimanager.CloseView(name)
end

function t.Close()
  
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)

end

return t
