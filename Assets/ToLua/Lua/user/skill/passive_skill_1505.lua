--血量减少的百分比，转化为自身的防御力百分比增加
function physicsDefense_1505(character)
	local result = character.HP / character.maxHP
  print('add physicsDefense'..tostring(result))
  return 1 - result
end

function magicDefense_1505(character)
	local result = character.HP / character.maxHP
  print('add magicDefense'..tostring(result))
  return 1 - result
end