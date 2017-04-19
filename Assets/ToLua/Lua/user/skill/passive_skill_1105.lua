--敌全体，150%伤害，当自身死亡时
function dead_1105(character)
  local iter
  if(character.isPlayer)  then
      iter = Logic.Character.Controller.EnemyController.instance.enemyDic:GetEnumerator()
   else
      iter = Logic.Character.Controller.PlayerController.instance.heroDic:GetEnumerator()
  end
  
  local mechanicsEffectId = 11055 --效果id
  local mechanicsEffect = Logic.Skill.Model.MechanicsData.GetMechanicsDataById(mechanicsEffectId)
  local lastTime = 999 --持续时间  
  character:AddBuff(character,character,null,mechanicsEffect,Logic.Enums.BuffType.Drain,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,lastTime,0,1,1,false,true)
  
  while iter:MoveNext() do
    local v = iter.Current.Value
    local skillId = 1100
    local mechanicsId = 11001
    local delay = 2.5
    local mechanicsValueA = 1.5
    local mechanicsValueB = 1
    local mechanicsValueC = 1
    local skillInfo = character:GetSkillInfoById(skillId)
    local mechanics = Logic.Skill.Model.MechanicsData.GetMechanicsDataById(mechanicsId)
    Logic.Fight.Controller.MechanicsController.instance:CalcDamage(character,v,skillInfo,mechanics,1,mechanicsValueA,mechanicsValueB,mechanicsValueC,delay)
  end   
  return 0
end