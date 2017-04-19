--血量低于40%，免疫控制
function addBuff_595(character,buffType)
    local lowRate = 0.4  --最低比率
    if(character.HP < character.maxHP * lowRate)  then
      if(buffType == Logic.Enums.BuffType.Swimmy or buffType == Logic.Enums.BuffType.Frozen or buffType == Logic.Enums.BuffType.Sleep or buffType == Logic.Enums.BuffType.Landification or buffType == Logic.Enums.BuffType.Tieup)  then
        local num = fightdataTable[character.characterInfo.instanceID] 
        if(num == nil) then

          character:AddBuff(character,target,null,null,Logic.Enums.BuffType.None,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.None,999,0,1,1,true,false)
          num = 1
          fightdataTable[character.characterInfo.instanceID] = num
        end
        return 0
      else
        return 1
      end
    end
	return 1
end

function attackedBuff_595(character, target, skillInfo, judgeType, actualHPValue)
	local num = fightdataTable['attackedBuff_595_'..target.characterInfo.instanceID] 
    if(num == nil) then
	  local lowRate = 0.4  --最低比率
	  if(target.HP < target.maxHP * lowRate)  then
		num = 1
		target:AddBuffIcon('skill/28',999)
		local mechanics = Logic.Skill.Model.MechanicsData.GetMechanicsDataById(5955)
		local lastTime = 6000  --持续时间
		target:AddBuff(target,target,null,mechanics,Logic.Enums.BuffType.PhysicsAttack,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.None,lastTime,0,1,1,false,true)		
		fightdataTable['attackedBuff_595_'..target.characterInfo.instanceID] =  num
	  end
	end
end
