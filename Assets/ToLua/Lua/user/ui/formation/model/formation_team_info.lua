--队伍
local formation_team_info = {}
 formation_team_info.__index = formation_team_info
 
function formation_team_info.New (teamType, formationId, teamPosTable)
  local o = {}
 
  setmetatable(o, formation_team_info)
  
  o.teamType = teamType
  o:SetFormationId(formationId)
  o.teamPosTable = teamPosTable -- position,id
  return o
end

function formation_team_info:ClearTeam ()
  self.teamPosTable = {}
end
--换阵型啦
function formation_team_info:ChangeFormationPosByFormationId(formationId)
  self:SetFormationId(formationId)
  local formationPosList = self.formationInfo.formationData:GetAllEnabledPosList()

  local tempIdList = {}
  local index = 1
  for k,v in pairs(self.teamPosTable) do
    tempIdList[index] = v
    index = index + 1
  end
  self:ClearTeam()
  for k,v in pairs(tempIdList) do
    self.teamPosTable[formationPosList[k]] = v
  end
end
function formation_team_info:SetFormationId(id)
  self.formationId = id
  self.formationInfo = gamemanager.GetModel('formation_model').GetFormationInfo(id)
end
function formation_team_info:Count()
  return table.count(self.teamPosTable)
end
function formation_team_info:IsHeroInFormation (heroInstanceID)
  for k, v in pairs(self.teamPosTable) do
    if v == heroInstanceID then
      return true
    end
  end
  return false
end

function formation_team_info:GetHeroAt (formationPosition)
  local heroInstanceID = self.teamPosTable[formationPosition]
  return gamemanager.GetModel('hero_model').GetHeroInfo(heroInstanceID)
end


function formation_team_info:GetHeroFormationPosition (heroInstanceID)
  local formationPosition = 0
  for k, v in pairs(self.teamPosTable) do
    if v == heroInstanceID then
      formationPosition = k
    end
  end
  return formationPosition
end

function formation_team_info:RemoveHeroByFormationPosition (formationPosition)
  if self.teamPosTable[formationPosition] ~= nil then
    self.teamPosTable[formationPosition] = nil
    return true
  end
  return false
end

function formation_team_info:RemoveHeroByHeroInstanceID (heroInstanceID)
  local formationPosition = self:GetHeroFormationPosition(heroInstanceID)
  return self:RemoveHeroByFormationPosition(formationPosition)
end

function formation_team_info:IsPositionEmpty (formationPosition)
  if self.teamPosTable[formationPosition] ~= nil then
    return false
  end
  return true
end

function formation_team_info:HasSameHeroInFormation(heroDataID)
  local gameModel = gamemanager.GetModel('game_model')
  local heroModel = gamemanager.GetModel('hero_model')
  for k,v in pairs(self.teamPosTable) do
    if gameModel.IsPlayer(v) == false then
      local heroInfo = heroModel.GetHeroInfo(v)
      if heroInfo.heroData.id == heroDataID then 
        return true
      end
    end
  end
  return false
end

function formation_team_info:AddHeroToFormaiton (formationPosition, heroInstanceID)
  --新英雄旧位置
  local oldFormationPosition = self:GetHeroFormationPosition(heroInstanceID)
  self.teamPosTable[oldFormationPosition] = nil
  
  --新位置已有英雄
  if not self:IsPositionEmpty(formationPosition) then
    --新位置旧英雄instanceID
    local newPositionOldCharacterInstanceID = self.teamPosTable[formationPosition]
    if oldFormationPosition >= 1 then --新英雄已在队伍中
      self.teamPosTable[oldFormationPosition] = newPositionOldCharacterInstanceID
      self.teamPosTable[formationPosition] = heroInstanceID
    else --新英雄不在队伍中
      self.teamPosTable[formationPosition] = heroInstanceID
    end
  elseif table.count(self.teamPosTable) < 5 then
    self.teamPosTable[formationPosition] = heroInstanceID
  end
end
--int,int,bool
function formation_team_info:CanAddToFormationPosition(formationPosition,addCharacterInstanceID,playerCanLeaveTeam)
  local enable = self.formationInfo.formationData:GetPosEnable(formationPosition) 
  if enable == false then
    return false
  end
  
  local oldId = self.teamPosTable[formationPosition] ;
  if oldId == nil then oldId = 0 end
  local can = true
  local isPosEmpty = self:IsPositionEmpty(formationPosition)
  local  playerFlag = true
  if playerCanLeaveTeam then playerFlag = true else playerFlag = gamemanager.GetModel('game_model').IsPlayer(oldId) == false end
  --新英雄在阵中
  if self:IsHeroInFormation(addCharacterInstanceID) then
    if isPosEmpty == false and oldId == addCharacterInstanceID then
      can = false
    end
  else 
    local heroModel = gamemanager.GetModel('hero_model')
    local heroInfo = heroModel.GetHeroInfo(addCharacterInstanceID)
    
    local hasSame = true
    if gamemanager.GetModel('game_model').IsPlayer(addCharacterInstanceID) then 
      hasSame = false 
    else 
      hasSame = self:HasSameHeroInFormation(heroInfo.heroData.id) 
    end
    
    if hasSame then
      if isPosEmpty  then
        can = false
      else 
        if playerFlag and heroModel.GetHeroInfo(oldId).heroData.id == heroInfo.heroData.id then
          can = true
        else 
          can = false
        end
      end
    else
      if isPosEmpty == false and playerFlag == false then
        can = false
      else 
        if table.count(self.teamPosTable) >= 5 then
          if isPosEmpty then
            can = false 
          else 
            can = true
          end
        else 
          can = true
        end
      end
    end
  
  end
  return can
end

function formation_team_info:GetInstacneIdByPosition(position)
  local v = self.teamPosTable[position]
  if v == nil then 
    return 0 
  end
  return v
end

function formation_team_info:GetPlayerPosition()
  local gameModel = gamemanager.GetModel('game_model')
  for k,v in pairs(self.teamPosTable) do
    if gameModel.IsPlayer(v) then
      return k
    end
  end
  return 0
end

function formation_team_info:TransferPlayer (newPlayerInstanceID)
  local gameModel = gamemanager.GetModel('game_model')
  for k,v in pairs(self.teamPosTable) do
    if gameModel.IsPlayer(v) then
      self.teamPosTable[k] = newPlayerInstanceID
      break
    end
  end
end

function formation_team_info:Power()
  local power = 0
  local gameModel = gamemanager.GetModel('game_model')
  local heroModel = gamemanager.GetModel('hero_model')
  for k,v in pairs(self.teamPosTable) do
    if gameModel.IsPlayer(v) then
      power = power + math.floor(gameModel.playerInfo:Power()) + gameModel.playerInfo:EquipmentsTotalPower()
    else
      power = power + math.floor(heroModel.GetHeroInfo(v):Power()) + heroModel.GetHeroInfo(v):EquipmentsTotalPower()
    end
  end
  if self.formationInfo ~= nil then
    power = power*(self.formationInfo:Power() + 1)
  end
  print('[formation_team_info]power',power)
  return math.floor(power)
end

return formation_team_info