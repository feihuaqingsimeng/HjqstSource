local role_info = require('ui/role/model/role_info')
local hero_info = role_info.extend()

hero_info.__index = hero_info

function hero_info:New( instanceID,  heroDataID,  breakthroughLevel,  strengthenLevel,  advanceLevel,  level)
  local o = {}
  setmetatable(o, hero_info)
  o:Init()
  o:Set(instanceID,  heroDataID,  breakthroughLevel,  strengthenLevel,  advanceLevel,  level)
  return o
end
function hero_info:NewByGameResData (gameResData)
  local instanceID = 0
  local heroDataID = gameResData.id
  local breakthroughLevel = 0
  local strengthenLevel = 0
  local advanceLevel = gameResData.star
  local level = 1
  
  return hero_info:New(instanceID,  heroDataID,  breakthroughLevel,  strengthenLevel,  advanceLevel,  level)
end
function hero_info:NewByHeroProtoData(heroProtoData)
  local o = {}
  setmetatable(o, hero_info)
  o:Init()
  o:SetHeroInfo(heroProtoData)
  return o
end
function hero_info.NewByDrawCardDropProto(drawDropCardProto)
  local o = {}
  setmetatable(o, hero_info)
  o:Init()
  o:Set(drawDropCardProto.id,drawDropCardProto.no,0,0,drawDropCardProto.star,1)
  return o
end

function hero_info:NewByTeamHeroProtoData(data)
  local o = {}
  setmetatable(o, hero_info)
  o:Init()
  if data.lv ~= 1 then
    self.level = data.lv
    
  end
  if data.star ~= 0 then
    self.advanceLevel = data.star
  end
  if data.aggrLv ~= -1 then
    self.strengthenLevel = data.aggrLv
  end
  if data.breakLayer ~= 0 then
    self.breakthroughLevel = data.breakLayer
  end
  return o
end

function hero_info:Clone()
  return self:GetHeroInfoCopy()
end

function hero_info:GetHeroInfoCopy()
  local info = hero_info:New(self.instanceID, self.heroData.id,  self.breakthroughLevel,self.strengthenLevel,self.advanceLevel,self.level)
  info.exp = self.exp
  info.strengthenExp = self.strengthenExp
  if self.relations then
    info.relations = {}
    for k,v in ipairs(self.relations) do
      info.relations[k] = v
    end
  end
  return info
end

function hero_info:Init()
  self.instanceID = 0
  self.modelId = 0
  self.breakLayer = 1
  self.star = 0
  self.exp = 0
  self.level = 1
  self.strengthenLevel = 0
  self.strengthenExp = 0
  self.relations = {}
  self.myPower = 0
  self.needUpdatePower = true
  self.isLocked = false
end

function hero_info:Set(instanceID,  heroDataID,  breakthroughLevel,  strengthenLevel,  advanceLevel,  level)
  self.instanceID = instanceID
  local hd = gamemanager.GetData('hero_data')  
  self.heroData = hd.GetDataById(heroDataID)
  if(self.heroData == nil) then
    print("these is no heroId:"..heroDataID)
  end
  self.breakthroughLevel = breakthroughLevel
  self.strengthenLevel = strengthenLevel
  self.advanceLevel = advanceLevel
  self.level = level
  self.needUpdatePower = true
end

function hero_info:SetHeroInfo(heroProtoData)
  self.instanceID = heroProtoData.id
  self.modelId=heroProtoData.modelId
  if heroProtoData.modelId ~= 0 then
    local hd = gamemanager.GetData('hero_data')
    self.heroData = hd.GetDataById(heroProtoData.modelId)
    if(self.heroData == nil) then
      print("these is no heroId:"..heroProtoData.modelId)
    end
  end
  if heroProtoData.breakLayer ~= 0 then 
    self.breakthroughLevel = heroProtoData.breakLayer
  end
  
  if heroProtoData.star ~= 0 then 
    self.advanceLevel = heroProtoData.star
  end
  
  if heroProtoData.exp ~= -1 then
    self.exp = heroProtoData.exp
  end
  if heroProtoData.lv ~= 0 then 
    self.level = heroProtoData.lv
  end
  --print('update hero strengthen','lv:'..self.strengthenLevel,'exp:'..self.strengthenExp,'resp.lv:'..heroProtoData.aggrLv,'resp.exp:'..heroProtoData.aggrExp)
  
  if heroProtoData.aggrLv ~= -1 then
    self.strengthenLevel = heroProtoData.aggrLv
  end
  
  if heroProtoData.aggrExp ~= -1 then
    self.strengthenExp = heroProtoData.aggrExp
  end
  --HeroRelationProto
  if heroProtoData.isUpRelations then  
    self.relations = {}
    for k,v in ipairs(heroProtoData.relations) do
      self.relations[k] = v
    end
  end
  self.needUpdatePower = true
end

function hero_info:IsActiveRelation(relationId)
  for k,v in ipairs(self.relations) do
    if v.id == relationId then
      return true
    end
  end
  return false
end
--英雄自身战力（不包括穿的装备）
function hero_info:GetMyPower()
  if self.needUpdatePower then
    self.needUpdatePower = false
    self.myPower = self:Power()
  end
  return self.myPower
end

return hero_info