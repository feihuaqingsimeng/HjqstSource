--敌全体，防御降低5%（常驻）并自身免疫2次攻击
function addHaloBuff_115(character)  
  local iter
  if(character.isPlayer)  then
    iter = Logic.Character.Controller.EnemyController.instance.enemyDic:GetEnumerator()
   else
    iter = Logic.Character.Controller.PlayerController.instance.heroDic:GetEnumerator()
  end
  while iter:MoveNext() do
    local v = iter.Current.Value
    local magicAttack = -0.05 --魔攻
    local magicDefense = -0.05 --魔防
    v:AddBuff(character,v,null,null,Logic.Enums.BuffType.MagicAttack,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,0,magicAttack,1,1,true,false)
    v:AddBuff(character,v,null,null,Logic.Enums.BuffType.MagicDefense,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,0,magicDefense,1,1,true,false)
  end   
  local lastCount = 2 --免疫次数
  character:AddBuff(character,character,null,null,Logic.Enums.BuffType.DamageImmuneCount,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.None,lastCount,0,0,1,1,false,false)
end