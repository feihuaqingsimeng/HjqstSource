local t = {}
local name = 'formation_model'
t.formationInfoTable = {} -- 阵型
t.formationTeamTable = {} -- 阵型队伍
t.trainPoint = 0--培养点
t.trainPointRecoverMax = 0 --培养点上限
t.trainPointNextRecoverTime = -1 --下次恢复时间（时间戳）(-1回复已满)
t.trainPointNextRecoverCountDown = 0 --回复倒计时（ms）
t.bringUpPointPurcasedTimes = 0 --培养点已购买次数

local serverTime = 0
local lastTime = 0
local isFormationPointRecoverFullTip = false

----------delegate-------------------
t.FormationChangeDelegate = void_delegate.New()--阵型更改通知
t.FormationUpgradeDelegate = void_delegate.New()--阵型升级通知
t.FormationBuyTrainPointDelegate = void_delegate.New()--购买培养点通知
t.FormationAdditionAttrActiveDelegate = void_delegate.New() -- 激活附加属性通知
t.onRecoverTimeUpdateDelegate = void_delegate.New()--阵型点恢复更新

t.CurrentPVESelectFormationTeamType = FormationTeamType.pveFirstTeam



local formation_data = gamemanager.GetData('formation_data')
local formation_Info = require('ui/formation/model/formation_info')

local function Start ()
  gamemanager.RegisterModel(name, t)
  coroutine.start(t.UpdateCoroutine)
end
function t.SetBringUpPointPurcasedTimes(times)
  t.bringUpPointPurcasedTimes = times
end
--队伍
function t.GetFormationTeam(formationTeamType)
  local team = t.formationTeamTable[formationTeamType] 
  --[[if team == nil then
    Debugger.LogError('FormationTeam is nil ,type:'..formationTeamType)
  end]]
  return team
end
--pve当前队伍
function t.GetCurPveFormationTeam()
  return t.formationTeamTable[t.CurrentPVESelectFormationTeamType] 
end
--阵型
function t.GetFormationInfo(id)
  return t.formationInfoTable[id]
end

function t.IsHeroInAnyTeam(heroInstanceId)
  for k,v in pairs(t.formationTeamTable) do
    if v:IsHeroInFormation(heroInstanceId) then
      return true
    end
  end
  return false
end

function t.IsHeroInAnyPVETeam (roleInstanceID)
  return t.IsHeroInTeamByType(FormationTeamType.pveFirstTeam, roleInstanceID) or t.IsHeroInTeamByType(FormationTeamType.pveSecondTeam, roleInstanceID) or t.IsHeroInTeamByType(FormationTeamType.pveThirdTeam, roleInstanceID)
end

function t.IsHeroInTeamByType(formationTeamType, heroInstanceId)
   local team = t.GetFormationTeam(formationTeamType)
   if team and team:IsHeroInFormation(heroInstanceId) then
     return true
    end
  return false
end

---获得所有阵型上的英雄id
function t.GetInAnyTeamHeroIdList()
  local data = {}
  for k,v in pairs(t.formationTeamTable) do
    for k1,v1 in pairs(v.teamPosTable) do
      data[v1] = v1
    end
  end
  return data
end
---获得所有pve阵型上的英雄id
function t.GetAnyPveTeamHeroIdList()
  local data = {}
  for k,v in pairs(t.formationTeamTable) do
    if k ~= FormationTeamType.expeditionTeam then
      for k1,v1 in pairs(v.teamPosTable) do
        data[v1] = v1
      end
    end
    
  end
  return data
end
function t.IsAnyInFormationHeroHasFitEquipment ()
  local inAnyFormationHeroIdList = t.GetAnyPveTeamHeroIdList()
  for k, v in pairs(inAnyFormationHeroIdList) do
    local roleInfo = nil
    if gamemanager.GetModel('game_model').IsPlayer(v) then
      roleInfo = gamemanager.GetModel('game_model').playerInfo
    else
      roleInfo = gamemanager.GetModel('hero_model').GetHeroInfo(v)
    end
    
    if roleInfo ~= nil and gamemanager.GetModel('equip_model').HasFitEquipment(roleInfo, nil) then
      return true
    end 
  end
  return false
end

function t.SetAdditionFormationAttrActive(formationId,active)
  local formationInfo = t.GetFormationInfo(formationId)
  formationInfo.isActiveAdditionAttr = active
end
--power
function t.FormationPower(formationId)
  if t.formationInfoTable[formationId] == nil then
    return 0
  end
  return t.formationInfoTable[formationId]:Power()
end
--
function t.AddAllUnlockFormation(lineupProtoDataList)
  if table.count(t.formationInfoTable) == 0 then
    for k,v in pairs(formation_data.data) do
      t.formationInfoTable[k] = formation_Info.New(v.id,1,FormationState.Locked)
    end
  end
  
  for k,v in ipairs(lineupProtoDataList) do
    local formationInfo = t.formationInfoTable[v.no]
    formationInfo.level = v.lv
    formationInfo.formationState = FormationState.NotInUse
    formationInfo.isActiveAdditionAttr = v.attrIsActive
  end
end
function t.SetFormationInfo(id,level,formationState)

  local formationInfo = t.formationInfoTable[id]
  formationInfo.level = level
  formationInfo.formationState = formationState
  
end
--
function t.AddAllTeamData(TeamProtoDataList)
  t.formationTeamTable = {}
  for k,v in ipairs(TeamProtoDataList) do
    t.AddTeamData(v)
  end
end

function t.AddTeamData(teamProtoData)
  local teamPosTable = {}
  local s = 'temPos:'
  for k, v in ipairs(teamProtoData.posList) do
    teamPosTable[v.posIndex] = v.heroId
    s= s..v.posIndex..'   '..v.heroId..' ; '
  end
  local teamInfo = require('ui/formation/model/formation_team_info')
  t.formationTeamTable[teamProtoData.teamNo] = teamInfo.New(teamProtoData.teamNo,teamProtoData.lineupNo,teamPosTable)
  --print('add team',teamProtoData.teamNo,teamProtoData.lineupNo,s)
end

function t.TransferPlayer(newPlayerInstanceID)
  for k,v in pairs(t.formationTeamTable) do
    v:TransferPlayer(newPlayerInstanceID)
  end
end
--当前选择的pve阵型
function t.SetCurrentPVESelectFormationTeamType(formationTeamType)
  t.CurrentPVESelectFormationTeamType = formationTeamType
end
--购买培养点价格
function t.GetTrainPointPurcase(purcaseTimes)
  local global_data = gamemanager.GetData('global_data')
  return global_data.for_coin_buy_a*(purcaseTimes+1)+global_data.for_coin_buy_b
end
--购买培养点上限
function t.TrainPointPurcasedMaxCount()
  return gamemanager.GetModel('vip_model').vipData.formation_add
end

function t.UpdateTrainPointByProtocol(trainPoint,recoverLimit,recoverTime)
  t.trainPoint = trainPoint
  t.trainPointRecoverMax = recoverLimit
  t.trainPointNextRecoverTime = recoverTime
  if t.trainPointNextRecoverTime == -1 then--回满
    t.trainPointNextRecoverCountDown = -1
    isFormationPointRecoverFullTip = true
  else
    t.trainPointNextRecoverCountDown = math.floor(math.max(t.trainPointNextRecoverTime - (TimeController.instance.ServerTimeTicksSecond -1)* 1000,0))
  end
  t.onRecoverTimeUpdateDelegateHandler()
end

function t.UpdateCoroutine()
  while (true) do
    coroutine.wait(1)
    --print('trainPointNextRecoverTime---------------------',t.trainPointNextRecoverTime,t.trainPoint ,t.trainPointRecoverMax)
    if t.trainPointNextRecoverTime ~= -1 then
      serverTime = TimeController.instance.ServerTimeTicksSecond
      if t.trainPoint < t.trainPointRecoverMax then
        if t.trainPointNextRecoverCountDown > 0 then
          t.trainPointNextRecoverCountDown = math.floor(math.max(t.trainPointNextRecoverTime - serverTime* 1000,0))
          if t.trainPointNextRecoverCountDown<= 0 then
            t.trainPointNextRecoverCountDown = 0
            gamemanager.GetCtrl('formation_controller').LineupPointSynReq()
          end
        end
      end
      t.onRecoverTimeUpdateDelegateHandler()
    end
    
    
  end
end
---------------------红点提示-------------------------
t.newFormationListByRedPoint = {}
function t.AddNewFormationTip(id)
  if not t.newFormationListByRedPoint[id] then
    t.newFormationListByRedPoint[id] = id
  end
end
function t.ClearNewFormationTip()
  t.newFormationListByRedPoint = {}
  isFormationPointRecoverFullTip = false
  gamemanager.GetModel('red_point_model').RefreshSpecific(RedPointType.RedPoint_Formation_specific)
  gamemanager.GetModel('red_point_model').RefreshSpecific(RedPointType.RedPoint_Formation)

end
function t.RemoveNewFormationTip(id)
  t.newFormationListByRedPoint[id] = nil
  gamemanager.GetModel('red_point_model').RefreshSpecific(RedPointType.RedPoint_Formation_specific)
  gamemanager.GetModel('red_point_model').RefreshSpecific(RedPointType.RedPoint_Formation)

end

-----单个阵型红点提示
function t.GetNewFormationTipByRedPoint(id)
  if not gamemanager.GetModel('function_open_model').IsFunctionOpen(FunctionOpenType.FormationTraining,false) then
    return false
  end
  return t.newFormationListByRedPoint[id] ~= nil  
end
--//有新阵型and培养点满
function t.GetNewFormationAndPointFullTipByRedPoint()
  if not gamemanager.GetModel('function_open_model').IsFunctionOpen(FunctionOpenType.FormationTraining,false) then
    return false
  end
  local item_model = gamemanager.GetModel('item_model')
  local game_model = gamemanager.GetModel('game_model')
  if table.count(t.newFormationListByRedPoint) > 0 or isFormationPointRecoverFullTip then
    for k,v in pairs(t.formationInfoTable) do
      if v.formationState ~= FormationState.Locked then
        local resData = v:UpgradeResCost()
        local ownCount = 0
        if resData.type == BaseResType.Item then
          ownCount = item_model.GetItemCountByItemID(resData.id)
        else
          ownCount = game_model.GetBaseResourceValue(resData.type)
        end
        if ownCount >= resData.count and t.trainPoint >= v:UpgradeTrainPointCost() then
          return true
        end
      end
    end
  end
  return false
end
-----------------------update deleagate --------------
function t.FormationChangeDelegateHandler()
  if t.FormationChangeDelegate ~=nil then
    t.FormationChangeDelegate:Invoke()
  end
  Observers.Facade.Instance:SendNotification('InitAllTeamAndFormationFromLua')
  Observers.Facade.Instance:SendNotification('OnFormationChange')
end
function t.FormationUpgradeDelegateHandler()
  t.FormationUpgradeDelegate:Invoke()
end
function t.FormationBuyTrainPointDelegateHandler()
  t.FormationBuyTrainPointDelegate:Invoke()
end
function t.onRecoverTimeUpdateDelegateHandler()
  if t.onRecoverTimeUpdateDelegate then
    t.onRecoverTimeUpdateDelegate:Invoke()
  end

end

Start()
return t