local t = {}

local PREFAB_PATH = 'ui/cdkey/cdkey_gift_view'

local login_controller = gamemanager.GetCtrl('login_controller')
local auto_destroy_tip_view = require('ui/tips/view/auto_destroy_tip_view')

function t.Open()
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay)
  t.transform = gameObject:GetComponent(typeof(Transform))
  t.BindDelegate()
  
  t.InitComponent()
end

function t.Close()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
end

function t.BindDelegate()
end
function t.UnbindDelegate()
end

function t.InitComponent()
  local frame = t.transform:Find('core/frame')
  t.inputFieldContent = frame:Find('InputField'):GetComponent(typeof(InputField))
  t.btnSure = frame:Find('btn_sure'):GetComponent(typeof(Button))
  t.btnSure.onClick:AddListener(t.ClickSureBtnHandler)
  t.btnClose = frame:Find('btn_close'):GetComponent(typeof(Button))
  t.btnClose.onClick:AddListener(t.Close)
end
--------------------------click event--------------

function t.ClickSureBtnHandler()
  local cdkey = t.inputFieldContent.text
  if cdkey == '' then
    return
  end
  gamemanager.GetCtrl('activity_controller').GiftCodeUseReq(cdkey)
end

----------------------update by protocol------------------

return t