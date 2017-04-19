local t = {}
local PREFAB_PATH = 'ui/hero_decompose/hero_decompose_view'

local common_hero_icon = require 'ui/common_icon/common_hero_icon'
local common_item_icon = require 'ui/common_icon/common_item_icon'
local global_data = gamemanager.GetData('global_data')
local hero_decomposition_data = gamemanager.GetData('hero_decomposition_data')
local item_data = gamemanager.GetData('item_data')
local hero_data = gamemanager.GetData('hero_data')
local game_model = gamemanager.GetModel('game_model')
local aggr_extra_exp_data = gamemanager.GetData('aggr_extra_exp_data')

local confirm_tip_view = require('ui/tips/view/confirm_tip_view')

function t.Open(heroInfo)
  uimanager.RegisterView(PREFAB_PATH,t)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  t.BindDelegate()
  t.InitComponent()
  
  t.heroItemCountByDecompose = 0
  t.SetHeroInfo(heroInfo)
end

function t.BindDelegate()
end

function t.UnbindDelegate()
end

function t.InitComponent()
  t.heroIconRoot = t.transform:Find('core/img_frame/img_inner_frame/hero_icon_root')
  t.heroNameText = t.transform:Find('core/img_frame/img_inner_frame/text_hero_name'):GetComponent(typeof(Text))
  t.toggleUseDiamond = t.transform:Find('core/img_frame/toggle_low_quality_never_remind'):GetComponent(typeof(Toggle))
  t.toggleUseDiamond.onValueChanged:AddListener(t.ClickToggleUseDiamondHandler)
  t.textToggleTip = ui_util.FindComp(t.toggleUseDiamond.transform,'Label',Text)
  t.cancelButton = t.transform:Find("core/img_frame/btn_cancel"):GetComponent(typeof(Button))
  t.cancelButton.onClick:AddListener(t.ClickCloseButtonHandler)
  t.confirmDecomposeButton = t.transform:Find("core/img_frame/btn_confirm_decompose"):GetComponent(typeof(Button))
  t.confirmDecomposeButton.onClick:AddListener(t.ClickConfirmDecomposeButtonHandler)

  t.textGainAward = ui_util.FindComp(t.transform,'core/img_frame/img_inner_frame/text_gain_award',Text)
end

function t.SetHeroInfo (heroInfo)
  t.heroInfo = heroInfo
  t.heroIcon = common_hero_icon.New(t.heroIconRoot)
  t.heroIcon:SetRoleInfo(t.heroInfo, false)
  t.heroNameText.text = LocalizationController.instance:Get(heroInfo.heroData.name)
  
  t.textToggleTip.text = string.format(LocalizationController.instance:Get('ui.hero_decompose_view.low_quality_never_remind_tips'),global_data.hero_decomposition_cost)
  
  t.RefreshGainAward()
end
function t.RefreshGainAward()
  t.heroItemCountByDecompose = 0
  local heroDecompositionData = hero_decomposition_data.GetCompositionDataByHeroIDAndStar (t.heroInfo.heroData.id, t.heroInfo.advanceLevel)
  local heroPieceGameResData = heroDecompositionData.heropiece
  local heroPieceItemData = item_data.GetDataById(heroPieceGameResData.id)
  --碎片
  local awardTip = LocalizationController.instance:Get('ui.hero_decompose_view.pieceAward')
  local tip = string.format(awardTip,LocalizationController.instance:Get(heroPieceItemData.name),heroPieceGameResData.count)
  
  
  if t.toggleUseDiamond.isOn then
    awardTip = LocalizationController.instance:Get('ui.hero_decompose_view.gainAward')
    --等级经验转化经验药水
    local expTotal = t.heroInfo:ExpTotal()
    print('exptotal',expTotal)
    local big = gamemanager.GetData('global_data').expSolutions[1]
    local mid = gamemanager.GetData('global_data').expSolutions[2]
    local small = gamemanager.GetData('global_data').expSolutions[3]
    local tempCount = math.floor(expTotal/big)
    local tempItem = nil
    if tempCount > 0 then
      tempItem = item_data.GetDataById(ITEM_ID.bigExpMedicine)
      tip = tip .. string.format(awardTip,LocalizationController.instance:Get(tempItem.name),tempCount)
    end
    expTotal = expTotal-tempCount* big
    tempCount = math.floor(expTotal/mid)
    if tempCount > 0 then
      tempItem = item_data.GetDataById(ITEM_ID.midExpMedicine)
      tip = tip .. string.format(awardTip,LocalizationController.instance:Get(tempItem.name),tempCount)
    end
    expTotal = expTotal-tempCount* mid
    tempCount = math.floor(expTotal/small)
    if tempCount > 0 then
      tempItem = item_data.GetDataById(ITEM_ID.smallExpMedicine)
      tip = tip .. string.format(awardTip,LocalizationController.instance:Get(tempItem.name),tempCount)
    end
    --强化经验转化羊
    awardTip = LocalizationController.instance:Get('ui.hero_decompose_view.pieceAward')
    local strengthenExpTotal = t.heroInfo:StrengthenExpTotal()
    print('strengthenExpTotal',strengthenExpTotal)
    big = aggr_extra_exp_data.GetDataById(ITEM_ID.bigExpSheep,1).exp
    mid = aggr_extra_exp_data.GetDataById(ITEM_ID.midExpSheep,1).exp
    small = aggr_extra_exp_data.GetDataById(ITEM_ID.smallExpSheep,1).exp
    local pieceCount = 20--整只羊变为碎片
    tempCount = math.floor(strengthenExpTotal/big)
    if tempCount > 0 then
      tempItem = hero_data.GetDataById(ITEM_ID.bigExpSheep)
      tip = tip .. string.format(awardTip,LocalizationController.instance:Get(tempItem.name),tempCount*pieceCount)
    end
    strengthenExpTotal = strengthenExpTotal - tempCount * big
    tempCount = math.floor(strengthenExpTotal/mid)
    if tempCount > 0 then
      tempItem = hero_data.GetDataById(ITEM_ID.midExpSheep)
      tip = tip .. string.format(awardTip,LocalizationController.instance:Get(tempItem.name),tempCount*pieceCount)
    end
    strengthenExpTotal = strengthenExpTotal - tempCount * mid
    tempCount = math.floor(strengthenExpTotal/small)
    if tempCount > 0 then
      tempItem = hero_data.GetDataById(ITEM_ID.smallExpSheep)
      tip = tip .. string.format(awardTip,LocalizationController.instance:Get(tempItem.name),tempCount*pieceCount)
    end
    
  end
  --去除回车建
  tip = string.sub(tip,0,-2)
  t.textGainAward.text = tip
  
  if t.toggleUseDiamond.isOn then
    t.confirmDecomposeButton.transform:Find('diamond_root').gameObject:SetActive(true)
    t.confirmDecomposeButton.transform:Find('text_confirm_decompose').gameObject:SetActive(false)
    t.confirmDecomposeButton.transform:Find('diamond_root/text_diamond'):GetComponent(typeof(Text)).text = global_data.hero_decomposition_cost
  else
    t.confirmDecomposeButton.transform:Find('diamond_root').gameObject:SetActive(false)
    t.confirmDecomposeButton.transform:Find('text_confirm_decompose').gameObject:SetActive(true)
  end
end

function t.Close()
  t.transform = nil
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
end
-----------------------------click event----------------------------
function t.ClickCloseButtonHandler ()
  t.Close()
end
function t.ClickToggleUseDiamondHandler(value)
  
  t.RefreshGainAward()
end
function t.ClickConfirmDecomposeButtonHandler ()
  t.Close()
  --[[if t.toggleUseDiamond.isOn then
    if t.heroItemCountByDecompose > 0 and game_model.CheckPackFull(true,false,t.heroItemCountByDecompose) then
      return
    end
    if not game_model.CheckBaseResEnoughByType(BaseResType.Diamond,global_data.hero_decomposition_cost) then
      return 
    end
  end]]
  gamemanager.GetCtrl('hero_controller').HeroDeComposeReq (t.heroInfo.instanceID,t.toggleUseDiamond.isOn)
end

return t