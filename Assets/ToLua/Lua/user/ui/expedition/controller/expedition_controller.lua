local t = {}
local name = 'expedition_controller'

require 'pve_pb'

local function Start()
  gamemanager.RegisterCtrl(name,t)
  netmanager.RegisterProtocol(MSG.ExpeditionResp,t.ExpeditionResp)
  netmanager.RegisterProtocol(MSG.SynExpeditionResp,t.SynExpeditionResp)
end
--请求远征战斗
function t.ExpeditionChallengeReq(dungeonId)
  local req = pve_pb.ExpeditionChallengeReq()
  req.dungeonId = dungeonId
  netmanager.SendProtocol(MSG.ExpeditionChallengeReq,req)
end
-------------------
--远征主界面消息
function t.ExpeditionResp()
  local resp = pve_pb.ExpeditionResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  local expedition_model = gamemanager.GetModel('expedition_model')
  expedition_model.resetCount = resp.remainRefreshTimes
  expedition_model.expeditionVipBuyTimes = resp.vipBuyTimes
  expedition_model.UpdateDungeonInfoDictionary(resp.lastPassDungeon,resp.getRewardDungeonIds)
  expedition_model.UpdateRoleDictionary(resp.heros)
  expedition_model.CheckDeadHeroAtFormation()
  expedition_model.UpdateResetSuccessByProtocol()
  
end
--更新消息
function t.SynExpeditionResp()
  local resp = pve_pb.ExpeditionResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  local expedition_model = gamemanager.GetModel('expedition_model')
  local rewardIdList = {}
  if resp.newGetRewardDungeon ~= 0 then
    rewardIdList[1] = resp.newGetRewardDungeon
  end
  expedition_model.UpdateDungeonInfoDictionary(resp.lastPassDungeon,rewardIdList)
  expedition_model.UpdateResetSuccessByProtocol()
end
-----------------------------open view----------------------
--远征阵型界面
function t.OpenExpeditionEmbattleView(isReadyFight)
  local view = dofile('ui/expedition/view/expedition_embattle_view')
  view.Open(isReadyFight)
end


Start()

return t
