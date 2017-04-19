--每次普攻，弱点攻击
function findTarget_425()
   local random = math.random()
    local probablity = 0.5   --概率
    if(random < probablity)  then
      return 1
    end
  return 0
end

function InitCharacterDatas_42(instanceID)
  local lastTime = 999
  fightdataTable['foreverBuffIcon'..instanceID] = {'skill/38',lastTime}
end