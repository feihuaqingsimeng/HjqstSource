--自身受到的魔法攻击降低

function magicDefense_165(character)
    local defense = -0.57  --防御
    return defense
end

--[[--免疫自身受到的魔法伤害

function damage_165(character,target,roleAttackAttributeType,skillInfo)
  if(roleAttackAttributeType == Logic.Enums.RoleAttackAttributeType) then
    local damgeRateDec = 1
    return damgeRateDec  
  end
end--]]

function InitCharacterDatas_16(instanceID)
  local lastTime = 999
  fightdataTable['foreverBuffIcon'..instanceID] = {'skill/05',lastTime}
end