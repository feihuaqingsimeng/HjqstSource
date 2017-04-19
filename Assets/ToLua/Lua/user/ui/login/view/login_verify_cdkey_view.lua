local t = {}

local PREFAB_PATH = 'ui/login/login_verify_cdkey_view'

local login_controller = gamemanager.GetCtrl('login_controller')

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
  login_controller.VerifyCDKEYSucDelegate:AddListener(t.VerifyCDKEYSucByProtocol)
end
function t.UnbindDelegate()
  login_controller.VerifyCDKEYSucDelegate:RemoveListener(t.VerifyCDKEYSucByProtocol)
end

function t.InitComponent()
  local frame = t.transform:Find('core/frame')
  t.inputFieldContent = frame:Find('InputField'):GetComponent(typeof(InputField))
  t.btnSure = frame:Find('btn_sure'):GetComponent(typeof(Button))
  t.btnSure.onClick:AddListener(t.ClickSureBtnHandler)
  t.btnGainCdkey = frame:Find('btn_gain_cdkey'):GetComponent(typeof(Button))
  t.btnGainCdkey.onClick:AddListener(t.ClickGainCDKeyBtnHandler)
end
--------------------------click event--------------

function t.ClickSureBtnHandler()
  local cdkey = t.inputFieldContent.text
  if cdkey == '' then
    return
  end
  login_controller.VerifyCDKEYReq(cdkey)
end

function t.ClickGainCDKeyBtnHandler()
  LuaCsTransfer.OpenURL('http://ka.9game.cn/code_index_1.html')
end
----------------------update by protocol------------------

function t.VerifyCDKEYSucByProtocol()
  t.Close()
end
return t