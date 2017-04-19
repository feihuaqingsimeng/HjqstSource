local t = {}
local PREFAB_PATH = 'ui/equipments/up_star/up_star_panel'

local common_error_tips_view = require('ui/tips/view/common_error_tips_view')
local auto_destroy_tips_view = require('ui/tips/view/auto_destroy_tip_view')
local global_data = gamemanager.GetData('global_data')
local item_model = gamemanager.GetModel('item_model')

function t.Open (rightParent ,equipInfo,trainingView)
  local gameObject = GameObject.Instantiate(Common.ResMgr.ResMgr.instance:Load(PREFAB_PATH))
  t.transform = gameObject:GetComponent(typeof(Transform))
  t.transform:SetParent(rightParent, false)
  
  t.InitComponent()
  
  t.trainingView = trainingView
  t.maxStar = global_data.equipment_star_max
  t.materialIcon = nil
  t.material = nil
  t.curEquipIcon = nil
  t.nextEquipIcon = nil
  
  for k,v in pairs(global_data.gen_star) do
    local itemInfo = item_model.GetItemInfoByItemID(v.item.id)
    if itemInfo then
      t.material = v
      break
    end
  end
  
  t.SetEquipInfo(equipInfo)
 
  t.BindDelegate()
end

function t.Close()
  if t.transform ~= nil then
    LeanTween.cancel(t.transform.gameObject)
    GameObject.Destroy(t.transform.gameObject)
    t.transform = nil
    t.UnbindDelegate()
  end
end

function t.BindDelegate()
  gamemanager.GetModel('equip_model').UpdateTrainingDelegate:AddListener(t.UpdateTrainingByProtocol)
end

function t.UnbindDelegate()
  gamemanager.GetModel('equip_model').UpdateTrainingDelegate:RemoveListener(t.UpdateTrainingByProtocol)
end

function t.InitComponent()
  t.bottomRoot = t.transform:Find('bottom')
  t.attrRoot = t.transform:Find('attr')
  t.tranMaterialIcon = t.bottomRoot:Find('btn_material')
  t.textMaterialCount = t.bottomRoot:Find('text_material_count'):GetComponent(typeof(Text))
  t.textMaterialTip = t.bottomRoot:Find('text_tip'):GetComponent(typeof(Text))
  
  t.tranCurIcon = t.attrRoot:Find('icon/cur_icon')
  t.UpStarOrDowStarSuccessFXGameObject = t.attrRoot:Find('icon/cur_icon/ui_zhuangbeishengxing').gameObject
  t.UpStarOrDowStarSuccessFXGameObject:SetActive(false)
  t.tranNextIcon = t.attrRoot:Find('icon/new_icon')
  t.textAddPercent = t.attrRoot:Find('attr_root/text_add_percent'):GetComponent(typeof(Text))
  t.textAttrName = t.attrRoot:Find('attr_root/text_attr_name'):GetComponent(typeof(Text))
  t.textAttrValue = t.attrRoot:Find('attr_root/base_attr/text_attr_value'):GetComponent(typeof(Text))
  t.textAttrAdd = t.attrRoot:Find('attr_root/base_attr/text_attr_add'):GetComponent(typeof(Text))
  
  t.btnDownStar = t.attrRoot:Find('btn/btn_down_star'):GetComponent(typeof(Button))
  t.btnDownStar.onClick:AddListener(t.ClickDownStarBtnHandler)
  t.btnUpStar = t.attrRoot:Find('btn/btn_up_star'):GetComponent(typeof(Button))
  t.btnUpStar.onClick:AddListener(t.ClickUpStarBtnHandler)
  t.btnMaterial = t.bottomRoot:Find('btn_material'):GetComponent(typeof(Button))
  t.btnMaterial.onClick:AddListener(t.ClickMaterialBtnHandler)
  
  t.goMaxTip = t.transform:Find('max_tip').gameObject
end

function t.SetEquipInfo(equipInfo)

  t.selectMaterialIndex = 1
  t.needSpacialHandleAttr = false
  t.equipInfo = equipInfo
  t.Refresh()
end

function t.Refresh()
  local common_equip_icon = require('ui/common_icon/common_equip_icon')
  local common_item_icon = require('ui/common_icon/common_item_icon')
  if t.curEquipIcon == nil then
    t.curEquipIcon = common_equip_icon.New(t.tranCurIcon)
    t.curEquipIcon.transform:SetAsFirstSibling()
  end
  
  --attr add
  local addPercentStr = ''
  local starGemsCount = #t.equipInfo.starGems
  local total = 0
  
  t.textAttrName.text = t.equipInfo.baseAttr:GetName().."："
  t.textAttrValue.text = t.equipInfo.baseAttr:GetValueString()
   
  if starGemsCount == 0 then
    t.textAddPercent.text = '无'
    t.textAttrAdd.text = ''
  else
    for k,v in pairs(t.equipInfo.starGems) do
      total = total + v
      if k == starGemsCount then
        if k == 1 then
          addPercentStr = string.format('%s%d%%',addPercentStr,v)
        else
          addPercentStr = string.format('%s%d%% = %d%%',addPercentStr,v,total)
        end
      else
        addPercentStr = string.format('%s%d%%+',addPercentStr,v)
      end
      t.textAddPercent.text = addPercentStr
       
      t.textAttrAdd.text = string.format('+%d',math.floor( t.equipInfo:GetUpStarAddValue()))
    end
  end
  --update btn
  t.btnDownStar.gameObject:SetActive(t.equipInfo.star ~= 0)
  t.btnUpStar.gameObject:SetActive(t.equipInfo.star < 6)
  
  t.curEquipIcon:SetEquipInfo(t.equipInfo)

  if t.equipInfo.star == t.maxStar then
    t.bottomRoot.gameObject:SetActive(false)
    t.tranNextIcon.gameObject:SetActive(false)
    t.goMaxTip:SetActive(true)
    return
  end
  t.goMaxTip:SetActive(false)
  t.bottomRoot.gameObject:SetActive(true)
  t.tranNextIcon.gameObject:SetActive(true)
  if t.nextEquipIcon == nil then
    t.nextEquipIcon = common_equip_icon.New(t.tranNextIcon)
  end
  --next tip
  local nextEquipInfo = t.equipInfo:NewBySelf()
  nextEquipInfo.star = nextEquipInfo.star+1
  t.nextEquipIcon:SetEquipInfo(nextEquipInfo)
  --material
  if t.materialIcon == nil and t.material ~= nil then
    t.materialIcon = common_item_icon.New(t.tranMaterialIcon)
    t.materialIcon.onClick:AddListener(t.ClickMaterialBtnHandler)
    
  end
  if t.material ~= nil then
    t.materialIcon:SetGameResData(t.material.item)
    t.materialIcon:ShowCount(false)
    local own = gamemanager.GetModel('item_model').GetItemCountByItemID(t.material.item.id)
    if own >= t.material.item.count then
      t.textMaterialCount.text = ui_util.FormatToGreenText(string.format('%d/%d',own,t.material.item.count))
    else
      t.textMaterialCount.text = ui_util.FormatToRedText(string.format('%d/%d',own,t.material.item.count))
    end
    
    t.textMaterialTip.text = ui_util.FormatToGreenText(string.format(LocalizationController.instance:Get('ui.equipment_training_view.star.attrAdd'),t.material.min,t.material.max))
  else
    t.textMaterialCount.text = ui_util.FormatToRedText('0/1')
    t.textMaterialTip.text = ''
  end
  

end
-------------------Click event--------------------------
function t.ClickDownStarBtnHandler()
  
  if t.equipInfo.star ~= 0 then
    local view = require('ui/equip/view/equip_training/up_star/down_star_confirm_view')
    local count = gamemanager.GetData('global_data').gen_back_star.count
    local str = LocalizationController.instance:Get('ui.equipment_training_view.star.downStarTip')
    str = string.format(str,t.equipInfo.star-1,t.equipInfo.star,t.equipInfo.starGems[#t.equipInfo.starGems])
    view.Open(count,str,t.ConfirmDownStarHandler)
  end
end
function t.ConfirmDownStarHandler()
  local count = gamemanager.GetData('global_data').gen_back_star.count
  local own = gamemanager.GetModel('game_model').GetBaseResourceValue(BaseResType.Diamond)
  if own < count then
    common_error_tips_view.Open(LocalizationController.instance:Get('ui.equipment_training_view.star.notEnoughDiamond'))
    return
  end
  t.isClickDownStar = true
  if t.equipInfo.star == 1 then
    t.needSpacialHandleAttr = true
  end
  --t.trainingView.equip_attr_panel:ShowShuXingEffect()
  gamemanager.GetCtrl('equip_controller').EquipStarReq(t.equipInfo.id,0,0)
end


function t.ClickUpStarBtnHandler()
  if t.material == nil then
    common_error_tips_view.Open(LocalizationController.instance:Get('ui.equipment_training_view.star.notChoiceMat'))
    return
  end
  local own = gamemanager.GetModel('item_model').GetItemCountByItemID(t.material.item.id)
  if own < t.material.item.count then
    common_error_tips_view.Open(LocalizationController.instance:Get('ui.equipment_training_view.notEnoughMaterial'))
    return
  end
  t.isClickDownStar = false
  t.trainingView.equip_attr_panel:ShowShuXingEffect()
  gamemanager.GetCtrl('equip_controller').EquipStarReq(t.equipInfo.id,1,t.material.item.id)
end

function t.ClickMaterialBtnHandler()
  --t.material = gamemanager.GetData('global_data').gen_star[1]
  --t.Refresh()
  local maker_view = dofile('ui/equip/view/equip_training/up_star/up_star_maker_view')
  maker_view.Open(t.selectMaterialIndex,t.SelectMaterialCallback)
end
function t.SelectMaterialCallback(index)
  t.selectMaterialIndex = index
  t.material = gamemanager.GetData('global_data').gen_star[index]
  t.Refresh()
end
-----------------------------update by protocol ------------------
function t.UpdateTrainingByProtocol()
  if t.isClickDownStar then
    LeanTween.delayedCall(t.transform.gameObject, 0.6, Action(function ()
        auto_destroy_tips_view.Open(LocalizationController.instance:Get('ui.equipment_training_view.star.downSuccess'))
      end)
    )
  else
    LeanTween.delayedCall(t.transform.gameObject, 0.6, Action(function ()
        auto_destroy_tips_view.Open(LocalizationController.instance:Get('ui.equipment_training_view.star.success'))
      end)
    )
  end
  t.UpStarOrDowStarSuccessFXGameObject:SetActive(false)
  t.UpStarOrDowStarSuccessFXGameObject:SetActive(true)
  t.needSpacialHandleAttr = false
  t.isClickDownStar = false

end

return t