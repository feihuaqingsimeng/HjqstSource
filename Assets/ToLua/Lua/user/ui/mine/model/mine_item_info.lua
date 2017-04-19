local mineItemInfo= {}
mineItemInfo.__index = mineItemInfo
function mineItemInfo:NewMineItem(mineItem)
  local o = {}  
  setmetatable(o,mineItemInfo)
  o:Init()
  o:Update(mineItem)
  return o
end
function mineItemInfo:Init()
  mineItemInfo.mineNo = 0
  mineItemInfo.occNum = 1
end

function mineItemInfo:Update(mineItem)
  self.mineNo = mineItem.mineNo
  self.occNum = mineItem.occNum
end
return mineItemInfo