
local self_mine_info = {}
self_mine_info.__index = self_mine_info
self_mine_info.m_reward = nil
self_mine_info.occTime = nil
self_mine_info.plunderTime = nil
self_mine_info.ownMineNo = nil
self_mine_info.endTime = 0
self_mine_info.award=0
self_mine_info.m_listLog = {}
self_mine_info.player = nil
self_mine_info.heros= nil

function self_mine_info:New(reward, loglist)
  local o = {}
  setmetatable(o, self_mine_info) 
  o:Set(reward, loglist)
  return o
end 

function self_mine_info:Set(reward, loglist)
  self_mine_info.m_listLog = {}
  self_mine_info.m_reward = reward
  for index,log in ipairs(loglist) do
   local LogInfo={type = nil,time = nil,params = {}}
   LogInfo.type = log.type
   LogInfo.time = log.time
   for index=1, #log.params do
      table.insert(LogInfo.params, log.params[index])
   end 
   table.insert(self_mine_info.m_listLog ,LogInfo)
  end
end
return self_mine_info