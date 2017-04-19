--友全体，受到的魔法伤害减少10%（常驻）
function addHaloBuff_1315(character)  
  local iter
  if(character.isPlayer)  then
      iter = Logic.Character.Controller.PlayerController.instance.heroDic:GetEnumerator()
   else
      iter = Logic.Character.Controller.EnemyController.instance.enemyDic:GetEnumerator()
  end
  while iter:MoveNext() do
    local v = iter.Current.Value
    local magicDefense = 0.1 --魔防
    v:AddBuff(character,v,null,null,Logic.Enums.BuffType.MagicDefense,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,0,magicDefense,1,1,true,false)
    end   
end