--每次攻击33%几率冰冻目标3秒
function attackBuff_275(character, target, skillInfo, judgeType)
  local random = math.random()
  local probablity = 0.33   --概率
  if(random < probablity)  then
    local mechanics = Logic.Skill.Model.MechanicsData.GetMechanicsDataById(2755)
    local lastTime = 3  --持续时间
    target:AddBuff(character,target,skillInfo,mechanics,Logic.Enums.BuffType.Frozen,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.None,lastTime,0,1,1,false,true)
  end    
--技能一，单体伤害，攻击被冰冻的敌人时伤害增加xx%
  if(skillInfo.skillData.skillId == 271 and target.Frozen) then
    local damgeRateDec = 0.5
    return damgeRateDec  
  end  
end
