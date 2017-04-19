--血量低于40%，暴击增加88%
function attackedBuff_85(character,target,skillInfo, judgeType, actualHPValue)
    local num = fightdataTable[character.characterInfo.instanceID]        
    if(num == nil) then		
      local lowRate = 0.4  --最低比率
      if(target.HP < target.maxHP * lowRate and target.HP > 0)  then
        num = 1
        fightdataTable[character.characterInfo.instanceID] = num
        --character:AddBuff(mechanics,buff类型,根据skill等级增加值或时间,buff增加百分比或固定值,基础时间,基础值,skillInfo.currentLevel)
        --local mechanics = Logic.Skill.Model.MechanicsData.GetMechanicsDataById(6055)
        local lastTime = 6000  --持续时间
        local crit = 0.88  --暴击比例
        target:AddBuff(character,target,skillInfo,null,Logic.Enums.BuffType.Crit,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Fixed,lastTime,crit,1,1,false,true)
      end
    end
end