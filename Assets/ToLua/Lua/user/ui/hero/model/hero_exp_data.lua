local t = {}
t.data = {}
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.lv)
	o.exp = tonumber(table.exp)
	o.exp_total = tonumber(table.exp_total)
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

local function Start()
	local origin = dofile('exp_hero')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t