--击杀者buff
function killerBuff_1075(character,skillInfo)
  local iter
    if(character.isPlayer)  then
        iter = Logic.Character.Controller.PlayerController.instance.heroDic:GetEnumerator()
      else
        iter = Logic.Character.Controller.EnemyController.instance.enemyDic:GetEnumerator()
    end
    
  local mechanics = Logic.Skill.Model.MechanicsData.GetMechanicsDataById(10755)
  while iter:MoveNext() do
    local v = iter.Current.Value
    local treatPercent = 0.12 --回血比例值
    v:AddBuff(character,v,skillInfo,mechanics,Logic.Enums.BuffType.TreatPercent,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,1,treatPercent,1,1,false,true)
  end
end