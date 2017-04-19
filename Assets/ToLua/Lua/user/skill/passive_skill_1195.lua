--友全体暴击增加6%
function addHaloBuff_1195(character)  
  local iter
  if(character.isPlayer)  then
      iter = Logic.Character.Controller.PlayerController.instance.heroDic:GetEnumerator()
   else
      iter = Logic.Character.Controller.EnemyController.instance.enemyDic:GetEnumerator()
  end
  --print(addHaloBuff_119)
    local mechanics = Logic.Skill.Model.MechanicsData.GetMechanicsDataById(88888888)
  while iter:MoveNext() do
    local v = iter.Current.Value
    local crit = 0.06 --暴击率
    v:AddBuff(character,v,null,mechanics,Logic.Enums.BuffType.Crit,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Fixed,0,crit,1,1,true,false)
  end   
end