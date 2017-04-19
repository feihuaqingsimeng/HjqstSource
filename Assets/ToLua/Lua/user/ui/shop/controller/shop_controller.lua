local t = {}
local name = 'shop_controller'
local shopModel = gamemanager.GetModel('shop_model')
local gameResData = require('ui/game/model/game_res_data')
local heroInfo = require('ui/hero/model/hero_info')
local equipInfo = require('ui/equip/model/equip_info')
local itemInfo = require('ui/item/model/item_info')
local autoDestroyTipView = require('ui/tips/view/auto_destroy_tip_view')
local confirmBuyItemTipsView = require('ui/tips/view/confirm_buy_item_tips_view')

local isDrawCardCostFree = false

--支付订单数据
local diamondShopItemInfo = nil

require 'shopping_pb'
require 'common_pb'
require 'login_pb'

local function Start ()
  netmanager.RegisterProtocol(MSG.PurchaseGoodsResp, t.PurchaseGoodsResp)
  netmanager.RegisterProtocol(MSG.PurchaseDrawCardGoodsResp, t.PurchaseDrawCardGoodsResp)
  netmanager.RegisterProtocol(MSG.DrawCardGoodsResp, t.DrawCardGoodsResp)
  netmanager.RegisterProtocol(MSG.DrawCardGoodsUpdateResp, t.DrawCardGoodsUpdateResp)
  netmanager.RegisterProtocol(MSG.LimitGoodsResp, t.LimitGoodsResp)
  netmanager.RegisterProtocol(MSG.LimitGoodsUpdateResp, t.LimitGoodsUpdateResp)
  netmanager.RegisterProtocol(MSG.OtherGoodsResp, t.OtherGoodsResp)
  netmanager.RegisterProtocol(MSG.OtherGoodsUpdateResp, t.OtherGoodsUpdateResp)

  netmanager.RegisterProtocol(MSG.PayOrderNoResp, t.PayOrderNoResp)
  netmanager.RegisterProtocol(MSG.DiamondGoodsResp, t.DiamondGoodsResp)
  netmanager.RegisterProtocol(MSG.AppStoreVerifyResp, t.AppStoreVerifyResp)
  netmanager.RegisterProtocol(MSG.PurchaseDrawAwardResp, t.PurchaseDrawAwardResp)
  netmanager.RegisterProtocol(MSG.MonthsCardInfoResp, t.MonthsCardInfoResp)
    
  gamemanager.RegisterCtrl(name,t)
end

-- [[ C2S ]] --
function t.PurchaseGoodsReq (goodsType, goodsNo, costType)
  local purchaseGoodsReq = shopping_pb.PurchaseGoodsReq()
  purchaseGoodsReq.goodsType = goodsType
  purchaseGoodsReq.goodsNo = goodsNo
  purchaseGoodsReq.costType = costType
  isDrawCardCostFree = costType == 0
  netmanager.SendProtocol(MSG.PurchaseGoodsReq, purchaseGoodsReq)
end

-- [[ S2C ]] --
function t.PurchaseGoodsResp ()
  local resp = shopping_pb.PurchaseGoodsResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  local shopItemName = ''
  local tempCost = nil
  local tempItem = nil
  if resp.goodsType == ShopItemType.Diamond then
    local shopDiamondItemInfo = shopModel.shopDiamondItemInfos[resp.goodsNo]
    if shopDiamondItemInfo ~= nil then
      shopItemName = Common.Localization.LocalizationController.instance:Get(shopDiamondItemInfo.shopDiamondData.name)
    end
  elseif resp.goodsType == ShopItemType.Action then
    local shopActionItemInfo = shopModel.shopActionItemInfos[resp.goodsNo]
    if shopActionItemInfo ~= nil then
      shopItemName = Common.Localization.LocalizationController.instance:Get(shopActionItemInfo.shopLimitItemData.name)
      tempCost = gameResData.NewByString(shopActionItemInfo.shopLimitItemData.cost)
      tempItem = gameResData.NewByString(shopActionItemInfo.shopLimitItemData.resource)
    end
  elseif resp.goodsType == ShopItemType.Gold then
    local shopGoldItemInfo = shopModel.shopGoldItemInfos[resp.goodsNo]
    if shopGoldItemInfo ~= nil then
      shopItemName = Common.Localization.LocalizationController.instance:Get(shopGoldItemInfo.shopLimitItemData.name)
      tempItem = gameResData.NewByString(shopGoldItemInfo.shopLimitItemData.resource)
      tempCost = gameResData.NewByString(shopGoldItemInfo.shopLimitItemData.cost)
    end
  elseif resp.goodsType == ShopItemType.Goods then
    local shopGoodsItemInfo = shopModel.shopGoodsItemInfos[resp.goodsNo]
    if shopGoodsItemInfo ~= nil then
      local itemGameResData = gameResData.NewByString(shopGoodsItemInfo.shopGoodItemData.item)
      if itemGameResData.type == BaseResType.Item then
        local itemData = gamemanager.GetData('item_data').GetDataById(itemGameResData.id)
        shopItemName = LocalizationController.instance:Get(itemData.name)
      elseif itemGameResData.type == BaseResType.Hero then
        local heroData = gamemanager.GetData('hero_data').GetDataById(itemGameResData.id)
        shopItemName = LocalizationController.instance:Get(heroData.name)
      end
      
      tempCost = gameResData.NewByString(shopGoodsItemInfo.shopGoodItemData.cost)
      tempItem = itemGameResData
    end
  end
  --talkingData--
  if tempCost and tempItem then
    if tempCost.type == BaseResType.Diamond then
      TalkingDataController.instance:TDGAItemOnPurchase("商店",tempItem.type,tempItem.id,tempItem.count,tempCost.count)
    end
  end
  --end--
  autoDestroyTipView.Open(string.format(Common.Localization.LocalizationController.instance:Get('ui.shop_view.buy_success_tips'), shopItemName))
end

-- [[ S2C ]] --
function t.PurchaseDrawCardGoodsResp ()
  local resp = shopping_pb.PurchaseDrawCardGoodsResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  local tempCardInfoList = ArrayList.New('heroInfo or equipInfo or itemInfo')
  local cardRandomData = gamemanager.GetData('shop_card_random_data').GetDataById(resp.goodsNo)
  local randomCardInfo = shopModel.shopHeroRandomCardInfos[resp.goodsNo]
  if cardRandomData.sheet_num == ShopItemType.HeroDrawCard then
    if randomCardInfo.shopCardRandomData.buy_type == 1 then           -- Single Draw
      local commonCardInfo = nil
      if resp.commonGoods[1].type == BaseResType.Hero then
        commonCardInfo = heroInfo.NewByDrawCardDropProto(resp.commonGoods[1])
      elseif  resp.commonGoods[1].type == BaseResType.Equipment then
        commonCardInfo = equipInfo.New(resp.commonGoods[1].instanceId, resp.commonGoods[1].no)
      elseif  resp.commonGoods[1].type == BaseResType.Item then
        commonCardInfo = itemInfo.New(resp.commonGoods[1].instanceId, resp.commonGoods[1].no, resp.commonGoods[1].num)
      end
      commonCardInfo.baseResType = resp.commonGoods[1].type
      tempCardInfoList:Add(commonCardInfo)
      t.OpenSingleDrawCardResultView(randomCardInfo, commonCardInfo)
    elseif randomCardInfo.shopCardRandomData.buy_type == 2 then    -- Ten Draw
      local commonCardInfo = nil
      local commonCardInfoList = {}
      for k, v in ipairs(resp.commonGoods) do
        if v.type == BaseResType.Hero then
          commonCardInfo = heroInfo.NewByDrawCardDropProto(v)
        elseif v.type == BaseResType.Equipment then
          commonCardInfo = equipInfo.New(v.instanceId, v.no)
        elseif v.type == BaseResType.Item then
          commonCardInfo = itemInfo.New(v.instanceId, v.no, v.num)
        end
        commonCardInfo.baseResType = v.type
        table.insert(commonCardInfoList, commonCardInfo)
        tempCardInfoList:Add(commonCardInfo)
      end
      
      local specialCommonCardInfo = nil
      if resp.specialGoods.type == BaseResType.Hero then
        specialCommonCardInfo = heroInfo.NewByDrawCardDropProto(resp.specialGoods)
      elseif resp.specialGoods.type == BaseResType.Equipment then
        specialCommonCardInfo = equipInfo.New(resp.specialGoods.instanceId, resp.specialGoods.no)
      elseif resp.specialGoods.type == BaseResType.Item then
        specialCommonCardInfo = itemInfo.New(resp.specialGoods.instanceId, resp.specialGoods.no, resp.specialGoods.num)
      end
      specialCommonCardInfo.baseResType = resp.specialGoods.type
      
      t.OpenTenDrawCardResultView(randomCardInfo, commonCardInfoList, specialCommonCardInfo)
      tempCardInfoList:Add(specialCommonCardInfo)
    end
    
    --talkingData--
    local tempCost = gameResData.NewByString(cardRandomData.cost)
    if tempCost.type == BaseResType.Diamond and not isDrawCardCostFree then
      if tempCardInfoList.Count == 1 then
        for k,v in pairs(tempCardInfoList:GetDatas()) do
          if v.baseResType == BaseResType.Hero then
            TalkingDataController.instance:TDGAItemOnPurchase("商店",v.baseResType,v.heroData.id,1,tempCost.count)
          elseif  v.baseResType == BaseResType.Equipment then
            TalkingDataController.instance:TDGAItemOnPurchase("商店",v.baseResType,v.data.id,1,tempCost.count)
          elseif v.baseResType == BaseResType.Item then
            TalkingDataController.instance:TDGAItemOnPurchase("商店",v.baseResType,v.itemData.id,v:Count(),tempCost.count)
          end
        end
      else
        TalkingDataController.instance:TDGAItemOnPurchaseByCount('商店十连抽',1,tempCost.count)
      end
    end
    
    --end--
  end
end

-- [[ C2S ]] --
function t.DrawCardGoodsReq ()
  local drawCardGoodsReq = shopping_pb.DrawCardGoodsReq()
  netmanager.SendProtocol(MSG.DrawCardGoodsReq, drawCardGoodsReq)
end

-- [[ S2C ]] --
function t.DrawCardGoodsResp ()
  local resp = shopping_pb.DrawCardGoodsResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  local drawCardGoods = resp.goods
  local heroDrawCardInfo = nil
  local articleDrawCardInfo = nil
  for k, v in ipairs(drawCardGoods) do
    heroDrawCardInfo = shopModel.shopHeroRandomCardInfos[v.goodsNo]
    articleDrawCardInfo = shopModel.shopArticleRandomCardInfos[v.goodsNo]
    
    if heroDrawCardInfo ~= nil then
      heroDrawCardInfo:SetFreeInfo(v.remainFreeTimes, v.freeDrawCoolingOverTime)
    end
    
    if articleDrawCardInfo ~= nil then
      articleDrawCardInfo:SetFreeInfo(v.remainFreeTimes, v.freeDrawCoolingOverTime)
    end
    
    heroDrawCardInfo = nil
    articleDrawCardInfo = nil
  end
  if resp.purchaseDrawCredit ~= nil then
    shopModel.purchaseDrawCredit = resp.purchaseDrawCredit
  end
  shopModel.OnShopHeroRandomCardInfoListUpdate()
  shopModel.OnShopArticleRandomCardInfoListUpdate()  
end

-- [[ S2C ]] --
function t.DrawCardGoodsUpdateResp ()
  local resp = shopping_pb.DrawCardGoodsUpdateResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  local updateGoods = resp.updateGoods
  local heroDrawCardInfo = shopModel.shopHeroRandomCardInfos[updateGoods.goodsNo]
  local articleDrawCardInfo = shopModel.shopArticleRandomCardInfos[updateGoods.goodsNo]
  if heroDrawCardInfo ~= nil then
    heroDrawCardInfo:SetFreeInfo(updateGoods.remainFreeTimes, updateGoods.freeDrawCoolingOverTime)
  elseif articleDrawCardInfo ~= nil then
    articleDrawCardInfo:SetFreeInfo(updateGoods.remainFreeTimes, updateGoods.freeDrawCoolingOverTime)
  end
    
  shopModel.OnShopHeroRandomCardInfoListUpdate()
  shopModel.OnShopArticleRandomCardInfoListUpdate() 
end

-- [[ C2S ]] --
function t.LimitGoodsReq ()
  local limitGoodsReq = shopping_pb.LimitGoodsReq()
  netmanager.SendProtocol(MSG.LimitGoodsReq, limitGoodsReq)
end

-- [[ S2C ]] --
function t.LimitGoodsResp ()
  local resp = shopping_pb.LimitGoodsResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  for k, v in pairs(shopModel.shopActionItemInfos) do
    v:SetInfo(false, 0)
  end
  
  for k, v in pairs(shopModel.shopGoldItemInfos) do
    v:SetInfo(false, 0)
  end
  
  local actionItemInfo = nil
  local goldItemInfo = nil
  
  for k, v in ipairs(resp.goods) do
    actionItemInfo = shopModel.shopActionItemInfos[v.goodsNo]
    goldItemInfo = shopModel.shopGoldItemInfos[v.goodsNo]
    
    if actionItemInfo ~= nil then
      actionItemInfo:SetInfo(true, v.remianPurchaseTimes)
    end
    if goldItemInfo ~= nil then
      goldItemInfo:SetInfo(true, v.remianPurchaseTimes)
    end
    actionItemInfo = nil
    goldItemInfo = nil
  end
  
  shopModel.OnShopActionItemInfoListUpdate()
  shopModel.OnShopGoldItemInfoListUpdate ()
end

-- [[ S2C ]] --
function t.LimitGoodsUpdateResp ()
  local resp = shopping_pb.LimitGoodsUpdateResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  -- [[ Add ]] --
  local actionItemInfo = nil
  local goldItemInfo = nil
  
  for k, v in ipairs(resp.addGoods) do
    actionItemInfo = shopModel.shopActionItemInfos[v.goodsNo]
    goldItemInfo = shopModel.shopGoldItemInfos[v.goodsNo]
    
    if actionItemInfo ~= nil then
      actionItemInfo:SetInfo(true, v.remianPurchaseTimes)
    elseif goldItemInfo ~= nil then
      goldItemInfo:SetInfo(true, v.remianPurchaseTimes)
    end
    
    actionItemInfo = nil
    goldItemInfo = nil
  end
  
  -- [[ Update ]] --
  if resp.updateGoods ~= nil then
    actionItemInfo = shopModel.shopActionItemInfos[resp.updateGoods.goodsNo]
    goldItemInfo = shopModel.shopGoldItemInfos[resp.updateGoods.goodsNo]
    if actionItemInfo ~= nil then
      actionItemInfo:SetInfo(true, resp.updateGoods.remianPurchaseTimes)
    elseif goldItemInfo ~= nil then
      goldItemInfo:SetInfo(true, resp.updateGoods.remianPurchaseTimes)
    end
  end
  
  -- [[ Delete ]] --
  actionItemInfo = shopModel.shopActionItemInfos[resp.delGoods]
  goldItemInfo = shopModel.shopGoldItemInfos[resp.delGoods]
  if actionItemInfo ~= nil then
    actionItemInfo:SetInfo(false, 0)
  elseif goldItemInfo ~= nil then
    goldItemInfo:SetInfo(false, 0)
  end
  
  shopModel.OnShopActionItemInfoListUpdate()
  shopModel.OnShopGoldItemInfoListUpdate ()
end

-- [[ C2S ]] --
function t.OtherGoodsReq ()
  local otherGoodsReq = shopping_pb.OtherGoodsReq()
  netmanager.SendProtocol(MSG.OtherGoodsReq, otherGoodsReq)
end

-- [[ S2C ]] --
function t.OtherGoodsResp ()
  local resp = shopping_pb.OtherGoodsResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  for k, v in pairs(shopModel.shopGoodsItemInfos) do
    v:SetRemainPurchaseTimes(0)
  end
  
  local info = nil
  for k, v in ipairs(resp.goods) do
    info = shopModel.shopGoodsItemInfos[v.goodsNo]
    if info ~= nil then
      info:SetRemainPurchaseTimes(v.remianPurchaseTimes)
    end
    info = nil
  end
  shopModel.OnShopGoodsItemInfoListUpdate()
end

-- [[ S2C ]] --
function t.OtherGoodsUpdateResp ()
  local resp = shopping_pb.OtherGoodsUpdateResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  local info = shopModel.shopGoodsItemInfos[resp.updateGoods.goodsNo]
  if info ~= nil then
    info:SetRemainPurchaseTimes(resp.updateGoods.remianPurchaseTimes)
    shopModel.OnShopGoodsItemInfoListUpdate()
  end
end
--订单请求
function t.PayOrderNoReq(productName,productDes,productTitle,productNo,price)
  local req = login_pb.PayOrderNoReq()
  req.productName = productName
  req.productDes = productDes
  req.productTitle = productTitle
  req.productNo = productNo
  print('req order id:'..tostring(price))
  req.money = price -- 单位分
  req.extension = ''
  netmanager.SendProtocol(MSG.PayOrderNoReq, req)
end
--订单回调
function t.PayOrderNoResp()
  local resp = login_pb.PayOrderNoResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  local stateCode = resp.stateCode
  local orderID = resp.orderID
  local extensionStr = resp.extension
  local extension = string.gsub(extensionStr,'"','\'') 
--  local strLen = string.len(extensionStr)
--  print('extensionStr len:'..tostring(strLen))
--  if(strLen > 0) then
--    for i = 1,string.len(extensionStr),1 do
--     print('char:'..string.sub(extensionStr,i,i))
--    end
--  end
  --print('extension',extension)
  
  local money = gameResData.NewByString(diamondShopItemInfo.shopDiamondData.cost)
  local diamond = gameResData.NewByString(diamondShopItemInfo.shopDiamondData.resource)
  local shopItemID = diamondShopItemInfo:GetShopItemID()
  local game_model = gamemanager.GetModel('game_model')
  local data = {}
  data['id'] = orderID
  data['itemid'] = shopItemID
  data['price'] = money.count
  data['productNum'] = diamond.count
  data['productName'] = LocalizationController.instance:Get(diamondShopItemInfo.shopDiamondData.name)
  data['productDes'] = LocalizationController.instance:Get(diamondShopItemInfo.shopDiamondData.name)
  data['extension'] = extension
  data['diamond'] = game_model.GetBaseResourceValue(BaseResType.Diamond)
  data['roleId'] = game_model.accountId
  data['accountLevel'] = game_model.accountLevel
  data['roleName'] = game_model.accountName
  data['serverId'] = game_model.lastServerId
  data['serveName'] = game_model.serverName
  data['vip'] = gamemanager.GetModel('vip_model').vipLevel
    
  --自己接vivo需要accessKey
  data['accessKey'] = '1'
  
  local channelId = game_model.channelId
    
  Debugger.LogError('--------------------------channelId:'..channelId)
  local str = lua_json.TableToJsonStr(data)
  LuaInterface.LuaCsTransfer.ShowSdkPay(str)
  
  --local game_model = gamemanager.GetModel('game_model')
  if game_model.channelId == PlatformType.iOS then--ios
    AdTrackingController.instance:AdTracking_OnPay(game_model.accountId, orderID, money.count, 'RMB', '')
  end
end
-- [[ Other public methods ]] --
function t.TryPurchaseGoods (shopItemInfo)
  if shopItemInfo:GetCostType() == 0 then   -- if cost type is free, send buy goods protocol to server instantly
    t.PurchaseGoods(shopItemInfo)
    return
  end
  
  local costGameResData = shopItemInfo:GetCostResData()
  local clickBuyAction = Action(function ()
      t.PurchaseGoods(shopItemInfo)
    end)
  
  local consumeTipType = 0
  local shopID = shopItemInfo:GetShopItemType () 
  local shopItemID = shopItemInfo:GetShopItemID()
  local discount = 1
  if shopID == ShopItemType.HeroDrawCard then
    if shopItemID == 11 then
      consumeTipType = ConsumeTipType.DiamondDrawSingleHero
    elseif shopItemID == 12 then
      consumeTipType = ConsumeTipType.DiamondDrawTenHeroes
    elseif shopItemID == 13 then
      consumeTipType = ConsumeTipType.HonorDrawSingleHero
    end
    if gamemanager.GetModel('shop_model').IsShopDiscountOpen() then
      discount = shopItemInfo.shopCardRandomData.discount
    end
  elseif shopID == ShopItemType.ArticleDrawCard then
    if shopItemID == 14 then
      consumeTipType = ConsumeTipType.GoldDrawSingleArticle
    elseif shopItemID == 15 then
      consumeTipType = ConsumeTipType.HonorDrawSingleArticle
    elseif shopItemID == 16 then
      consumeTipType = ConsumeTipType.GoldDrawTenArticles
    end
  elseif shopID == ShopItemType.Diamond then
    consumeTipType = 0
  elseif shopID == ShopItemType.Action then
    consumeTipType = ConsumeTipType.DiamondBuyPveAction
    if gamemanager.GetModel('shop_model').IsShopDiscountOpen() then
      discount = shopItemInfo.shopLimitItemData.discount
    end
  elseif shopID == ShopItemType.Gold then
    consumeTipType = ConsumeTipType.DiamondBuyCoin
    if gamemanager.GetModel('shop_model').IsShopDiscountOpen() then
      discount = shopItemInfo.shopLimitItemData.discount
    end
  elseif shopID == ShopItemType.Goods then
    consumeTipType = 0
    if gamemanager.GetModel('shop_model').IsShopDiscountOpen() then
      discount = shopItemInfo.shopGoodItemData.discount
    end
  end
  

  local realCost = math.floor(costGameResData.count * discount)
  if costGameResData.type == BaseResType.Diamond and realCost > gamemanager.GetModel('game_model').GetBaseResourceValue(BaseResType.Diamond) then
    local diamondNotEnoughTipsString = LocalizationController.instance:Get('ui.common_tips.not_enough_diamond_and_go_to_buy')
    local confirm_tip_view = require('ui/tips/view/confirm_tip_view')
    confirm_tip_view.Open(diamondNotEnoughTipsString, t.GoToBuyDiamond, ConsumeTipType.None);
    return
  end

  
  local consume_tip_model = gamemanager.GetModel('consume_tip_model')
  if consume_tip_model.GetConsumeTipEnable(consumeTipType) then
    LuaInterface.LuaCsTransfer.OpenConfirmBuyShopItemTipsView(shopItemInfo:GetShopItemName (), costGameResData.type, costGameResData.id, math.floor(costGameResData.count * discount), clickBuyAction, consumeTipType)
  else
    t.PurchaseGoods(shopItemInfo)
  end
end

function t.GoToBuyDiamond ()
  gamemanager.GetModel('function_open_model').OpenFunction(FunctionOpenType.MainView_Shop, 3)
end

function t.PurchaseGoods (shopItemInfo)
  local shopID = shopItemInfo:GetShopItemType() 
  local shopItemID = shopItemInfo:GetShopItemID()
  local costType = shopItemInfo:GetCostType()
  if shopID == ShopItemType.Diamond then    
    diamondShopItemInfo = shopItemInfo
    local game_model = gamemanager.GetModel('game_model')
    if game_model.channelId == PlatformType.iOS then--ios payment
      local shopItemID = diamondShopItemInfo:GetShopItemID()
      --此段为修复客户端表与app store商品ID不一致的问题所加
      if shopItemID == 105 then
        shopItemID = 26
      elseif shopItemID == 106 then
        shopItemID = 27
      end
      --此段为修复客户端表与app store商品ID不一致的问题所加
      local data = {}
      data['itemid'] = shopItemID      
      local str = lua_json.TableToJsonStr(data)
      LuaInterface.LuaCsTransfer.ShowSdkPay(str)    
    elseif game_model.channelId == 0 then
      t.PurchaseGoodsReq(shopID, shopItemID, costType)
    else      
      local productName = LocalizationController.instance:Get(shopItemInfo.shopDiamondData.name)
      local productDes = LocalizationController.instance:Get(shopItemInfo.shopDiamondData.des)
      local price = gameResData.NewByString(diamondShopItemInfo.shopDiamondData.cost).count
      t.PayOrderNoReq(productName,productDes,productName,shopItemID,price)    
    end
  else
      t.PurchaseGoodsReq(shopID, shopItemID, costType)
  end
end

function t.DiamondGoodsReq ()
  netmanager.SendProtocol(MSG.DiamondGoodsReq, nil)
end

function t.DiamondGoodsResp ()

  local diamondGoodsResp = shopping_pb.DiamondGoodsResp()
  diamondGoodsResp:ParseFromString(netmanager.GetProtocolData())
  gamemanager.GetModel('shop_model').OnShopDiamondItemInfoListUpdate (diamondGoodsResp.firstChargeGoodsList)
end

function t.AppStoreVerifyReq(goodsNo,base64Encoding,transactionIdentifier)
  local appStoreVerifyReq = shopping_pb.AppStoreVerifyReq()
  appStoreVerifyReq.goodsNo = goodsNo
  appStoreVerifyReq.info = base64Encoding
  appStoreVerifyReq.orderId = transactionIdentifier
  netmanager.SendProtocol(MSG.AppStoreVerifyReq, appStoreVerifyReq)
end

function t.AppStoreVerifyResp()
  local resp = common_pb.StringProto()
  resp:ParseFromString(netmanager.GetProtocolData())
  print('状态码'..resp.value)
  if resp.value == 'SUCCESS' then
    print('IOSVerifySuccess')
    LuaInterface.LuaCsTransfer.IOSVerifySuccess()
  end
end

function t.PurchaseDrawAwardReq ()
  netmanager.SendProtocol(MSG.PurchaseDrawAwardReq, nil)
end

function t.PurchaseDrawAwardResp ()
  local resp = common_pb.DoubleIntProto()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  shopModel.purchaseDrawCredit = resp.value2
  print('PurchaseDrawAwardResp.value1:'..resp.value1)
  print('PurchaseDrawAwardResp.value2:'..resp.value2)
  
  shopModel.OnPurchaseDrawAwardSuccess()
  
  local heroInfo = gamemanager.GetModel('hero_model').GetHeroInfo(resp.value1)
  heroInfo.baseResType = BaseResType.Hero
  t.OpenPurchaseDrawAwardResultView (heroInfo)
end

function t.MonthsCardInfoResp ()
  local resp = common_pb.DoubleIntProto()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  shopModel.monthCardRemainDays = resp.value1
  shopModel.extremeMonthCardRemainDays = resp.value2
  shopModel.OnMonthCardsInfoUpdate()
  print('[[t.MonthsCardInfoResp]]')
  print('[Month Card Remain Days = '..shopModel.monthCardRemainDays..']')
  print('[Extreme Month Card Remain Days = '..shopModel.extremeMonthCardRemainDays..']')
  print('[[t.MonthsCardInfoResp]]')
end

-- [ OPEN VIEW METHODS ] --
function t.OpenShopView (toggleIndex)
  if not gamemanager.GetModel('function_open_model').IsFunctionOpen(FunctionOpenType.MainView_Shop,true) then
    return
  end
  local shopView = require('ui/shop/view/shop_view')
  shopView.Open(toggleIndex)
end

function t.OpenRecruitSingleHeroResultView (shopItemInfo, heroInfo)
  local recruitSingleHeroResultView = require('ui/shop/view/recruit_single_hero_result_view')
  recruitSingleHeroResultView.Open(shopItemInfo, heroInfo)
end

function t.OpenRecruitTenHeroesResultView (shopItemInfo, heroInfos, specialHeroInfo)
  local recruitTenHeroesResultView = dofile('ui/shop/view/recruit_ten_heroes_result_view')
  recruitTenHeroesResultView.Open(shopItemInfo, heroInfos, specialHeroInfo)
end

function t.OpenDrawSingleArticleResultView (shopItemInfo, articleInfo)
  local drawSingleArticleResultView = dofile('ui/shop/view/draw_single_article_result_view')
  drawSingleArticleResultView.Open(shopItemInfo, articleInfo)
end

function t.OpenDrawTenArticlesResultView (shopItemInfo, articleInfos, specialArticleInfo)
  local drawTenArticlesResultView = dofile('ui/shop/view/draw_ten_articles_result_view')
  drawTenArticlesResultView.Open(shopItemInfo, articleInfos, specialArticleInfo)
end

function t.OpenPurchaseDrawAwardResultView (heroInfo)
  local singleDrawCardResultView = dofile('ui/shop/view/single_draw_card_result_view')
  singleDrawCardResultView.OpenAsPurchaseDrawAwardResultView(heroInfo)
end

function t.OpenSingleDrawCardResultView (shopItemInfo, cardItemInfo)
  local singleDrawCardResultView = dofile('ui/shop/view/single_draw_card_result_view')
  singleDrawCardResultView.Open(shopItemInfo, cardItemInfo)
end

function t.OpenTenDrawCardResultView (shopItemInfo, commonCardInfoList, specialCommonCardInfo)
  local tenDrawCardResultView = dofile('ui/shop/view/ten_draw_card_result_view')
  tenDrawCardResultView.Open(shopItemInfo, commonCardInfoList, specialCommonCardInfo)
end
-- [ OPEN VIEW METHODS ] --



-- [[ Other public methods ]] --

Start ()
return t