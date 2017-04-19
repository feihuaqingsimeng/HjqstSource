
local role_info = require('ui/role/model/role_info')
local t = role_info.extend()

function t:New(instanceID,  playerID, hairCutIndex,  hairColorIndex, faceIndex, playerName, skinIndex)
  local o = {}
  self.__index = self
  setmetatable(o,self)
  o:Init()
  o:Set(instanceID,  playerID, hairCutIndex,  hairColorIndex, faceIndex, playerName, skinIndex)
  return o
end
function t:NewByPlayerProtoData(playerProtoData)
  local o = {}
  self.__index = self
  setmetatable(o,self)
  o:Init()
  o.instanceID = playerProtoData.id
  
  o:SetByPlayerProtoData(playerProtoData)
  
  
  return o
end

function t:Clone()
  return self:GetCopyPlayerInfo()
end

function t:GetCopyPlayerInfo ()
  local copyPlayerInfo = t:New(self.instanceID, self.playerData.id, self.hairCutIndex, self.hairColorIndex, self.faceIndex, self.name, self.skinIndex)

  copyPlayerInfo.exp = self.exp
  copyPlayerInfo.level = self.level
  copyPlayerInfo.advanceLevel = self.advanceLevel
  copyPlayerInfo.strengthenLevel = self.strengthenLevel
  copyPlayerInfo.strengthenExp = self.strengthenExp
  copyPlayerInfo.breakthroughLevel = self.breakthroughLevel
  self.needUpdatePower = true
  return copyPlayerInfo
end

 function t:Init()
  self.hairCutIndex = 0
  self.hairColorIndex = 0
  self.faceIndex = 0
  self.skinIndex = 0  
-- talent skill---------
  self.passiveSkillId = 0
  self.passiveSkillLevel = 1
  self.summonEffectId = 0
  self.summonSkillLevel = 1
--talent active---effectid,level-
  self.normalPassiveSkilIdTable = {}
  self.myPower = 0
  self.needUpdatePower = true
end
  
function t:Set(skinIndex)
  self.skinIndex = skinIndex
end
  
function t:Set(instanceID,  playerID, hairCutIndex,  hairColorIndex, faceIndex, playerName, skinIndex)
  self.instanceID = instanceID
  
  self.playerData = gamemanager.GetData('player_data').GetDataById(playerID)
  self.heroData = gamemanager.GetData('hero_data').GetDataById(self.playerData.heroId)
  ---need add avatarData in here------------------
  self.hairCutIndex = hairCutIndex
  self.hairColorIndex = hairColorIndex
  self.faceIndex = faceIndex
  if(skinIndex == nil) then
    self.skinIndex = 0
  else
    self.skinIndex = skinIndex
  end
  self.name = playerName
  self.advanceLevel = self.heroData.starMin
  self.needUpdatePower = true
end

function t:SetByPlayerProtoData(playerProtoData)
  if playerProtoData.modelId ~=0 then
    self.playerData = gamemanager.GetData('player_data').GetDataById(playerProtoData.modelId)
    if self.playerData == nil then
      print('主角的playerData is null, modelId:'..playerProtoData.modelId)
    end
    self.heroData = gamemanager.GetData('hero_data').GetDataById(self.playerData.heroId)
    self.advanceLevel = self.heroData.starMin
    self.name = gamemanager.GetModel('player_model').playerName
  end
  ---need add avatarData in here------------------
  if playerProtoData.hairCutId ~= 0 then
    self.hairCutIndex = playerProtoData.hairCutId
  end
  if playerProtoData.hairColorId ~= 0 then
    self.hairColorIndex = playerProtoData.hairColorId
  end
  if playerProtoData.faceId ~= 0 then
    self.faceIndex = playerProtoData.faceId
  end
  if playerProtoData.skinId ~= 0 then
    self.skinIndex = playerProtoData.skinId
  end
  if playerProtoData.exp > 0 or self.level ~= playerProtoData.lv then
    self.exp = playerProtoData.exp
  end
  if playerProtoData.lv ~= 0 then 
    self.level = playerProtoData.lv
  end
  if playerProtoData.star ~= 0 then
    self.advanceLevel = playerProtoData.star
  end
  if playerProtoData.aggrLv ~= -1 then
    self.strengthenLevel = playerProtoData.aggrLv
  end
  if playerProtoData.aggrExp ~= -1 then
    self.strengthenExp = playerProtoData.aggrExp
  end
  if playerProtoData.breakLayer ~= 0 then
    self.breakthroughLevel = playerProtoData.breakLayer
  end
  self.needUpdatePower = true
end
-------------------update talent skill-----------------------
function t:UpdateNormalActiveSkillTalent(normalActiveTable)
  if table.count(normalActiveTable) == 0 then return end
  
  self.normalPassiveSkilIdTable = {}
  
  for k,v in pairs(normalActiveTable) do
    self.normalPassiveSkilIdTable[k] = v
  end
  
end
function t:UpdateSelectSkillTalent(passiveSkillId, passiveSkillLevel, summonId, summonSkillLevel)
  self.passiveSkillId = passiveSkillId;
  self.passiveSkillLevel = passiveSkillLevel;
  self.summonEffectId = summonId;
  self.summonSkillLevel = summonSkillLevel;
end
--------------------end-----------------------

function t:PetHeadIcon()
  local petData = gamemanager.GetData('pet_data').GetDataById(self.playerData.pet_id)
  return 'sprite/head_icon/'..petData.head_icon
end
--英雄自身战力（不包括穿的装备）
function t:GetMyPower()
  if self.needUpdatePower then
    self.needUpdatePower = false
    self.myPower = self:Power()
  end
  return self.myPower
end

return t