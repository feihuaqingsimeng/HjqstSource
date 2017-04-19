--友方全体物攻增加8%
function addHaloBuff_203(character)  
  local iter
  if(character.isPlayer)  then
      iter = Logic.Character.Controller.EnemyController.instance.heroDic:GetEnumerator()
   else
      iter = Logic.Character.Controller.PlayerController.instance.enemyDic:GetEnumerator()
  end
  while iter:MoveNext() do
    local v = iter.Current.Value
    local physicsAttack = 0.08 --物攻
    v:AddBuff(character,v,null,null,Logic.Enums.BuffType.PhysicsAttack,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,0,physicsAttack,1,1,true,false)
    end   
end