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
	o.type = tonumber(table.type)
	o.resource = table.resource
	o.name = table.name
	o.pic = table.pic
	o.des = table.des
	o.cost = table.cost
	o.pre_item = tonumber(table.pre_item)
	o.item_num = tonumber(table.item_num)
  o.discount = tonumber(table.discount)
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

function t.GetActionItemDataList ()
  local result = {}
  for k, v in pairs(t.data) do
    -- [[ v.sheet_num == 14 表示行动力商品 ]] --
    if v.sheet_num == 14 then
      result[k] = v
    end
  end
  return result
end

function t.GetGoldItemDataList ()
  local result = {}
  for k, v in pairs(t.data) do
    -- [[ v.sheet_num == 14 表示金币商品 ]] --
    if v.sheet_num == 15 then
      result[k] = v
    end
  end
  return result
end

local function Start()
	local origin = dofile('shop_limit_item')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t