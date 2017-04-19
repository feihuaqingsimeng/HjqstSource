local t = {}
t.data = {}
local item = {}
item.__index = item

local game_res_data = require('ui/game/model/game_res_data')

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id =  tonumber( table.id)
	o.hero = game_res_data.NewByString(table.hero_id)
	o.need_cost = game_res_data.NewByString(table.need_cost)
	o.need_material = game_res_data.ParseGameResDataList(table.need_material)
	return o
end

function t.GetDataById(id)
	return t.data[id]
end
function t.GetDataByHeroId(id)
  for k,v in pairs(t.data) do
    if v.hero.id == id then
      return v
    end 
  end
  return nil
end

local function Start()
	local origin = dofile('new_herocompose')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t