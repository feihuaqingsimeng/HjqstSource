local t = {}
local name = 'friend_controller'

require 'friend_pb'
require 'common_pb'

local function Start()
  gamemanager.RegisterCtrl(name,t)
  netmanager.RegisterProtocol(MSG.RoleInfoLookUpResp,t.RoleInfoLookUpResp) 
end

--请求好友列表
function t.FriendListReq()
 	if gamemanager.GetModel('function_open_model').IsFunctionOpen(FunctionOpenType.MainView_Friend,false) then
  		netmanager.SendProtocol(MSG.FriendListReq)
	end
end

--请求添加好友
function t.FriendAddReq(roleName)
  LuaCsTransfer.FriendAddReq(roleName)
end

--请求查看玩家信息
t.roleInfoLookFunctionOpenType = nil

function t.RoleInfoLookUpReq(roleId,functionOpenType)
  t.roleInfoLookFunctionOpenType = functionOpenType
  local req = common_pb.IntProto()
  req.value = roleId
  netmanager.SendProtocol(MSG.RoleInfoLookUpReq,req)
  
end

--查看玩家信息
function t.RoleInfoLookUpResp()
  local resp = friend_pb.RoleInfoLookUpResp()    
  resp:ParseFromString(netmanager.GetProtocolData())  
  --resp.id,resp.team
  local player_information_info = dofile('ui/player_information_check/model/player_information_info')
  
  local playerInfomationInfo = player_information_info.NewByRoleDetInfoProto(resp.info)
  
  if t.roleInfoLookFunctionOpenType == FunctionOpenType.FightCenter_Arena then
    local player_information_check_view = dofile('ui/player_information_check/view/player_information_check_simple_view')
    player_information_check_view.Open(playerInfomationInfo,function(info)
        gamemanager.GetCtrl('arena_controller').OpenArenaEmbattleView(true)
      end)
  else
    local player_information_check_view = dofile('ui/player_information_check/view/player_information_check_view')
    player_information_check_view.Open(playerInfomationInfo)
  end
  
  
end
--响应添加好友 0:申请成功 1:添加好友成功
function t.FriendAddRespByCSharp(state)
  print('响应添加好友:state',state)
end





Start()
return t
