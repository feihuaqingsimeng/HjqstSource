local t = {}
t.data = {}
t.victoryReward = nil--gameResData
t.failReward = nil--gameResData
local item = {}
item.__index = item

local game_res_data = require('ui/game/model/game_res_data')

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.rank_min = tonumber(table.rank_min)
	o.rank_max = tonumber(table.rank_max)
	o.award = game_res_data.ParseGameResDataList(table.award)
	return o
end

function t.GetDataById(id)
	return t.data[id]
end
function t.GetDatas()
  return t.data
end

function t.GetVictoryReward()
  
end

function t.GetFailReward()
  
end

local function Start()
	local origin = dofile('gulid_pvp_award')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
    if id == 998 then
      t.failReward = game_res_data.ParseGameResDataList(table.award)
    elseif id == 999 then
      t.victoryReward = game_res_data.ParseGameResDataList(table.award)
    else
      t.data[id] = newItem
    end
		
	end)
  table.sort(t.data,function(a,b)
      return a.id < b.id
    end)
end

Start()
return t