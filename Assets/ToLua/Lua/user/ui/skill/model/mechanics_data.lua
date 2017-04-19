local t = {}
t.data = {}
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.Id = tonumber(table.Id)
	o.TargetType = table.TargetType
	o.RangeType = tonumber(table.RangeType)
	o.NeedTarget = table.NeedTarget
	o.MechanicsType = table.MechanicsType
	o.EffectIds = table.EffectIds
	o.AudioType = table.AudioType
	o.AudioDelay = table.AudioDelay
	o.MaxLayer = table.MaxLayer
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

local function Start()
	local origin = dofile('mechanics')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t