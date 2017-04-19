local t = {}
t.data = {}
local item = {}
item.__index = item

local game_res_data = require('ui/game/model/game_res_data')

function item.New(table)
	local o = {}
	setmetatable(o,item)
  
	o.id = tonumber(table.id)
  o.day = tonumber(table.day)
	o.type = tonumber(table.type)
	o.task = table.task
	o.reward_data = table.reward
	o.des = table.des  
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

function t.GetDataByTask(taskId)
  for k, v in pairs(t.data) do
    if v.task == tostring(taskId) then do return v end end
  end
  return 1
end

function t.GetDefaultTypeByday(bday)
  for k, v in pairs(t.data) do
    if v.day == bday then do return v.type end end
  end
  return 1
end

function t.GetDataByDayAndType(bday, btype)
  local list = {}
  for k, v in pairs(t.data) do
    if v.day == bday and v.type == btype then table.insert(list, v) end
  end
  return list
end

local function Start()
	local origin = dofile('seven_days_party')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t