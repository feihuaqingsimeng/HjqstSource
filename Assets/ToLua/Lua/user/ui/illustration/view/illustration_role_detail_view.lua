local t = {}
local PREFAB_PATH = 'ui/illustrated_handbook/illustration_detail_view'
local name = PREFAB_PATH

local role_model = gamemanager.GetModel('role_model')
local game_model = gamemanager.GetModel('game_model')
local hero_model = gamemanager.GetModel('hero_model')
local illustration_ctrl = gamemanager.GetCtrl('illustration_ctrl')
local common_skill_icon = require('ui/common_icon/common_skill_icon')

function t.Open(roleInfo)
  if uimanager.GetView(name) then
    return
  end
  uimanager.RegisterView(name,t)
  
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  local common_top_bar = require ('ui/common_top_bar/common_top_bar')
  common_top_bar = common_top_bar:Create(t.transform:Find('core'))
  common_top_bar:SetAsCommonStyle(LocalizationController.instance:Get("ui.illustration_view.roleFormation"),t.OnClickBackBtnHandler,true,true,true,false,false,false,false)
  
  
  
  t.roleInfo = roleInfo
  t.attrTypeList = MList.New('number')
  
  
  
  t.InitComponent()
  t.InitAttribute()
  t.Refresh()
end
function t.Close()
  
  t.DespawnCharacter()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  
end


function t.InitComponent()
  local role_info_panel = t.transform:Find('core/role_info_panel')
  
  t.imgRoleType = role_info_panel:Find('img_hero_type_icon'):GetComponent(typeof(Image))
  t.textRoleName = role_info_panel:Find('name_root/txt_hero_name'):GetComponent(typeof(Text))
  t.textLevel = role_info_panel:Find('text_hero_level'):GetComponent(typeof(Text))
  t.sliderExp = role_info_panel:Find('slider_hero_exp'):GetComponent(typeof(Slider))
  t.textExp = role_info_panel:Find('text_hero_exp_percent'):GetComponent(typeof(Text))
  t.textDescription = t.transform:Find('core/img_des_bg/Scroll View/Viewport/Content/text_des'):GetComponent(typeof(Text))
  --attr
  t.tranRoleAttributeRoot = role_info_panel:Find('Scroll View/Viewport/Content')
  t.goAttrPrefab = role_info_panel:Find('Scroll View/Viewport/hp_item').gameObject
  t.goAttrPrefab:SetActive(false)
  --skill
  local skill_root = role_info_panel:Find('skills_root')
  t.skillIconTable  = {}
  t.skillIconTable[1] = common_skill_icon.BindTransform(skill_root:Find('img_skill_01'))
  t.skillIconTable[2] = common_skill_icon.BindTransform(skill_root:Find('img_skill_02'))
  t.skillIconTable[3] = common_skill_icon.BindTransform(skill_root:Find('passive/img_skill_03'))
  --model
  t.roleRankImage = t.transform:Find('core/img_role_rank_bg/img_role_rank'):GetComponent(typeof(Image))
  t.roleRankRoleTypeImage = t.roleRankImage.transform:Find('img_role_rank_type'):GetComponent(typeof(Image))
  t.tranModelRoot = t.transform:Find('core/role_model_anchor/role_model_root')
  t.btnClickHeroVictory = t.transform:Find('core/role_model_anchor/btn_click_hero'):GetComponent(typeof(Button))
  t.btnClickHeroVictory.onClick:AddListener(t.ClickHeroVictoryHandler)
  --btn
  role_info_panel:Find('btn_show_skill'):GetComponent(typeof(Button)).onClick:AddListener(t.ClickShowSkillBtnHandler)
  role_info_panel:Find('btn_path'):GetComponent(typeof(Button)).onClick:AddListener(t.ClickPathBtnHandler)
  role_info_panel:Find('btn_highStatus'):GetComponent(typeof(Button)).onClick:AddListener(t.ClickHighStatusBtnHandler)
  role_info_panel:Find('btn_relationship'):GetComponent(typeof(Button)).onClick:AddListener(t.ClickRelationshipBtnHandler)
  
end

function t.InitAttribute()
  t.attrTypeList:Clear()
  t.attrTypeList:Add(RoleAttributeType.HP)
  if t.roleInfo:GetRoleAttackAttributeType() == RoleAttackAttributeType.PhysicalAttack then
    t.attrTypeList:Add(RoleAttributeType.NormalAtk)
  else
    t.attrTypeList:Add(RoleAttributeType.MagicAtk)
  end
  t.attrTypeList:Add(RoleAttributeType.Def)
  t.attrTypeList:Add(RoleAttributeType.Speed)
end
function t.Refresh()
  t.RefreshRoleRank()
  t.RefreshTitle()
  t.RefreshAttribute()
  t.RefreshRoleModel()
  t.RefreshSkill()
end

function t.RefreshRoleRank ()
  t.roleRankImage.sprite = ResMgr.instance:LoadSprite(t.roleInfo.heroData:GetRankImagePath())
  t.roleRankRoleTypeImage.sprite = t.roleInfo.heroData:GetRankRoleTypeSprite()
end

function t.RefreshTitle()
  t.imgRoleType.sprite = ui_util.GetRoleTypeSmallIconSprite(t.roleInfo.heroData.roleType)
  t.textRoleName.text = t.roleInfo.heroData:GetNameWithQualityColor()
  t.textLevel.text = string.format("%d/%d", t.roleInfo.level, gamemanager.GetData('global_data').player_lv_max)
  t.sliderExp.value = 0
  t.textExp.text = '0%'
  t.textDescription.text = LocalizationController.instance:Get(t.roleInfo.heroData.description)
end
function  t.RefreshAttribute()
  ui_util.ClearChildren(t.tranRoleAttributeRoot, true)
  local attrDic = role_model.CalcRoleAttributesDic(t.roleInfo)
  for k, v in pairs(t.attrTypeList:GetDatas()) do
    local go = GameObject.Instantiate(t.goAttrPrefab)
    go:SetActive(true)
    local tran = go.transform
    tran:SetParent(t.tranRoleAttributeRoot,false)
    tran:Find('text_title'):GetComponent(typeof(Text)).text = attrDic[v]:GetName()
    tran:Find('text_value'):GetComponent(typeof(Text)).text = attrDic[v]:GetValueString()
  end
end

function t.RefreshRoleModel()
  t.DespawnCharacter()
 LeanTween.delayedCall(0.4,Action(t.RefreshRoleModelDelay))
end

function t.RefreshRoleModelDelay()
  
  if t.roleInfo.heroData:IsPlayer() then
    t.characterEntity = Logic.Character.CharacterEntity.CreatePlayerEntityAsUIElementByPlayerInfoLuaTable(t.roleInfo, t.tranModelRoot, false, true)
  else
    t.characterEntity = Logic.Character.CharacterEntity.CreateHeroEntityAsUIElementByHeroInfoLuaTable(t.roleInfo,t.tranModelRoot, false, true)
  end
  t.ClickHeroVictoryHandler()
end

function t.RefreshSkill()
  t.skillIconTable[1]:SetSkillData(t.roleInfo.heroData.skillId1,t.roleInfo.advanceLevel,t.roleInfo.heroData.starMin,false)
  t.skillIconTable[2]:SetSkillData(t.roleInfo.heroData.skillId2,t.roleInfo.advanceLevel,t.roleInfo.heroData.starMin,false)
  t.skillIconTable[3]:SetSkillData(t.roleInfo.heroData.passiveId1,t.roleInfo.advanceLevel,t.roleInfo.heroData.starMin,true)
end

function t.DespawnCharacter()
  if t.characterEntity then
    Logic.Pool.Controller.PoolController.instance:Despawn(t.characterEntity.name, t.characterEntity)
    t.characterEntity = nil
  end
end

-------------------------------click event------------------------------
function t.OnClickBackBtnHandler()
  uimanager.CloseView(name)
end

--技能展示
function t.ClickShowSkillBtnHandler()
  LuaCsTransfer.ClickIllustrationSkillDisplayHandler(t.roleInfo,t.roleInfo.heroData:IsPlayer())
end

---查看来源
function t.ClickPathBtnHandler()
  LuaCsTransfer.OpenGoodsJumpPath(BaseResType.Hero, t.roleInfo.heroData.id,t.roleInfo.advanceLevel)
end
--高级形态
function t.ClickHighStatusBtnHandler()
   illustration_ctrl.OpenIllustrationHeroHighStatusView(t.roleInfo)
end

--羁绊
function t.ClickRelationshipBtnHandler()
  illustration_ctrl.OpenRelationShipView(t.roleInfo.heroData.id)
end

--点击英雄模型播放胜利动画
function t.ClickHeroVictoryHandler()
  
  if t.roleInfo then
    gamemanager.GetModel('audio_model').PlayRandomAudioInView(AudioViewType.mainView,t.roleInfo.heroData.id)
      AnimatorUtil.CrossFade(t.characterEntity.anim,AnimatorUtil.VICOTRY_ID,0.3)
  end
end

return t