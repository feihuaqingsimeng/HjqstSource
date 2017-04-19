local t = {}
t.data = {}
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.heroId = tonumber(table.hero_id)
	o.portrait = table.portrait
	o.avatar = tonumber(table.avatar)
	o.figureImage = table.figureImage
	o.offence = tonumber(table.offence)
	o.defence = tonumber(table.defence)
	o.task_condition = table.task_condition
	o.transfer_item = table.transfer_item
	o.hopping = table.hopping
	o.summonId = tonumber(table.summonId)
	o.summonSkillId = tonumber(table.summonSkillId)
	o.summonMax = tonumber(table.summonMax)
	o.pet_breakthrough_item = table.pet_breakthrough_item
	o.pet_id = tonumber(table.pet_id)
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

function item:GetChangeProfessionPlayerDataList ()
  local changeProfessionPlayerDataList = {}
  local hoppingIDs = string.split(self.hopping, ';')
  for k, v in pairs(hoppingIDs) do
    table.insert (changeProfessionPlayerDataList, t.data[tonumber(v)])
  end
  return changeProfessionPlayerDataList
end

function item:GetPetBreakthroughItemGameResData ()
  local game_res_data = require('ui/game/model/game_res_data')
  local gameResData = game_res_data.NewByString(self.pet_breakthrough_item)
  return gameResData
end

local function Start()
	local origin = dofile('player')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t