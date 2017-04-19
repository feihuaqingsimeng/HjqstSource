local t = {}
local PREFAB_PATH = 'ui/shop/ten_draw_card_result_view'
local shopController = gamemanager.GetCtrl('shop_controller')
local uiUtil = require('util/ui_util')
local common_card = require('ui/shop/view/common_card')

function t.Open (shopItemInfo, commonCardInfoList, specialCommonCardInfo)
  t.gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Replace)
  t.InitComponent ()
  t.SetInfo(shopItemInfo, commonCardInfoList, specialCommonCardInfo)
end

function t.Close()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end

function t.InitComponent ()
  t.coreTransform = t.gameObject.transform:Find('core')
  
  t.commonCardPrefab = t.coreTransform:Find('common_card')
  t.commonCardPrefab.gameObject:SetActive(false)
  t.commonCardRoots = {}
  for i = 1, 10 do
    t.commonCardRoots[i] = t.coreTransform:Find('common_cards_root/card_'..i)
  end
  t.commonCardRoots['gift'] = t.coreTransform:Find('common_cards_root/card_gift')
  
  --t.bottom_buttons_root = t.coreTransform:Find('bottom_bar/buttons_root')
  --t.bottom_buttons_root.gameObject:SetActive(false)
  
  t.closeButton = t.coreTransform:Find('bottom_bar/buttons_root/btn_back'):GetComponent(typeof(Button))
  t.closeButton.onClick:RemoveAllListeners()
  t.closeButton.onClick:AddListener(t.ClickCloseHandler)
  
  t.turnOverAllButton = t.coreTransform:Find('bottom_bar/buttons_root/btn_turn_over_all'):GetComponent(typeof(Button))
  t.turnOverAllButton.onClick:RemoveAllListeners()
  t.turnOverAllButton.onClick:AddListener(t.ClickTurnOverAllHandler)
  
  t.drawAgainCostImage = t.coreTransform:Find('bottom_bar/buttons_root/btn_again/img_cost_icon'):GetComponent(typeof(Image))
  t.drawAgainText = t.coreTransform:Find('bottom_bar/buttons_root/btn_again/text'):GetComponent(typeof(Text))
  t.drawAgainButton = t.coreTransform:Find('bottom_bar/buttons_root/btn_again'):GetComponent(typeof(Button))
  t.drawAgainButton.onClick:RemoveAllListeners()
  t.drawAgainButton.onClick:AddListener(t.ClickDrawAgainHandler)
  
  t.closeButton.gameObject:SetActive(false)
  t.turnOverAllButton.gameObject:SetActive(true)
  t.drawAgainButton.gameObject:SetActive(false)
end

function t.SetInfo (shopItemInfo, commonCardInfoList, specialCommonCardInfo)
  t.shopItemInfo = shopItemInfo
  t.commonCardInfoList = commonCardInfoList
  t.specialCommonCardInfo = specialCommonCardInfo
  
  t.commonCardList = {}
  local commonCardObj = nil
  local commonCard = nil
  local commonCardInfo = nil
  for i = 1, 10 do
    Common.Util.TransformUtil.ClearChildren(t.commonCardRoots[i], true)
    commonCardObj = Instantiate(t.commonCardPrefab)
    commonCardObj:SetParent(t.commonCardRoots[i], false)
    commonCardObj.localPosition = Vector3(0, 0, 0)
    commonCard = common_card.NewByGameObject(commonCardObj.gameObject, t.commonCardInfoList[i])
    --commonCard:TurnOverAfter(i * 0.25)
    commonCard.OnTurnOverCompleteDelegate:AddListener(t.OnTurnOverCompleteHandler)
    commonCard.OnShowCompleteDelegate:AddListener(t.OnShowCompleteHandler)
    table.insert(t.commonCardList, commonCard)
    commonCardObj.gameObject:SetActive(true)
    commonCardObj = nil
    commonCard = nil
  end
  
  Common.Util.TransformUtil.ClearChildren(t.commonCardRoots['gift'], true)
  commonCardObj = Instantiate(t.commonCardPrefab)
  commonCardObj:SetParent(t.commonCardRoots['gift'], false)
  commonCardObj.localPosition = Vector3(0, 0, 0)
  commonCard = common_card.NewByGameObject(commonCardObj.gameObject, t.specialCommonCardInfo)
  --commonCard:TurnOverAfter (11 * 0.25)
  commonCard.OnTurnOverCompleteDelegate:AddListener(t.OnTurnOverCompleteHandler)
  commonCard.OnShowCompleteDelegate:AddListener(t.OnShowCompleteHandler)
  table.insert(t.commonCardList, commonCard)
  commonCardObj.gameObject:SetActive(true)
  commonCardObj = nil
  commonCard = nil
  
  t.drawAgainCostImage.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(uiUtil.GetBaseResIconPath(t.shopItemInfo:GetCostResData().type))
  t.drawAgainCostImage:SetNativeSize()
  local discount = 1
  if gamemanager.GetModel('shop_model').IsShopDiscountOpen() then
    discount = t.shopItemInfo.shopCardRandomData.discount
  end
  t.drawAgainText.text = string.format(LocalizationController.instance:Get('ui.recruit_single_hero_result_view.again'), math.floor(t.shopItemInfo:GetCostResData().count * discount))
end

function t.OnTurnOverCompleteHandler (commonCardInfo)
  if commonCardInfo.baseResType == BaseResType.Hero then
    --gamemanager.GetModel('fight_result_model').OpenFightResultHeroDisplayView(commonCardInfo.heroData.id, commonCardInfo.advanceLevel, t.OnHeroDisplayViewCloseHandler)
    
    local showNewHero = gamemanager.GetCtrl('hero_controller').ShowNewHero (commonCardInfo.heroData.id, commonCardInfo.advanceLevel, t.OnHeroDisplayViewCloseHandler, true, false)
    if not showNewHero then
      t.OnHeroDisplayViewCloseHandler()
    end
  else
    if t.autoTurnOverAll == true then
      t.AutoTunrOverNextCard(0)
    end
  end
end

function t.OnShowCompleteHandler ()
  for k, v in ipairs(t.commonCardList) do
    if not v.isTurnOverFinished then
      return
    end
  end
  t.closeButton.gameObject:SetActive(true)
  t.turnOverAllButton.gameObject:SetActive(false)
  t.drawAgainButton.gameObject:SetActive(true)
end

-- [[ UI event handlers ]] --
function t.ClickCloseHandler ()
  t.Close()
end

function t.ClickTurnOverAllHandler ()
  t.autoTurnOverAll = true
  t.turnOverAllButton.gameObject:SetActive(false)
  --[[
  local delay = 0
  for k, v in ipairs(t.commonCardList) do
    if not v.isTurnOverStarted then
      v:TurnOverAfter(delay)
      delay = delay + 0.25
    end
  end
  ]]--
  for k, v in ipairs(t.commonCardList) do
    v.clickable = false
  end
  t.AutoTunrOverNextCard(0)
end

function t.AutoTunrOverNextCard (delay)
  for k, v in ipairs(t.commonCardList) do
    if not v.isTurnOverStarted then
      v:TurnOverAfter(delay)
      v.turnOverbutton.gameObject:SetActive(false)
      return
    end
  end
end

function t.OnHeroDisplayViewCloseHandler ()
    if t.autoTurnOverAll == true then
      t.AutoTunrOverNextCard(0)
    end
end

function t.ClickDrawAgainHandler ()
  shopController.TryPurchaseGoods(t.shopItemInfo)
end
-- [[ UI event handlers ]] --

return t