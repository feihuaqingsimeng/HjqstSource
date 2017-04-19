local t = {}
t.dataByStrengthenType = {}
local item = {}
item.__index = item

local game_res_data = require('ui/game/model/game_res_data')

function item.New(table)
	local o = {}
	setmetatable(o,item)
  o.id = tonumber(table.id)
  o.level = tonumber(table.level)
	o.strengthen_type = tonumber(table.strengthen_type)
  o.cost = tonumber(table.cost)
	o.attr_add_a = tonumber(table.attr_add_a)

	return o
end

function t.GetDataByStrengthenTypeAndLevel(strengthenType,level)
	return t.dataByStrengthenType[strengthenType][level]
end

local function Start()
	local origin = dofile('equip_strengthen')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
    if t.dataByStrengthenType[newItem.strengthen_type] == nil then
      t.dataByStrengthenType[newItem.strengthen_type] = {}
    end
		t.dataByStrengthenType[newItem.strengthen_type][newItem.level] = newItem
	end)
end

Start()
return t