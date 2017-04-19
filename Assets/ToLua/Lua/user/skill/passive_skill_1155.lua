--友全体，每秒回复血量上限的2%（常驻）
function addHaloBuff_1155(character)
  local iter
  if(character.isPlayer)  then
    iter = Logic.Character.Controller.PlayerController.instance.heroDic:GetEnumerator()
   else
    iter = Logic.Character.Controller.EnemyController.instance.enemyDic:GetEnumerator()
  end
  while iter:MoveNext() do
    local v = iter.Current.Value
    local percent = 0.005  --回血比例
    v:AddBuff(character,v,null,null,Logic.Enums.BuffType.TreatPercent,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Percent,999,percent,1,1,true,false)  end       
end