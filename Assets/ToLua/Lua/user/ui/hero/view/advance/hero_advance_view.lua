local t = {}
local PREFAB_PATH = 'ui/hero_advance/hero_advance_view'

local player_controller = gamemanager.GetCtrl('player_controller')
local player_model = gamemanager.GetModel('player_model')
local hero_controller = gamemanager.GetCtrl('hero_controller')
local hero_model = gamemanager.GetModel('hero_model')
local role_model = gamemanager.GetModel('role_model')
local game_model = gamemanager.GetModel('game_model')
local item_model = gamemanager.GetModel('item_model')
local hero_advance_data = gamemanager.GetData('hero_advance_data')
local common_item_icon = require('ui/common_icon/common_item_icon')
local common_error_tips_view = require('ui/tips/view/common_error_tips_view')

function t.Open(heroInstanceId)
  uimanager.RegisterView(PREFAB_PATH,t)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  if game_model.IsPlayer(heroInstanceId) then
    t.heroInfo = game_model.playerInfo
  else
    t.heroInfo = hero_model.GetHeroInfo(heroInstanceId)
  end
  
  t.itemIconMaterial = {}
  t.isClickAdvanceBtn = false
  local common_top_bar = require ('ui/common_top_bar/common_top_bar')
  common_top_bar = common_top_bar:Create(t.transform:Find('core'))
  common_top_bar:SetAsCommonStyle(LocalizationController.instance:Get("ui.hero_advance_view.hero_advance_title"),t.OnClickBackBtnHandler,true,true,true,false,false,false,false)
  
  t.BindDelegate()
  t.InitComponent()
  t.Refresh(false)
end

function t.Close()
  t.transform = nil
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
  t.DespawnCharacter(t.characterEntity1)
  t.DespawnCharacter(t.characterEntity2)
end

function t.BindDelegate()
  game_model.onUpdateBaseResourceDelegate:AddListener(t.OnBaseResourceUpdateHandler)
  player_model.OnPlayerAdvanceSuccessDelegate:AddListener(t.UpdateHeroAdvanceSuccessByProtocol)
  hero_model.onHeroAdvanceSuccessDelegate:AddListener(t.UpdateHeroAdvanceSuccessByProtocol)
  item_model.updateItemInfoListDelegate:AddListener(t.RefreshMaterial)
end
function t.UnbindDelegate()
  game_model.onUpdateBaseResourceDelegate:RemoveListener(t.OnBaseResourceUpdateHandler)
  player_model.OnPlayerAdvanceSuccessDelegate:RemoveListener(t.UpdateHeroAdvanceSuccessByProtocol)
  hero_model.onHeroAdvanceSuccessDelegate:RemoveListener(t.UpdateHeroAdvanceSuccessByProtocol)
  item_model.updateItemInfoListDelegate:RemoveListener(t.RefreshMaterial)
end

function t.InitComponent()
  t.beforeComponentTable = {}
  t.afterComponentTable = {}
  
  local before = t.transform:Find('core/advance_model/before_advance_hero_root')
  t.beforeComponentTable.root = before
  t.beforeComponentTable.imgRoleType = before:Find('name_root/lv_root/img_role_type'):GetComponent(typeof(Image))
  t.beforeComponentTable.textLevel = before:Find('name_root/lv_root/text_lv'):GetComponent(typeof(Text))
  t.beforeComponentTable.textName = before:Find('name_root/text_before_name'):GetComponent(typeof(Text))
  t.beforeComponentTable.tranModelRoot = before:Find('before_advance_model_root')
  t.beforeComponentTable.goStars = {}
  local star_root = before:Find('star_root')
  for i = 1,6 do
    t.beforeComponentTable.goStars[i] = star_root:Find('img_star_'..i).gameObject
  end
  t.newStarEffectTransform = before:Find('star_root/ui_shengxing_01')
  t.newStarEffectTransform.gameObject:SetActive(false)
  
  local after = t.transform:Find('core/advance_model/after_advance_hero_root')
  t.afterComponentTable.root = after
  t.afterComponentTable.imgRoleType = after:Find('name_root/lv_root/img_role_type'):GetComponent(typeof(Image))
  t.afterComponentTable.textLevel = after:Find('name_root/lv_root/text_lv'):GetComponent(typeof(Text))
  t.afterComponentTable.textName = after:Find('name_root/text_after_name'):GetComponent(typeof(Text))
  t.afterComponentTable.tranModelRoot = after:Find('after_advance_model_root')
  t.afterComponentTable.goStars = {}
  local star_root2 = after:Find('star_root')
  for i = 1,6 do
    t.afterComponentTable.goStars[i] = star_root2:Find('img_star_'..i).gameObject
  end
  --bottom
  local attr_frame = t.transform:Find('core/center_part_root/img_attributes_frame')
  t.tranAttrRoot = attr_frame:Find('attribute_root')
  t.goAttrPrefab = attr_frame:Find('attribute_template').gameObject
  t.goAttrPrefab:SetActive(false)
  local material_frame = t.transform:Find('core/center_part_root/img_attributes_frame/img_material_bg')
  
  t.tranAdvanceOperationRoot = material_frame:Find('advance_operation_root')
  t.tranMaterials = {}
  t.tranMaterials[1] = t.tranAdvanceOperationRoot:Find('material')
  t.textMoney = t.tranAdvanceOperationRoot:Find('text_coin'):GetComponent(typeof(Text))
  t.btnAdvance = t.tranAdvanceOperationRoot:Find('btn_advance'):GetComponent(typeof(Button))
  t.btnAdvance.onClick:AddListener(t.ClickAdvanceHandler)
  t.goStarReachMax = material_frame:Find('text_star_reach_max').gameObject
end

function t.Refresh(showNewStarEffect)
  t.ResetHeroModels(showNewStarEffect)
  t.RefreshAttribute()
  t.RefreshMaterial()
end

function t.ResetHeroModels(showNewStarEffect)
  local spriteRoleType = ui_util.GetRoleTypeSmallIconSprite(t.heroInfo.heroData.roleType)
  local name = t.heroInfo.heroData:GetNameWithQualityColor()
  local nameColor = role_model.GetRoleNameColor(t.heroInfo:RoleStrengthenStage())
  
  t.DespawnCharacter(t.characterEntity1)
  t.DespawnCharacter(t.characterEntity2)

  --cur hero
  t.beforeComponentTable.imgRoleType.sprite = spriteRoleType
  t.beforeComponentTable.textLevel.text = t.heroInfo.level
  t.beforeComponentTable.textName.text = name
  t.beforeComponentTable.textName.color = nameColor
  for k,v in pairs(t.beforeComponentTable.goStars) do
    t.beforeComponentTable.goStars[k]:SetActive(k <= t.heroInfo.advanceLevel)
    if showNewStarEffect and k == t.heroInfo.advanceLevel then
      local newStarImage = v:GetComponent(typeof(Image))
      newStarImage:CrossFadeAlpha(0, 0, true)
      LeanTween.delayedCall(0.6, Action(function()
        newStarImage:CrossFadeAlpha(1, 0, true)
      end))
      
      t.newStarEffectTransform:SetParent(v.transform, false)
      t.newStarEffectTransform.localPosition = Vector3(0, 0, 0)
      t.newStarEffectTransform.gameObject:SetActive(false)
      t.newStarEffectTransform.gameObject:SetActive(true)
    end
  end
  ui_util.ClearChildren(t.beforeComponentTable.tranModelRoot,true)
  if game_model.IsPlayer(t.heroInfo.instanceID) then
    t.characterEntity1 = Logic.Character.CharacterEntity.CreatePlayerEntityAsUIElement(game_model.playerInfo.instanceID, t.beforeComponentTable.tranModelRoot, false, true)
  else
    t.characterEntity1 = Logic.Character.CharacterEntity.CreateHeroEntityAsUIElementByHeroInfoLuaTable(t.heroInfo,t.beforeComponentTable.tranModelRoot, false, true)
  end
  --next hero
  ui_util.ClearChildren(t.afterComponentTable.tranModelRoot,true)
  local isMax = t.heroInfo.advanceLevel == t.heroInfo.heroData.starMax 
  if isMax then
    t.afterComponentTable.root.gameObject:SetActive(false)
    LeanTween.moveLocalX(t.beforeComponentTable.root.gameObject,0,0.2)
  else
    t.afterComponentTable.root.gameObject:SetActive(true)
    t.afterComponentTable.imgRoleType.sprite = spriteRoleType
    t.afterComponentTable.textLevel.text = t.heroInfo.level
    t.afterComponentTable.textName.text = name
    t.afterComponentTable.textName.color = nameColor
    for k,v in pairs(t.beforeComponentTable.goStars) do
      t.afterComponentTable.goStars[k]:SetActive(k <= t.heroInfo.advanceLevel+1)
    end
    
    if game_model.IsPlayer(t.heroInfo.instanceID) then
      local mockPlayerInfo = game_model.playerInfo:GetCopyPlayerInfo()
      mockPlayerInfo.advanceLevel = mockPlayerInfo.advanceLevel + 1
      t.characterEntity2 = Logic.Character.CharacterEntity.CreatePlayerEntityAsUIElementByPlayerInfoLuaTable(mockPlayerInfo, t.afterComponentTable.tranModelRoot, false, true)
    else
      local nextHeroInfo = t.heroInfo:Clone()
      nextHeroInfo.advanceLevel = t.heroInfo.advanceLevel + 1
      t.characterEntity2 = Logic.Character.CharacterEntity.CreateHeroEntityAsUIElementByHeroInfoLuaTable(nextHeroInfo,t.afterComponentTable.tranModelRoot,false,true)
    end
  end
end

function t.DespawnCharacter(characterEntity)
  if characterEntity ~= nil then
    Logic.Pool.Controller.PoolController.instance:Despawn(characterEntity.name, characterEntity)
    if t.characterEntity1 == characterEntity then
      t.characterEntity1 = nil
    elseif t.characterEntity2 == characterEntity then
      t.characterEntity2 = nil
    end
  end
end

function t.RefreshAttribute()
  ui_util.ClearChildren(t.tranAttrRoot,true)
  local attrList = role_model.CalcRoleMainAttributesList(t.heroInfo)
  local isMax = t.heroInfo.advanceLevel == t.heroInfo.heroData.starMax 
  local newRoleInfo = t.heroInfo:Clone()
  newRoleInfo.advanceLevel = newRoleInfo.advanceLevel + 1
  local nextAttrList = role_model.CalcRoleMainAttributesList(newRoleInfo)
  for k,v in pairs(attrList) do
    local go = GameObject.Instantiate(t.goAttrPrefab)
    go:SetActive(true)
    local tran = go.transform
    tran:SetParent(t.tranAttrRoot,false)
    local textName = tran:Find('text_title'):GetComponent(typeof(Text))
    local textValue = tran:Find('text_value'):GetComponent(typeof(Text))
    local textAdd = tran:Find('text_add'):GetComponent(typeof(Text))
    textName.text = v:GetName()
    textValue.text = v:GetValueString()
    if isMax then
      textAdd.text = string.format('(%s)',LocalizationController.instance:Get("ui.hero_advance_view.role_attribute_max_remark"))
    else
      textAdd.text = string.format('(+%d)',math.floor(nextAttrList[k].value-v.value))
    end
  end
end

function t.RefreshMaterial()
  --[[
  local data = hero_advance_data.GetDataById(t.heroInfo.advanceLevel)
  local materials = {}
  if data ~= nil then
    materials = data:GetMaterialsByRoleType(t.heroInfo.heroData.roleType)
  end
  
  local isMax = t.heroInfo.advanceLevel == t.heroInfo.heroData.starMax
  
  t.goStarReachMax:SetActive(isMax)
  t.tranAdvanceOperationRoot.gameObject:SetActive(isMax == false)
  
  for k,v in pairs(t.tranMaterials) do
    
    local textCount = v:Find('text_count'):GetComponent(typeof(Text))
    local itemIcon = nil
    if t.itemIconMaterial[k] == nil then
      t.itemIconMaterial[k] = common_item_icon.New(v)
        
      t.itemIconMaterial[k].onClick:AddListener(t.ClickMaterialHandler)
    end
    itemIcon = t.itemIconMaterial[k]  
    if isMax then--最大星级了
      itemIcon:SetActive(false)
      textCount.gameObject:SetActive(false)
    else
      if materials[k] == nil then--没材料
        itemIcon:SetActive(false)
        textCount.gameObject:SetActive(false)
      else
        itemIcon:SetActive(true)
        itemIcon:SetGameResData(materials[k])
        itemIcon:ShowCount(false)
        textCount.gameObject:SetActive(true)
        local own = item_model.GetItemCountByItemID(materials[k].id)
        local countString = string.format('%d/%d',own,materials[k].count)
        if own >= materials[k].count then
          textCount.text = ui_util.FormatToGreenText(countString)
        else
          textCount.text = ui_util.FormatToRedText(countString)
        end
      end
    end
  end
  --money
  if isMax == false then
    local ownMoney = game_model.GetBaseResourceValue(BaseResType.Gold)
    if ownMoney >= data.gold then
      t.textMoney.text = ui_util.FormatToGreenText(data.gold)
    else
      t.textMoney.text = ui_util.FormatToRedText(data.gold)
    end
    --need level
    local needLvString = string.format(LocalizationController.instance:Get("ui.hero_advance_view.need_level"), data.lv_limit);
    if t.heroInfo.level < data.lv_limit then
      t.textNeedLevel.text = ui_util.FormatToRedText(needLvString)
    else
      t.textNeedLevel.text = ui_util.FormatToGreenText(needLvString)
    end
  end
  ]]--
  
  local isMax = t.heroInfo.advanceLevel == t.heroInfo.heroData.starMax
  
  t.goStarReachMax:SetActive(isMax)
  t.tranAdvanceOperationRoot.gameObject:SetActive(isMax == false)
  
  if not isMax then
    local advanceData = hero_advance_data.GetAdvanceDataByHeroInfo(t.heroInfo)
    local advanceMaterialGameResData = advanceData.materialGameResDataList
    for k, v in pairs(t.tranMaterials) do
      local textCount = v:Find('text_count'):GetComponent(typeof(Text))
      local itemIcon = nil
      if t.itemIconMaterial[k] == nil then
        t.itemIconMaterial[k] = common_item_icon.New(v)
          
        t.itemIconMaterial[k].onClick:AddListener(t.ClickMaterialHandler)
      end
      itemIcon = t.itemIconMaterial[k]  
      if isMax then--最大星级了
        itemIcon:SetActive(false)
        textCount.gameObject:SetActive(false)
      else
        if advanceMaterialGameResData[k] == nil then--没材料
          itemIcon:SetActive(false)
          textCount.gameObject:SetActive(false)
        else
          itemIcon:SetActive(true)
          itemIcon:SetGameResData(advanceMaterialGameResData[k])
          itemIcon:ShowCount(false)
          textCount.gameObject:SetActive(true)
          local own = item_model.GetItemCountByItemID(advanceMaterialGameResData[k].id)
          local countString = string.format('%d/%d', own, advanceMaterialGameResData[k].count)
          if own >= advanceMaterialGameResData[k].count then
            textCount.text = ui_util.FormatToGreenText(countString)
          else
            textCount.text = ui_util.FormatToRedText(countString)
          end
        end
      end
    end
    
    --money
    local ownMoney = game_model.GetBaseResourceValue(BaseResType.Gold)
    if ownMoney >= advanceData.gold then
      t.textMoney.text = ui_util.FormatToGreenText(advanceData.gold)
    else
      t.textMoney.text = ui_util.FormatToRedText(advanceData.gold)
    end
  else
      t.tranMaterials[1]:Find('text_count').gameObject:SetActive(false)
      if t.itemIconMaterial[1] then
        t.itemIconMaterial[1]:SetActive(false)
      end
  end
end

------------------------------click event-----------------------
function t.OnClickBackBtnHandler()
  t.Close()
end
--升星
function t.ClickAdvanceHandler()
  if t.isClickAdvanceBtn then
    return
  end
  local cost = hero_advance_data.GetAdvanceDataByHeroInfo(t.heroInfo).gold
  if not game_model.CheckBaseResEnoughByType(BaseResType.Gold,cost) then
    return
  end
  if hero_model.IsHeroCanAdvance(t.heroInfo,true) then
    if game_model.IsPlayer(t.heroInfo.instanceID) then
      player_controller.PlayerAdvanceReq(t.heroInfo.instanceID)
    else
      hero_controller.HeroAdvanceReq(t.heroInfo.instanceID, 0)
    end
    t.isClickAdvanceBtn = true
    coroutine.start(function()   
      coroutine.wait(2)
        t.isClickAdvanceBtn  = false
      end)
  end
end

function t.ClickMaterialHandler(itemIcon)
  LuaInterface.LuaCsTransfer.OpenGoodsJumpPath(BaseResType.Item,itemIcon.itemInfo.itemData.id,0)
end
------------------------update by protocol------------------------

function t.UpdateHeroAdvanceSuccessByProtocol(roleInstanceID)
  print('升星成功啦！！！')
  
  -- [[SHOW ROLE EXHIBITION VIEW]] --
  local roleInfo = nil
  if gamemanager.GetModel('game_model').IsPlayer(roleInstanceID) then
    roleInfo = gamemanager.GetModel('game_model').playerInfo
  else
    roleInfo = gamemanager.GetModel('hero_model').GetHeroInfo (roleInstanceID)
  end
  local role_exhibition_view = require 'ui/role_exhibition/view/role_exhibition_view'
  role_exhibition_view.Open(roleInfo, role_exhibition_view.ExhibitionType.Advance)
  -- [[SHOW ROLE EXHIBITION VIEW]] --
  AudioController.instance:PlayAudio( "advance",false,1.5)
  t.Refresh(true)
end

function t.OnBaseResourceUpdateHandler ()
  t.Refresh(false)
end


return t