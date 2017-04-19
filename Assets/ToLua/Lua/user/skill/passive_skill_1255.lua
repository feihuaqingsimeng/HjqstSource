--主角死亡时，自身全属性大幅提升
function friendDead_1255(target,character)
  if(target.isRole) then
    local addedCount = fightdataTable[character.characterInfo.instanceID]
    if(addedCount == nil)  then    --只执行一次
      local addRate = 0.12      
      local mechanicsId = 12555
      local mechanics = Logic.Skill.Model.MechanicsData.GetMechanicsDataById(mechanicsId)
      character:AddBuff(character,character,null,mechanics,Logic.Enums.BuffType.HPLimit,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,0,addRate,1,1,true,false)
      character:AddBuff(character,character,null,null,Logic.Enums.BuffType.PhysicsAttack,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,0,addRate,1,1,true,false)
      character:AddBuff(character,character,null,null,Logic.Enums.BuffType.MagicAttack,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,0,addRate,1,1,true,false)
      character:AddBuff(character,character,null,null,Logic.Enums.BuffType.PhysicsDefense,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,0,addRate,1,1,true,false)
      character:AddBuff(character,character,null,null,Logic.Enums.BuffType.MagicDefense,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,0,addRate,1,1,true,false)
      addedCount = 1
      fightdataTable[character.characterInfo.instanceID] = addedCount
    end
  end  
end

function InitCharacterDatas_125(instanceID)
  local lastTime = 999
  fightdataTable['foreverBuffIcon'..instanceID] = {'skill/53',lastTime}
end