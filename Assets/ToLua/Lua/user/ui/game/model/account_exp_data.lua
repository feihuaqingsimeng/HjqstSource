local t = {}
t.data = {}
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.lv = tonumber(table.lv)
	o.exp = tonumber(table.exp)
	o.exp_total = tonumber(table.exp_total)
	o.pve_action = tonumber(table.pve_action)
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

local function Start()
	local origin = dofile('exp_account')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t