local equip_util = {}

function equip_util.canEquip (equipInfo, roleInfo)
  local canEquip = true
  canEquip = (roleInfo.level >= equipInfo.data.useLv) and (roleInfo.heroData.roleType == equipInfo.data.equipmentRoleType)
  return canEquip
end

return equip_util