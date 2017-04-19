--自身HP40%以下物防增加20%
function attackedBuff_615(character,target,skillInfo,judgeType,actualHPValue)
    local num = fightdataTable[target.characterInfo.instanceID]        
    if(num == nil) then		
      local lowRate = 0.4  --最低比率
      if(target.HP < target.maxHP * lowRate and target.HP > 0)  then
        --character:AddBuff(mechanics,buff类型,根据skill等级增加值或时间,buff增加百分比或固定值,基础时间,基础值,skillInfo.currentLevel)
        local mechanics = Logic.Skill.Model.MechanicsData.GetMechanicsDataById(6155)
        local lastTime = 999  --持续时间
        local physicsDefense = 0.4  --物理防御比例
        target:AddBuff(target,target,skillInfo,mechanics,Logic.Enums.BuffType.PhysicsDefense,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,lastTime,physicsDefense,1,1,false,true)
        num = 1
        fightdataTable[target.characterInfo.instanceID] = num
      end
    end
end