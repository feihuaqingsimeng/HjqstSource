local t = {}
t.__index = t

local expedition_data = gamemanager.GetData('expedition_data')

function t.New(dataId,isFinished,isGetReward,isUnlocked)
  
  local o = {}
  setmetatable(o,t)
  o.id = dataId
  o.data = expedition_data.GetDataById(dataId)
  o.isFinished = isFinished
  o.isGetReward = isGetReward
  o.isUnlocked = isUnlocked
  
  return o
  
end

return t