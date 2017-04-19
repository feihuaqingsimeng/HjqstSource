--自身免疫所有异常状态
function addHaloBuff_1475(character)
  character:AddBuff(character,character,null,null,Logic.Enums.BuffType.Immune, Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.None,0,0,1,1,true,false)
end


function InitCharacterDatas_147(instanceID)  
  --免疫buff列表
  local immunebuff = {Logic.Enums.BuffType.ForceKill}
  fightdataTable['immunebuff'..instanceID] = immunebuff
  --免疫buff icon
  --fightdataTable['immuneBuffIcon'..instanceID] = 'skill/48'
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
end