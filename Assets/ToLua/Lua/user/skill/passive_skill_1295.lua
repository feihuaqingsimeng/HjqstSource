--敌全体，降低6%魔防（常驻）
function addHaloBuff_1295(character)  
  local iter
  if(character.isPlayer)  then
      iter = Logic.Character.Controller.EnemyController.instance.enemyDic:GetEnumerator()
   else
      iter = Logic.Character.Controller.PlayerController.instance.heroDic:GetEnumerator()
  end
  while iter:MoveNext() do
    local v = iter.Current.Value
    local magicDefense = -0.06 --魔防
    v:AddBuff(character,v,null,null,Logic.Enums.BuffType.MagicDefense,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,0,magicDefense,1,1,true,false)
    end   
end