local t = {}
local PREFAB_PATH = 'ui/hero_breakthrough/hero_breakthrough_view'

local player_controller = gamemanager.GetCtrl('player_controller')
local hero_controller = gamemanager.GetCtrl('hero_controller')
local player_model = gamemanager.GetModel('player_model')
local player_info = require('ui/player/model/player_info')
local hero_model = gamemanager.GetModel('hero_model')
local role_model = gamemanager.GetModel('role_model')
local game_model = gamemanager.GetModel('game_model')
local item_model = gamemanager.GetModel('item_model')
local common_hero_icon = require('ui/common_icon/common_hero_icon')
local common_item_icon = require('ui/common_icon/common_item_icon')
local breakthrough_data = gamemanager.GetData('breakthrough_data')
local common_error_tips_view = require('ui/tips/view/common_error_tips_view')
local auto_destroy_tip_view = require('ui/tips/view/auto_destroy_tip_view')

function t.Open(heroInstanceId)
  uimanager.RegisterView(PREFAB_PATH,t)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
  --init attr
  t.isPlayer = game_model.IsPlayer(heroInstanceId)
  if t.isPlayer then
    t.roleInfo = game_model.playerInfo
  else
    t.roleInfo = hero_model.GetHeroInfo(heroInstanceId)
  end
  t.materialHeroInfoList = {}
  t.curHeroIcon = nil
  t.materialHeroIcon = nil
  t.itemIconMaterials = {}
  
  local common_top_bar = require ('ui/common_top_bar/common_top_bar')
  common_top_bar = common_top_bar:Create(t.transform:Find('core'))
  local title = ''
  if t.isPlayer then
    title = LocalizationController.instance:Get("ui.hero_breakthrough_view.player_breakthrough_title")
  else
    title = LocalizationController.instance:Get("ui.hero_breakthrough_view.hero_breakthrough_title")
  end
  common_top_bar:SetAsCommonStyle(title,t.OnClickBackBtnHandler,true,true,true,false,false,false,false)
  
  t.BindDelegate()
  t.InitComponent()
  t.Refresh()
end

function t.Close()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
end

function t.BindDelegate()
  game_model.onUpdateBaseResourceDelegate:AddListener(t.OnBaseResourceUpdateHandler)
  item_model.updateItemInfoListDelegate:AddListener(t.OnItemsUpdateHandler)
  player_model.OnPlayerBreakthroughSuccessDelegate:AddListener(t.OnBreakthroughSuccessHandler)
  hero_model.onHeroBreakthroughSuccessDelegate:AddListener(t.OnBreakthroughSuccessHandler)
end

function t.UnbindDelegate()
  game_model.onUpdateBaseResourceDelegate:RemoveListener(t.OnBaseResourceUpdateHandler)
  item_model.updateItemInfoListDelegate:RemoveListener(t.OnItemsUpdateHandler)
  player_model.OnPlayerBreakthroughSuccessDelegate:RemoveListener(t.OnBreakthroughSuccessHandler)
  hero_model.onHeroBreakthroughSuccessDelegate:RemoveListener(t.OnBreakthroughSuccessHandler)
end

function t.InitComponent()
  local left = t.transform:Find('core/left_part')
  t.roleIconLeftAnchor = left:Find('breakthrough_role_icon_left_anchor')
  t.roleIconMiddleAnchor = left:Find('breakthrough_role_icon_middle_anchor')
  t.tranCurIconRoot = left:Find('img_main_avatar_frame')
  t.tranMaterialRoot = left:Find('btn_material_icon')
  local BtnMaterial = t.tranMaterialRoot:GetComponent(typeof(Button))
  BtnMaterial.onClick:AddListener(t.ClickEmptyMaterialBtnHandler)
  t.breakthroughMaterialItemIconImage = t.tranMaterialRoot:Find('img_breakthrough_material_item_icon'):GetComponent(typeof(Image))
  t.breakthroughMaterialItemCountText = t.tranMaterialRoot:Find('text_breakthrough_material_item_count'):GetComponent(typeof(Text))
  t.breakthroughMaterialItemNameText = t.tranMaterialRoot:Find('text_breakthrough_material_item_name'):GetComponent(typeof(Text))
  
  t.tranInfoFrame = left:Find('img_attributes_frame/breakthrough_info_root')
  t.textCurHeroMaxLv = t.tranInfoFrame:Find('text_current_hero_max_level'):GetComponent(typeof(Text))
  t.textSuccessResult = t.tranInfoFrame:Find('text_breakthrough_success_result'):GetComponent(typeof(Text))
  t.textMoney = t.tranInfoFrame:Find('text_gold'):GetComponent(typeof(Text))
  t.btnBreakThrougth = t.tranInfoFrame:Find('btn_breakthrough'):GetComponent(typeof(Button))
  t.btnBreakThrougth.onClick:AddListener(t.ClickBreakThroughHandler)
  t.textAttrTitle = left:Find('img_attributes_frame/img_attributes_title_bg/text_attributes_title'):GetComponent(typeof(Text))
  t.goReachMaxTip = left:Find('img_attributes_frame/text_level_reach_max').gameObject
  
  t.scrollContent = t.transform:Find('core/img_big_frame/scrollrect/content'):GetComponent(typeof(Common.UI.Components.ScrollContent))
  t.scrollContent.onResetItem:AddListener(t.ResetItemHandler)
  
  t.goNoneMaterialTip = t.transform:Find('core/img_big_frame/text_no_available_material').gameObject
end

function t.Refresh()
  --cur icon
  if t.curHeroIcon == nil then
    t.curHeroIcon = common_hero_icon.CreateBig(t.tranCurIconRoot)
  end
  t.curHeroIcon:SetRoleInfo(t.roleInfo,t.isPlayer)
  if t.isPlayer then
    t.curHeroIcon:UsePetIcon()
  end
  --is max
  local nextData = breakthrough_data.GetNextBreakthroughDataByRoleInfo(t.roleInfo)
  t.tranMaterialRoot.gameObject:SetActive(nextData ~= nil)
  if nextData == nil then
    t.tranCurIconRoot.localPosition = t.roleIconMiddleAnchor.localPosition
  else
    t.tranCurIconRoot.localPosition = t.roleIconLeftAnchor.localPosition
  end
  --refresh
  t.RegenerateUsableMaterialHeroButtons()
  t.RefreshMaterial()
  t.RefreshBreakthroughInfo()
end

function t.RegenerateUsableMaterialHeroButtons()
  t.breakthroughDataList = breakthrough_data.GetBreakthroughDataListByQuality(t.roleInfo.heroData.quality)
  t.scrollContent:Init(#t.breakthroughDataList, false, 0)
  t.goNoneMaterialTip:SetActive(false)
end

function t.RefreshScrollContent()
  t.scrollContent:RefreshAllContentItems()
end

function t.RefreshMaterial()
  local nextBreakthroughData = breakthrough_data.GetNextBreakthroughDataByRoleInfo(t.roleInfo)
  
  if nextBreakthroughData == nil then
    return
  end
  
  local costItemGameResData = nextBreakthroughData.costItemGameResData
  local costItemInfo = item_model.GetItemInfoWithoutNilByItemId(costItemGameResData.id)
  
  t.breakthroughMaterialItemIconImage.sprite = ResMgr.instance:LoadSprite(costItemInfo.itemData:IconPath())
  local materialCountString = string.format(LocalizationController.instance:Get('common.value/max_lua') ,costItemInfo.count, costItemGameResData.count)
  if costItemInfo.count < costItemGameResData.count then
    materialCountString = string.format(LocalizationController.instance:Get('common.red_text_template_lua'), materialCountString)
  else
    materialCountString = string.format(LocalizationController.instance:Get('common.green_text_template_lua'), materialCountString)
  end
  t.breakthroughMaterialItemCountText.text = materialCountString
  t.breakthroughMaterialItemNameText.text = ui_util.FormatStringWithinQualityColor(costItemInfo.itemData.quality, LocalizationController.instance:Get(costItemInfo.itemData.name))
end

function t.RefreshBreakthroughInfo()
  local nextData = breakthrough_data.GetNextBreakthroughDataByRoleInfo(t.roleInfo)
  if nextData == nil then
    t.tranInfoFrame.gameObject:SetActive(false)
    t.goReachMaxTip.gameObject:SetActive(true)
    t.textAttrTitle.text = LocalizationController.instance:Get("ui.hero_breakthrough_view.breakthrough_intoduction_title.breakthrough_level_reach_max")
    return
  end
  t.tranInfoFrame.gameObject:SetActive(true) 
  t.goReachMaxTip.gameObject:SetActive(false)
  t.textAttrTitle.text = LocalizationController.instance:Get("ui.hero_breakthrough_view.breakthrough_intoduction_title")
  
  --cur level
  local breakthroughMaxLevel = breakthrough_data.GetDataById(t.roleInfo.breakthroughLevel).levelMax
  local str = ''
  if t.isPlayer  then
    str = LocalizationController.instance:Get("ui.hero_breakthrough_view.current_player_max_level")
  else
    str = LocalizationController.instance:Get("ui.hero_breakthrough_view.current_hero_max_level")
  end
  t.textCurHeroMaxLv.text = string.format(str,breakthroughMaxLevel)
  --next level
  local nextHeroMaxLevel = 0
  
  if nextData ~= nil then
    nextHeroMaxLevel = nextData.levelMax
  end
  if t.isPlayer then
    str = LocalizationController.instance:Get("ui.hero_breakthrough_view.breakthrough_success_result_player")
  else
    str = LocalizationController.instance:Get("ui.hero_breakthrough_view.breakthrough_success_result")
  end
  t.textSuccessResult.text = string.format(str,nextHeroMaxLevel)
  --money
  local money = 0
  if nextData ~= nil then
    money = nextData.costGoldGameResData.count
  end
  local ownMoney = game_model.GetBaseResourceValue(BaseResType.Gold)
  if ownMoney < money then
    t.textMoney.text = ui_util.FormatToRedText(tostring(money))
  else
    t.textMoney.text = ui_util.FormatToGreenText(tostring(money))
  end
end
-----------------------click event-----------------------
function t.OnClickBackBtnHandler()
  t.Close()
end
function t.ResetItemHandler(go,index)
  if t.itemIconMaterials[go] == nil then
    t.itemIconMaterials[go] = common_item_icon.NewByGameObject(go)
    t.itemIconMaterials[go].onClick:AddListener(t.ClickItemHandler)
  end
  
  local costItemGameResData = t.breakthroughDataList[index + 1].costItemGameResData
  local costItemInfo = item_model.GetItemInfoWithoutNilByItemId(costItemGameResData.id)
  t.itemIconMaterials[go]:SetItemInfo(costItemInfo)
  t.itemIconMaterials[go]:SetSelect(t.roleInfo.breakthroughLevel == t.breakthroughDataList[index + 1].layer - 1)
  t.itemIconMaterials[go]:ShowCount(true)
end

function t.ClickEmptyMaterialBtnHandler()
  local nextBreakthroughData = breakthrough_data.GetNextBreakthroughDataByRoleInfo(t.roleInfo)
  if nextBreakthroughData == nil then
    return
  end
  
  local costItemGameResData = nextBreakthroughData.costItemGameResData
  LuaInterface.LuaCsTransfer.OpenGoodsJumpPath(costItemGameResData.type, costItemGameResData.id, 0)
end

function t.ClickMaterialBtnHandler(heroIcon)
  t.RefreshMaterial()
  t.RefreshScrollContent()
end

function t.ClickItemHandler(itemIcon)
  local nextBreakthroughData = breakthrough_data.GetNextBreakthroughDataByRoleInfo(t.roleInfo)
  if nextBreakthroughData == nil then
    auto_destroy_tip_view.Open(LocalizationController.instance:Get('ui.hero_breakthrough_view.tips.breakthrough_level_reach_max_cant_select_material'))
    return
  end
  
  local costItemGameResData = nextBreakthroughData.costItemGameResData

  if itemIcon.itemInfo.itemData.id ~= costItemGameResData.id then
    auto_destroy_tip_view.Open(LocalizationController.instance:Get('ui.hero_breakthrough_view.tips.not_enough_breakthrough_level'))
  end
end

function t.ClickBreakThroughHandler()
  if not hero_model.IsHeroCanBreakthrough(t.roleInfo, t.isPlayer, true, true) then
    return
  end
  
  local costIndex = 0
  if t.isPlayer then
    player_controller.PlayerBreakReq(costIndex)
  else
    hero_controller.HeroBreakReq(t.roleInfo.instanceID, costIndex)
  end
end

--------------------------update by protocol---------------
function t.OnBaseResourceUpdateHandler ()
  t.RefreshBreakthroughInfo()
end

function t.OnItemsUpdateHandler ()
  t.RefreshScrollContent()
  t.RefreshMaterial()
end

function t.OnBreakthroughSuccessHandler(roleInstanceID)
  -- [[SHOW ROLE EXHIBITION VIEW]] --
  local roleInfo = nil
  if gamemanager.GetModel('game_model').IsPlayer(roleInstanceID) then
    roleInfo = gamemanager.GetModel('game_model').playerInfo
  else
    roleInfo = gamemanager.GetModel('hero_model').GetHeroInfo(roleInstanceID)
  end
  local role_exhibition_view = require 'ui/role_exhibition/view/role_exhibition_view'
  role_exhibition_view.Open(roleInfo, role_exhibition_view.ExhibitionType.Breakthrough)
  -- [[SHOW ROLE EXHIBITION VIEW]] --
  t.Refresh()
end
return t
  