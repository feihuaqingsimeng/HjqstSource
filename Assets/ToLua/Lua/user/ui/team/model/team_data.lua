local t = {}
t.data = {}
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.team_id = tonumber(table.team_id)
  
	o.id_lv_1 = table.id_lv_1
	o.id_lv_2 = table.id_lv_2
	o.id_lv_3 = table.id_lv_3
	o.id_lv_4 = table.id_lv_4
	o.id_lv_5 = table.id_lv_5
	o.id_lv_6 = table.id_lv_6
	o.id_lv_7 = table.id_lv_7
	o.id_lv_8 = table.id_lv_8
	o.id_lv_9 = table.id_lv_9
  
  if table.boss ~= nil then
    o.boss = tonumber(table.boss)
  end
  
	o.pre_dialog = tonumber(table.pre_dialog)
	o.end_dialog = tonumber(table.end_dialog)
  
  if table.monster_scale1 ~= nil then
    o.monster_scale1 = table.monster_scale1
  else
    o.monster_scale1 = 1
  end

  if table.monster_scale2 ~= nil then
    o.monster_scale2 = table.monster_scale2
  else
    o.monster_scale2 = 1
  end
  
  if table.monster_scale3 ~= nil then
    o.monster_scale3 = table.monster_scale3
  else
    o.monster_scale3 = 1
  end
  
  if table.monster_scale4 ~= nil then
    o.monster_scale4 = table.monster_scale4
  else
    o.monster_scale4 = 1
  end
  
  if table.monster_scale5 ~= nil then
    o.monster_scale5 = table.monster_scale5
  else
    o.monster_scale5 = 1
  end

  if table.monster_scale6 ~= nil then
    o.monster_scale6 = table.monster_scale6
  else
    o.monster_scale6 = 1
  end
  
  if table.monster_scale7 ~= nil then
    o.monster_scale7 = table.monster_scale7
  else
    o.monster_scale7 = 1
  end
	
  if table.monster_scale8 ~= nil then
    o.monster_scale8 = table.monster_scale8
  else
    o.monster_scale8 = 1
  end

  if table.monster_scale9 ~= nil then
    o.monster_scale9 = table.monster_scale9
  else
    o.monster_scale9 = 1
  end
	return o
end

function item:HasBoss ()
  return self.boss ~= nil
end

function item:GetBossHeroInfo ()
  local bossHeroInfo = nil
  
  local bossDataString = nil
  if self.boss == 1 then
    bossDataString = self.id_lv_1
  elseif self.boss == 2 then
    bossDataString = self.id_lv_2
  elseif self.boss == 3 then
    bossDataString = self.id_lv_3
  elseif self.boss == 4 then
    bossDataString = self.id_lv_4
  elseif self.boss == 5 then
    bossDataString = self.id_lv_5
  elseif self.boss == 6 then
    bossDataString = self.id_lv_6
  elseif self.boss == 7 then
    bossDataString = self.id_lv_7
  elseif self.boss == 8 then
    bossDataString = self.id_lv_8
  elseif self.boss == 9 then
    bossDataString = self.id_lv_9
  end
  
  if bossDataString ~= nil then
    local hero_info = require 'ui/hero/model/hero_info'
    local bossDataSplitStringList = string.split(bossDataString, ';')
    local heroDataID = tonumber(bossDataSplitStringList[1])
    local advanceLevel = tonumber(bossDataSplitStringList[2])
    bossHeroInfo = hero_info:New(0, heroDataID, 0, 0, advanceLevel, 0)
  end
  
  return bossHeroInfo
end

function t.GetDataById(id)
	return t.data[id]
end

local function Start()
	local origin = dofile('team')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t