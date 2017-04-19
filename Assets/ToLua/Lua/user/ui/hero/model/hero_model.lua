local name = 'hero_model'
local hero_model = {}
hero_model.heroInfos = {}
hero_model.newHeroMarks = {}

--one param: iscrit
hero_model.onHeroStrengthenSuccessDelegate = void_delegate.New()
--none param 
hero_model.onHeroAdvanceSuccessDelegate = void_delegate.New()
hero_model.onHeroBreakthroughSuccessDelegate = void_delegate.New()
hero_model.onHeroInfoListUpdateDelegate = void_delegate.New()
--新英雄提示no param 
hero_model.OnNewHeroMarksChangedDelegate = void_delegate.New()
hero_model.OnHeroRelationShipRespDelegate = void_delegate.New()
--英雄合成one param
hero_model.OnHeroComposeSuccessDelegate = void_delegate.New()

-- 英雄锁定 no param
hero_model.OnLockHeroSuccessDelegate = void_delegate.New()
-- 英雄解锁 no param
hero_model.OnUnlockHeroSuccessDelegate = void_delegate.New()
-- 英雄锁定信息变更 no param
hero_model.OnLockedHeroChangedDelegate = void_delegate.New()

----view & data
local hero_advance_data = gamemanager.GetData('hero_advance_data')
local breakthrough_data = gamemanager.GetData('breakthrough_data')
local common_error_tips_view = require('ui/tips/view/common_error_tips_view')

---------下面是红点提示相关--------------
--阵上英雄提示
hero_model.advanceHeroDic = Dictionary.New('number','boolean')
hero_model.breakthroughHeroDic = Dictionary.New('number','boolean')
hero_model.relationshipHeroDic = Dictionary.New('number','boolean')
-------------end---------------------------
local function Start ()
  gamemanager.RegisterModel(name, hero_model)
end

function hero_model.GetAllHeroInfoList ()
  return hero_model.heroInfos
end

function hero_model.GetAllHeroCount ()
  return table.count(hero_model.heroInfos)
end

--由于阵型系统尚未移植到lua，此方法暂不能实现
function hero_model.GetAllInBagHeroInfoList ()
  -- need to impelment later ^_^
end

--获得不在阵型中（pve、pvp、远征）的英雄列表
function hero_model.GetNotInAnyTeamHeroInfoList ()
  local formation_model = gamemanager.GetModel('formation_model')
  local infoList = {}
  local index = 1
  for k,v in pairs(hero_model.heroInfos) do
    if not formation_model.IsHeroInAnyTeam(v.instanceID) and not hero_model.IsHeroInRelations(v.instanceID) and not hero_model.IsHeroInExploring(v.instanceID) then
      infoList[index] = v
      index = index +1
    end
  end
  return infoList
end

--获取不在羁绊中的英雄列表
function hero_model.GetHeroListExpectRelationship()
  local relationHeroInfoIdList = hero_model.GetRelationshipHeroIds()
  local heroList = {}
  for k, v in pairs(hero_model.heroInfos) do
    if not relationHeroInfoIdList[k] then
      heroList[k] = v
      
    end
  end
  return heroList
end
--获取处于羁绊中的英雄表
function hero_model.GetRelationshipHeroIds()
  local relationHeroInfoIds = {}
  for k, v in pairs(hero_model.heroInfos) do
    if table.count(v.relations) > 0 then
      for k1,v1 in pairs(v.relations) do
        for k2,id in ipairs(v1.friends) do
          relationHeroInfoIds[id] = id
          --print('-------------羁绊id',id)
        end
      end
    end
    
  end
  return relationHeroInfoIds
end
function hero_model.IsHeroInRelations(instanceId)
  for k, v in pairs(hero_model.heroInfos) do
      if v.instanceID == instanceId and table.count(v.relations) > 0 then
        return true
      end
    for k1,v1 in pairs(v.relations) do
      for k2,v2 in ipairs(v1.friends) do
        if v2 == instanceId then
          return true
        end
      end
    end
  end
  return false
end

function hero_model.IsHeroInExploring(instanceId)
  local heros = gamemanager.GetModel('exploremodel').GetUnfinishExploringHeroes ()
  if not heros then
    return false
  end
  for k,v in pairs(heros) do
    if v == instanceId then
      return true
    end
  end
  return false
end

function hero_model.GetHeroInfo (heroInstanceID)
  return hero_model.heroInfos[heroInstanceID]
end

function hero_model.GetHeroInfoListByHeroDataID (heroDataID)
  local tempList = {}
  local index = 1
  local heroInfos = hero_model.GetNotInAnyTeamHeroInfoList()
  for k, v in pairs(heroInfos) do
    if (v.heroData.id == heroDataID) then
      tempList[index] = v
      index = index + 1
    end
  end
  return tempList
end

function hero_model.GetHeroInfosLevelMoreThan (level)
  local heroInfos = {}
  for k, v in pairs(hero_model.heroInfos) do
    if (v.level >= level) then
      heroInfos[k] = v
    end
  end
  return heroInfos
end

function hero_model.GetHeroInfosStarMoreThan (star)
  local heroInfos = {}
  for k, v in pairs(hero_model.heroInfos) do
    if (v.advanceLevel >= star) then
      heroInfos[k] = v
    end
  end
  return heroInfos
end

function hero_model.HeroRelationShipResp()
  hero_model.OnHeroRelationShipRespDelegate:Invoke()
end

function hero_model.OnUpdateHeroInfoList ()
  hero_model.onHeroInfoListUpdateDelegate:Invoke()
  Observers.Facade.Instance.SendNotification(Observers.Facade.Instance,'OnUpdateHeroInfoList')
end

function hero_model.OnUpdateHero (heroInstanceID)
    Observers.Facade.Instance.SendNotification(Observers.Facade.Instance,'OnUpdateHero', heroInstanceID)
end

function hero_model.UpdateHeroes (heroInfos)
  for k, v in pairs(heroInfos) do
    OnUpdateHero(v.instanceID)
  end
end

function hero_model.AddHero (heroInfo, isNew)
  hero_model.heroInfos[heroInfo.instanceID] = heroInfo
  if isNew == true then
    hero_model.newHeroMarks[heroInfo.instanceID] = heroInfo
  end
end

function hero_model.RemoveHero (heroInstanceID)
  hero_model.heroInfos[heroInstanceID] = nil
end

function hero_model.RemoveHeroes (heroInstanceIDs)
  for k, v in ipairs(heroInstanceIDs) do
    hero_model.RemoveHero (v)
  end
end

function hero_model.HasNewHero ()
  if not gamemanager.GetModel('function_open_model').IsFunctionOpen(FunctionOpenType.MainView_Hero,false) then
    return false
  end
  return table.count(hero_model.newHeroMarks) > 0
end

function hero_model.IsNewHero (heroInstanceID)
  return hero_model.newHeroMarks[heroInstanceID] ~= nil
end

function hero_model.SetHeroAsChecked (heroInstanceID)
  local d = hero_model.newHeroMarks[heroInstanceID]
  hero_model.newHeroMarks[heroInstanceID] = nil
  if d then
    hero_model.OnNewHeroMarksChanged()
  end
end

function hero_model.ClearNewHeroMarks ()
  hero_model.newHeroMarks = {}
  hero_model.OnNewHeroMarksChanged()
end

function hero_model.OnNewHeroMarksChanged ()
  hero_model.OnNewHeroMarksChangedDelegate:Invoke()
  Observers.Facade.Instance.SendNotification(Observers.Facade.Instance,'OnNewHeroMarksChanged')
end

function  hero_model.PutOnEquipment (roleInfo, equipmentInstanceID)
  local equipmentInfo = gamemanager.GetModel('equip_model').GetEquipmentInfoByInstanceID(equipmentInstanceID)
  if equipmentInfo.data.equipmentType == EquipmentType.PhysicalWeapon or equipmentInfo.data.equipmentType == EquipmentType.MagicalWeapon then
    local oldEquipmentInfo = gamemanager.GetModel('equip_model').GetEquipmentInfoByInstanceID(roleInfo.weaponID)
    if oldEquipmentInfo ~= nil then
      oldEquipmentInfo:SetOwnerId(0)
    end
    roleInfo.weaponID = equipmentInstanceID
  elseif equipmentInfo.data.equipmentType == EquipmentType.Armor then
    local oldEquipmentInfo = gamemanager.GetModel('equip_model').GetEquipmentInfoByInstanceID(roleInfo.armorID)
    if oldEquipmentInfo ~= nil then
      oldEquipmentInfo:SetOwnerId(0)
    end
    roleInfo.armorID = equipmentInstanceID
  elseif equipmentInfo.data.equipmentType == EquipmentType.Accessory then
    local oldEquipmentInfo = gamemanager.GetModel('equip_model').GetEquipmentInfoByInstanceID(roleInfo.accessoryID)
    if oldEquipmentInfo ~= nil then
      oldEquipmentInfo:SetOwnerId(0)
    end
     roleInfo.accessoryID = equipmentInstanceID
  end
  
  gamemanager.GetModel('equip_model').SetEquipOwnerID(equipmentInfo.id, roleInfo.instanceID)
end

function hero_model.PutOffEquipment (roleInfo, equipmentInstanceID)
  local equipmentInfo = gamemanager.GetModel('equip_model').GetEquipmentInfoByInstanceID(equipmentInstanceID)
  
  if (equipmentInfo ~= nil) then
      if equipmentInfo.data.equipmentType == EquipmentType.PhysicalWeapon or equipmentInfo.data.equipmentType == EquipmentType.MagicalWeapon then
        roleInfo.weaponID = 0
      elseif equipmentInfo.data.equipmentType == EquipmentType.Armor then
        roleInfo.armorID = 0
      elseif equipmentInfo.data.equipmentType == EquipmentType.Accessory then
        roleInfo.accessoryID = 0
      end
      gamemanager.GetModel('equip_model').SetEquipOwnerID(equipmentInfo.id, 0)
    end
end

function hero_model.UpdateHeroEquipments (isPlayer , heroInstanceID, equipmentIDs)
  local roleInfo
  if isPlayer then
    roleInfo = gamemanager.GetModel('player_model').GetPlayerInfo(heroInstanceID)
  else
    roleInfo = hero_model.GetHeroInfo(heroInstanceID)
  end
  local weaponID = 0
  local armorID = 0
  local accessoryID = 0
  
  if equipmentIDs == nil or #equipmentIDs <= 0 then
    return
  end
  
  local hasNegativeOne = false
  
  for k, v in ipairs(equipmentIDs) do
    if v == -1 then
      hasNegativeOne = true
      break
    elseif v > 0 then
      
      local equipmentInfo = gamemanager.GetModel('equip_model').GetEquipmentInfoByInstanceID(v)
      
      if equipmentInfo.data.equipmentType == EquipmentType.PhysicalWeapon or equipmentInfo.data.equipmentType == EquipmentType.MagicalWeapon then
        weaponID = v
      elseif equipmentInfo.data.equipmentType == EquipmentType.Armor then
        armorID = v
      elseif equipmentInfo.data.equipmentType == EquipmentType.Accessory then
        accessoryID = v
      end
    end
  end
  if hasNegativeOne == true then
    hero_model.PutOffEquipment(roleInfo, roleInfo.weaponID)
    hero_model.PutOffEquipment(roleInfo, roleInfo.armorID)
    hero_model.PutOffEquipment(roleInfo, roleInfo.accessoryID)
    return
  end
  
  if weaponID > 0 then
    hero_model.PutOnEquipment(roleInfo, weaponID)
  else
    hero_model.PutOffEquipment(roleInfo, roleInfo.weaponID)
  end
  if armorID > 0 then
    hero_model.PutOnEquipment(roleInfo, armorID)
  else
    hero_model.PutOffEquipment(roleInfo, roleInfo.armorID)
  end
  if accessoryID > 0 then
    hero_model.PutOnEquipment(roleInfo, accessoryID)
  else
    hero_model.PutOffEquipment(roleInfo, roleInfo.accessoryID)
  end
end
--等级降序
function hero_model.CompareHeroByLevelDesc(aRoleInfo,bRoleInfo)
  local a = aRoleInfo.level
  local b = bRoleInfo.level
  if a == b then
    return hero_model.CompareHeroByQualityDesc(aRoleInfo,bRoleInfo)
  end
  return a > b
end
--强化等级降序
function hero_model.CompareHeroByStrengthenLevelDesc(aRoleInfo,bRoleInfo)
  local a = aRoleInfo.strengthenLevel
  local b = bRoleInfo.strengthenLevel
  if a == b then
    return hero_model.CompareHeroByQualityDesc(aRoleInfo,bRoleInfo)
  end
  return a > b
end
--星级降序
function hero_model.CompareHeroByAdvanceDesc(aRoleInfo,bRoleInfo)
  return hero_model.CompareHeroByQualityDesc(aRoleInfo,bRoleInfo)
end

--品质降序, 并保证上阵英雄在前
function hero_model.CompareHeroByQualityDescConsiderInFormationHero (aRoleInfo, bRoleInfo)

  local a = 0
  local b = 0
  
  if gamemanager.GetModel('formation_model').IsHeroInAnyPVETeam(aRoleInfo.instanceID) then
    a = 1
  end
  if gamemanager.GetModel('formation_model').IsHeroInAnyPVETeam(bRoleInfo.instanceID) then
    b = 1
  end
  
  if a == b then
    a = aRoleInfo.heroData.quality
    b = bRoleInfo.heroData.quality
  end
  
  if a == b then
    a = aRoleInfo:Power()
    b = bRoleInfo:Power()
  end
  
  if a == b then
    a = aRoleInfo.heroData.id
    b = bRoleInfo.heroData.id
  end
  
  if a == b then
    a = aRoleInfo.instanceID
    b = bRoleInfo.instanceID
  end
  
  return a > b
end

--品质降序
function hero_model.CompareHeroByQualityDesc(aRoleInfo,bRoleInfo)
  local a = aRoleInfo.heroData.quality
  local b = bRoleInfo.heroData.quality
  
  if a == b then
    a = aRoleInfo:Power()
    b = bRoleInfo:Power()
  end
  
  if a == b then
    a = aRoleInfo.heroData.id
    b = bRoleInfo.heroData.id
  end
  
  if a == b then
    a = aRoleInfo.instanceID
    b = bRoleInfo.instanceID
  end
  
  return a > b
end
--品质升序
function hero_model.CompareHeroByQualityAsc(aRoleInfo,bRoleInfo)
  local a = aRoleInfo.heroData.quality
  local b = bRoleInfo.heroData.quality
  
  if a == b then
    a = aRoleInfo:Power()
    b = bRoleInfo:Power()
  end
  
  if a == b then
    a = aRoleInfo.heroData.id
    b = bRoleInfo.heroData.id
  end
  
  if a == b then
    a = aRoleInfo.instanceID
    b = bRoleInfo.instanceID
  end
  
  return b > a
end

function hero_model.CompareHeroByRoleTypeAndQualityDesc (aRoleInfo, bRoleInfo)
  local a = aRoleInfo.heroData.roleType
  local b = bRoleInfo.heroData.roleType
  if a ~= b then
    return a > b
  end
  return hero_model.CompareHeroByQualityDesc(aRoleInfo,bRoleInfo)
end

function hero_model.CompareHeroAsMaterialByQualityAsc (aRoleInfo, bRoleInfo)
  local a = 0
  local b = 0
  if aRoleInfo:CanBeUsedAsMaterial() then
    a = 1
  end
  if bRoleInfo:CanBeUsedAsMaterial() then
    b = 1
  end

  if a == b then
    return hero_model.CompareHeroByQualityAsc(aRoleInfo,bRoleInfo)
  end
  return a > b
end

function hero_model.CompareHeroAsMaterialByQualityDesc (aRoleInfo, bRoleInfo)
  local a = 0
  local b = 0
  if aRoleInfo:CanBeUsedAsMaterial() then
    a = 1
  end
  if bRoleInfo:CanBeUsedAsMaterial() then
    b = 1
  end

  if a == b then
    return hero_model.CompareHeroByQualityDesc(aRoleInfo,bRoleInfo)
  end
  return a > b
end

---获得等级大于参数的英雄个数
function hero_model.GetHeroInfosLevelMoreThanCount(level)
  local count = 0
  for k,v in pairs(hero_model.heroInfos) do
    if v.level >= level then
      count = count + 1
    end
  end
  return count
end
--获得星级大于参数的英雄个数
function hero_model.GetHeroInfosStarMoreThanCount(star)
  local count = 0
  for k,v in pairs(hero_model.heroInfos) do
    if v.advanceLevel >= star then
      count = count + 1
    end
  end
  return count
end
--------判断英雄是否可以升星--------------
function hero_model.IsHeroCanAdvance(heroInfo,showTips)
  if heroInfo.advanceLevel == heroInfo:MaxAdvanceLevel() then
    return false
  end
  
  local advanceData = hero_advance_data.GetAdvanceDataByHeroInfo(heroInfo)
  local advanceMaterialGameResData = advanceData.materialGameResDataList

  local item_model = gamemanager.GetModel('item_model')
  for k,v in pairs(advanceMaterialGameResData) do
    local own = item_model.GetItemCountByItemID(v.id)
    if own < v.count then
      if showTips then
        common_error_tips_view.Open(LocalizationController.instance:Get("ui.hero_advance_view.material_not_enough"))
      end
      --Debugger.LogError('进阶材料不足id:'..heroInfo.instanceID)
      return false
    end
  end
  --money
  local ownMoney = gamemanager.GetModel('game_model').GetBaseResourceValue(BaseResType.Gold)
  if ownMoney < advanceData.gold then
    if showTips then
      common_error_tips_view.Open(LocalizationController.instance:Get("ui.hero_advance_view.gold_not enough"))
    end
    --Debugger.LogError('进阶金币不足id:'..heroInfo.instanceID)
    return false
  end
  return true
end
--------------判断英雄是否可以突破---------------------
function hero_model.IsHeroCanBreakthrough(roleInfo,isPlayer,showTips,ignoreMaterialCheck)
  --材料
  local item_model = gamemanager.GetModel('item_model')
  local nextBreakthroughData = breakthrough_data.GetNextBreakthroughDataByRoleInfo(roleInfo)
  if nextBreakthroughData == nil then
    --print('role id'..roleInfo.instanceID..'[Warning]:[突破等级已达上限]')
    return false
  end
  
  if not ignoreMaterialCheck then
    local breakthroughCostItemGameResData = nextBreakthroughData.costItemGameResData
    local breakthroughCostItemInfo = item_model.GetItemInfoWithoutNilByItemId(breakthroughCostItemGameResData.id)
    
    if breakthroughCostItemInfo.count < breakthroughCostItemGameResData.count then
      --print('role id'..roleInfo.instanceID..'[Warning]:[突破所需道具数量不足]')
      return false
    end
  end
  
  --突破等级
  if roleInfo.level < nextBreakthroughData.levelMin then
    if showTips then
      local str = string.format(LocalizationController.instance:Get("ui.hero_breakthrough_view.tips.not_enough_hero_level"), nextBreakthroughData.levelMin)
      common_error_tips_view.Open(str)
    end
    --print('role id'..roleInfo.instanceID..'[Warning]:[英雄等级不足，不可突破]')
    return false
  end
  
  --钱
  local myGold = gamemanager.GetModel('game_model').GetBaseResourceValue(BaseResType.Gold)
  local costGold = nextBreakthroughData.costGoldGameResData.count
  if myGold < costGold then
    if showTips then
      common_error_tips_view.Open(LocalizationController.instance:Get("ui.hero_breakthrough_view.tips.not_enough_gold"))
    end
    --print('role id'..roleInfo.instanceID..'[Warning]:[金币不足，不可突破]')
    return false
  end
  
  return true
end
---------------------end
---------下面是红点提示相关---------------------------
--进阶突破提示
function hero_model.CheckHasAdvanceBreakthroughHeroByRedPoint()
  coroutine.stop(hero_model.checkRedPointCoroutine)
  hero_model.checkRedPointCoroutine = coroutine.start(hero_model.CheckHasAdvanceBreakthroughHeroByRedPointDelay)
end
function hero_model.CheckHasAdvanceBreakthroughHeroByRedPointDelay()
  coroutine.wait(1)
  local game_model = gamemanager.GetModel('game_model')
 -- Debugger.LogError('CheckHasAdvanceBreakthroughHeroByRedPointDelay')
  local openAdvance = gamemanager.GetModel('function_open_model').IsFunctionOpen(FunctionOpenType.HeroAdvance,false)
  --local openBreakthrough = gamemanager.GetModel('function_open_model').IsFunctionOpen(FunctionOpenType.HeroBreakthrough,false)
  
  if not openAdvance then
    return
  end
  if openAdvance then
    hero_model.advanceHeroDic:Clear()
  end
  hero_model.breakthroughHeroDic:Clear()
  local infoList = gamemanager.GetModel('formation_model').GetAnyPveTeamHeroIdList()
  local info = nil
  for k,v in pairs(infoList) do
    if game_model.IsPlayer(v) then
       if openAdvance and hero_model.IsHeroCanAdvance(game_model.playerInfo,false) then
          hero_model.advanceHeroDic:Add(k,true)
         -- Debugger.LogError('can advance id:'..k..',level:'..info.advanceLevel)
        end
        --if openBreakthrough and hero_model.IsHeroCanBreakthrough(game_model.playerInfo,false,false,false) then
         -- hero_model.breakthroughHeroDic:Add(k,true)
          --Debugger.LogError('can breakthrough id:'..k..',level:'..info.level)
        --end
    else
      info = hero_model.GetHeroInfo(k)
      if info then
        if openAdvance and hero_model.IsHeroCanAdvance(info,false) then
          hero_model.advanceHeroDic:Add(k,true)
         -- Debugger.LogError('can advance id:'..k..',level:'..info.advanceLevel)
        end
        --if openBreakthrough and hero_model.IsHeroCanBreakthrough(info,false,false,false) then
          --hero_model.breakthroughHeroDic:Add(k,true)
          --Debugger.LogError('can breakthrough id:'..k..',level:'..info.level)
        --end
      end
    end
    
  end
  gamemanager.GetModel('red_point_model').RefreshSpecific(RedPointType.RedPoint_HeroAdvance)
  --gamemanager.GetModel('red_point_model').RefreshSpecific(RedPointType.RedPoint_HeroBreakthrough)
  gamemanager.GetModel('red_point_model').RefreshSpecific(RedPointType.RedPoint_All_Hero_advance_breakthrough)

end
--阵上英雄可羁绊提示
function hero_model.CheckHasRelationshipByRedPoint()
  coroutine.stop(hero_model.checkRelationshipRedPointCoroutine)
  hero_model.checkRelationshipRedPointCoroutine = coroutine.start(hero_model.CheckHasRelationshipByRedPointDelay)
end
function hero_model.CheckHasRelationshipByRedPointDelay()
  coroutine.wait(1)
  --print('----------------------开始检测阵上英雄羁绊提示------------------------')
  hero_model.relationshipHeroDic:Clear()
  local hero_relationship_data = gamemanager.GetData('hero_relationship_data')
  local infoList = gamemanager.GetModel('formation_model').GetAnyPveTeamHeroIdList()
  local info = nil
  local relationList = nil 
  local friendIdStarList = nil
  local freeInfoList = hero_model.GetHeroListExpectRelationship()
  local hasRelation = false
  for k,v in pairs(infoList) do --检测pve阵型上羁绊提示
    info = hero_model.GetHeroInfo(k)
    if info ~= nil then
      relationList = hero_relationship_data.GetDataBydataid(info.heroData.id)
      if relationList ~= nil then--有羁绊列表
        for k1,v1 in pairs(relationList) do
          hasRelation = true
          if info:IsActiveRelation(v1.id) then --已经激活啦 亲！！！
            hasRelation = false
          else
            for k2,idAndStar in pairs(v1.friend_id) do
              local hasFound = hero_model.HasHeroInfoByDataIdAndStar(freeInfoList,idAndStar[1],idAndStar[2])
              --print('英雄modelId:',info.heroData.id,'羁绊index:',k1,'羁绊id：',idAndStar[1],'羁绊star:',idAndStar[2],'hasFound:',hasFound)
              if not hasFound then ---只要有一个不符合要求的英雄  砍shi ta
                hasRelation = false
                break
              end
            end
          end
          
          if hasRelation then--吐血a 查找到了
            hero_model.relationshipHeroDic:Add(info.instanceID,true)
           -- print('可羁绊的英雄id',info.instanceID)
            
            break
          end
        end
      end
      
    end
    
  end
  gamemanager.GetModel('red_point_model').RefreshSpecific(RedPointType.RedPoint_hero_relationship)
  gamemanager.GetModel('red_point_model').RefreshSpecific(RedPointType.RedPoint_all_hero_relationship)
end
function hero_model.HasHeroInfoByDataIdAndStar(infoList,dataId,star)
  for k,v in pairs(infoList) do
    if v.heroData.id == dataId and v.advanceLevel == star then
      return true
    end
  end
  return false
end
----------------------------------------end---------------------------------

--------------------update by protocol---------------

function hero_model.OnLockHeroSuccess ()
  hero_model.OnLockHeroSuccessDelegate:Invoke()
end

function hero_model.OnUnlockHeroSuccess ()
  hero_model.OnUnlockHeroSuccessDelegate:Invoke()
end

function hero_model.OnLockedHeroChanged ()
  hero_model.OnLockedHeroChangedDelegate:Invoke()
end

function hero_model.updateLockedHeroes (lockedHeroIDs)
  for k, v in pairs(hero_model.heroInfos) do
    v.isLocked = false
  end
  for k, v in ipairs(lockedHeroIDs)  do
    hero_model.GetHeroInfo(v).isLocked = true
  end
end

function hero_model.UpdateHeroStrengthenSuccessByProtocol(isCrit)
  hero_model.onHeroStrengthenSuccessDelegate:InvokeOneParam(isCrit)
  --Observers.Facade.Instance.SendNotification(Observers.Facade.Instance,'UpdateStrengthenSuccess', isCrit)
end
function hero_model.UpdateHeroAdvanceSuccessByProtocol(roleInstanceID)
  hero_model.onHeroAdvanceSuccessDelegate:InvokeOneParam(roleInstanceID)
  --Observers.Facade.Instance.SendNotification(Observers.Facade.Instance,'UpdateHeroAdvanceByProtocol')
end
function hero_model.UpdateHeroBreakthroughSuccessByProtocol(roleInstanceID)
  hero_model.onHeroBreakthroughSuccessDelegate:InvokeOneParam(roleInstanceID)
end
function hero_model.UpdateHeroComposeSuccessByProtocol(newHeroId)
  hero_model.OnHeroComposeSuccessDelegate:InvokeOneParam(newHeroId)
end

Start()
return hero_model