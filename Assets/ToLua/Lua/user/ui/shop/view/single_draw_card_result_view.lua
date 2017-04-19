local t = {}
local PREFAB_PATH = 'ui/shop/single_draw_card_result_view'
local shopController = gamemanager.GetCtrl('shop_controller')
local uiUtil = require('util/ui_util')
local common_card = require('ui/shop/view/common_card')

function t.Open (shopItemInfo, commonCardInfo)
  t.gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Replace)
  t.InitComponent ()
  t.SetInfo(shopItemInfo, commonCardInfo)
  
  LeanTween.delayedCall(0.5, Action(t.OnViewReady))
end

function t.OpenAsPurchaseDrawAwardResultView (commonCardInfo)
  t.gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Replace)
  t.InitComponent ()
  t.SetInfo(nil, commonCardInfo)
  
  t.drawAgainButton.gameObject:SetActive(false)
  
  LeanTween.delayedCall(0.5, Action(t.OnViewReady))
end

function t.Close()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end

function t.OnViewReady ()
  Observers.Facade.Instance:SendNotification(string.format('%s::%s', PREFAB_PATH, 'OnViewReady'))
end

function t.InitComponent ()
  t.coreTransform = t.gameObject.transform:Find('core')
  
  t.commonCardPrefab = t.coreTransform:Find('common_card')
  t.commonCardPrefab.gameObject:SetActive(false)
  t.commonCardRoot = t.coreTransform:Find('common_card_root')
  
  t.closeButton = t.coreTransform:Find('bottom_bar/buttons_root/btn_back'):GetComponent(typeof(Button))
  t.closeButton.onClick:RemoveAllListeners()
  t.closeButton.onClick:AddListener(t.ClickCloseHandler)
  
  t.drawAgainCostImage = t.coreTransform:Find('bottom_bar/buttons_root/btn_again/img_cost_icon'):GetComponent(typeof(Image))
  t.drawAgainText = t.coreTransform:Find('bottom_bar/buttons_root/btn_again/text'):GetComponent(typeof(Text))
  t.drawAgainButton = t.coreTransform:Find('bottom_bar/buttons_root/btn_again'):GetComponent(typeof(Button))
  t.drawAgainButton.onClick:RemoveAllListeners()
  t.drawAgainButton.onClick:AddListener(t.ClickDrawAgainHandler)
  
end

function t.SetInfo (shopItemInfo, commonCardInfo)
  t.shopItemInfo = shopItemInfo
  t.commonCardInfo = commonCardInfo
  
  Common.Util.TransformUtil.ClearChildren(t.commonCardRoot, true)
  local commonCardObj = Instantiate(t.commonCardPrefab)
  commonCardObj:SetParent(t.commonCardRoot, false)
  commonCardObj.localPosition = Vector3(0, 0, 0)
  local commonCard = common_card.NewByGameObject(commonCardObj.gameObject, t.commonCardInfo)
  commonCard:TurnOverAfter(0)
  commonCardObj.gameObject:SetActive(true)
  
  local showNewHero = false
  if t.commonCardInfo.baseResType == BaseResType.Hero then
    showNewHero = gamemanager.GetCtrl('hero_controller').ShowNewHero (t.commonCardInfo.heroData.id, t.commonCardInfo.advanceLevel, t.OnViewStay, true, false)
  end
  
  if not showNewHero then
    t.OnViewStay()
  end
  
  if t.shopItemInfo ~= nil then
    t.drawAgainCostImage.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(uiUtil.GetBaseResIconPath(t.shopItemInfo:GetCostResData().type))
    t.drawAgainCostImage:SetNativeSize()
    local discount = 1
    if gamemanager.GetModel('shop_model').IsShopDiscountOpen() then
      discount = t.shopItemInfo.shopCardRandomData.discount
    end
    t.drawAgainText.text = string.format(LocalizationController.instance:Get('ui.recruit_single_hero_result_view.again'), math.floor(t.shopItemInfo:GetCostResData().count * discount))
  end
end

function t.OnViewStay ()
  Observers.Facade.Instance:SendNotification(string.format('%s::%s', PREFAB_PATH, 'OnViewStay'))
  print('ui/shop/single_draw_card_result_view::OnViewStay')
end

-- [[ UI event handlers ]] --
function t.ClickCloseHandler ()
  t.Close()
end

function t.ClickDrawAgainHandler ()
  shopController.TryPurchaseGoods(t.shopItemInfo)
end
-- [[ UI event handlers ]] --

return t