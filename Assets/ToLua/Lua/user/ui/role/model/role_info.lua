local role_info = 
{
  instanceID = 0,
  level = 1,
  exp = 0,
  advanceLevel = 0,
  strengthenLevel = 0,
  strengthenExp = 0,
  breakthroughLevel = 1,
  weaponID = 0,
  armorID = 0,
  accessoryID = 0,
}

role_info.__index = role_info

local breakthrough_data = gamemanager.GetData('breakthrough_data')
local hero_exp_data = gamemanager.GetData('hero_exp_data')
local hero_strengthen_need_data = gamemanager.GetData('hero_strengthen_need_data')
function role_info.New()
  local o = {}
  setmetatable(o,role_info)
  
  return o
end

function role_info.extend()
    return role_info.New()
end
--模型名
function role_info:ModelName()
  return self.heroData.modelNames[self.advanceLevel]
end
--头像icon
function role_info:HeadIcon()
  return 'sprite/head_icon/'..self.heroData.headIcons[self.advanceLevel]
end
--角色类型icon
function role_info:RoleTypeIconSprite ()
  local ui_util = require('util/ui_util')
  return ui_util.GetRoleTypeSmallIconSprite(self.heroData.roleType)
end
--等级对应的经验比例（最大为1）
function role_info:ExpPercent()
  local data = hero_exp_data.GetDataById(self.level)
  if data ~= nil then
    local p =  self.exp/data.exp
    if p > 1 then
      --p = 1
    end
    return p
  end
  return 0
end
--等级对应的总经验
function role_info:ExpTotal()
  if self.level <= 1 then
    return self.exp
  else
    local data = hero_exp_data.GetDataById(self.level-1)
    print('ExpTotal',data.exp_total,self.exp)
    return data.exp_total + self.exp
  end
end
--强化等级对应的经验比例(最大为1)
function role_info:StrengthenExpPercent()
  local data = hero_strengthen_need_data.GetDataById(self.strengthenLevel)
   if data ~= nil then
    return self.strengthenExp/data.exp_need
  end
  return 0
end
--强化等级对应的总经验
function role_info:StrengthenExpTotal()
  if self.strengthenLevel == 0 then
    return self.strengthenExp
  else
    local data = hero_strengthen_need_data.GetDataById(self.strengthenLevel-1)
    return data.exp_total + self.strengthenExp
  end
end

--最大等级
function role_info:MaxLevel()
  return gamemanager.GetModel('game_model').accountLevel
end
--最大星级
function role_info:MaxAdvanceLevel()
    return self.heroData.starMax
end
--攻击类型（物理攻击，魔法攻击）RoleAttackAttributeType
function role_info:GetRoleAttackAttributeType()
  return gamemanager.GetModel('role_model').GetRoleAttackAttributeType(self.heroData.roleType)
end
--强化阶数
function role_info:RoleStrengthenStage()
  local stage = 0
  local strengthenNeedData = gamemanager.GetData('hero_strengthen_need_data').GetDataById(self.strengthenLevel-1)
  if strengthenNeedData ~= nil then
    stage = strengthenNeedData.color
  end
  return stage
end
--强化+几
function role_info:GetStrengthenAddShowValue()
  local strengthenLevelShow = 0
  local strengthenNeedData = gamemanager.GetData('hero_strengthen_need_data').GetDataById(self.strengthenLevel-1)
  if strengthenNeedData ~= nil then
    strengthenLevelShow = strengthenNeedData.strengthenAddShowValue
  end
  return strengthenLevelShow
end
--是否穿装备
function role_info:IsWearEquipment()
  return self.weaponID > 0 or self.armorID > 0 or self.accessoryID > 0
end
--战力
function role_info:Power()
  return gamemanager.GetModel('role_model').CalcRolePower(self)
end

function role_info:EquipmentsTotalPower ()
  local equip_model = gamemanager.GetModel('equip_model')
  local equipmentsTotalPower = 0
  if self.weaponID > 0 then
    local weaponInfo = equip_model.GetEquipmentInfoByInstanceID (self.weaponID)
    equipmentsTotalPower = equipmentsTotalPower + weaponInfo:Power()
  end
  if self.armorID > 0 then
    local armorInfo = equip_model.GetEquipmentInfoByInstanceID (self.armorID)
    equipmentsTotalPower = equipmentsTotalPower + armorInfo:Power()
  end
  if self.accessoryID > 0 then
    local accessoryInfo = equip_model.GetEquipmentInfoByInstanceID (self.accessoryID)
    equipmentsTotalPower = equipmentsTotalPower + accessoryInfo:Power()
  end
  return equipmentsTotalPower
end

function role_info:PowerIncludeEquipments ()
  return self:Power() + self:EquipmentsTotalPower()
end

function role_info:CanBeUsedAsMaterial ()
  local formation_model = gamemanager.GetModel('formation_model')
  local hero_model = gamemanager.GetModel('hero_model')
  if self.isLocked or formation_model.IsHeroInAnyTeam(self.instanceID) or hero_model.IsHeroInRelations(self.instanceID) or hero_model.IsHeroInExploring(self.instanceID) then
    return false
  end
  return true
end

return role_info