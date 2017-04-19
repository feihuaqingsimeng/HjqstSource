--自身血量未满时，每秒回复3%的生命值
function addHaloBuff_1035(character)
    local percent = 0.01  --回血比例
    character:AddBuff(character,character,null,null,Logic.Enums.BuffType.TreatPercent,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,999,percent,1,1,true,false)    
end

function InitCharacterDatas_103(instanceID)
  local lastTime = 999
  fightdataTable['foreverBuffIcon'..instanceID] = {'skill/32',lastTime}
end