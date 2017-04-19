local t = {}
t.data = {}
local item = {}
item.__index = item

local game_res_data = require('ui/game/model/game_res_data')

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.rule_id = tonumber(table.rule_id)
	o.item = game_res_data.NewByString(table.item)
	o.scale = table.scale
  o.materials = {}
  local index = 1
  if table.cost1 ~= '-1' then
    o.materials[index] = game_res_data.NewByString(table.cost1)
    index = index + 1
  end
  if table.cost2 ~= '-1' then
    o.materials[index] = game_res_data.NewByString(table.cost2)
    index = index + 1
  end
  if table.cost3 ~= '-1' then
    o.materials[index] = game_res_data.NewByString(table.cost3)
    index = index + 1
  end
  if table.cost4 ~= '-1' then
    o.materials[index] = game_res_data.NewByString(table.cost4)
    index = index + 1
  end
  
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

local function Start()
	local origin = dofile('market_rule')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t