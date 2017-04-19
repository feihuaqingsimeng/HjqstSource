local t = {}
t.data = {}
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.des = table.des
	o.repeat_times = table.repeat_times
	o.repeat_every_time = table.repeat_every_time
	o.des1 = table.des1
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

local function Start()
	local origin = dofile('broadcast')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t