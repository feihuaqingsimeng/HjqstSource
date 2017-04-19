local t = {}
t.data = {}----star,{quality,{item}}
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.star = tonumber(table.star)
	o.exp_provide = tonumber(table.exp_provide)
	o.quality = tonumber(table.quality)
	return o
end

function t.GetDataByStarAndQuality(star,quality)
  local starTable = t.data[star]
  if starTable == nil then
    return nil
  end
	return starTable[quality]
end

local function Start()
	local origin = dofile('aggr')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
    if not t.data[newItem.star] then
       t.data[newItem.star] = {}
    end
    local starTable = t.data[newItem.star]
    starTable[newItem.quality] = newItem
	end)
end

Start()
return t