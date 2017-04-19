--自身，免疫浮空，并使友方全体受到的物理伤害减少10%（常驻）
function addHaloBuff_1375(character)  
  local iter
  if(character.isPlayer)  then
      iter = Logic.Character.Controller.PlayerController.instance.heroDic:GetEnumerator()
   else
      iter = Logic.Character.Controller.EnemyController.instance.enemyDic:GetEnumerator()
  end
  while iter:MoveNext() do
    local v = iter.Current.Value
    local physicsDefense = 0.1 --物防
    v:AddBuff(character,v,null,null,Logic.Enums.BuffType.PhysicsDefense,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,0,physicsDefense,1,1,true,false)
    end   
end

function addBuff_1375(character,buffType)
    if(buffType == Logic.Enums.BuffType.Float)  then
      return 0
    else
      return 1
    end
end
