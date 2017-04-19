local t = {}
t.data = {}
local item = {}
item.__index = item
local game_res_data = require('ui/game/model/game_res_data')

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.sheet_num = tonumber(table.sheet_num)
	o.buy_type = tonumber(table.buy_type)
	o.resource = table.resource
	o.name = table.name
	o.pic = table.pic
	o.des = table.des
	o.cost = table.cost
	o.first_award = game_res_data.NewByString(table.first_award)
  o.buy_award = game_res_data.NewByString(table.buy_award)
	o.double_base = tonumber(table.double_base)
  o.mounth_card = tonumber(table.mounth_card) > 0
  o.sort = tonumber(table.sort)
  o.ios_is_show = tonumber(table.ios_is_show) > 0
	o.name1 = table.name1
	o.des1 = table.des1
	return o
end
function t.GetDataById(id)
	return t.data[id]
end

local function Start()
	local origin = dofile('shop_diamond')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t