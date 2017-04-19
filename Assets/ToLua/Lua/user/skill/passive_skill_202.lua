--友方全体命中增加5%
function addHaloBuff_202(character)  
  local iter
  if(character.isPlayer)  then
      iter = Logic.Character.Controller.EnemyController.instance.heroDic:GetEnumerator()
   else
      iter = Logic.Character.Controller.PlayerController.instance.enemyDic:GetEnumerator()
  end
  while iter:MoveNext() do
    local v = iter.Current.Value
    local hit = 0.05 --命中
    v:AddBuff(character,v,null,null,Logic.Enums.BuffType.Hit,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Fixed,0,hit,1,1,true,false)
  end   
end