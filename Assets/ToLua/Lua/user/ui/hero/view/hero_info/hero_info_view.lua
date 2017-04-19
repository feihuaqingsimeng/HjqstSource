local t = {}
local PREFAB_PATH = 'ui/role/role_info_view'
local name = PREFAB_PATH
local hero_controller = gamemanager.GetCtrl('hero_controller')
local player_model = gamemanager.GetModel('player_model')
local hero_model = gamemanager.GetModel('hero_model')
local role_model = gamemanager.GetModel('role_model')
local game_model = gamemanager.GetModel('game_model')
local equip_model = gamemanager.GetModel('equip_model')
local item_model = gamemanager.GetModel('item_model')
local item_controller = gamemanager.GetCtrl('item_controller')
local hero_exp_data = gamemanager.GetData('hero_exp_data')
local formation_model = gamemanager.GetModel('formation_model')

local common_error_tips_view = require('ui/tips/view/common_error_tips_view')
local bag_item = require('ui/hero/view/hero_info/bag_item')
local common_skill_icon = require('ui/common_icon/common_skill_icon')
local common_equip_icon = require('ui/common_icon/common_equip_icon')
local medicine_auto_use_button = require('ui/hero/view/hero_info/medicine_auto_use_button')
local auto_destroy_tip_view = require('ui/tips/view/auto_destroy_tip_view')

function t.Open(defaultSelectedRoleInstanceID)
  if uimanager.GetView(name) then
    return
  end
  uimanager.RegisterView(name,t)
  
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  t.sliderValueChangeAction = nil
  t.scrollItems = {}
  t.equipmentItems = {}
  t.selectHeroInfo = nil
  t.isClickMedicine = false -- 经验药水升级服务端不同步刷新
  t.characterEntity = nil
  --1. 单行列表显示模式 2.多行列表显示模式
  t.viewState = 0
  t.isInitSingleScroll = false
  t.isInitMultiScroll = false
  t.selectMultiToggle = nil
  t.InitHeroInfoList()
  
   
  local common_top_bar = require ('ui/common_top_bar/common_top_bar')
  common_top_bar = common_top_bar:Create(t.transform:Find('core'))
  common_top_bar.transform:SetAsFirstSibling()
  common_top_bar:SetAsCommonStyle('',t.OnClickBackBtnHandler,true,true,true,false,false,false,false)
  
  t.BindDelegate()
  t.InitComponent()
  t.Refresh()
  
  t.RefreshExpMedicine()
  print('defaultSelectedRoleInstanceID',defaultSelectedRoleInstanceID)
  if defaultSelectedRoleInstanceID ~= nil then
    if game_model.playerInfo.instanceID == defaultSelectedRoleInstanceID then
      t.selectHeroInfo = game_model.playerInfo
    else
      t.selectHeroInfo = hero_model.GetHeroInfo(defaultSelectedRoleInstanceID)
    end
    
    local defaultSelectHeroIndex = 0
    for k, v in ipairs(t.heroInfoList) do
      if v.instanceID == defaultSelectedRoleInstanceID then
        defaultSelectHeroIndex = k
        break
      end
    end
    t.bottomScrollContent:ScrollTo(defaultSelectHeroIndex)
    t.bottomScrollContent:RefreshAllContentItems()
  end
  t.RrefreshSelectedRoleExhibition ()
end

function t.Close()
  t.transform = nil
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.DespawnCharacter()
  t.UnbindDelegate()
  t.CancelMedicineAction()
  hero_model.ClearNewHeroMarks()
end

function t.BindDelegate()
  player_model.OnPlayerInfoUpdateDelegate:AddListener(t.OnPlayerInfoUpdateHandler)
  hero_model.onHeroInfoListUpdateDelegate:AddListener(t.OnHeroInfoListUpdateHandler)
  hero_model.OnLockHeroSuccessDelegate:AddListener(t.OnLockHeroSuccessHandler)
  hero_model.OnUnlockHeroSuccessDelegate:AddListener(t.OnUnlockHeroSuccessHandler)
  hero_model.OnLockedHeroChangedDelegate:AddListener(t.OnLockedHeroChangedHandler)
  equip_model.OnEquipmentInfoListUpdateDelegate:AddListener(t.RefreshEquipmentByProtocol)
  item_model.updateItemInfoListDelegate:AddListener(t.RefreshExpMedicine)
  item_model.updateExpPotionDelegate:AddListener(t.updateExpPotionByProtocol)
  game_model.UpdateHeroCellNumDelegate:AddListener(t.OnHeroCellNumUpdateHandler)
end
function t.UnbindDelegate()
  player_model.OnPlayerInfoUpdateDelegate:RemoveListener(t.OnPlayerInfoUpdateHandler)
  hero_model.onHeroInfoListUpdateDelegate:RemoveListener(t.OnHeroInfoListUpdateHandler)
  hero_model.OnLockHeroSuccessDelegate:RemoveListener(t.OnLockHeroSuccessHandler)
  hero_model.OnUnlockHeroSuccessDelegate:RemoveListener(t.OnUnlockHeroSuccessHandler)
  hero_model.OnLockedHeroChangedDelegate:RemoveListener(t.OnLockedHeroChangedHandler)
  equip_model.OnEquipmentInfoListUpdateDelegate:RemoveListener(t.RefreshEquipmentByProtocol)
  item_model.updateItemInfoListDelegate:RemoveListener(t.RefreshExpMedicine)
  item_model.updateExpPotionDelegate:RemoveListener(t.updateExpPotionByProtocol)
  game_model.UpdateHeroCellNumDelegate:RemoveListener(t.OnHeroCellNumUpdateHandler)

end

function t.InitComponent()
  t.combatCapabilityText = t.transform:Find('core/role_info_panel/combat_capability_root/text_combat_capability'):GetComponent(typeof(Text))
  t.btnMultiScrollChange = t.transform:Find('core/btn_multi_line_role_list'):GetComponent(typeof(Button))
  t.arrowImageTransform = t.transform:Find('core/btn_multi_line_role_list/img_arrow')
  t.btnMultiScrollChange.onClick:AddListener(t.ClickMultiScrollChangeHandler)
  --bottom scroll
  t.tranBottomScrollContentRoot = t.transform:Find('core/role_list_root')
  t.bottomScrollContent = t.tranBottomScrollContentRoot:Find('Scroll View/Viewport/Content'):GetComponent(typeof(Common.UI.Components.ScrollContent))
  t.bottomScrollContent.onResetItem:AddListener(t.OnResetItemHandler)
  --top scroll
  t.tranMultiScrollContentRoot = t.transform:Find('core/multi_line_role_scroll_view')
  t.multiScrollContent = t.tranMultiScrollContentRoot:Find('Viewport/Content'):GetComponent(typeof(Common.UI.Components.ScrollContent))
  t.multiScrollContent.onResetItem:AddListener(t.OnResetItemHandler)
  t.toggleSortByLevel = t.tranMultiScrollContentRoot:Find('sort_group/toggle_level'):GetComponent(typeof(Toggle))
  t.toggleSortByLevel:GetComponent(typeof(EventTriggerDelegate)).onClick:AddListener(t.ClickMultiToggleSort)
  t.toggleSortByStrengthen = t.tranMultiScrollContentRoot:Find('sort_group/toggle_strengthen_level'):GetComponent(typeof(Toggle))
  t.toggleSortByStrengthen:GetComponent(typeof(EventTriggerDelegate)).onClick:AddListener(t.ClickMultiToggleSort)
  t.toggleSortByAdvance = t.tranMultiScrollContentRoot:Find('sort_group/toggle_advance_level'):GetComponent(typeof(Toggle))
  t.toggleSortByAdvance:GetComponent(typeof(EventTriggerDelegate)).onClick:AddListener(t.ClickMultiToggleSort)
  t.toggleSortByRoleType = t.tranMultiScrollContentRoot:Find('sort_group/toggle_role_type'):GetComponent(typeof(Toggle))
  t.toggleSortByRoleType:GetComponent(typeof(EventTriggerDelegate)).onClick:AddListener(t.ClickMultiToggleSort)
  t.selectMultiToggle = t.toggleSortByLevel
  --bag
  t.textBagCapacity = t.transform:Find('core/text_bag'):GetComponent(typeof(Text))
  --btn
  t.tranBtnRoot = t.transform:Find('core/role_info_panel/role_function_buttons_root')
  t.btnPlayerStrengthen = t.tranBtnRoot:Find('btn_player_strengthen'):GetComponent(typeof(Button))
  t.btnPlayerAdvance = t.tranBtnRoot:Find('btn_player_advance'):GetComponent(typeof(Button))
  t.btnStrengthen = t.tranBtnRoot:Find('btn_hero_strengthen'):GetComponent(typeof(Button))
  t.btnAdvance = t.tranBtnRoot:Find('btn_hero_advance'):GetComponent(typeof(Button))
  t.btnRoleEquipment = t.tranBtnRoot:Find('btn_role_equipment'):GetComponent(typeof(Button))
  t.btnRelationShip = t.tranBtnRoot:Find('btn_hero_relationship'):GetComponent(typeof(Button))
  t.btnSelectProfession = t.tranBtnRoot:Find('btn_select_profession'):GetComponent(typeof(Button))

  t.btnPlayerStrengthen.onClick:AddListener(t.ClickPlayerStrengthenHandler)
  t.btnPlayerAdvance.onClick:AddListener(t.ClickPlayerAdvanceHandler)
  t.btnStrengthen.onClick:AddListener(t.ClickStrengthenHandler)
  t.btnAdvance.onClick:AddListener(t.ClickAdvanceHandler)
  t.btnRoleEquipment.onClick:AddListener(t.ClickRoleEquipmentHandler)
  t.btnRelationShip.onClick:AddListener(t.ClickRelationShipHandler)
  t.btnSelectProfession.onClick:AddListener(t.ClickSelectProfession)
  --red point view
  t.heroAdvanceRedPointView = t.tranBtnRoot:Find('btn_hero_advance/img_red'):GetComponent('RedPointView')
  t.playerAdvanceRedPointView = t.tranBtnRoot:Find('btn_player_advance/img_red'):GetComponent('RedPointView')
  t.heroRelationshipRedPointView = t.tranBtnRoot:Find('btn_hero_relationship/img_red'):GetComponent('RedPointView')
  t.equipmentRedPointView = t.tranBtnRoot:Find('btn_role_equipment/img_red'):GetComponent('RedPointView')
  --model--
  t.roleRankBGImage = t.transform:Find('core/img_role_rank_root/img_role_rank_bg'):GetComponent(typeof(Image))
  t.roleRankRoleTypeImage = t.transform:Find('core/img_role_rank_root/img_role_rank_role_type'):GetComponent(typeof(Image))
  local role_model_anchor = t.transform:Find('core/role_model_anchor')
  t.tranModelRoot = role_model_anchor:Find('role_model_root')
  t.goUpgradeEffect = role_model_anchor:Find('ui_shengji').gameObject
  t.goUpgradeEffect:SetActive(false)
  local heroStarRoot = ui_util.FindComp(t.transform,'core/stars_root',Transform)
  t.tranHeroStarsTable = {}
  for i = 1,6 do 
    t.tranHeroStarsTable[i] = ui_util.FindComp(heroStarRoot,'img_star_'..i,Transform)
  end
  t.btnClickHeroVictory = ui_util.FindComp(t.transform,'core/role_model_anchor/btn_click_hero',Button)
  t.btnClickHeroVictory.onClick:AddListener(t.ClickHeroVictoryHandler)
  --
  local info_panel = t.transform:Find('core/role_info_panel')
  t.goInfoPanel = info_panel.gameObject
  local attr_root = info_panel:Find('hero_attr_root')
  t.imgRoleType = attr_root:Find('img_hero_type_icon'):GetComponent(typeof(Image))
  t.textHeroName = attr_root:Find('txt_hero_name'):GetComponent(typeof(Text))
  t.textHeroLevel = attr_root:Find('text_hero_level'):GetComponent(typeof(Text))
  t.sliderHeroExp = attr_root:Find('slider_hero_exp'):GetComponent(typeof(Slider))
  t.levelUpImageGO = attr_root:Find('slider_hero_exp/img_upArrow').gameObject
  t.levelUpImageGO:SetActive(false)
  t.textExpPercent = attr_root:Find('text_hero_exp_percent'):GetComponent(typeof(Text))
  --attr
  t.tranAttrTable = {}
  local scrollContent = info_panel:Find('Scroll View/Viewport/Content')
  local child = nil
  for i = 1,8 do
    local tb = {}
    child = scrollContent:GetChild(i-1)
    tb.textTitle = child:Find('role_attribute_title'):GetComponent(typeof(Text))
    tb.textValue = child:Find('role_attribute_value_root/role_attribute_value'):GetComponent(typeof(Text))
    tb.textAdd = child:Find('role_attribute_value_root/role_attribute_add_value'):GetComponent(typeof(Text))
    t.tranAttrTable[i] = tb
  end
  --skill
  local skill_root = info_panel:Find('skills_root')
  t.skillIconTable  = {}
  t.skillIconTable[1] = common_skill_icon.BindTransform(skill_root:Find('img_skill_01'))
  t.skillIconTable[2] = common_skill_icon.BindTransform(skill_root:Find('img_skill_02'))
  t.skillIconTable[3] = common_skill_icon.BindTransform(skill_root:Find('passive/img_skill_03'))
  --exp medicine
  t.tranExpMedicineRoot = info_panel:Find('exp_Medicine_root')
  t.tranExpMedicine = {}
  t.textExpMedicineCount = {}
  --t.tranExpMedicine[1] = medicine_auto_use_button.BindTransform( t.tranExpMedicineRoot:Find('big'))
  --t.tranExpMedicine[2] = medicine_auto_use_button.BindTransform( t.tranExpMedicineRoot:Find('middle'))
  --t.tranExpMedicine[3] = medicine_auto_use_button.BindTransform( t.tranExpMedicineRoot:Find('small'))
  t.tranExpMedicine[ITEM_ID.bigExpMedicine] = t.tranExpMedicineRoot:Find('big')
  t.bigExpPotionRedPointView = t.tranExpMedicine[ITEM_ID.bigExpMedicine]:Find('img_red'):GetComponent('RedPointView')
  t.textExpMedicineCount[ITEM_ID.bigExpMedicine] = t.tranExpMedicineRoot:Find('big/text_count'):GetComponent(typeof(Text))
  
  t.tranExpMedicine[ITEM_ID.midExpMedicine] = t.tranExpMedicineRoot:Find('middle')
  t.midExpPotionRedPointView = t.tranExpMedicine[ITEM_ID.midExpMedicine]:Find('img_red'):GetComponent('RedPointView')
  t.textExpMedicineCount[ITEM_ID.midExpMedicine] = t.tranExpMedicineRoot:Find('middle/text_count'):GetComponent(typeof(Text))
  
  t.tranExpMedicine[ITEM_ID.smallExpMedicine] = t.tranExpMedicineRoot:Find('small')
  t.smallExpPotionRedPointView = t.tranExpMedicine[ITEM_ID.smallExpMedicine]:Find('img_red'):GetComponent('RedPointView')
  t.textExpMedicineCount[ITEM_ID.smallExpMedicine] = t.tranExpMedicineRoot:Find('small/text_count'):GetComponent(typeof(Text))

  for k,v in pairs(t.tranExpMedicine) do
    LuaInterface.LuaCsTransfer.GetItemDesButton(v.gameObject, k,true)
    v:GetComponent(typeof(Button)).onClick:AddListener(function()
        t.ClickExpMedicineHandler(k)
      end)
  end
  --equipment
  local equip_root = info_panel:Find('equipments_root')
  t.tranEquipRoot = {}
  t.tranEquipRoot[1] = equip_root:Find('weapon')
  t.tranEquipRoot[2] = equip_root:Find('armor')
  t.tranEquipRoot[3] = equip_root:Find('accessory')
  for k,v in ipairs(t.tranEquipRoot) do
    v:GetComponent(typeof(Button)).onClick:AddListener(function()
        t.ClickEquipmentSlotHandler(k)
      end)
  end
  
  t.hasFitEquipmentHintMarks = {}
  t.hasFitEquipmentHintMarks[1] = t.tranEquipRoot[1]:Find('img_hint')
  t.hasFitEquipmentHintMarks[2] = t.tranEquipRoot[2]:Find('img_hint')
  t.hasFitEquipmentHintMarks[3] = t.tranEquipRoot[3]:Find('img_hint')
  
  t.btnDecompose = t.transform:Find('core/role_info_panel/hero_attr_root/btn_decompose'):GetComponent(typeof(Button))
  t.btnDecompose.onClick:AddListener(t.ClickDecomposeButtonHandler)
  
  t.btnLock = t.transform:Find('core/role_info_panel/btn_lock'):GetComponent(typeof(Button))
  t.btnLock.onClick:AddListener(t.ClickLockButtonHandler)
end
function t.DespawnCharacter()
  if t.characterEntity then
    PoolController.instance:Despawn(t.characterEntity.name,t.characterEntity)
    t.characterEntity = nil
  end
end
function t.InitHeroInfoList()
  t.heroInfoList = {}
  local infoList = hero_model.GetAllHeroInfoList()
  local index = 0
  for k,v in pairs(infoList) do
    index = index + 1
    t.heroInfoList[index] = v
  end
  t.heroInfoListCount = index
  
  
end
function t.Refresh()
  if t.viewState < 1 then
    t.SetViewState(1)
  end
  t.RefreshCombatCapability()
  t.RefreshModel()
  t.RefreshAttribute()
  t.RefreshSkill()
  t.RefreshEquipment()
  t.RefreshBagCapacity()
  t.RefreshRoleFunctionButtons()
  t.RefreshDecomposeButton ()
  t.RefreshLockButton ()
end

function t.RrefreshSelectedRoleExhibition ()
  t.RefreshCombatCapability()
  t.RefreshModel()
  t.RefreshAttribute()
  t.RefreshSkill()
  t.RefreshEquipment()
  t.RefreshBagCapacity()
  t.RefreshRoleFunctionButtons()
  t.RefreshDecomposeButton ()
  t.RefreshLockButton ()
end

--切换scroll的显示
function t.SetViewState(state)
  
  if state == t.viewState then
    return
  end
  t.viewState = state
  
  if t.viewState == 1 then
    t.arrowImageTransform.localScale = Vector3(1, 1, 1)
  else
    t.arrowImageTransform.localScale = Vector3(-1, 1, 1)
  end
  
  local isSingle = state == 1
  t.ResortHeroInfoList()
  if t.selectHeroInfo == nil then
    t.selectHeroInfo = game_model.playerInfo
  end
  if isSingle then
    
    if not t.isInitSingleScroll then
      t.RegenarateScrollContent()
      t.isInitSingleScroll = true
    else
      t.RefreshScrollContent()
    end
    t.RefreshModel()
  else
    if not t.isInitMultiScroll then
      t.RegenarateScrollContent()
      t.isInitMultiScroll = true
    else 
      t.RefreshScrollContent()
    end
  end
  t.tranBottomScrollContentRoot.gameObject:SetActive(isSingle)
  t.tranMultiScrollContentRoot.gameObject:SetActive(not isSingle)
  t.tranExpMedicineRoot.gameObject:SetActive(isSingle)
  t.tranModelRoot.gameObject:SetActive(isSingle)
end

function t.RegenarateScrollContent()
  if t.viewState == 1 then--单行
    t.bottomScrollContent:Init(game_model.GetHeroCellNum()+2,false,0)
  else--多行
    t.multiScrollContent:Init(game_model.GetHeroCellNum()+2,false,0)
  end
end
function t.RefreshScrollContent()
  if t.viewState == 1 then--单行
    t.bottomScrollContent:Reset(game_model.GetHeroCellNum()+2)
  else--多行
    t.multiScrollContent:Reset(game_model.GetHeroCellNum()+2)
  end
end
function t.RefreshScrollContent2()
  if t.viewState == 1 then--单行
    t.bottomScrollContent:RefreshAllContentItems()
  else--多行
    t.multiScrollContent:RefreshAllContentItems()
  end
end
function t.RefreshBagCapacity()
  local str = LocalizationController.instance:Get('ui.role_info_view.bag_capacity')
  t.textBagCapacity.text = string.format(str,hero_model.GetAllHeroCount(),game_model.GetHeroCellNum())
end

function t.RefreshCombatCapability ()
  if t.selectHeroInfo then
    t.combatCapabilityText.text = math.floor(t.selectHeroInfo:PowerIncludeEquipments())
  else
    t.combatCapabilityText.text = ''
  end
end

function t.RefreshRoleRank ()
  t.roleRankBGImage.sprite = ResMgr.instance:LoadSprite(t.selectHeroInfo.heroData:GetRankImagePath())
  t.roleRankBGImage:SetNativeSize()
  t.roleRankRoleTypeImage.sprite = t.selectHeroInfo.heroData:GetRankRoleTypeSprite ()
  t.roleRankRoleTypeImage:SetNativeSize()
end

function t.RefreshModel()
  if not t.selectHeroInfo then
    return
  end
  if t.viewState == 2 then
    return
  end
  t.RefreshRoleRank ()
  LeanTween.delayedCall(0.05,Action(t.RefreshModelDelay))
  
end
function t.RefreshModelDelay()
  t.DespawnCharacter()
  ui_util.ClearChildren(t.tranModelRoot,true)
  
  if t.selectHeroInfo.instanceID == game_model.playerInfo.instanceID then
    t.characterEntity = CharacterEntity.CreatePlayerEntityAsUIElementByPlayerInfoLuaTable(game_model.playerInfo, t.tranModelRoot, false, true)
  else
    t.characterEntity = CharacterEntity.CreateHeroEntityAsUIElementByHeroInfoLuaTable(t.selectHeroInfo, t.tranModelRoot, false, true)
  end
  
  t.tranModelRoot.localRotation = Quaternion.Euler(t.selectHeroInfo.heroData.home_rotation)
  
  AnimatorUtil.CrossFade(t.characterEntity.anim,AnimatorUtil.VICOTRY_ID,0.3)
  for k,v in pairs(t.tranHeroStarsTable) do
    
    v.gameObject:SetActive(k <= t.selectHeroInfo.advanceLevel)
  end
end

function t.RefreshAttribute()
  t.RefreshAttributeExtra(false,t.selectHeroInfo.level)
end
--排除经验条刷新
function t.RefreshAttributeExtra(exceptExpBar,level)
  
  t.imgRoleType.sprite = ui_util.GetRoleTypeBigIconSprite(t.selectHeroInfo.heroData.roleType)
  t.textHeroName.text = t.selectHeroInfo.heroData:GetNameWithQualityColor()
  t.textHeroLevel.text = string.format(LocalizationController.instance:Get('ui.role_info_view.cur_level'),level,t.selectHeroInfo:MaxLevel())
  --不刷新等级he经验(为了处理吃经验药水自动增加经验冲突)
  if not exceptExpBar then
    
    local percent = t.selectHeroInfo:ExpPercent()
    t.sliderHeroExp.value = percent
    t.textExpPercent.text = string.format('%d%%',math.floor(percent*100))
  end
  
  --attr
  local attrList = role_model.CalcRoleAttributesDic(t.selectHeroInfo)
  
  local equipAddAttrList = role_model.CalcRoleAttributesDicByEquip(t.selectHeroInfo)
  local showAttrList = {}
  showAttrList[1] = attrList[RoleAttributeType.HP]
  if t.selectHeroInfo:GetRoleAttackAttributeType() == RoleAttackAttributeType.PhysicalAttack then
    showAttrList[2] = attrList[RoleAttributeType.NormalAtk]
  else
    showAttrList[2] = attrList[RoleAttributeType.MagicAtk]
  end
  showAttrList[3] = attrList[RoleAttributeType.Def]
  showAttrList[4] = attrList[RoleAttributeType.Speed]
  showAttrList[5] = attrList[RoleAttributeType.Crit]
  showAttrList[6] = attrList[RoleAttributeType.Dodge]
  showAttrList[7] = attrList[RoleAttributeType.Block]
  showAttrList[8] = attrList[RoleAttributeType.Hit]
  
  local curAttr = nil
  local addAttr = nil
  for k,v in pairs(t.tranAttrTable) do
    curAttr = showAttrList[k]
    v.textTitle.text = curAttr:GetName()
    v.textValue.text = curAttr:GetValueString()
    addAttr = equipAddAttrList[curAttr.type]
    if addAttr == nil then
      v.textAdd.text = ''
    else
      v.textAdd.text = string.format('(+%s)',addAttr:GetValueString())
    end
  end
end

function t.RefreshSkill()
  t.skillIconTable[1]:SetSkillData(t.selectHeroInfo.heroData.skillId1,t.selectHeroInfo.advanceLevel,t.selectHeroInfo.heroData.starMin,false)
  t.skillIconTable[2]:SetSkillData(t.selectHeroInfo.heroData.skillId2,t.selectHeroInfo.advanceLevel,t.selectHeroInfo.heroData.starMin,false)
  t.skillIconTable[3]:SetSkillData(t.selectHeroInfo.heroData.passiveId1,t.selectHeroInfo.advanceLevel,t.selectHeroInfo.heroData.starMin,true)
end
function t.RefreshExpMedicine()
  --[[t.tranExpMedicine[1]:SetItemData(ITEM_ID.bigExpMedicine,item_model.GetItemCountByItemID(ITEM_ID.bigExpMedicine))
  t.tranExpMedicine[2]:SetItemData(ITEM_ID.midExpMedicine,item_model.GetItemCountByItemID(ITEM_ID.midExpMedicine))
  t.tranExpMedicine[3]:SetItemData(ITEM_ID.smallExpMedicine,item_model.GetItemCountByItemID(ITEM_ID.smallExpMedicine))
  for k,v in pairs(t.tranExpMedicine) do
    v.onMedicineStartCallback = t.onMedicineStartCallback
    v.onAutoAddExpCallback = t.onAutoAddExpCallback
    v.onMedicineEndCallback = t.onMedicineEndCallback
  end]]
  for k,v in pairs(t.textExpMedicineCount) do
    local count =  item_model.GetItemCountByItemID(k)
    if count == 0 then
      v.text = ui_util.FormatToRedText(count)
    else
      v.text = ui_util.FormatToGreenText(count)
    end
  end
end

function t.RefreshEquipment()
  --t.tranEquipRoot equipmentItems
  local equipIds = {}
  equipIds[1] = t.selectHeroInfo.weaponID
  equipIds[2] = t.selectHeroInfo.armorID
  equipIds[3] = t.selectHeroInfo.accessoryID
  local item = nil
  for k,v in ipairs(t.tranEquipRoot) do
    item = t.equipmentItems[k]
    if equipIds[k] > 0 then
      if item == nil then
        item = common_equip_icon.New(v)
        t.equipmentItems[k] = item
        item.onClick:AddListener(function()
            t.ClickEquipmentSlotHandler(k)
          end)
      end
      item:SetActive(true)
      item:SetEquipInfo(equip_model.GetEquipmentInfoByInstanceID(equipIds[k]))
    else
      if item then
        item:SetActive(false)
      end
    end
    
    local show = equipIds[k] <= 0 
    and gamemanager.GetModel('equip_model').HasFitEquipment(t.selectHeroInfo, k)
    and gamemanager.GetModel('formation_model').IsHeroInAnyTeam(t.selectHeroInfo.instanceID)
    t.hasFitEquipmentHintMarks[k].gameObject:SetActive(show)
  end
    
end

function t.RefreshDecomposeButton ()
  if t.selectHeroInfo.instanceID == game_model.playerInfo.instanceID then  -- Player
    t.btnDecompose.gameObject:SetActive(false)
  else -- Hero
    t.btnDecompose.gameObject:SetActive(true)
  end
end

function t.RefreshLockButton ()
  local isPlayer = t.selectHeroInfo.instanceID == game_model.playerInfo.instanceID
  t.btnLock.gameObject:SetActive(not isPlayer)
  
  if t.selectHeroInfo.isLocked then  -- HERO LOCEDK
    t.btnLock:GetComponent(typeof(Image)).sprite = ResMgr.instance:LoadSprite('sprite/main_ui/icn_lock_close')
  else                               -- HERO UNLOCKED
    t.btnLock:GetComponent(typeof(Image)).sprite = ResMgr.instance:LoadSprite('sprite/main_ui/icn_lock_open')
  end
end

function t.RefreshRoleFunctionButtons ()
  if t.selectHeroInfo == nil then
    return
  end
  
  if t.selectHeroInfo.instanceID == game_model.playerInfo.instanceID then  -- Player
      t.btnPlayerStrengthen.gameObject:SetActive(true)
      t.btnPlayerAdvance.gameObject:SetActive(true)
      t.btnStrengthen.gameObject:SetActive(false)
      t.btnAdvance.gameObject:SetActive(false)
      t.btnRoleEquipment.gameObject:SetActive(true)
      t.btnRelationShip.gameObject:SetActive(false)
      t.btnSelectProfession.gameObject:SetActive(true)
      -----------red point view
      t.playerAdvanceRedPointView:SetId(t.selectHeroInfo.instanceID)
      t.heroRelationshipRedPointView:SetId(0)
  else -- Hero
      t.btnPlayerStrengthen.gameObject:SetActive(false)
      t.btnPlayerAdvance.gameObject:SetActive(false)
      t.btnStrengthen.gameObject:SetActive(true)
      t.btnAdvance.gameObject:SetActive(true)
      t.btnRoleEquipment.gameObject:SetActive(true)
      t.btnRelationShip.gameObject:SetActive(true)
      t.btnSelectProfession.gameObject:SetActive(false)
      ------red point view
      t.heroAdvanceRedPointView:SetId(t.selectHeroInfo.instanceID)
      t.heroRelationshipRedPointView:SetId(t.selectHeroInfo.instanceID)
  end
  t.equipmentRedPointView:SetId(t.selectHeroInfo.instanceID)
  t.bigExpPotionRedPointView:SetId(t.selectHeroInfo.instanceID)
  t.midExpPotionRedPointView:SetId(t.selectHeroInfo.instanceID)
  t.smallExpPotionRedPointView:SetId(t.selectHeroInfo.instanceID)
end

function t.ResortHeroInfoList()
  if t.viewState == 1 then
    table.sort(t.heroInfoList,hero_model.CompareHeroByQualityDescConsiderInFormationHero)
  else
    if t.selectMultiToggle == t.toggleSortByLevel then
    table.sort(t.heroInfoList,hero_model.CompareHeroByLevelDesc)
    elseif t.selectMultiToggle == t.toggleSortByStrengthen then
      table.sort(t.heroInfoList,hero_model.CompareHeroByStrengthenLevelDesc)
    elseif t.selectMultiToggle == t.toggleSortByAdvance then
      table.sort(t.heroInfoList,hero_model.CompareHeroByAdvanceDesc)
    elseif t.selectMultiToggle == t.toggleSortByRoleType then
      table.sort(t.heroInfoList,hero_model.CompareHeroByRoleTypeAndQualityDesc)
    end
  end
end
------------------click event-------------------------
function t.ClickEquipmentSlotHandler(slotIndex)
  equip_model.OpenRoleEquipView(t.selectHeroInfo.instanceID, game_model.IsPlayer(t.selectHeroInfo.instanceID), slotIndex)
end

local tempItem = nil
function t.OnResetItemHandler(go,index)
  go.name = '-1'
  tempItem = t.scrollItems[go]
  if tempItem == nil then
    tempItem = bag_item.BindGameObject(go)
    tempItem.onClick:AddListener(t.ClickItemsHandler)
    t.scrollItems[go] = tempItem
  end
  
  if index <= game_model.GetHeroCellNum() then
    if index <= t.heroInfoListCount then--有英雄
      local info = nil
      if index == 0 then
        info = game_model.playerInfo
      else
        info = t.heroInfoList[index]
      end
      tempItem:SetRoleInfo(info, index)
      tempItem:SetSelect( info == t.selectHeroInfo)
      if info == t.selectHeroInfo then
        t.selectHeroIndex = index
      end
      tempItem:SetInFormation(formation_model.IsHeroInAnyTeam(info.instanceID))
      go.name = info.heroData.id
    else--背包格子
      tempItem:SetRoleInfo(nil, index)
    end
  else--加号
    tempItem:SetAsExpandRolesBagButton(index)
  end
end
function t.ClickItemsHandler(bagItem)
  if bagItem.roleInfo == t.selectHeroInfo then
    return
  end
  
  if bagItem.index <= game_model.GetHeroCellNum() then
    if bagItem.index <= t.heroInfoListCount then--有英雄
      t.CancelMedicineAction()
      hero_model.SetHeroAsChecked(bagItem.roleInfo.instanceID)
      t.selectHeroInfo = bagItem.roleInfo
      t.selectHeroIndex = bagItem.index
      t.RefreshScrollContent2()
      t.RefreshCombatCapability()
      t.RefreshModel()
      t.RefreshAttribute()
      t.RefreshSkill()
      t.RefreshEquipment()
      t.RefreshRoleFunctionButtons ()
      t.RefreshDecomposeButton ()
      t.RefreshLockButton ()
    else--背包格子
    end
  else--加号)
    t.ClickExpandBagHandler()
  end
end

function t.ClickExpandBagHandler(bagItem)
    local global_data = gamemanager.GetData('global_data')
   if game_model.equipCellNum >= global_data.equip_package_max_num then
    common_error_tips_view.Open(LocalizationController.instance:Get("ui.common_tips.equipment_bag_reach_max"))
    return
  end

  local cost = (game_model.heroCellBuyNum +1)*global_data.hero_package_buy_a + global_data.hero_package_buy_b
  local common_expand_bag_tip_view = require('ui/tips/view/common_expand_bag_tip_view')
  common_expand_bag_tip_view.Open2(1,cost)
end
function t.ClickMultiScrollChangeHandler()
  t.ClickBtnCallback() 

  local state = 1
  if t.viewState == 1 then
    state = 2
  else
    state = 1
  end
  t.SetViewState(state)
end


function t.OnClickBackBtnHandler()
  t.Close()
end

function t.ClickMultiToggleSort(go)
  if go == t.selectMultiToggle then
    return
  end
  t.selectMultiToggle = go:GetComponent(typeof(Toggle))
  
  t.ResortHeroInfoList()
  t.RefreshScrollContent()
  
end

function t.ClickPlayerStrengthenHandler ()
  t.ClickBtnCallback()
  hero_controller.OpenHeroStrengthenView(t.selectHeroInfo.instanceID)
end

function t.ClickPlayerAdvanceHandler ()
  t.ClickBtnCallback()
  hero_controller.OpenHeroAdvanceView(t.selectHeroInfo.instanceID)
end

function t.ClickPlayerBreakthroughHandler ()
  t.ClickBtnCallback()
  hero_controller.OpenHeroBreakthroughView(t.selectHeroInfo.instanceID)
end

function t.ClickStrengthenHandler()
  t.ClickBtnCallback()
  hero_controller.OpenHeroStrengthenView(t.selectHeroInfo.instanceID)
end
function t.ClickAdvanceHandler()
  t.ClickBtnCallback()
  hero_controller.OpenHeroAdvanceView(t.selectHeroInfo.instanceID)
end
function t.ClickBreakthroughHandler()
  t.ClickBtnCallback()
  hero_controller.OpenHeroBreakthroughView(t.selectHeroInfo.instanceID)
end
function t.ClickRoleEquipmentHandler()
  t.ClickBtnCallback()
  equip_model.OpenRoleEquipView(t.selectHeroInfo.instanceID, game_model.IsPlayer(t.selectHeroInfo.instanceID), RoleEquipPos.Weapon)
end


function t.ClickRelationShipHandler()
  t.ClickBtnCallback()
  hero_controller.OpenHeroRelationShipView(t.selectHeroInfo)
end

function t.ClickSelectProfession ()
  t.ClickBtnCallback()
  hero_controller.OpenChangeProfessionView()
end

t.isWaitExpMedicineProtocolBack = false
t.UpdateWaitExpBackCoroutine = nil
function t.ClickExpMedicineHandler(id)
  
  --coroutine.start( t.ClickExpDelay,id) 
  
  if t.onMedicineStartCallback(id) then
     
      
      if t.isWaitExpMedicineProtocolBack then
        print('wait')
        return
      end
      t.isWaitExpMedicineProtocolBack = true
      coroutine.stop(t.UpdateWaitExpBackCoroutine)
      coroutine.start(t.UpdateWaitExpBackCoroutine)
      
       item_controller.ExpPotionReq(t.selectHeroInfo.instanceID,id,1)
      t.isClickMedicine = true
    end
end
function t.UpdateWaitExpBackCoroutine()
  coroutine.wait(1)
  t.isWaitExpMedicineProtocolBack = false
end

--only test
function t.ClickExpDelay(id)
  while t.onMedicineStartCallback(id) do
     if t.isWaitExpMedicineProtocolBack then
        Debugger.LogError('wait')
        coroutine.wait(0.00001)
      else
        t.isWaitExpMedicineProtocolBack = true
        item_controller.ExpPotionReq(t.selectHeroInfo.instanceID,id,1)
        t.isClickMedicine = true
        --coroutine.wait(0.0001)
        
      end
     
  end
end


function t.ClickBtnCallback()
  t.CancelMedicineAction()
end

function t.ClickDecomposeButtonHandler ()
  if gamemanager.GetModel('formation_model').IsHeroInAnyTeam(t.selectHeroInfo.instanceID) then
    local auto_destroy_tip_view = require 'ui/tips/view/auto_destroy_tip_view'
    auto_destroy_tip_view.Open(LocalizationController.instance:Get('ui.role_info_view.in_formaiton_hero_cant_be_decomposed_tips'))
    return
  end
  
  if hero_model.IsHeroInRelations(t.selectHeroInfo.instanceID) then
    local auto_destroy_tip_view = require 'ui/tips/view/auto_destroy_tip_view'
    auto_destroy_tip_view.Open(LocalizationController.instance:Get('ui.role_info_view.in_relationship_hero_cant_be_decomposed_tips'))
    return
  end
  
  if hero_model.IsHeroInExploring(t.selectHeroInfo.instanceID) then
    local auto_destroy_tip_view = require 'ui/tips/view/auto_destroy_tip_view'
    auto_destroy_tip_view.Open(LocalizationController.instance:Get('ui.role_info_view.in_explore_hero_cant_be_decomposed_tips'))
    return
  end
  if not gamemanager.GetModel('function_open_model').IsFunctionOpen(FunctionOpenType.HeroDecompose,true) then
    return
  end
  hero_controller.OpenHeroDecomposeView(t.selectHeroInfo)
  
end

function t.ClickLockButtonHandler ()
  if not t.selectHeroInfo.isLocked then
    --hero_controller.HeroLockReq (t.selectHeroInfo.instanceID)
    local confirm_tip_view = require('ui/tips/view/confirm_tip_view')
    confirm_tip_view.Open(LocalizationController.instance:Get('ui.role_info_view.lock_hero_tips'), t.ClickConfirmLock)
  else
    hero_controller.HeroUnLockReq (t.selectHeroInfo.instanceID)
  end
end

function t.ClickConfirmLock ()
  hero_controller.HeroLockReq(t.selectHeroInfo.instanceID)
end

-------------以下为经验药水回调------------
t.selectHeroLevel  = 0
t.selectOldHeroLevel = 0
t.selectHeroCurUpdateLevel = 0
t.selectHeroExp = 0
t.selectMaxLevel = 0
function t.onMedicineStartCallback(itemId)
  if t.selectHeroLevel == 0 then
    t.selectHeroLevel = t.selectHeroInfo.level
    t.selectOldHeroLevel = t.selectHeroInfo.level

    t.selectHeroCurUpdateLevel = t.selectHeroLevel
    t.selectHeroExp = t.selectHeroInfo.exp
    t.selectMaxLevel = t.selectHeroInfo:MaxLevel()
  end
  --print('selectHeroLevel',t.selectHeroLevel)
  local count = item_model.GetItemCountByItemID(itemId)
  if count == 0 then
    auto_destroy_tip_view.Open(string.format(LocalizationController.instance:Get("ui.role_info_view.notEnoughMedicine"),
        LocalizationController.instance:Get(gamemanager.GetData('item_data').GetDataById(itemId).name)))
      return false
  end
  
  if t.selectHeroLevel >= t.selectMaxLevel then
    auto_destroy_tip_view.Open(LocalizationController.instance:Get("ui.role_info_view.reachMaxLvTip"))
    return false
  end
  if t.selectHeroLevel >= game_model.accountLevel then
    auto_destroy_tip_view.Open(LocalizationController.instance:Get("ui.role_info_view.reachAccountLevelTip"))
    return false
  end
  
  return true
end


function t.onAutoAddExpCallback(addExp)

  local flag = true
  if t.selectHeroLevel >= t.selectMaxLevel then
    t.selectHeroLevel = t.selectMaxLevel
    flag = false
  end
  if t.selectHeroLevel >= game_model.accountLevel then
    t.selectHeroLevel = game_model.accountLevel
    flag = false
  end

  local oldExpData = hero_exp_data.GetDataById(t.selectOldHeroLevel)
  local expData = hero_exp_data.GetDataById(t.selectHeroLevel)
  print('[onAutoAddExpCallback]','selectLv:'..t.selectHeroLevel,'oldLv:'..t.selectOldHeroLevel,'heroExp:'..t.selectHeroExp,'expData.exp:'..expData.exp)
  if expData then
    --经验更新
    local oldExpPercent = t.selectOldHeroExp / oldExpData.exp
    print('[onAutoAddExpCallback]经验更新oldExpPercent:'..oldExpPercent)
    if oldExpPercent >= 1 then
      t.UpdateExpBar(t.selectHeroLevel-t.selectOldHeroLevel + t.selectHeroExp/expData.exp - 1)
      t.UpdateReach100Percent()
    else
      if(t.selectHeroExp == 0) then --解决升级后经验刚好为0的bug ,而进度条却是100%
        t.UpdateExpBar(t.selectHeroLevel-t.selectOldHeroLevel + t.selectHeroExp/expData.exp+0.001)   
      else
        t.UpdateExpBar(t.selectHeroLevel-t.selectOldHeroLevel + t.selectHeroExp/expData.exp)
      end
       
    end
  end
  t.isClickMedicine = true
  return flag
end
function t.UpdateExpBar(endPercent)
  if t.sliderValueChangeAction == nil then
    t.sliderValueChangeAction = Common.UI.Components.SliderValueChangeAction.Get(t.sliderHeroExp.gameObject)
    t.sliderValueChangeAction:SetValueChangeUpdate(t.UpdateExpBarValueChange)
    t.sliderValueChangeAction:SetReach100PercentUpdate(t.UpdateReach100Percent)
  end
  t.sliderValueChangeAction:StartValueChangeActionList(endPercent,1,true)
  
  
end
--每帧更新
function t.UpdateExpBarValueChange(rate)
  local intRate = math.floor(rate)
  local percent = 0
  if intRate == rate then
    percent = 100
  else
    percent = math.floor(((rate-intRate)*100))
  end
  
  t.textExpPercent.text = string.format('%d%%',percent)
end
--到达100%时回调
function t.UpdateReach100Percent()
  print('UpdateReach100Percent')
  t.selectHeroCurUpdateLevel = t.selectHeroCurUpdateLevel + 1
  if t.selectHeroCurUpdateLevel <= t.selectHeroLevel then
    t.selectHeroInfo.level = t.selectHeroCurUpdateLevel
    t.ResortHeroInfoList()
    t.RefreshScrollContent2()
    t.RefreshAttributeExtra(true,t.selectHeroCurUpdateLevel)
    
    t.goUpgradeEffect:SetActive(false)
    t.goUpgradeEffect:SetActive(true)
    t.PlayLevelUpFX ()
    t.RefreshCombatCapability()
  end
  
  if t.selectHeroCurUpdateLevel == t.selectHeroLevel then
    hero_model.CheckHasAdvanceBreakthroughHeroByRedPoint()
  end
  print('selectHeroCurUpdateLevel',t.selectHeroCurUpdateLevel)
end

--[[function t.onMedicineEndCallback(itemId,useCount)
  print('useMedicineCount',useCount)
  if useCount ~= 0 then
    LeanTween.delayedCall(0.2,Action(function()
      gamemanager.GetCtrl('item_controller').ExpPotionReq(t.selectHeroInfo.instanceID,itemId,useCount)
    end))
  end
  
end]]

function t.CancelMedicineAction()
  --fix bug 发消息了就不在放动画了
  if t.sliderValueChangeAction then
    t.sliderValueChangeAction:CancelAction()
  end
  if t.selectHeroLevel ~= 0 then
    t.selectHeroInfo.level = t.selectHeroLevel
    t.selectHeroInfo.exp = t.selectHeroExp
    --print('[CancelMedicineAction]','selectLv:'..t.selectHeroLevel,'exp:'..t.selectHeroExp)
    t.RefreshAttributeExtra(false,t.selectHeroLevel)
    t.ResortHeroInfoList()
    t.RefreshScrollContent()
  end
  t.selectHeroLevel  = 0
  t.selectHeroCurUpdateLevel = 0
  t.selectHeroExp = 0
  t.selectMaxLevel = 0
  hero_model.CheckHasAdvanceBreakthroughHeroByRedPoint()
   -- local percent = t.selectHeroInfo:ExpPercent()
   -- t.sliderHeroExp.value = percent
   -- t.textExpPercent.text = string.format('%d%%',math.floor(percent*100))
end

-------------END------------

function t.PlayLevelUpFX ()
  if t.playLevelUpFXCo then
    coroutine.stop(t.playLevelUpFXCo)
  end
  LeanTween.cancel(t.levelUpImageGO)
  t.levelUpImageGO:SetActive(false)
  t.levelUpImageGO:SetActive(true)
  t.levelUpImageGO.transform.localScale = Vector3(0, 1, 1)
  LeanTween.scaleX(t.levelUpImageGO, 1, 0.1)
  CommonFadeToAnimation.Get(t.levelUpImageGO):init(0, 1, 0.2, 0)
  t.playLevelUpFXCo = coroutine.start(function()
    coroutine.wait(1)
    if t.transform then
      CommonFadeToAnimation.Get(t.levelUpImageGO):init(1, 0, 0.5, 0)
      coroutine.wait(0.7)
      if t.transform then
        t.levelUpImageGO:SetActive(false)
      end

    end
  end)
end
--点击英雄模型播放胜利动画
function t.ClickHeroVictoryHandler()
  
  if t.selectHeroInfo then
    local id = t.selectHeroInfo.instanceID
    if game_model.playerInfo.instanceID == id then
      gamemanager.GetModel('audio_model').PlayRandomAudioInView(AudioViewType.mainView,game_model.playerInfo.heroData.id)
    else
      gamemanager.GetModel('audio_model').PlayRandomAudioInView(AudioViewType.mainView,hero_model.GetHeroInfo(id).heroData.id)
    end
      AnimatorUtil.CrossFade(t.characterEntity.anim,AnimatorUtil.VICOTRY_ID,0.3)
  end
end
-----------------update by protocol--------------
t.clickMedicineResetCoroutine = nil

function t.OnPlayerInfoUpdateHandler ()
  coroutine.stop(t.clickMedicineResetCoroutine)
  if t.isClickMedicine then
    t.clickMedicineResetCoroutine = coroutine.start(function ()
        coroutine.wait(0.6)
        t.isClickMedicine = false
      end)
    return
  end
  if player_model.IsPlayerInstanceID(t.selectHeroInfo.instanceID) then
    t.selectHeroInfo = game_model.playerInfo
  end
  
  t.InitHeroInfoList()
  t.ResortHeroInfoList()
  t.RefreshScrollContent()
  t.Refresh()
end

function t.OnHeroInfoListUpdateHandler()
  coroutine.stop(t.clickMedicineResetCoroutine)
  if t.isClickMedicine then
    t.clickMedicineResetCoroutine = coroutine.start(function ()
        coroutine.wait(0.6)
        t.isClickMedicine = false
      end)
    return
  end
  t.InitHeroInfoList()
  t.ResortHeroInfoList()
  
  print('[start] selected hero index:'..t.selectHeroIndex)
  local isSelectedRoleExist = false
  if t.selectHeroInfo ~= nil then
    if t.selectHeroInfo == game_model.playerInfo then
      t.selectHeroIndex = 0
      isSelectedRoleExist = true
    else
      for k, v in ipairs(t.heroInfoList) do
        if v.instanceID == t.selectHeroInfo.instanceID then
          t.selectHeroIndex = k
          isSelectedRoleExist = true
          break
        end
      end
    end
  end
  
  if not isSelectedRoleExist then
    t.selectHeroIndex = t.selectHeroIndex - 1
    if t.selectHeroIndex == 0 then
      t.selectHeroInfo = game_model.playerInfo
    else
      t.selectHeroInfo = t.heroInfoList[t.selectHeroIndex]
    end
  end
  print('[end] selected hero index:'..t.selectHeroIndex)
  -- Add
  
  --[[
  if t.selectHeroIndex == 0 then
    t.selectHeroInfo = game_model.playerInfo
  elseif table.count(t.heroInfoList) < t.selectHeroIndex then
    t.selectHeroIndex = table.count(t.heroInfoList)
    if t.selectHeroIndex > 0 then
      t.selectHeroInfo = t.heroInfoList[t.selectHeroIndex]
    end
  elseif t.heroInfoList[t.selectHeroIndex] ~= t.selectHeroInfo then
    t.selectHeroInfo = t.heroInfoList[t.selectHeroIndex]
  end
  ]]--
  
  t.RefreshScrollContent()
  t.Refresh() 
end

function t.OnHeroCellNumUpdateHandler()
  t.RefreshScrollContent()
  t.RefreshBagCapacity()
  if t.viewState == 1 then--单行
    t.bottomScrollContent:ScrollToEnd()
  else--多行
    t.multiScrollContent:ScrollToEnd()
  end
end
-----------------update by protocol--------------
function t.updateExpPotionByProtocol(id)
  local addExps = gamemanager.GetData('global_data').expSolutions
  local addExp = 0
  if id == ITEM_ID.bigExpMedicine then
    addExp = addExps[1]
  elseif id == ITEM_ID.midExpMedicine then
    addExp = addExps[2]
  else
    addExp = addExps[3]
  end
  t.selectOldHeroLevel = t.selectHeroLevel
  t.selectOldHeroExp = t.selectHeroExp
  t.selectHeroLevel = t.selectHeroInfo.level
  t.selectHeroExp = t.selectHeroInfo.exp
  print('吃经验了old level :'..t.selectOldHeroLevel,'selectHerolevel:'..t.selectHeroLevel,'exp:'..t.selectHeroExp,'exppercent:'..t.selectHeroInfo:ExpPercent(),'addExp:'..addExp)
  t.onAutoAddExpCallback(addExp)
  t.isWaitExpMedicineProtocolBack = false
end
function t.RefreshEquipmentByProtocol()
  print('[hero_info_view]RefreshEquipmentByProtocol')
  t.RefreshAttribute()
  t.RefreshEquipment()
  t.RefreshCombatCapability()
end

function t.OnLockHeroSuccessHandler ()
  t.RefreshLockButton ()
end

function t.OnUnlockHeroSuccessHandler ()
  t.RefreshLockButton ()
end

function t.OnLockedHeroChangedHandler ()
  t.RefreshLockButton ()
end

return t