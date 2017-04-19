local t = {}
t.data = {}
local item = {}
item.__index = item
local game_res_data = require('ui/game/model/game_res_data')
function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id =tonumber(table.id)
	o.quality =tonumber(table.quality)
	o.prize_id =tonumber(table.prize_id)
	o.need_equip = table.need_equip
	o.need_gold_cost =game_res_data.NewByString(table.need_gold_cost) 
	o.gold_chance = table.gold_chance
	o.need_diamond_cost =game_res_data.NewByString(table.need_diamond_cost)  
	o.diamond_chance = table.diamond_chance
	return o
end

function t.GetDataById(id)
	return t.data[id]
end
function t.GetDataByQuality(quality)
  local data = nil
  for k,v in pairs(t.data) do
      if v.quality == quality then
        data = v
        break
      end
  end
  return data
end

local function Start()
	local origin = dofile('equip_compose')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t