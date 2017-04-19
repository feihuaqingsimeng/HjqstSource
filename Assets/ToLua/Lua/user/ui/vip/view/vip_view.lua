local t = {}
local PREFAB_PATH = 'ui/vip/vip_view'

local vipController = gamemanager.GetCtrl('vip_controller')
local vipModel = gamemanager.GetModel('vip_model')
local vipData = gamemanager.GetData('vip_data')
local commonRewardIcon = require('ui/common_icon/common_reward_icon')
local auto_destroy_tip_view = require('ui/tips/view/auto_destroy_tip_view')
local confirm_tip_view = require('ui/tips/view/confirm_tip_view')

t.currentVIPLevel = 0

function t.Open ()
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
 
  local common_top_bar = require ('ui/common_top_bar/common_top_bar')
  common_top_bar = common_top_bar:Create(t.transform:Find('core'))
  common_top_bar.transform:SetAsFirstSibling()
  common_top_bar:SetAsCommonStyle(Common.Localization.LocalizationController.instance:Get('ui.vip_view.title'), t.ClickCloseHandler, true, true, true, true, false, false, false)
  
  t.currentVIPLevel = vipModel.vipLevel
  
  t.InitComponent()
  t.BindDelegate()
  t.RefreshMyVIPInfo()
  t.RefreshVIPDetail()
  t.RefreshPreUnreceivedGiftTipIcon ()
end

function t.Close ()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
end

function t.BindDelegate ()
  vipModel.OnVIPInfoUpdateDelegate:AddListener(t.OnVIPInfoUpdateHandler)
  vipModel.OnDrawVIPBenefitsSuccessDelegate:AddListener(t.OnDrawVIPBenefitsSuccessHandler)
end

function t.UnbindDelegate ()
  vipModel.OnVIPInfoUpdateDelegate:RemoveListener(t.OnVIPInfoUpdateHandler)
  vipModel.OnDrawVIPBenefitsSuccessDelegate:RemoveListener(t.OnDrawVIPBenefitsSuccessHandler)
end

function t.InitComponent()
  t.myVIPLevelText = t.transform:Find('core/img_frame/img_top_frame/img_vip_badge/text_vip_level'):GetComponent(typeof(Text))
  t.myVIPExpText = t.transform:Find('core/img_frame/img_top_frame/text_vip_exp'):GetComponent(typeof(Text))
  t.myVIPExpSlider = t.transform:Find('core/img_frame/img_top_frame/slider_vip_exp'):GetComponent(typeof(Slider))

  t.vipDetailContent = t.transform:Find('core/img_frame/img_frame/img_vip_detail_mask/Content')
  t.vipPrivilegeDetailItemPrefab = t.transform:Find('core/img_frame/img_frame/img_vip_detail_mask/Content/vip_privilege_detail_item').gameObject
  t.vipPrivilegeDetailItemPrefab:SetActive(false)

  t.conditionToNextVIPLevelText = t.transform:Find('core/img_frame/img_top_frame/text_condition_to_next_vip_level'):GetComponent(typeof(Text))
  t.vipDetailTitleText = t.transform:Find('core/img_frame/img_frame/img_banner/text_vip_detail_title'):GetComponent(typeof(Text))
  t.buyDiamondButton = t.transform:Find('core/img_frame/img_top_frame/btn_buy_diamond'):GetComponent(typeof(Button))
  t.buyDiamondButton.onClick:AddListener(t.ClickBuyDiamondHandler)
  
  t.previousVIPDetailButton = t.transform:Find('core/img_frame/img_frame/btn_previous_vip'):GetComponent(typeof(Button))
  t.previousVIPDetailButton.onClick:AddListener(t.ClickPreviousVIPLevelDetailHandler)
  t.preUnreceivedGiftTipIcon = t.transform:Find('core/img_frame/img_frame/btn_previous_vip/pre_unreceived_gift_tip_icon')
  t.nextVIPDetailButton = t.transform:Find('core/img_frame/img_frame/btn_next_vip'):GetComponent(typeof(Button))
  t.nextVIPDetailButton.onClick:AddListener(t.ClickNextVIPLevelDetailHandler)
  
  t.VIPBenefitsRootGameObject = t.transform:Find('core/img_frame/img_bottom_frame/benefits_root').gameObject

  t.vipBenefitsItemsRoot = t.transform:Find('core/img_frame/img_bottom_frame/benefits_root/benefit_items')
  t.hasReceivedGO = t.transform:Find('core/img_frame/img_bottom_frame/benefits_root/text_has_received').gameObject
  t.drawVIPBenefitsButton = t.transform:Find('core/img_frame/img_bottom_frame/benefits_root/btn_draw_vip_benefits'):GetComponent(typeof(Button))
  t.drawVIPBenefitsButton.onClick:AddListener(t.ClickDrawVIPBenefits)
  
  local vipPrivilegeDetailItem = nil
  for k, v in pairs(vipData.GetAllVIPDataList()) do
    vipPrivilegeDetailItem = Instantiate(t.vipPrivilegeDetailItemPrefab)
    vipPrivilegeDetailItem.transform:Find('Viewport/Content/text_vip_detail'):GetComponent(typeof(Text)).text = Common.Localization.LocalizationController.instance:Get(v.des)
    vipPrivilegeDetailItem.transform:SetParent(t.vipDetailContent, false)
    vipPrivilegeDetailItem:SetActive(true)
    vipPrivilegeDetailItem = nil
  end
end

function t.RefreshMyVIPInfo ()
  local currentVIPData = vipData.GetVIPData (vipModel.vipLevel)
  t.myVIPLevelText.text = vipModel.vipLevel
  local vipExp = vipModel.totalRecharge * 10
  
  if currentVIPData ~= nil then
    if not currentVIPData:IsMaxLevel() then
      local nextVipData = currentVIPData:GetNextLevelVIPData()
      t.myVIPExpText.text = string.format("%s/%s", vipExp, nextVipData.exp * 10) 
      t.myVIPExpSlider.value = vipModel.totalRecharge / nextVipData.exp
      t.myVIPExpSlider.gameObject:SetActive(true)
      t.conditionToNextVIPLevelText.text = string.format(Common.Localization.LocalizationController.instance:Get('ui.condition_to_next_vip_level'), (nextVipData.exp - vipModel.totalRecharge) * 10, Common.Localization.LocalizationController.instance:Get(nextVipData.name))
      t.conditionToNextVIPLevelText.gameObject:SetActive(true)
    else
      t.myVIPExpText.text = vipModel.totalRecharge * 10
      t.myVIPExpSlider.gameObject:SetActive(false)
      t.conditionToNextVIPLevelText.gameObject:SetActive(false)
    end
  end
end

function t.RefreshVIPDetail ()
  local currentVIPData = vipData.GetVIPData (t.currentVIPLevel)
  t.vipDetailTitleText.text = Common.Localization.LocalizationController.instance:Get(currentVIPData.name)
  
  t.previousVIPDetailButton.gameObject:SetActive(not currentVIPData:IsMinLevel())
  t.nextVIPDetailButton.gameObject:SetActive(not currentVIPData:IsMaxLevel())
  
  local prefabWidth = t.vipPrivilegeDetailItemPrefab:GetComponent(typeof(LayoutElement)).preferredWidth
  LeanTween.moveLocalX(t.vipDetailContent.gameObject, (-t.currentVIPLevel - 0.5)* prefabWidth, 0.25)
  
  Common.Util.TransformUtil.ClearChildren(t.vipBenefitsItemsRoot, true)
  for k, v in pairs(currentVIPData:GetBenefitItems()) do
    commonRewardIcon.New(t.vipBenefitsItemsRoot, v)
  end
  t.VIPBenefitsRootGameObject:SetActive(table.count(currentVIPData:GetBenefitItems()) > 0)
  t.hasReceivedGO:SetActive(vipModel.HasReceivedGift(t.currentVIPLevel))
  t.drawVIPBenefitsButton.gameObject:SetActive(not vipModel.HasReceivedGift(t.currentVIPLevel))
end

function t.RefreshPreUnreceivedGiftTipIcon ()
  local hasUnreceivedGiftBefore = vipModel.HasUnreceivedGiftBefore(t.currentVIPLevel)
  t.preUnreceivedGiftTipIcon.gameObject:SetActive(hasUnreceivedGiftBefore)
end

-- [[ Proxy callback ]] --
function t.OnVIPInfoUpdateHandler ()
  t.RefreshMyVIPInfo ()
  t.RefreshVIPDetail ()
  t.RefreshPreUnreceivedGiftTipIcon()
end

function t.OnDrawVIPBenefitsSuccessHandler (vipLevel)
  local vData = vipData.GetVIPData (vipLevel)
  local vipName = Common.Localization.LocalizationController.instance:Get(vData.name)
  local tips = string.format(Common.Localization.LocalizationController.instance:Get('ui.vip_view.draw_vip_benefit_success_tips'), vipName)
  auto_destroy_tip_view.Open(tips)
  
    
  local vipLevel = vipModel.GetFirstUnreceivedGiftLevel()
  if vipLevel == -1 then
    vipLevel = vipModel.vipLevel
  end
  t.currentVIPLevel = vipLevel
  t.RefreshVIPDetail ()
  t.RefreshPreUnreceivedGiftTipIcon()
end
-- [[ Proxy callback ]] --

-- [[ UI event handlers ]] --
function t.ClickCloseHandler ()
  t.Close()
end

function t.ClickBuyDiamondHandler ()
  gamemanager.GetCtrl('shop_controller').OpenShopView(3)
  t.Close()
end

function t.ClickPreviousVIPLevelDetailHandler ()
  t.currentVIPLevel = t.currentVIPLevel - 1
  t.RefreshVIPDetail()
  t.RefreshPreUnreceivedGiftTipIcon()
end

function t.ClickNextVIPLevelDetailHandler ()
  t.currentVIPLevel = t.currentVIPLevel + 1
  t.RefreshVIPDetail()
  t.RefreshPreUnreceivedGiftTipIcon()
end

function t.ClickDrawVIPBenefits ()
  if vipModel.vipLevel < t.currentVIPLevel then
    confirm_tip_view.Open(Common.Localization.LocalizationController.instance:Get('ui.vip_view.vip_level_not_enough_tips'), t.ClickBuyDiamondHandler)
    return
  end
  vipController.VipGiftBagReq(t.currentVIPLevel)
end
-- [[ UI event handlers ]] --
return t
