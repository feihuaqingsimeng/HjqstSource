--消灭任意目标后，增加自身物攻18%，减少物防18%
function killerBuff_1095(character, skillInfo)
  local lastTime = 6000 --持续时间
  local physicsAttack = 0.18 --增加物攻    
  character:AddBuff(character,character,skillInfo,null,Logic.Enums.BuffType.PhysicsAttack,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,lastTime,physicsAttack,1,1,false,true)
  local physicsDefense = 0.18 --增加物防   
  character:AddBuff(character,character,skillInfo,null,Logic.Enums.BuffType.PhysicsDefense,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,lastTime,physicsDefense,1,1,false,true)
end