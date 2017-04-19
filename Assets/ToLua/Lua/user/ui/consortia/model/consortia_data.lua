local t = {}
t.data = {}
local item = {}
item.__index = item

local game_res_data = require('ui/game/model/game_res_data')

t.maxLevel = 0

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.guild_lv = tonumber(table.guild_lv)
	o.exp = tonumber(table.exp)
	o.guild_number = tonumber(table.guild_number)
	o.sign_reward = game_res_data.NewByString(table.sign_reward)
	o.sign_exp = tonumber(table.sign_exp)
	o.gvg_open = tonumber(table.gvg_open)
	o.guild_donation_number = tonumber(table.guild_donation_number)
	o.guild_donation_gold = game_res_data.ParseGameResDataList(table.guild_donation_gold)
	o.guild_donation_award = game_res_data.ParseGameResDataList(table.guild_donation_award)
	o.guild_donation_exp = string.split(table.guild_donation_exp,';')
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

function t.GetMaxLevel()
  return t.maxLevel
end

local function Start()
	local origin = dofile('gulid_list')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
    if t.maxLevel < newItem.guild_lv then
      t.maxLevel = newItem.guild_lv
    end
	end)
end

Start()
return t