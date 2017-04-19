local t = {}
local PREFAB_PATH = 'ui/equipments/role_equipments_view'
local name = PREFAB_PATH

local uiUtil = require('util/ui_util')
local equipUtil =require('ui/equip/equip_util')
local commonEquipIcon = require('ui/common_icon/common_equip_icon')
local game_res_data = require('ui/game/model/game_res_data')

t.selectedEquipPos = RoleEquipPos.Weapon

local function Start ()
  uimanager.RegisterView(name,t)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  local common_top_bar = require ('ui/common_top_bar/common_top_bar')
  common_top_bar = common_top_bar:Create(t.transform:Find('core'))
  common_top_bar.transform:SetAsFirstSibling()
  common_top_bar:SetAsCommonStyle(LuaInterface.LuaCsTransfer.LocalizationGet("ui.role_equipments.role_equipments_title"),t.OnClickBackBtnHandler,false,true,true,false,false,false,false)
  
  t.roleHeadIconRoot = t.transform:Find('core/left_part/role_head_icon_root')
  
  t.weaponFrame = t.transform:Find('core/left_part/weapon_frame')
  t.armorFrame = t.transform:Find('core/left_part/armor_frame')
  t.accessory1Frame = t.transform:Find('core/left_part/accessory_1_frame')
  t.selectedRoleEquipSlotMask = t.transform:Find('core/left_part/img_selected_equipment_slot_mask')
  
  t.weaponButton = t.weaponFrame:GetComponent(typeof(Button))
  t.weaponButton.onClick:AddListener(t.OnClickWeaponButtonHandler)
  
  t.armorButton = t.armorFrame:GetComponent(typeof(Button))
  t.armorButton.onClick:AddListener(t.OnClickArmorButtonHandler)
  
  t.accessory1Button = t.accessory1Frame:GetComponent(typeof(Button))
  t.accessory1Button.onClick:AddListener(t.OnClickAccessoryButtonHandler)

  t.selectedEquipInfoFrameTransform = t.transform:Find('core/left_part/img_selected_equipment_info_frame')
  
  t.selectedEquipmentNameText = t.selectedEquipInfoFrameTransform:Find('img_title_bg/text_selected_equipment_name'):GetComponent(typeof(Text))
  
  t.tranAttrRoot = t.selectedEquipInfoFrameTransform:Find('Scroll View/Viewport/Content')
  t.goPowerRootPrefab = t.selectedEquipInfoFrameTransform:Find('Scroll View/Viewport/power_root').gameObject
  t.goAttrItemPrefab = t.selectedEquipInfoFrameTransform:Find('Scroll View/Viewport/attr_item').gameObject
  
  t.goPowerRootPrefab:SetActive(false)
  t.goAttrItemPrefab:SetActive(false)
  
  t.putOnButton = t.selectedEquipInfoFrameTransform:Find('btn_put_on'):GetComponent(typeof(Button))
  t.putOnButton.onClick:AddListener(t.OnClickPutOnButtonHandler)
  t.putOffButton = t.selectedEquipInfoFrameTransform:Find('btn_put_off'):GetComponent(typeof(Button))
  t.putOffButton.onClick:AddListener(t.OnClickPutOffButtonHandler)
  t.trainButton = t.selectedEquipInfoFrameTransform:Find('btn_train'):GetComponent(typeof(Button))
  t.trainButton.onClick:AddListener(t.OnClickTrainButtonHandler)
  
  t.weaponIcon = commonEquipIcon.New(t.weaponFrame)
  t.weaponIcon.transform:GetComponent(typeof(Button)).onClick:AddListener(t.OnClickWeaponButtonHandler)
  t.armorIcon = commonEquipIcon.New(t.armorFrame)
  t.armorIcon.transform:GetComponent(typeof(Button)).onClick:AddListener(t.OnClickArmorButtonHandler)
  t.accessory1Icon = commonEquipIcon.New(t.accessory1Frame)
  t.accessory1Icon.transform:GetComponent(typeof(Button)).onClick:AddListener(t.OnClickAccessoryButtonHandler)
  
  
  t.equipmentCellNumText = t.transform:Find('core/img_equipments_frame/img_title_bar/text_equipments_cell_num'):GetComponent(typeof(Text))
  
  t.addExpandEquipBagButton = t.transform:Find('core/img_equipments_frame/img_title_bar/btn_add_cell'):GetComponent(typeof(Button))
  t.addExpandEquipBagButton.onClick:AddListener(t.OnClickExpandEquipBagButtonHandler)
  
  t.equipTypeDropdown = t.transform:Find('core/img_equipments_frame/img_title_bar/dropdown_equipment_type'):GetComponent(typeof(Dropdown))
  t.equipTypeDropdown.captionText.text = LocalizationController.instance:Get('ui.role_equipments.all_equipments_title')
  t.equipTypeDropdown.options:Add(UnityEngine.UI.Dropdown.OptionData.New(LocalizationController.instance:Get('ui.role_equipments.all_equipments_title')))
  t.equipTypeDropdown.options:Add(UnityEngine.UI.Dropdown.OptionData.New(LocalizationController.instance:Get('ui.role_equipments.weapon_title')))
  t.equipTypeDropdown.options:Add(UnityEngine.UI.Dropdown.OptionData.New(LocalizationController.instance:Get('ui.role_equipments.armor_title')))
  t.equipTypeDropdown.options:Add(UnityEngine.UI.Dropdown.OptionData.New(LocalizationController.instance:Get('ui.role_equipments.accessory_title')))
  t.equipTypeDropdown.onValueChanged:AddListener(t.OnEquipTypeChanged)
  
  t.scrollContent = t.transform:Find('core/img_equipments_frame/scroll_rect/content'):GetComponent(typeof(Common.UI.Components.ScrollContent))
  t.scrollContent.onResetItem:AddListener(t.OnResetItemHandler)
  
  t.noWearableEquipmentGO = t.transform:Find('core/img_equipments_frame/scroll_rect/text_no_wearable_equipment').gameObject
  t.goDungeonButton = t.transform:Find('core/img_equipments_frame/scroll_rect/btn_go_dungeon'):GetComponent(typeof(Button))
  t.goDungeonButton.onClick:AddListener(t.ClickGoDungeonButton)
  
  t.SetSelectedEquipmentInfo(nil)
  t.RefreshRoleEquipmentIcons ()
  t.RefreshEquipmentCellNumText()
  t.BindDelegate()
end

function t.Close()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
  
end

function t.BindDelegate()
  gamemanager.GetModel('game_model').UpdateEquipCellNumDelegate:AddListener(t.OnEquipCellNumUpdate)
  
  local equip_model = gamemanager.GetModel('equip_model')
  equip_model.OnEquipmentInfoUpdateDelegate:AddListener(t.OnEquipmentInfoUpdateHandler)
  equip_model.OnEquipmentInfoListUpdateDelegate:AddListener(t.OnEquipmentInfoListUpdate)
  equip_model.OnNewEquipmentMarksChangedDelegate:AddListener(t.OnNewEquipmentMarksChanged) 
end

function t.UnbindDelegate()
  gamemanager.GetModel('game_model').UpdateEquipCellNumDelegate:RemoveListener(t.OnEquipCellNumUpdate)
  
  local equip_model = gamemanager.GetModel('equip_model')
  equip_model.OnEquipmentInfoUpdateDelegate:RemoveListener(t.OnEquipmentInfoUpdateHandler)
  equip_model.OnEquipmentInfoListUpdateDelegate:RemoveListener(t.OnEquipmentInfoListUpdate)
  equip_model.OnNewEquipmentMarksChangedDelegate:RemoveListener(t.OnNewEquipmentMarksChanged) 
end

function t.SetInfo (roleInfo, isPlayer, defaultRoleEquipPos)
  t.roleInfo = roleInfo
  local commonHeroIcon = require('ui/common_icon/common_hero_icon')
  local c = commonHeroIcon.CreateBig(t.roleHeadIconRoot)
  c:SetRoleInfo(t.roleInfo, isPlayer)
  c:AddRoleDesButton(false)
  --c.onClick:AddListener()
  
  t.RegenerateFreeEquipmentItems (true)
  t.RefreshRoleEquipmentIcons ()
  
  
  if defaultRoleEquipPos == RoleEquipPos.Weapon then
    t.OnClickWeaponButtonHandler ()
  elseif defaultRoleEquipPos == RoleEquipPos.Armor then
    t.OnClickArmorButtonHandler ()
  elseif defaultRoleEquipPos == RoleEquipPos.Accessory then
    t.OnClickAccessoryButtonHandler ()
  end
end

function t.SetSelectedEquipmentInfo (equipInfo)
  t.selectedEquipInfo = equipInfo
  t.RefreshSelectedEquipmentInfo ()
  t.RefreshFreeEquipmentItems ()
end

function t.RefreshSelectedEquipmentInfo ()
  ui_util.ClearChildren(t.tranAttrRoot,true)
  
  if t.selectedEquipInfo ~= nil then
    t.RefreshPower()
    t.RefreshMainAttr()
    t.RefreshRandomAttr()
    t.RefreshGemAttr()
    t.selectedEquipmentNameText.text = LocalizationController.instance:Get(t.selectedEquipInfo.data.name)
    t.selectedEquipmentNameText.color = ui_util.GetEquipQualityColor(t.selectedEquipInfo.data)
    
    if t.selectedEquipInfo.ownerId > 0 then
      t.putOnButton.gameObject:SetActive(false)
      t.putOffButton.gameObject:SetActive(true)
      
      local trainButtonPos = Vector3(96, t.trainButton.transform.localPosition.y, t.trainButton.transform.localPosition.z)
      t.trainButton.transform.localPosition = trainButtonPos
      t.trainButton.gameObject:SetActive(true)
    else
      local canWear = equipUtil.canEquip (t.selectedEquipInfo, t.roleInfo)
      
      t.putOnButton.gameObject:SetActive(canWear)
      t.putOffButton.gameObject:SetActive(false)
      
      local trainButtonPos = Vector3.zero
      if canWear then 
        trainButtonPos = Vector3(96, t.trainButton.transform.localPosition.y, t.trainButton.transform.localPosition.z)
      else
        trainButtonPos = Vector3(0, t.trainButton.transform.localPosition.y, t.trainButton.transform.localPosition.z)
      end
      t.trainButton.transform.localPosition = trainButtonPos
      t.trainButton.gameObject:SetActive(true)
    end
  else
    t.selectedEquipmentNameText.text = ''
    t.putOnButton.gameObject:SetActive(false)
    t.putOffButton.gameObject:SetActive(false)
    t.trainButton.gameObject:SetActive(false)
  end
end
function t.RefreshPower()
  local go = GameObject.Instantiate(t.goPowerRootPrefab)
  go:SetActive(true)
  local tran = go.transform
  tran:SetParent(t.tranAttrRoot,false)
  
  local tranIconRoot = tran:Find('selected_equip_icon_root')
  
  local textPower = tran:Find('text_power'):GetComponent(typeof(Text))
  if t.selectedEquipInfo then
    textPower.text = t.selectedEquipInfo:Power()
    local equipIcon = commonEquipIcon.New(tranIconRoot)
    equipIcon:SetEquipInfo(t.selectedEquipInfo)
  else
    textPower.text = '0'
  end
end
--基础属性
function t.RefreshMainAttr()
  local item = t.CreateAttrItem(t.tranAttrRoot)
  item.textTitle.text = ''
  item.imgIcon.gameObject:SetActive(false)
  if t.selectedEquipInfo then
    local totalBaseAttr = t.selectedEquipInfo:GetTotalBaseAttr()
    item.textName.text = totalBaseAttr:GetName()
    item.textNum.text = totalBaseAttr:GetValueString()
  else
    item.textName.text = ''
    item.textNum.text = ''
  end
  
  
end
--随机属性
function t.RefreshRandomAttr()
  local colors = ui_util.GetEquipRandomColor(t.selectedEquipInfo)
  for k,v in ipairs(t.selectedEquipInfo.randomAttrs) do
    local item = t.CreateAttrItem(t.tranAttrRoot)
    item.textTitle.text = LocalizationController.instance:Get('common.equipment.attribute.random_attribute_title')..k
    item.textName.text = v:GetName()
    item.textNum.text = v:GetValueString()
    item.textNum.color = colors[k]
    item.textName.color = colors[k]
    item.imgIcon.gameObject:SetActive(false)
  end
  
end
--宝石属性
function t.RefreshGemAttr()
    
  local item_data = gamemanager.GetData('item_data')
  local gem_attr_data = gamemanager.GetData('gem_attr_data')
  
  for k,v in ipairs(t.selectedEquipInfo.gemInsertIds) do
    local gemAttrData = gem_attr_data.GetDataById(v)
    
    if gemAttrData  then
      
      local itemData = item_data.GetDataById(gemAttrData.id)
      local item = t.CreateAttrItem(t.tranAttrRoot)
      item.textTitle.text = string.format(LocalizationController.instance:Get('common.role_icon.common_lv'),itemData.lv)
      item.textName.text = gemAttrData.equipAttr:GetName()
      item.textNum.text = gemAttrData.equipAttr:GetValueString()
      item.imgIcon.sprite = ResMgr.instance:LoadSprite(itemData:IconPath())
    end
  end
end
--
function t.CreateAttrItem(parent)
  local go = GameObject.Instantiate(t.goAttrItemPrefab)
  go:SetActive(true)
  local tran = go.transform
  tran:SetParent(parent,false)
  local item = {}
  item.root = tran
  item.textName = tran:Find('text_name'):GetComponent(typeof(Text))  
  item.textTitle = tran:Find('text_title'):GetComponent(typeof(Text))
  item.textNum = tran:Find('text_num'):GetComponent(typeof(Text))
  item.imgIcon = tran:Find('img_icon'):GetComponent(typeof(Image))
  return item
end

function t.RefreshRoleEquipmentIcons ()
  local roleWeaponInfo = nil
  local roleArmorInfo = nil
  local roleAccessoryInfo = nil
  
  if t.roleInfo ~= nil then
    roleWeaponInfo = gamemanager.GetModel('equip_model').GetEquipmentInfoByInstanceID(t.roleInfo.weaponID)
    roleArmorInfo = gamemanager.GetModel('equip_model').GetEquipmentInfoByInstanceID(t.roleInfo.armorID)
    roleAccessoryInfo = gamemanager.GetModel('equip_model').GetEquipmentInfoByInstanceID(t.roleInfo.accessoryID)
    
  end
  
  if roleWeaponInfo ~= nil then
    t.weaponIcon.transform.gameObject:SetActive(true)
    t.weaponIcon:SetEquipInfo(roleWeaponInfo)
  else
    t.weaponIcon.transform.gameObject:SetActive(false)
  end
  
  if roleArmorInfo ~= nil then
    t.armorIcon.transform.gameObject:SetActive(true)
    t.armorIcon:SetEquipInfo(roleArmorInfo)
  else
    t.armorIcon.transform.gameObject:SetActive(false)
  end
  
  if roleAccessoryInfo ~= nil then
    t.accessory1Icon.transform.gameObject:SetActive(true)
    t.accessory1Icon:SetEquipInfo(roleAccessoryInfo)
  else
    t.accessory1Icon.transform.gameObject:SetActive(false)
  end
end

function t.RegenerateFreeEquipmentItems (playAnimation)
  local equip_model = gamemanager.GetModel('equip_model')
  t.freeEquipmentInfoListOfRoleType = equip_model.GetFreeEquipmentInfoListOfRoleType(t.roleInfo.heroData.roleType)
  t.showEquipmentInfoList = {}
  
  if t.selectedEquipPos == RoleEquipPos.All then
    t.showEquipmentInfoList = t.freeEquipmentInfoListOfRoleType
  else
    for k, v in pairs(t.freeEquipmentInfoListOfRoleType) do
      if v.data.equipmentType == t.selectedEquipPos then
        table.insert(t.showEquipmentInfoList, v)
      end
    end
  end
  
  t.noWearableEquipmentGO:SetActive(table.count(t.showEquipmentInfoList) <= 0)
  local showGoDungeonButton = table.count(t.showEquipmentInfoList) <= 0
  if t.selectedEquipPos == 1 then
    showGoDungeonButton = showGoDungeonButton and t.roleInfo.weaponID <= 0
  elseif t.selectedEquipPos == 2 then
    showGoDungeonButton = showGoDungeonButton and t.roleInfo.armorID <= 0
  elseif t.selectedEquipPos == 3 then
    showGoDungeonButton = showGoDungeonButton and t.roleInfo.accessoryID <= 0
  end
  t.goDungeonButton.gameObject:SetActive(showGoDungeonButton)
  
  table.sort(t.showEquipmentInfoList, t.compareEquipInfo)
  t.scrollContent:Init(#t.showEquipmentInfoList, playAnimation, 0)
  t.selectedEquipmentIcon = nil
end

function t.RefreshFreeEquipmentItems ()
  if t.showEquipmentInfoList ~= nil then
    table.sort(t.showEquipmentInfoList, t.compareEquipInfo)
    t.scrollContent:RefreshAllContentItems()
  end
end

function t.OnResetItemHandler(gameObject, index)
  local equipInfo = t.showEquipmentInfoList[index + 1]
  local isNew = gamemanager.GetModel('equip_model').IsNewEquipment(equipInfo.id)
  local newEquipmentMarkTransform = gameObject.transform:Find('img_new_equipment_mark')
  newEquipmentMarkTransform.gameObject:SetActive(isNew)
  
  local levelNotEnoughGO = gameObject.transform:Find('text_level_not_enough').gameObject
  levelNotEnoughGO:SetActive(t.roleInfo.level < equipInfo.data.useLv)
  
  local commonEquipmentIcon = nil
  local transform = gameObject.transform:Find('icon_root/common_equipment_icon')
  if transform == nil then
    commonEquipmentIcon = commonEquipIcon.New(gameObject.transform:Find('icon_root'))
    transform = commonEquipmentIcon.transform
    transform.name = 'common_equipment_icon'
  else
    commonEquipmentIcon = commonEquipIcon.NewByGameObject(transform)
  end
  commonEquipmentIcon:SetEquipInfo(equipInfo)
  if t.selectedEquipInfo ~= nil and equipInfo.id == t.selectedEquipInfo.id then
    commonEquipmentIcon:SetSelect(true)
  else
    commonEquipmentIcon:SetSelect(false)
  end
  local levelTextColor = Color(1, 1, 1, 1)
  if t.roleInfo.level < equipInfo.data.useLv then
    levelTextColor = Color(1, 0, 0, 1)
  end
  commonEquipmentIcon:SetLevelColor(levelTextColor)
  commonEquipmentIcon.onClick:AddListener(t.OnClickEquipmentButton)
  
  
  local imgIconTransform = transform:Find('img_icon')
  local maskTransform = imgIconTransform:Find('mask')
  if maskTransform == nil then
    maskTransform = gameObject.transform:Find('mask')
    maskTransform:SetParent(imgIconTransform, false)
  end
  
  maskTransform.gameObject:SetActive(not equipUtil.canEquip (equipInfo, t.roleInfo))
end

function t.RefreshEquipmentCellNumText ()
		--equipmentCellNumText.text = string.Format(Localization.Get("common.value/max"), EquipmentProxy.instance.GetFreeEquipmentInfoList().Count, GameProxy.instance.EquipCellNum);
    t.equipmentCellNumText.text = table.count(gamemanager.GetModel('equip_model').GetFreeEquipmentInfoList())..'/'..gamemanager.GetModel('game_model').equipCellNum
end
    

function t.OnClickBackBtnHandler ()
  uimanager.CloseView(name)
end

function t.OnClickPutOnButtonHandler ()
  print('OnClickPutOnButtonHandler')
  
  if t.selectedEquipInfo ~= nil then
    if t.selectedEquipInfo.data.equipmentType == RoleEquipPos.Weapon and t.roleInfo.weaponID > 0 then
      local costGameResData = gamemanager.GetModel('equip_model').GetEquipmentInfoByInstanceID(t.roleInfo.weaponID).data.unloadCostGRD
      if costGameResData.count > 0 then        
        LuaInterface.LuaCsTransfer.OpenConfirmSubstituteEquipmentTipsView(t.roleInfo.instanceID, t.selectedEquipInfo.id, costGameResData.type, costGameResData.id, costGameResData.count)
        return
      end
    end
    if t.selectedEquipInfo.data.equipmentType == RoleEquipPos.Armor and t.roleInfo.armorID > 0 then
      local costGameResData = gamemanager.GetModel('equip_model').GetEquipmentInfoByInstanceID(t.roleInfo.armorID).data.unloadCostGRD
      if costGameResData.count > 0 then
        LuaInterface.LuaCsTransfer.OpenConfirmSubstituteEquipmentTipsView(t.roleInfo.instanceID, t.selectedEquipInfo.id, costGameResData.type, costGameResData.id, costGameResData.count)
        return
      end
    end
    if t.selectedEquipInfo.data.equipmentType == RoleEquipPos.Accessory and t.roleInfo.accessoryID > 0 then
      local costGameResData = gamemanager.GetModel('equip_model').GetEquipmentInfoByInstanceID(t.roleInfo.accessoryID).data.unloadCostGRD
      if costGameResData.count > 0 then
        LuaInterface.LuaCsTransfer.OpenConfirmSubstituteEquipmentTipsView(t.roleInfo.instanceID, t.selectedEquipInfo.id, costGameResData.type, costGameResData.id, costGameResData.count)
        return
      end
    end
    
    gamemanager.GetCtrl('equip_controller').EquipWearOffReq(t.selectedEquipInfo.id, 1, false, t.roleInfo.instanceID)
  end
end

function t.OnClickPutOffButtonHandler ()
  print('OnClickPutOffButtonHandler')
  local costGameResData = t.selectedEquipInfo.data.unloadCostGRD
  if t.selectedEquipInfo ~= nil then
    if costGameResData.count <= 0 then
      gamemanager.GetCtrl('equip_controller').EquipWearOffReq(t.selectedEquipInfo.id, 0, false, t.roleInfo.instanceID)
    else
      LuaInterface.LuaCsTransfer.OpenConfirmPutOffEquipmentTipsView(t.selectedEquipInfo.id, t.roleInfo.instanceID, costGameResData.type, costGameResData.id, costGameResData.count)
    end
  end
end

function t.OnClickTrainButtonHandler ()
  print('OnClickTrainButtonHandler')
  if t.selectedEquipInfo ~= nil then
    if t.selectedEquipInfo.ownerId == 0 then
      gamemanager.GetModel('equip_model').OpenTrainingViewByBag(t.selectedEquipInfo.id)
    else
      gamemanager.GetModel('equip_model').OpenTrainingViewByRoleInfo( t.roleInfo.instanceID,t.selectedEquipInfo.id)
    end
    
  end
end

function t.OnClickWeaponButtonHandler ()
  print('======click weaponButton======')
  local equipInfo = gamemanager.GetModel('equip_model').GetEquipmentInfoByInstanceID(t.roleInfo.weaponID)
  t.SetSelectedEquipmentInfo(equipInfo)
  t.selectedRoleEquipSlotMask.localPosition = t.weaponFrame.localPosition
  t.selectedEquipPos = RoleEquipPos.Weapon
  t.equipTypeDropdown.value = t.selectedEquipPos
  t.RegenerateFreeEquipmentItems(false)
end

function t.OnClickArmorButtonHandler ()
  print('======click armorButton======')
  local equipInfo = gamemanager.GetModel('equip_model').GetEquipmentInfoByInstanceID(t.roleInfo.armorID)
  t.SetSelectedEquipmentInfo(equipInfo)
  t.selectedRoleEquipSlotMask.localPosition = t.armorFrame.localPosition
  t.selectedEquipPos = RoleEquipPos.Armor
  t.equipTypeDropdown.value = t.selectedEquipPos
  t.RegenerateFreeEquipmentItems(false)
end

function t.OnClickAccessoryButtonHandler ()
  print('======click accessoryButton======')
  local equipInfo = gamemanager.GetModel('equip_model').GetEquipmentInfoByInstanceID(t.roleInfo.accessoryID)
  t.SetSelectedEquipmentInfo(equipInfo)
  t.selectedRoleEquipSlotMask.localPosition = t.accessory1Frame.localPosition
  t.selectedEquipPos = RoleEquipPos.Accessory
  t.equipTypeDropdown.value = t.selectedEquipPos
  t.RegenerateFreeEquipmentItems(false)
end

function t.OnClickExpandEquipBagButtonHandler ()
  local erro_tip_view = require('ui/tips/view/common_error_tips_view')
  local game_model = gamemanager.GetModel('game_model')
  local global_data = gamemanager.GetData('global_data')
  if game_model.equipCellNum >= global_data.equip_package_max_num then
    erro_tip_view.Open(LocalizationController.instance:Get("ui.common_tips.equipment_bag_reach_max"))
    return
  end
  local cost = (game_model.equipCellBuyNum + 1) * global_data.equip_package_buy_a + global_data.equip_package_buy_b
  local common_expand_bag_tip_view = require('ui/tips/view/common_expand_bag_tip_view')
  common_expand_bag_tip_view.Open2(BagType.EquipmentBag,cost)
end

function t.OnClickConfirmExpandBagButtonHandler ()
  gamemanager.GetCtrl('game_controller').BuyPackCellReq (BagType.EquipmentBag)
end

function t.OnEquipTypeChanged (index)
  print('OnEquipTypeChanged:'..index)
  t.selectedEquipPos = index
  t.RegenerateFreeEquipmentItems(true)
end

function t.OnClickEquipmentButton (commonEquipentIcon)
  if t.selectedEquipmentIcon ~= nil then
    t.selectedEquipmentIcon:SetSelect(false)
  end
  
  commonEquipentIcon:SetSelect(true)
  t.SetSelectedEquipmentInfo(commonEquipentIcon.equipInfo)
  t.selectedEquipmentIcon = commonEquipentIcon
end

function t.ClickGoDungeonButton ()
  if t.selectedEquipPos == 1 then
    local basicWeaponGameResData = game_res_data.New(BaseResType.Equipment, t.roleInfo.heroData.basicWeaponID, 0, 0)
    local dropMessageData = gamemanager.GetData('drop_message_data').GetDataByMockGameResData(basicWeaponGameResData)
    LuaCsTransfer.OpenDungeonDetailView(dropMessageData.jump_page1)
  elseif t.selectedEquipPos == 2 then
    local basicArmorGameResData = game_res_data.New(BaseResType.Equipment, t.roleInfo.heroData.basicArmorID, 0, 0)
    local dropMessageData = gamemanager.GetData('drop_message_data').GetDataByMockGameResData(basicArmorGameResData)
    LuaCsTransfer.OpenDungeonDetailView(dropMessageData.jump_page1)
  elseif t.selectedEquipPos == 3 then
    local basicAccessoryGameResData = game_res_data.New(BaseResType.Equipment, t.roleInfo.heroData.basicAccessoryID, 0, 0)
    local dropMessageData = gamemanager.GetData('drop_message_data').GetDataByMockGameResData(basicAccessoryGameResData)
    LuaCsTransfer.OpenDungeonDetailView(dropMessageData.jump_page1)
  end
end

-- proxy callback --
function t.OnEquipCellNumUpdate ()
  t.RefreshEquipmentCellNumText()
end

function t.OnEquipmentInfoUpdateHandler ()
  if t.selectedEquipInfo ~= nil then
    local euqipInfo = gamemanager.GetModel('equip_model').GetEquipmentInfoByInstanceID (t.selectedEquipInfo.id)
    t.SetSelectedEquipmentInfo (equipInfo)
  end
end

function t.OnEquipmentInfoListUpdate ()
  t.RefreshRoleEquipmentIcons ()
  t.RegenerateFreeEquipmentItems (false)
  t.RefreshEquipmentCellNumText ()
  
  if t.selectedEquipPos == RoleEquipPos.Weapon then
    t.OnClickWeaponButtonHandler()
  elseif t.selectedEquipPos == RoleEquipPos.Armor then
    t.OnClickArmorButtonHandler()
  elseif t.selectedEquipPos == RoleEquipPos.Accessory then
    t.OnClickAccessoryButtonHandler()
  end
end

function t.OnNewEquipmentMarksChanged ()
end
-- proxy callback --

-- 排序方法 --
function t.compareEquipInfo(equipInfoA, equipInfoB)
  local weightMax = 100
  local a = weightMax
  local b = weightMax
  
  -- 职业 --
  if equipInfoA.data.equipmentRoleType ~= t.roleInfo.heroData.roleType then  
    a = equipInfoA.data.equipmentRoleType
  end
  if equipInfoB.data.equipmentRoleType ~= t.roleInfo.heroData.roleType then  
    b = equipInfoB.data.equipmentRoleType
  end
  
  -- 部位 --
  if a == b then
    if equipInfoA.data.equipmentType == t.selectedEquipPos then
      a = weightMax
    else
      a = equipInfoA.data.equipmentType
    end
    if equipInfoB.data.equipmentType == t.selectedEquipPos then
      b = weightMax
    else
      b = equipInfoB.data.equipmentType
    end
  end
  
  -- 可穿戴 --
  if a == b then
    if equipUtil.canEquip(equipInfoA, t.roleInfo) then
      a = weightMax
    else
      a = 0
    end
    if equipUtil.canEquip(equipInfoB, t.roleInfo) then
      b = weightMax
    else
      b = 0
    end
  end
  
  -- 战力 --
  if a == b then
    a = equipInfoA:Power()
    b = equipInfoB:Power()
  end
  
  return a > b
end
-- 排序方法 --

Start ()
return t