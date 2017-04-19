local t = {}
t.__index = t

local hero_info = require('ui/hero/model/hero_info')
local player_info = require('ui/player/model/player_info')
local equip_info = require('ui/equip/model/equip_info')

function t:Reset()
  self.id = 0
  self.roleInfo = nil
  self.posIndex = -1
  --装备类型(武器，防具等)作为key，便于查找
  self.equipInfoDictionary = Dictionary.New('number','equipInfo')
end
-- 英雄  [参考 好友查看协议]
function t.NewByRoleHeroProtoData(data)
  local o = {}
  setmetatable(o,t)
  o:Reset()
  o.id = data.id
  o.roleInfo = hero_info:New( data.id,  data.heroNo,  data.breakLayer,  data.aggrLv,  data.star,  data.lv)
  if data.breakLayer == 0 then
    o.roleInfo.breakthroughLevel = 1
  end
  o.posIndex = data.posIndex
  
  --print('[role check info] hero:','id',o.id,'playerNo',data.heroNo,'level',data.lv,'advanceLevel',data.lv,'aggrLv',data.aggrLv,'breakLayer',data.breakLayer,'posIndex',data.posIndex)
  
  
  for k,v in ipairs(data.equips) do
    local equipInfo = equip_info:NewByEquip(v)
    o.equipInfoDictionary:Add(equipInfo.data.equipmentType,equipInfo)
  end
    
  return o
end
-- 主角  [参考 好友查看协议]
function t.NewByRolePlayerProtoData(data,hairCutIndex,  hairColorIndex, faceIndex,skinIndex)
  local o = {}
  setmetatable(o,t)
  o:Reset()
  o.id = data.id
  o.roleInfo = player_info:New( data.id,  data.playerNo, hairCutIndex,  hairColorIndex, faceIndex, '', skinIndex)
  o.roleInfo.level = data.lv
  o.roleInfo.advanceLevel = data.star
  o.roleInfo.strengthenLevel = data.aggrLv
  o.roleInfo.breakthroughLevel = data.breakLayer
  if data.breakLayer == 0 then
    o.roleInfo.breakthroughLevel = 1
  end
  o.posIndex = data.posIndex
  
  --print('[role check info] player:','id',o.id,'playerNo',data.playerNo,'level',data.lv,'advanceLevel',data.lv,'aggrLv',data.aggrLv,'breakLayer',data.breakLayer,'posIndex',data.posIndex)
  
  for k,v in ipairs(data.equips) do
    local equipInfo = equip_info:NewByEquip(v)
    o.equipInfoDictionary:Add(equipInfo.data.equipmentType,equipInfo)
  end
  return o
end

function t:Power()
  local power = self.roleInfo:Power()
  for k,v in pairs(self.equipInfoDictionary:GetDatas()) do
    power = power + v:Power()
  end
  return power
end

function t:IsPlayer()
  return self.roleInfo.heroData:IsPlayer()
end

function t:GetWeaponEquip()
  local weapon = self.equipInfoDictionary:Get(EquipmentType.PhysicalWeapon)
  if weapon then
    return weapon
  end
  return self.equipInfoDictionary:Get(EquipmentType.MagicalWeapon)
end

function t:GetArmorEquip()
  return self.equipInfoDictionary:Get(EquipmentType.Armor)
end

function t:GetAccessoryEquip()
  return self.equipInfoDictionary:Get(EquipmentType.Accessory)
end

return t