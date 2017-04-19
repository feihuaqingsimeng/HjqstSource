local t = {}
t.data = {}
local item = {}
item.__index = item

local game_res_data = require('ui/game/model/game_res_data')

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.piece_id = table.piece_id
	o.hero_id = table.hero_id
  o.can_compose = (tonumber(table.can_compose) == 1)
	return o
end

function item:GetPieceGameResData ()
  return game_res_data.NewByString(self.piece_id)
end

function item:GetHeroGameResData ()
  return game_res_data.NewByString(self.hero_id)
end

function item:GetEquipGameResData ()
  return game_res_data.NewByString(self.hero_id)
end

function t.GetDataById(id)
	return t.data[id]
end

function t.GetAllHeroPieceDataDic ()
  local heroPieceDataDic = {}
  for k, v in pairs(t.data) do
    if v:GetHeroGameResData().type == BaseResType.Hero then
      heroPieceDataDic[k] = v
    end
  end
  return heroPieceDataDic
end

local function Start()
	local origin = dofile('piece')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t