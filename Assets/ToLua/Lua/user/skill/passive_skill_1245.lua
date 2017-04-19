--每次命中目标，30%概率沉默目标9秒
function attackBuff_1245(character,target,skillInfo,judgeType)  
  local random = math.random()
  local probability = 0.3 --概率
  --print(random)
  --概率
  if(random < probability)  then
    local silence = 9  --沉默时间
    local mechanicsId = 12455 --效果id
    local mechanics = Logic.Skill.Model.MechanicsData.GetMechanicsDataById(mechanicsId)
    target:AddBuff(character,target,skillInfo,mechanics,Logic.Enums.BuffType.Silence,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.None,silence,0,1,1,false,true)
  end
end