--血量低于50%，吸血效果翻倍
function attackBuff_325(character,target,skillInfo,judgeType)  
    local lowRate = 0.5 --最低比率
    if(character.HP < character.maxHP * lowRate)  then
      if(skillInfo.skillData.skillId == 321)  then                  
          local lastTime = 1 --持续时间
          local drain = 0.5 --吸血值
          character:AddBuff(character,character,skillInfo,null,Logic.Enums.BuffType.Drain,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,lastTime,drain,1,1,false,true)
      end    
    end
end

--播放一个一直存在的吸血翻倍特效
function attackedBuff_325(character,target,skillInfo, judgeType, actualHPValue)
    local lowRate = 0.4  --最低比率
    if(target.HP < target.maxHP * lowRate and target.HP > 0)  then 
      local num = fightdataTable[character.characterInfo.instanceID]        
      if(num == nil) then		
          num = 1
          fightdataTable[character.characterInfo.instanceID] = num
          local mechanicsId = 3255 --效果id
          local mechanics = Logic.Skill.Model.MechanicsData.GetMechanicsDataById(mechanicsId)
          local lastTime = 999 --持续时间
          local drain = 0 --吸血值
          target:AddBuff(character,target,skillInfo,mechanics,Logic.Enums.BuffType.Drain,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,lastTime,drain,1,1,false,true)
      end
    end
end