local t = {}
t.data = {}
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.sheet_num = tonumber(table.sheet_num)
	o.buy_type = tonumber(table.buy_type)
	o.item = table.item
	o.name = table.name
	o.des = table.des
	o.pic = table.pic
	o.cost = table.cost
	o.item_num = tonumber(table.item_num)
  o.discount = tonumber(table.discount)
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

local function Start()
	local origin = dofile('shop_goods')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t