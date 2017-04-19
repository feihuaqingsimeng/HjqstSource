local t = {}
t.data = {}
local item = {}
item.__index = item

local game_res_data = require('ui/game/model/game_res_data')

function item.New(table)
	local o = {}
	setmetatable(o,item)
  
	o.id = tonumber(table.id)
	o.name = table.name
	o.des = table.des
  o.pos = {}
	o.pos[1] = tonumber(table.p1) ~= 0
	o.pos[2] = tonumber(table.p2) ~= 0
	o.pos[3] = tonumber(table.p3) ~= 0
	o.pos[4] = tonumber(table.p4) ~= 0
	o.pos[5] = tonumber(table.p5) ~= 0
	o.pos[6] = tonumber(table.p6) ~= 0
	o.pos[7] = tonumber(table.p7) ~= 0
	o.pos[8] = tonumber(table.p8) ~= 0
	o.pos[9] = tonumber(table.p9) ~= 0
	o.max_lv = tonumber(table.max_lv)
	o.upgrade_cost = game_res_data.NewByString(table.upgrade_cost)
	o.upgrade_formula_a = tonumber(table.upgrade_formula_a)
	o.upgrade_formula_b = tonumber(table.upgrade_formula_b)
	o.upgrade_formula_c = tonumber(table.upgrade_formula_c)
	o.upgrade_base_cost_a = tonumber(table.upgrade_base_cost_a)
	o.upgrade_base_cost_b = tonumber(table.upgrade_base_cost_b)
  o.formationConditionIdsList = {}
	local condition = string.split(table.formation_condition,';')
  for k, v in pairs(condition) do
    o.formationConditionIdsList[k] = tonumber(v)
  end
  o.unlock_cost = game_res_data.NewByString(table.unlock_cost)
	return o
end

function item:GetPosEnable(index)
  if index<= 0 or index > #self.pos then 
    return false
  end
  return self.pos[index]
  
end
function item:GetAllEnabledPosList()
  local p = {}
  local index = 1
  for k,v in pairs(self.pos) do
    if v then
      p[index] = k
      index = index + 1
    end
  end
  return p
end

function t.GetDataById(id)
	return t.data[id]
end


local function Start()
	local origin = dofile('formation')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t