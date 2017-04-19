--自身，免疫2次伤害，并友全体免疫即死
function addHaloBuff_1395(character)
  local lastCount = 2
  character:AddBuff(character,character,null,null,Logic.Enums.BuffType.DamageImmuneCount, Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.None,lastCount,0,0,1,1,false,false)
  local iter
  if(character.isPlayer)  then
    iter = Logic.Character.Controller.PlayerController.instance.heroDic:GetEnumerator()
  else
    iter = Logic.Character.Controller.EnemyController.instance.enemyDic:GetEnumerator()
  end
    
  --local mechanics = Logic.Skill.Model.MechanicsData.GetMechanicsDataById(10751)
  while iter:MoveNext() do
    local v = iter.Current.Value
    v:AddBuff(character,v,null,null,Logic.Enums.BuffType.Immune,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.None,0,0,1,1,true,false)
    v:AddBuffIcon('skill/48',999)
  end
end


function InitCharacterDatas_139(instanceID)  
  --免疫buff列表
  local immunebuff = {Logic.Enums.BuffType.ForceKill}
  fightdataTable['immunebuff'..instanceID] = immunebuff
  --免疫buff icon
  --fightdataTable['immuneBuffIcon'..instanceID] = 'skill/48'
end