local t = {}
t.__index = t

function t:New(info)
  local o = {}
  setmetatable(o, self)
  
  o.Update = function(newo)
    o.type = newo.type
    o.subType = newo.subType
    o.conditionValue = newo.conditionValue
    o.scheduleStus = newo.scheduleStus
  end
  
  o.type = tonumber(info.type)
  o.subType = tonumber(info.subType)
  o.conditionValue = tonumber(info.conditionValue)
  o.scheduleStus = Dictionary.New('int','int')
  
  local strScheduleStus = string.split2(info.scheduleStus, ';', ':')
  if nil == strScheduleStus then do return o end end
  
  for i = 1, #strScheduleStus do 
    local id = tonumber(strScheduleStus[i][1])    
    local State = 3 --状态(0过期未领取，1可以领取未领取，2已领取, 3表示还不可领取)
    if nil ~= strScheduleStus[i][2] then State = tonumber(strScheduleStus[i][2]) end
    o.scheduleStus:Add(id, State)
  end
  
  return o
end

function t:GetStateFromId(o, id)
  if nil == o.scheduleStus then do return end end  
  for k, v in pairs(o.scheduleStus.data) do 
    if k == id then return v end 
  end   
  return 3
end

function t:IsCanAward()
  local isCanAward = false
  for k, v in pairs(self.scheduleStus.data) do
    if v == 1 then  isCanAward = true end
  end   
  return isCanAward
end

function t:IsAlreadyAward()
  local isAlreadyAward = false
  for k, v in pairs(self.scheduleStus.data) do
    if v == 2 then  isAlreadyAward = true end
  end   
  return isAlreadyAward
end

function t:IsCanAwardBySubType(subType)
  local isCanAward = false
  if self.subType ~= subType then do return isCanAward end end
  
  for k, v in pairs(self.scheduleStus.data) do
    if v == 1 then  isCanAward = true end
  end   
  return isCanAward
end

return t