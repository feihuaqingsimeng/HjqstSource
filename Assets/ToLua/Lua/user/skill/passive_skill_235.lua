--每次命中，自身回血，伤害的10%
function attackFinishBuff_235(character, target, skillInfo, judgeType, actualHPValue)
  local hpValue = actualHPValue * 0.1
  print('treat value',hpValue)
  Logic.Fight.Controller.MechanicsController.instance:SetTreatValue(character,hpValue,false)
end