local t = {}
local uiUtil = require 'util/ui_util'
local commonItemIcon = require 'ui/common_icon/common_item_icon'
local PREFAB_PATH = 'ui/equipments/enchanting/scroll_making_view'

function Start ()
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  t.coreTransform = t.transform:Find('core')
  
  t.scrollNameText = t.coreTransform:Find('text_scroll_name'):GetComponent(typeof(Text))
  t.scrollIconImage = t.coreTransform:Find('img_scroll_icon'):GetComponent(typeof(Image))
  t.scrollCountText = t.coreTransform:Find('text_scroll_count'):GetComponent(typeof(Text))
  t.scrollAttrText = t.coreTransform:Find('text_scroll_attr'):GetComponent(typeof(Text))
  
  t.materialItemRootTransforms = {}
  t.materialItemRootTransforms[1] = t.coreTransform:Find('mat1_frame/root')
  t.materialItemRootTransforms[2] = t.coreTransform:Find('mat2_frame/root')
  t.materialItemRootTransforms[3] = t.coreTransform:Find('mat3_frame/root')
  t.materialItemRootTransforms[4] = t.coreTransform:Find('mat4_frame/root')
  t.materialItemRootTransforms[5] = t.coreTransform:Find('mat5_frame/root')
  t.materialItemRootTransforms[6] = t.coreTransform:Find('mat6_frame/root')
  
  --[[
  t.materialItemImages = {}
  for i = 1, 6 do
    t.materialItemImages[i] = t.materialItemRootTransforms[i]:Find('img_item'):GetComponent(typeof(Image))
  end
  ]]--
  
  t.materialItemIcons = {}
  for i = 1, 6 do
    t.materialItemIcons[i] = commonItemIcon.New(t.materialItemRootTransforms[i])
    t.materialItemIcons[i]:ShowCount(false)
    t.materialItemIcons[i].onClick:AddListener(t.ClickItemIcon)
    t.materialItemIcons[i].transform:SetSiblingIndex(0)
  end
  
  t.materialCountTexts = {}
  for i = 1, 6 do
    t.materialCountTexts[i] = t.materialItemRootTransforms[i]:Find('text_count'):GetComponent(typeof(Text))
    t.materialCountTexts[i].transform:SetSiblingIndex(1)
  end
  
  t.closeButton = t.transform:Find('core/btn_close'):GetComponent(typeof(Button))
  t.closeButton.onClick:AddListener(t.ClickClose)
  
  t.makeButton = t.transform:Find('core/btn_make'):GetComponent(typeof(Button))
  t.makeButton.onClick:AddListener(t.ClickMake)
  
  t.BindDelegate ()
end

function t.Close ()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate ()
end

function t.BindDelegate ()
  gamemanager.GetModel('equip_model').OnEnchantingScrollComposeSuccessDelegate:AddListener(t.OnEnchantingScrollComposeSuccessHandler)
end

function t.UnbindDelegate ()
    gamemanager.GetModel('equip_model').OnEnchantingScrollComposeSuccessDelegate:RemoveListener(t.OnEnchantingScrollComposeSuccessHandler)
end

function t:SetScrollItemId (scrollItemID)
  gamemanager.GetModel('item_model')
  
  t.scrollItemID = scrollItemID
  local scrollItemInfo = gamemanager.GetModel('item_model').GetScrollItemInfo (scrollItemID)
    
  local gem_attr_data = gamemanager.GetData('gem_attr_data')
  local attrData = gem_attr_data.GetDataById(scrollItemInfo.itemData.id)
    
    
  local nameString = LuaInterface.LuaCsTransfer.LocalizationGet(scrollItemInfo.itemData.name)
  local levelString = 'Lv'..scrollItemInfo.itemData.lv
  local attrString = string.format('%s %s', attrData.equipAttr:GetName(), attrData.equipAttr:GetValueString())
  
  t.scrollNameText.text = string.format('%s %s', nameString, levelString)
  t.scrollIconImage.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(scrollItemInfo:ItemIcon())
  t.scrollCountText.text = scrollItemInfo.count
  t.scrollAttrText.text = attrString
  
  t.RefreshMaterials()
end

function t.RefreshMaterials()
  local gem_synthesis_data = gamemanager.GetData('gem_synthesis_data')
  local gemSynthesisData = gem_synthesis_data.GetDataById(t.scrollItemID)
  --local materials = gemSynthesisData.Material
  
  local materials = {}
  for k, v in pairs(gemSynthesisData.Material) do
    table.insert(materials, v)
  end
  
  if gemSynthesisData.money ~= nil then
    table.insert(materials, gemSynthesisData.money)
  end
  
  for i = 1, 6 do
    if materials[i] ~= nil then
      if materials[i].type == BaseResType.Item then
        local materialItemInfo = gamemanager.GetModel('item_model').GetItemInfoWithoutNilByItemId(materials[i].id)
        --t.materialItemImages[i].sprite = Common.ResMgr.ResMgr.instance:LoadSprite(materialItemInfo:ItemIcon())
        t.materialItemIcons[i]:SetItemInfo(materialItemInfo)
        t.materialItemIcons[i]:ShowCount(false)
        
        local itemCountString = string.format('%s/%s', materialItemInfo.count,  materials[i].count)
        if materialItemInfo.count >= materials[i].count then
          itemCountString =uiUtil.FormatToGreenText(itemCountString)
        else
          itemCountString = uiUtil.FormatToRedText(itemCountString)
        end
        t.materialCountTexts[i].text = itemCountString
      elseif materials[i].type == BaseResType.Gold then
        local goldItemData = gamemanager.GetData('item_data').GetBasicResItemByType(BaseResType.Gold)
        --t.materialItemImages[i].sprite = Common.ResMgr.ResMgr.instance:LoadSprite(goldItemData:IconPath())
        t.materialItemIcons[i]:SetItemDataId(goldItemData.id)
        
        local ownGold = gamemanager.GetModel('game_model').GetBaseResourceValue(BaseResType.Gold)
        local itemCountString = string.format('%s/%s', ownGold, materials[i].count)
        if ownGold >= materials[i].count then
          itemCountString =uiUtil.FormatToGreenText(itemCountString)
        else
          itemCountString = uiUtil.FormatToRedText(itemCountString)
        end
        t.materialCountTexts[i].text = itemCountString 
      end
      t.materialItemRootTransforms[i].gameObject:SetActive(true)
    else
      t.materialItemRootTransforms[i].gameObject:SetActive(false)
    end
  end
end

function t.ClickItemIcon (itemIcon)
  local itemInfo = itemIcon.itemInfo
  LuaInterface.LuaCsTransfer.OpenGoodsJumpPath(BaseResType.Item, itemInfo.itemData.id, 0)
end

-- ui event handlers --
function t.ClickClose ()
  t.Close()
end

function t.ClickMake ()
  print('====== clicked make button ======')
  gamemanager.GetCtrl('equip_controller').EnchantingScrollComposeReq(t.scrollItemID)
end
-- ui event handlers --

-- proxy callback --
function t.OnEnchantingScrollComposeSuccessHandler ()
  t:SetScrollItemId (t.scrollItemID)
end
-- proxy callback --

Start ()
return t