local t = {}
local PREFAB_PATH = 'ui/illustrated_handbook/illustration_highstatus_view'
local name = PREFAB_PATH

local role_item = dofile('ui/illustration/view/high_status/role_item')
local common_skill_icon = require('ui/common_icon/common_skill_icon')
local role_model = gamemanager.GetModel('role_model')

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
  t.roleItems = {}
  t.tranPanelPos = {}
  t.attrTypeList = MList.New('number')
  
  t.InitComponent()
  t.InitAttribute()
  t.Refresh()
end
function t.Close()
  for k,v in pairs(t.roleItems) do
    v:Close()
  end
  t.roleItems = nil
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  
end


function t.InitComponent()
  t.tranRoleRoot = t.transform:Find('core/root/role_root')
  t.goRolePrefab = t.transform:Find('core/root/role_template').gameObject
  t.goRolePrefab:SetActive(false)
  --info panel
  local role_info_panel = t.transform:Find('core/role_info_panel')
  t.role_info_panel = role_info_panel
  t.role_info_panel.gameObject:SetActive(false)
  --pos
  for i = 1,3 do
    t.tranPanelPos[i] = t.transform:Find('core/pos'..i)
  end
  --
  t.imgRoleType = role_info_panel:Find('img_hero_type_icon'):GetComponent(typeof(Image))
  t.textRoleName = role_info_panel:Find('txt_hero_name'):GetComponent(typeof(Text))
  t.textLevel = role_info_panel:Find('text_hero_level'):GetComponent(typeof(Text))
  t.sliderExp = role_info_panel:Find('slider_hero_exp'):GetComponent(typeof(Slider))
  t.textExp = role_info_panel:Find('text_hero_exp_percent'):GetComponent(typeof(Text))
  role_info_panel:Find('btn_close'):GetComponent(typeof(Button)).onClick:AddListener(t.ClickCloseInfoPanelHandler)
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
end

function t.Refresh()
  local go = nil
  local tran = nil
  local roleInfo = nil
  for i = 1,3 do
    roleInfo = t.roleInfo:Clone()
    roleInfo.advanceLevel = i*2
    if roleInfo.advanceLevel <= roleInfo:MaxAdvanceLevel() and roleInfo:ModelName() ~= nil then
       go = GameObject.Instantiate(t.goRolePrefab)
      go:SetActive(true)
      tran = go.transform
      tran:SetParent(t.tranRoleRoot,false)
      t.roleItems[i] = role_item.BindTransform(tran)
      t.roleItems[i]:SetRoleInfo(roleInfo,i)
      t.roleItems[i]:SetCheckCallback(t.ClickCheckCallback)
      t.roleItems[i]:ShowArrow(i ~= 1)
    end
  end
  t.RefreshRoleInfoPanel(t.roleInfo)
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

function t.RefreshRoleInfoPanel(roleInfo)
  -- refresh title
  t.imgRoleType.sprite = ui_util.GetRoleTypeSmallIconSprite(roleInfo.heroData.roleType)
  t.textRoleName.text = LocalizationController.instance:Get(roleInfo.heroData.name)
  t.textLevel.text = string.format("%d/%d", roleInfo.level, gamemanager.GetData('global_data').player_lv_max)
  t.sliderExp.value = 0
  t.textExp.text = '0%'
  --refresh attr
  ui_util.ClearChildren(t.tranRoleAttributeRoot, true)
  local attrDic = role_model.CalcRoleAttributesDic(roleInfo)
  for k, v in pairs(t.attrTypeList:GetDatas()) do
    local go = GameObject.Instantiate(t.goAttrPrefab)
    go:SetActive(true)
    local tran = go.transform
    tran:SetParent(t.tranRoleAttributeRoot,false)
    tran:Find('text_title'):GetComponent(typeof(Text)).text = attrDic[v]:GetName()
    tran:Find('text_value'):GetComponent(typeof(Text)).text = attrDic[v]:GetValueString()
  end
  --refresh skill
  t.skillIconTable[1]:SetSkillData(roleInfo.heroData.skillId1,roleInfo.advanceLevel,roleInfo.heroData.starMin,false)
  t.skillIconTable[2]:SetSkillData(roleInfo.heroData.skillId2,roleInfo.advanceLevel,roleInfo.heroData.starMin,false)
  t.skillIconTable[3]:SetSkillData(roleInfo.heroData.passiveId1,roleInfo.advanceLevel,roleInfo.heroData.starMin,true)
end

-------------------------click event----------------
function t.OnClickBackBtnHandler()
  uimanager.CloseView(name)
end

function t.ClickCheckCallback(roleInfo,id)
  t.role_info_panel:SetParent(t.tranPanelPos[id],false)
  t.role_info_panel.localPosition = Vector3(0,0,0)
  t.role_info_panel.gameObject:SetActive(true)
  t.RefreshRoleInfoPanel(roleInfo)
end

function t.ClickCloseInfoPanelHandler()
  LeanTween.delayedCall(0.1,Action(function()
      t.role_info_panel.gameObject:SetActive(false)
    end))
  
end

return t