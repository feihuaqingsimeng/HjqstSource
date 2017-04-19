--[[
local t = {}
t.data = {}
local item = {}
item.__index = item

local game_res_data = require('ui/game/model/game_res_data')

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.star)
	o.lv_limit = tonumber(table.lv_limit)
	o.gold = tonumber(table.gold)
  o.materials = {}
	o.materials[1] = item.ParseMaterialString(table.hero_type_1)
	o.materials[2] = item.ParseMaterialString(table.hero_type_2)
	o.materials[3] = item.ParseMaterialString(table.hero_type_3)
	o.materials[4] = item.ParseMaterialString(table.hero_type_4)
	o.materials[5] = item.ParseMaterialString(table.hero_type_5)
	o.hero_type_all = item.ParseMaterialString(table.hero_type_all)
	return o
end

function item.ParseMaterialString(str)
  local resList = {}
  local s = string.split2(str,';',':')
  for k,v in pairs(s) do
    local res = game_res_data.New(BaseResType.Item,tonumber(v[1]),tonumber(v[2]),0)
    resList[k] = res
  end
  return resList
end

function item:GetMaterialsByRoleType(roleType)
  return self.materials[roleType]
end

function t.GetDataById(id)
	return t.data[id]
end

local function Start()
	local origin = dofile('advance')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t
]]--

local t = {}
t.data = {}
local item = {}
item.__index = item

local game_res_data = require('ui/game/model/game_res_data')

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.heroid = tonumber(table.heroid)
	o.star = tonumber(table.star)
	o.gold = tonumber(table.gold)
	o.heropiece = table.heropiece
  
  --[[ EXTENDED DATA ]]--
  o.materialGameResDataList = game_res_data.ParseGameResDataList(o.heropiece)
  --[[ EXTENTED DATA ]]--
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

function t.GetAdvanceDataByHeroDataIDAndStar (heroDataID, star)
  for k, v in pairs(t.data) do
    if v.heroid == heroDataID and v.star == star then
      return v
    end
  end
  return nil
end

function t.GetAdvanceDataByHeroInfo (heroInfo)
  return t.GetAdvanceDataByHeroDataIDAndStar(heroInfo.heroData.id, heroInfo.advanceLevel)
end

local function Start()
	local origin = dofile('advance')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t