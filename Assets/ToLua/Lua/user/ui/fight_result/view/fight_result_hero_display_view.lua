local t = {}
local PREFAB_PATH = 'ui/fight_result/fight_result_hero_display_view'

local role_model = gamemanager.GetModel('role_model')
local hero_model = gamemanager.GetModel('hero_model')
local hero_info = require('ui/hero/model/hero_info')

local common_skill_icon = require('ui/common_icon/common_skill_icon')

function t.Open(modelId, star, onViewCloseHandler, showPreheatEffect)
  t.showPreheatEffect = showPreheatEffect
  t.OnViewCloseDelegate = void_delegate.New()
  t.OnViewCloseDelegate:RemoveAllListener()
  if onViewCloseHandler ~= nil then
    t.OnViewCloseDelegate:AddListener(onViewCloseHandler)
  end
  t.spriteStarNormal = Common.ResMgr.ResMgr.instance:LoadSprite('sprite/main_ui/icon_star')
  t.spriteStarDefault = Common.ResMgr.ResMgr.instance:LoadSprite('sprite/main_ui/icon_star2_big_disable')

  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
  t.roleInfo = hero_info:New(0,  modelId,  1,  0,  star,  1)
  t.InitComponent()
  
  

  
  LeanTween.delayedCall(0.1, Action(t.OnViewReady)) 
end

function t.OnViewReady ()
  Observers.Facade.Instance:SendNotification(string.format('%s::%s', PREFAB_PATH, 'OnViewReady'))
end

function t.Close()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.DespawnCharacter(t.characterEntity)
  Observers.Facade.Instance:SendNotification(string.format("%s::%s", 'ui/fight_result/fight_result_view', "OnViewStay"))
  Observers.Facade.Instance:SendNotification(string.format("%s::%s", 'ui/fight_result/fight_result_hero_display_view', "OnViewClose"))
end

function t.InitComponent()
  t.IsShowCompleted = false
  t.mask = t.transform:Find('core/img_mask')
  t.mask.gameObject:SetActive(true)
  
  t.preheatEffect = t.transform:Find('core/ui_chouka_dan_ani')
  t.preheatEffect.gameObject:SetActive(false)
  
  t.roleExhibitionRoot = t.transform:Find('core/role_exhibition_root')
  t.heroNameText = t.roleExhibitionRoot:Find('hero_title_root/text_hero_name'):GetComponent(typeof(Text))
  t.imgHeroStars = {}
  for i = 1, 6 do
    t.imgHeroStars[i] = t.roleExhibitionRoot:Find('hero_title_root/hero_star_root/img_star_'..i):GetComponent(typeof(Image))
  end
  t.heroModelRoot = t.roleExhibitionRoot:Find('hero_model_root')
  
  t.closeButton = t.transform:Find('core/btn_back'):GetComponent(typeof(Button))
  t.closeButton.onClick:RemoveAllListeners()
  t.closeButton.onClick:AddListener(t.ClickCloseHandler)
  t.goEffectDrawCard = t.roleExhibitionRoot:Find('ui_chouka_dan').gameObject
  t.goEffectDrawCard:SetActive(false)
  
  t.roleInfoPanel = t.transform:Find('core/role_info_panel_root/role_info_panel')
  t.roleInfoPanel.localPosition = Vector3(1136, 0, 0)
  
  t.roleRankRoot = t.transform:Find('core/img_role_rank_root')
  t.roleRankRoot.gameObject:SetActive(false)
  t.roleRankBGImage = t.transform:Find('core/img_role_rank_root/img_role_rank_bg'):GetComponent(typeof(Image))
  t.roleRankRoleTypeImage = t.transform:Find('core/img_role_rank_root/img_role_rank_role_type'):GetComponent(typeof(Image))
  t.roleTypeImage = t.roleInfoPanel:Find('hero_attr_root/img_hero_type_icon'):GetComponent(typeof(Image))
  t.heroNameText2 = t.roleInfoPanel:Find('hero_attr_root/txt_hero_name'):GetComponent(typeof(Text))
  
  --attr
  t.tranAttrTable = {}
  local scrollContent = t.roleInfoPanel:Find('Scroll View/Viewport/Content')
  local child = nil
  for i = 1, 4 do
    local tb = {}
    child = scrollContent:GetChild(i-1)
    tb.textTitle = child:Find('role_attribute_title'):GetComponent(typeof(Text))
    tb.textValue = child:Find('role_attribute_value'):GetComponent(typeof(Text))
    tb.textAdd = child:Find('role_attribute_add_value'):GetComponent(typeof(Text))
    t.tranAttrTable[i] = tb
  end
  
  t.heroDescriptionText = t.roleInfoPanel:Find('text_hero_description'):GetComponent(typeof(Text))
  
  t.skill_root = t.roleInfoPanel:Find('skills_root')
  t.skillIconTable  = {}
  t.skillIconTable[1] = common_skill_icon.BindTransform(t.skill_root:Find('img_skill_01'))
  t.skillIconTable[2] = common_skill_icon.BindTransform(t.skill_root:Find('img_skill_02'))
  t.skillIconTable[3] = common_skill_icon.BindTransform(t.skill_root:Find('passive/img_skill_03'))
  
  t.clickAnyWhereToQuitTipsText = t.transform:Find('core/img_bottom_bar/text_click_any_where_to_quit_tips'):GetComponent(typeof(Text))
  t.clickAnyWhereToQuitTipsText:CrossFadeAlpha(0, 0, true)

  t.roleExhibitionRoot.gameObject:SetActive(false)
  
  if t.showPreheatEffect then
    t.ltDescr0 = LeanTween.delayedCall(1.1, Action(t.StartToShowHero))
    t.preheatEffect.gameObject:SetActive(true)
  else
    t.StartToShowHero()
  end
end

function t.StartToShowHero ()
  AudioController.instance:PlayAudio( 'gainHero',false, 0)
  t.mask.gameObject:SetActive(false)
  t.Refresh()
  t.roleExhibitionRoot.gameObject:SetActive(true)
  t.ltDescr1 = LeanTween.moveLocalX(t.roleExhibitionRoot.gameObject, -120, 0.3):setEase(LeanTweenType.easeInOutSine):setDelay(2.8)
  t.ltDescr2 = LeanTween.moveLocalX(t.roleInfoPanel.gameObject, 0, 0.7):setEase(LeanTweenType.easeInOutSine):setDelay(2.6)
  t.ltDescr3 = LeanTween.delayedCall(4.2, Action(function ()
      t.clickAnyWhereToQuitTipsText:CrossFadeAlpha(1, 0.5, true)
    end)
  )
  t.ltDescr4 = LeanTween.delayedCall(4.5, Action(function ()
      t.IsShowCompleted = true
    end)
  )
end

function t.Refresh()
  t.ResetHero()
  t.RefreshHeroInfoPanel()
end
function t.ResetHero ()
  t.heroNameText.text = t.roleInfo.heroData:GetNameWithQualityColor()
  for i = 1, 6 do
    t.imgHeroStars[i].gameObject:SetActive(i <= t.roleInfo:MaxAdvanceLevel())
    if i<= t.roleInfo.advanceLevel then
      t.imgHeroStars[i].sprite = t.spriteStarNormal
    else
      t.imgHeroStars[i].sprite = t.spriteStarDefault
    end
  end
  t.DespawnCharacter(t.characterEntity)
  t.characterEntity = Logic.Character.CharacterEntity.CreateHeroEntityAsUIElementByHeroInfoLuaTable(t.roleInfo, t.heroModelRoot, false, true)
  coroutine.start(t.ShowEffectCoroutine)
  
end
function t.RefreshHeroInfoPanel ()
  t.roleRankBGImage.sprite = ResMgr.instance:LoadSprite(t.roleInfo.heroData:GetRankImagePath())
  t.roleRankRoleTypeImage.sprite = t.roleInfo.heroData:GetRankRoleTypeSprite()
  t.roleRankRoot.gameObject:SetActive(true)
  t.roleTypeImage.sprite = ui_util.GetRoleTypeBigIconSprite(t.roleInfo.heroData.roleType)
  t.heroNameText2.text = t.roleInfo.heroData:GetNameWithQualityColor()
  
  -- [[ ATTRIBUTES ]] --
  local attrList = role_model.CalcRoleAttributesDic(t.roleInfo)
  local showAttrList = {}
  showAttrList[1] = attrList[RoleAttributeType.HP]
  if t.roleInfo:GetRoleAttackAttributeType() == RoleAttackAttributeType.PhysicalAttack then
    showAttrList[2] = attrList[RoleAttributeType.NormalAtk]
  else
    showAttrList[2] = attrList[RoleAttributeType.MagicAtk]
  end
  showAttrList[3] = attrList[RoleAttributeType.Def]
  showAttrList[4] = attrList[RoleAttributeType.Speed]
  
  local curAttr = nil
  local addAttr = nil
  for k,v in pairs(t.tranAttrTable) do
    curAttr = showAttrList[k]
    v.textTitle.text = curAttr:GetName()
    v.textValue.text = curAttr:GetValueString()
    v.textAdd.text = ''
  end
  -- [[ ATTRIBUTES ]] --
  
  t.heroDescriptionText.text = LocalizationController.instance:Get(t.roleInfo.heroData.description)
  
  t.skillIconTable[1]:SetSkillData(t.roleInfo.heroData.skillId1, t.roleInfo.advanceLevel, t.roleInfo.heroData.starMin,false)
  t.skillIconTable[2]:SetSkillData(t.roleInfo.heroData.skillId2, t.roleInfo.advanceLevel, t.roleInfo.heroData.starMin,false)
  t.skillIconTable[3]:SetSkillData(t.roleInfo.heroData.passiveId1, t.roleInfo.advanceLevel, t.roleInfo.heroData.starMin,true)
end
function t.ShowEffectCoroutine()
  t.heroModelRoot.gameObject:SetActive(false)
  coroutine.wait(0.02)
  
  t.goEffectDrawCard:SetActive(false)
  t.goEffectDrawCard:SetActive(true)
  coroutine.wait(0.1)
  t.heroModelRoot.gameObject:SetActive(true)
  AnimatorUtil.CrossFade(t.characterEntity.anim,AnimatorUtil.VICOTRY_ID,0.3)
end
function t.DespawnCharacter(characterEntity)
  if characterEntity ~= nil then
    Logic.Pool.Controller.PoolController.instance:Despawn(characterEntity.name, characterEntity)
    if t.characterEntity == characterEntity then
      t.characterEntity = nil
    end
  end
end

-----------------click event----------------------
function t.ClickCloseHandler()
  if t.IsShowCompleted then
    t.OnViewCloseDelegate:Invoke()
    t.Close()
  elseif t.ltDescr0 ~= nil then
    t.ltDescr0:setDelay(0)
    t.ltDescr0:setTime(0)
    t.preheatEffect.gameObject:SetActive(false)
    t.ltDescr0 = nil
  else
    if t.ltDescr1 ~= nil then
      t.ltDescr1:setDelay(0)
    end
    if t.ltDescr2 ~= nil then
      t.ltDescr2:setDelay(0)
    end
    if t.ltDescr3 ~= nil then
      t.ltDescr3:setDelay(0)
      t.ltDescr3:setTime(0.8)
    end
    if t.ltDescr4 ~= nil then
      t.ltDescr4:setDelay(0)
      t.ltDescr4:setTime(1)
    end
  end
end
return t