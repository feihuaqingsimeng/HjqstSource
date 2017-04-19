--每次普攻，有几率点燃目标，每秒11%伤害，持续5秒
function attackBuff_495(character,target,skillInfo,judgeType)
  if(skillInfo.skillData.skillId == 490) then
    local random = math.random()
    local probablity = 0.60   --概率
    --print(random)
    --概率
    if(random < probablity)  then
        --local mechanics = Logic.Skill.Model.MechanicsData.GetMechanicsDataById(3755)
        local lastTime = 5  --持续时间
        local damageRate = 0.11
        target:AddBuff(character,target,skillInfo,null,Logic.Enums.BuffType.Ignite,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Fixed,lastTime,damageRate,1,1,false,true)
    end
  end
end