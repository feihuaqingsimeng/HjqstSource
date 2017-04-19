--闪避增加
function dodge_105()
    return 0.20
end

function InitCharacterDatas_10(instanceID)
  local lastTime = 999
  fightdataTable['foreverBuffIcon'..instanceID] = {'skill/19',lastTime}
end
--自身闪避20%
