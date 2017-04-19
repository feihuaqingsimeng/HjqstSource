local t = {}
local global_data = gamemanager.GetData('global_data')
t.maxScrollLevel = global_data.max_scroll_lv
t.selectedScrollLevelIndex = 0
local PREFAB_PATH = 'ui/equipments/enchanting/equipment_enchanting_panel'
local scroll_level_item = dofile('ui/equip/view/equip_training/enchanting/scroll_level_item')

function t.Open (leftParent, rightParent, equipInfo,trainingView)
  t.equipInfo = equipInfo
  local prefab = Common.ResMgr.ResMgr.instance:Load(PREFAB_PATH)
  local gameObject = GameObject.Instantiate(prefab)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  t.transform:SetParent(rightParent, false)
  t.trainingView = trainingView
  t.bottomLeft = t.transform:Find('bottom_left')
  t.bottomLeft:SetParent(leftParent, false)
  t.bottomLeft.localPosition = Vector3(0, 0, 0)
  t.right = t.transform:Find('right')
  
  t.scrollIconImage = t.bottomLeft:Find('img_scroll_icon'):GetComponent(typeof(Image))
  t.scrollNameText = t.bottomLeft:Find('text_scroll_name'):GetComponent(typeof(Text))
  t.scrollAttrText = t.bottomLeft:Find('text_scroll_attr'):GetComponent(typeof(Text))
  
  t.useButton = t.bottomLeft:Find('btn_use'):GetComponent(typeof(Button))
  t.useButton.onClick:AddListener(t.ClickUse)
  t.makeButton = t.bottomLeft:Find('btn_make'):GetComponent(typeof(Button))
  t.makeButton.onClick:AddListener(t.ClickMake)
  
  t.scrollLevelDropdown = t.right:Find('img_top_bar/dropdown_scroll_level'):GetComponent(typeof(Dropdown))
  t.scrollLevelDropdown.captionText.text = LocalizationController.instance:Get("ui.equipment_training_view.equipment_enchanting_panel.all_scroll")
  t.scrollLevelDropdown.onValueChanged:AddListener(t.OnScrollLevelChanged)
  t.scrollLevelDropdown.options:Add(UnityEngine.UI.Dropdown.OptionData.New(LocalizationController.instance:Get("ui.equipment_training_view.equipment_enchanting_panel.all_scroll")))
  for i = 1, t.maxScrollLevel do
    local scrollLevelName = string.format(LocalizationController.instance:Get("ui.equipment_training_view.equipment_enchanting_panel.scroll_of_level"), i)
    t.scrollLevelDropdown.options:Add(UnityEngine.UI.Dropdown.OptionData.New(scrollLevelName))
  end
  
  t.scrollItems = {}
  t.scrollContentExpand = t.right:Find('scrollview/Viewport/Content'):GetComponent('ScrollContentExpand')
  t.scrollContentExpand:AddResetItemListener(t.OnResetItemHandler)
  t.RegenerateScrollLevelItems(true, 0)
  
  t.selectId = 0
  t.RefreshSelectedScrollInfo()
  
  t.BindDelegate()
end

function t.Close ()
  t.UnbindDelegate()
  if t.transform ~= nil then
    GameObject.Destroy(t.transform.gameObject)
    t.transform = nil
  end
end

function t.BindDelegate ()
  gamemanager.GetModel('equip_model').OnEquipEnchantSuccessDelegate:AddListener(t.OnEquipEnchantSuccessHandler)
  gamemanager.GetModel('equip_model').OnEnchantingScrollComposeSuccessDelegate:AddListener(t.OnEnchantingScrollComposeSuccess)
end

function t.UnbindDelegate ()
  gamemanager.GetModel('equip_model').OnEquipEnchantSuccessDelegate:RemoveListener(t.OnEquipEnchantSuccessHandler)
  gamemanager.GetModel('equip_model').OnEnchantingScrollComposeSuccessDelegate:RemoveListener(t.OnEnchantingScrollComposeSuccess)
end

function t.RefreshSelectedScrollInfo ()
  if t.selectId > 0 then
    local selectedScrollItemInfo = gamemanager.GetModel('item_model').GetScrollItemInfo (t.selectId)
    t.scrollIconImage.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(selectedScrollItemInfo:ItemIcon())
    
    local gem_attr_data = gamemanager.GetData('gem_attr_data')
    local attrData = gem_attr_data.GetDataById(selectedScrollItemInfo.itemData.id)
    
    t.scrollNameText.text = LuaInterface.LuaCsTransfer.LocalizationGet(selectedScrollItemInfo.itemData.name)
    local attrString = string.format('%s %s', attrData.equipAttr:GetName(), attrData.equipAttr:GetValueString())
    t.scrollAttrText.text = attrString
    t.bottomLeft.gameObject:SetActive(true)
  else
    t.bottomLeft.gameObject:SetActive(false)
  end
end

function t.RegenerateScrollLevelItems (playAnimation, delay)
  if (t.selectedScrollLevelIndex == 0) then
    t.selectedScrollLevelList = {}
    for i = 1, t.maxScrollLevel do
      table.insert(t.selectedScrollLevelList, i)
    end
  else
    t.selectedScrollLevelList = {t.selectedScrollLevelIndex}
  end
  t.scrollContentExpand:Init(#t.selectedScrollLevelList, playAnimation, delay)
end

function t.RefreshScrollLevelItems ()
  t.scrollContentExpand:RefreshAllContentItems()
end

-- ui event handler --
function t.ClickUse ()
	t.trainingView.equip_attr_panel:ShowShuXingEffect()
  gamemanager.GetCtrl('equip_controller').EquipEnchantReq(t.equipInfo.id, t.selectId)
end

function t.ClickMake ()
  local scrollMakingView = dofile('ui/equip/view/equip_training/enchanting/scroll_making_view')
  scrollMakingView:SetScrollItemId(t.selectId)
end

function t.OnScrollLevelChanged (index)
  t.selectedScrollLevelIndex = index
  t.RegenerateScrollLevelItems(true, 0)
end

function t.OnResetItemHandler (gameObject, index)
  if t.scrollItems[gameObject] == nil then
    t.scrollItems[gameObject] = scroll_level_item.New(gameObject, t.ClickScrollItemHandler)
  end
  local scrollLevelItem = t.scrollItems[gameObject]
  scrollLevelItem:SetScrollLevel(t.selectedScrollLevelList[index + 1])
end

function t.ClickScrollItemHandler (commonItemIcon)
  if commonItemIcon == nil then
    if t.selectCommonItemIcon ~= nil then
      t.selectCommonItemIcon:SetSelect(false)
      t.selectCommonItemIcon = nil
      t.selectId = 0
    end
  else
    if commonItemIcon == t.selectCommonItemIcon then
      commonItemIcon:SetSelect(false)
      t.selectCommonItemIcon = nil
    else
      if t.selectCommonItemIcon ~= nil then
        t.selectCommonItemIcon:SetSelect(false)
      end
      commonItemIcon:SetSelect(true)
      t.selectCommonItemIcon = commonItemIcon
    end
    
    if t.selectCommonItemIcon == nil then
      t.selectId = 0
    else 
      t.selectId = t.selectCommonItemIcon.itemInfo.itemData.id
    end
  end
  t.RefreshSelectedScrollInfo()
end
-- ui event handler --

-- proxy callback --
function t.OnEnchantingScrollComposeSuccess ()
  t.RefreshScrollLevelItems ()
end

function t.OnEquipEnchantSuccessHandler ()
  t.RefreshScrollLevelItems ()
  t.ClickScrollItemHandler(nil)
end
-- proxy callback --
return t
