local t = {}
local name = 'role_model'

local function Start ()
  gamemanager.RegisterModel(name, t)
end



---------计算战力---------------
function t.CalcRolePower(roleInfo)
  local global_data = gamemanager.GetData('global_data')
  local illustration_model = gamemanager.GetModel('illustration_model')
  if roleInfo == nil then return 0 end
  local correction = roleInfo.heroData.correction
  local powerBasic = gamemanager.GetData('global_data').powerBasic
  --等级
  --local levelFactor = roleInfo.level
  --[[if roleInfo.level <= 50 then
    levelFactor = roleInfo.level-1
  else
    levelFactor = 2*roleInfo.level-51
  end]]
  --强化
  local strengthenNeedData = gamemanager.GetData('hero_strengthen_need_data').GetDataById(roleInfo.strengthenLevel-1)
  local aggFactor = 0
  if strengthenNeedData ~= nil then
    aggFactor = strengthenNeedData.aggr_value
  end
  --羁绊
  local relationsFactor = 0
  
  if roleInfo.relations then
    for k,v in pairs(roleInfo.relations) do
      local hero_relationship_data = gamemanager.GetData('hero_relationship_data').GetDataById(v.id)
      for k1,v1 in pairs(hero_relationship_data.comat) do
        relationsFactor = relationsFactor + v1
      end
    end
    
  end
  
  local rolePower = correction/100.0*15*(powerBasic+(roleInfo.level-1+aggFactor) * 2)* global_data.starAttr[roleInfo.advanceLevel]*roleInfo.heroData.quality_attr * (1 + relationsFactor) + illustration_model.power
  
  --print('power:',rolePower,'correction',correction,'powerBasic',powerBasic,'level',roleInfo.level,'aggFactor',aggFactor,'star',roleInfo.advanceLevel,'starAttr',global_data.starAttr[roleInfo.advanceLevel],'quality_attr',roleInfo.heroData.quality_attr,'relationsFactor',relationsFactor,'illustrationPower',illustration_model.power)
  
  --print('roleInfo:power',rolePower)
 -- rolePower = math.floor(rolePower)*(1+relationsFactor)
  --------------need add equipment power here-------------
  return math.floor(rolePower)
end

function t.GetStrengthenAddShowValueString(roleInfo)
  local stage = roleInfo:RoleStrengthenStage()
  local show = ''
  local showLevel = roleInfo:GetStrengthenAddShowValue()
  if showLevel > 0 then
    show = string.format('+%d',showLevel)
  end
  return t.FormatStrengthenLevelColor(stage,show)
end
---强化颜色（字符串）
function t.FormatStrengthenLevelColor(strengthenStage,s)
  local stage = strengthenStage
  local colorFormatString = ''
   if stage == 0 then   --白
    colorFormatString = '<color=#D3D3D3>%s</color>'
  elseif stage == 1 then --绿
    colorFormatString = '<color=#57D313>%s</color>'
  elseif stage == 2 then --蓝
    colorFormatString = '<color=#208EFF>%s</color>'
  elseif stage == 3 then --紫
    colorFormatString = '<color=#DC53D5>%s</color>'
  elseif stage == 4 then -- 橙
    colorFormatString = '<color=#FF7B06>%s</color>'
  end
  return string.format(colorFormatString,s)
end
---强化颜色名
function t.GetStrengthenLevelColorName(strengthenLv)
  local data = gamemanager.GetData('hero_strengthen_need_data').GetDataById(strengthenLv-1)
  local stage = 0
  if data ~= nil then
    stage = data.color
  end
  local str = ''
  if stage == 0 then   --白
    str = LocalizationController.instance:Get("common.color_White")
  elseif stage == 1 then --绿
    str = LocalizationController.instance:Get("common.color_Green")
  elseif stage == 2 then --蓝
    str = LocalizationController.instance:Get("common.color_Blue")
  elseif stage == 3 then --紫
    str = LocalizationController.instance:Get("common.color_Purple")
  elseif stage == 4 then -- 橙
    str = LocalizationController.instance:Get("common.color_Orange")
  end
  return str
end
---强化颜色（color）
function t.GetRoleNameColor(roleStrengthenStage)
  local color = ''
  local stage = roleStrengthenStage
  if stage == 0 then   --白
    color = Color(211.0 / 255, 211.0 / 255, 211.0 / 255)
  elseif stage == 1 then --绿
    color = Color(87.0 / 255, 211.0 / 255, 19.0 / 255)
  elseif stage == 2 then --蓝
    color = Color(32.0 / 255, 142.0 / 255, 255.0 / 255)
  elseif stage == 3 then --紫
    color = Color(220.0 / 255, 83.0 / 255, 213.0 / 255)
  elseif stage == 4 then -- 橙
    color = Color(255.0 / 255, 123.0 / 255, 6.0 / 255)
  end
  return color
end
--英雄名+strengthenLevel
function t.GetRoleNameWithStrengthenLevel(roleInfo)
  local roleNameStr = LocalizationController.instance:Get(roleInfo.heroData.name)
  local strengthenStage = roleInfo:RoleStrengthenStage()
  local showLevel = roleInfo:GetStrengthenAddShowValue()
  local str = nil
  if showLevel >  0 then
    str = string.format('%s+%d',roleNameStr,showLevel)
  else
    str = roleNameStr
  end
  return t.FormatStrengthenLevelColor(strengthenStage,str)
end

--英雄强化加几
function t.GetStrengthenAddShowValue(strengthenLv)
  local data = gamemanager.GetModel('hero_strengthen_need_data').GetDataById(strengthenLv-1)
  if data ~= nil then
    return data.strengthenAddShowValue
  end
  return 0
end
--英雄攻击类型（物理、魔法）
function t.GetRoleAttackAttributeType(roleType)
  local attrType = RoleAttackAttributeType.MagicalAttack 
  if roleType == RoleType.Defence or roleType == RoleType.Offence or roleType == RoleType.Mighty then
    attrType = RoleAttackAttributeType.PhysicalAttack
  end
  return attrType
end

--获得英雄主属性（不包含装备）
function t.CalcRoleMainAttributesList(roleInfo)
  local attrDic = t.CalcRoleAttributesDic(roleInfo)
  local roleAttackType = t.GetRoleAttackAttributeType(roleInfo.heroData.roleType)
  
  local mainAttrList = {}
  local index = 0
  
  for k,v in pairs(attrDic) do
    if v.type == RoleAttributeType.HP then
      mainAttrList[index] = v
      index = index+1
    elseif v.type == RoleAttributeType.MagicAtk and roleAttackType == RoleAttackAttributeType.MagicalAttack then
      mainAttrList[index] = v
      index = index+1
    elseif  v.type == RoleAttributeType.NormalAtk and roleAttackType == RoleAttackAttributeType.PhysicalAttack then
      mainAttrList[index] = v
      index = index+1
    elseif v.type == RoleAttributeType.Def then
      mainAttrList[index] = v
      index = index+1
    end
  end
  return mainAttrList
end

--计算英雄属性（不包含装备）RoleAttributeType, RoleAttribute
function t.CalcRoleAttributesDic(roleInfo)
  local role_attr = require('ui/role/model/role_attr')
  
  local roleAttrDic =  t.CalcRoleAttributesDicByHeroData(roleInfo.heroData,roleInfo.level,roleInfo.advanceLevel,roleInfo.strengthenLevel)

  local AddAttrDic = {}
  --羁绊属性加成
  local hero_relationship_data = gamemanager.GetData('hero_relationship_data')
  if roleInfo.relations ~= nil then
    for k,v in ipairs( roleInfo.relations) do
      local shipData = hero_relationship_data.GetDataById(v.id)
      if shipData ~= nil then
        for k1,v1 in pairs(shipData.attr) do
          if AddAttrDic[v1.type] then--存在累加
            AddAttrDic[v1.type].value = AddAttrDic[v1.type].value + v1.value
          else
            AddAttrDic[v1.type] = role_attr.New(v1.type,v1.value)
          end
        end
      end
    end
  end
  --图鉴属性加成
    local illustration_model = gamemanager.GetModel('illustration_model')
    for k,v in pairs(illustration_model.IllustrationAddAttrDictionary:GetDatas()) do
        if AddAttrDic[v.type] then--存在累加
          AddAttrDic[v.type].value = AddAttrDic[v.type].value + v.value
        else
          AddAttrDic[v.type] = role_attr.New(v.type,v.value)
        end
    end
    ---------------------------------完美分割线哟-------------------------------------------------
     --固定加成
    for k,v in pairs(AddAttrDic) do
      if k < RoleAttributeType.HPPercent then --百分比另算
        roleAttrDic[k].value = roleAttrDic[k].value + v.value
      end
    end
    --百分比加成
    for k,v in pairs(AddAttrDic) do
      
      if k == RoleAttributeType.HPPercent then
        roleAttrDic[RoleAttributeType.HP].value = roleAttrDic[RoleAttributeType.HP].value * (1+v.value)
      elseif k == RoleAttributeType.NormalAtkPercent then
        roleAttrDic[RoleAttributeType.NormalAtk].value = roleAttrDic[RoleAttributeType.NormalAtk].value * (1+v.value)
      elseif k == RoleAttributeType.MagicAtkPercent then
        roleAttrDic[RoleAttributeType.MagicAtk].value = roleAttrDic[RoleAttributeType.MagicAtk].value * (1+v.value)  
      elseif k == RoleAttributeType.DefPercent then
        roleAttrDic[RoleAttributeType.Def].value = roleAttrDic[RoleAttributeType.Def].value * (1+v.value) 
      end
    end

  return roleAttrDic
end
--计算英雄属性（不包含装备）RoleAttributeType, RoleAttribute
function t.CalcRoleAttributesDicByHeroData(heroData,level ,advanceLevel,strengthenLevel)
  if heroData == nil then
    return nil
  end
  local hero_strengthen_need_data = gamemanager.GetData('hero_strengthen_need_data')
  local role_attr = require('ui/role/model/role_attr')
  local global_data = gamemanager.GetData('global_data')
  
  local attributeDic = {}
  local factor = 0-- 成长系数
  local canGrowUp = false
  local basic = 0
  for i = 0,RoleAttributeType.MAX do
    canGrowUp = false
    basic = 0
    if i == RoleAttributeType.HP then
      basic = heroData.hp
      factor = heroData.hp_add/1000
      canGrowUp = true
    elseif i == RoleAttributeType.NormalAtk then
      basic = heroData.normal_atk
      factor = heroData.normal_atk_add/1000
      canGrowUp = true
    elseif i == RoleAttributeType.MagicAtk then
      basic = heroData.magic_atk
      factor = heroData.magic_atk_add/1000
      canGrowUp = true
    elseif i == RoleAttributeType.Def then
      basic = heroData.normal_def
      factor = heroData.normal_def_add/1000
      canGrowUp = true
    elseif i == RoleAttributeType.Speed then
      basic = heroData.speed
    elseif i == RoleAttributeType.Hit then
      basic = heroData.hit
    elseif i == RoleAttributeType.Dodge then
      basic = heroData.dodge
    elseif i == RoleAttributeType.Crit then
      basic = heroData.crit
    elseif i == RoleAttributeType.AntiCrit then
      basic = heroData.anti_crit
    elseif i == RoleAttributeType.Block then
      basic = heroData.block
    elseif i == RoleAttributeType.AntiBlock then
      basic = heroData.anti_block
    elseif i == RoleAttributeType.CounterAtk then
      basic = heroData.counter_atk
    elseif i == RoleAttributeType.CritHurtAdd then
      basic = heroData.crit_hurt_add
    elseif i == RoleAttributeType.CritHurtDec then
      basic = heroData.crit_hurt_dec
    elseif i == RoleAttributeType.Armor then
      basic = heroData.armor
    elseif i == RoleAttributeType.DamageDec then
      basic = heroData.damage_dec
    elseif i == RoleAttributeType.DamageAdd then
      basic = heroData.damage_add
    end
    local multiple = 2 
    if level <= 50 then
      multiple = 1
    end
    local total = basic
    if canGrowUp then
      local data = hero_strengthen_need_data.GetDataById(strengthenLevel -1)
      local aggFactor = 0
      if data ~= nil then
        aggFactor = data.aggr_value
      end
      --print('属性','total:',total,'basic:',basic,'level:',level,'multiple:',multiple,'factor',factor,'aggfactor',aggFactor,'star:',advanceLevel,'quality:',heroData.quality_attr)
      
      total = (basic + (level * multiple-1-50*(multiple-1))*factor +  aggFactor*factor) * global_data.GetStarAttr(advanceLevel) * heroData.quality_attr
      
    end
    local roleAttr = role_attr.New(i,total)
    attributeDic[i] = roleAttr
  end
  return attributeDic
end
--计算装备提升的属性（只是提升的属性，不包含英雄自身属性）
function t.CalcRoleAttributesDicByEquip(roleInfo)
  local equip_model = gamemanager.GetModel('equip_model')
  local equipList = {}
  equipList[1] = equip_model.GetEquipmentInfoByInstanceID(roleInfo.armorID)
  equipList[2] = equip_model.GetEquipmentInfoByInstanceID(roleInfo.accessoryID)
  equipList[3] = equip_model.GetEquipmentInfoByInstanceID(roleInfo.weaponID)
  
  return t.CalcRoleAttributesDicByEquipList(roleInfo,equipList)
end

--key: index,value:equipInfo  计算装备提升的属性（只是提升的属性，不包含英雄自身属性）
function t.CalcRoleAttributesDicByEquipList(roleInfo,equipInfoTable)
  
  local equipAttrList = {}
  for k,v in pairs(equipInfoTable) do
    
    if v ~= nil then
      local tempList = v:GetAllAttribute()
      for k1,v1 in pairs(tempList) do
        ui_util.CheckSameAttribute(equipAttrList,v1)
      end
    end
  end
  local roleAttrList = t.CalcRoleMainAttributesList(roleInfo)
  return t.CalcRoleAttrByEquipAttr(roleAttrList,equipAttrList)
end
local function GetRoleAttrValue(roleAttr)
  if roleAttr == nil then
    return 0
  end
  return roleAttr.value
end
--计算装备提升的属性（只是提升的属性，不包含英雄自身属性）
function t.CalcRoleAttrByEquipAttr(roleAttrList,equipAttrList)
  local role_attr = require('ui/role/model/role_attr')
  
  local addAttrList = {}
  if roleAttrList == nil or equipAttrList == nil then
    return addAttrList
  end
  
  local value = 0
  local add = 0
  for k,v in pairs(equipAttrList) do
    if k == EquipmentAttributeType.Def then
      
      addAttrList[RoleAttributeType.Def] = role_attr.New(v.type,v.value)
    elseif k == EquipmentAttributeType.HPPercent then
      
      add = GetRoleAttrValue(addAttrList[RoleAttributeType.HP])
      value =( GetRoleAttrValue(roleAttrList[RoleAttributeType.HP]) + add)* v.value + add
      addAttrList[RoleAttributeType.HP] = role_attr.New(RoleAttributeType.HP,value)
      
    elseif k == EquipmentAttributeType.NormalAtkPercent then
      
      add = GetRoleAttrValue(addAttrList[RoleAttributeType.NormalAtk])
      value =( GetRoleAttrValue(roleAttrList[RoleAttributeType.NormalAtk]) + add)* v.value + add
      addAttrList[RoleAttributeType.NormalAtk] = role_attr.New(RoleAttributeType.NormalAtk,value)
      
    elseif k == EquipmentAttributeType.MagicAtkPercent then
      
      add = GetRoleAttrValue(addAttrList[RoleAttributeType.MagicAtk])
      value =( GetRoleAttrValue(roleAttrList[RoleAttributeType.MagicAtk]) + add)* v.value + add
      addAttrList[RoleAttributeType.MagicAtk] = role_attr.New(RoleAttributeType.MagicAtk,value)
      
    elseif k == EquipmentAttributeType.DefPercent then
      
      add = GetRoleAttrValue(addAttrList[RoleAttributeType.Def])
      value =( GetRoleAttrValue(roleAttrList[RoleAttributeType.Def]) + add)* v.value + add
      addAttrList[RoleAttributeType.Def] = role_attr.New(RoleAttributeType.Def,value)
    else
      addAttrList[k] = role_attr.New(v.type,v.value)
    end
  end
  return addAttrList
end

Start()
return t