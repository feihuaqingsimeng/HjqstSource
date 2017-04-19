--对流血状态目标造成的伤害提高20%
function attackBuff_145(character,target,skillInfo,judgeType)     
  if(target:ExistBuff(Logic.Enums.BuffType.Bleed)) then
    return 0.2
  end
end