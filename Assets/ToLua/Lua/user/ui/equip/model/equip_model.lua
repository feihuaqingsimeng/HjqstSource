local equip_model = {}
equip_model.equipInfos = {}
equip_model.newEquipMarks = {}
equip_model.isRecast = false
equip_model.equipRecastAttrList = {}

local name = 'equip_model'

--培养协议 界面更新  no param
equip_model.UpdateTrainingDelegate = void_delegate.New()
equip_model.OnEquipmentInfoUpdateDelegate = void_delegate.New()
equip_model.OnEquipmentInfoListUpdateDelegate = void_delegate.New()
equip_model.OnNewEquipmentMarksChangedDelegate = void_delegate.New()
equip_model.OnEquipmentStrengthenSuccessDelegate = void_delegate.New()
equip_model.OnEnchantingScrollComposeSuccessDelegate = void_delegate.New()
equip_model.OnEquipEnchantSuccessDelegate = void_delegate.New()
equip_model.OnEquipGemInsertSuccessDelegate = void_delegate.New()
equip_model.OnStarGemComposeSuccessDelegate  = void_delegate.New()
equip_model.OnEquipInheritSuccessDelegate  = void_delegate.New()

local function Start ()
  print('equip_model start')
  gamemanager.RegisterModel(name, equip_model)
end

function equip_model.GetAllEquipmentInfoDictionary ()
  return equip_model.equipInfos
end

function equip_model.GetAllEquipmentInfoList ()
  local equipInfoList = {}
  for k, v in pairs(equip_model.equipInfos) do
    table.insert(equipInfoList, v)
  end
  return equipInfoList
end

function equip_model.GetAllEquipmentCount ()
  return table.count(equip_model.equipInfos)
end

function equip_model.GetFreeEquipmentInfoList ()
  local freeEquipmentInfos = {}
  local index = 1
  for k, v in pairs(equip_model.equipInfos) do
    if v.ownerId == 0 then
      freeEquipmentInfos[index] = v
      index = index + 1
    end
  end
  return freeEquipmentInfos
end

function equip_model.GetFreeEquipmentCount ()
  return table.count(equip_model.GetFreeEquipmentInfoList())
end

function equip_model.GetFreeEquipmentInfoListOfRoleType (roleType)
  local freeEquipmentInfos = {}
  local index = 1
  for k, v in pairs(equip_model.equipInfos) do
    if v.ownerId == 0 and v.data.equipmentRoleType == roleType then
      freeEquipmentInfos[index] = v
      index = index + 1
    end
  end
  return freeEquipmentInfos
end

function equip_model.GetEquipmentInfoByInstanceID (id)
  return equip_model.equipInfos[id]
end
function equip_model.GetEquipmentInfoListByEquipId(equipId)
  local tempList = {}
  local index = 1
  local equipList = equip_model.GetFreeEquipmentInfoList()
  for k,v in pairs(equipList) do
    if v.data.id == equipId then
      tempList[index] = v
      index = index + 1
    end
  end
  return tempList
end

-- [[ 判断英雄是否有可穿戴的装备 ]] --
-- 判断英雄是否有可穿戴的武器
function equip_model.HasFitEquipment (roleInfo, equipmentType)
  if roleInfo == nil then
    return false
  end
  
  local freeEquipmentInfoList = equip_model.GetFreeEquipmentInfoList ()
  for k, v in ipairs(freeEquipmentInfoList) do
    if equipmentType ~= nil then
      if v.data.equipmentRoleType == roleInfo.heroData.roleType 
      and v.data.equipmentType == equipmentType 
      and roleInfo.level >= v.data.useLv then
        if v.data.equipmentType == 1 and roleInfo.weaponID <= 0 then
          return true
        end
        if v.data.equipmentType == 2 and roleInfo.armorID <= 0  then
          return true
        end
        if v.data.equipmentType == 3 and roleInfo.accessoryID <= 0  then
          return true
        end
        return false
      end
    else
      if v.data.equipmentRoleType == roleInfo.heroData.roleType 
      and roleInfo.level >= v.data.useLv then
        if v.data.equipmentType == 1 and roleInfo.weaponID <= 0 then
          return true
        end
        if v.data.equipmentType == 2 and roleInfo.armorID <= 0  then
          return true
        end
        if v.data.equipmentType == 3 and roleInfo.accessoryID <= 0  then
          return true
        end
        return false
      end
    end
  end
  return false
end
-- [[ 判断英雄是否有可穿戴的装备 ]] --

function equip_model.AddEquipmentInfo (equipInfo, isNew)
  --print('add equip in lua:'..equipInfo.id,equipInfo.data.id)
  equip_model.equipInfos[equipInfo.id] = equipInfo
  if isNew == true then
    equip_model.newEquipMarks[equipInfo.id] = equipInfo
  end
end

function equip_model.RemoveEquipmentInfo (id)
  equip_model.equipInfos[id] = nil
end

function equip_model.UpdateEquipmentInfo (equip)
  local equipInfo = equip_model.equipInfos[equip.id]
  equipInfo:Update(equip)
  equip_model.OnEquipmentInfoUpdate(equip.id)
  
  if equipInfo.ownerId > 0 then
    equip_model.SetEquipmentAsChecked(equip.id)
  end
end

function equip_model.UpdateEquipmentInfoList (addEquips, delEquips, updateEquips)
  local equip_info = require('ui/equip/model/equip_info')
  local hasAdd = false
  for k, v in ipairs(addEquips) do
    local equipInfo = equip_info:NewByEquip(v)
    equip_model.AddEquipmentInfo(equipInfo, true)
    hasAdd = true
  end
  local tempInfo = nil
  for k, v in ipairs(delEquips) do
    tempInfo = equip_model.GetEquipmentInfoByInstanceID(v)
    if tempInfo == nil then
      --Debugger.LogError('[UpdateEquipmentInfoList]can not find equipInfo to remove instanceId:'..v)
    else
      TalkingDataController.instance:TDGAItemOnUse(tempInfo.data.id,1,BaseResType.Equipment)
    end
    equip_model.RemoveEquipmentInfo(v)
    equip_model.SetEquipmentAsChecked(v)
    
  end
  
  for k, v in ipairs(updateEquips) do
    equip_model.UpdateEquipmentInfo(v)
  end
  
  equip_model.OnEquipmentInfoListUpdate ()
  if hasAdd then
    gamemanager.GetModel('red_point_model').RefreshSpecific(RedPointType.RedPoint_New_Equip)
  end
end


--获得装备强化花费的金币
function equip_model.GetStrengthenMoney(strengthenType,curLevel,addLevel)
  local money = 0
  local equip_strengthen_data = gamemanager.GetData('equip_strengthen_data')
  for i = 1,addLevel do
    money = money + equip_strengthen_data.GetDataByStrengthenTypeAndLevel(strengthenType,curLevel+i-1 ).cost
  end
  return money
end


function equip_model.HasNewEquipment ()
  return table.count(equip_model.newEquipMarks) > 0
end

function equip_model.IsNewEquipment (instanceID)
  return equip_model.newEquipMarks[instanceID] ~= nil
end

function equip_model.SetEquipmentAsChecked (instanceID)
  equip_model.newEquipMarks[instanceID] = nil
  equip_model.OnNewEquipmentMarksChanged()
end

function equip_model.ClearNewEquipmentMarks ()
  equip_model.newEquipMarks = {}
  equip_model.OnNewEquipmentMarksChanged()
end

function equip_model.SetEquipOwnerID (equipID, ownerID)
  local equipInfo = equip_model.GetEquipmentInfoByInstanceID(equipID)
  equipInfo.ownerId = ownerID
  if equipInfo.ownerId > 0 then
    equip_model.SetEquipmentAsChecked(equipInfo.id)
  end
end

--重铸属性暂时保留
function equip_model.SetEquipRecastAttrList(randomAttrProto)
  
  equip_model.equipRecastAttrList = {}
  if randomAttrProto ~= nil then 
    local equip_attr = require('ui/equip/model/equip_attr')
    for k,v in ipairs(randomAttrProto) do
      equip_model.equipRecastAttrList[k] = equip_attr.NewByEquipAttr(v)
      print('recast attr',v.type,v.value,'|',equip_model.equipRecastAttrList[k].type,equip_model.equipRecastAttrList[k].value)
    end
  end
  
end

-- 计算战力
function equip_model.CalcEquipPower(equipInfo)
  

  if equipInfo == nil then
    return 0 
  end
  local equip_attr_data = gamemanager.GetData('equip_attr_data')
  local equip_strengthen_data = gamemanager.GetData('equip_strengthen_data')
  local power = 0
  local basePower = 0
  --基础属性战力
  local attrData = equip_attr_data.GetDataByAttrId(equipInfo.data:GetFirstBaseIdAttr(),equipInfo.baseAttr.type)
  basePower = math.floor(attrData.attr_comat_const)
  --强化战力
  basePower = basePower + math.floor(equipInfo:GetStrengthenAttrValue()/ attrData.value_max * attrData.attr_comat_const)
  power = power + basePower
  --随机属性战力
  for k,v in ipairs(equipInfo.randomAttrs) do
    local attrData = equip_attr_data.GetDataByAttrId(equipInfo.data.randomAttrIdList[k],v.type)
    if attrData ~= nil then
      power = power + math.floor( v.value/attrData.value_max*attrData.attr_comat_const)
    end
  end
  --宝石战力
  local gem_attr_data = gamemanager.GetData('gem_attr_data')
  for k,v in ipairs(equipInfo.gemInsertIds) do
    if v > 0 then
      local attrData = gem_attr_data.GetDataById(v)
      if attrData ~= nil then 
        power = power + attrData.comat
      end
    end
  end
  --升星战力
  local equipStarFactor = 0
  for k,v in ipairs(equipInfo.starGems) do
    equipStarFactor = equipStarFactor + v/100
  end
    power = power + math.floor(basePower) * equipStarFactor
  return math.floor(power)
end
--品质升序
function equip_model.CompareEquipmentByQualityAsc(aEquipInfo,bEquipInfo)
  local a = aEquipInfo.data.quality
  local b = bEquipInfo.data.quality
  if a == b then
    a = aEquipInfo.strengthenLevel
    b = bEquipInfo.strengthenLevel
  end
  if a == b then
    a = aEquipInfo.star
    b = bEquipInfo.star
  end
  if a == b then
    a = aEquipInfo.id
    b = bEquipInfo.id
  end
  return a < b
end
--品质降序
function equip_model.CompareEquipmentByQualityDsc(aEquipInfo,bEquipInfo)
  local a = aEquipInfo.data.quality
  local b = bEquipInfo.data.quality
  if a == b then
    a = aEquipInfo.strengthenLevel
    b = bEquipInfo.strengthenLevel
  end
  if a == b then
    a = aEquipInfo.star
    b = bEquipInfo.star
  end
  if a == b then
    a = aEquipInfo.id
    b = bEquipInfo.id
  end
  return b < a
end
---------------------update by protocol------------------------------
function equip_model.OnEquipmentInfoUpdate (equipmentInstanceID)
  Observers.Facade.Instance.SendNotification(Observers.Facade.Instance,'OnEquipmentInfoUpdate', equipmentInstanceID)
  
  equip_model.OnEquipmentInfoUpdateDelegate:InvokeOneParam(equipmentInstanceID)
end

function equip_model.OnEquipmentInfoListUpdate ()
  Observers.Facade.Instance.SendNotification(Observers.Facade.Instance,'OnEquipmentInfoListUpdate')
  
  equip_model.OnEquipmentInfoListUpdateDelegate:Invoke()
end

function equip_model.OnNewEquipmentMarksChanged ()
  Observers.Facade.Instance.SendNotification(Observers.Facade.Instance,'OnNewEquipmentMarksChanged')
  
  equip_model.OnNewEquipmentMarksChangedDelegate:Invoke()
  gamemanager.GetModel('red_point_model').RefreshSpecific(RedPointType.RedPoint_New_Equip)
end

function equip_model.OnEquipmentStrengthenSuccess ()
  equip_model.OnEquipmentStrengthenSuccessDelegate:Invoke()
  equip_model.UpdateTrainingByProtocol()
end

function equip_model.OnEnchantingScrollComposeSuccess ()
  equip_model.OnEnchantingScrollComposeSuccessDelegate:Invoke()
end

function equip_model.OnEquipEnchantSuccess ()
  equip_model.OnEquipEnchantSuccessDelegate:Invoke()
end

function equip_model.UpdateTrainingByProtocol()
  equip_model.UpdateTrainingDelegate:Invoke()
end
function equip_model.UpdateStarGemComposeSuccessByProtocol()
  equip_model.OnStarGemComposeSuccessDelegate:Invoke()
end
function equip_model.OnEquipGemInsertSuccessByProtocol(slot)
  equip_model.OnEquipGemInsertSuccessDelegate:InvokeOneParam(slot)
end
--------------------------open view-----------------------

--装备界面
function equip_model.OpenEquipBrowseView()
  LuaCsTransfer.UIMgrOpen('ui/equipments/equipments_browse_view',EUISortingLayer.MainUI,UIOpenMode.Replace)
end
--分解   --equipInfo装备,itemInfo装备碎片 赋值一个就可
function equip_model.OpenEquipDecomposeView(equipInfo,itemInfo)
  dofile('ui/equip/view/equip_decompose/equip_decompose_view').Open(equipInfo,itemInfo)
end

--装备穿戴
function equip_model.OpenRoleEquipView (roleInstanceID, isPlayer, defaultRoleEquipPos)
  if not gamemanager.GetModel('function_open_model').IsFunctionOpen(FunctionOpenType.MainView_Equipment,true) then
    return
  end
  local roleInfo = nil
  if isPlayer then
    roleInfo = gamemanager.GetModel('game_model').playerInfo
  else
    roleInfo = gamemanager.GetModel('hero_model').GetHeroInfo(roleInstanceID)
  end
  
  local roleEquipView = dofile('ui/equip/view/role_equip_view')
  roleEquipView.SetInfo(roleInfo, isPlayer, defaultRoleEquipPos)
end
--装备培养(背包装备循环)
function equip_model.OpenTrainingViewByBag(selectEquipInstanceId,toggleIndex)
  if not gamemanager.GetModel('function_open_model').IsFunctionOpen(FunctionOpenType.EquipTraining,true) then
    return
  end
  local view = dofile('ui/equip/view/equip_training/equip_training_view')
  if not toggleIndex then
    toggleIndex = 1
  end
  view.OpenByEquipBag(selectEquipInstanceId,toggleIndex)
end

--装备培养(英雄装备循环)
function equip_model.OpenTrainingViewByRoleInfo(heroInstanceId,selectEquipInstanceId,toggleIndex)
  if not gamemanager.GetModel('function_open_model').IsFunctionOpen(FunctionOpenType.EquipTraining,true) then
    return
  end
  local view = dofile('ui/equip/view/equip_training/equip_training_view')
  if not toggleIndex then
    toggleIndex = 1
  end
  view.OpenByRoleInfo(heroInstanceId,selectEquipInstanceId,toggleIndex)
end

Start()
return equip_model