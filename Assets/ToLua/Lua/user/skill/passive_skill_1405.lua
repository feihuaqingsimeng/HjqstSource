--敌全体，降低5%命中
function addHaloBuff_1405(character)
  local addCount = fightdataTable[character.characterInfo.instanceID]
  if(addCount == nil)  then    --只执行一次
    local iter
    if(character.isPlayer)  then
        iter = Logic.Character.Controller.EnemyController.instance.enemyDic:GetEnumerator()
      else
        iter = Logic.Character.Controller.PlayerController.instance.heroDic:GetEnumerator()
    end
    
    --local mechanics = Logic.Skill.Model.MechanicsData.GetMechanicsDataById(11755)
    while iter:MoveNext() do
      local v = iter.Current.Value
      --v:AddBuff(character,v,null,null,Logic.Enums.BuffType.DamageImmuneCount,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.None,次数,0,0,1,1)
      local hitRate = -0.05 --减少命中几率
      v:AddBuff(character,v,null,null,Logic.Enums.BuffType.Hit,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Fixed,0,hitRate,1,1,true,false)
    end
    
    addCount = 1
    fightdataTable[character.characterInfo.instanceID] = addCount
  end
end