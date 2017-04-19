local t = {}
local PREFAB_PATH = 'ui/player_information_check/role_check_info_view'
local name = PREFAB_PATH

local role_model = gamemanager.GetModel('role_model')
local common_skill_icon = require('ui/common_icon/common_skill_icon')
local common_equip_icon = require('ui/common_icon/common_equip_icon')

--roleCheckInfo:玩家英雄信息[参考：role_check_info.lua] ,localPosition:面板位置
function t.Open(roleCheckInfo,localPosition)
  uimanager.RegisterView(name,t)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  t.roleCheckInfo = roleCheckInfo
  t.panelLocalPosition = localPosition
  t.InitComponent()
  t.Refresh()
end


function t.Close()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end

function t.InitComponent()
  local btnClose = t.transform:Find('core/btn_close'):GetComponent(typeof(Button))
  btnClose.onClick:AddListener(t.ClickCloseHandler)
  local info_panel = t.transform:Find('core/role_info_panel')
  t.tranInfoPanel = info_panel
  local attr_root = info_panel:Find('hero_attr_root')
  t.imgRoleType = attr_root:Find('img_hero_type_icon'):GetComponent(typeof(Image))
  t.textHeroName = attr_root:Find('txt_hero_name'):GetComponent(typeof(Text))
  t.textHeroLevel = attr_root:Find('text_hero_level'):GetComponent(typeof(Text))
  t.combatCapabilityText = info_panel:Find('combat_capability_root/text_combat_capability'):GetComponent(typeof(Text))

  --attr
  t.tranAttrTable = {}
  local scrollContent = info_panel:Find('Scroll View/Viewport/Content')
  local child = nil
  for i = 1,8 do
    local tb = {}
    child = scrollContent:GetChild(i-1)
    tb.textTitle = child:Find('role_attribute_title'):GetComponent(typeof(Text))
    tb.textValue = child:Find('role_attribute_value'):GetComponent(typeof(Text))
    tb.textAdd = child:Find('role_attribute_add_value'):GetComponent(typeof(Text))
    t.tranAttrTable[i] = tb
  end
  --skill
  local skill_root = info_panel:Find('skills_root')
  t.skillIconTable  = {}
  t.skillIconTable[1] = common_skill_icon.BindTransform(skill_root:Find('img_skill_01'))
  t.skillIconTable[2] = common_skill_icon.BindTransform(skill_root:Find('img_skill_02'))
  t.skillIconTable[3] = common_skill_icon.BindTransform(skill_root:Find('passive/img_skill_03'))
  
  --equipment
  local equip_root = info_panel:Find('equipments_root')
  t.tranEquipRoot = {}
  t.tranEquipRoot[1] = equip_root:Find('weapon')
  t.tranEquipRoot[2] = equip_root:Find('armor')
  t.tranEquipRoot[3] = equip_root:Find('accessory')
end
function t.Refresh()
  
  if t.panelLocalPosition then
    t.tranInfoPanel.localPosition = t.panelLocalPosition
  end
  t.combatCapabilityText.text = math.floor(t.roleCheckInfo.roleInfo:PowerIncludeEquipments())
  
  t.RefreshAttribute()
  t.RefreshSkill()
  t.RefreshEquipment()
end

function t.RefreshAttribute()
  local roleInfo = t.roleCheckInfo.roleInfo
  t.imgRoleType.sprite = ui_util.GetRoleTypeBigIconSprite(roleInfo.heroData.roleType)
  t.textHeroName.text = roleInfo.heroData:GetNameWithQualityColor()
  t.textHeroLevel.text = string.format(LocalizationController.instance:Get('ui.role_info_view.cur_level'),roleInfo.level,roleInfo:MaxLevel())
  --attr
  local attrList = role_model.CalcRoleAttributesDic(roleInfo)
  
  local equipAddAttrList = role_model.CalcRoleAttributesDicByEquipList(roleInfo,t.roleCheckInfo.equipInfoDictionary:GetDatas())
  local showAttrList = {}
  showAttrList[1] = attrList[RoleAttributeType.HP]
  if roleInfo:GetRoleAttackAttributeType() == RoleAttackAttributeType.PhysicalAttack then
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
  local roleInfo = t.roleCheckInfo.roleInfo
  t.skillIconTable[1]:SetSkillData(roleInfo.heroData.skillId1,roleInfo.advanceLevel,roleInfo.heroData.starMin,false)
  t.skillIconTable[2]:SetSkillData(roleInfo.heroData.skillId2,roleInfo.advanceLevel,roleInfo.heroData.starMin,false)
  t.skillIconTable[3]:SetSkillData(roleInfo.heroData.passiveId1,roleInfo.advanceLevel,roleInfo.heroData.starMin,true)
end

function t.RefreshEquipment()
  --t.tranEquipRoot equipmentItems
  local roleInfo = t.roleCheckInfo.roleInfo
  local equipIds = {}
  equipIds[1] = t.roleCheckInfo:GetWeaponEquip()
  equipIds[2] = t.roleCheckInfo:GetArmorEquip()
  equipIds[3] = t.roleCheckInfo:GetAccessoryEquip()
  local item = nil
  for k,v in ipairs(t.tranEquipRoot) do
    print(equipIds[k])
    if equipIds[k] then
      item = common_equip_icon.New(v)
      item:SetEquipInfo(equipIds[k])
      item:AddEquipDesButton()
    end
  end
    
end


------------------------------click event------------------
function t.ClickCloseHandler()
  t.Close()
end
return t