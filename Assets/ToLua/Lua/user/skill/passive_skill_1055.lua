--自身死亡复活
function dead_1055(character)
    local rebornNum = fightdataTable[character.characterInfo.instanceID]
    if(rebornNum == nil) then
        --print('reborn')
        rebornNum = 1
        local hpRate = 0.55  --复活百分比
        if(character.isPlayer) then
            Logic.Character.Controller.PlayerController.instance:Reborn(character,hpRate,3)
        else          
            Logic.Character.Controller.EnemyController.instance:Reborn(character,hpRate,3)
        end
        ---只是为了播放一个特效，没有任何作用
        local mechanics = Logic.Skill.Model.MechanicsData.GetMechanicsDataById(3655)
        character:AddBuff(character,character,null,mechanics,Logic.Enums.BuffType.MagicAttack,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,1,0,1,1,false,true)
        fightdataTable[character.characterInfo.instanceID] = rebornNum
        return 1
    else 
      return 0
    end      
end


function InitCharacterDatas_101(instanceID)
  local lastTime = 999
  fightdataTable['foreverBuffIcon'..instanceID] = {'skill/39',lastTime}
end