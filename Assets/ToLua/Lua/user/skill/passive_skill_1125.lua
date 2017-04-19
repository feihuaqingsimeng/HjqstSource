--攻击被标记目标，额外造成50%伤害
function attackBuff_1125(character,target,skillInfo,judgeType)  
  if(target.Tag) then
      --target:RemoveBuff(Logic.Enums.BuffType.Tag)
      return 0.5
  end
end
  
  --[[local exceptions = fightdataTable["exceptionBuffs"..character.characterInfo.instanceID]
  for k,v in ipairs(exceptions) do
    if target:ExsitBuff(v) then
      return 0.5
      break
    end
  end
  return 0
end

function InitCharacterDatas_112(instanceID)
  --异常buff库
  fightdataTable["exceptionBuffs"..instanceID] = 
  {
     Logic.Enums.MechanicsType.Tag,
     
  }
end--]]
