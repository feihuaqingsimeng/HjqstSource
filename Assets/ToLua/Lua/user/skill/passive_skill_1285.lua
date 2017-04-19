--自身，物攻增加50%，战斗开场20秒
function addHaloBuff_1285(character)  
    local physicsAttack = 0.5 --物攻
    local lastTime = 20  --持续时间
    character:AddBuff(character,character,null,null,Logic.Enums.BuffType.PhysicsAttack,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,lastTime,physicsAttack,1,1,false,true)
end