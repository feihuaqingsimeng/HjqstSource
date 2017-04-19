local t = {}
t.__index = t


local game_model = gamemanager.GetModel('game_model')
local item_model = gamemanager.GetModel('item_model')
local common_hero_icon = require('ui/common_icon/common_hero_icon')
local common_reward_icon = require('ui/common_icon/common_reward_icon')
local item_data = gamemanager.GetData('item_data')
local common_error_tip_view = require('ui/tips/view/common_error_tips_view')
local consume_tip_model = gamemanager.GetModel('consume_tip_model')
local black_market_controller = gamemanager.GetModel('black_market_controller')

function t.BindGameObject(go)
  local o = {}
  setmetatable(o,t)
  o.transform = go:GetComponent(typeof(Transform))
  o:InitComponent()
  o.blackMarketInfo = nil
  o.rewardIcon = nil
  o.bigHeroIcon = nil
  o.needRisePrice = false
  --oneParam:blackMarketInfo
  o.onClick = void_delegate.New()
  return o
end

function t:InitComponent()
  self.textRemaindCount = self.transform:Find('text_count'):GetComponent(typeof(Text))
  local mask = self.transform:Find('img_mask')
  if mask then
    self.goMask = mask.gameObject
  end
  self.tranItemRoot = self.transform:Find('item_root')
  local limitLv = self.transform:Find('img_mask/text_lv')
  if limitLv then
    self.textLimitLevel = limitLv:GetComponent(typeof(Text))
  end
  self.textName = self.transform:Find('text_name'):GetComponent(typeof(Text))
  self.btnBuy = self.transform:Find('btn_buy'):GetComponent(typeof(Button))
  self.btnBuy.onClick:AddListener(function()
      self:ClickBuyHandler()
    end)
  self.textBuyCount = ui_util.FindComp(self.btnBuy.transform,'text_count',Text)
  self.imgMaterialIcon = ui_util.FindComp(self.btnBuy.transform,'text_count/img_icon',Image)
  self.textBuyOver = ui_util.FindComp(self.btnBuy.transform,'text_over',Text)
end
--needRisePrice：操蛋的策划说第一个页签价格增长的
function t:SetData(blackMarketInfo,needRisePrice)
  
  self.blackMarketInfo = blackMarketInfo
  self.needRisePrice = needRisePrice
  local item = blackMarketInfo:GetItemResData()
  
  
  if self.rewardIcon and self.rewardIcon.gameResData.type ~= item.type  then
    GameObject.Destroy(self.rewardIcon.transform.gameObject)
    self.rewardIcon = nil
  end
  if not self.rewardIcon then
    self.rewardIcon = common_reward_icon.New(self.tranItemRoot,item)
  end
  self.rewardIcon:SetGameResData(item)
  self.rewardIcon:AddDesButton()
  self.textName.text = self.blackMarketInfo:GetGoodsName()
  self.textRemaindCount.text = blackMarketInfo:GetRemindCountString()
  if self.blackMarketInfo.remaindCount == 0 then
    self.textBuyCount.gameObject:SetActive(false)
    self.textBuyOver.gameObject:SetActive(true)
  else
    self.textBuyCount.gameObject:SetActive(true)
    self.textBuyOver.gameObject:SetActive(false)
  end
  if self.goMask then
    self.goMask:SetActive(game_model.accountLevel < blackMarketInfo:GetLimitLevel())
  end
  if self.textLimitLevel then
    self.textLimitLevel.text = string.format(LocalizationController.instance:Get("ui.black_market_view.limit_lv"),blackMarketInfo:GetLimitLevel())
  end
  self:RefreshMaterial()
end
function t:RefreshMaterial()
  local materials = self.blackMarketInfo:GetMaterials()
  local mt = materials[1]
  local own = 0
  if mt.type == BaseResType.Item then
    
    own = item_model.GetItemCountByItemID(mt.id)
    self.imgMaterialIcon.sprite = ResMgr.instance:LoadSprite( item_data.GetDataById(mt.id):IconPath())
  else
    own = game_model.GetBaseResourceValue(mt.type)
    self.imgMaterialIcon.sprite = ResMgr.instance:LoadSprite(ui_util.GetBaseResIconPath(mt.type))
  end
  local exchangeCost = mt.count
  if self.needRisePrice then
    exchangeCost = self.blackMarketInfo:GetRisePrice()    
  end
  if own < exchangeCost then
    self.textBuyCount.text =  ui_util.FormatToRedText(string.format('%s',exchangeCost))
  else
    self.textBuyCount.text = ui_util.FormatToGreenText(string.format('%s',exchangeCost))
  end
 
  
end

--------------------------click event-----------------------

--购买
function t:ClickBuyHandler()

  if self.blackMarketInfo:GetLimitLevel() > game_model.accountLevel then
    common_error_tip_view.Open(LocalizationController.instance:Get("ui.black_market_view.levelNotEnough"))
    return
  end
  if self.blackMarketInfo.remaindCount == 0 then
    common_error_tip_view.Open(LocalizationController.instance:Get("ui.black_market_view.exchangeEmpty"))
    return
  end
  
  
  local buyItem = self.blackMarketInfo:GetItemResData()
  if (buyItem.type == BaseResType.Hero and game_model.CheckPackFull(true,false) ) or (buyItem.type == BaseResType.Equipment and game_model.CheckPackFull(false,true)) then
    return
  end
  
  local materials = self.blackMarketInfo:GetMaterials()
  local hasDiamond = false
  local diamondResData = nil
  for k,v in pairs(materials) do
    local ownCount = 0
    if v.type == BaseResType.Item then
      ownCount = item_model.GetItemCountByItemID(v.id)
    else
      ownCount = game_model.GetBaseResourceValue(v.type)
      if v.type == BaseResType.Diamond then
        hasDiamond = true
        diamondResData = v
      end
    end
     local exchangeCost = v.count
    if self.needRisePrice then
      exchangeCost = self.blackMarketInfo:GetRisePrice()    
    end
    
    if  v.type == BaseResType.Item then
      if ownCount < exchangeCost then
        common_error_tip_view.Open(LocalizationController.instance:Get("ui.confirm_buy_item_tips_view.tip.notEnoughTitle"))
        return
      end
    else
       if not game_model.CheckBaseResEnoughByType(v.type,exchangeCost) then
        return 
      end
    end
    
   
  end
  local confirm_cost_tip_view = require('ui/tips/view/confirm_cost_tip_view')
  if hasDiamond and consume_tip_model.GetConsumeTipEnable(ConsumeTipType.DiamondBuyBlackmarket) then
      local exchangeCost = diamondResData.count
      if self.needRisePrice then
        exchangeCost = self.blackMarketInfo:GetRisePrice()    
      end
      local str = LocalizationController.instance:Get("ui.black_market_view.confirmbuy")
      confirm_cost_tip_view.Open(diamondResData.type,exchangeCost,str,function() self:ExchangeSureHandler() end,ConsumeTipType.DiamondBuyBlackmarket)
  else
    self:ExchangeSureHandler()
  end
  
end
function t:ExchangeSureHandler()
  self.onClick:InvokeOneParam(self.blackMarketInfo)
end

return t