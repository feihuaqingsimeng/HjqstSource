--自身受到的物理伤害减少20%（常驻）
function damge_1335(character,target,roleAttackAttributeType,skillInfo)
  if(roleAttackAttributeType == Logic.Enums.RoleAttackAttributeType.PhysicalAttack) then
    local damgeRateDec = 0.2
    return damgeRateDec
  end  
  return 0
end