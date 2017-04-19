local t = {}
t.data = {}
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.name = table.name
  o.function_name = table.function_name
	o.player_level = tonumber(table.player_level)
	o.dungeon_pass = tonumber(table.dungeon_pass)
	o.task_get = table.task_get
	o.task_finish = table.task_finish
	o.notice = table.notice
	o.vip = tonumber(table.vip)
	o.show_main_position = table.show_main_position
	o.show_sheet_position = table.show_sheet_position
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

local function Start()
	local origin = dofile('newfunction')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t