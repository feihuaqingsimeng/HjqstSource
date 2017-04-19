--敌全体，物攻减少10%（常驻）
function addHaloBuff_1335(character)  
  local iter
  if(character.isPlayer)  then
      iter = Logic.Character.Controller.EnemyController.instance.enemyDic:GetEnumerator()
   else
      iter = Logic.Character.Controller.PlayerController.instance.heroDic:GetEnumerator()
  end
  while iter:MoveNext() do
    local v = iter.Current.Value
    local physicsAttack = -0.1 --物攻
    v:AddBuff(character,v,null,null,Logic.Enums.BuffType.PhysicsAttack,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,0,physicsAttack,1,1,true,false)
    end   
end