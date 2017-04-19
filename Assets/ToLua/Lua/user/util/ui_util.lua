local t = {}

t.iconPath = 'sprite/main_ui/'
t.ROLE_QUALITY_FRAME_SPRITE_PATH = 'sprite/main_ui/'
t.ITEM_QUALITY_FRAME_SPRITE_PATH = 'sprite/main_ui/'

t.equipQualityColor = {}
t.equipQualityColor[EquipmentQuality.White] = Color(1, 1, 1)
t.equipQualityColor[EquipmentQuality.Green] = Color(0, 1, 0)
t.equipQualityColor[EquipmentQuality.Blue] = Color(0.1255, 0.5569, 1)
t.equipQualityColor[EquipmentQuality.Purple] = Color(0.8627, 0.3255, 0.8353)
t.equipQualityColor[EquipmentQuality.Orange] = Color(1, 0.4824, 0.0235)
t.equipQualityColor[EquipmentQuality.Red] = Color(1, 0, 0)

--查找组件
function t.FindComp(transformRoot,path,componentType)
  local comp = transformRoot:Find(path)
  if comp == nil then
    Debugger.LogError('can not find component, path:'..path)
  else
    return comp:GetComponent(typeof(componentType))
  end
  return nil
end

function t.GetGrayMat()
  if not t.grayMat then
    t.grayMat = Common.ResMgr.ResMgr.instance:LoadMaterial("material/gray")
  end
  return t.grayMat
end

function t.SetImageGray(image,isGray)
  local color = image.color
  if isGray then
      color.r = 0
     -- imageOrText.material = t.GetGrayMat()
  else
      color.r = 1
     -- imageOrText.material = nil
  end
  image.color = color
end
function t.SetTextGray(text,isGray)
  local color = text.color
  if isGray then
      color.r = 0
      text.material = t.GetGrayMat()
  else
      color.r = 1
      text.material = nil
  end
  text.color = color
  local outLine = text.transform:GetComponent(typeof(UITextOutline))
  if outLine then
    outLine.enabled = not isGray
  end
end
function t.SetGrayChildren(transform,isGray,isContainsChild)
  local img = transform:GetComponent(typeof(Image))
  if img then
    t.SetImageGray(img,isGray)
  end
  local txt = transform:GetComponent(typeof(Text))
  if txt then
    t.SetTextGray(txt,isGray)
  end
  if isContainsChild then
    local count = transform.childCount
    local child = nil
    for i = 0,count-1 do
      child = transform:GetChild(i)
      t.SetGrayChildren(child,isGray,isContainsChild)
    end
  end
end

---移除所有的子节点，immediate是否立即移除
function t.ClearChildren(transform,immediate)
  if transform == nil then
    return
  end
  if immediate == nil then
    immediate = true
  end
  local childTable = {}
  local count = transform.childCount
  if count == 0 then 
    return 
  end
  
  for i = 1,count do
    childTable[i] = transform:GetChild(i-1).gameObject
  end
  --[[if immediate then
    for k,v in pairs(childTable) do
      GameObject.DestroyImmediate(v)
    end
  else ]]
    for k,v in pairs(childTable) do
      GameObject.Destroy(v)
    end
  --end
end

function t.SwitchLayer(transform,layer)
  if transform == nil then
     return 
  end
  transform.gameObject.layer = layer
  local count = transform.childCount
  if count == 0 then return end
  for i = 1 , count do
    t.SwitchLayer(transform.GetChild(i-1),layer)
  end
  
end

function t.GetRoleTypeName(roleType)
  roleType=tonumber(roleType)
  if roleType == RoleType.Defence then
    return LocalizationController.Get("hero_type_name_def");
  elseif roleType == RoleType.Offence then
   return LocalizationController.Get("hero_type_name_atk");
  elseif roleType == RoleType.Mage then
   return LocalizationController.Get("hero_type_name_magic");
  elseif roleType == RoleType.Mighty then
   return LocalizationController.Get("hero_type_name_all");
  elseif roleType == RoleType.Support then
   return LocalizationController.Get("hero_type_name_assist");
  else
   return '';
  end
end

--职业类型多语言描述
function t.GetHeroTypeDes(typeid)
    if typeid == 1 then
      return LocalizationController.instance:Get('hero_type_name_def')
    elseif typeid == 2 then
      return LocalizationController.instance:Get('hero_type_name_atk')
    elseif typeid == 3 then
      return LocalizationController.instance:Get('hero_type_name_magic')
    elseif typeid == 4 then
      return LocalizationController.instance:Get('hero_type_name_assist')
    elseif typeid == 5 then
      return LocalizationController.instance:Get('hero_type_name_all')
    else
      return 'unknown type:'..typeid
    end
end

--职业类型图标(小)
function t.GetRoleTypeSmallIconSprite(roleType)
  local path = ''
  
  if roleType == RoleType.Defence then
    path = t.iconPath..'icon_small_role_type_defence_2'
  elseif roleType == RoleType.Mage then
    path = t.iconPath..'icon_small_role_type_mage_2'
  elseif roleType == RoleType.Mighty then
    path = t.iconPath..'icon_small_role_type_mighty_2'
  elseif roleType == RoleType.Offence then
    path = t.iconPath..'icon_small_role_type_offence_2'
  elseif roleType == RoleType.Support then
    path = t.iconPath..'icon_small_role_type_support_2'
  end
  return Common.ResMgr.ResMgr.instance:LoadSprite(path)
end
--职业类型图标(大)
function t.GetRoleTypeBigIconSprite(roleType)
  return t.GetRoleTypeSmallIconSprite(roleType)
end

function t.GetRankRoleTypeSprite (roleType)
  local path = ''
  
  if roleType == RoleType.Defence then
    path = t.iconPath..'icon_rank_defence'
  elseif roleType == RoleType.Mage then
    path = t.iconPath..'icon_rank_mage'
  elseif roleType == RoleType.Mighty then
    path = t.iconPath..'icon_rank_mighty'
  elseif roleType == RoleType.Offence then
    path = t.iconPath..'icon_rank_offence'
  elseif roleType == RoleType.Support then
    path = t.iconPath..'icon_rank_support'
  end
  return Common.ResMgr.ResMgr.instance:LoadSprite(path)
end

function t.GetEnoughColorText(text,own,need)
  if own >= need then
    return t.FormatToGreenText(text)
  end
  return t.FormatToRedText(text)
end

function t.FormatToRedText(text)
  return string.format('<color=#ff0000>%s</color>',text)
end
function t.FormatToGreenText(text)
  return string.format('<color=#00ff00>%s</color>',text)
end

function t.GetEquipRandomColor(equipInfo)
  local colors = {}
  local equip_attr_data = gamemanager.GetData('equip_attr_data')
  
  for k,v in pairs(equipInfo.randomAttrs) do
    local attr_id = equipInfo.data.randomAttrIdList[k]
    colors[k] = t.GetEquipRandomColorByEquipAttr(attr_id,v)
     
  end
  return colors
end
function t.GetEquipRandomColorByEquipAttr(attrId,equipAttr)
  if equipAttr == nil then
    return nil
  end
  
  local equip_attr_data = gamemanager.GetData('equip_attr_data')
  
  local color = t.equipQualityColor[EquipmentQuality.White]
  local percent = 0
  local attrData = equip_attr_data.GetDataByAttrId(attrId,equipAttr.type)
    if attrData ~= nil then
      percent = attrData:ValuePercent(equipAttr.value)
      if percent < 0.2 then--白色
        color = t.equipQualityColor[EquipmentQuality.White]
      elseif percent < 0.4 then--绿色
        color = t.equipQualityColor[EquipmentQuality.Green]
      elseif percent < 0.6 then--蓝色
        color = t.equipQualityColor[EquipmentQuality.Blue]
      elseif percent <0.8 then--紫色
        color = t.equipQualityColor[EquipmentQuality.Purple]
      else
        color = t.equipQualityColor[EquipmentQuality.Orange]
      end
    end
    return color
end

function t.GetEquipQualityColor (equipData)
  return t.equipQualityColor[equipData.quality]
end
--解析玩家头像id ，获得icon路径
function t.ParseHeadIcon(headNo)
  local headHeroId = math.floor(headNo/10)
  local headStar = (headNo - headHeroId * 10)
  local heroData = gamemanager.GetData('hero_data').GetDataById(headHeroId)
  if(heroData ~= nil) then
      return 'sprite/head_icon/'..heroData.headIcons[headStar]
  end
	--print(string.format("头像找不到 headHeroId :%d,star:%d",headHeroId,headStar))
  return gamemanager.GetModel('game_model').playerInfo:HeadIcon()
end

--获得宝石槽类型图标
function t.GetGemSlotIconSprite( gemtype)
  local name = ''
  if gemtype == 1 then
    name = 'sprite/main_ui/icon_gem_02'
  elseif gemtype == 2 then
    name = 'sprite/main_ui/icon_gem_03'
  else
    name = 'sprite/main_ui/icon_gem_01'
  end
  return Common.ResMgr.ResMgr.instance:LoadSprite(name)
end

t.BASE_RES_ICON_PATH = "sprite/main_ui/"
function t.GetBaseResIconPath (baseResType)
  local baseResIconPath = ''

  if baseResType == BaseResType.PveAction then
    baseResIconPath = t.BASE_RES_ICON_PATH.."icon_key_small"
  elseif baseResType == BaseResType.PvpAction then
    baseResIconPath = t.BASE_RES_ICON_PATH.."icon_key_pvp_small"
  elseif baseResType == BaseResType.TowerAction then
    baseResIconPath = t.BASE_RES_ICON_PATH.."world_tree_fruit"
  elseif baseResType == BaseResType.Gold then
    baseResIconPath = t.BASE_RES_ICON_PATH.."icon_coin_small"
  elseif baseResType == BaseResType.Crystal then
    baseResIconPath = t.BASE_RES_ICON_PATH..""
  elseif baseResType == BaseResType.ArenaPoint then
    baseResIconPath = t.BASE_RES_ICON_PATH..'24'
  elseif baseResType == BaseResType.Diamond then
    baseResIconPath = t.BASE_RES_ICON_PATH.."icon_gem_small"
  elseif baseResType == BaseResType.Honor then
    baseResIconPath = t.BASE_RES_ICON_PATH.."icon_honor_small"
  elseif baseResType == BaseResType.RMB then
    baseResIconPath = t.BASE_RES_ICON_PATH.."icon_rmb_sml"
  elseif baseResType == BaseResType.Hero_Exp then                             --伙伴经验
    baseResIconPath = t.BASE_RES_ICON_PATH.."icon_exp_green_sml"
  elseif baseResType ==  BaseResType.Account_Exp then                         --账号经验
    baseResIconPath = t.BASE_RES_ICON_PATH.."icon_exp_sml"
  elseif baseResType == BaseResType.ExpeditionPoint then                      --远征币(类似荣誉)
    baseResIconPath = t.BASE_RES_ICON_PATH.."icon_expedition_small"
  elseif baseResType == BaseResType.WorldBossResource then                    --世界Boss资源
    baseResIconPath = t.BASE_RES_ICON_PATH.."icon_type_sml_06"
  elseif baseResType == BaseResType.FormationTrainPoint then                  --阵型培养点
    baseResIconPath = t.BASE_RES_ICON_PATH.."ui_team_3"
  
  elseif baseResType == BaseResType.EquipSoul then
    baseResIconPath = t.BASE_RES_ICON_PATH.."25"
  end
  
  return baseResIconPath;
end

--合并重复属性
function t.CheckSameAttribute(attrList,equipAttr)
  
  if equipAttr == nil then
    return
  end
local equip_attr = require('ui/equip/model/equip_attr')
  local isAdd = false
  for k,v in pairs(attrList) do
    if k == equipAttr.type then
      isAdd = true
      v.value = v.value + equipAttr.value
      break
    end
  end
  if isAdd == false then
    attrList[equipAttr.type] = equip_attr.NewByEquipAttr(equipAttr)
  end
end
--合并相同类型GameResData元素
function t.CombineGameResList(gameResDataList)
  local game_res_data = require('ui/game/model/game_res_data')
  local tempList = {}
  local index = 1
  for k,v in pairs(gameResDataList) do
    local alreadyAdd = false
    for k1,v1 in pairs(tempList) do
      if v1.type == BaseResType.Hero then -- 英雄合并
        if v1.type == v.type and v1.id == v.id and v1.star == v.star then
          alreadyAdd = true
          v1.count = v1.count + v.count
          break
        end
      elseif v1.type == BaseResType.Equipment then --装备合并
        if v1.type == v.type and v1.id == v.id then
          alreadyAdd = true
          v1.count = v1.count + v.count
          break
        end
      elseif v1.type == BaseResType.Item then --道具合并
        if v1.type == v.type and v1.id == v.id then
          alreadyAdd = true
          v1.count = v1.count + v.count
          break
        end
      else
        if v1.type == v.type then
          alreadyAdd = true
          v1.count = v1.count + v.count
          break
        end
      end
    end
    if not alreadyAdd then
      tempList[index] = game_res_data.New(v.type,v.id,v.count,v.star)
      index = index + 1
    end
  end
  return tempList
end
--获得公会con标志
function t.GetConsortiaMarkIconSprite(id)
  local count = #gamemanager.GetData('global_data').guild_mark
  if id <= 0 or id > count  then
    
    print(t.FormatToRedText('公会标志找不到 index:'..id))
    id = 1
  end
  return ResMgr.instance:LoadSprite('sprite/main_ui/'..gamemanager.GetData('global_data').guild_mark[id])
end

function t.GetRoleQualityFrameSprite (roleQuality)
  local roleQualityFrameSpritePath = ''
  if roleQuality == RoleQuality.White then
    roleQualityFrameSpritePath = t.ROLE_QUALITY_FRAME_SPRITE_PATH..'ui_items_02'
  elseif roleQuality == RoleQuality.Green then
    roleQualityFrameSpritePath = t.ROLE_QUALITY_FRAME_SPRITE_PATH..'ui_items_02_lv1'
  elseif roleQuality == RoleQuality.Blue then
    roleQualityFrameSpritePath = t.ROLE_QUALITY_FRAME_SPRITE_PATH..'ui_items_02_lv2'
  elseif roleQuality == RoleQuality.Purple then
    roleQualityFrameSpritePath = t.ROLE_QUALITY_FRAME_SPRITE_PATH..'ui_items_02_lv3'
  elseif roleQuality == RoleQuality.Orange then
    roleQualityFrameSpritePath = t.ROLE_QUALITY_FRAME_SPRITE_PATH..'ui_items_02_lv4'
  elseif roleQuality == RoleQuality.Red then
    roleQualityFrameSpritePath = t.ROLE_QUALITY_FRAME_SPRITE_PATH..'ui_items_02_lv4'
  else
    roleQualityFrameSpritePath = t.ROLE_QUALITY_FRAME_SPRITE_PATH..'ui_items_02'
  end
  return ResMgr.instance:LoadSprite(roleQualityFrameSpritePath)
end

function t.GetItemQualityFrameSprite (itemQuality)
  local itemQualityFrameSpritePath = ''
  if itemQuality == ItemQuality.White then
    itemQualityFrameSpritePath = t.ITEM_QUALITY_FRAME_SPRITE_PATH..'ui_items_02'
  elseif itemQuality == ItemQuality.Green then
    itemQualityFrameSpritePath = t.ITEM_QUALITY_FRAME_SPRITE_PATH..'ui_items_02_lv1'
  elseif itemQuality == ItemQuality.Blue then
    itemQualityFrameSpritePath = t.ITEM_QUALITY_FRAME_SPRITE_PATH..'ui_items_02_lv2'
  elseif itemQuality == ItemQuality.Purple then
    itemQualityFrameSpritePath = t.ITEM_QUALITY_FRAME_SPRITE_PATH..'ui_items_02_lv3'
  elseif itemQuality == ItemQuality.Orange then
    itemQualityFrameSpritePath = t.ITEM_QUALITY_FRAME_SPRITE_PATH..'ui_items_02_lv4'
  elseif itemQuality == ItemQuality.Red then
    itemQualityFrameSpritePath = t.ITEM_QUALITY_FRAME_SPRITE_PATH..'ui_items_02_lv4'
  else
    itemQualityFrameSpritePath = t.ITEM_QUALITY_FRAME_SPRITE_PATH..'ui_items_02'
  end
  return ResMgr.instance:LoadSprite(itemQualityFrameSpritePath)
end

function t.GetQualityColorString (quality)
  local colorString = ''
  if quality == RoleQuality.White then
    colorString = LocalizationController.instance:Get('common.quality.white')
  elseif quality == RoleQuality.Green then
    colorString = LocalizationController.instance:Get('common.quality.green')
  elseif quality == RoleQuality.Blue then
    colorString = LocalizationController.instance:Get('common.quality.blue')
  elseif quality == RoleQuality.Purple then
    colorString = LocalizationController.instance:Get('common.quality.purple')
  elseif quality == RoleQuality.Orange then
    colorString = LocalizationController.instance:Get('common.quality.orange')
  elseif quality == RoleQuality.Red then
    colorString = LocalizationController.instance:Get('common.quality.red')
  else
    colorString = LocalizationController.instance:Get('common.quality.white')
  end
  return colorString
end

function t.FormatStringWithinQualityColor (quality, str)
  local formatString = ''
  if quality == RoleQuality.White then
    formatString = LocalizationController.instance:Get('common.quality.formt_string.white')
  elseif quality == RoleQuality.Green then
    formatString = LocalizationController.instance:Get('common.quality.formt_string.green')
  elseif quality == RoleQuality.Blue then
    formatString = LocalizationController.instance:Get('common.quality.formt_string.blue')
  elseif quality == RoleQuality.Purple then
    formatString = LocalizationController.instance:Get('common.quality.formt_string.purple')
  elseif quality == RoleQuality.Orange then
    formatString = LocalizationController.instance:Get('common.quality.formt_string.orange')
  elseif quality == RoleQuality.Red then
    formatString = LocalizationController.instance:Get('common.quality.formt_string.orange')
  else
    formatString = LocalizationController.instance:Get('common.quality.formt_string.white')
  end
  return string.format(formatString, str)
end

return t