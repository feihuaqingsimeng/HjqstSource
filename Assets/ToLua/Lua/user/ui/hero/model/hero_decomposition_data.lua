local t = {}
t.data = {}
t.optimizedData = {}
local item = {}
item.__index = item

local game_res_data = require 'ui/game/model/game_res_data'

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.heroid = tonumber(table.heroid)
	o.star = tonumber(table.star)
	o.heropiece = game_res_data.NewByString(table.heropiece)
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

function t.GetCompositionDataByHeroIDAndStar (heroID, star)
  return t.optimizedData[heroID][star]
end

local function Start()
	local origin = dofile('hero_decomposition')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)

  for k, v in pairs(t.data) do
    if t.optimizedData[v.heroid] == nil then
      t.optimizedData[v.heroid] = {}
    end
    t.optimizedData[v.heroid][v.star] = v
  end
end

Start()
return t