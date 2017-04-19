function attackedBuff_505(character, target, skillInfo, judgeType, actualHPValue)
    local lowRate1 = 0.5  --最低比率
    local lowRate2 = 0.15  --
    if(target.HP < target.maxHP * lowRate1 and target.HP > target.maxHP * lowRate2)  then
      local num = fightdataTable[target.characterInfo.instanceID..'1']        
      if(num == nil) then		
        num = 1
        fightdataTable[target.characterInfo.instanceID..'1'] = num
        local transfromId = 1057  --heroTransform.csv transfromId
        local mechanicsId = 5055
        Logic.Fight.Controller.MechanicsController.instance:Transform(target,transfromId,mechanicsId)
      end
    elseif(target.HP < target.maxHP * lowRate2 and target.HP > 0)  then
      local num = fightdataTable[target.characterInfo.instanceID..'2']        
      if(num == nil) then		
        num = 1
        fightdataTable[target.characterInfo.instanceID..'2'] = num
        local transfromId = 1058  --heroTransform.csv transfromId
        local mechanicsId = 50556
        Logic.Fight.Controller.MechanicsController.instance:Transform(target,transfromId,mechanicsId)
      end
    end
end

function judgeMechanics_505(mechanicsType)
    if(mechanicsType == Logic.Enums.MechanicsType.ForceKill)  then
      return 0
    else
      return 1
    end
end
