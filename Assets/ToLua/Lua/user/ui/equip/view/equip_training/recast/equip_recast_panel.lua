local t = {}
local PREFAB_PATH = 'ui/equipments/recast/recast_panel'

local common_error_tips_view = require('ui/tips/view/common_error_tips_view')
local auto_destroy_tips_view = require('ui/tips/view/auto_destroy_tip_view')
local equip_model = gamemanager.GetModel('equip_model')
local item_model = gamemanager.GetModel('item_model')

function t.Open (rightParent, equipInfo,trainingView)
  local gameObject = GameObject.Instantiate(Common.ResMgr.ResMgr.instance:Load(PREFAB_PATH))
  t.transform = gameObject:GetComponent(typeof(Transform))
  t.transform:SetParent(rightParent, false)
  t.InitComponent()
  t.trainingView = trainingView
  t.SetEquipInfo(equipInfo)
  
  t.BindDelegate()
end

function t.Close()
  if t.transform ~= nil then
    GameObject.Destroy(t.transform.gameObject)
    t.transform = nil
    t.trainingView = nil
    t.UnbindDelegate()
    
  end
end

function t.BindDelegate()
  equip_model.UpdateTrainingDelegate:AddListener(t.UpdateTrainingByProtocol)
  item_model.updateItemInfoListDelegate:AddListener(t.Refresh)
end

function t.UnbindDelegate()
  equip_model.UpdateTrainingDelegate:RemoveListener(t.UpdateTrainingByProtocol)
  item_model.updateItemInfoListDelegate:RemoveListener(t.Refresh)
end

function t.InitComponent()
  
  t.leftRoot = t.transform:Find('change_attr_root/root')
  t.rightRoot = t.transform:Find('attr')
  t.tranMaterial = t.leftRoot:Find('grid/left/material_icon')
  t.textCount = t.leftRoot:Find('grid/left/text_count'):GetComponent(typeof(Text))
  t.btnRecast = t.rightRoot:Find('btn_recast'):GetComponent(typeof(Button))
  t.btnRetain = t.rightRoot:Find('btn_retain'):GetComponent(typeof(Button))
  t.btnUse = t.rightRoot:Find('btn_use'):GetComponent(typeof(Button))
  local attrRoot = {}
  attrRoot[1]= t.rightRoot:Find('img_current_attributes')
  attrRoot[2] = t.rightRoot:Find('img_after_recast')
  t.curAttrTable = {}
  t.afterAttrTable = {}
  t.curAttrTable.textPower = attrRoot[1]:Find('text_power'):GetComponent(typeof(Text))
  t.curAttrTable.tranAttrRoot = attrRoot[1]:Find('attr_root')
  t.afterAttrTable.textPower = attrRoot[2]:Find('text_power'):GetComponent(typeof(Text))
  t.afterAttrTable.tranAttrRoot = attrRoot[2]:Find('attr_root')
  
  t.goAttrPrefab = t.rightRoot:Find('attr_template').gameObject
  t.goAttrPrefab:SetActive(false)
  
  t.goNoneTip = t.transform:Find('none_tip').gameObject
  t.textNoneTip = t.transform:Find('none_tip/text_tip'):GetComponent(typeof(Text))
  
  t.btnRecast.onClick:AddListener(t.ClickRecastBtnHandler)
  t.btnRetain.onClick:AddListener(t.ClickRetainBtnHandler)
  t.btnUse.onClick:AddListener(t.ClickUseBtnHandler)
end

function t.SetEquipInfo(equipInfo)
  t.equipInfo = equipInfo
  t.Refresh()
end

function t.Refresh()
  
    --no material
  local material = t.equipInfo.data.recastMaterialGRD
  
  t.leftRoot.gameObject:SetActive(material ~= nil)
  if material == nil then
    t.rightRoot.gameObject:SetActive(false)
    t.goNoneTip:SetActive(true)
    t.textNoneTip.text =  LocalizationController.instance:Get('ui.equipment_training_view.recast.not_recast')
    print(ui_util.FormatToGreenText('material is nil,can not recast'))
    return 
  end
  -- hide btn
  t.btnRecast.gameObject:SetActive(not equip_model.isRecast)
  t.btnRetain.gameObject:SetActive(equip_model.isRecast)
  t.btnUse.gameObject:SetActive(equip_model.isRecast)
  
  t.RefreshMaterial()
  t.RefreshAttr()
end

function t.RefreshMaterial()
  ui_util.ClearChildren(t.tranMaterial,true)
  local material = t.equipInfo.data.recastMaterialGRD
  local common_reward_icon = require('ui/common_icon/common_reward_icon')
  local materialIcon = common_reward_icon.New(t.tranMaterial,material)
  materialIcon.onClick:AddListener(t.ClickMaterialHandler)
  materialIcon:HideCount()
  local own = 0
  if material.type == BaseResType.Item then
    own = gamemanager.GetModel('item_model').GetItemCountByItemID(material.id)
  else
    own = gamemanager.GetModel('game_model').GetBaseResourceValue(material.type)
  end
  local str = string.format('%d/%d',own,material.count)
  if own >= material.count then
    t.textCount.text = ui_util.FormatToGreenText(str)
  else
    t.textCount.text = ui_util.FormatToRedText(str)
  end
end

function t.RefreshAttr()
  --cur attr
  local colors = ui_util.GetEquipRandomColor(t.equipInfo)
  ui_util.ClearChildren(t.curAttrTable.tranAttrRoot,true)
  ui_util.ClearChildren(t.afterAttrTable.tranAttrRoot,true)
  for k,v in pairs(t.equipInfo.randomAttrs) do
    local go = GameObject.Instantiate(t.goAttrPrefab)
    go:SetActive(true)
    local tran = go.transform
    tran:SetParent(t.curAttrTable.tranAttrRoot,false)
    local textName = tran:Find('text_name'):GetComponent(typeof(Text))
    local textValue = tran:Find('text_value'):GetComponent(typeof(Text))
    textName.text = v:GetName()
    textValue.text = v:GetValueString()
    textName.color = colors[k]
    textValue.color = colors[k]
  end
  t.curAttrTable.textPower.text = t.equipInfo:Power()
  --recast attr

  if equip_model.isRecast then
    local tempEquipInfo = t.equipInfo:NewBySelf()
    for k,v in pairs(equip_model.equipRecastAttrList) do
      local go = GameObject.Instantiate(t.goAttrPrefab)
      go:SetActive(true)
      local tran = go.transform
      tran:SetParent(t.afterAttrTable.tranAttrRoot,false)
      local textName = tran:Find('text_name'):GetComponent(typeof(Text))
      local textValue = tran:Find('text_value'):GetComponent(typeof(Text))
      local color = ui_util.GetEquipRandomColorByEquipAttr(t.equipInfo.data.randomAttrIdList[k],equip_model.equipRecastAttrList[k])
      textName.text = v:GetName()
      textValue.text = v:GetValueString()
      textName.color = color
      textValue.color = color
      --计算战力用
      tempEquipInfo.randomAttrs = v
    end
    
    
    
    t.afterAttrTable.textPower.text = tempEquipInfo:Power()
  else
    local count = #t.equipInfo.randomAttrs
    for i = 1,count do
      local go = GameObject.Instantiate(t.goAttrPrefab)
      go:SetActive(true)
      local tran = go.transform
      tran:SetParent(t.afterAttrTable.tranAttrRoot,false)
      local textName = tran:Find('text_name'):GetComponent(typeof(Text))
      local textValue = tran:Find('text_value'):GetComponent(typeof(Text))
      textName.text = '???'
      textValue.text = '????'
    end
    t.afterAttrTable.textPower.text = "??"
  end
  
end

-------------------click btn------------------------
function t.ClickMaterialHandler(rewardIcon)
   LuaInterface.LuaCsTransfer.OpenGoodsJumpPath(BaseResType.Item, rewardIcon.itemIcon.itemInfo.itemData.id,0)
end
--点击重铸
function t.ClickRecastBtnHandler()
  
  local error_tip_view = require('ui/tips/view/common_error_tips_view')
  
  local material = t.equipInfo.data.recastMaterialGRD
  if material == nil then 
    error_tip_view.Open(LocalizationController.instance:Get('ui.equipment_training_view.not_recast'))
    return
  end
  
  local own = 0
  if material.type == BaseResType.Item then
    own = gamemanager.GetModel('item_model').GetItemCountByItemID(material.id)
  else
    own = gamemanager.GetModel('game_model').GetBaseResourceValue(material.type)
  end
  if material.count > own then
    error_tip_view.Open(LocalizationController.instance:Get("ui.equipment_training_view.notEnoughMaterial"))
    return
  end
  gamemanager.GetCtrl('equip_controller').EquipRecastReq(t.equipInfo.id)
  
end
--点击保留
function t.ClickRetainBtnHandler()
  gamemanager.GetCtrl('equip_controller').EquipRecastAffirmReq(t.equipInfo.id,false)
end
--点击使用
function t.ClickUseBtnHandler()
  gamemanager.GetCtrl('equip_controller').EquipRecastAffirmReq(t.equipInfo.id,true)
  t.trainingView.equip_attr_panel:ShowShuXingEffect()
end

--------------------update by protocol------------------
function t.UpdateTrainingByProtocol()
  print(ui_util.FormatToGreenText('装备重铸啦'))
  t.Refresh()
end

return t