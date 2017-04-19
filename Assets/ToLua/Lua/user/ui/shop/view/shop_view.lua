local t = {}
local PREFAB_PATH = 'ui/shop/shop_view'
local shopController = gamemanager.GetCtrl('shop_controller')
local shopModel = gamemanager.GetModel('shop_model')
local shopItem = require('ui/shop/view/shop_item')
local vipModel = gamemanager.GetModel('vip_model')
local vipData = gamemanager.GetData('vip_data')
local vipView = require('ui/vip/view/vip_view')

t.normalShopItems = nil

function t.Open (toggleIndex)
  --local v = uimanager.GetView(PREFAB_PATH)
  --if not v then
    local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Replace)
    t.transform = gameObject:GetComponent(typeof(Transform))
   
    local common_top_bar = require ('ui/common_top_bar/common_top_bar')
    common_top_bar = common_top_bar:Create(t.transform:Find('core'))
    --common_top_bar.transform:SetAsFirstSibling()
    common_top_bar:SetAsCommonStyle(LocalizationController.instance:Get('ui.shop_view.shop_view_title'), t.ClickCloseButtonHandler, true, true, true, true, false, false, false)
    
    t.BindDelegate()
    t.InitComponent()
    t.selectedNormalShopToggleIndex = 0
    shopController.DrawCardGoodsReq ()
    shopController.LimitGoodsReq ()
    shopController.OtherGoodsReq ()
    shopController.DiamondGoodsReq ()
    uimanager.RegisterView(PREFAB_PATH,t)
    gamemanager.GetModel('audio_model').PlayRandomAudioInView(AudioViewType.shopView,0,0.3)
  --end
  
  
  t.RefreshMyVIPInfo ()
  if toggleIndex == nil then
    toggleIndex = 1
  end
  t.ClickNormalShopToggleHandler (t.toggles[toggleIndex])

  if toggleIndex > 0 then
    t.SetToggleIndex(toggleIndex)
  end
end

function t.Close()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
  t.CloseNormalShopItems()
end

function t.BindDelegate()
  vipModel.OnVIPInfoUpdateDelegate:AddListener(t.OnVIPInfoUpdateHandler)
  
  shopModel.OnShopHeroRandomCardInfoListUpdateDelegate:AddListener(t.OnShopHeroRandomCardInfoListUpdateHandler)
  shopModel.OnShopArticleRandomCardInfoListUpdateDelegate:AddListener(t.OnShopArticleRandomCardInfoListUpdateHandler)
  shopModel.OnShopDiamondItemInfoListUpdateDelegate:AddListener(t.OnShopDiamondItemInfoListUpdateHandler)
  shopModel.OnMonthCardsInfoUpdateDelegate:AddListener(t.OnMonthCardsInfoUpdateHandler)
  shopModel.OnShopActionItemInfoListUpdateDelegate:AddListener(t.OnShopActionItemInfoListUpdateHandler)
  shopModel.OnShopGoldItemInfoListUpdateDelegate:AddListener(t.OnShopGoldItemInfoListUpdateHandler)
  shopModel.OnShopGoodsItemInfoListUpdateDelegate:AddListener(t.OnShopGoodsItemInfoListUpdateHandler)
  shopModel.OnPurchaseDrawAwardSuccessDelegate:AddListener(t.OnPurchaseDrawAwardSuccessHandler)
end

function t.UnbindDelegate()
  vipModel.OnVIPInfoUpdateDelegate:RemoveListener(t.OnVIPInfoUpdateHandler)
  
  shopModel.OnShopHeroRandomCardInfoListUpdateDelegate:RemoveListener(t.OnShopHeroRandomCardInfoListUpdateHandler)
  shopModel.OnShopArticleRandomCardInfoListUpdateDelegate:RemoveListener(t.OnShopArticleRandomCardInfoListUpdateHandler)
  shopModel.OnShopDiamondItemInfoListUpdateDelegate:RemoveListener(t.OnShopDiamondItemInfoListUpdateHandler)
    shopModel.OnMonthCardsInfoUpdateDelegate:RemoveListener(t.OnMonthCardsInfoUpdateHandler)
  shopModel.OnShopActionItemInfoListUpdateDelegate:RemoveListener(t.OnShopActionItemInfoListUpdateHandler)
  shopModel.OnShopGoldItemInfoListUpdateDelegate:RemoveListener(t.OnShopGoldItemInfoListUpdateHandler)
  shopModel.OnShopGoodsItemInfoListUpdateDelegate:RemoveListener(t.OnShopGoodsItemInfoListUpdateHandler)
  shopModel.OnPurchaseDrawAwardSuccessDelegate:RemoveListener(t.OnPurchaseDrawAwardSuccessHandler)
end

function t.InitComponent()
  t.core = t.transform:Find('core')
  t.titleBar = t.core:Find('img_frame/img_title_bar')
  
  t.accumulatedDrawButton = t.transform:Find('core/accumulated_draw/btn_accumulated_draw'):GetComponent(typeof(Button))
  t.accumulatedDrawButton.onClick:AddListener(t.ClickAccumulatedDrawButton) 
  t.accumulatedDrawSlider = t.transform:Find('core/accumulated_draw/slider'):GetComponent(typeof(Slider))
  t.accumulatedDrawPercentText = t.transform:Find('core/accumulated_draw/text_percent'):GetComponent(typeof(Text))
  t.accumulatedDrawSlider.value = 0
  t.accumulatedDrawPercentText.text = '0/'..gamemanager.GetData('global_data').accumulatedExpMax
  
  t.accumulatedDrawNotReadyBookFXGameObject = t.transform:Find('core/accumulated_draw/btn_accumulated_draw/ui_kapaiji').gameObject
  t.accumulatedDrawReadyBookFXGameObject = t.transform:Find('core/accumulated_draw/btn_accumulated_draw/ui_orangecard').gameObject
  t.accumulatedDrawReadyExpSliderFXGameObject = t.transform:Find('core/accumulated_draw/slider/ui_mancitishi').gameObject
  t.accumulatedDrawNotReadyBookFXGameObject:SetActive(false)
  t.accumulatedDrawReadyBookFXGameObject:SetActive(false)
  t.accumulatedDrawReadyExpSliderFXGameObject:SetActive(false)

  t.myVIPLevelText = t.titleBar:Find('img_vip_badge/text_vip_level'):GetComponent(typeof(Text))
  t.myVIPExpText = t.titleBar:Find('text_vip_exp'):GetComponent(typeof(Text))
  t.myVIPExpSlider = t.titleBar:Find('slider_vip_exp'):GetComponent(typeof(Slider))
  t.conditionToNextVIPLevelText = t.titleBar:Find('text_condition_to_next_vip_level'):GetComponent(typeof(Text))

  t.vipButton = t.titleBar:Find('btn_vip'):GetComponent(typeof(Button))
  t.vipButton.onClick:RemoveAllListeners()
  t.vipButton.onClick:AddListener(t.ClickVIPHandler)

  t.privilegeText = t.titleBar:Find('btn_vip/text'):GetComponent(typeof(Text))
  t.shopTypeText = t.titleBar:Find('btn_shop_type/text'):GetComponent(typeof(Text))

  t.normalShop = t.core:Find('img_frame/normal_shop')
  t.normalShopPanelsRoot = t.normalShop:Find('panels_root')
  t.normalScrollRect = t.normalShop:Find('scrollview'):GetComponent(typeof(ScrollRect))
  t.normalScrollRect.onValueChanged:AddListener(t.OnScrollRectValueChanged)
  t.normalScrollContent = t.normalShop:Find('scrollview/viewport/content'):GetComponent(typeof(Common.UI.Components.ScrollContent))
  t.normalScrollContent.onResetItem:RemoveAllListeners()
  t.normalScrollContent.onResetItem:AddListener(t.OnResetNormalShopItemHandler)
  
  t.shopItemRectTransform = t.normalShop:Find('scrollview/viewport/shop_item_view')
  t.leftArrowImage = t.normalShop:Find('img_left_arrow'):GetComponent(typeof(Image))
  t.rightArrowImage = t.normalShop:Find('img_right_arrow'):GetComponent(typeof(Image))

  t.toggles = {}
  t.toggles[1] = t.normalShop:Find('toggles/toggle_heroes'):GetComponent(typeof(Toggle))
  t.toggles[2] = t.normalShop:Find('toggles/toggle_articles'):GetComponent(typeof(Toggle))
  t.toggles[3] = t.normalShop:Find('toggles/toggle_gems'):GetComponent(typeof(Toggle))
  t.toggles[4] = t.normalShop:Find('toggles/toggle_action_point'):GetComponent(typeof(Toggle))
  t.toggles[5] = t.normalShop:Find('toggles/toggle_coin'):GetComponent(typeof(Toggle))
  t.toggles[6] = t.normalShop:Find('toggles/toggle_others'):GetComponent(typeof(Toggle))
  
  for k, v in pairs(t.toggles) do
    local toggleDelegate = v.transform:GetComponent(typeof(Common.UI.Components.EventTriggerDelegate))
    toggleDelegate.onClick:RemoveAllListeners()
    toggleDelegate.onClick:AddListener(t.ClickNormalShopToggleHandler)
  end
end

function t.SetToggleIndex(toggleIndex)
  for i = 1, 6 do
    t.toggles[i].isOn = (i == toggleIndex)
  end
end
function t.CloseNormalShopItems()
  if t.normalShopItems ~= nil then
    for k, v in pairs(t.normalShopItems) do
      v:Destroy()
    end
    t.normalShopItems = nil
  end
end
function t.RegenerateNormalShopItems ()
  t.CloseNormalShopItems()
  t.normalShopItems = nil
  t.normalShopItems = {}
  print('ClickNormalShopToggleHandler',t.selectedNormalShopToggleIndex)
  local selectedShopItemInfos = nil
  if t.selectedNormalShopToggleIndex == 1 then
    selectedShopItemInfos = shopModel.shopHeroRandomCardInfos
  elseif t.selectedNormalShopToggleIndex == 2 then
    selectedShopItemInfos = shopModel.shopArticleRandomCardInfos
  elseif t.selectedNormalShopToggleIndex == 3 then
    selectedShopItemInfos = shopModel.shopDiamondItemInfos
  elseif t.selectedNormalShopToggleIndex == 4 then
    selectedShopItemInfos = shopModel.GetEnabledActionItemInfos()
  elseif t.selectedNormalShopToggleIndex == 5 then
    selectedShopItemInfos = shopModel.GetEnabledGoldItemInfos()
  elseif t.selectedNormalShopToggleIndex == 6 then
    selectedShopItemInfos = shopModel.shopGoodsItemInfos
  end
  
  t.cachedShopItemInfoList = {}
  for k, v in pairs(selectedShopItemInfos) do
    if v.isOpen ~= false then
      table.insert(t.cachedShopItemInfoList, v)
    end
  end
  
  if t.selectedNormalShopToggleIndex == 1 then
    table.sort(t.cachedShopItemInfoList, shopModel.SortShopHeroRandomCardInfos)
  elseif t.selectedNormalShopToggleIndex == 2 then
    table.sort(t.cachedShopItemInfoList, shopModel.SortShopArticleRandomCardInfos)
  elseif t.selectedNormalShopToggleIndex == 3 then
    table.sort(t.cachedShopItemInfoList, shopModel.SortShopDiamondItemInfos)
  elseif t.selectedNormalShopToggleIndex == 4 then
    table.sort(t.cachedShopItemInfoList, shopModel.SortShopActionItemInfos)
  elseif t.selectedNormalShopToggleIndex == 5 then
    table.sort(t.cachedShopItemInfoList, shopModel.SortShopGoldItemInfos)
  elseif t.selectedNormalShopToggleIndex == 6 then
    table.sort(t.cachedShopItemInfoList, shopModel.SortShopGoodsItemInfos)
  end
  
  t.normalScrollContent:Init(#t.cachedShopItemInfoList, false, 0)
  
  t.leftArrowImage.gameObject:SetActive(false)
  t.rightArrowImage.gameObject:SetActive(false)
  t.OnScrollRectValueChanged(Vector2(0, 0))
end

function t.RefreshAllNormalShopItems ()
  t.normalScrollContent:RefreshAllContentItems()
end

function t.RefreshMyVIPInfo ()
  local currentVIPData = vipData.GetVIPData (vipModel.vipLevel)
  t.myVIPLevelText.text = vipModel.vipLevel
  
  if currentVIPData ~= nil then
    if not currentVIPData:IsMaxLevel() then
      local nextVipData = currentVIPData:GetNextLevelVIPData()
      t.myVIPExpText.text = string.format("%s/%s", vipModel.totalRecharge * 10, nextVipData.exp * 10) 
      t.myVIPExpSlider.value = vipModel.totalRecharge / nextVipData.exp
      t.myVIPExpSlider.gameObject:SetActive(true)
      local conditionToNextVIPLevelString = LocalizationController.instance:Get('ui.shop_view.condition_to_next_vip_level')
      conditionToNextVIPLevelString =  string.format(conditionToNextVIPLevelString, (nextVipData.exp - vipModel.totalRecharge) * 10, nextVipData:ShortName())
      t.conditionToNextVIPLevelText.text = conditionToNextVIPLevelString
      t.conditionToNextVIPLevelText.gameObject:SetActive(true)
    else
      t.myVIPExpText.text = vipModel.totalRecharge * 10
      t.myVIPExpSlider.gameObject:SetActive(false)
      t.conditionToNextVIPLevelText.gameObject:SetActive(false)
    end
  end
end

function t.RefreshAccumulatedDraw ()
  t.accumulatedDrawSlider.value = shopModel.purchaseDrawCredit / gamemanager.GetData('global_data').accumulatedExpMax
  t.accumulatedDrawPercentText.text = shopModel.purchaseDrawCredit..'/'..gamemanager.GetData('global_data').accumulatedExpMax
  
  local canDraw = shopModel.purchaseDrawCredit >= gamemanager.GetData('global_data').accumulatedExpMax
  
  t.accumulatedDrawNotReadyBookFXGameObject:SetActive(not canDraw)
  t.accumulatedDrawReadyBookFXGameObject:SetActive(canDraw)
  t.accumulatedDrawReadyExpSliderFXGameObject:SetActive(canDraw)
end

-- [[ Proxy callback ]] --
function t.OnVIPInfoUpdateHandler ()
  t.RefreshMyVIPInfo ()
end

function t.OnShopHeroRandomCardInfoListUpdateHandler ()
  --[[
  if t.selectedNormalShopToggleIndex == 1 then
    t.RegenerateNormalShopItems ()
  end
  ]]--
  t.RefreshAllNormalShopItems ()
  t.RefreshAccumulatedDraw ()
end

function t.OnShopArticleRandomCardInfoListUpdateHandler ()
  --[[
  if t.selectedNormalShopToggleIndex == 2 then
    t.RegenerateNormalShopItems ()
  end
  ]]--
  t.RefreshAllNormalShopItems ()
end

function t.OnShopDiamondItemInfoListUpdateHandler ()
  if t.selectedNormalShopToggleIndex == 3 then
    t.RegenerateNormalShopItems ()
  end
end

function t.OnMonthCardsInfoUpdateHandler ()
  if t.selectedNormalShopToggleIndex == 3 then
    t.RefreshAllNormalShopItems()
  end
end

function t.OnShopActionItemInfoListUpdateHandler ()
  if t.selectedNormalShopToggleIndex == 4 then
    t.RegenerateNormalShopItems ()
  end
end

function t.OnShopGoldItemInfoListUpdateHandler ()
  if t.selectedNormalShopToggleIndex == 5 then
    t.RegenerateNormalShopItems ()
  end
end

function t.OnShopGoodsItemInfoListUpdateHandler ()
  if t.selectedNormalShopToggleIndex == 6 then
    t.RegenerateNormalShopItems ()
  end
end

function t.OnPurchaseDrawAwardSuccessHandler ()
  t.RefreshAccumulatedDraw ()
end
-- [[ Proxy callback ]] --

-- [[ UI event handlers ]] --
function t.OnScrollRectValueChanged (pos)
  --[[
  if pos.x < 0 or pos.x > 1 then
    return
  end
  ]]--
  
  local viewportWidth = t.normalScrollRect:GetComponent(typeof(RectTransform)).rect.size.x
  local contentRectTransform = t.normalScrollContent:GetComponent(typeof(RectTransform))
  local contentX = contentRectTransform.anchoredPosition.x
  local contentWidth = contentRectTransform.rect.size.x
  t.leftArrowImage.gameObject:SetActive(contentX <= -t.shopItemRectTransform.rect.size.x)
  t.rightArrowImage.gameObject:SetActive(contentX + contentWidth >= viewportWidth + t.shopItemRectTransform.rect.size.x)
end

function t.ClickAccumulatedDrawButton ()
  if shopModel.purchaseDrawCredit < gamemanager.GetData('global_data').accumulatedExpMax then
    local auto_destroy_tips_view = require('ui/tips/view/auto_destroy_tip_view')
    auto_destroy_tips_view.Open(LocalizationController.instance:Get('ui.shop_view.purchase_draw_credit_not_enough'))
    return
  end
  
  shopController.PurchaseDrawAwardReq()
end

function t.ClickVIPHandler ()
  vipView.Open()
end

function t.OnResetNormalShopItemHandler (gameObject, index)
  local item = t.normalShopItems[gameObject]
  if item == nil then
    item = shopItem.NewByGameObject(gameObject)
    t.normalShopItems[gameObject] = item
  end
  
  local itemInfo = t.cachedShopItemInfoList[index + 1]
  if t.selectedNormalShopToggleIndex == 1 then
    item:SetInfo(ShopItemType.HeroDrawCard, itemInfo)
  elseif t.selectedNormalShopToggleIndex == 2 then
    item:SetInfo(ShopItemType.ArticleDrawCard, itemInfo)
  elseif t.selectedNormalShopToggleIndex == 3 then
    item:SetInfo(ShopItemType.Diamond, itemInfo)
  elseif t.selectedNormalShopToggleIndex == 4 then
    item:SetInfo(ShopItemType.Action, itemInfo)
  elseif t.selectedNormalShopToggleIndex == 5 then
    item:SetInfo(ShopItemType.Gold, itemInfo)
  elseif t.selectedNormalShopToggleIndex == 6 then
    item:SetInfo(ShopItemType.Goods, itemInfo)
  end
end

function t.ClickNormalShopToggleHandler (toggle)
  print('toggle name: '..toggle.gameObject.name)
  for k, v in pairs(t.toggles) do
    if toggle.gameObject == v.gameObject then
      if t.selectedNormalShopToggleIndex ~= k then
        t.selectedNormalShopToggleIndex = k
        t.RegenerateNormalShopItems ()
        
      end
    end
  end
end

function t.ClickCloseButtonHandler ()
  t.Close()
end
-- [[ UI event handlers ]] --
return t