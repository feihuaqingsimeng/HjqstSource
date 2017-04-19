--自身，魔力提高15%，主角在场时
function magicDefense_1015(character)
  local defense = 0
  if(character.isPlayer)  then    
    local iter = Logic.Character.Controller.PlayerController.instance.heroDic:GetEnumerator()
    while iter:MoveNext() do
      local v = iter.Current.Value
      if(v.isRole) then
        defense = 0.15
      end
    end
  end  
  return defense
end

function InitCharacterDatas_101(instanceID)
  local lastTime = 999
  --fightdataTable['foreverBuffIcon'..instanceID] = {'skill/19',lastTime}
end