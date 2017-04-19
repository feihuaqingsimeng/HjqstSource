local t = {}
t.data = {}
local item = {}
item.__index = item

local equip_attr = require('ui/equip/model/equip_attr')

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.ID)
	o.gen_type = tonumber(table.gen_type)
  
	o.equipAttr = equip_attr.New( tonumber(table.AttributeType),tonumber(table.Attribute))
  o.comat = tonumber(table.comat)
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

local function Start()
	local origin = dofile('GemAttribute')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t