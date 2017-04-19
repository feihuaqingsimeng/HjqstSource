local t = {}
t.__index = t

local consortia_log = require('ui/consortia/model/consortia_log')
local consortia_data = gamemanager.GetData('consortia_data')

function t.New (id)
  local o = {}
  setmetatable(o, t)
  o:Set(id,1,'name'..id,'creator',1,0,1,'notice',0)
  o.logList = {}
  for i = 1,20 do
    o.logList[i] = consortia_log.New(id,TimeController.instance.ServerTimeTicksSecond,{})
  end
  return o
end
function t.NewByGuildInfoProto(guildInfoProto)
   local o = {}
  setmetatable(o, t)
  o.logList = {}
  o:SetByGuildInfoProto(guildInfoProto)
  
  return o
end
function t:UpdateLog(guildLogProtoList)
  if # guildLogProtoList == 0 then
    return
  end
  self.logList = {}
  for k,v in ipairs(guildLogProtoList) do
    self.logList[k] = consortia_log.New(v.type,v.time,v.params)
  end
  table.sort(self.logList,function(a,b)
      return a.time < b.time
    end)
end
function t:IsMaxLevel()
  return self.lv >= consortia_data.GetMaxLevel()
end
function t:ExpPercent()
  if self:IsMaxLevel() then
    return 0
  end
  return self.exp/consortia_data.GetDataById(self.lv+1).exp
end
function t:Set(id,headNo,name,creatorName,lv,curNum,maxNum,notice,exp)
   self.id = id
   self.headNo = headNo
   self.name = name
   self.creatorName = creatorName
   self.lv = lv
   self.curNum = curNum
   self.maxNum = maxNum
   self.notice = notice
   self.exp = exp
end

function t:SetByGuildInfoProto(guildInfoProto)
  if guildInfoProto.no ~= 0 then
    self.id = guildInfoProto.no
  end
  if guildInfoProto.headNo ~= 0 then
    self.headNo = guildInfoProto.headNo
  end
  if guildInfoProto.name ~= '' and guildInfoProto.name ~= nil then
    self.name = guildInfoProto.name
  end
  if guildInfoProto.lv ~= 0 then
    self.lv = guildInfoProto.lv
    --print('lv', self.lv)
  end
  if guildInfoProto.presidentName ~= '' then
    self.creatorName = guildInfoProto.presidentName
  end
  if guildInfoProto.curNum ~= 0 then
    self.curNum = guildInfoProto.currNum
  end
  if guildInfoProto.maxNum ~= 0 then
    self.maxNum = guildInfoProto.maxNum
  end
  if guildInfoProto.notice ~= '' then
    self.notice = guildInfoProto.notice
  end
  if guildInfoProto.exp ~= -1 then
    self.exp = guildInfoProto.exp
   -- print('exp', self.exp)
  end
  --日志
  if #guildInfoProto.logList then
    self:UpdateLog(guildInfoProto.logList)
  end
  --玩家个人公会信息
  if guildInfoProto.roleGuild then
    self.roleGuild = {}
    self.roleGuild.remainPresentTimes = guildInfoProto.roleGuild.remainPresentTimes
    self.roleGuild.isSign = guildInfoProto.roleGuild.isSign
    --print('isSign', guildInfoProto.roleGuild.isSign)
    self.roleGuild.contribute = guildInfoProto.roleGuild.devote
  end
  
end
--公会自动通过条件
function t:SetAutoPassCondition(isOpen,level,power)
  self.limitIsOpen = isOpen
  self.limitLevel = level
  self.limitPower = power
end

return t