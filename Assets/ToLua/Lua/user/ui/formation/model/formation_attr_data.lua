local t = {}
t.data = {}
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
  o.type = tonumber(table.type)--1基础属性 2花钱激活属性
	o.comat_a = tonumber(table.comat_a)
	o.comat_b = tonumber(table.comat_b)
	o.time = tonumber(table.time)
	o.count = tonumber(table.counts)
	o.interval = tonumber(table.interval)
	o.targetType = tonumber(table.target)
	o.effectType = tonumber(table.effect_type)
	o.effect_base = tonumber(table.effect_base)
	o.effect_upgrade = tonumber(table.effect_upgrade)
	o.unlock_lv = tonumber(table.unlock_lv)
	return o
end
function item:GetEffectAttrValue(level)
  if self.type == 1 then
    return self.effect_base + (level - 1) * self.effect_upgrade
  else
    return self.effect_base
  end
  
end

function t.GetDataById(id)
	return t.data[id]
end
function t.GetFormationDatasByFormationId(formationId)
  local result = {}
  local index = 1
  for k,v in ipairs(t.data) do
    if v.id == formationId then
      result[index] = v
      index = index + 1
    end
  end
  return result
end

function t.GetFormationDatas(formationid,level)
  local result = {}
  local index = 1
  for k,v in ipairs(t.data) do
    if v.id == formationid and v.unlock_lv <= level then
      result[index] = v
      index = index + 1
    end
  end
  return result
end


local function Start()
	local origin = dofile('formation_attr')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t