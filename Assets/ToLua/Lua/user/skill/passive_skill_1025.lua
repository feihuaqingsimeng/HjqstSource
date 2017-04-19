--友全体魔攻增加3%，魔防增加3%（常驻）
function addHaloBuff_1025(character)  
  local iter
  if(character.isPlayer)  then
      iter = Logic.Character.Controller.PlayerController.instance.heroDic:GetEnumerator()
   else
      iter = Logic.Character.Controller.EnemyController.instance.enemyDic:GetEnumerator()
  end
  while iter:MoveNext() do
    local v = iter.Current.Value
    local magicAttack = 0.03 --魔攻
    local magicDefense = 0.03 --魔防
    v:AddBuff(character,v,null,null,Logic.Enums.BuffType.MagicAttack,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,0,magicAttack,1,1,true,false)
    v:AddBuff(character,v,null,null,Logic.Enums.BuffType.MagicDefense,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,0,magicDefense,1,1,true,false)
  end   
end