--天生10%暴击
function crit_1225()
	return 0.1
end

function InitCharacterDatas_122(instanceID)
  local lastTime = 999
  fightdataTable['foreverBuffIcon'..instanceID] = {'skill/52',lastTime}
end