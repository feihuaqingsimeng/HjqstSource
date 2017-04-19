local t = {}
local name = 'mine_controller'
local mineModel = gamemanager.GetModel('mine_model')
local mineData = gamemanager.GetData('mine_data')
local mineMapView = nil
local otherMineView = nil
local selfMineView = nil
local roleMineView = nil
require 'mine_pb'
require 'common_pb'
local function Start()
  netmanager.RegisterProtocol(MSG.GetMineMapResp, t.GetMineMapResp)
  netmanager.RegisterProtocol(MSG.GetMineInfoResp, t.GetMineInfoResp)
  netmanager.RegisterProtocol(MSG.GetOccRoleInfoResp, t.GetOccRoleInfoResp)
  netmanager.RegisterProtocol(MSG.GetOwnInfoResp, t.GetOwnInfoResp)
  netmanager.RegisterProtocol(MSG.OccMineResp, t.OccMineResp)
  netmanager.RegisterProtocol(MSG.AbandonMineResp, t.AbandonMineResp)
  netmanager.RegisterProtocol(MSG.RobMineResp, t.RobMineResp)
  netmanager.RegisterProtocol(MSG.PlunderMineResp, t.PlunderMineResp)
  netmanager.RegisterProtocol(MSG.GetMineAwardResp, t.GetMineAwardResp)
  netmanager.RegisterProtocol(MSG.ChangeMineInfoResp, t.ChangeMineInfoResp)
  netmanager.RegisterProtocol(MSG.MineFightOverResp, t.MineFightOverResp)
  gamemanager.RegisterCtrl(name,t)
  mineMapView = dofile('ui/mine/view/mine_view')
  otherMineView = dofile('ui/mine/view/other_mine_view')
  
  selfMineView = dofile('ui/mine/view/self_mine_view')
  roleMineView = dofile('ui/mine/view/role_info_view')
end


function t.OpenMineMapView()
    mineMapView.Open()
end
function t.OpenOtherMineView()
    print("otherMineView=========",otherMineView)
    otherMineView.Open()
end
function t.OpenSelfMineView()
    print("selfMineView=========",selfMineView)
    selfMineView.Open()
end
function t.OpenRoleInfoView()
    roleMineView.Open()
end

--Req
function t.GetMineMapReq()
  netmanager.SendProtocol(MSG.GetMineMapReq,nil)
  print('send message map')
end

function t.GetMineInfoReq(mineId)
  local req = common_pb.IntProto()
  req.value = mineId
  netmanager.SendProtocol(MSG.GetMineInfoReq,req)
end

function t.GetOccRoleInfoReq(mineId,playerId)
  local req = common_pb.DoubleIntProto()
  req.value1 = mineId
  req.value2 = playerId
  netmanager.SendProtocol(MSG.GetOccRoleInfoReq,req)
end

function t.GetOwnInfoReq()
  print("发送信息")
  netmanager.SendProtocol(MSG.GetOwnInfoReq,nil)
  
end

function t.OccMineReq(mineId)
  local req = common_pb.IntProto()
  req.value = mineId
  netmanager.SendProtocol(MSG.OccMineReq,req)
end

function t.AbandonMineReq()
  netmanager.SendProtocol(MSG.AbandonMineReq,nil)
end

function t.RobMineReq(mineId,playerId)
  local req = common_pb.DoubleIntProto()
  req.value1 = mineId
  req.value2 = playerId
  netmanager.SendProtocol(MSG.RobMineReq,req)
end

function t.PlunderMineReq(mineId,playerId)
  local req = common_pb.DoubleIntProto()
  req.value1 = mineId
  req.value2 = playerId
  netmanager.SendProtocol(MSG.PlunderMineReq,req)
end

function t.GetMineAwardReq()
  netmanager.SendProtocol(MSG.GetMineAwardReq,nil)
end

function t.ChangeMineInfoReq(mineId)
  local req = common_pb.IntProto()
  req.value = mineId
  netmanager.SendProtocol(MSG.ChangeMineInfoReq,req)
end

function t.MineFightOverReq(result)
  local req = mine_pb.MineFightOverReq()
  req.result = result
  netmanager.SendProtocol(MSG.MineFightOverReq,req)
end


--Resp
function t.GetMineMapResp()
  print("获取到所有矿的资源")
  local mineMapResp = mine_pb.GetMineMapResp()
  mineMapResp:ParseFromString(netmanager.GetProtocolData()) 
  mineModel.SetOwnMineInfo(mineMapResp.occTime,mineMapResp.plunderTime,mineMapResp.ownMineNo,mineMapResp.endTime,mineMapResp.inCome)
  mineModel.m_aryMineItems ={}
  local itemInfo = require('ui/mine/model/mine_item_info')
  for k,v in ipairs(mineMapResp.mines) do
    local mineItem = itemInfo:NewMineItem(v)
    mineModel.AddMineItemList(mineItem)
  end
  local allMine = mineData.GetMineItemsList()
  for key,value in pairs(allMine) do
    if mineModel.m_aryMineItems[key] == nil then
      local item ={}
      item.mineNo = value.id
      item.occNum = 0
      local mineItem = itemInfo:NewMineItem(item)
      mineModel.AddMineItemList(item)
    end
  end
  --table.sort(mineModel.m_aryMineItems,mineModel.MineConpare)
  mineModel.RefreshMineMap()
end

function t.GetMineInfoResp()
  print("获取到未占领矿的资源")
  local mineInfoResp = mine_pb.GetMineInfoResp()
  mineInfoResp:ParseFromString(netmanager.GetProtocolData())
  mineModel.m_aryMineRoles ={}
  local length = #mineInfoResp.occRoles
  print("获取到未占领矿的资源length:",length)
  local roleInfo = require('ui/mine/model/enemy_role_info')
  for index =1,length do
    local mineRole = roleInfo:NewMineRole(mineInfoResp.occRoles[index])
    mineModel.AddMineRoleList(mineRole)
  end
  mineModel.RefreshOtherMine()
end

function t.GetOccRoleInfoResp()
  print("玩家信息")
  local roleInfoResp = mine_pb.GetOccRoleInfoResp()
  roleInfoResp:ParseFromString(netmanager.GetProtocolData())  
  local team = roleInfoResp.team
  local playerInfo = require('ui/player/model/player_info')
  local player = playerInfo:New(team.player.id,team.player.playerNo,0,0,0,'')
  player.level = team.player.lv
  player.advanceLevel = team.player.star
  player.strengthenLevel= team.player.aggrLv
  player.breakthroughLevel= team.player.breakLayer
  mineModel.m_curRolePlayerInfo = player
  local hero_info = require('ui/hero/model/hero_info')
  mineModel.m_curAryRoleHeroInfo ={}
  print("team.heros:",#team.heros)
  for index, hero in ipairs(team.heros) do 
    local he = hero_info:New(hero.id,hero.heroNo,hero.breakLayer,hero.aggrLv,hero.star,hero.lv)
    table.insert(mineModel.m_curAryRoleHeroInfo,he)
  end
  mineModel.m_curRoleProtectTime = roleInfoResp.lastPlunderTime
  mineModel.m_curRoleGold = roleInfoResp.inCome
  --print("mineModel.m_curRoleProtectTime============",mineModel.m_curRoleProtectTime)
  --print("mineModel.m_curRoleGold============",mineModel.m_curRoleGold)
  --print("roleInfoResp.lineNo============",roleInfoResp.lineNo)
  mineModel.m_curFormationNo = roleInfoResp.lineNo
  mineModel.RefreshOtherRole()
end

function t.GetOwnInfoResp()
  print("自己信息")
  local ownInfoResp = mine_pb.GetOwnInfoResp()
  ownInfoResp:ParseFromString(netmanager.GetProtocolData())  
  print("ownInfoResp.logList",#ownInfoResp.logList)
  local selfMineInfo = require('ui/mine/model/self_mine_info')
  local selfInfo = selfMineInfo:New(ownInfoResp.inCome,ownInfoResp.logList)
  mineModel.m_selfMineInfo.m_reward =selfInfo.m_reward
  mineModel.m_selfMineInfo.m_listLog = selfInfo.m_listLog
  mineModel.RefreshSelfMine()
end

function t.OccMineResp()
  print("占领矿坑")
  mineModel.CloseOtherMine()
end

function t.AbandonMineResp()  
  print("give up矿坑")
  mineModel.CloseSelfMine()
end

function t.RobMineResp()
  local robMineResp = mine_pb.RobMineResp()
  robMineResp.ParseFromString(netmanager.GetProtocolData())  
  mineModel.m_selfTeamInfo = robMineResp.fightData.selfTeamData
  mineModel.m_enemyTeamInfo = robMineResp.fightData.opponentTeamData

end

function t.PlunderMineResp()
  local plunderMineResp = mine_pb.PlunderMineResp()
  plunderMineResp:ParseFromString(netmanager.GetProtocolData())   
  mineModel.m_selfTeamInfo = plunderMineResp.fightData.selfTeamData
  mineModel.m_enemyTeamInfo = plunderMineResp.fightData.opponentTeamData
end

function t.GetMineAwardResp()
  print("reward 矿坑")
  mineModel.CloseSelfMine()
end

function t.ChangeMineInfoResp()  
 -- mineModel.RefreshOtherMine()
end

function t.MineFightOverResp()
  local mineFightOverResp = mine_pb.MineFightOverResp()
  mineFightOverResp:ParseFromString(netmanager.GetProtocolData())  
  mineModel.FightOverReward()
end
Start ()
return t