--自身，暴击时伤害为3倍
function attackBuff_1215(character,target,skillInfo,judgeType)  
   if(judgeType >= 3) then
     return 0.5
  end
  return 0   
end