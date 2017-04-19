--每次攻击时30%几率吸血，回复伤害的50%
function attackFinishBuff_1305(character, target, skillInfo, judgeType, actualHPValue)
  if(character == target) then return end
  local random = math.random()
  local probability = 1 --概率
  --print(random)
  --概率
  if(random < probability)  then
    local hpValue = actualHPValue * 0.5
    print('treat value',hpValue)
    Logic.Fight.Controller.MechanicsController.instance:SetTreatValue(character,hpValue,false)
  end
end

function InitCharacterDatas_130(instanceID)
  local lastTime = 999
  fightdataTable['foreverBuffIcon'..instanceID] = {'skill/58',lastTime}
end