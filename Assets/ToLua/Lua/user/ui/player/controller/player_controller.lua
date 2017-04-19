local t = {}
local name = 'player_controller'

require 'player_pb'
local player_model = gamemanager.GetModel('player_model')
--local player_info = require('ui/player/model/player_info')
--local game_model = gamemanager.GetModel('game_model')
--local formation_model = gamemanager.GetModel('formation_model')

local function Start()
  netmanager.RegisterProtocol(MSG.GetAllPlayerResp,t.GetAllPlayerResp)
  netmanager.RegisterProtocol(MSG.PlayerUpdateResp,t.PlayerUpdateResp)
  netmanager.RegisterProtocol(MSG.PlayerTransferResp,t.PlayerTransferResp)
  netmanager.RegisterProtocol(MSG.PlayerChangeResp,t.PlayerChangeResp)
  netmanager.RegisterProtocol(MSG.TalentSynResp,t.TalentSynResp)
  netmanager.RegisterProtocol(MSG.TalentActivateResp,t.TalentActivateResp)
  netmanager.RegisterProtocol(MSG.TalentUpgradeResp,t.TalentUpgradeResp)
  netmanager.RegisterProtocol(MSG.TalentChooseResp,t.TalentChooseResp)
  netmanager.RegisterProtocol(MSG.PlayerAggrResp,t.PlayerAggrResp)
  netmanager.RegisterProtocol(MSG.PlayerBreakResp,t.PlayerBreakResp)
  netmanager.RegisterProtocol(MSG.PlayerAdvanceResp,t.PlayerAdvanceResp)
  
  gamemanager.RegisterCtrl(name,t)
end

-------------------resp-----------------
function t.GetAllPlayerResp()
  local resp = player_pb.GetAllPlayerResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  local player_model = gamemanager.GetModel('player_model')
  local player_info = require('ui/player/model/player_info')

  for k,v in ipairs(resp.players) do
    local playerInfo = player_info:NewByPlayerProtoData(v)
    player_model.AddPlayer(playerInfo)
    --------need update player equipments------------------
    player_model.UpdatePlayerEquipments(playerInfo.instanceID, v.wearEquips)
    player_model.UpdateSkillTalent(v.modelId,v.talnets,v.selectedTalnet)
  end
  gamemanager.GetModel('game_model').playerInfo = player_model.GetPlayerInfo(resp.currPlayerId)
  Observers.Facade.Instance:SendNotification('GET_ALL_PLAYER_RESP_handler')
end
--更新主角
function t.PlayerUpdateResp()
  local resp = player_pb.PlayerUpdateResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  local protoData = resp.player
  local player_model = gamemanager.GetModel('player_model')
  
  local playerInfo = player_model.GetPlayerInfo(protoData.id)
  playerInfo:SetByPlayerProtoData(protoData)
  --------need update player equipments------------------
  player_model.UpdatePlayerEquipments(playerInfo.instanceID, protoData.wearEquips)
  player_model.UpdateSkillTalent(protoData.modelId,protoData.talnets,protoData.selectedTalnet)
 -- print('update player.............',protoData.id)
  
  Observers.Facade.Instance:SendNotification('PLAYER_UPDATE_RESP_handler',protoData.id)
  player_model.OnPlayerInfoUpdateByProtocol()
end
--转职
function t.PlayerTransferResp()
  local resp = player_pb.PlayerTransferResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  local player_model = gamemanager.GetModel('player_model')
  local player_info = require('ui/player/model/player_info')
  local game_model = gamemanager.GetModel('game_model')
  local formation_model = gamemanager.GetModel('formation_model')
  
  local playerInfo = player_info:NewByPlayerProtoData(resp.newPlayer)
  playerInfo.faceIndex = game_model.playerInfo.faceIndex
  playerInfo.hairCutIndex = game_model.playerInfo.hairCutIndex
  playerInfo.hairColorIndex = game_model.playerInfo.hairColorIndex
  playerInfo.skinIndex = game_model.playerInfo.skinIndex
  player_model.AddPlayer(playerInfo)
  formation_model.TransferPlayer(playerInfo.instanceID)
  game_model.playerInfo = player_model.GetPlayerInfo(playerInfo.instanceID)
  print('转职啦,id',playerInfo.instanceID)
  Observers.Facade.Instance:SendNotification('LOBBY2CLIENT_PLAYER_TRANSFER_RESP_handler')
  player_model.OnPlayerInfoUpdateByProtocol()
  player_model.OnPlayerActivateProfessionSucess()
end
--切换阵型
function t.PlayerChangeResp()
  local resp = player_pb.PlayerChangeResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  local player_model = gamemanager.GetModel('player_model')
  local game_model = gamemanager.GetModel('game_model')
  local formation_model = gamemanager.GetModel('formation_model')
  
  formation_model.TransferPlayer(resp.desInstanceId)
  game_model.playerInfo = player_model.GetPlayerInfo(resp.desInstanceId)
  game_model.playerInfo:SetByPlayerProtoData(resp.newPlayer)
  print('改主角啦,id',resp.desInstanceId)
  Observers.Facade.Instance:SendNotification('LOBBY2CLIENT_PLAYER_CHANGE_RESP_handler',resp.desInstanceId)
  
  player_model.OnPlayerInfoUpdateByProtocol()
end
--同步天赋变化
function t.TalentSynResp()
  local resp = player_pb.TalentSynResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  local game_model = gamemanager.GetModel('game_model')
  local player_model = gamemanager.GetModel('player_model')
  player_model.UpdateSkillTalent(game_model.playerInfo.playerData.id,resp.talnets,resp.selectedTalnet)
  Observers.Facade.Instance:SendNotification('TalentSynResp')
end
--请求激活天赋
function t.TalentActivateReq(id)
  local req = player_pb.TalentActivateReq()
  req.no = id
  netmanager.SendProtocol(MSG.TalentActivateReq, req)
end
--响应激活天赋
function t.TalentActivateResp()
  local resp = player_pb.TalentActivateResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  local player_model = gamemanager.GetModel('player_model')
  local info = player_model.GetSkillTalentInfo(resp.no)
  info:Set(info.id,1,0)
  Observers.Facade.Instance:SendNotification('TalentActivateResp',resp.no)
end
--请求升级天赋
function t.TalentUpgradeReq(id)
  local req = player_pb.TalentUpgradeReq()
  req.no = id
  netmanager.SendProtocol(MSG.TalentUpgradeReq, req)
end
--响应升级天赋
function t.TalentUpgradeResp()
  local resp = player_pb.TalentUpgradeResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  local player_model = gamemanager.GetModel('player_model')
  local info = player_model.GetSkillTalentInfo(resp.no)
  info:Set(info.id,info.level+1,0)
  Observers.Facade.Instance:SendNotification('TalentUpgradeResp',resp.no)
end
--请求变更主动天赋
function t.TalentChooseRep(passiveId,summonId)
  local req = player_pb.TalentChooseRep()
  if passiveId ~= 0 then
    req.selectedTalnet:append(passiveId)
  end
  if summonId ~= 0 then
    req.selectedTalnet:append(summonId)
  end
  netmanager.SendProtocol(MSG.TalentChooseRep, req)
end
function t.TalentChooseResp()
   local resp = player_pb.TalentChooseResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  local player_model = gamemanager.GetModel('player_model')
  local game_model = gamemanager.GetModel('game_model')
  player_model.UpdateSkillTalent(game_model.playerInfo.playerData.id,nil,resp.selectedTalnet)
  Observers.Facade.Instance:SendNotification('TalentChooseResp')
end

-- temporarily in C# --
function t.PlayerTransferReq (desInstanceId)
end

function t.PlayerChangeReq (desInstanceId)
  local playerChangeReq = player_pb.PlayerChangeReq()
  playerChangeReq.desInstanceId = desInstanceId
  netmanager.SendProtocol(MSG.PlayerChangeReq, playerChangeReq)
end

function t.PlayerAggrReq (playerId, consumeIds)
  local playerAggrReq = player_pb.PlayerAggrReq()
  playerAggrReq.playerId = playerId
  for k, v in pairs(consumeIds) do
    playerAggrReq.consumeIds:append(v)
  end
  netmanager.SendProtocol(MSG.PlayerAggrReq, playerAggrReq)
end

function t.PlayerAggrResp ()
  local playerAggrResp = player_pb.PlayerAggrResp()
  playerAggrResp:ParseFromString(netmanager.GetProtocolData())
  
  player_model.OnPlayerInfoUpdateByProtocol()
  player_model.OnPlayerAggrSuccess(playerAggrResp.isCrit)
end

function t.PlayerBreakReq (costIndex)
  local playerBreakReq = player_pb.PlayerBreakReq()
  playerBreakReq.costIndex = costIndex
  netmanager.SendProtocol(MSG.PlayerBreakReq, playerBreakReq)
end

function t.PlayerBreakResp ()
  player_model.OnPlayerInfoUpdateByProtocol()
  player_model.OnPlayerBreakthroughSuccess(gamemanager.GetModel('game_model').playerInfo.instanceID)
end

function t.PlayerAdvanceReq (playerInstanceID)
  local playerAdvanceReq = player_pb.PlayerAdvanceReq()
  t.LastAdvancePlayerInstanceID = playerInstanceID
  playerAdvanceReq.playerId = playerInstanceID
  netmanager.SendProtocol(MSG.PlayerAdvanceReq, playerAdvanceReq)
end

function t.PlayerAdvanceResp ()
  local playerAdvanceResp = player_pb.PlayerAdvanceResp()
  playerAdvanceResp:ParseFromString(netmanager.GetProtocolData())
  
  if playerAdvanceResp.result == 1 then
    player_model.OnPlayerAdvanceSuccess(t.LastAdvancePlayerInstanceID)
  end
end

Start()
return t