local t = {}
local name = 'login_controller'

require 'login_pb'

t.VerifyCDKEYSucDelegate = void_delegate.New()

local function Start ()
  
  netmanager.RegisterProtocol(MSG.VerifyCDKEYResp, t.VerifyCDKEYResp)

  gamemanager.RegisterCtrl(name,t)
end

function t.VerifyCDKEYReq(cdkey)
  local req = login_pb.VerifyCDKEYReq()
  local game_model = gamemanager.GetModel('game_model')
  req.account = game_model.platformAccountId
  req.partnerId = game_model.channelId
  req.cdkey = cdkey
  netmanager.SendProtocol(MSG.VerifyCDKEYReq, req)
end



function t.VerifyCDKEYResp()
  Debugger.LogError('验证成功')
  t.VerifyCDKEYSucDelegate:Invoke()
end



-----------------=================open view---------------

function t.OpenLoginVerifyCDKeyView()
  
  local view = require('ui/login/view/login_verify_cdkey_view')
  view.Open()
  
end

Start()
return t