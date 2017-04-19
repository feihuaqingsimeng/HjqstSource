local t = {}
t.__index = t
    
function t:New(info)
  local o = {}
  setmetatable(o, self)
  
  o.id = info.id
  o.InActivity = true
  o.completed = info.completed
  o.getReward = info.getReward
  if #info.conditions > 0 then
    o.taskConditionId = info.conditions[1].id
    o.taskConditionValue = info.conditions[1].value
  else
    local taskData = gamemanager.GetData('task_data').GetDataById(o.id)
    if nil == taskData then do return end end
  
    o.taskConditionId = taskData.transfer
    o.taskConditionValue = 0
  end
  
  return o
end

function t:NewData(data)
  local o = {}
  setmetatable(o, self)
  
  o.id = tonumber(data.task)
  o.InActivity = false
  o.completed = false
  o.getReward = false
  
  local taskData = gamemanager.GetData('task_data').GetDataById(o.id)
  if nil == taskData then do return end end
  
  o.taskConditionId = taskData.transfer
  o.taskConditionValue = 0
  
  return o
end

return t