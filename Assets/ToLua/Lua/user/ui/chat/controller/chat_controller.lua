local t = {}
local name = 'chat_controller'

require 'chat_pb'

local function Start ()
  netmanager.RegisterProtocol(MSG.ChatResp, t.ChatResp)
  netmanager.RegisterProtocol(MSG.ChatInfoResp, t.ChatInfoResp)
  netmanager.RegisterProtocol(MSG.CheckRoleGuildResp, t.CheckRoleGuildResp)
        
  gamemanager.RegisterCtrl(name, t)
    -- t.ChatInfoReq()

end

function OpenChat()

  if gamemanager.GetModel('chat_model').hasGuild then
    -- print('不在公会')
    netmanager.SendProtocol(MSG.ChatInfoReq)
  else
    -- print('在公会')
    t.CheckRoleGuildReq()
  end
 
  if t.view then
    t.view=dofile('ui/chat/view/chat_view')
    t.view.Open()
    gamemanager.GetCtrl('friend_controller').FriendListReq()

  else
    -- t.ChatInfoReq()
    t.view=dofile('ui/chat/view/chat_view')
    t.view.Open()
  end

  -- t.CheckRoleGuildReq()
end

function t.IsViewOpen()
  if t.view then
    return t.view.open
  end
end
--------------------------protocol---------------------
function t.ChatInfoReq()
  netmanager.SendProtocol(MSG.ChatInfoReq)
end

function t.ChatReq(chatType,revRoleName,content)
  local req = chat_pb.ChatReq()
  req.chatType = chatType
  req.revRoleName = revRoleName
  req.content = content
  -- print('ChatReq:',chatType,revRoleName,content)
  netmanager.SendProtocol(MSG.ChatReq,req)
end

function t.CheckRoleGuildReq()
  netmanager.SendProtocol(MSG.CheckRoleGuildReq)
end

function t.ChatResp()
  local resp = chat_pb.ChatResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  -- print('响应聊天')
  gamemanager.GetModel('chat_model').ChatResp(resp)
end

function t.ChatInfoResp()
  local resp = chat_pb.ChatInfoResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  -- print('响应聊天列表')
  gamemanager.GetModel('chat_model').ChatInfoResp(resp)
end

function t.CheckRoleGuildResp()
  local resp = chat_pb.CheckRoleGuildResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  -- print('响应聊天列表')
  gamemanager.GetModel('chat_model').CheckRoleGuildResp(resp)
end

Start ()
return t