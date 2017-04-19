--自身，对石化状态目标造成额外12%伤害（常驻）
function attackBuff_1485(character,target,skillInfo,judgeType)     
  if(target:ExistBuff(Logic.Enums.BuffType.Frozen)) then
    return 0.12
  end
end