--每次攻击33%概率眩晕目标
function attackBuff_345(character,target,skillInfo,judgeType)
    local random = math.random()
    local probablity = 0.33   --概率
    --print(random)
    --概率
    if(random < probablity)  then
        local lastTime = 5  --持续时间
        target:AddBuff(character,target,skillInfo,null,Logic.Enums.BuffType.Swimmy,Logic.Enums.SkillLevelBuffAddType.Time,Logic.Enums.BuffAddType.None,lastTime,0,1,1,false,true)
    end
end

function InitCharacterDatas_34(instanceID)
  local lastTime = 999
  fightdataTable['foreverBuffIcon'..instanceID] = {'skill/55',lastTime}
end