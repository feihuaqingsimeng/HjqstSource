--50%概率治疗效果翻倍
function treatBuff_175(target)
  local random = math.random()
  local probability = 0.5 --概率
  if(random < probability)  then
    return 0.5
  end    
end

function InitCharacterDatas_17(instanceID)
  fightdataTable["dispersefriendsbuff"..instanceID] = {
    Logic.Enums.BuffType.Swimmy,
    Logic.Enums.BuffType.Silence,
    Logic.Enums.BuffType.Blind,
    Logic.Enums.BuffType.Poisoning,
    Logic.Enums.BuffType.PhysicsDefense,
    Logic.Enums.BuffType.MagicDefense,
    Logic.Enums.BuffType.PhysicsAttack,
    Logic.Enums.BuffType.MagicAttack,
    Logic.Enums.BuffType.Hit,
    Logic.Enums.BuffType.Dodge,
    Logic.Enums.BuffType.Crit,
    Logic.Enums.BuffType.AntiCrit,
    Logic.Enums.BuffType.Block,
    Logic.Enums.BuffType.AntiBlock,
    Logic.Enums.BuffType.CritHurtAdd,
    Logic.Enums.BuffType.CritHurtDec,
    Logic.Enums.BuffType.Armor,
    Logic.Enums.BuffType.DamageDec,
    Logic.Enums.BuffType.DamageAdd,
    Logic.Enums.BuffType.Frozen,
    Logic.Enums.BuffType.Ignite,
    Logic.Enums.BuffType.Bleed,
    Logic.Enums.BuffType.Sleep,
    Logic.Enums.BuffType.Landification,
    Logic.Enums.BuffType.Tieup,
  }
  print('dispersefriendsbuff count:'..#fightdataTable["dispersefriendsbuff"..instanceID])
end