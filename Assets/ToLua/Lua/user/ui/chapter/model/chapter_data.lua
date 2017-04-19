local t = {}
t.data = {}
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.name = table.name
	o.chapter_position = table.chapter_position
	o.chapter_bg = table.chapter_bg
	o.chapter_name = table.chapter_name
	o.positions = table.positions
  
  o.easyDungeonIDList = string.split2number(table.dungeons_easy, ";")
  o.normalDungeonIDList = string.split2number(table.dungeons_normal, ";")
  o.hardDungeonIDList = string.split2number(table.dungeons_hard, ";")
  
	o.isChapterOpen = tonumber(table.chapter_open) > 0
	return o
end

function item:GetDungeonIDListOfDungeonType (dungeonType)
  if dungeonType == DungeonType.Easy then
    return self.easyDungeonIDList
  elseif dungeonType == DungeonType.Normal then
    return self.normalDungeonIDList
  elseif dungeonType == DungeonType.Hard then
    return self.hardDungeonIDList
  end
  return nil
end

function t.GetDataById(id)
	return t.data[id]
end

local function Start()
	local origin = dofile('chapter')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t