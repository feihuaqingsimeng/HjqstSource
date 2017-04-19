local t = {}
t.data = {}
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.dungeon_type = tonumber(table.dungeon_type)
  o.chapterID = tonumber(table.chapterId)
	o.star_number = tonumber(table.star_number)
  o.chestPosition = tonumber(table.chest_position)
	o.award = table.award
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

function t.GetDungeonStarDataList (dungeonType)
  local dungeonStarDataList = {}
  for k, v in pairs(t.data) do
    if (v.dungeon_type == dungeonType) then
      dungeonStarDataList[k] = v
    end
  end
  return dungeonStarDataList
end

function t.GetDungeonStarDataList ()
  local dungeonStarDataList = {}
  for k, v in pairs(t.data) do
      dungeonStarDataList[k] = v
  end
  return dungeonStarDataList
end

function t.SortData (aDungeonStarData, bDungeonStarData)
  return aDungeonStarData.id < bDungeonStarData.id
end

local function Start()
	local origin = dofile('dungeon_star')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
  table.sort(t.data, t.SortData)
end

Start()
return t