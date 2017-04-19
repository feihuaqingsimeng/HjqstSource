--自身受到的物理伤害减少55%
function damge_245(character,target,roleAttackAttributeType,skillInfo)
  if(roleAttackAttributeType == Logic.Enums.RoleAttackAttributeType.PhysicalAttack) then
    local damgeRateDec = 0.55
    return damgeRateDec
  end  
  return 0
end


function InitCharacterDatas_24(instanceID)
  local lastTime = 999
  fightdataTable['foreverBuffIcon'..instanceID] = {'skill/06',lastTime}
end