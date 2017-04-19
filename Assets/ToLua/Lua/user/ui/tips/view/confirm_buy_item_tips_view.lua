local t = {}
local PREFAB_PATH = 'ui/tips/confirm_buy_item_tips_view'

local uiUtil = require ('util/ui_util')
local gameResData = require('ui/game/model/game_res_data')

function t.Open (shopItemInfo, consumeTipType)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.Tips, UIOpenMode.Overlay)
  t.transform = gameObject.transform
  
  t.InitComponent()
  t.SetShopItemInfo(shopItemInfo, consumeTipType)
end

function t.InitComponent ()
  t.coreTransform = t.transform:Find('core')
  t.tipsText = t.coreTransform:Find('img_frame/text_tips'):GetComponent(typeof(Text))
  t.costResourceIconImage = t.coreTransform:Find('img_frame/img_cost_resource_icon'):GetComponent(typeof(Image))
  t.costResourceCountText = t.coreTransform:Find('img_frame/text_cost_resource_count'):GetComponent(typeof(Text))
  
  t.cancelButton = t.coreTransform:Find('img_frame/btn_cancel'):GetComponent(typeof(Button))
  t.cancelButton.onClick:AddListener(t.ClickCancelButtonHandler)
  t.buyButton = t.coreTransform:Find('img_frame/btn_buy'):GetComponent(typeof(Button))
  t.buyButton.onClick:AddListener(t.ClickBuyButtonHandler)
end

function t.SetShopItemInfo (shopItemInfo, consumeTipType)
  t.shopItemInfo = shopItemInfo
  
  local shopItemType = t.shopItemInfo:GetShopItemType()
  local shopItemID = t.shopItemInfo:GetShopItemID()
  local shopItemName = t.shopItemInfo:GetShopItemName ()
  local costGameResData = t.shopItemInfo:GetCostResData ()
  local costType = 1
  
  if shopItemType == ShopItemType.HeroDrawCard then
    costType = 1
  elseif shopItemType == ShopItemType.ArticleDrawCard then
    costType = 1
  elseif shopItemType == ShopItemType.Diamond then
    costType = 1
  elseif shopItemType == ShopItemType.Action then
    costType = 1
  elseif shopItemType == ShopItemType.Gold then
    costType = 1
  elseif shopItemType == ShopItemType.Goods then
    costType = 1
  end
  
  t.tipsText.text = string.format(LocalizationController.instance:Get("ui.confirm_buy_item_tips_view.tips"), shopItemName)
  t.costResourceIconImage.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(uiUtil.GetBaseResIconPath (costGameResData.type))
  t.costResourceIconImage:SetNativeSize()
  t.costResourceCountText.text = costGameResData.count
end

-- [[ UI event handlers ]] --
function t.ClickCancelButtonHandler ()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end

function t.ClickBuyButtonHandler ()
  gamemanager.GetCtrl('shop_controller').PurchaseGoods(t.shopItemInfo)
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end
-- [[ UI event handlers ]] --

return t