local t = {}
local name = 'expedition_model'

local expedition_role_info = require('ui/expedition/model/expedition_role_info')
local expedition_data = gamemanager.GetData('expedition_data')
local expedition_dungeon_info = require('ui/expedition/model/expedition_dungeon_info')

--远征副本数据 id,expedition_dungeon_info
t.expeditionDungeonInfoDic = Dictionary.New('number','table')
--远征英雄数据id,expeditionRoleInfo
t.expeditionRoleDictionary = Dictionary.New('number','table')
--当前副本id
t.currentExpeditionDungeonId = 0
--剩余刷新次数
t.resetCount = 0
--vip购买次数
t.expeditionVipBuyTimes = 0


---------delegate---------------------
--远征副本刷新
t.UpdateResetSucDelegate = void_delegate.New()

local function Start()
  gamemanager.RegisterModel(name,t)
end

--设置远征英雄数据
function t.UpdateRoleDictionary(expeditionHeroProtoList)
  t.expeditionRoleDictionary:Clear()
  
  local game_model = gamemanager.GetModel('game_model')
  local hero_model = gamemanager.GetModel('hero_model')
  
  for k,v in ipairs(expeditionHeroProtoList) do
    local expeditionRoleInfo = expedition_role_info.New(v.heroId,v.hpPercent/10000)
    t.expeditionRoleDictionary:Add(v.heroId,expeditionRoleInfo)
  end
end
--第一次初始化
function t.InitDungeonInfoDictionary()
  for k,v in pairs(expedition_data.data) do
    t.expeditionDungeonInfoDic:Add(v.id,expedition_dungeon_info.New(v.id,false,false,false))
  end
end
--设置远征副本数据
function t.UpdateDungeonInfoDictionary(lastDungeonId,getRewardDungeonIdsList)
  if t.expeditionDungeonInfoDic.Count == 0 then
    t.InitDungeonInfoDictionary()
  end
  local valueList = t.expeditionDungeonInfoDic:GetValuesList()
  local flag = false
  for k,v in pairs(valueList) do
    flag = v.id <= lastDungeonId
    v.isFinished = flag
    v.isGetReward = flag
    v.isUnlocked = flag
  end
  --unlock
  if lastDungeonId == -1 then
    t.currentExpeditionDungeonId = expedition_data.firstData.id
    t.expeditionDungeonInfoDic:Get(t.currentExpeditionDungeonId).isUnlocked = true
  else
    local nextData = expedition_data.GetNextExpeditionData(lastDungeonId)
    if nextData then
      t.currentExpeditionDungeonId = nextData.id
      t.expeditionDungeonInfoDic:Get(t.currentExpeditionDungeonId).isUnlocked = true
      if nextData.type ~= ExpeditionDungeonType.Normal then
        t.expeditionDungeonInfoDic:Get(t.currentExpeditionDungeonId).isFinished = true
      end
    else
      t.currentExpeditionDungeonId = lastDungeonId
    end
  end
  --reward
  if getRewardDungeonIdsList ~= nil then
    for k,v in ipairs(getRewardDungeonIdsList) do
      t.expeditionDungeonInfoDic:Get(v).isGetReward = true
    end
  end
end
--获取可用的远征英雄(同类型英雄选战力最高者)
function t.GetEnableExpeditionHeroList()
  local game_model = gamemanager.GetModel('game_model')
  local hero_model = gamemanager.GetModel('hero_model')
  --以modelId为key
  local expditionInfoDic = {}
  --主角
  expditionInfoDic[game_model.playerInfo.playerData.id] = expedition_role_info.New(game_model.playerInfo.instanceID,1)
  --英雄
  local heroList = hero_model.GetAllHeroInfoList()
  local tempExpeditionInfo = nil
  for k,v in pairs(heroList) do
    tempExpeditionInfo = expditionInfoDic[v.heroData.id]
    if tempExpeditionInfo  then
      if tempExpeditionInfo.roleInfo:Power() < v:Power() then
        expditionInfoDic[v.heroData.id] = expedition_role_info.New(v.instanceID,1)
      end
    else
      expditionInfoDic[v.heroData.id] = expedition_role_info.New(v.instanceID,1)
    end
  end
  -- 设置hpRate
  local enableHeroList = {}
  local index = 1
  for k,v in pairs(expditionInfoDic) do
    
    tempExpeditionInfo = t.expeditionRoleDictionary:Get(v.roleInfo.instanceID)
    if tempExpeditionInfo then
      enableHeroList[index] = tempExpeditionInfo
    else
      enableHeroList[index] = v
    end
    
    index = index + 1
  end
    return enableHeroList
end
---死亡下阵 and 同英雄战力高者上
function t.CheckDeadHeroAtFormation()
  local heroInfoList = t.GetEnableExpeditionHeroList()
  local teamInfo = gamemanager.GetModel('formation_model').GetFormationTeam(FormationTeamType.expeditionTeam)
  if teamInfo == nil then
    return 
  end
  local removeTable = {}
  local expeditionRoleInfo = nil
  for k, v in pairs(teamInfo.teamPosTable) do
    expeditionRoleInfo = t.expeditionRoleDictionary:Get(v)
    if expeditionRoleInfo then
      if expeditionRoleInfo:IsDead() then--死翘翘了
        removeTable[k] = v
      else 
        for k1,v1 in pairs(heroInfoList) do--检测战力爆表者
          if expeditionRoleInfo.roleInfo.heroData.id == v1.roleInfo.heroData.id and expeditionRoleInfo.roleInfo:Power() < v1.roleInfo:Power() then
            teamInfo.teamPosTable[k] = v1.instanceID
            break
          end
        end
      end
    end
  end
  for k,v in pairs(removeTable) do
    teamInfo.teamPosTable[k] = nil
  end
  gamemanager.GetCtrl('formation_controller').TeamChangeReq(FormationTeamType.expeditionTeam)
end
----------------------------update by protocol----------------------
function t.UpdateResetSuccessByProtocol()
  t.UpdateResetSucDelegate:Invoke()
  Observers.Facade.Instance:SendNotification('LOBBY2CLIENT_Expedition_RESP_handler')
end

Start()

return t

