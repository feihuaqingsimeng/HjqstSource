--每次攻击有几率点燃状态
function attackBuff_295(character,target,skillInfo,judgeType)
  if(skillInfo.skillData.skillId == 290)  then
      --local mechanics = Logic.Skill.Model.MechanicsData.GetMechanicsDataById(2951)
      local lastTime = 5 --持续时间
      local igniteRate = 0.13 --点燃值
      target:AddBuff(character,target,skillInfo,null,Logic.Enums.BuffType.Ignite,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,lastTime,igniteRate,1,1,false,true)
  end
end