local t = {}
t.data = {}
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.type = tonumber(table.type)
	o.name = table.name
	o.icon = table.icon
	o.des = table.des
	o.quality = tonumber(table.quality)
  o.lv = tonumber(table.lv)
	o.money = tonumber(table.money)
	o.prize_id = table.prize_id
	o.stack = table.stack
	o.jump_page = table.jump_page
	o.hero_type = string.split(table.hero_type,';')
	o.sort = tonumber(table.count)
	o.universal_id = tonumber(table.universal_id)
  
	return o
end

function item:IconPath()
  return 'sprite/item_icon/'..self.icon
end

function t.GetDataById(id)
	if t.data[id] then
		return t.data[id]
	else
		Debugger.LogError(string.format('can not get item id from config:%s',id))
	end
end

function t.GetBasicResItemByType(baseResType)
  for k, v in pairs(t.data) do
    if v.type == baseResType then
      return v
    end
  end
  return nil
end  

local function Start()
	local origin = dofile('item')
  print('item origin ',origin)
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t