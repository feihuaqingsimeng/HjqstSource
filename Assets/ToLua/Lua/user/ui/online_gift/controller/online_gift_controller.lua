local t = {}
local name = 'online_gift_controller'
require 'activity_pb'
require 'common_pb'

local function Start()
  gamemanager.RegisterCtrl(name,t)
  
  netmanager.RegisterProtocol(MSG.OnlineGiftResp,t.OnlineGiftResp)
  netmanager.RegisterProtocol(MSG.OnlineGiftSynResp,t.OnlineGiftSynResp)
end

function t.OnlineGiftSynReq()
  netmanager.SendProtocol(MSG.OnlineGiftSynReq,nil)
  print('请求在线礼包')
end

--
function t.OnlineGiftReq(id)
  local req = common_pb.IntProto()
  req.value = id
  netmanager.SendProtocol(MSG.OnlineGiftReq,req)
end
--响应领取在线礼包
function t.OnlineGiftResp()
  local resp = common_pb.IntProto()
  resp:ParseFromString(netmanager.GetProtocolData())
  print('响应领取在线礼包',resp.value)
  local online_gift_model = gamemanager.GetModel('online_gift_model')
  online_gift_model.OnlineGiftResp(resp.value)
end
--同步在线礼包
function t.OnlineGiftSynResp()
  local resp = common_pb.DoubleIntProto()
  resp:ParseFromString(netmanager.GetProtocolData())
  print('同步在线礼包',resp.value1,resp.value2)
  local online_gift_model = gamemanager.GetModel('online_gift_model')
  online_gift_model.OnlineGiftSynResp(resp.value1,resp.value2)
end
--打开礼包奖励详情
function t.OpenOnlineGiftRewardView()
 local online_gift_reward_view = dofile('ui/online_gift/view/online_gift_reward_view')
 online_gift_reward_view.Open()
end 

Start()

return t