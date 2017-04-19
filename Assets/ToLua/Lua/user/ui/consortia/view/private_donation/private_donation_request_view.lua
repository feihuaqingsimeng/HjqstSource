local t = {}
local PREFAB_PATH = 'ui/consortia/private_donation/private_donation_request_view'

local item_model = gamemanager.GetModel('item_model')
local illustration_model = gamemanager.GetModel('illustration_model')

local common_item_icon = require('ui/common_icon/common_item_icon')

function t.Open ()
  local gameObject = UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay)
  t.transform = gameObject.transform
  
  t.InitComponent ()
  t.BindDelegate ()
end

function t.InitComponent ()
  t.closeButton = t.transform:Find('core/img_frame/img_title_frame/btn_close'):GetComponent(typeof(Button))
  t.closeButton.onClick:AddListener(t.ClickCloseButtonHandler)
  
  t.scrollContent = t.transform:Find('core/img_frame/img_frame/scrollview/viewport/content'):GetComponent(typeof(ScrollContent))
  t.scrollContent.onResetItem:AddListener(t.OnResetItem)
  
  t.confirmButton = t.transform:Find('core/img_frame/btn_confirm'):GetComponent(typeof(Button))
  t.confirmButton.onClick:AddListener(t.ClickConfirmButtonHandler)
  
  t.RegenerateHeroes ()
end

function t.BindDelegate ()
end

function t.UnbindDelegate ()
end

function t.RegenerateHeroes ()
  t.itemIcons = {}
  
  local shouldShowHeroPieceItemIDList = {}
  local heroPieceDataDic = gamemanager.GetData('piece_data').GetAllHeroPieceDataDic ()
  for k, v in pairs(heroPieceDataDic) do
    if illustration_model.IsHeroGotInIllustration(v:GetHeroGameResData().id)
      or item_model.GetItemCountByItemID(v.id) > 0 then
      table.insert(shouldShowHeroPieceItemIDList, k)
    end
  end
  
  t.heroPieceItemInfoList = item_model.GetItemInfoListByIDList (shouldShowHeroPieceItemIDList)
  table.sort(t.heroPieceItemInfoList, item_model.SortItemInfoByQuality)
  t.scrollContent:Init(table.count(t.heroPieceItemInfoList), true, 0)
end

function t.OnResetItem (gameObject, index)
  local itemIcon = t.itemIcons[gameObject]
  local itemInfo = t.heroPieceItemInfoList[index + 1]
  if itemIcon == nil then
    itemIcon = common_item_icon.NewByGameObject(gameObject)
    itemIcon.onClick:AddListener(t.ClickItemHandler)
    t.itemIcons[gameObject] = itemIcon
  end
  itemIcon:SetItemInfo(itemInfo)
  --itemIcon:ShowNewMark(itemInfo.isNew)
  itemIcon:ShowNewMark(false)
  itemIcon.index = index + 1
  itemIcon:SetSelect(t.selectedItemIndex == itemIcon.index)
end

function t.Close ()
  t.UnbindDelegate ()
  UIMgr.instance:Close(PREFAB_PATH)
end

function t.ClickCloseButtonHandler ()
  t.Close()
end

function t.ClickConfirmButtonHandler ()
  if t.heroPieceItemInfoList[t.selectedItemIndex] == nil then
    local auto_destroy_tips_view = require('ui/tips/view/auto_destroy_tip_view')
    auto_destroy_tips_view.Open(LocalizationController.instance:Get('ui.private_donation_request_view.please_select_request_hero_tips'))
    return
  end
  
  local selectedheroPieceItemData = t.heroPieceItemInfoList[t.selectedItemIndex].itemData
  local private_donation_request_confirm_view = require 'ui/consortia/view/private_donation/private_donation_request_confirm_view'
  private_donation_request_confirm_view.Open(selectedheroPieceItemData)
end

function t.ClickItemHandler (itemIcon)
  t.selectedItemIndex = itemIcon.index
  t.scrollContent:RefreshAllContentItems()
end

return t