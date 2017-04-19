local t = {}
local PREFAB_PATH = 'ui/role_exhibition/role_exhibition_view'
local ROLE_IMPROVE_EXHIBITION_3D_VIEW_PATH = 'ui_3d/role_improve_exhibition/role_improve_exhibition_view'

local game_model = gamemanager.GetModel('game_model')
local role_model = gamemanager.GetModel('role_model')
local common_skill_icon = require('ui/common_icon/common_skill_icon')

t.ExhibitionType = {}
t.ExhibitionType.Breakthrough = 1
t.ExhibitionType.Advance = 2

function t.Open (roleInfo, exhibitionType)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  t.exhibitionType = exhibitionType
  
  t.InitComponent ()
  t.SetRoleInfo(roleInfo)
end

function t.Close ()
  LeanTween.cancel(t.transform.gameObject)
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.RemoveObserver()
  t.DespawnCharacter()
  Common.Util.TransformUtil.ClearChildren(UIMgr.instance.ui3DRoot, true)
  t.transform = nil
  t.roleImproveExhibitionViewGameObject = nil
end

function t.InitComponent ()
  t.closeButton = t.transform:Find("core/btn_close"):GetComponent(typeof(Button))
  t.closeButton.onClick:AddListener(t.ClickCloseButton)
  t.closeButton.gameObject:SetActive(false)
  
  local roleImproveExhibitionViewPrefab = ResMgr.instance:Load(ROLE_IMPROVE_EXHIBITION_3D_VIEW_PATH)
  t.roleImproveExhibitionViewGameObject = GameObject.Instantiate(roleImproveExhibitionViewPrefab)
  t.roleImproveExhibitionViewGameObject.transform:SetParent(UIMgr.instance.ui3DRoot, false)
  
  t.roleModelRoot = t.roleImproveExhibitionViewGameObject.transform:Find('role_model_root')
  
  t.roleInfoPanel = t.transform:Find('core/frame/role_info_panel_root/role_info_panel')
  t.roleInfoPanel.localPosition = Vector3(1136, 0, 0)
  
  t.roleTypeImage = t.roleInfoPanel:Find('hero_attr_root/img_hero_type_icon'):GetComponent(typeof(Image))
  t.heroNameText2 = t.roleInfoPanel:Find('hero_attr_root/txt_hero_name'):GetComponent(typeof(Text))
  
  t.levelRoot = t.roleInfoPanel:Find('level_root')
  t.levelRoot.gameObject:SetActive(t.exhibitionType == t.ExhibitionType.Breakthrough)
  t.starsRoot = t.roleInfoPanel:Find('stars_root')
  t.starsRoot.gameObject:SetActive(t.exhibitionType == t.ExhibitionType.Advance)
  
  t.oldLevelMaxText = t.levelRoot:Find('text_old_level_max'):GetComponent(typeof(Text))
  t.newLevelMaxText = t.levelRoot:Find('text_new_level_max'):GetComponent(typeof(Text))
  t.starTransforms = {}
  for i = 1, 6 do
    t.starTransforms[i] = t.starsRoot:Find('img_star_'..i)
  end
  t.newStarEffectTransform = t.starsRoot:Find('ui_shengxing_01')
  t.newStarEffectTransform.gameObject:SetActive(false)
  
  --attr
  t.advanceRoleAttrScrollViewTransfrom = t.roleInfoPanel:Find('advance_role_attr_scroll_view')
  t.breakthroughRoleAttrScrollViewTransform = t.roleInfoPanel:Find('breakthrough_role_attr_scroll_view')
  t.advanceRoleAttrScrollViewTransfrom.gameObject:SetActive(t.exhibitionType == t.ExhibitionType.Advance)
  t.breakthroughRoleAttrScrollViewTransform.gameObject:SetActive(t.exhibitionType == t.ExhibitionType.Breakthrough)
  
  t.breakthroughRoleAttrTable = {}
  local breakthroughRoleAttrScrollContent = t.roleInfoPanel:Find('breakthrough_role_attr_scroll_view/Viewport/Content')
  local child = nil
  for i = 1, 4 do
    local breakthroughRoleAttrItem = {}
    child = breakthroughRoleAttrScrollContent:GetChild(i-1)
    breakthroughRoleAttrItem.textTitle = child:Find('role_attribute_title'):GetComponent(typeof(Text))
    breakthroughRoleAttrItem.textValue = child:Find('role_attribute_value'):GetComponent(typeof(Text))
    breakthroughRoleAttrItem.textAdd = child:Find('role_attribute_add_value'):GetComponent(typeof(Text))
    t.breakthroughRoleAttrTable[i] = breakthroughRoleAttrItem
  end
  
  t.advanceRoleAttrTable = {}
  local advanceRoleAttrScrollContent = t.roleInfoPanel:Find('advance_role_attr_scroll_view/Viewport/Content')
  for i = 1, 4 do
    local advanceRoleAttrItem = {}
    child = advanceRoleAttrScrollContent:GetChild(i-1)
    advanceRoleAttrItem.titleText = child:Find('text_role_attribute_title'):GetComponent(typeof(Text))
    advanceRoleAttrItem.oldValueText = child:Find('text_role_attribute_old_value'):GetComponent(typeof(Text))
    advanceRoleAttrItem.newValueText = child:Find('text_role_attribute_new_value'):GetComponent(typeof(Text))
    t.advanceRoleAttrTable[i] = advanceRoleAttrItem
  end
  
  t.skill_root = t.roleInfoPanel:Find('skills_root')
  t.skillIconTable  = {}
  t.skillIconTable[1] = common_skill_icon.BindTransform(t.skill_root:Find('img_skill_01'))
  t.skillIconTable[2] = common_skill_icon.BindTransform(t.skill_root:Find('img_skill_02'))
  t.skillIconTable[3] = common_skill_icon.BindTransform(t.skill_root:Find('passive/img_skill_03'))
  
  t.RegisterObserver()
end

function t.RegisterObserver ()
  Observers.Facade.Instance:RegisterObserver('AnimationEventLuaForwarder::OnAnimationEventTriggered', t.OnAnimationEventTriggered)
end

function t.RemoveObserver ()
  Observers.Facade.Instance:RemoveObserver('AnimationEventLuaForwarder::OnAnimationEventTriggered', t.OnAnimationEventTriggered)
end

function t.SetRoleInfo (roleInfo)
  t.roleInfo = roleInfo
  
  Common.Util.TransformUtil.ClearChildren(t.roleModelRoot, true)
 
  if t.roleInfo.instanceID == game_model.playerInfo.instanceID then
    t.mockOldRoleInfo = game_model.playerInfo:GetCopyPlayerInfo()
  else
    t.mockOldRoleInfo = t.roleInfo:GetHeroInfoCopy()
  end
  
  if t.exhibitionType == t.ExhibitionType.Breakthrough then
    t.mockOldRoleInfo.breakthroughLevel = t.mockOldRoleInfo.breakthroughLevel - 1
  elseif t.exhibitionType == t.ExhibitionType.Advance then
    t.mockOldRoleInfo.advanceLevel = t.mockOldRoleInfo.advanceLevel - 1
  end
  
  t.RefreshHeroInfoPanel()
  
  t.oldLevelMaxText.text = t.roleInfo.level
  t.newLevelMaxText.text = t.roleInfo:MaxLevel()
  for i = 1, 6 do
    t.starTransforms[i].gameObject:SetActive(i <= t.roleInfo.advanceLevel)
  end
  t.starTransforms[t.roleInfo.advanceLevel]:GetComponent(typeof(Image)):CrossFadeAlpha(0, 0, true)
  t.newStarEffectTransform:SetParent(t.starTransforms[t.roleInfo.advanceLevel], false)
  t.newStarEffectTransform.localPosition = Vector3(0, 0, 0)
  
  if t.roleInfo.instanceID == game_model.playerInfo.instanceID then
    t.characterEntity = CharacterEntity.CreatePlayerEntityAs3DUIElementByPlayerInfoLuaTable(game_model.playerInfo, t.roleModelRoot, false, false)
    Logic.Shaders.ShadersUtil.SetShaderKeyword(t.characterEntity.petEntity, Logic.Shaders.ShadersUtil.RIMLIGHT_OFF, Logic.Shaders.ShadersUtil.RIMLIGHT_ON)
    Logic.Shaders.ShadersUtil.SetMainColor(t.characterEntity.petEntity, Logic.Shaders.ShadersUtil.MAIN_COLOR)
  else
    t.characterEntity = CharacterEntity.CreateHeroEntityAs3DUIElementByHeroInfoLuaTable(t.roleInfo, t.roleModelRoot, false, false)
  end
  Logic.Shaders.ShadersUtil.SetShaderKeyword(t.characterEntity, Logic.Shaders.ShadersUtil.RIMLIGHT_OFF, Logic.Shaders.ShadersUtil.RIMLIGHT_ON)
  Logic.Shaders.ShadersUtil.SetMainColor(t.characterEntity, Logic.Shaders.ShadersUtil.MAIN_COLOR)
  
  if t.exhibitionType == t.ExhibitionType.Breakthrough then
  elseif t.exhibitionType == t.ExhibitionType.Advance then
    if t.roleInfo.instanceID == game_model.playerInfo.instanceID then
      t.oldCharacterEntity = CharacterEntity.CreatePlayerEntityAs3DUIElementByPlayerInfoLuaTable(t.mockOldRoleInfo, t.roleModelRoot, false, false)
      Logic.Shaders.ShadersUtil.SetShaderKeyword(t.oldCharacterEntity.petEntity, Logic.Shaders.ShadersUtil.RIMLIGHT_OFF, Logic.Shaders.ShadersUtil.RIMLIGHT_ON)
      Logic.Shaders.ShadersUtil.SetMainColor(t.oldCharacterEntity.petEntity, Logic.Shaders.ShadersUtil.MAIN_COLOR)
    else
      t.oldCharacterEntity = CharacterEntity.CreateHeroEntityAs3DUIElementByHeroInfoLuaTable(t.mockOldRoleInfo, t.roleModelRoot, false, false)
    end
    Logic.Shaders.ShadersUtil.SetShaderKeyword(t.oldCharacterEntity, Logic.Shaders.ShadersUtil.RIMLIGHT_OFF, Logic.Shaders.ShadersUtil.RIMLIGHT_ON)
    Logic.Shaders.ShadersUtil.SetMainColor(t.oldCharacterEntity, Logic.Shaders.ShadersUtil.MAIN_COLOR)

    t.oldCharacterEntity.gameObject:SetActive(true)
    t.characterEntity.gameObject:SetActive(false)
  end
end

function t.DespawnCharacter ()
  if t.oldCharacterEntity then
    PoolController.instance:Despawn(t.oldCharacterEntity.name, t.oldCharacterEntity)
    t.oldCharacterEntity = nil
  end
  if t.characterEntity then
    PoolController.instance:Despawn(t.characterEntity.name, t.characterEntity)
    t.characterEntity = nil
  end
end

function t.RefreshHeroInfoPanel ()
  t.roleTypeImage.sprite = ui_util.GetRoleTypeBigIconSprite(t.roleInfo.heroData.roleType)
  t.heroNameText2.text = t.roleInfo.heroData:GetNameWithQualityColor()
  
  -- [[ ATTRIBUTES ]] --
  local oldAttrList = role_model.CalcRoleAttributesDic(t.mockOldRoleInfo)
  local oldShowAttrList = {}
  local attrList = role_model.CalcRoleAttributesDic(t.roleInfo)
  local showAttrList = {}
  
  oldShowAttrList[1] = oldAttrList[RoleAttributeType.HP]
  showAttrList[1] = attrList[RoleAttributeType.HP]
  if t.roleInfo:GetRoleAttackAttributeType() == RoleAttackAttributeType.PhysicalAttack then
    oldShowAttrList[2] = oldAttrList[RoleAttributeType.NormalAtk]
    showAttrList[2] = attrList[RoleAttributeType.NormalAtk]
  else
    oldShowAttrList[2] = oldAttrList[RoleAttributeType.MagicAtk]
    showAttrList[2] = attrList[RoleAttributeType.MagicAtk]
  end
  oldShowAttrList[3] = oldAttrList[RoleAttributeType.Def]
  showAttrList[3] = attrList[RoleAttributeType.Def]
  oldShowAttrList[4] = oldAttrList[RoleAttributeType.Speed]
  showAttrList[4] = attrList[RoleAttributeType.Speed]
  
  local curAttr = nil
  local addAttr = nil
  for k,v in pairs(t.breakthroughRoleAttrTable) do
    curAttr = showAttrList[k]
    v.textTitle.text = curAttr:GetName()
    v.textValue.text = curAttr:GetValueString()
    v.textAdd.text = ''
  end
  
  local oldAttr = nil
  local newAttr = nil
  for k, v in pairs(t.advanceRoleAttrTable) do
    oldAttr = oldShowAttrList[k]
    newAttr = showAttrList[k]
    v.titleText.text = oldAttr:GetName()
    v.oldValueText.text = oldAttr:GetValueString()
    v.newValueText.text = newAttr:GetValueString()

  end
  -- [[ ATTRIBUTES ]] --
  
  t.skillIconTable[1]:SetSkillData(t.roleInfo.heroData.skillId1, t.roleInfo.advanceLevel, t.roleInfo.heroData.starMin,false)
  t.skillIconTable[2]:SetSkillData(t.roleInfo.heroData.skillId2, t.roleInfo.advanceLevel, t.roleInfo.heroData.starMin,false)
  t.skillIconTable[3]:SetSkillData(t.roleInfo.heroData.passiveId1, t.roleInfo.advanceLevel, t.roleInfo.heroData.starMin,true)
end

function t.PlayNewStarEffect ()
  t.newStarEffectTransform.gameObject:SetActive(false)
  t.newStarEffectTransform.gameObject:SetActive(true)
  
  LeanTween.delayedCall(t.transform.gameObject, 0.7, Action(function ()
    t.ShowNewStar ()
    end)
  )
end

function t.ShowNewStar ()
  t.starTransforms[t.roleInfo.advanceLevel]:GetComponent(typeof(Image)):CrossFadeAlpha(1, 0, true)
end

function t.ShowFrame ()
  t.roleInfoPanel.localPosition = Vector3(1136, 0, 0)
  local ltDescr = LeanTween.moveLocalX(t.roleInfoPanel.gameObject, 0, 0.7)
  
  if t.exhibitionType == t.ExhibitionType.Advance then
    LeanTween.delayedCall(t.transform.gameObject, 1.2, Action(function ()
      t.PlayNewStarEffect()
      end)
    )
  end
end

function t.OnShouldReplaceRoleModel ()
  if t.exhibitionType == t.ExhibitionType.Breakthrough then
  elseif t.exhibitionType == t.ExhibitionType.Advance then
    t.oldCharacterEntity.gameObject:SetActive(false)
    t.characterEntity.gameObject:SetActive(true)
  end
  Logic.Action.Controller.ActionController.instance:PlayerAnimAction(t.characterEntity, Common.Animators.AnimatorUtil.VICOTRY_ID)
  
end

function t.OnCloseUpEnd ()
  t.ShowFrame()
end

function t.OnAnimationEnd ()
  local modelRotateAndAnim = t.characterEntity:GetComponent(typeof(Logic.Model.View.ModelRotateAndAnim))
  --[[
  if modelRotateAndAnim ~= nil then
    modelRotateAndAnim.canClick = true
    modelRotateAndAnim.canDrag = true
  end
  ]]--
  t.closeButton.gameObject:SetActive(true)
end

function t.OnAnimationEventTriggered (notification)
  if notification.Body == 'OnShouldReplaceRoleModel' then
    t.OnShouldReplaceRoleModel()
  elseif notification.Body == 'OnCloseUpEnd' then
    t.OnCloseUpEnd()
  elseif notification.Body == 'OnAnimationEnd' then
    t.OnAnimationEnd()
  end
  return true
end

function t.ClickCloseButton ()
  t.Close()
end

return t