local t = {}
t.data = {}
local item = {}
item.__index = item

local game_res_data = require('ui/game/model/game_res_data')

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.event_id = tonumber(table.event_id)	
  o.event_type = tonumber(table.event_type)
	o.event_icon = table.event_icon
	o.event_name = table.event_name
	o.event_describe = table.event_describe
	o.event_resources = table.event_resources
	o.event_timestart = table.event_timestart
	o.event_timeover = table.event_timeover
	o.event_small_type = table.event_small_type
	o.small_id = table.small_id
	o.event_buymoney = table.event_buymoney
	o.event_param = table.event_param
	o.param_des = table.param_des
	o.event_award = table.event_award
	o.open = table.open
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

function t.GetDataBySmallId(type, small_id)
	for k, v in pairs(t.data) do
    if v.event_type == type then 
      if tonumber(v.small_id) == small_id then return v end
    end
  end
end

function t.GetDataByType (type)
  for k, v in pairs(t.data) do    
    if v.event_type == type then
      return v
    end
  end
end

function t.GetTypeDatasByType(type)
  local list = {}
  for k, v in pairs(t.data) do
    --if Mathf.Floor(v.event_type/10) == Mathf.Floor(type/10) then
      --table.insert(list, v)
    --end
	if v.event_type == type then
      table.insert(list, v)
    end
  end
  return list
end

function t.IsContainSubType(type, subType)
  local isContain = false
  for k, v in pairs(t.data) do 
    if Mathf.Floor(v.event_type/10) == Mathf.Floor(type/10) and tonumber(v.event_small_type) == subType  then 
      isContain = true
    end      
  end
  
  return isContain
end

function t.GetDataBySubType(type, subType)  
  for k, v in pairs(t.data) do 
    if Mathf.Floor(v.event_type/10) == Mathf.Floor(type/10) and tonumber(v.event_small_type) == subType  then 
      return v
    end      
  end
end

function t.GetDatasBySubType(type, subType)
  local list = {}
  for k, v in pairs(t.data) do
    if  Mathf.Floor(v.event_type/10) == Mathf.Floor(type/10) and tonumber(v.event_small_type) == subType then
      table.insert(list, v)
    end
  end
  return list
end
--大转盘数据
function t.GetTurntableData()
  return t.GetDataByType(170)
end

local function Start()
  
	local origin = dofile('event'..gamemanager.GetModel('game_model').lastServerId)
  if not origin then
    Debugger.LogError(" event table name:event"..gamemanager.GetModel('game_model').lastServerId.." does not exist now load origin event ")
    origin = dofile('event')
  end
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t