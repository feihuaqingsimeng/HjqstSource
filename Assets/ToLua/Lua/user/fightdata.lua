
function InitFightdata()
    fightdataTable = {}
    --驱散友方buff列表
    --fightdataTable["dispersefriendsbuffs"] = 
    --{
    --  Logic.Enums.BuffType.Swimmy,
    --Logic.Enums.BuffType.Ignite
    --}
    --驱散敌方buff列表
    --fightdataTable["disperseenemiesbuffs"] = 
    --{
    --    Logic.Enums.BuffType.Treat   
    --}
        
--    --免疫buff列表
--    fightdataTable["immunebuff"] =
--    {
--        Logic.Enums.BuffType.Swimmy
--    }
    
    InitBuffIcons()
    --InitRandomBuffs()
    --InitExceptionBuffs()
    
    print("init fight data ..")
end

function DestroyFightData()
  fightdataTable = nil
  print("clear fight data ..")
end

function InitBuffIcons()
  fightdataTable['buffIcons'] = 
  {
    {Logic.Enums.BuffType.Swimmy,0,'skill/24'},    
    {Logic.Enums.BuffType.Invincible,1,'skill/36'},
    {Logic.Enums.BuffType.Silence,0,'skill/40'},
    {Logic.Enums.BuffType.Blind,0,'skill/43'},
    {Logic.Enums.BuffType.Poisoning,0,'skill/45'},
    {Logic.Enums.BuffType.Treat,0,'skill/32'},
    {Logic.Enums.BuffType.Shield,1,'skill/22'},
    {Logic.Enums.BuffType.Drain,1,'skill/58'},
    {Logic.Enums.BuffType.PhysicsDefense,1,'skill/11'},
    {Logic.Enums.BuffType.PhysicsDefense,0,'skill/12'},
    {Logic.Enums.BuffType.MagicDefense,1,'skill/13'},
    {Logic.Enums.BuffType.MagicDefense,0,'skill/14'},
    {Logic.Enums.BuffType.PhysicsAttack,1,'skill/07'},	
    {Logic.Enums.BuffType.PhysicsAttack,0,'skill/08'},		
    {Logic.Enums.BuffType.MagicAttack,1,'skill/09'},	
    {Logic.Enums.BuffType.MagicAttack,0,'skill/10'},		
    {Logic.Enums.BuffType.Hit,0,'skill/21'},		
    {Logic.Enums.BuffType.Dodge,1,'skill/19'},
    {Logic.Enums.BuffType.Dodge,0,'skill/20'},
    {Logic.Enums.BuffType.Crit,1,'skill/15'},
    {Logic.Enums.BuffType.Crit,0,'skill/16'},
    {Logic.Enums.BuffType.Block,1,'skill/17'},
    {Logic.Enums.BuffType.Block,0,'skill/18'},	
    {Logic.Enums.BuffType.Frozen,0,'skill/33'},
		
		
		
    {Logic.Enums.BuffType.ForceKill,0,'skill/59'},	
    {Logic.Enums.BuffType.Landification,0,'skill/31'},
	
    {Logic.Enums.BuffType.Ignite,0,'skill/25'},
    {Logic.Enums.BuffType.Bleed,0,'skill/26'},
	
    {Logic.Enums.BuffType.DamageImmuneCount,1,'skill/47'},
	
	{Logic.Enums.BuffType.DamageAdd,1,'skill/01'},
	
	{Logic.Enums.BuffType.TreatAdd,0,'skill/34'},
	
	{Logic.Enums.BuffType.Rebound,1,'skill/37'},
	
	{Logic.Enums.BuffType.DamageDec,1,'skill/02'},
	{Logic.Enums.BuffType.DamageDec,0,'skill/03'},
	
	{Logic.Enums.BuffType.Tag,0,'skill/46'},	

    {Logic.Enums.BuffType.GeneralSkillHit,1,'skill/61'},
    {Logic.Enums.BuffType.GeneralSkillCrit,1,'skill/62'},
  }
  
end

function GetForeverBuffIcon(character)
  local r = fightdataTable['foreverBuffIcon'..character.characterInfo.instanceID]
  if(r ~= nil) then
    return unpack(r)
  end
end

function GetImmuneBuffIcon(character)
  local r = fightdataTable['immuneBuffIcon'..character.characterInfo.instanceID]
  if(r ~= nil) then
    return r
  end
end

function GetDisperseFriendsBuffs(character)
  local r = fightdataTable["dispersefriendsbuff"..character.characterInfo.instanceID]
  if(r ~= nil) then
    return unpack(r)
  end
end

function GetDisperseEnemiesBuffs(character)
  local r = fightdataTable["disperseenemiesbuff"..character.characterInfo.instanceID]
  if(r ~= nil) then
    return unpack(r)
  end
end

function GetImmuneBuffs(character)
  local r = fightdataTable['immunebuff'..character.characterInfo.instanceID]
  if(r ~= nil) then
    return unpack(r)
  end
end

function GetRandomBuffs(character,index)
  local r = fightdataTable["randombuffs"..character.characterInfo.instanceID..'_'..index]
  if(r ~= nil) then
    return unpack(r)
  end
end

function GetExceptionBuffs(character,index)
  local r = fightdataTable["exceptionBuffs"..character.characterInfo.instanceID..'_'..index]
  if(r ~= nil) then
    return unpack(r)
  end
end
--[[function InitRandomBuffs()
    --初始化随机buff库1
    fightdataTable["randombuffs11551"] = 
    {
        --buff类型,时间,数值,buff随机概率
        {Logic.Enums.MechanicsType.Swimmy,100,0,0.5},
        {Logic.Enums.MechanicsType.Ignite,100,0.3,0.5}
    }
    
    --初始化随机buff库2
    fightdataTable["randombuffs11552"] = 
    {
        --buff类型,时间,数值,buff随机概率
        {Logic.Enums.MechanicsType.Swimmy,100,0,0.5},
        {Logic.Enums.MechanicsType.Ignite,100,0.3,0.5}
    }
end--]]

--异常状态库
--[[function InitExceptionBuffs()
  fightdataTable["exceptionBuffs"] = 
  {
     Logic.Enums.MechanicsType.Swimmy,
     
  }  
end--]]
