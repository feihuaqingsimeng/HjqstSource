--血量低于40%时间增加魔法攻击力
function attackedBuff_525(character,target,skillInfo,judgeType,actualHPValue)
  local num = fightdataTable[character.characterInfo.instanceID]        
  if(num == nil) then		      
    local lowRate = 0.4  --最低比率
    if(target.HP < target.maxHP * lowRate and target.HP > 0)  then
      num = 1
      fightdataTable[character.characterInfo.instanceID] = num
      --character:AddBuff(mechanics,buff类型,根据skill等级增加值或时间,buff增加百分比或固定值,基础时间,基础值,skillInfo.currentLevel)
      local mechanics = Logic.Skill.Model.MechanicsData.GetMechanicsDataById(1021)
      local lastTime = 6000  --持续时间
      local magicAttack = 0.32  --魔法攻击比例
      target:AddBuff(character,target,skillInfo,mechanics,Logic.Enums.BuffType.MagicAttack,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,lastTime,magicAttack,1,1,false,true)
      end
    end
end

function judgeMechanics_525(mechanicsType)
    if(mechanicsType == Logic.Enums.MechanicsType.ForceKill)  then
      return 0
    else
      return 1
    end
end
