local t = {}
t.data = {}
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.type = tonumber(table.type)
	o.item_id = tonumber(table.item_id)
	o.soul = tonumber(table.soul)
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

function t.GetDataByTypeAndItemId(baseResType,itemId)
  for k,v in pairs(t.data) do
    if v.type == baseResType and v.item_id == itemId then
      return v
    end
  end
  return nil
end

local function Start()
	local origin = dofile('equip_decomposition')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t