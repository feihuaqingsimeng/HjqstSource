local t = {}
t.__index = t

function t.New(dungeonID)
  local o = {}
  setmetatable(o, t)
  o.id = dungeonID
  o.dungeonData = gamemanager.GetData('dungeon_data').GetDataById(dungeonID)
  o.isLock = true
  o.star = 0
  o.todayChallengedTimes = 0
  o.dayRefreshTimes = 0
  return o
end

return t