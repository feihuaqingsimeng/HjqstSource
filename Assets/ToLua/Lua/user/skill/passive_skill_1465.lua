--敌全体，降低6%物攻魔攻（常驻）
function addHaloBuff_1295(character)  
  local iter
  if(character.isPlayer)  then
      iter = Logic.Character.Controller.EnemyController.instance.enemyDic:GetEnumerator()
   else
      iter = Logic.Character.Controller.PlayerController.instance.heroDic:GetEnumerator()
  end
  while iter:MoveNext() do
    local v = iter.Current.Value
    local magicAttack = -0.06 --魔攻
    v:AddBuff(character,v,null,null,Logic.Enums.BuffType.MagicAttack,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,0,magicAttack,1,1,true,false)
    end   
end