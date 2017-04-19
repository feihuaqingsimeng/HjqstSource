local t = {}
t.data = {}
local item = {}
item.__index = item

t.marketTypeList = {}
function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber( table.id)
	o.market_type = tonumber(table.item_type)
	o.name = table.black_tab
  o.icon = table.black_tab_icon
	o.item_type_name = table.item_type_name
	o.limit_type = tonumber(table.limit_type)
	o.limit_num = tonumber(table.limit_num)
	o.limit_lv = tonumber(table.limit_lv)
	o.random_rule = tonumber(table.random_rule)
	o.on_sell = tonumber(table.on_sell)
	o.sys_refresh = table.sys_refresh
	o.manual_refresh = tonumber(table.manual_refresh)
  
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

local function Start()
	local origin = dofile('black_market')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
    
    local tempTable = t.marketTypeList[newItem.market_type]
    if not tempTable then
      tempTable = {}
      tempTable.type = newItem.market_type
    end
    if newItem.name then
      tempTable.name = newItem.name
    end
    if newItem.manual_refresh then
      tempTable.manual_refresh = newItem.manual_refresh
    end 
    if newItem.icon then
      tempTable.icon = newItem.icon
    end
    if table.refresh_money ~= '0' then
      tempTable.refresh_diamond = string.split(table.refresh_money,';')
      for k,v in pairs(tempTable.refresh_diamond) do
        tempTable.refresh_diamond[k] = tonumber(v)
      end
    end
    t.marketTypeList[newItem.market_type] = tempTable
	end)

end

Start()
return t