--自身免疫眩晕，并使友方群体首次伤害减免
function addBuff_1175(character,buffType)
    if(buffType == Logic.Enums.BuffType.Swimmy)  then
      return 0
    else
      return 1
    end
end

function addHaloBuff_1175(character)
  local killCount = fightdataTable[character.characterInfo.instanceID]
  if(killCount == nil)  then    --只执行一次
    local iter
    if(character.isPlayer)  then
        iter = Logic.Character.Controller.PlayerController.instance.heroDic:GetEnumerator()
      else
        iter = Logic.Character.Controller.EnemyController.instance.enemyDic:GetEnumerator()
    end
    
    --local mechanics = Logic.Skill.Model.MechanicsData.GetMechanicsDataById(11755)
    while iter:MoveNext() do
      local v = iter.Current.Value
      --v:AddBuff(character,v,null,null,Logic.Enums.BuffType.DamageImmuneCount,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.None,次数,0,0,1,1)
      local lastCount = 1 --免疫次数
      v:AddBuff(character,v,null,null,Logic.Enums.BuffType.DamageImmuneCount,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.None,lastCount,0,0,1,1,false,false)
    end
    
    killCount = 1
    fightdataTable[character.characterInfo.instanceID] = killCount
  end
end

function InitCharacterDatas_117(instanceID)
  local lastTime = 999
  fightdataTable['foreverBuffIcon'..instanceID] = {'skill/28',lastTime}
end