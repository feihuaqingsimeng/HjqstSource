local t = {}
local name = "golden_touch_controller"

require 'shopping_pb'

t.UpdateGoldenInfoListDelegate = void_delegate.New()
t.UpdateGoldenSuccessDelegate = void_delegate.New()

local function Start ()
  netmanager.RegisterProtocol(MSG.GoldHandInfoResp, t.GoldHandInfoResp)
  netmanager.RegisterProtocol(MSG.GoldHandUseResp, t.GoldHandUseResp)

  gamemanager.RegisterCtrl(name,t)

end
--请求点金手信息
function t.GoldHandInfoReq()
  netmanager.SendProtocol(MSG.GoldHandInfoReq,nil)
end
--响应点金手信息
function t.GoldHandInfoResp()
  local resp = shopping_pb.GoldHandInfoResp()    
  resp:ParseFromString(netmanager.GetProtocolData())
  gamemanager.GetModel('golden_touch_model').AddUseLogList(resp,true)
  t.UpdateGoldenInfoListDelegate:Invoke()
end
--请求点金手
function t.GoldHandUseReq()
  netmanager.SendProtocol(MSG.GoldHandUseReq,nil)
end
--响应点金手
function t.GoldHandUseResp()
  
  local resp = shopping_pb.GoldHandInfoResp()    
  resp:ParseFromString(netmanager.GetProtocolData())
  gamemanager.GetModel('golden_touch_model').AddUseLogList(resp,true)
  t.UpdateGoldenSuccessDelegate:Invoke()
end
--------------------open view----------------------
function t.OpenGoldenTouchView()
  local view = dofile('ui/golden_touch/view/gold_touch_view')
  view.Open()
end

Start()
return t
