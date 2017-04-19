--每次攻击命中目标时，增加物攻xx%，可叠加，有上限
function attackBuff_1435(character,target,skillInfo,judgeType)     
    local total = fightdataTable[character.characterInfo.instanceID]
    local lastTime = 6000 --持续时间
    local physicsAttack = 0.03 --每次增加值物攻
    local max = 0.25 --最高上限
    if (total) then
        total = total + physicsAttack
    else
        total = physicsAttack
    end
    if (total > max) then
        total = max
    end
    character:AddBuff(character,character,skillInfo,null,Logic.Enums.BuffType.PhysicsAttack,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,lastTime,total,1,1,false,true)
    fightdataTable[character.characterInfo.instanceID] = total
end