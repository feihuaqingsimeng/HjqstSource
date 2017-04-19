--自身血量未满时，每秒回复5%的生命值
--每次播放技能(包括普攻)
function playSkill_1000605(character)
    local percent = 1  --回血比例
    character:AddBuff(character,character,null,null,Logic.Enums.BuffType.TreatPercent,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,1,percent,1,1,false,true)    
end
