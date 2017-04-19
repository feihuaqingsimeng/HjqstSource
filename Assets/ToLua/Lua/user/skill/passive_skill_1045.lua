--友全体，命中增加10%（常驻）
function addHaloBuff_1045(character)  
  local iter
  if(character.isPlayer)  then
      iter = Logic.Character.Controller.PlayerController.instance.heroDic:GetEnumerator()
   else
      iter = Logic.Character.Controller.EnemyController.instance.enemyDic:GetEnumerator()
  end
  while iter:MoveNext() do
    local v = iter.Current.Value
    local hit = 0.1 --命中
    v:AddBuff(character,v,null,null,Logic.Enums.BuffType.Hit,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Fixed,0,hit,1,1,true,false)
    end   
end