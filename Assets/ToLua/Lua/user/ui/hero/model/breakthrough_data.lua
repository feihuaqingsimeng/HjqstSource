local t = {}
t.data = {}
local item = {}
item.__index = item

local game_res_data = require('ui/game/model/game_res_data')

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.layer = tonumber(table.layer)
	o.quality = tonumber(table.quality)
	o.levelMin = tonumber(table.lock_lev)
	o.levelMax = tonumber(table.lock_release)
  
  o.costGoldGameResData = nil
  if table.cost_1 ~= '0' then
    o.costGoldGameResData = game_res_data.NewByString(table.cost_1)
  end
  
  o.costItemGameResData = nil
  if table.cost_2 ~= '0' then
    o.costItemGameResData = game_res_data.NewByString(table.cost_2)
  end
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

function t.GetBreakthroughDataListByQuality (quality)
  local breakthroughDataListOfQuality = {}
  for k, v in ipairs(t.data) do
    if v.quality == quality and v.layer > 1 then
      table.insert(breakthroughDataListOfQuality, v)
    end
  end
  return breakthroughDataListOfQuality
end

function t.GetNextBreakthroughDataByRoleInfo (roleInfo)
  for k, v in ipairs(t.data) do
    if v.quality == roleInfo.heroData.quality and v.layer == roleInfo.breakthroughLevel + 1 then
      return v
    end
  end
  return nil
end

local function Start()
	local origin = dofile('breakthrough')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t