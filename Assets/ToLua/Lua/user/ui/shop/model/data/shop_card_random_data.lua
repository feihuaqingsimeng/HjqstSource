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
	o.prize_id = tonumber(table.prize_id)
	o.prize_pro_id = tonumber(table.prize_pro_id)
	o.first_prize = table.first_prize
	o.name = table.name
	o.pic = table.pic
  o.bg_pic = table.bg_pic
	o.des = table.des
	o.cost = table.cost
	o.free_time = tonumber(table.free_time)
	o.max = tonumber(table.max)
  o.discount = tonumber(table.discount)
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

function t.GetHeroRandomItemDataList ()
  local result = {}
  for k, v in pairs(t.data) do
    --[[英雄抽卡 sheet_num == 11]]--
    if v.sheet_num == 11 then 
      result[k] = v
    end
  end
  return result
end

function t.GetArticleRandomDataList ()
  local result = {}
  for k, v in pairs(t.data) do
    --[[装备抽卡 sheet_num == 12]]--
    if v.sheet_num == 12 then 
      result[k] = v
    end
  end
  return result
end

local function Start()
	local origin = dofile('shop_card_random')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t