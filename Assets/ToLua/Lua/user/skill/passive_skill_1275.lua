--当有“蓄力”buff的情况下，增加伤害
function attackBuff_1275(character,target,skillInfo,judgeType)
  if(skillInfo.skillData.skillId == 1272) then
    if(target.ExistBuff(Logic.Enums.BuffType.AccumulatorTag)) then
      target.RemoveBuff(Logic.Enums.BuffType.AccumulatorTag)
      return 0.2
    end
  end
end