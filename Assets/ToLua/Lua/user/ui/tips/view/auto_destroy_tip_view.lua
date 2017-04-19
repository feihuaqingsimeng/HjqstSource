local t = {}
local PREFAB_PATH = 'ui/tips/common_auto_destroy_tips_view'
local delay = 0.2

function t.Open(tipStr)
  local go = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.Tips,UIOpenMode.Overlay)
  t.transform = go:GetComponent(typeof(Transform))
  t.tranFrame = t.transform:Find('core/img_frame')
  t.textTip = t.tranFrame:Find('text_tips'):GetComponent(typeof(Text))
  t.textTip.text = tipStr
  local x = t.textTip.preferredWidth+20
  if x < 500 then
    x = 500
  end  
  t.tranFrame.sizeDelta = Vector2(x,t.tranFrame.sizeDelta.y)
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