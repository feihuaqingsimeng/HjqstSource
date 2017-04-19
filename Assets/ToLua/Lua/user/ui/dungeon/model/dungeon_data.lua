local t = {}
t.data = {}
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.name = table.name
	o.description = table.description
	o.dungeon_map = table.dungeon_map
	o.unlock_lv = tonumber(table.unlock_lv)
	o.unlock_dungeon_id_pre1 = tonumber(table.unlock_dungeon_id_pre1)
	o.unlock_dungeon_id_pre2 = tonumber(table.unlock_dungeon_id_pre2)
	o.unlock_dungeon_id_next1 = tonumber(table.unlock_dungeon_id_next1)
	o.unlock_dungeon_id_next2 = tonumber(table.unlock_dungeon_id_next2)
	o.teams = table.teams
	o.each_loot_id = table.each_loot_id
	o.special_loot_id = table.special_loot_id
	o.first_loot_id = table.first_loot_id
	o.each_loot_present = table.each_loot_present
	o.hero_present = table.hero_present
	o.rate_method = tonumber(table.rate_method)
	o.action_need = tonumber(table.action_need)
	o.power_fix = tonumber(table.power_fix)
	o.type = tonumber(table.type)
	o.review_id = tonumber(table.review_id)
	o.dungeon_show = table.dungeon_show
	o.tree_show = table.tree_show
  o.day_times = tonumber(table.day_times)
  
  o.item_cost = nil
  if table.item_cost ~= '-1' then
    local game_res_data = require('ui/game/model/game_res_data')
    o.item_cost = game_res_data.NewByString(table.item_cost) 
  end
	return o
end

function item:GetDungeonTypeNamme ()
  local dungeonTypeNames = 
  {
    [DungeonType.Easy] = LocalizationController.instance:Get('ui.select_chapter_view.easy_type'),
    [DungeonType.Normal] = LocalizationController.instance:Get('ui.select_chapter_view.normal_type'),
    [DungeonType.Hard] = LocalizationController.instance:Get('ui.select_chapter_view.hard_type')
  }
  
  local dungeonTypeName = dungeonTypeNames[self.type]
  return dungeonTypeName
end

function item:GetOrderName ()
  return LocalizationController.instance:Get(self.dungeon_show)
end

function item:GetDungeonName ()
  return LocalizationController.instance:Get(self.name)
end

function item:GetTeamIDList ()
  local teamIDList = {}
  local teamIDStrs = string.split(self.teams, ';')
  for k, v in ipairs(teamIDStrs) do
    teamIDList[k] = tonumber(v)
  end
  return teamIDList
end

function item:HasBoss ()
  local teamIDList = self:GetTeamIDList ()
  local team_data = gamemanager.GetData('team_data')
  
  for k, v in ipairs(teamIDList) do
    local teamData = team_data.GetDataById(v)
    if teamData:HasBoss() then
      return true
    end
  end
  return false
end

function item:GetBossHeroInfo ()
  local teamIDList = self:GetTeamIDList ()
  local team_data = gamemanager.GetData('team_data')
  
  for k, v in ipairs(teamIDList) do
    local teamData = team_data.GetDataById(v)
    if teamData:HasBoss() then
      return teamData:GetBossHeroInfo()
    end
  end
  return nil
end

function t.GetDataById(id)
	return t.data[id]
end

function t.GetAllDungeonData ()
  return t.data
end

function t.GetDungeonDataListByDungeonType (dungeonType)
  local dungeonDataList = {}
  for k, v in pairs(t.data) do
    dungeonDataList[k] = v
  end
  return dungeonDataList
end

local function Start()
	local origin = dofile('dungeon')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t