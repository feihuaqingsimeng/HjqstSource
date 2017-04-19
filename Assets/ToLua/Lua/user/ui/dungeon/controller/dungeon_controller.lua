local t = {}
local name = 'dungeon_controller'
require 'pve_pb'

local function Start ()
  gamemanager.RegisterCtrl(name,t)
  netmanager.RegisterProtocol(MSG.PveInfoResp, t.PveInfoResp)
  netmanager.RegisterProtocol(MSG.PveFightResp, t.PveFightResp)
  netmanager.RegisterProtocol(MSG.PveFightOverResp, t.PveFightOverResp)
  netmanager.RegisterProtocol(MSG.PveMopUpResp, t.PveMopUpResp)
  netmanager.RegisterProtocol(MSG.PveTenMopUpResp, t.PveTenMopUpResp)
end

--[[ Request ]]--
function t.PveInfoReq ()
  local pveInfoReq = pve_pb.PveInfoReq()
  netmanager.SendProtocol(MSG.PveInfoReq, pveInfoReq)
end

function t.PveFightReq (dungeonID)
  local pveFightReq = pve_pb.PveFightReq()
  pveFightReq.dungeonId = dungeonID  
  netmanager.SendProtocol(MSG.PveFightReq, pveFightReq)
end

function t.PveFightOverReq (dungeonID, passTime, result, outDamage, deadNum, remainHPPercent, comboCount, dieHeroIDs)
  local pveFightOverReq = pve_pb.PveFightOverReq()
  pveFightOverReq.dungeonId = dungeonID
  pveFightOverReq.passTime = passTime
  pveFightOverReq.result = result
  pveFightOverReq.dieNum = deadNum
  pveFightOverReq.outDamage = outDamage
  pveFightOverReq.remainHpPercent = remaindHpPercent
  pveFightOverReq.combo = comboCount
  table.addtable(pveFightOverReq.dieHeroIds, dieHeroIDs)
  netmanager.SendProtocol(MSG.PveFightOverReq, pveFightOverReq)
end

function t.PveMopUpReq (dungeonID)
  local pveMopUpReq = pve_pb.PveMopUpReq()
  pveMopUpReq.dungeonId = dungeonID
  netmanager.SendProtocol(MSG.PveMopUpReq, pveMopUpReq)
end

function t.PveTenMopUpReq (dungeonID)
  local pveTenMopUpReq = pve_pb.PveTenMopUpReq()
  pveTenMopUpReq.dungeonId = dungeonID
  netmanager.SendProtocol(MSG.PveTenMopUpReq, pveTenMopUpReq)
end
--[[ Request ]]--

--[[ Response ]]--
function t.PveInfoResp ()
  local pveInfoResp = pve_pb.PveInfoResp()
  pveInfoResp:ParseFromString(netmanager.GetProtocolData())
  
  gamemanager.GetModel('dungeon_model').UpdateDungeonInfos(pveInfoResp)
  Logic.UI.SoftGuide.Model.SoftGuideProxy.instance:UpdateSoftGuide()
end

function t.PveFightResp ()
  local pveFightResp = pve_pb.PveFightResp()
  pveFightResp:ParseFromString(netmanager.GetProtocolData())
end

function t.PveFightOverResp ()
  local pveFightOverResp = pve_pb.PveFightOverResp()
  pveFightOverResp:ParseFromString(netmanager.GetProtocolData())
end

function t.PveMopUpResp ()
  local pveMopUpResp = pve_pb.PveMopUpResp()
  pveMopUpResp:ParseFromString(netmanager.GetProtocolData())
end

function t.PveTenMopUpResp ()
  local pveTenMopUpResp = pve_pb.PveTenMopUpResp()
  pveTenMopUpResp:ParseFromString(netmanager.GetProtocolData())
end
--[[ Response ]]--

Start ()
return t