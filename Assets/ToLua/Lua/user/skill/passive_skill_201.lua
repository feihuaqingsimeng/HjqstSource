--友方全体魔攻增加8%
function addHaloBuff_201(character)  
  local iter
  if(character.isPlayer)  then
      iter = Logic.Character.Controller.EnemyController.instance.heroDic:GetEnumerator()
   else
      iter = Logic.Character.Controller.PlayerController.instance.enemyDic:GetEnumerator()
  end
  while iter:MoveNext() do
    local v = iter.Current.Value
    local magicAttack = 0.08 --魔攻
    v:AddBuff(character,v,null,null,Logic.Enums.BuffType.MagicAttack,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,0,magicAttack,1,1,true,false)
    end   
end