--全体队友免疫中毒、流血、点燃等持续掉血技能
function addHaloBuff_1525(character)  
  local iter
  if(character.isPlayer)  then
      iter = Logic.Character.Controller.PlayerController.instance.heroDic:GetEnumerator()
   else
      iter = Logic.Character.Controller.EnemyController.instance.enemyDic:GetEnumerator()
  end
  --print(addHaloBuff_119)
  while iter:MoveNext() do
    local v = iter.Current.Value
    v:AddBuff(character,v,null,null,Logic.Enums.BuffType.Immune,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.None,999,0,1,1,true,false)
  end   
end

function InitCharacterDatas_152(instanceID)  
  --免疫buff列表
  local immunebuff = {
	Logic.Enums.BuffType.Poisoning,
	Logic.Enums.BuffType.Bleed,
	Logic.Enums.BuffType.Ignite  
  }
  fightdataTable['immunebuff'..instanceID] = immunebuff
end