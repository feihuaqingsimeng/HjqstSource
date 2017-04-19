local t = {}
local PREFAB_PATH = 'ui/pack/pack_view'

local game_model = gamemanager.GetModel('game_model')
local item_model = gamemanager.GetModel('item_model')
local equip_model = gamemanager.GetModel('equip_model')

local common_item_icon = require('ui/common_icon/common_item_icon')
local common_equip_icon = require('ui/common_icon/common_equip_icon')
local equip_decompose_data = gamemanager.GetData('equip_decompose_data')
local equip_controller = gamemanager.GetCtrl('equip_controller')

t.currentToggleIndex = 0
t.selectedEquipmentRoleTypeIndex = 0
t.selectedEquipmentIndex = 1

t.dropdownIndexRoleTypeDic = {}
t.dropdownIndexRoleTypeDic[0] = RoleType.Invalid     --全部     
t.dropdownIndexRoleTypeDic[1] = RoleType.Defence     --防御型
t.dropdownIndexRoleTypeDic[2] = RoleType.Offence     --攻击型
t.dropdownIndexRoleTypeDic[3] = RoleType.Mage        --魔法型
t.dropdownIndexRoleTypeDic[4] = RoleType.Support     --辅助型
t.dropdownIndexRoleTypeDic[5] = RoleType.Mighty      --全能型

function t.Open ()
  uimanager.RegisterView(PREFAB_PATH,t)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  --local common_top_bar = require ('ui/common_top_bar/common_top_bar')
  --common_top_bar = common_top_bar:Create(t.transform:Find('core'))
  --common_top_bar.transform:SetAsLastSibling()
  --common_top_bar:SetAsCommonStyle(LocalizationController.instance:Get('ui.pack_view.pack_title'), t.OnClickBackBtnHandler, true, true, true, false, false, false, false)
  
  t.InitComponent()
  t.BindDelegate ()
end

function t.InitComponent ()
  t.btnClose = t.transform:Find('core/img_title/btn_close'):GetComponent(typeof(Button))
  t.btnClose.onClick:AddListener(t.OnClickBackBtnHandler)
  t.desTitleText = t.transform:Find('core/des_frame/img_title/text_des_title'):GetComponent(typeof(Text))
  t.listTitleText = t.transform:Find('core/img_title/text_list_title'):GetComponent(typeof(Text))
  
  t.desFrame = t.transform:Find('core/des_frame')
  t.itemDesRoot = t.desFrame:Find('item_des_root')
  t.equipmentDesRoot = t.desFrame:Find('equipment_des_root')
  
  t.selectedItemIconTransform = t.transform:Find('core/des_frame/item_des_root/item_icon_root/common_item_icon_lua')
  t.selectedItemIcon = common_item_icon.NewByGameObject(t.selectedItemIconTransform)
  t.selectedItemNameText = t.transform:Find('core/des_frame/item_des_root/text_item_name'):GetComponent(typeof(Text))
  t.seletcedItemCountText = t.transform:Find('core/des_frame/item_des_root/img_item_count_frame/text_item_count'):GetComponent(typeof(Text))
  
  t.selectedItemDesTitleText = t.transform:Find('core/des_frame/item_des_root/img_des_frame/item_des_scroll_view/viewport/content/text_item_des_title'):GetComponent(typeof(Text))
  t.selectedItemDesTitleText.text = LocalizationController.instance:Get('ui.pack_view.item_des_title')
  t.selectedItemDesText = t.transform:Find('core/des_frame/item_des_root/img_des_frame/item_des_scroll_view/viewport/content/text_item_des'):GetComponent(typeof(Text))
  t.selectedItemProduceTitleText = t.transform:Find('core/des_frame/item_des_root/img_des_frame/item_des_scroll_view/viewport/content/text_item_produce_title'):GetComponent(typeof(Text))
  t.selectedItemProduceTitleText.text = LocalizationController.instance:Get('ui.pack_view.item_produce_title')
  t.selectedItemProduceText = t.transform:Find('core/des_frame/item_des_root/img_des_frame/item_des_scroll_view/viewport/content/text_item_produce'):GetComponent(typeof(Text))
  
  t.useButton = t.transform:Find('core/des_frame/item_des_root/btns_root/btn_use'):GetComponent(typeof(Button))
  t.useButton.onClick:AddListener(t.ClickUseButton)
  t.composeButton = t.transform:Find('core/des_frame/item_des_root/btns_root/btn_compose'):GetComponent(typeof(Button))
  t.composeButton.onClick:AddListener(t.ClickComposeButton)
  t.decomposeEquipByPiece = t.transform:Find('core/des_frame/item_des_root/btns_root/btn_equip_decompose'):GetComponent(typeof(Button))
  t.decomposeEquipByPiece.onClick:AddListener(t.ClickDecomposeEquipmentButton)

  t.selectedEquipmentIconTransform = t.equipmentDesRoot:Find('equip_icon_root/common_equipment_icon')
  t.selectedEquipmentIcon = common_equip_icon.NewByGameObject(t.selectedEquipmentIconTransform)
  
  t.selectedEquipmentNameText = t.equipmentDesRoot:Find('text_equipment_name'):GetComponent(typeof(Text))
  t.selectedEquipmentPowerRoot = t.equipmentDesRoot:Find('img_equipment_power_frame')
  t.selectedEquipmentPowerText = t.equipmentDesRoot:Find('img_equipment_power_frame/text_equipment_power'):GetComponent(typeof(Text))
  t.equipmentAttributesScrollView = t.equipmentDesRoot:Find('scroll_view_equipment_attributes')
  t.equipmentAttributeItemPrefabGameObject = t.equipmentAttributesScrollView:Find('viewport/attr_item').gameObject
  t.equipmentAttributeItemPrefabGameObject:SetActive(false)
  t.equipmentAttributesScollContentTransform = t.equipmentAttributesScrollView:Find('viewport/content')
  t.trainEquipmentButton = t.equipmentDesRoot:Find('btns_root/btn_train_equipment'):GetComponent(typeof(Button))
  t.trainEquipmentButton.onClick:AddListener(t.ClickTrainEquipmentButton)
  t.decomposeEquipmentButton = t.equipmentDesRoot:Find('btns_root/btn_equip_decompose'):GetComponent(typeof(Button))
  t.decomposeEquipmentButton.onClick:AddListener(t.ClickDecomposeEquipmentButton)
  t.listFrame = t.transform:Find('core/img_list_frame')
  
  t.itemsScrollView = t.listFrame:Find('scroll_view_items')
  t.itemsScrollContent = t.itemsScrollView:Find('viewport/content'):GetComponent(typeof(ScrollContent))
  t.itemsScrollContent.onResetItem:AddListener(t.OnResetItemHandler)
  
  t.equipmentsScrollView = t.listFrame:Find('scroll_view_equipments')
  t.equipmentsScrollContent = t.equipmentsScrollView:Find('viewport/content'):GetComponent(typeof(ScrollContent))
  t.equipmentsScrollContent.onResetItem:AddListener(t.OnResetEquipmentHandler)
  
  t.equipmentsBottomRoot = t.equipmentsScrollView:Find('bottom_root')
  t.equipmentsCountText = t.equipmentsBottomRoot:Find('text_equipments_count'):GetComponent(typeof(Text))
  t.expandEquipmentPackButton = t.equipmentsBottomRoot:Find('btn_expand_equipment_pack'):GetComponent(typeof(Button))
  t.expandEquipmentPackButton.onClick:AddListener(t.ClickExpandEquipmentPackButton)
  t.equipmentRoleTypeDropDown = t.equipmentsBottomRoot:Find('dropdown_equipment_type'):GetComponent(typeof(Dropdown))
  t.equipmentRoleTypeDropDown.captionText.text = LocalizationController.instance:Get("ui.equipments_browse_view.equipment_type.all")
  
  t.equipmentRoleTypeDropDown.options:Add(UnityEngine.UI.Dropdown.OptionData.New(LocalizationController.instance:Get("ui.equipments_browse_view.equipment_type.all"))) 
  t.equipmentRoleTypeDropDown.options:Add(UnityEngine.UI.Dropdown.OptionData.New(LocalizationController.instance:Get("ui.equipments_browse_view.equipment_type.defence")))
  t.equipmentRoleTypeDropDown.options:Add(UnityEngine.UI.Dropdown.OptionData.New(LocalizationController.instance:Get("ui.equipments_browse_view.equipment_type.offence")))
  t.equipmentRoleTypeDropDown.options:Add(UnityEngine.UI.Dropdown.OptionData.New(LocalizationController.instance:Get("ui.equipments_browse_view.equipment_type.mage")))
  t.equipmentRoleTypeDropDown.options:Add(UnityEngine.UI.Dropdown.OptionData.New(LocalizationController.instance:Get("ui.equipments_browse_view.equipment_type.support")))
  t.equipmentRoleTypeDropDown.options:Add(UnityEngine.UI.Dropdown.OptionData.New(LocalizationController.instance:Get("ui.equipments_browse_view.equipment_type.mighty"))) 
  
  t.equipmentRoleTypeDropDown.onValueChanged:AddListener(t.OnEquipmentRoleTypeChanged)
  
  t.itemTypeTogglesRoot = t.transform:Find('core/scroll_view/viewport/content')
  t.itemTypeTogglePrefab = t.transform:Find('core/scroll_view/viewport/toggle').gameObject
  
  t.toggles = {}
  t.allPackageDataList = gamemanager.GetData('package_data').GetAllPackageData()
  for k, v in ipairs(t.allPackageDataList) do
    local toggle = Instantiate(t.itemTypeTogglePrefab):GetComponent(typeof(Toggle))
    toggle.transform:Find('img_icon'):GetComponent(typeof(Image)).sprite = Common.ResMgr.ResMgr.instance:LoadSprite('sprite/main_ui/'..v.type_icon)
    toggle.transform:Find('text'):GetComponent(typeof(Text)).text = LocalizationController.instance:Get(v.type_name)
    toggle:GetComponent(typeof(Common.UI.Components.EventTriggerDelegate)).onClick:AddListener(t.ClickToggleHandler)
    toggle.transform:SetParent(t.itemTypeTogglesRoot, false)
    toggle.name = 'toggle_item'..v.id
    t.toggles[k] = toggle
    t.toggles[k].transform:Find('img_red_point'):GetComponent('RedPointView').enabled = false
  end
  local toggle = Instantiate(t.itemTypeTogglePrefab):GetComponent(typeof(Toggle))
  toggle.transform:Find('img_icon'):GetComponent(typeof(Image)).sprite = Common.ResMgr.ResMgr.instance:LoadSprite('sprite/main_ui/icon_page_zhuangbei')
  toggle.transform:Find('text'):GetComponent(typeof(Text)).text = LocalizationController.instance:Get('ui.pack_view.tag.equipments')
  toggle:GetComponent(typeof(Common.UI.Components.EventTriggerDelegate)).onClick:AddListener(t.ClickToggleHandler)
  toggle.transform:SetParent(t.itemTypeTogglesRoot, false)
  t.toggles[#t.toggles + 1] = toggle
  t.equipmentToggle = toggle
  t.equipmentToggle.name = 'toggle_equipment'
  t.newEquipmentRedPointView = t.equipmentToggle.transform:Find('img_red_point'):GetComponent('RedPointView')
  t.newEquipmentRedPointView.luaType = {RedPointType.RedPoint_New_Equip}
  t.itemTypeTogglePrefab:SetActive(false)
  
  t.SetToggleOn(1)
  t.RefreshNewMarkOfToggles()
  
  t.RefreshEquipmentCount ()
end

function t.BindDelegate ()
  item_model.updateItemInfoListDelegate:AddListener(t.OnItemInfoListUpdateHandler)
  equip_model.OnEquipmentInfoUpdateDelegate:AddListener(t.OnEquipmentInfoUpdateHandler)
  equip_model.OnEquipmentInfoListUpdateDelegate:AddListener(t.OnEquipmentInfoListUpdateHandler)
  game_model.UpdateEquipCellNumDelegate:AddListener(t.OnUpdateEquipCellNumHandler)
end

function t.UnbindDelegate ()
  item_model.updateItemInfoListDelegate:RemoveListener(t.OnItemInfoListUpdateHandler)
  equip_model.OnEquipmentInfoUpdateDelegate:RemoveListener(t.OnEquipmentInfoUpdateHandler)
  equip_model.OnEquipmentInfoListUpdateDelegate:RemoveListener(t.OnEquipmentInfoListUpdateHandler)
  game_model.UpdateEquipCellNumDelegate:RemoveListener(t.OnUpdateEquipCellNumHandler)
end

function t.OnResetItemHandler(gameObject,index)
  local itemIcon = t.itemIcons[gameObject]
  local itemInfo = t.currentItemInfoList[index + 1]
  if itemIcon == nil then
    itemIcon = common_item_icon.NewByGameObject(gameObject)
    itemIcon.onClick:AddListener(t.ClickItemHandler)
    t.itemIcons[gameObject] = itemIcon
  end
  itemIcon:SetItemInfo(itemInfo)
  itemIcon:ShowNewMark(itemInfo.isNew)
  itemIcon.index = index + 1
  itemIcon:SetSelect(t.selectedItemIndex == itemIcon.index)
  gameObject.name = itemInfo.itemData.id
end

function t.OnResetEquipmentHandler (gameObject, index)
  local equipmentIcon = t.equipmentIcons[gameObject]
  local equipmentInfo = t.currentEquipInfoList[index + 1]
  if equipmentIcon == nil then
    equipmentIcon = common_equip_icon.NewByGameObject(gameObject)
    equipmentIcon.onClick:AddListener(t.ClickEquipmentHandler)
    t.equipmentIcons[gameObject] = equipmentIcon
  end
  equipmentIcon:SetEquipInfo(equipmentInfo)
  equipmentIcon:ShowNewMark(equip_model.IsNewEquipment(equipmentInfo.id))
  equipmentIcon.index = index + 1
  equipmentIcon:SetSelect(t.selectedEquipmentIndex == equipmentIcon.index)
end

function t.RefreshItemDes ()
  selectedItemInfo = t.currentItemInfoList[t.selectedItemIndex]
  if selectedItemInfo ~= nil then
    t.selectedItemIcon:SetItemInfo(selectedItemInfo)
    t.selectedItemIcon:ShowCount(false)
    t.selectedItemIconTransform.gameObject:SetActive(true)
    t.selectedItemNameText.text = LocalizationController.instance:Get(selectedItemInfo.itemData.name)
    t.seletcedItemCountText.text = string.format(LocalizationController.instance:Get('ui.pack_view.item_count'), selectedItemInfo.count)
    t.selectedItemDesTitleText.gameObject:SetActive(true)
    t.selectedItemDesText.text = LocalizationController.instance:Get(selectedItemInfo.itemData.des)
    t.selectedItemProduceTitleText.gameObject:SetActive(true)
    t.selectedItemProduceText.text = LuaCsTransfer.GetDropDataDes(BaseResType.Item, selectedItemInfo.itemData.id)
    
    if selectedItemInfo.itemData.type == ItemType.ExpPotion then
      t.useButton.gameObject:SetActive(true)
      t.composeButton.gameObject:SetActive(false)
    elseif selectedItemInfo.itemData.type == ItemType.Gem then
      t.useButton.gameObject:SetActive(true)
      local canCombine, count, materialCount = item_model.CheckGemCombine(selectedItemInfo.itemData.id)
      t.composeButton.gameObject:SetActive(canCombine)
    elseif selectedItemInfo.itemData.type == ItemType.Enchant then
      t.useButton.gameObject:SetActive(true)
      t.composeButton.gameObject:SetActive(false)
    elseif selectedItemInfo.itemData.type == ItemType.StarLevelUpGem then
      t.useButton.gameObject:SetActive(true)
      t.composeButton.gameObject:SetActive(true)
    elseif selectedItemInfo.itemData.type == ItemType.HeroPiece then
      local heroPieceData = gamemanager.GetData('piece_data').GetDataById(selectedItemInfo.itemData.id)
      t.useButton.gameObject:SetActive(false)
      t.composeButton.gameObject:SetActive(heroPieceData.can_compose)
    elseif selectedItemInfo.itemData.type == ItemType.EquipPiece then
      local equipPieceData = gamemanager.GetData('piece_data').GetDataById(selectedItemInfo.itemData.id)
      t.useButton.gameObject:SetActive(false)
      t.composeButton.gameObject:SetActive(equipPieceData.can_compose)
    elseif selectedItemInfo.itemData.type == ItemType.RandomGift then
      t.useButton.gameObject:SetActive(true)
      t.composeButton.gameObject:SetActive(false)
    else
      t.useButton.gameObject:SetActive(false)
      t.composeButton.gameObject:SetActive(false)
    end
    
    local itemInfo = selectedItemInfo
    print(itemInfo.itemData.id ,itemInfo.itemData.type == ItemType.EquipPiece and equip_decompose_data.GetDataByTypeAndItemId(BaseResType.Item,itemInfo.itemData.id) )
    if itemInfo.itemData.type == ItemType.EquipPiece and equip_decompose_data.GetDataByTypeAndItemId(BaseResType.Item,itemInfo.itemData.id) then
      t.decomposeEquipByPiece.gameObject:SetActive(true)
    else
      t.decomposeEquipByPiece.gameObject:SetActive(false)
    end
  else
    t.selectedItemIconTransform.gameObject:SetActive(false)
    t.selectedItemNameText.text = ''
    t.seletcedItemCountText.text = ''
    t.selectedItemDesTitleText.gameObject:SetActive(false)
    t.selectedItemDesText.text = ''
    t.selectedItemProduceTitleText.gameObject:SetActive(false)
    t.selectedItemProduceText.text = ''
    t.useButton.gameObject:SetActive(false)
    t.composeButton.gameObject:SetActive(false)
    t.decomposeEquipByPiece.gameObject:SetActive(false)
  end
end

-- [[ CREATE EQUIPMENT ATTRIBUTE ITEM ]] --
function t.CreateAttrItem(parent)
  local go = GameObject.Instantiate(t.equipmentAttributeItemPrefabGameObject)
  go:SetActive(true)
  local tran = go.transform
  tran:SetParent(parent, false)
  local item = {}
  item.root = tran
  item.textName = tran:Find('text_name'):GetComponent(typeof(Text))  
  item.textTitle = tran:Find('text_title'):GetComponent(typeof(Text))
  item.textNum = tran:Find('text_num'):GetComponent(typeof(Text))
  item.imgIcon = tran:Find('img_icon'):GetComponent(typeof(Image))
  return item
end
-- [[ CREATE EQUIPMENT ATTRIBUTE ITEM ]] --

-- [[ REFRESH EQUIPMENT MAIN ATTRIBUTE ]] --
function t.RefreshMainAttr()
  local currentSelectedEquipmentInfo = t.GetCurrentSelectedEquipmentInfo()
  
  local item = t.CreateAttrItem(t.equipmentAttributesScollContentTransform)
  item.textTitle.text = LocalizationController.instance:Get('common.equipment.attribute.base_attribute_title')
  local totalBaseAttr = currentSelectedEquipmentInfo:GetTotalBaseAttr()
  item.textName.text = totalBaseAttr:GetName()
  item.textNum.text = totalBaseAttr:GetValueString()
  item.imgIcon.gameObject:SetActive(false)
end
-- [[ REFRESH EQUIPMENT MAIN ATTRIBUTE ]] --

-- [[ REFRESH EQUIPMENT RANDOM ATTRIBUTE ]] --
function t.RefreshRandomAttr()
  local currentSelectedEquipmentInfo = t.GetCurrentSelectedEquipmentInfo()
  
  local colors = ui_util.GetEquipRandomColor(currentSelectedEquipmentInfo)
  for k,v in ipairs(currentSelectedEquipmentInfo.randomAttrs) do
    local item = t.CreateAttrItem(t.equipmentAttributesScollContentTransform)
    item.textTitle.text = LocalizationController.instance:Get('common.equipment.attribute.random_attribute_title')..k
    item.textName.text = v:GetName()
    item.textNum.text = v:GetValueString()
    item.textNum.color = colors[k]
    item.textName.color = colors[k]
    item.imgIcon.gameObject:SetActive(false)
  end
end
-- [[ REFRESH EQUIPMENT RANDOM ATTRIBUTE ]] --

-- [[ REFRESH EQUIPMENT GEM ATTRIBUTE ]] --
function t.RefreshGemAttr()
  local currentSelectedEquipmentInfo = t.GetCurrentSelectedEquipmentInfo()
  
  local item_data = gamemanager.GetData('item_data')
  local gem_attr_data = gamemanager.GetData('gem_attr_data')
  for k,v in ipairs(currentSelectedEquipmentInfo.gemInsertIds) do
    local gemAttrData = gem_attr_data.GetDataById(v)
    if gemAttrData  then
      local itemData = item_data.GetDataById(gemAttrData.id)
      local item = t.CreateAttrItem(t.equipmentAttributesScollContentTransform)
      item.textTitle.text = string.format(LocalizationController.instance:Get('common.role_icon.common_lv'),itemData.lv)
      item.textName.text = gemAttrData.equipAttr:GetName()
      item.textNum.text = gemAttrData.equipAttr:GetValueString()
      item.imgIcon.sprite = ResMgr.instance:LoadSprite(itemData:IconPath())
    end
  end
end
-- [[ REFRESH EQUIPMENT GEM ATTRIBUTE ]] --

-- [[ REFRESH EQUIPMENT ATTRIBUTE ]] --
function t.RefreshAttr()
  ui_util.ClearChildren(t.equipmentAttributesScollContentTransform, true)
  
  local currentSelectedEquipmentInfo = t.GetCurrentSelectedEquipmentInfo ()
  if currentSelectedEquipmentInfo == nil then
    return
  end
 
  t.RefreshMainAttr()
  t.RefreshRandomAttr()
  t.RefreshGemAttr()
end
-- [[ REFRESH EQUIPMENT ATTRIBUTE ]] --

function t.RefreshEquipmentDes ()
  local currentSelectedEquipmentInfo = t.GetCurrentSelectedEquipmentInfo()
  if currentSelectedEquipmentInfo ~= nil then
    t.selectedEquipmentIcon:SetEquipInfo(currentSelectedEquipmentInfo)
    t.selectedEquipmentIcon.transform.gameObject:SetActive(true)
    t.selectedEquipmentNameText.text = LocalizationController.instance:Get(currentSelectedEquipmentInfo.data.name)
    t.selectedEquipmentNameText.color = ui_util.GetEquipQualityColor(currentSelectedEquipmentInfo.data)
    t.selectedEquipmentPowerText.text = currentSelectedEquipmentInfo:Power()
    t.selectedEquipmentPowerRoot.gameObject:SetActive(true)
    t:RefreshAttr()
    t.equipmentAttributesScrollView.gameObject:SetActive(true)
    t.trainEquipmentButton.gameObject:SetActive(true)
    if equip_decompose_data.GetDataByTypeAndItemId(BaseResType.Equipment,currentSelectedEquipmentInfo.data.id) then
      t.decomposeEquipmentButton.gameObject:SetActive(true)
    else
      t.decomposeEquipmentButton.gameObject:SetActive(false)
    end
  else
    t.selectedEquipmentIcon.transform.gameObject:SetActive(false)
    t.selectedEquipmentNameText.text = ''
    t.selectedEquipmentPowerRoot.gameObject:SetActive(false)
    t.equipmentAttributesScrollView.gameObject:SetActive(false)
    t.trainEquipmentButton.gameObject:SetActive(false)
    t.decomposeEquipmentButton.gameObject:SetActive(false)
  end
  
  
end

-- [[ 刷新道具列表 ]] --
function t.RefreshItemList ()
  local packageData = t.allPackageDataList[t.currentToggleIndex]
  t.listTitleText.text = LocalizationController.instance:Get(packageData.type_name)
  
  t.currentItemInfoList = {}
  local itemTypes = packageData:GetItemTypes ()
  for k, v in ipairs(itemTypes) do
    local tempItemInfoList = item_model.GetItemInfoListByItemType(v)
    for k2, v2 in pairs(tempItemInfoList) do
      table.insert(t.currentItemInfoList, v2)
    end
  end
  
  table.sort(t.currentItemInfoList, t.SortItemInfo)
  
  t.itemIcons = {}
  if t.selectedItemIndex > #t.currentItemInfoList then
    t.selectedItemIndex = #t.currentItemInfoList
  end
  t.itemsScrollContent:Init(#t.currentItemInfoList, false, 0)
  t.RefreshItemDes()
end

-- [[ 刷新装备列表 ]] --
function t.RefreshEquipmentList ()
  t.equipmentIcons = {}
  if t.selectedEquipmentRoleTypeIndex == 0 then
    t.currentEquipInfoList = equip_model.GetFreeEquipmentInfoList()
  else
    selectedRoleType = t.dropdownIndexRoleTypeDic[t.selectedEquipmentRoleTypeIndex]
    t.currentEquipInfoList = equip_model.GetFreeEquipmentInfoListOfRoleType(selectedRoleType)
  end
  table.sort(t.currentEquipInfoList, t.SortEquipmentInfo)
  t.equipmentsScrollContent:Init(table.count(t.currentEquipInfoList), false, 0)
  t.RefreshEquipmentDes()
end

-- [[ 刷新装备数量 ]] --
function t.RefreshEquipmentCount ()
  t.equipmentsCountText.text = string.format(LocalizationController.instance:Get('common.value/max_lua'), equip_model.GetFreeEquipmentCount(), game_model.equipCellNum)
end

function t.RefreshNewMarkOfToggles ()
  for k, v in ipairs(t.allPackageDataList) do
    local toggle = t.toggles[k]
    local newMarkGameObject = toggle.transform:Find('img_red_point').gameObject
    local showNewMark = false
    local itemTypeList = v:GetItemTypes ()
    for k2, v2 in ipairs(itemTypeList) do
      if item_model.HasNewItemOfType(v2) then
        showNewMark = true
        break
      end
    end
    newMarkGameObject:SetActive(showNewMark)
  end
end

function t.Close ()
  t.transform = nil
  item_model.ClearAllNewItemMark()
  equip_model.ClearNewEquipmentMarks()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
end

function t.OnEquipmentRoleTypeChanged (index)
  t.selectedEquipmentRoleTypeIndex = index
  t.selectedEquipmentIndex = 1
  t.RefreshEquipmentList()
end

-- [[ PROXY CALLBACK ]] --
function t.OnItemInfoListUpdateHandler ()
  t.RefreshItemList()
end

function t.OnEquipmentInfoUpdateHandler (equipmentInstanceID)
  t.RefreshEquipmentList()
end

function t.OnEquipmentInfoListUpdateHandler ()
  t.RefreshEquipmentList()
  t.RefreshEquipmentCount()
end

function t.OnUpdateEquipCellNumHandler ()
  t.RefreshEquipmentCount()
end
-- [[ PROXY CALLBACK ]] --

function t.OnClickBackBtnHandler ()
  t.Close()
end

function t.RefreshAsItemList ()
  t.itemsScrollView.gameObject:SetActive(true)
  t.equipmentsScrollView.gameObject:SetActive(false)
  t.desTitleText.text = LocalizationController.instance:Get('ui.pack_view.item_des_title')
  t.itemDesRoot.gameObject:SetActive(true)
  t.equipmentDesRoot.gameObject:SetActive(false)
end

function t.RefreshAsEquipmentList ()
  t.itemsScrollView.gameObject:SetActive(false)
  t.equipmentsScrollView.gameObject:SetActive(true)
  t.desTitleText.text = LocalizationController.instance:Get('ui.pack_view.equipment_des_title')
  t.itemDesRoot.gameObject:SetActive(false)
  t.equipmentDesRoot.gameObject:SetActive(true)
  t.listTitleText.text = LocalizationController.instance:Get('ui.pack_view.equipments_title')
end

function t.ClickToggleHandler(gameObject)
  
  for k, v in ipairs(t.toggles) do
    if v.gameObject == gameObject then
      t.currentToggleIndex = k
      v.isOn = true
    else
      v.isOn = false
    end
  end
  if t.equipmentToggle.gameObject == gameObject then -- 点击装备页签
    t.selectedEquipmentIndex = 1
    t.RefreshAsEquipmentList()
    t.RefreshEquipmentList()
  else                                               -- 点击出装备以外的其他页签
    t.RefreshAsItemList()
    
    t.selectedItemIndex = 1
    t.RefreshItemList()
  end
  
end

function t.SetToggleOn(index)
  if index >= 1 and index <= #t.toggles then
    t.toggles[index]:GetComponent(typeof(Toggle)).isOn = true
    t.ClickToggleHandler(t.toggles[index].gameObject)
  end
end

function t.ClickItemHandler(itemIcon)
  t.selectedItemIndex = itemIcon.index
  item_model.SetItemAsChecked (itemIcon.itemInfo.itemData.id)
  t.itemsScrollContent:RefreshAllContentItems()
  t.RefreshItemDes ()
  t.RefreshNewMarkOfToggles ()
end

function t.ClickEquipmentHandler (equipmentIcon)
  t.selectedEquipmentIndex = equipmentIcon.index
  equip_model.SetEquipmentAsChecked(equipmentIcon.equipInfo.id)
  t.equipmentsScrollContent:RefreshAllContentItems()
  t.RefreshEquipmentDes()
end

function t.ClickUseButton ()
  selectedItemInfo = t.currentItemInfoList[t.selectedItemIndex]
  if selectedItemInfo == nil then
    return
  end
  
  if selectedItemInfo.itemData.type == ItemType.ExpPotion then
    gamemanager.GetCtrl('hero_controller').OpenHeroInfoView()
  elseif selectedItemInfo.itemData.type == ItemType.Gem then
    LuaCsTransfer.OpenEquipmentsBrowseView()
  elseif selectedItemInfo.itemData.type == ItemType.Enchant then
    LuaCsTransfer.OpenEquipmentsBrowseView()
  elseif selectedItemInfo.itemData.type == ItemType.StarLevelUpGem then
    LuaCsTransfer.OpenEquipmentsBrowseView()
  elseif selectedItemInfo.itemData.type == ItemType.HeroPiece then
    -- do nothing
  elseif selectedItemInfo.itemData.type == ItemType.RandomGift then
    gamemanager.GetCtrl('item_controller').OpenGiftBagReq(selectedItemInfo.itemData.id)
  end
end

function t.ClickGetItemButtonHandler ()
  local pieceData = gamemanager.GetData('piece_data').GetDataById(selectedItemInfo.itemData.id)
  local heroGameResData = pieceData:GetHeroGameResData()
  LuaInterface.LuaCsTransfer.OpenGoodsJumpPath(BaseResType.Item, selectedItemInfo.itemData.id, 0)
end

function t.ClickComposeButton ()
  selectedItemInfo = t.currentItemInfoList[t.selectedItemIndex]
  if selectedItemInfo == nil then
    return
  end
  
  if selectedItemInfo.itemData.type == ItemType.ExpPotion then
  elseif selectedItemInfo.itemData.type == ItemType.Gem then
    local canCombine, count, materialCount = item_model.CheckGemCombine(selectedItemInfo.itemData.id)
    if canCombine == true then
      gamemanager.GetCtrl('equip_controller').InlayGemComposeReq(selectedItemInfo.itemData.id, 0, 0)
    end
  elseif selectedItemInfo.itemData.type == ItemType.Enchant then
  elseif selectedItemInfo.itemData.type == ItemType.StarLevelUpGem then
    gamemanager.GetCtrl('equip_controller').StarGemComposeReq(selectedItemInfo.itemData.id)
  elseif selectedItemInfo.itemData.type == ItemType.HeroPiece then
    local heroPieceData = gamemanager.GetData('piece_data').GetDataById(selectedItemInfo.itemData.id)
    local pieceGameResData = heroPieceData:GetPieceGameResData()
    
    if gamemanager.GetModel('item_model').GetItemCountByItemID(pieceGameResData.id) < pieceGameResData.count then
      local confirm_tip_view = dofile('ui/tips/view/confirm_tip_view')
      confirm_tip_view.Open(LocalizationController.instance:Get('ui.pack_view.tips.item_not_enough'), t.ClickGetItemButtonHandler)
      return
    end
    gamemanager.GetCtrl('hero_controller').HeroPieceComposeReq(heroPieceData.id)
  elseif selectedItemInfo.itemData.type == ItemType.EquipPiece then
    local equipPieceData = gamemanager.GetData('piece_data').GetDataById(selectedItemInfo.itemData.id)
    local pieceGameResData = equipPieceData:GetPieceGameResData()
    
    if gamemanager.GetModel('item_model').GetItemCountByItemID(pieceGameResData.id) < pieceGameResData.count then
      local confirm_tip_view = dofile('ui/tips/view/confirm_tip_view')
      confirm_tip_view.Open(LocalizationController.instance:Get('ui.pack_view.tips.item_not_enough'), t.ClickGetItemButtonHandler)
      return
    end
    gamemanager.GetCtrl('equip_controller').EquipPieceComposeReq(equipPieceData.id)
  end
end

function t.ClickTrainEquipmentButton ()
  equip_model.OpenTrainingViewByBag(t.GetCurrentSelectedEquipmentInfo().id)
end
--分解
function t.ClickDecomposeEquipmentButton ()
  local equipInfo = nil
  local itemInfo = nil
  if t.equipmentToggle.gameObject == t.toggles[t.currentToggleIndex].gameObject then
    equipInfo = t.GetCurrentSelectedEquipmentInfo()
  else
    itemInfo = t.currentItemInfoList[t.selectedItemIndex]
  end
  if equipInfo ~= nil and (equipInfo.star> 0 or equipInfo.strengthenLevel > 0 ) then
    equip_model.OpenEquipDecomposeView(equipInfo,itemInfo)
  else
    if gamemanager.GetModel('consume_tip_model').GetConsumeTipEnable(ConsumeTipType.EquipDecompose) then
      equip_model.OpenEquipDecomposeView(equipInfo,itemInfo)
    else
      equip_controller.EquipDeComposeReqByEquipInfo(equipInfo)
      equip_controller.EquipDeComposeReqByEquipPiece(itemInfo)
    end
  end
  
  
end
function t.ClickExpandEquipmentPackButton ()
  gamemanager.GetCtrl('equip_controller').ExpandEquipmentCell()
end

 -- [[ GET CURRENT SELETCED EQUIPMENT INFO ]] --
function t.GetCurrentSelectedEquipmentInfo ()
  return t.currentEquipInfoList[t.selectedEquipmentIndex]
end
 -- [[ GET CURRENT SELETCED EQUIPMENT INFO ]] --

function t.SortItemInfo (aItemInfo, bItemInfo)
  local aWeight = 0
  local bWeight = 0
  
  if aItemInfo.isNew then
    aWeight = -1
  end
  if bItemInfo.isNew then
    bWeight = -1
  end
  
  if aWeight == bWeight then
    local packageData = t.allPackageDataList[t.currentToggleIndex]

    local itemTypeWeightTable = {}
    local itemTypes = packageData:GetItemTypes ()
    for k, v in ipairs(itemTypes) do
      itemTypeWeightTable[v] = k
    end
    
    aWeight = itemTypeWeightTable[aItemInfo.itemData.type]
    bWeight = itemTypeWeightTable[bItemInfo.itemData.type]
  end
  
  if aWeight == bWeight then
    aWeight = aItemInfo.itemData.quality
    bWeight = bItemInfo.itemData.quality
  end
  
  if aWeight == bWeight then
    aWeight = aItemInfo.itemData.id
    bWeight = bItemInfo.itemData.id
  end
  
  return aWeight < bWeight
end

function t.SortEquipmentInfo (aEquipmentInfo, bEquipmentInfo)
  local aWeight = 0
  local bWeight = 0
  
  if equip_model.IsNewEquipment(aEquipmentInfo.id) then
    aWeight = -1
  end
  
  if equip_model.IsNewEquipment(bEquipmentInfo.id) then
    bWeight = -1
  end
  
  if aWeight == bWeight then
    aWeight = aEquipmentInfo.data.quality
    bWeight = bEquipmentInfo.data.quality
  end
  
  if aWeight == bWeight then
    aWeight = aEquipmentInfo.data.id
    bWeight = bEquipmentInfo.data.id
  end
  
  if aWeight == bWeight then
    aWeight = aEquipmentInfo:Power()
    bWeight = bEquipmentInfo:Power()
  end
  
  return aWeight < bWeight
end

return t