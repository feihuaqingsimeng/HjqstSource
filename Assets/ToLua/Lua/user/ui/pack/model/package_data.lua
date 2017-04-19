local t = {}
t.data = {}
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.type_name = table.type_name
	o.type_icon = table.type_icon
	o.item_type = table.item_type
	return o
end

function item:GetItemTypes ()
  local itemTypes = {}
  local itemTypeStrings = string.split(self.item_type, ';')
  for k, v in ipairs(itemTypeStrings) do
    table.insert(itemTypes, tonumber(v))
  end
  return itemTypes
end

function t.GetDataById(id)
	return t.data[id]
end

function t.GetAllPackageData ()
  return t.data
end

local function Start()
	local origin = dofile('package')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t