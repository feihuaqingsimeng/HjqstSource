--自身血量越低，攻击越高
function attackBuff_1365(character,target,skillInfo,judgeType)  
  local lowRate = 0.3
  if(character.HP < character.maxHP * lowRate and character.HP > 0)  then
    return 2
  end
end