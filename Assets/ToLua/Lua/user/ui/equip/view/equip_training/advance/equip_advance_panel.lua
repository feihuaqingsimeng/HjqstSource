local t = {}
local PREFAB_PATH = 'ui/equipments/advance/advance_panel'

local common_error_tips_view = require('ui/tips/view/common_error_tips_view')
local auto_destroy_tips_view = require('ui/tips/view/auto_destroy_tip_view')

function t.Open (leftParent,rightParent, equipInfo,trainingView)
  local gameObject = GameObject.Instantiate(Common.ResMgr.ResMgr.instance:Load(PREFAB_PATH))
  t.transform = gameObject:GetComponent(typeof(Transform))
  t.transform:SetParent(rightParent, false)
  t.leftParent = leftParent
  t.trainingView = trainingView
  t.InitComponent()
  t.equipInfo = equipInfo
  t.isClickStrengthen = false
  t.Refresh()
  t.BindDelegate()
end

function t.Close()
  if t.transform ~= nil then
    GameObject.Destroy(t.transform.gameObject)
    t.transform = nil
    t.UnbindDelegate()
  end
end

function t.BindDelegate()
  gamemanager.GetModel('equip_model').UpdateTrainingDelegate:AddListener(t.UpdateTrainingByProtocol)
  gamemanager.GetModel('item_model').updateItemInfoListDelegate:AddListener(t.Refresh)

end

function t.UnbindDelegate()
  gamemanager.GetModel('equip_model').UpdateTrainingDelegate:RemoveListener(t.UpdateTrainingByProtocol)
  gamemanager.GetModel('item_model').updateItemInfoListDelegate:RemoveListener(t.Refresh)

end

function t.InitComponent()
  t.canvas = t.transform:GetComponentInParent(typeof(UnityEngine.Canvas))
  t.goRoot =  t.transform:Find('root').gameObject
  t.tranNewIcon = t.transform:Find('root/new_icon_root')
  t.btnStrengthen = t.transform:Find('root/btn_advance'):GetComponent(typeof(Button))
  t.btnStrengthen.onClick:AddListener(t.ClickAdanceHandler)
  t.tranMaterialRoot = t.transform:Find('root/materials_frame/material_root')
  t.textCondition = t.transform:Find('root/condition_frame/text_condition'):GetComponent(typeof(Text))
  t.goMaterialPrefab = t.transform:Find('root/material_prefab').gameObject
  t.goMaterialPrefab:SetActive(false)
  t.textGold = t.transform:Find('root/text_money'):GetComponent(typeof(Text))
  t.tranChangeAttrRoot = t.transform:Find('change_attr_root/root')
  t.tranBefore = t.tranChangeAttrRoot:Find('attr/before')
  t.tranAfter = t.tranChangeAttrRoot:Find('attr/after')
  t.textBeforeAttrName = t.tranBefore:Find('text_name'):GetComponent(typeof(Text))
  t.textBeforeAttrValue = t.tranBefore:Find('text_value'):GetComponent(typeof(Text))
  
  t.textAfterAttrName = t.tranAfter:Find('text_name'):GetComponent(typeof(Text))
  t.textAfterAttrValue = t.tranAfter:Find('text_value'):GetComponent(typeof(Text))
  t.goNoneTip = t.transform:Find('none_tip').gameObject
  t.textNoneTip = t.transform:Find('none_tip/text_tip'):GetComponent(typeof(Text))
  
  t.tranChangeAttrRoot:SetParent(t.leftParent,false)
  --effect
  local goShuaXingTiShi = t.tranAfter:Find('ui_shuaxintishi').gameObject
  particle_util.ChangeParticleSortingOrder(goShuaXingTiShi,t.canvas.sortingLayerName,t.canvas.sortingOrder)
  --装备强化特效
   t.goEffectZhuangbeiQiangHua = t.transform:Find('ui_zhuangbeiqianghua').gameObject
    particle_util.ChangeParticleSortingOrderByCanvas(t.goEffectZhuangbeiQiangHua,t.canvas)
  t.goEffectZhuangbeiQiangHua:SetActive(false)
  --初始尽然位置不对，延时显示
  goShuaXingTiShi:SetActive(false)
  coroutine.start(function()
      coroutine.wait(0.01)
      goShuaXingTiShi:SetActive(true)
    end)
end

function t.Refresh()
  ui_util.ClearChildren(t.tranNewIcon,true)
  ui_util.ClearChildren(t.tranMaterialRoot,true)
  --cur attr
  local totalBaseAttr = t.equipInfo:GetTotalBaseAttr()
  t.textBeforeAttrName.text = totalBaseAttr:GetName()
  t.textBeforeAttrValue.text = totalBaseAttr:GetValueString()
  
    --max
  local nextEquipData = gamemanager.GetData('equip_data').GetDataById(t.equipInfo.data.next_id)
  if nextEquipData == nil then
    --t.textCondition.text = ui_util.FormatToRedText( LocalizationController.instance:Get('ui.equipment_training_view.maxLevelUp'))
    t.goRoot:SetActive(false)
    t.goNoneTip:SetActive(true)
    t.tranAfter.gameObject:SetActive(false)
    t.textNoneTip.text = LocalizationController.instance:Get('ui.equipment_training_view.maxLevelUp')
    return
  end
  
  --need advance
  if t.equipInfo.data.isAdvance == false then
    --t.textCondition.text = ui_util.FormatToRedText( LocalizationController.instance:Get('ui.equipment_training_view.needLevelUp'))
    t.goRoot:SetActive(false)
    t.goNoneTip:SetActive(true)
    t.tranAfter.gameObject:SetActive(false)
    t.textNoneTip.text = LocalizationController.instance:Get('ui.equipment_training_view.needLevelUp')
    return
  end

  
   --show
  t.goRoot:SetActive(true)
  t.goNoneTip:SetActive(false)
  t.tranAfter.gameObject:SetActive(true)
  
  t.RefreshMaterial()
  

  
  --cur equip icon
  local game_res_data = require('ui/game/model/game_res_data')
  local common_equip_icon = require('ui/common_icon/common_equip_icon')
  local curIcon = common_equip_icon.New(t.tranNewIcon)
  curIcon:SetEquipInfo(t.equipInfo)
  
  --change attr
  local equip_attr = require('ui/equip/model/equip_attr')
  local nextBaseAttr = equip_attr.New(nextEquipData.baseAttrType,nextEquipData.baseAttrValue+t.equipInfo:GetTotalBaseAttr().value-t.equipInfo.baseAttr.value)

  t.textAfterAttrName.text = nextBaseAttr:GetName()
  t.textAfterAttrValue.text = nextBaseAttr:GetValueString()
  
  --tip 
  if t.equipInfo.ownerId ~= 0 then
    local des = LocalizationController.instance:Get("ui.equipment_training_view.advanceNeed")
    des = string.format(des,nextEquipData.useLv)
    local level = 0
    if gamemanager.GetModel('game_model').IsPlayer(t.equipInfo.ownerId) then
      level = gamemanager.GetModel('game_model').playerInfo.level
    else
      local heroInfo = gamemanager.GetModel('hero_model').GetHeroInfo(t.equipInfo.ownerId)
      level = heroInfo.level
    end
    
    if level >= nextEquipData.useLv then
      t.textCondition.text = ui_util.FormatToGreenText(des)
    else
      t.textCondition.text = ui_util.FormatToRedText(des)
    end
  else
    local des = LocalizationController.instance:Get("ui.equipment_training_view.advanceWearNeed")
    t.textCondition.text = ui_util.FormatToGreenText(string.format(des,nextEquipData.useLv))
  end
  
end

function t.RefreshMaterial()
  local materials = {}
  local gold = 0
  local index = 1
  --金币要取出来单独放
  for k,v in pairs(t.equipInfo.data.strengthenAdvanceMaterialGRD) do
    if v.type == BaseResType.Gold then
      gold = v.count
    else
      materials[index] = v
      index = index + 1
    end
  end
  if gold == 0 then
    t.textGold.gameObject:SetActive(false)
  else
    t.textGold.gameObject:SetActive(true)
    local own = gamemanager.GetModel('game_model').GetBaseResourceValue(BaseResType.Gold)
    local des = string.format('%d/%d', own,gold)
    if own > gold then
        t.textGold.text = ui_util.FormatToGreenText(des)
      else
        t.textGold.text = ui_util.FormatToRedText(des);
      end
  end
  --其他材料
  local materialsCount = #materials
  local prefab 
  local count = 4
  
  local common_reward_icon = require('ui/common_icon/common_reward_icon')
  
  for i = 1,count do
    prefab = GameObject.Instantiate(t.goMaterialPrefab):GetComponent(typeof(Transform))
    prefab:SetParent(t.tranMaterialRoot,false)
    prefab.gameObject:SetActive(true)
    if i> materialsCount then
      prefab:Find('text_count').gameObject:SetActive(false)
    else
      local gameResData = materials[i]
      local rewardIcon = common_reward_icon.New(prefab,gameResData)
      rewardIcon.onClick:AddListener(t.ClickMaterialHandler)
      rewardIcon:HideCount()
      local textCount = prefab:Find('text_count'):GetComponent(typeof(Text))
      textCount.gameObject:SetActive(true)
      local own = 0
      if gameResData.type == BaseResType.Item then
        own = gamemanager.GetModel('item_model').GetItemCountByItemID(gameResData.id)
      else
        own = gamemanager.GetModel('game_model').GetBaseResourceValue(gameResData.type)
      end
      local countString = string.format('%d/%d',own,gameResData.count)
      if own >= gameResData.count then
        textCount.text = ui_util.FormatToGreenText(countString)
      else
        textCount.text = ui_util.FormatToRedText(countString);
      end
    end
  end
  
end
-----------------click event-----------------
--点击材料
function t.ClickMaterialHandler(rewardIcon)
   LuaInterface.LuaCsTransfer.OpenGoodsJumpPath(BaseResType.Item, rewardIcon.itemIcon.itemInfo.itemData.id,0)
end
--点击进阶
function t.ClickAdanceHandler()
  local error_tip_view = require('ui/tips/view/common_error_tips_view')
  
    --max
  local nextEquipData = gamemanager.GetData('equip_data').GetDataById(t.equipInfo.data.next_id)
  if nextEquipData == nil then
    error_tip_view.Open(LocalizationController.instance:Get("ui.equipment_training_view.maxLevelUp"))
    return
  end
  
   --need strengthen
  if t.equipInfo.data.isAdance == false then
    error_tip_view.Open (LocalizationController.instance:Get('ui.equipment_training_view.needLevelUp'))
    return
  end
  

  --level enough
  local tip = nil
  if t.equipInfo.ownerId ~= 0 then
    local level = 0
    if gamemanager.GetModel('game_model').IsPlayer(t.equipInfo.ownerId) then
      level = gamemanager.GetModel('game_model').playerInfo.level
    else
      local heroInfo = gamemanager.GetModel('hero_model').GetHeroInfo(t.equipInfo.ownerId)
      level = heroInfo.level
    end
    if level < nextEquipData.useLv then
      tip = LocalizationController.instance:Get("ui.equipment_training_view.advanceNeed")
      error_tip_view.Open(string.format(tip,nextEquipData.useLv))
      return
    end
  end
  
  -- material enough
  local item_model = gamemanager.GetModel('item_model')
  local game_model = gamemanager.GetModel('game_model')
  local materials = t.equipInfo.data.strengthenAdvanceMaterialGRD
  for k,v in pairs(materials) do 
    local own = 0
    if v.type == BaseResType.Item then
      own = item_model.GetItemCountByItemID(v.id)
    else
      own = game_model.GetBaseResourceValue(v.type)
    end
    if v.count > own then
      error_tip_view.Open(LocalizationController.instance:Get("ui.equipment_training_view.notEnoughMaterial"))
      return
    end
   
  end
   if (t.isClickStrengthen) then
     return
    end
    t.isClickStrengthen = true
  coroutine.start(function()
      if t.transform == nil then
        return
      end
      t.goEffectZhuangbeiQiangHua:SetActive(true)
      
      coroutine.wait(1.05)
      t.goEffectZhuangbeiQiangHua:SetActive(false)
      t.trainingView.equip_attr_panel:ShowShuXingEffect()
      t.isClickStrengthen = false
      gamemanager.GetCtrl('equip_controller').EquipUpgradeReq(t.equipInfo.id)
    end)
end
-------------------------update by protocol--------------------
function t.UpdateTrainingByProtocol()
  
  auto_destroy_tips_view.Open(LocalizationController.instance:Get('ui.equipment_training_view.advance.success'))
  
  t.Refresh()
  --if t.equipInfo.data.isAdvance == false then
  --  t.trainingView.SetToggleOn(1)
  --end
  --efffect
  t.trainingView.equip_attr_panel:ShowEquipIconEffect()
end

return t