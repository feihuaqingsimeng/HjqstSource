local t = {}
t.__index = t

local player_model = gamemanager.GetModel('player_model')

function t.New(guildMemberProto)
  local o = {}
  setmetatable(o,t)
  o:Set(guildMemberProto)
  
  return o
  
end
function t:Set(guildMemberProto)
  self.id = guildMemberProto.id
  self.headNo = guildMemberProto.headNo
  self.name = guildMemberProto.roleName
  self.lv = guildMemberProto.lv
  self.contribute = guildMemberProto.devot
  self.presidentName = guildMemberProto.presidentName
  self.lastLoginTime = guildMemberProto.lastLoginTime
  self.job = guildMemberProto.job--职务0成员1会长
  --print(guildMemberProto.id,guildMemberProto.headNo,guildMemberProto.roleName,guildMemberProto.lv,guildMemberProto.devot,guildMemberProto.presidentName,guildMemberProto.job)
end
--会长否？
function t:IsPresident()
  return self.job == 1
end
return t