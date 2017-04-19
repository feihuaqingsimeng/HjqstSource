local t = {}
local PREFAB_PATH = 'ui/change_profession/change_profession_view'

local global_data = gamemanager.GetData('global_data')
local game_model = gamemanager.GetModel('game_model')
local role_model = gamemanager.GetModel('role_model')
local player_model = gamemanager.GetModel('player_model')
local player_controller = gamemanager.GetCtrl('player_controller')
local common_skill_icon = require('ui/common_icon/common_skill_icon')
local player_info = require('ui/player/model/player_info')
local hero_data = gamemanager.GetModel('hero_data')

local profession_item = require('ui/player/view/profession_item')

t.isSkillsPanelShowing = true
t.professionItems = {}

function t.Open ()
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI, UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform)) 
   
  local common_top_bar = require ('ui/common_top_bar/common_top_bar')
  common_top_bar = common_top_bar:Create(t.transform:Find('core'))
  common_top_bar.transform:SetAsFirstSibling()
  common_top_bar:SetAsCommonStyle(Common.Localization.LocalizationController.instance:Get('ui.change_profession_view.change_profession_title'), t.ClickCloseButton, true, true, true, true, false, false, false)
  
  t.InitComponent()
  t.BindDelegate()
end

function t.InitComponent ()
  t.roleModelRootTransform = t.transform:Find('core/player_model_root/model_root')
  t.activateProfessionFXGameObject = t.transform:Find('core/player_model_root/ui_shimojihuo').gameObject
  t.activateProfessionFXGameObject:SetActive(false)
  
  t.changeProfessionText = t.transform:Find('core/btn_change_profession/text'):GetComponent(typeof(Text))
  t.changeProfessionButton = t.transform:Find('core/btn_change_profession'):GetComponent(typeof(Button))
  t.changeProfessionButton.onClick:AddListener(t.ClickChangeProfessionButton)
  
  t.infoFrameTransform = t.transform:Find('core/right_anchor/img_info_frame')
  
  t.professionNameText = t.infoFrameTransform:Find('text_profession_name'):GetComponent(typeof(Text))
  t.playerLevelText = t.infoFrameTransform:Find('text_player_level'):GetComponent(typeof(Text))
  t.playerLevelMaxText = t.infoFrameTransform:Find('text_player_level_max'):GetComponent(typeof(Text))
  t.playerExpSlider = t.infoFrameTransform:Find('slider_player_exp'):GetComponent(typeof(Slider))
  t.playerExpPercentText = t.infoFrameTransform:Find('text_player_exp_percent'):GetComponent(typeof(Text))
  
  t.showHideSkillButton = t.infoFrameTransform:Find('skills_root/skills_panel_root/btn_bar'):GetComponent(typeof(Button))
  t.showHideSkillButton.onClick:AddListener(t.ClickShowHideSkillsPanel)
  t.skillsPanelShowHideArrowTransform = t.showHideSkillButton.transform:Find('img_arrow')
  t.skillsPanelRootGameObject = t.infoFrameTransform:Find('skills_root/skills_panel_root').gameObject
  t.skillPanelHidePosTransform = t.infoFrameTransform:Find('skills_root/skills_panel_hide_pos')
  t.skillPanelShowPosTransform = t.infoFrameTransform:Find('skills_root/skills_panel_show_pos')

  t.hpText = t.infoFrameTransform:Find('basic_info_root/text_hp'):GetComponent(typeof(Text))
  t.attackText = t.infoFrameTransform:Find('basic_info_root/text_attack'):GetComponent(typeof(Text))
  t.defenceText = t.infoFrameTransform:Find('basic_info_root/text_defence'):GetComponent(typeof(Text))
  t.speedText = t.infoFrameTransform:Find('basic_info_root/text_speed'):GetComponent(typeof(Text))
  t.critText = t.infoFrameTransform:Find('basic_info_root/text_critical'):GetComponent(typeof(Text))
  t.dodgeText = t.infoFrameTransform:Find('basic_info_root/text_dodge'):GetComponent(typeof(Text))
  t.blockText = t.infoFrameTransform:Find('basic_info_root/text_block'):GetComponent(typeof(Text))
  t.hitText = t.infoFrameTransform:Find('basic_info_root/text_hit'):GetComponent(typeof(Text))
  
  -- [[ Skill Buttons ]] --
  local skillIconsRoot = t.infoFrameTransform:Find('skills_root/skills_panel_root/skills_root')
  t.skillIconTable  = {}
  t.skillIconTable[1] = common_skill_icon.BindTransform(skillIconsRoot:Find('img_skill_01'))
  t.skillIconTable[2] = common_skill_icon.BindTransform(skillIconsRoot:Find('img_skill_02'))
  t.skillIconTable[3] = common_skill_icon.BindTransform(skillIconsRoot:Find('passive/img_skill_03'))
  -- [[ Skill Buttons ]] --
  
  t.GenerateProfessionButtons()
  t.Refresh()
end

function t.BindDelegate ()
  player_model.OnPlayerInfoUpdateDelegate:AddListener(t.OnPlayerInfoUpdateHandler)
  player_model.OnPlayerActivateProfessionSuccessDelegate:AddListener(t.OnPlayerActivateProfessionSuccessHandler)
end

function t.UnbindDelegate ()
  player_model.OnPlayerInfoUpdateDelegate:RemoveListener(t.OnPlayerInfoUpdateHandler)
  player_model.OnPlayerActivateProfessionSuccessDelegate:RemoveListener(t.OnPlayerActivateProfessionSuccessHandler)
end

function t.DespawnCharacter()
  if t.characterEntity then
    PoolController.instance:Despawn(t.characterEntity.name,t.characterEntity)
    t.characterEntity = nil
  end
end

function t.GenerateProfessionButtons ()
  t.professionButtonsRoot = t.infoFrameTransform:Find('professions_root/scrollrect/profession_buttons_root')
  t.professionButtonPrefab = t.infoFrameTransform:Find('professions_root/scrollrect/profession_buttons_root/btn_profession')
  t.professionButtonPrefab.gameObject:SetActive(false)
  
  local currentPlayerInfo = game_model.playerInfo
  t.professionItems = {}
  
  local currentPlayerProfessionButton = Instantiate(t.professionButtonPrefab)
  local currentPlayerProfessionItem = profession_item.NewByGameObject(currentPlayerProfessionButton, currentPlayerInfo)
  currentPlayerProfessionItem.transform:GetComponent(typeof(Button)).onClick:AddListener(function ()
          t.ClickProfessionItem (currentPlayerProfessionItem)
        end)
  table.insert(t.professionItems, currentPlayerProfessionItem)
  currentPlayerProfessionButton.transform:SetParent(t.professionButtonsRoot, false)
  currentPlayerProfessionButton.gameObject:SetActive(true)
  t.ClickProfessionItem (currentPlayerProfessionItem)
  
  for k, v in pairs(currentPlayerInfo.playerData:GetChangeProfessionPlayerDataList ()) do
    --local playerInfo = player_model.GetPlayerDataCorrespondingPlayerInfo(v.id)
  --  if playerInfo == nil then
     local playerInfo = player_info:New(0,  v.id, currentPlayerInfo.hairCutIndex,  currentPlayerInfo.hairColorIndex, currentPlayerInfo.faceIndex, currentPlayerInfo.name, currentPlayerInfo.skinIndex)
     playerInfo.exp = currentPlayerInfo.exp
     playerInfo.level = currentPlayerInfo.level
     playerInfo.strengthenLevel = currentPlayerInfo.strengthenLevel
     playerInfo.strengthenExp = currentPlayerInfo.strengthenExp
     playerInfo.breakthroughLevel = currentPlayerInfo.breakthroughLevel
    --end
    local professionButton = Instantiate(t.professionButtonPrefab)
    local professionItem = profession_item.NewByGameObject(professionButton, playerInfo)
    professionItem.transform:GetComponent(typeof(Button)).onClick:AddListener(function ()
          t.ClickProfessionItem (professionItem)
        end)
    table.insert(t.professionItems, professionItem)
    professionButton.transform:SetParent(t.professionButtonsRoot, false)
    professionButton.gameObject:SetActive(true)
  end
end

function t.RefreshModel()
  LeanTween.delayedCall(0.05, Action(t.RefreshModelDelay)) 
end

function t.RefreshModelDelay()
  t.DespawnCharacter()
  ui_util.ClearChildren(t.roleModelRootTransform,true)
  t.characterEntity = CharacterEntity.CreatePlayerEntityAsUIElementByPlayerInfoLuaTable(t.selectedPlayerInfo, t.roleModelRootTransform, true, true)
  AnimatorUtil.CrossFade(t.characterEntity.anim,AnimatorUtil.VICOTRY_ID,0.3)
end

function t.RefreshChangeProfessionButton ()
  if player_model.IsPlayerUnlocked(t.selectedPlayerInfo.playerData.id) then
    t.changeProfessionText.text = Common.Localization.LocalizationController.instance:Get('ui.change_profession_view.change_profession')
  else
    t.changeProfessionText.text = Common.Localization.LocalizationController.instance:Get('ui.change_profession_view.transfer_profession')
  end
  
  local shouldShowchangeProfessionButton = not (t.selectedPlayerInfo.playerData.id == game_model.playerInfo.playerData.id)
  t.changeProfessionButton.gameObject:SetActive(shouldShowchangeProfessionButton)
end

function t.RefreshPlayerInfos ()
  t.professionNameText.text = t.selectedPlayerInfo.heroData:GetNameWithQualityColor()
  t.playerLevelText.text = t.selectedPlayerInfo.level
  t.playerLevelMaxText.text = t.selectedPlayerInfo:MaxLevel()
  t.playerExpSlider.value = t.selectedPlayerInfo:ExpPercent()
  t.playerExpPercentText.text = string.format('%d%%',math.floor(t.selectedPlayerInfo:ExpPercent()*100))

  -- [[ Player Attr ]] --
  local attrList = role_model.CalcRoleAttributesDic(t.selectedPlayerInfo)
  local equipAddAttrList = role_model.CalcRoleAttributesDicByEquip(t.selectedPlayerInfo)
  local showAttrList = {}
  showAttrList[1] = attrList[RoleAttributeType.HP]
  if game_model.playerInfo:GetRoleAttackAttributeType() == RoleAttackAttributeType.PhysicalAttack then
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
  
  t.hpText.text = showAttrList[1]:GetValueString()
  t.attackText.text = showAttrList[2]:GetValueString()
  t.defenceText.text = showAttrList[3]:GetValueString()
  t.speedText.text = showAttrList[4]:GetValueString()
  t.critText.text = showAttrList[5]:GetValueString()
  t.dodgeText.text = showAttrList[6]:GetValueString()
  t.blockText.text = showAttrList[7]:GetValueString()
  t.hitText.text = showAttrList[8]:GetValueString()
  -- [[ Player Attr ]] --
end

function t.RefreshSkill ()
  if t.selectedPlayerInfo == nil then
    return
  end
  
  t.skillIconTable[1]:SetSkillData(t.selectedPlayerInfo.heroData.skillId1, t.selectedPlayerInfo.advanceLevel, t.selectedPlayerInfo.heroData.starMin,false)
  t.skillIconTable[2]:SetSkillData(t.selectedPlayerInfo.heroData.skillId2, t.selectedPlayerInfo.advanceLevel, t.selectedPlayerInfo.heroData.starMin,false)
  t.skillIconTable[3]:SetSkillData(t.selectedPlayerInfo.heroData.passiveId1, t.selectedPlayerInfo.advanceLevel, t.selectedPlayerInfo.heroData.starMin,true)
end

function t.RefreshProfessionItems ()
  for k, v in pairs(t.professionItems) do
    v:Refresh()
    if v.playerInfo.instanceID == game_model.playerInfo.instanceID then
      v.transform:SetAsFirstSibling()
    end
  end
end

function t.Refresh ()
  t.RefreshModel()
  t.RefreshChangeProfessionButton()
  t.RefreshPlayerInfos()
  t.RefreshSkill()
  t.RefreshProfessionItems ()
end

function t.Close ()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.DespawnCharacter()
  t.UnbindDelegate()
end

-- [[ proxy callback ]] --
function t.OnPlayerInfoUpdateHandler ()
  t.Refresh()
end

function t.OnPlayerActivateProfessionSuccessHandler ()
  t.activateProfessionFXGameObject:SetActive(false)
  t.activateProfessionFXGameObject:SetActive(true)
end
-- [[ proxy callback ]] --

-- [[ UI event handles ]] --
function t.ClickCloseButton ()
  t.Close()
end

function t.ClickShowHideSkillsPanel ()
  if t.isSkillsPanelShowing then
    LeanTween.moveLocalY(t.skillsPanelRootGameObject, t.skillPanelHidePosTransform.localPosition.y, 0.2)
    t.skillsPanelShowHideArrowTransform.localRotation = Quaternion.Euler(0, 0, 180)
  else
    LeanTween.moveLocalY(t.skillsPanelRootGameObject, t.skillPanelShowPosTransform.localPosition.y, 0.2)
    t.skillsPanelShowHideArrowTransform.localRotation = Quaternion.Euler(0, 0, 0)
  end
  t.isSkillsPanelShowing = not t.isSkillsPanelShowing
end

function t.ClickProfessionItem (professionItem)
  t.selectedPlayerInfo = professionItem.playerInfo
  for k, v in pairs(t.professionItems) do
      v:SetSelected(false)
  end

  professionItem:SetSelected(true)
  t.Refresh()
end

function t.ClickChangeProfessionButton ()
  if player_model.IsPlayerUnlocked(t.selectedPlayerInfo.playerData.id) then
    local playerDataId = t.selectedPlayerInfo.playerData.id
    player_controller.PlayerChangeReq (player_model.GetPlayerDataCorrespondingPlayerInfo(playerDataId).instanceID)
  else
    LuaInterface.LuaCsTransfer.OpenActivateProfessionView(t.selectedPlayerInfo.playerData.id)
  end
end

function t.ClickChangeProfessionStrengthenButton ()
end
-- [[ UI event handles ]] --

return t