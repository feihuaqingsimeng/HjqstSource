local t = {}
local name = 'formation_controller'

require 'team_pb'
require 'common_pb'

local function Start ()
  
  netmanager.RegisterProtocol(MSG.TeamInfoResp, t.TeamInfoResp)
  netmanager.RegisterProtocol(MSG.TeamAddResp, t.TeamAddResp)
  netmanager.RegisterProtocol(MSG.LineupAttrActiveResp, t.LineupAttrActiveResp)
  netmanager.RegisterProtocol(MSG.LineupUpgradeResp, t.LineupUpgradeResp)
  netmanager.RegisterProtocol(MSG.LineupPointBuyResp, t.LineupPointBuyResp)
  netmanager.RegisterProtocol(MSG.LineupPointSynResp, t.LineupPointSynResp)
  netmanager.RegisterProtocol(MSG.LineupAddResp, t.LineupAddResp)

  gamemanager.RegisterCtrl(name,t)
end

------------------req---------------------
function t.TeamChangeReq(formationTeamType)
  local req = team_pb.TeamChangeReq()
  
  local formationTeamInfo = gamemanager.GetModel('formation_model').GetFormationTeam(formationTeamType)
 -- req.team = team_pb.TeamProtoData()

  req.team.teamNo = formationTeamType
  req.team.lineupNo = formationTeamInfo.formationId
  

  for k,v in pairs(formationTeamInfo.teamPosTable) do
    local posData = req.team.posList:add()
    posData.posIndex = k
    posData.heroId = v
   -- Debugger.LogError('[TeamChangeReq]pos:'..k..'  heroId:'..v)
  end
  netmanager.SendProtocol(MSG.TeamChangeReq, req)
  gamemanager.GetModel('formation_model').FormationChangeDelegateHandler()
  gamemanager.GetModel('hero_model').CheckHasAdvanceBreakthroughHeroByRedPoint()
  gamemanager.GetModel('hero_model').CheckHasRelationshipByRedPoint()
end

--------------resp----------------------
function t.TeamInfoResp()
  local resp = team_pb.TeamInfoResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  local formation_model = gamemanager.GetModel('formation_model')
  formation_model.AddAllUnlockFormation(resp.lineupList)
  formation_model.AddAllTeamData(resp.teamList)
  formation_model.bringUpPointPurcasedTimes = resp.bringUpPointPurcasedTimes
  formation_model.SetCurrentPVESelectFormationTeamType(resp.currTeamNo)
  Observers.Facade.Instance:SendNotification('InitAllTeamAndFormationFromLua')
 -- print('...................TeamInfoResp.................',table.count(resp.lineupList),table.count(resp.teamList))
end
--响应新增队伍(S->C)
function t.TeamAddResp()
  local resp = team_pb.TeamAddResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  local formation_model = gamemanager.GetModel('formation_model')
  for k,v in ipairs(resp.newTeamList) do 
    formation_model.AddTeamData(v)
  end
  
  Observers.Facade.Instance:SendNotification('InitAllTeamAndFormationFromLua')
 -- print('...................TeamAddResp.................',#resp.newTeamList)

end
--请求激活附加属性
function t.LineupAttrActiveReq(formationId)
  local req = common_pb.IntProto()
  req.value = formationId
  netmanager.SendProtocol(MSG.LineupAttrActiveReq, req)
end
--响应激活附加属性
function t.LineupAttrActiveResp()
  local resp = common_pb.IntProto()
  resp:ParseFromString(netmanager.GetProtocolData())
  local formationId = resp.value
  print('=============响应激活附加属性========formationId',formationId)
  local formation_model = gamemanager.GetModel('formation_model')
  formation_model.SetAdditionFormationAttrActive(formationId,true)
  formation_model.FormationAdditionAttrActiveDelegate:Invoke()
  Observers.Facade.Instance:SendNotification('InitAllTeamAndFormationFromLua')
end
--请求阵型升级(C->S)
function t.LineupUpgradeReq(id)
  local req = team_pb.LineupUpgradeReq()
  req.no = id
  netmanager.SendProtocol(MSG.LineupUpgradeReq, req)
end
--响应阵型升级(S->C)
function t.LineupUpgradeResp()
  local resp = team_pb.LineupUpgradeResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  local formation_model = gamemanager.GetModel('formation_model')
  formation_model.GetFormationInfo(resp.no).level = resp.lv
  formation_model.FormationUpgradeDelegateHandler()
  Observers.Facade.Instance:SendNotification('InitAllTeamAndFormationFromLua')
end
--请求购买培养点
function t.LineupPointBuyReq()
  local req = team_pb.LineupPointBuyReq()
  netmanager.SendProtocol(MSG.LineupPointBuyReq, req)
end
--响应购买培养点(S->C)
function t.LineupPointBuyResp()
  local resp = team_pb.LineupPointBuyResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  local formation_model = gamemanager.GetModel('formation_model')
  formation_model.bringUpPointPurcasedTimes = resp.bringUpPointPurcasedTimes
  formation_model.FormationBuyTrainPointDelegateHandler()
end
--请求培养点同步
function t.LineupPointSynReq()
  local req = team_pb.LineupPointSynReq()
  netmanager.SendProtocol(MSG.LineupPointSynReq, req)
end
--响应培养点同步(S->C)
function t.LineupPointSynResp()
  local resp = team_pb.LineupPointSynResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  local formation_model = gamemanager.GetModel('formation_model')
  formation_model.UpdateTrainPointByProtocol(resp.point, resp.recoverUpperLimit, resp.nextRecoverTime)
end
--响应新增阵型
function t.LineupAddResp()
  local resp = team_pb.LineupAddResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  local formation_model = gamemanager.GetModel('formation_model')
  for k,v in ipairs(resp.newLineupList) do
    local info = formation_model.GetFormationInfo(v.no)
    info:Update(v)
    formation_model.AddNewFormationTip(v.no)
  end
  Observers.Facade.Instance:SendNotification('InitAllTeamAndFormationFromLua')
end

-------------------------------OPEN View -----------------
function t.OpenTrainingFormationView(formationTeamType)
  if gamemanager.GetModel('function_open_model').IsFunctionOpen(FunctionOpenType.FormationTraining,true) then
    local view = dofile('ui/formation/view/training/train_formation_view')
    view.Open(formationTeamType)
  end
end



Start ()
return t