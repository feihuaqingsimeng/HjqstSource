local t = {}
local name = 'player_model'

t.playerName = ''
t.playerInfoTable = {}
--talent
t.playerSkillTalentInfoTable = {}--playerid,{id,skillTalentInfo}

t.OnPlayerInfoUpdateDelegate = void_delegate.New()
t.OnPlayerActivateProfessionSuccessDelegate = void_delegate.New()
t.OnPlayerBreakthroughSuccessDelegate = void_delegate.New()
t.OnPlayerAggrSuccessDelegate = void_delegate.New()
t.OnPlayerAdvanceSuccessDelegate = void_delegate.New()

local function Start()
  gamemanager.RegisterModel(name,t)
end

function t.IsPlayerInstanceID (roleInstanceID)
  for k, v in pairs(t.playerInfoTable) do
    if v.instanceID == roleInstanceID then
      return true
    end
  end
  return false
end

function t.IsPlayerUnlocked(playerDataId)
  local count = table.count(t.playerInfoTable)
  if count == 0 then return false end
  for k,v in pairs(t.playerInfoTable) do
    if v.playerData.id == playerDataId then 
      return true
    end
  end
  return false
end

function t.GetPlayerDataCorrespondingPlayerInfo(playerDataId)
  for k,v in pairs(t.playerInfoTable) do
    if v.playerData.id == playerDataId then 
      return v
    end
  end
  return nil
end

function t.AddPlayer(playerInfo)
  print('add player.........',playerInfo.instanceID,playerInfo.name)
  t.playerInfoTable[playerInfo.instanceID] = playerInfo
end

function t.GetPlayerInfo (playerInstanceID)
  return t.playerInfoTable[playerInstanceID]
end

function t.GetPlayerInfoByModelId(playerModelId)
  for k,v in pairs(t.playerInfoTable) do
    if  v.playerData.id == playerModelId then 
      return v
    end
  end
  return nil
end

function t.InitPlayerSkillTalentInfoTable()
  if table.count(t.playerSkillTalentInfoTable) ~= 0 then
    return 
  end
  local pet_talent_data = gamemanager.GetData('pet_talent_data')
  for k,v in pairs(pet_talent_data) do
    if type(k) == 'number' then
      if t.playerSkillTalentInfoTable[v.pet_id] == nil then
        t.playerSkillTalentInfoTable[v.pet_id] = {}
      end
      local playerTalent = t.playerSkillTalentInfoTable[v.pet_id]
      local skill_talent_info = require('ui/player/model/player_skill_talent_info')
      playerTalent[k] = skill_talent_info.New(k,0,0)
    end
  end
end
--更新天赋
function t.UpdateSkillTalent(playerModelId,talentProtoList,selectIdList)
  t.InitPlayerSkillTalentInfoTable()
  
  local playerTalent = t.playerSkillTalentInfoTable[playerModelId]
  if playerTalent == nil then
    return
  end
  
  if talentProtoList ~= nil then
    for k ,v in ipairs(talentProtoList) do
      if playerTalent[v.no] ~= nil then
        playerTalent[v.no]:Set(v.no,v.lv,v.exp)
        --print('talent active...................',v.no,v.lv,v.exp)
      end
    end
  end
  
  if selectIdList ~= nil then
    for k ,v in pairs(playerTalent) do
      v:SetCarry(false)
    end
    for k,v in ipairs(selectIdList) do
      if playerTalent[v] ~= nil then
        playerTalent[v]:SetCarry(true)
        --print('talent select............................',v)
      end
    end
    
  end
  --通过id获得天赋
  function t.GetSkillTalentInfo(id)
    for k,v in pairs(t.playerSkillTalentInfoTable) do
      for k1,v1 in pairs(v) do
        if k1 == id then
          return v1
        end
      end
    end
    return nil
  end
  ---update player info talent--------------
  local playerInfo = t.GetPlayerInfoByModelId(playerModelId)
  if playerInfo ~= nil then
    local activeNormalTalentDic = {}
    local passiveId = 0
    local passiveLevel = 0
    local summonEffectId = 0
    local summonEffectLevel = 0
    for k,v in pairs(playerTalent) do
      if v.level >= 1 and v.talentData.groupType == 0 then --passiveNormal
        activeNormalTalentDic[v.talentData.effect] = v.level
      end
      if v.talentData.groupType == 1 and v:IsCarry() then --PassiveThreeChoiceOne
        passiveId = v.talentData.effect
        passiveLevel = v.level
      elseif v.talentData.groupType == 2 and v:IsCarry() then --SummonThreeChoiceOne
        summonEffectId = v.talentData.effect
        summonEffectLevel = v.level
      end
    end
    playerInfo:UpdateSelectSkillTalent(passiveId, passiveLevel, summonEffectId, summonEffectLevel);
    playerInfo:UpdateNormalActiveSkillTalent(activeNormalTalentDic);
    print('talent player info ...........................',passiveId, passiveLevel, summonEffectId, summonEffectLevel)
  end 
end

function t.UpdatePlayerEquipments (playerInstanceID, equipmentIDs)  
  local heroModel = gamemanager.GetModel('hero_model')
  heroModel.UpdateHeroEquipments(true, playerInstanceID, equipmentIDs)
end


function t.OnPlayerInfoUpdateByProtocol()
  t.OnPlayerInfoUpdateDelegate:Invoke()
  gamemanager.GetModel('hero_model').CheckHasAdvanceBreakthroughHeroByRedPoint()
end

function t.OnPlayerActivateProfessionSucess ()
  t.OnPlayerActivateProfessionSuccessDelegate:Invoke()
end

function t.OnPlayerBreakthroughSuccess (roleInstanceID)
  t.OnPlayerBreakthroughSuccessDelegate:InvokeOneParam(roleInstanceID)
end

function t.OnPlayerAggrSuccess (isCrit)
  t.OnPlayerAggrSuccessDelegate:InvokeOneParam(isCrit)
end

function t.OnPlayerAdvanceSuccess(playerInstanceID)
  t.OnPlayerAdvanceSuccessDelegate:InvokeOneParam(playerInstanceID)
end
-----------------------------update by protocol--------------------
Start()
return t
