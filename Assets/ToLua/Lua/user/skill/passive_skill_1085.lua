--同一列的友方角色受到的所有伤害减少xx%
function addHaloBuff_1085(character)
  local iter
  if(character.isPlayer)  then
      iter = Logic.Character.Controller.PlayerController.instance.heroDic:GetEnumerator()
    else
      iter = Logic.Character.Controller.EnemyController.instance.enemyDic:GetEnumerator()
  end
    
  --local mechanics = Logic.Skill.Model.MechanicsData.GetMechanicsDataById(10855)
  while iter:MoveNext() do
    local v = iter.Current.Value
    if (v.positionData.columnNum == character.positionData.columnNum) then
        local damageDec = 0.09 --伤害减少值
		print('id'..v.characterInfo.instanceID)
        v:AddBuff(character,v,null,null,Logic.Enums.BuffType.DamageDec,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.Fixed,999,damageDec,1,1,true,false)
    end
  end
end