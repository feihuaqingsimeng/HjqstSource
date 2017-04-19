--自身血量30%以下时，普攻附带即死效果，6%几率
function attackBuff_1345(character,target,skillInfo,judgeType)  
  if(skillInfo.skillData.skillId == 1340) then
    local lowRate = 0.3
    if(character.HP < character.maxHP * lowRate and character.HP > 0)  then
    local random = math.random()
      local probability = 0.06 --概率
      --print(random)
      --概率
      if(random < probability)  then
        Logic.Fight.Controller.MechanicsController.instance.SetDamageValue(character,target,skillInfo,target.HP,false,0,1)
      end
    end
  end
end

function attackedBuff_1345(character, target, skillInfo, judgeType, actualHPValue)
  local lowRate = 0.3  
  local num = fightdataTable['attackedBuff_1435_'..target.characterInfo.instanceID]  
  if(num == nil) then
	if(target.HP < target.maxHP * lowRate and target.HP > 0)  then
		local lastTime = 999
		target:AddBuffIcon('skill/59',lastTime)
		num = 1
		fightdataTable['attackedBuff_1435_'..target.characterInfo.instanceID] = num
	end
  end
end
--[[function InitCharacterDatas_134(instanceID)
  local lastTime = 999
  fightdataTable['foreverBuffIcon'..instanceID] = {'skill/59',lastTime}
end--]]