local t = {}
t.__index = t
local uiUtil = require('util/ui_util')
local gameResData = require('ui/game/model/game_res_data')
local event_data = gamemanager.GetData('event_data')
local shopController = gamemanager.GetCtrl('shop_controller')
local shop_model = gamemanager.GetModel('shop_model')

local nornalItemBGSpritePath = 'sprite/main_ui/ui_equip_shop_6'
local monthCardItemBGSpritePath = 'sprite/main_ui/ui_equip_shop_7'
local extremeMonthCardItemBGSpritePath = 'sprite/main_ui/ui_equip_shop_8'


function t.NewByGameObject (gameObject)
  local o = {}
  setmetatable(o, t)
  
  o:InitComponent(gameObject)
  return o
end

function t:InitComponent (gameObject)
  self.clickedBuy = false
  self.transform = gameObject:GetComponent(typeof(Transform))
  
  self.bgImage = self.transform:Find('img_bg'):GetComponent(typeof(Image))
  self.nameText = self.transform:Find('img_top_bar/text_name'):GetComponent(typeof(Text))
  self.descriptionText = self.transform:Find('text_desc'):GetComponent(typeof(Text))
  self.itemIconImage = self.transform:Find('img_icon'):GetComponent(typeof(Image))
  self.freeCountDownSlider = self.transform:Find('slider'):GetComponent(typeof(Slider))
  self.freeCountDownText = self.transform:Find('slider/text_free_count_down'):GetComponent(typeof(Text))

  self.freeTimesRoot = self.transform:Find('free_times_root')
  self.freeTimesText = self.freeTimesRoot:Find('text_free_times'):GetComponent(typeof(Text))
  
  self.limitTimesRoot = self.transform:Find('limit_times_root')
  self.limitTimesText = self.limitTimesRoot:Find('text_limit_times'):GetComponent(typeof(Text))

  self.buyButton = self.transform:Find('img_bottom_bar/btn_buy'):GetComponent(typeof(Button))
  self.buyButton.onClick:AddListener(function ()
      self:ClickBuyButtonHandler()
      end)
  
  self.costResourceIcon = self.buyButton.transform:Find('img_resource_icon'):GetComponent(typeof(Image))
  self.costResourceCountText = self.buyButton.transform:Find('text_cost'):GetComponent(typeof(Text))
  self.soldOutButton = self.transform:Find('img_bottom_bar/btn_sold_out'):GetComponent(typeof(Button))
  self.soldOutText = self.transform:Find('img_bottom_bar/btn_sold_out/text_sold_out'):GetComponent(typeof(Text))
  self.freeParticleRoot = self.transform:Find('img_bottom_bar/free_particle_root')
  self.freeParticleRoot.gameObject:SetActive(false)
  
  self.first_recharge_double_root = self.transform:Find('img_first_recharge_double')
  self.discount_info_root = self.transform:Find('img_discount_info_root')
  self.discount_des_text = self.discount_info_root:Find('text_des'):GetComponent(typeof(Text))
  self.discount_remaining_time_text = self.discount_info_root:Find('text_time'):GetComponent(typeof(Text))
  self.extra_gain_text = self.discount_info_root:Find('text_extra_gain'):GetComponent(typeof(Text))
  self.thirtyDayTipGameObject = self.transform:Find('thirty_day_tip').gameObject
  
  self.first_recharge_double_root.gameObject:SetActive(false)
  self.discount_info_root.gameObject:SetActive(false)
  self.thirtyDayTipGameObject:SetActive(false)
  
end

function t:Destroy ()
  LeanTween.cancel(self.transform.gameObject)
end

function t:Refresh ()
  if self.shopItemType == ShopItemType.HeroDrawCard then
    self:RefreshAsHeroDrawCardItem ()
  elseif self.shopItemType == ShopItemType.ArticleDrawCard then
    self:RefreshAsArticleDrawCardItem ()
  elseif self.shopItemType == ShopItemType.Diamond then
    self:RefreshAsDiamondItem ()
  elseif self.shopItemType == ShopItemType.Action then
    self:RefreshAsActionItem ()
  elseif self.shopItemType == ShopItemType.Gold then
    self:RefreshAsGoldItem ()
  elseif self.shopItemType == ShopItemType.Goods then
    self:RefreshAsGoodsItem ()
  end
end

function t:SetInfo (shopItemType, shopItemInfo)
  self.shopItemType = shopItemType
  self.shopItemInfo = shopItemInfo
  self:Refresh()
end

function t:RefreshAsHeroDrawCardItem ()
  self.bgImage.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/'..self.shopItemInfo.shopCardRandomData.bg_pic)
  if self.shopItemInfo ~= nil then
    self.nameText.text = LocalizationController.instance:Get(self.shopItemInfo.shopCardRandomData.name)
    self.descriptionText.text = LocalizationController.instance:Get(self.shopItemInfo.shopCardRandomData.des)
    self.itemIconImage.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(res_path.GetShopItemIconPath(self.shopItemInfo.shopCardRandomData.pic))
    self.itemIconImage:SetNativeSize()
    self.freeTimesRoot.gameObject:SetActive(false)
    self.freeTimesText = ''
    self.limitTimesRoot.gameObject:SetActive(false)
    self.limitTimesText.text = ''
    self.buyButton.gameObject:SetActive(true)
    self.soldOutButton.gameObject:SetActive(false)
    self:CheckCost()
    
    self.first_recharge_double_root.gameObject:SetActive(false)
	self.discount_des_text.gameObject:SetActive(true)
	self.discount_remaining_time_text .gameObject:SetActive(true)
	self.extra_gain_text.gameObject:SetActive(false)
    
    if gamemanager.GetModel('shop_model').IsShopDiscountOpen() and self.shopItemInfo.shopCardRandomData.discount < 1 then
      self.discount_des_text.text = string.format(LocalizationController.instance:Get('ui.shop_view.normal_shop.discount_description'), self.shopItemInfo.shopCardRandomData.discount * 10)
      local discountEventData = event_data.GetDataByType(130)
      local dateTime = System.DateTime.Parse(discountEventData.event_timeover)
      local serverDateTime = TimeController.instance.ServerTime
      local remianTimeSpan = dateTime - serverDateTime
      self.discount_remaining_time_text.text = string.format(LocalizationController.instance:Get('ui.shop_view.normal_shop.discount_remaining_time'), remianTimeSpan.Days)
      self.discount_info_root.gameObject:SetActive(true)
    else
      self.discount_info_root.gameObject:SetActive(false)
    end
  end
end

function t:RefreshAsArticleDrawCardItem ()
  self.bgImage.sprite = ResMgr.instance:LoadSprite(nornalItemBGSpritePath)
  if self.shopItemInfo ~= nil then
    self.nameText.text = LocalizationController.instance:Get(self.shopItemInfo.shopCardRandomData.name)
    self.descriptionText.text = LocalizationController.instance:Get(self.shopItemInfo.shopCardRandomData.des)
    self.itemIconImage.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(res_path.GetShopItemIconPath(self.shopItemInfo.shopCardRandomData.pic))
    self.itemIconImage:SetNativeSize()
    self.freeTimesRoot.gameObject:SetActive(false)
    self.freeTimesText = ''
    self.limitTimesRoot.gameObject:SetActive(false)
    self.limitTimesText.text = ''
    self.buyButton.gameObject:SetActive(true)
    self.soldOutButton.gameObject:SetActive(false)
    self:CheckCost()
    
    self.first_recharge_double_root.gameObject:SetActive(false)
	self.discount_des_text.gameObject:SetActive(true)
	self.discount_remaining_time_text .gameObject:SetActive(true)
	self.extra_gain_text.gameObject:SetActive(false)
    
    if gamemanager.GetModel('shop_model').IsShopDiscountOpen() and self.shopItemInfo.shopCardRandomData.discount < 1 then
      self.discount_des_text.text = string.format(LocalizationController.instance:Get('ui.shop_view.normal_shop.discount_description'), self.shopItemInfo.shopCardRandomData.discount * 10)
      local discountEventData = event_data.GetDataByType(130)
      local dateTime = System.DateTime.Parse(discountEventData.event_timeover)
      local serverDateTime = TimeController.instance.ServerTime
      local remianTimeSpan = dateTime - serverDateTime
      self.discount_remaining_time_text.text = string.format(LocalizationController.instance:Get('ui.shop_view.normal_shop.discount_remaining_time'), remianTimeSpan.Days)
      self.discount_info_root.gameObject:SetActive(true)
    else
      self.discount_info_root.gameObject:SetActive(false)
    end
  end
end

function t:RefreshAsDiamondItem ()
  if self.shopItemInfo ~= nil then
    self.nameText.text = LocalizationController.instance:Get(self.shopItemInfo.shopDiamondData.name)
    self.descriptionText.text = LocalizationController.instance:Get(self.shopItemInfo.shopDiamondData.des)
    self.itemIconImage.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(res_path.GetShopItemIconPath(self.shopItemInfo.shopDiamondData.pic))
    self.itemIconImage:SetNativeSize()

    self.freeTimesRoot.gameObject:SetActive(false)
    self.limitTimesRoot.gameObject:SetActive(false)
    self.freeCountDownSlider.gameObject:SetActive(false)

    local costGameResData = gameResData.NewByString(self.shopItemInfo.shopDiamondData.cost)
    self.costResourceIcon.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(uiUtil.GetBaseResIconPath(costGameResData.type))
    self.costResourceIcon:SetNativeSize()
    self.costResourceCountText.text = tostring(costGameResData.count)
    
    self.buyButton.gameObject:SetActive(true)
    self.soldOutText.text = LocalizationController.instance:Get('ui.shop_view.sold_out')
    self.soldOutButton.gameObject:SetActive(false)
    
    self.first_recharge_double_root.gameObject:SetActive(self.shopItemInfo.isFirstRecharge)
    self.discount_des_text.gameObject:SetActive(false)
    self.discount_remaining_time_text .gameObject:SetActive(false)
    self.extra_gain_text.gameObject:SetActive(true)
    
    if self.shopItemInfo.shopDiamondData.mounth_card then
      self.first_recharge_double_root.gameObject:SetActive(false)
      local gainDiamondGameResData = gameResData.NewByString(self.shopItemInfo.shopDiamondData.resource)
      self.extra_gain_text.text = string.format(LocalizationController.instance:Get( 'ui.shop_view.normal_shop.buy_gain'), gainDiamondGameResData.count)
      self.discount_info_root.gameObject:SetActive(true)
      
      if self.shopItemInfo.shopDiamondData.id == 105 then
        self.bgImage.sprite = ResMgr.instance:LoadSprite(monthCardItemBGSpritePath)
        self.thirtyDayTipGameObject:SetActive(true)
      elseif self.shopItemInfo.shopDiamondData.id == 106 then
        self.bgImage.sprite = ResMgr.instance:LoadSprite(extremeMonthCardItemBGSpritePath)
        self.thirtyDayTipGameObject:SetActive(true)
      else
        self.bgImage.sprite = ResMgr.instance:LoadSprite(nornalItemBGSpritePath)
        self.thirtyDayTipGameObject:SetActive(false)
      end

      if self.shopItemInfo.shopDiamondData.id == 105 and shop_model.monthCardRemainDays > 0 then
        self.buyButton.gameObject:SetActive(false)
        self.soldOutText.text = string.format(LocalizationController.instance:Get('ui.shop_view.remain_days'), shop_model.monthCardRemainDays)
        self.soldOutButton.gameObject:SetActive(true)
      elseif self.shopItemInfo.shopDiamondData.id == 106 and shop_model.extremeMonthCardRemainDays > 0 then
        self.buyButton.gameObject:SetActive(false)
        self.soldOutText.text = string.format(LocalizationController.instance:Get('ui.shop_view.remain_days'), shop_model.extremeMonthCardRemainDays)
        self.soldOutButton.gameObject:SetActive(true)
      end 
    else
      if self.shopItemInfo.isFirstRecharge == true then
        self.extra_gain_text.text = string.format(LocalizationController.instance:Get('ui.shop_view.normal_shop.first_recharge_extra_gain'), self.shopItemInfo.shopDiamondData.first_award.count)
        self.discount_info_root.gameObject:SetActive(self.shopItemInfo.shopDiamondData.first_award.count > 0)
      else
        self.extra_gain_text.text = string.format(LocalizationController.instance:Get('ui.shop_view.normal_shop.buy_extra_gain'), self.shopItemInfo.shopDiamondData.buy_award.count)
        self.discount_info_root.gameObject:SetActive(self.shopItemInfo.shopDiamondData.buy_award.count > 0)
      end
    end
    self:CheckCost()
  end
end

function t:RefreshAsActionItem ()
  self.bgImage.sprite = ResMgr.instance:LoadSprite(nornalItemBGSpritePath)
  if self.shopItemInfo ~= nil then
    self.nameText.text = LocalizationController.instance:Get(self.shopItemInfo.shopLimitItemData.name)
    self.descriptionText.text = LocalizationController.instance:Get(self.shopItemInfo.shopLimitItemData.des)
    self.itemIconImage.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(res_path.GetShopItemIconPath(self.shopItemInfo.shopLimitItemData.pic))
    self.itemIconImage:SetNativeSize()

    self.freeTimesRoot.gameObject:SetActive(false)
    self.limitTimesRoot.gameObject:SetActive(false)
    self.freeCountDownSlider.gameObject:SetActive(false)

    local costGameResData = gameResData.NewByString(self.shopItemInfo.shopLimitItemData.cost)
    self.costResourceIcon.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(uiUtil.GetBaseResIconPath(costGameResData.type))
    self.costResourceIcon:SetNativeSize()
    self.costResourceCountText.text = tostring(costGameResData.count)
    
    local available = self.shopItemInfo.remainPurchaseTimes > 0
    self.buyButton.gameObject:SetActive(available)
    self.soldOutButton.gameObject:SetActive(not available)
    
    self.first_recharge_double_root.gameObject:SetActive(false)
	self.discount_des_text.gameObject:SetActive(true)
	self.discount_remaining_time_text .gameObject:SetActive(true)
	self.extra_gain_text.gameObject:SetActive(false)
    
    if gamemanager.GetModel('shop_model').IsShopDiscountOpen() and self.shopItemInfo.shopLimitItemData.discount < 1 then
      self.discount_des_text.text = string.format(LocalizationController.instance:Get('ui.shop_view.normal_shop.discount_description'), self.shopItemInfo.shopLimitItemData.discount * 10)
	  local discountEventData = event_data.GetDataByType(130)
      local dateTime = System.DateTime.Parse(discountEventData.event_timeover)
      local serverDateTime = TimeController.instance.ServerTime
      local remianTimeSpan = dateTime - serverDateTime
      self.discount_remaining_time_text.text = string.format(LocalizationController.instance:Get('ui.shop_view.normal_shop.discount_remaining_time'), remianTimeSpan.Days)
	  self.discount_info_root.gameObject:SetActive(true)
    else
      self.discount_info_root.gameObject:SetActive(false)
    end
    
    self:CheckCost()
  end
end

function t:RefreshAsGoldItem ()
  self.bgImage.sprite = ResMgr.instance:LoadSprite(nornalItemBGSpritePath)
  if self.shopItemInfo ~= nil then
    self.nameText.text = LocalizationController.instance:Get(self.shopItemInfo.shopLimitItemData.name)
    self.descriptionText.text = LocalizationController.instance:Get(self.shopItemInfo.shopLimitItemData.des)
    self.itemIconImage.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(res_path.GetShopItemIconPath(self.shopItemInfo.shopLimitItemData.pic))
    self.itemIconImage:SetNativeSize()

    self.freeTimesRoot.gameObject:SetActive(false)
    self.limitTimesRoot.gameObject:SetActive(false)
    self.freeCountDownSlider.gameObject:SetActive(false)

    local costGameResData = gameResData.NewByString(self.shopItemInfo.shopLimitItemData.cost)
    self.costResourceIcon.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(uiUtil.GetBaseResIconPath(costGameResData.type))
    self.costResourceIcon:SetNativeSize()
    self.costResourceCountText.text = tostring(costGameResData.count)
    
    local available = self.shopItemInfo.remainPurchaseTimes > 0
    self.buyButton.gameObject:SetActive(available)
    self.soldOutButton.gameObject:SetActive(not available)
    
    self.first_recharge_double_root.gameObject:SetActive(false)
	self.discount_des_text.gameObject:SetActive(true)
	self.discount_remaining_time_text .gameObject:SetActive(true)
	self.extra_gain_text.gameObject:SetActive(false)
    
    if gamemanager.GetModel('shop_model').IsShopDiscountOpen() and self.shopItemInfo.shopLimitItemData.discount < 1 then
      self.discount_des_text.text = string.format(LocalizationController.instance:Get('ui.shop_view.normal_shop.discount_description'), self.shopItemInfo.shopLimitItemData.discount * 10)
      local discountEventData = event_data.GetDataByType(130)
      local dateTime = System.DateTime.Parse(discountEventData.event_timeover)
      local serverDateTime = TimeController.instance.ServerTime
      local remianTimeSpan = dateTime - serverDateTime
      self.discount_remaining_time_text.text = string.format(LocalizationController.instance:Get('ui.shop_view.normal_shop.discount_remaining_time'), remianTimeSpan.Days)
	  self.discount_info_root.gameObject:SetActive(true)
    else
      self.discount_info_root.gameObject:SetActive(false)
    end
    
    self:CheckCost()
  end
end

function t:RefreshAsGoodsItem ()
  self.bgImage.sprite = ResMgr.instance:LoadSprite(nornalItemBGSpritePath)
  if self.shopItemInfo ~= nil then

    
    local itemGameResData = gameResData.NewByString(self.shopItemInfo.shopGoodItemData.item)
    
    local name = nil
    local des = nil
    if self.shopItemInfo.shopGoodItemData.name ~= nil then
      name = LocalizationController.instance:Get(self.shopItemInfo.shopGoodItemData.name)
    else
      if itemGameResData.type == BaseResType.Item then
        local itemData = gamemanager.GetData('item_data').GetDataById(itemGameResData.id)
        name = LocalizationController.instance:Get(itemData.name)
      elseif itemGameResData.type == BaseResType.Hero then
        local heroData = gamemanager.GetData('hero_data').GetDataById(itemGameResData.id)
        name = LocalizationController.instance:Get(heroData.name)
      end
    end
  
    if self.shopItemInfo.shopGoodItemData.des ~= nil then
      des = LocalizationController.instance:Get(self.shopItemInfo.shopGoodItemData.des)
    else
      if itemGameResData.type == BaseResType.Item then
        local itemData = gamemanager.GetData('item_data').GetDataById(itemGameResData.id)
        des = LocalizationController.instance:Get(itemData.des)
      elseif itemGameResData.type == BaseResType.Hero then
        local heroData = gamemanager.GetData('hero_data').GetDataById(itemGameResData.id)
        des = LocalizationController.instance:Get(heroData.description)
      end
    end
    
    self.nameText.text = name
    self.descriptionText.text = des
    
    local itemIconSpritePath = nil
    if self.shopItemInfo.shopGoodItemData.pic ~= nil then
      itemIconSpritePath = res_path.GetShopItemIconPath(self.shopItemInfo.shopGoodItemData.pic)
    else
      if itemGameResData.type == BaseResType.Item then
        local itemData = gamemanager.GetData('item_data').GetDataById(itemGameResData.id)
        itemIconSpritePath = itemData:IconPath()
      elseif itemGameResData.type == BaseResType.Hero then
        local heroData = gamemanager.GetData('hero_data').GetDataById(itemGameResData.id)
        itemIconSpritePath = res_path.GetCharacterHeadIconPath(heroData.headIcons[heroData.starMin])
      end
    end
    self.itemIconImage.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(itemIconSpritePath)
    self.itemIconImage:SetNativeSize()
    
    local costGameResData = gameResData.NewByString(self.shopItemInfo.shopGoodItemData.cost)
    self.costResourceIcon.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(uiUtil.GetBaseResIconPath(costGameResData.type))
    self.costResourceIcon:SetNativeSize()
    self.costResourceCountText.text = tostring(costGameResData.count)
    
    self.freeTimesRoot.gameObject:SetActive(false)
    self.freeCountDownSlider.gameObject:SetActive(false)

    if self.shopItemInfo.shopGoodItemData.item_num > 0 then
      if self.shopItemInfo.remainPurchaseTimes > 0 then
        self.buyButton.gameObject:SetActive(true)
        self.soldOutButton.gameObject:SetActive(false)
      else
        self.buyButton.gameObject:SetActive(false)
        self.soldOutButton.gameObject:SetActive(true)
      end
      self.limitTimesText.text = string.format('%s/%s', self.shopItemInfo.remainPurchaseTimes, self.shopItemInfo.shopGoodItemData.item_num)
      self.limitTimesRoot.gameObject:SetActive(true)
    else
      self.limitTimesRoot.gameObject:SetActive(false)
      self.buyButton.gameObject:SetActive(true)
      self.soldOutButton.gameObject:SetActive(false)
    end
    
    self.first_recharge_double_root.gameObject:SetActive(false)
	self.discount_des_text.gameObject:SetActive(true)
	self.discount_remaining_time_text .gameObject:SetActive(true)
	self.extra_gain_text.gameObject:SetActive(false)
    
    if gamemanager.GetModel('shop_model').IsShopDiscountOpen() and self.shopItemInfo.shopGoodItemData.discount < 1 then
      self.discount_des_text.text = string.format(LocalizationController.instance:Get('ui.shop_view.normal_shop.discount_description'), self.shopItemInfo.shopGoodItemData.discount * 10)
      local discountEventData = event_data.GetDataByType(130)
      local dateTime = System.DateTime.Parse(discountEventData.event_timeover)
      local serverDateTime = TimeController.instance.ServerTime
      local remianTimeSpan = dateTime - serverDateTime
      self.discount_remaining_time_text.text = string.format(LocalizationController.instance:Get('ui.shop_view.normal_shop.discount_remaining_time'), remianTimeSpan.Days)
	  self.discount_info_root.gameObject:SetActive(true)
    else
      self.discount_info_root.gameObject:SetActive(false)
    end
    self:CheckCost()
  end
end

-- [[ UI event handles ]] --
function t:ClickBuyButtonHandler ()
  local freeTime = -1
  local costGameResData = nil
  local discount = 1
  
  if self.shopItemInfo.shopCardRandomData ~= nil then
    costGameResData = gameResData.NewByString(self.shopItemInfo.shopCardRandomData.cost)
    if gamemanager.GetModel('shop_model').IsShopDiscountOpen() then
      discount = self.shopItemInfo.shopCardRandomData.discount
    end
    freeTime = self.shopItemInfo.shopCardRandomData.free_time
  elseif self.shopItemInfo.shopDiamondData ~= nil then
    costGameResData = gameResData.NewByString(self.shopItemInfo.shopDiamondData.cost)
    discount = 1
  elseif self.shopItemInfo.shopLimitItemData ~= nil then
    costGameResData = gameResData.NewByString(self.shopItemInfo.shopLimitItemData.cost)
    if gamemanager.GetModel('shop_model').IsShopDiscountOpen() then
      discount = self.shopItemInfo.shopLimitItemData.discount
    end
  elseif self.shopItemInfo.shopGoodItemData ~= nil then
    costGameResData = gameResData.NewByString(self.shopItemInfo.shopGoodItemData.cost)
    if gamemanager.GetModel('shop_model').IsShopDiscountOpen() then
      discount = self.shopItemInfo.shopGoodItemData.discount
    end
  end
  
  local realCost = math.floor(costGameResData.count * discount)
  if freeTime > 0 and self.shopItemInfo.remainFreeTimes > 0 and self.shopItemInfo:GetNextFreeBuyCountDownTime() <= 0  then
    realCost = 0
  end
  
  if costGameResData.type == BaseResType.Diamond and realCost > gamemanager.GetModel('game_model').GetBaseResourceValue(BaseResType.Diamond) then
    local diamondNotEnoughTipsString = LocalizationController.instance:Get('ui.common_tips.not_enough_diamond_and_go_to_buy')
    local confirm_tip_view = require('ui/tips/view/confirm_tip_view')
    confirm_tip_view.Open(diamondNotEnoughTipsString, t.ClickGoToBuyDiamond, ConsumeTipType.None);
    return
  end
  
  if self.transform == nil or self.clickedBuy then
    return
  end
  shopController.TryPurchaseGoods(self.shopItemInfo)
  self.clickedBuy = true
  
  LeanTween.delayedCall(self.transform.gameObject, 0.6, Action(function()
        self.clickedBuy = false
      end))
end

function t.ClickGoToBuyDiamond ()
  gamemanager.GetModel('function_open_model').OpenFunction(FunctionOpenType.MainView_Shop, 3)
end

function t:CheckCost ()
  local freeTime = -1
  local costGameResData
  local discount = 1
  
  if self.shopItemInfo.shopCardRandomData ~= nil then
    costGameResData = gameResData.NewByString(self.shopItemInfo.shopCardRandomData.cost)
    if gamemanager.GetModel('shop_model').IsShopDiscountOpen() then
      discount = self.shopItemInfo.shopCardRandomData.discount
    end
    freeTime = self.shopItemInfo.shopCardRandomData.free_time
  elseif self.shopItemInfo.shopDiamondData ~= nil then
    costGameResData = gameResData.NewByString(self.shopItemInfo.shopDiamondData.cost)
    discount = 1
  elseif self.shopItemInfo.shopLimitItemData ~= nil then
    costGameResData = gameResData.NewByString(self.shopItemInfo.shopLimitItemData.cost)
    if gamemanager.GetModel('shop_model').IsShopDiscountOpen() then
      discount = self.shopItemInfo.shopLimitItemData.discount
    end
  elseif self.shopItemInfo.shopGoodItemData ~= nil then
    costGameResData = gameResData.NewByString(self.shopItemInfo.shopGoodItemData.cost)
    if gamemanager.GetModel('shop_model').IsShopDiscountOpen() then
      discount = self.shopItemInfo.shopGoodItemData.discount
    end
  end
  
  self.costResourceIcon.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(uiUtil.GetBaseResIconPath(costGameResData.type))
  self.costResourceIcon:SetNativeSize()
  local realCost = math.floor(costGameResData.count * discount)
  self.costResourceCountText.text = tostring(realCost)
  self.freeCountDownSlider.gameObject:SetActive(false)
  self.freeCountDownText.gameObject:SetActive(false)

  self.freeParticleRoot.gameObject:SetActive(false)
  if freeTime > 0 then
    if self.shopItemInfo.remainFreeTimes > 0 then
      if self.shopItemInfo:GetNextFreeBuyCountDownTime() > 0 then
        self.freeCountDownSlider.value = (freeTime - self.shopItemInfo:GetNextFreeBuyCountDownTime()) / freeTime
        self.freeCountDownText.text = Common.Util.TimeUtil.FormatSecondToHour(self.shopItemInfo:GetNextFreeBuyCountDownTime())
        self.freeCountDownSlider.gameObject:SetActive(true)
        self.freeCountDownText.gameObject:SetActive(true)
        
        LeanTween.delayedCall(self.transform.gameObject, 1, Action(function ()
              self:CheckCost ()
            end))
      else
        self.freeParticleRoot.gameObject:SetActive(true)
        self.costResourceCountText.text = LocalizationController.instance:Get('ui.shop_view.free')
        self.freeCountDownSlider.gameObject:SetActive(false)
      end
    end
  end
end
-- [[ UI event handles ]] --

return t