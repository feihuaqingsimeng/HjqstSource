local t = {}
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = table.id
	o.pet_id = tonumber(table.pet_id)
	o.effect = tonumber(table.effect)
	o.pre_id = tonumber(table.pre_id)
	o.exp_need = tonumber(table.xp_need)
  o.costResData = {}
	o.costResData[1] = table.lv1
	o.costResData[2] = table.lv2
	o.costResData[3] = table.lv3
	o.costResData[4] = table.lv4
	o.costResData[5] = table.lv5
	o.costResData[6] = table.lv6
	o.costResData[7] = table.lv7
	o.costResData[8] = table.lv8
	o.costResData[9] = table.lv9
	o.costResData[10] = table.lv10
	o.max_lv = tonumber(table.max_lv)
	o.icon = table.icon
	o.name = table.name
	o.des = table.des
	o.groupType = tonumber(table.group)
	o.comat_lv1 = table.comat_lv1
	o.comat_add = table.comat_add
	return o
end

function t.GetDataById(id)
	return t[id]
end

local function Start()
	local origin = dofile('pet_talent')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t[id] = newItem
	end)
end

Start()
return t