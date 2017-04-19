--友全体暴击增加3%，伤害增加3%
function addHaloBuff_665(character)
  local iter
  if(character.isPlayer)  then
      iter = Logic.Character.Controller.PlayerController.instance.heroDic:GetEnumerator()
    else
      iter = Logic.Character.Controller.EnemyController.instance.enemyDic:GetEnumerator()
  end
    
  --local mechanics = Logic.Skill.Model.MechanicsData.GetMechanicsDataById(10855)
  while iter:MoveNext() do
    local v = iter.Current.Value
    local crit = 0.03
    local damageAdd = 0.03 --伤害增加值
    v:AddBuff(character,v,null,null,Logic.Enums.BuffType.DamageAdd,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Fixed,999,damageAdd,1,1,true,false)
    v:AddBuff(character,v,null,null,Logic.Enums.BuffType.Crit,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Fixed,999,crit,1,1,true,false)
  end
end