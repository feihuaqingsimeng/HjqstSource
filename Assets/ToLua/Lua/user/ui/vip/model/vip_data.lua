local t = {}
t.data = {}
t.minLevel = 1
t.maxLevel = 1
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
  
	o.index = tonumber(table.index)
	o.id = tonumber(table.id)
	o.name = table.name
	o.des = table.des
	o.rewards = table.rewards
	o.daily_welfare = table.daily_welfare
	o.exp = tonumber(table.exp)
	o.faraway = tonumber(table.faraway)
	o.pve_action_add = tonumber(table.pve_action_add)
	o.daily_dungeon_buytimes = tonumber(table.daily_dungeon_buytimes)
	o.pve_action_buytimes = tonumber(table.pve_action_buytimes)
	o.gold_buytimes = tonumber(table.gold_buytimes)
	o.pvp_refresh = tonumber(table.pvp_refresh)
	o.pvp_add = tonumber(table.pvp_add)
	o.world_tree_add = tonumber(table.world_tree_add)
	o.world_tree_buytimes = tonumber(table.world_tree_buytimes)
	o.dungeon_gold_add = tonumber(table.dungeon_gold_add)
	o.daily_dungeon_add = tonumber(table.daily_dungeon_add)
	o.formation_add = tonumber(table.formation_add)
	o.explore_TaskNum = tonumber(table.explore_TaskNum)
  o.plunder_occ = tonumber(table.plunder_occ)
	o.plunder_num = tonumber(table.plunder_num)
  o.three_speed = table.three_speed
	o.blackmarket_BuyNum = table.blackmarket_BuyNum
	o.dayRefreshTimes = table.dayRefreshTimes
	o.dayGoldHandTimes = tonumber(table.dayGoldHandTimes)
  o.fund_lv_limit = tonumber(table.fund_lv_limit)
	return o
end

function t.GetAllVIPDataList ()
  return t.data
end

function t.GetVIPData (vipLevel)
  return t.data[vipLevel]
end

function t.SortData (aVIPData, bVIPData)
  return aVIPData.id < bVIPData.id
end

function item:GetBenefitItems ()
  local gameResData = require('ui/game/model/game_res_data')
  local benefitItems = gameResData.ParseGameResDataList(self.rewards)
  return benefitItems
end

function item:GetNextLevelVIPData ()
  return t.data[self.id + 1]
end

function item:IsMinLevel ()
  return self.id == t.minLevel
end

function item:IsMaxLevel ()
  return self.id == t.maxLevel
end

function item:IsCanBuyFoundLevelGift()
  return self.fund_lv_limit == 1
end

function item:ShortName ()
  return string.format(LocalizationController.instance:Get('common.vip_short_name_lua'), self.id)
end

local function Start()
	local origin = dofile('vip_config')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[newItem.id] = newItem
	end)

  table.sort(t.data, t.SortData)
  
  for k, v in pairs(t.data) do
    if v.id < t.minLevel then
      t.minLevel = v.id
    end
    if v.id > t.maxLevel then
      t.maxLevel = v.id
    end
  end
end

Start()
return t