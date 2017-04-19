local t = {}
t.data = {}
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tostring(table.id)
	o.model = table.model
	o.scale = table.scale
	o.speed = tonumber(table.speed)
	o.home_rotation = table.home_rotation
	o.rotation = table.rotation
	o.stay_animation = table.stay_animation
	o.home_Offset = table.home_Offset
	o.Offset = table.Offset
	o.head_icon = table.head_icon
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

local function Start()
	local origin = dofile('pet')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t