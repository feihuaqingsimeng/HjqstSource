local t = {}
t.__index = t


function t:Reset()
  self.id = 0
  self.level = 0
  self.name = ''
  self.power = 0
  self.lastLoginTime = 0
  self.isGetPveAction = true--///是否领取体力了 true:领  false:未领
  self.isDonate = false
  self.isBothAuth = false--双向朋友
  self.headIcon = ''
  self.vip = 0
  self.playerInfomationInfo = nil -- 好友英雄、阵型信息[参考：player_infomation_check/model/player_infomation_info.lua]
end

function t:SetDonate(isDonate)
  self.isDonate = isDonate
end

function t.NewByCSharpFriendInfo(friendInfo)
  local o = {}
  setmetatable(o,t)
  o:Reset()
  o.id = friendInfo.id
  o.level = friendInfo.level
  o.name = friendInfo.name
  o.power = friendInfo.power
  o.lastLoginTime = friendInfo.lastLoginTime
  o.isGetPveAction = friendInfo.isGetPveAction
  o.isDonate = friendInfo.isDonate
  o.isBothAuth = friendInfo.isBothAuth
  o.headIcon = friendInfo.headIcon
  o.vip = friendInfo.vip
  return o
end

function t:ToString()
  return string.format('id:%d,name:%s')
end

return t