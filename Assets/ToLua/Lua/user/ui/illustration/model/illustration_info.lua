local t = {}
t.__index = t

function t.NewByRoleInfo(illustrationDataId,roleInfo)
  local o = {}
  setmetatable(o,t)
  o.roleInfo = roleInfo
  o.id = roleInfo.heroData.id
  o.star = roleInfo.advanceLevel
  o.illustrationDataId = illustrationDataId
  return o
end

function t.NewByEquipInfo(illustrationDataId,equipInfo)
  local o = {}
  setmetatable(o,t)
  o.equipInfo = equipInfo
  o.id = equipInfo.data.id
  o.illustrationDataId = illustrationDataId
  return o
end
function t.NewByItemInfo(illustrationDataId,itemInfo)
  local o = {}
  setmetatable(o,t)
  o.itemInfo = itemInfo
  o.id = itemInfo.itemData.id
  o.illustrationDataId = illustrationDataId
  return o
end
return t
