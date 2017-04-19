--自身，伤害提升，场上队友越少伤害提升越高
function attackBuff_1385(character,target,skillInfo,judgeType)  
  local count = 0
  if(character.isPlayer)  then
      count = Logic.Character.Controller.PlayerController.instance.heroDic.Count
   else
      count = Logic.Character.Controller.EnemyController.instance.enemyDic.Count
  end
  
  local lowRate = 0.3
  if(count < 3 and character.HP > 0)  then
    return 2
  end
end