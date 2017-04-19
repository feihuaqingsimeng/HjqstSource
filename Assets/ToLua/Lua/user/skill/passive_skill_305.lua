--每次攻击使敌人受到的伤害增加10%，持续8秒
function attackBuff_305(character,target,skillInfo,judgeType)
    local mechanics = Logic.Skill.Model.MechanicsData.GetMechanicsDataById(3055)
    local lastTime = 8 --持续时间
    local damageDec = -0.10 --伤害增加
    target:AddBuff(character,target,skillInfo,mechanics,Logic.Enums.BuffType.DamageDec,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,lastTime,damageDec,1,1,false,true)
end