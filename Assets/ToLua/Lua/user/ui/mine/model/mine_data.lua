local t = {}
t.data = {}
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.quality =tonumber(table.quality)
	o.foundation =tonumber (table.foundation)
	o.man_max = tonumber(table.man_max)
	o.time =tonumber(table.time)
	o.protect_time =tonumber( table.protect_time)
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

local function Start()
	local origin = dofile('plunder')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

function t.GetMineItemsList()
  return t.data
end

Start()
return t