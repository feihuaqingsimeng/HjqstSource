local t = {}
t.data = {}
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.type = tonumber(table.type)
	o.parameter1 = table.parameter1
	o.parameter2 = table.parameter2
	o.desc = table.desc
	o.comparison_type = table.comparison_type
	o.cumulation = table.cumulation
	o.goto_type = tonumber(table.goto_type)
	o.goto_num = tonumber(table.goto_num)
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

local function Start()
	local origin = dofile('task_condition')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t