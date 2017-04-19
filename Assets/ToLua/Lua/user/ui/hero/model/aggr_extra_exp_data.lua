local t = {}
t.data = {}
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.heroId = tonumber(table.heroId)
	o.star = tonumber(table.star)
	o.exp = tonumber(table.exp)
	return o
end

function t.GetDataById(id,star)
  local idstr = id..','..star
	return t.data[idstr]
end
function t.GetExtraExp(id,star)
  local d = t.GetDataById(id,star)
  if not d then
    return 0
  end
  return d.exp
end

local function Start()
	local origin = dofile('aggr_extraExp')
	local idstr = ''
  origin.ForEach(function(id,table)
		local newItem = item.New(table)
    idstr = newItem.heroId..','..newItem.star
		t.data[idstr] = newItem
	end)
end

Start()
return t