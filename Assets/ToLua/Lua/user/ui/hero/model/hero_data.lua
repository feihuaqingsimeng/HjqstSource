local t = {}
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.hero_type = tonumber(table.hero_type) -- 区别英雄、主角、boss
	o.hitId = tonumber(table.hitId)
	o.skillId1 = tonumber(table.skillId1)
	o.skillId2 = tonumber(table.skillId2)
	o.passiveId1 = tonumber(table.passiveId1)
	o.passiveId2 = tonumber(table.passiveId2)
	o.name = table.name
	o.description = table.description
  o.modelNames = string.split(table.model,';')
  
	o.height = tonumber(table.height)
	o.shadows = tonumber(table.shadows)
  local rot = string.split2number(table.home_rotation,';')
	o.home_rotation = Vector3(rot[1],rot[2],rot[3])
	o.rotation = table.rotation
	o.scale = table.scale
	o.headIcons = string.split(table.headIcon,';')
	o.expMax = tonumber(table.expMax)
	o.expMax50 = tonumber(table.expMax50)
	o.hp = tonumber(table.hp)
	o.hp_add = tonumber(table.hp_add)
	o.normal_atk = tonumber(table.normal_atk)
	o.normal_atk_add = tonumber(table.normal_atk_add)
	o.magic_atk = tonumber(table.magic_atk)
	o.magic_atk_add = tonumber(table.magic_atk_add)
	o.normal_def = tonumber(table.normal_def)
	o.normal_def_add = tonumber(table.normal_def_add)
	o.magic_def = tonumber(table.magic_def)
	o.magic_def_add = tonumber(table.magic_def_add)
	o.speed = tonumber(table.speed)
	o.hit = tonumber(table.hit)
	o.dodge = tonumber(table.dodge)
	o.crit = tonumber(table.crit)
	o.anti_crit = tonumber(table.anti_crit)
	o.block = tonumber(table.block)
	o.anti_block = tonumber(table.anti_block)
	o.counter_atk = tonumber(table.counter_atk)
	o.crit_hurt_add = tonumber(table.crit_hurt_add)
	o.crit_hurt_dec = tonumber(table.crit_hurt_dec)
	o.armor = tonumber(table.armor)
	o.damage_dec = table.damage_dec
	o.damage_add = table.damage_add
	o.starMin = tonumber(table.starMin)
	o.starMax = tonumber(table.starMax)
	o.correction = tonumber(table.correction)
	o.roleType = tonumber(table.type)
	o.weaponType = tonumber(table.weaponType)
	o.floatable = tonumber(table.floatable)
  
  local heroBaseEquipIDStrs = string.split(table.hero_equment, ';')
  o.basicWeaponID = tonumber(heroBaseEquipIDStrs[1])
  o.basicArmorID = tonumber(heroBaseEquipIDStrs[2])
  o.basicAccessoryID = tonumber(heroBaseEquipIDStrs[3])
  
  o.quality = tonumber(table.quality)
  o.quality_attr = tonumber(table.quality_attr)
  o.quality_icon = table.quality_icon
	return o
end
function item:IsPlayer()
  return self.hero_type == 2
end

function item:GetNameWithQualityColor ()
  return ui_util.FormatStringWithinQualityColor(self.quality, LocalizationController.instance:Get(self.name))
end

function item:GetRankImagePath ()
  return 'sprite/main_ui/'..self.quality_icon
end

function item:GetRankRoleTypeSprite ()
  local ui_util = require('util/ui_util')
  return ui_util.GetRankRoleTypeSprite (self.roleType)
end

function t.GetDataById(id)
	return t[id]
end



local function Start()
	local origin = dofile('hero')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t[id] = newItem
	end)
end

Start()
return t