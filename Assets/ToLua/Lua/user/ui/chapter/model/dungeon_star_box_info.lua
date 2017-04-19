local t = {}
t.__index = t
local dungeon_star_data = gamemanager.GetData('dungeon_star_data')

function t.New (dungeonStarDataID, hasReceived)
  local o = {}
  setmetatable(o, t)
  
  o.dungeonStarData = dungeon_star_data.GetDataById(dungeonStarDataID)
  o.hasReceived = hasReceived
  return o
end

function t:CanReceive()
  local totalStarCount = LuaInterface.LuaCsTransfer.GetTotalStarCountOfDungeonType(self.dungeonStarData.dungeon_type)
  return totalStarCount >= self.dungeonStarData.star_number
end

return t