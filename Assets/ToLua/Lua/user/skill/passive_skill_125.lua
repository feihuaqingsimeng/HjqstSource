--开场提升伤害10% 持续7秒
function addHaloBuff_125(character)
  local damageAddRate = 0.1
  local lastTime = 7
  character:AddBuff(character,character,null,null,Logic.Enums.BuffType.DamageAdd,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Fixed,lastTime,damageAddRate,1,1,false,false)
end
