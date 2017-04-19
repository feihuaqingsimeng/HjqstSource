local t = {}
t.data = {}
t.attrData = {}

local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.attr_id = tonumber(table.attr_id)
	o.describe = table.describe
	o.attr_type = tonumber(table.attr_type)
	o.value_min = tonumber(table.value_min)
	o.value_max = tonumber(table.value_max)
	o.weight = tonumber(table.weight)
	o.attr_comat_const = tonumber(table.attr_comat_const)
	return o
end

function item:ValuePercent(value)
  if self.value_max == self.value_min then
    return 1
  end
   
  return (value-self.value_min)/(self.value_max-self.value_min)
end

function t.GetDataById(id)
	return t.data[id]
end

function t.GetDataListByAttrId(attrId)
  local datas = t.attrData[attrId]
  return datas
end

function t.GetFirstDataByAttrId(attrId)
  local datas = t.attrData[attrId]
  if not datas then
    return nil
  end
  for k,v in pairs (datas) do
    return v
  end
  return nil
end

function t.GetDataByAttrId(attrId,attrType)
  local datas = t.attrData[attrId]
  if datas == nil then 
    print(ui_util.FormatToRedText('[error]equip_attr_data can not find attrId:',attrId))
    return nil
  end
  for k,v in pairs(datas) do
    if v.attr_type == attrType then
     return v
    end
  end
  return nil
end

local function Start()
	local origin = dofile('equip_attr')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[newItem.id] = newItem
    if t.attrData[newItem.attr_id] == nil then
      t.attrData[newItem.attr_id] = {}
    end
    t.attrData[newItem.attr_id][newItem.id] = newItem
	end)
end

Start()
return t