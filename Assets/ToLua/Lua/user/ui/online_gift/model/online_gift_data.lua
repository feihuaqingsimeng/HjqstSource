local t = {}
t.data = {}
local item = {}
item.__index = item
local game_res_data = require('ui/game/model/game_res_data')

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.time = tonumber(table.time)
	o.award = game_res_data.ParseGameResDataList(table.award)
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

function t.GetDataLength()
  return table.count(t.data)
end

local function Start()
	local origin = dofile('online_package_award')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t