local t = {}
local item = {}
item.__index = item

local game_res_data = require('ui/game/model/game_res_data')
local equip_attr = require('ui/equip/model/equip_attr')

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.name = table.name
	o.icon = table.icon
	o.equipmentType = tonumber(table.type) --装备类型 1武器 2防具 3饰品
	o.equipmentRoleType = tonumber(table.career)--穿戴职业（攻击型等）
	o.description = table.description
	o.quality = tonumber(table.quality)
	o.useLv = 0
	o.price = tonumber(table.price)
	o.isAdvance = 0
	o.next_id = 0
	o.special_hero=tonumber(table.special_hero)
  o.strengthen_type = tonumber(table.strengthen_type)
  --基础属性
  o.baseAttrIdList = {}
  local idList
  if table.base_attr_id then
    o.baseAttrIdList = string.split2number( table.base_attr_id,';')
  end
  
  --随机属性id
  o.randomAttrIdList = {}
  if table.random_attr_id then
     o.randomAttrIdList = string.split2number( table.random_attr_id,';')
  end
  
  --专属属性
  o.specialAttrIdList = {}
  if table.special_attr_id then
    o.specialAttrIdList = string.split2number(table.special_attr_id,';')
  end
  
  
  if table.unload_diamond == '0' then
    o.unloadCostGRD = game_res_data.New(BaseResType.Diamond,0,0,0)
  else
    o.unloadCostGRD = game_res_data.NewByString(table.unload_diamond)
  end
  
  o.recastMaterialGRD = nil
  if table.recast_material ~= '0' then
    o.recastMaterialGRD = game_res_data.NewByString(table.recast_material)
  end
  --宝石槽 -1代表锁住
  local gem_type = string.split2number(table.gen_type,';')
  local gem_lock = string.split2number(table.gen_lock,';')
  for k,v in pairs(gem_lock) do
    if v == -1 then
      gem_type[k] = -1
    end
  end
  o.gem = gem_type
	return o
end

-- return  id
function item:GetFirstBaseIdAttr()
  for k,v in pairs(self.baseAttrIdList) do
    return v
  end
  return nil
end
-- return  equip_attr
function item:GetFirstBaseAttr()
  local id = 0
  for k,v in pairs(self.baseAttrIdList) do
    id = v
    break
  end
  return equip_attr.NewByEquipAttrDataId(id)
end
-- return  id
function item:GetFirstSpecialIdAttr()
  for k,v in pairs(self.specialAttrIdList) do
    return v
  end
  return nil
end

function item:IconPath()
  return 'sprite/equipment_icon/'..self.icon
end

function item:RoleTypeIconSprite ()
  local ui_util = require('util/ui_util')
  return ui_util.GetRoleTypeSmallIconSprite(self.equipmentRoleType)
end

function item:QualityName()
  if self.quality == 1 then
    return LocalizationController.instance:Get('common.color_White')
  elseif self.quality == 2 then
    return LocalizationController.instance:Get('common.color_Green')
  elseif self.quality == 3 then
    return LocalizationController.instance:Get('common.color_Blue')
  elseif self.quality == 4 then
    return LocalizationController.instance:Get('common.color_Purple')
  elseif self.quality == 5 then
    return LocalizationController.instance:Get('common.color_Orange')
  elseif self.quality == 6 then
    return LocalizationController.instance:Get('common.color_Red')
  end
  return 'none'
end


function t.GetDataById(id)
  local data = t[id]
  if data == nil then
    print(ui_util.FormatToRedText('equip data is nil , id:'..id))
  end
	return data
end

local function Start()
	local origin = dofile('equip')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t[id] = newItem
	end)
end

Start()
return t