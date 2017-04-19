local name = 'shop_model'
local shopCardRandomData = gamemanager.GetData('shop_card_random_data')
local shopDiamondData = gamemanager.GetData('shop_diamond_data')
local shopLimitItemData = gamemanager.GetData('shop_limit_item_data')
local shopGoodsData = gamemanager.GetData('shop_goods_data')

local shopHeroRandomCardInfo = require('ui/shop/model/info/shop_hero_random_card_info')
local shopArticleRandomCardInfo = require('ui/shop/model/info/shop_article_random_card_info')
local shopDiamondInfo = require('ui/shop/model/info/shop_diamond_info')
local shopActionItemInfo = require('ui/shop/model/info/shop_action_item_info')
local shopGoldItemInfo = require('ui/shop/model/info/shop_gold_item_info')
local shopGoodsItemInfo =require('ui/shop/model/info/shop_goods_item_info')

local t = {}

t.purchaseDrawCredit = 0

t.monthCardRemainDays = 0
t.extremeMonthCardRemainDays = 0

t.shopHeroRandomCardInfos = {}
t.shopArticleRandomCardInfos = {}
t.shopDiamondItemInfos = {}
t.shopActionItemInfos = {}
t.shopGoldItemInfos = {}
t.shopGoodsItemInfos = {}

t.OnShopHeroRandomCardInfoListUpdateDelegate = void_delegate.New()
t.OnShopArticleRandomCardInfoListUpdateDelegate = void_delegate.New()
t.OnShopDiamondItemInfoListUpdateDelegate = void_delegate.New()
t.OnShopDiamondFirstRechargeInfoUpdateDelegate = void_delegate.New()
t.OnMonthCardsInfoUpdateDelegate = void_delegate.New()
t.OnShopActionItemInfoListUpdateDelegate = void_delegate.New()
t.OnShopGoldItemInfoListUpdateDelegate = void_delegate.New()
t.OnShopGoodsItemInfoListUpdateDelegate = void_delegate.New()
t.OnFreeItemCountUpdateDelegate = void_delegate.New()
t.OnPurchaseDrawAwardSuccessDelegate = void_delegate.New()

local function Start ()
  t.LoadShopHeroRandomCardInfos ()
  t.LoadShopEquipmentRandomCardInfos ()
  t.LoadShopDiamondItemInfos ()
  t.LoadShopActionItemInfos ()
  t.LoadShopGoldItemInfos ()
  t.LoadShopGoodsItemInfos ()
  
  t.CheckFreeItemsCount()
  
  gamemanager.RegisterModel(name, t)
end

function t.SortShopHeroRandomCardInfos (aShopHeroRandomCardInfo, bShopHeroRandomCardInfo)
  return aShopHeroRandomCardInfo.shopCardRandomData.id < bShopHeroRandomCardInfo.shopCardRandomData.id
end

function t.SortShopArticleRandomCardInfos (aShopArticleRandomCardInfo, bShopArticleRandomCardInfo)
  return aShopArticleRandomCardInfo.shopCardRandomData.id < bShopArticleRandomCardInfo.shopCardRandomData.id
end

function t.SortShopDiamondItemInfos (aShopDiamondItemInfo, bShopDiamondItemInfo)
  return aShopDiamondItemInfo.shopDiamondData.sort < bShopDiamondItemInfo.shopDiamondData.sort
end

function t.SortShopActionItemInfos (aShopActionItemInfo, bShopActionItemInfo)
  return aShopActionItemInfo.shopLimitItemData.id < bShopActionItemInfo.shopLimitItemData.id
end

function t.SortShopGoldItemInfos (aShopGoldItemInfo, bShopGoldItemInfo)
  return aShopGoldItemInfo.shopLimitItemData.id < bShopGoldItemInfo.shopLimitItemData.id
end

function t.SortShopGoodsItemInfos (aShopGoodsItemInfo, bShopGoodsItemInfo)
  return aShopGoodsItemInfo.shopGoodItemData.id < bShopGoodsItemInfo.shopGoodItemData.id
end

-- [[ Load info method]] --
function t.LoadShopHeroRandomCardInfos ()
  for k, v in pairs(shopCardRandomData.GetHeroRandomItemDataList()) do
    t.shopHeroRandomCardInfos[k] = shopHeroRandomCardInfo.New(v)
  end
end

function t.LoadShopEquipmentRandomCardInfos ()
  for k, v in pairs(shopCardRandomData.GetArticleRandomDataList()) do
    t.shopArticleRandomCardInfos[k] = shopArticleRandomCardInfo.New(v)
  end
end

function t.LoadShopDiamondItemInfos ()  
  local game_model = gamemanager.GetModel('game_model')
  for k, v in pairs(shopDiamondData.data) do
    if game_model.channelId == PlatformType.iOS then--ios payment
      if(v.ios_is_show == true) then
        t.shopDiamondItemInfos[k] = shopDiamondInfo.New(v)
      end
    else
      t.shopDiamondItemInfos[k] = shopDiamondInfo.New(v)
    end
  end
end

function t.LoadShopActionItemInfos ()
  for k, v in pairs(shopLimitItemData.GetActionItemDataList()) do
    t.shopActionItemInfos[k] = shopActionItemInfo.New(v)
  end
end

function t.LoadShopGoldItemInfos ()
  for k, v in pairs(shopLimitItemData.GetGoldItemDataList()) do
    t.shopGoldItemInfos[k] = shopGoldItemInfo.New(v)
  end
end

function t.LoadShopGoodsItemInfos ()
  for k, v in pairs(shopGoodsData.data) do
    t.shopGoodsItemInfos[k] = shopGoodsItemInfo.New(v)
  end
end
-- [[ Load info method]] --

-- [[ Get enable shop item ]] --
function t.GetEnabledActionItemInfos ()
  local enabledActionItemInfos = {}
  for k, v in pairs(t.shopActionItemInfos) do
    if v.isOpen then
      enabledActionItemInfos[k] = v
    end
  end
  return enabledActionItemInfos
end

function t.GetEnabledGoldItemInfos ()
  local enabledGoldItemInfos = {}
  for k, v in pairs(t.shopGoldItemInfos) do
    if v.isOpen then
      enabledGoldItemInfos[k] = v
    end
  end
  return enabledGoldItemInfos
end
-- [[ Get enable shop item ]] --

-- [[ some other methods ]] --
function t.GetCurrentFreeItemsCount ()
  local currentFreeItemsCount = 0
  for k, v in pairs(t.shopHeroRandomCardInfos) do
    if v.remainFreeTimes > 0 and v:GetNextFreeBuyCountDownTime() <= 0 then
      currentFreeItemsCount = currentFreeItemsCount + 1
    end
  end
  return currentFreeItemsCount
end

function t.CheckFreeItemsCount()
  if t.currentFreeItemsCount == nil then
    t.currentFreeItemsCount = 0
  end
  
  currentFreeItemsCount = t.GetCurrentFreeItemsCount ()
  if t.currentFreeItemsCount ~= currentFreeItemsCount then
    t.currentFreeItemsCount = currentFreeItemsCount
    t.OnFreeItemCountUpdate()
  end
   LeanTween.delayedCall(1, Action(t.CheckFreeItemsCount))
end

function t.IsShopDiscountOpen ()
  return gamemanager.GetModel('activity_model'):IsShopDiscountOpen()
end
-- [[ some other methods ]] --
  
-- [[ Update UI method ]] --
function t.OnShopHeroRandomCardInfoListUpdate ()
  t.OnShopHeroRandomCardInfoListUpdateDelegate:Invoke()
  Observers.Facade.Instance:SendNotification('OnRedPointChange')
end

function t.OnShopArticleRandomCardInfoListUpdate ()  
  t.OnShopArticleRandomCardInfoListUpdateDelegate:Invoke()
end

function t.OnShopDiamondItemInfoListUpdate (firstChargeGoodsList)
  for k, v in ipairs(firstChargeGoodsList) do
    if t.shopDiamondItemInfos[v] ~= nil then
      t.shopDiamondItemInfos[v].isFirstRecharge = false
    end
  end
  t.OnShopDiamondItemInfoListUpdateDelegate:Invoke()
end

function t.OnShopDiamondFirstRechargeInfoUpdate ()
  t.OnShopDiamondFirstRechargeInfoUpdateDelegate:Invoke()
end

function t.OnMonthCardsInfoUpdate ()
  t.OnMonthCardsInfoUpdateDelegate:Invoke()
end

function t.OnShopActionItemInfoListUpdate()
  t.OnShopActionItemInfoListUpdateDelegate:Invoke()
end

function t.OnShopGoldItemInfoListUpdate ()
  t.OnShopGoldItemInfoListUpdateDelegate:Invoke()
end

function t.OnShopGoodsItemInfoListUpdate()  
  t.OnShopGoodsItemInfoListUpdateDelegate:Invoke()
end

function t.OnFreeItemCountUpdate ()
  t.OnFreeItemCountUpdateDelegate:Invoke()
  Observers.Facade.Instance:SendNotification('OnRedPointChange')
end

function t.OnPurchaseDrawAwardSuccess ()
  t.OnPurchaseDrawAwardSuccessDelegate:Invoke()
end
-- [[ Update UI method ]] --

Start ()
return t