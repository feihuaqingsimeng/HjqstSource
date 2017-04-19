local t = {}
local item = {}
item.__index = item

local game_res_data = require('ui/game/model/game_res_data')

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
  o.PreId = tonumber(table.PreId)
	o.NextId = tonumber(table.NextId)
  
  if table.money == nil or table.money == '0' then
    o.money = nil
  else
    o.money = game_res_data.NewByString(table.money)
  end
  
  o.Material = {}
  if table.Material ~= '0' then
    o.Material = game_res_data.ParseGameResDataList(table.Material)
  end
	return o
end

function t.GetDataById(id)
	return t[id]
end

local function Start()
	local origin = dofile('GemSynthesis')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t[id] = newItem
	end)
end

Start()
return t