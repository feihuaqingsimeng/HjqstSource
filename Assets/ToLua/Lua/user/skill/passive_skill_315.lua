--每次命中目标增加自身魔攻3%，最高提升25%
function attackBuff_315(character,target,skillInfo,judgeType)     
    local total = fightdataTable[character.characterInfo.instanceID]
    local lastTime = 6000 --持续时间
    local magicAttack = 0.03 --每次增加值魔攻
    local max = 0.25 --最高上限
    if (total) then
        total = total + magicAttack
    else
        total = magicAttack
    end
    if (total > max) then
        total = max
    end
    character:AddBuff(character,character,skillInfo,null,Logic.Enums.BuffType.MagicAttack,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,lastTime,total,1,1,false,true)
    fightdataTable[character.characterInfo.instanceID] = total
end