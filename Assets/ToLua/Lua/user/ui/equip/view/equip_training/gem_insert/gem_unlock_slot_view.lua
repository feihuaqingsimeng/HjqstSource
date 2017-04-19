local t = {}
local PREFAB_PATH = 'ui/equipments/gem_insert/gem_unlock_slot_view'


--slotIndex 从1开始
function t.Open(equipInfo,slotIndex)
  t.equipInfo = equipInfo
  t.slotIndex = slotIndex
  t.nextUnlockEquipData = nil
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay)
  t.transform = gameObject:GetComponent(typeof(Transform))
  t.InitComponent()
  t.Refresh()
  t.BindDelegate()
end
function t.Close()
  t.transform = nil
  t.equipInfo = nil
  t.nextUnlockEquipData = nil
  t.UnbindDelegate()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end

function t.BindDelegate()
  gamemanager.GetModel('equip_model').UpdateTrainingDelegate:AddListener(t.UpdateTrainingByProtocol)
end

function t.UnbindDelegate()
  gamemanager.GetModel('equip_model').UpdateTrainingDelegate:RemoveListener(t.UpdateTrainingByProtocol)
end

function t.InitComponent()
  local frame = t.transform:Find('core/frame')
  t.goCondition = frame:Find('condition').gameObject
  t.textUnlockTip = frame:Find('text_unlock_tip'):GetComponent(typeof(Text))
  t.tranMaterialRoot = frame:Find('condition/material_root')
  t.textTitle = frame:Find('title/Text'):GetComponent(typeof(Text))
  t.tranMaterialIcon = t.tranMaterialRoot:Find('img_icon')
  t.imgSlotTypeIcon = frame:Find('img_slot/img_slot_type'):GetComponent(typeof(Image))
  t.textMaterialCount = t.tranMaterialRoot:Find('text_count'):GetComponent(typeof(Text))
  local btnClose = frame:Find('btn_close'):GetComponent(typeof(Button))
  btnClose.onClick:AddListener(t.ClickCloseBtnHandler)
  local btnUnlock = frame:Find('condition/btn_unlock'):GetComponent(typeof(Button))
  btnUnlock.onClick:AddListener(t.ClickUnlockBtnHandler)
end

function t.Refresh()
   local material = gamemanager.GetData('global_data').gen_unlock_item[t.slotIndex]
  local gemType = t.equipInfo.data.gem[t.slotIndex]
  if t.equipInfo.gemInsertIds[t.slotIndex] == 0 then
    t.goCondition:SetActive(false)
    t.textUnlockTip.gameObject:SetActive(true)
    t.textUnlockTip.text = string.format(LocalizationController.instance:Get('ui.equipment_training_view.gem_insert.canInsertTip'), LocalizationController.instance:Get('ui.equipment_training_view.gem_insert.gem'..gemType))
    t.textTitle.text = LocalizationController.instance:Get('ui.equipment_training_view.gem_insert.alreadyUnlock')
  else
    t.goCondition:SetActive(true)
    t.textUnlockTip.gameObject:SetActive(false)
   
    if gemType > 0 then
      t.textTitle.text = LocalizationController.instance:Get('ui.equipment_training_view.gem_insert.gem'..gemType)
      t.imgSlotTypeIcon.sprite = ui_util.GetGemSlotIconSprite(gemType)
    end
    
    local common_reward_icon = require('ui/common_icon/common_reward_icon')
    local icon = common_reward_icon.New(t.tranMaterialIcon,material)
    icon:HideCount()
    icon.onClick:AddListener(t.ClickMaterialHandler)
    local own = 0
    if material.type == BaseResType.Item then
      own = gamemanager.GetModel('item_model').GetItemCountByItemID(material.id)
    else
      own = gamemanager.GetModel('game_model').GetBaseResourceValue(material.type)
    end
    t.textMaterialCount.text = ui_util.GetEnoughColorText(string.format('%d/%d',own,material.count),own,material.count)
  end
  
  
end

---------------------click event---------------------
function t.ClickCloseBtnHandler()
  t.Close()
end
function t.ClickUnlockBtnHandler()
  local common_error_tips_view = require('ui/tips/view/common_error_tips_view')

  --等级不足
  if t.equipInfo.data.gem[t.slotIndex] == -1 then
    common_error_tips_view.Open(LocalizationController.instance:Get('ui.equipment_training_view.gem_insert.can_not_unlock'))
    return
  end
  --材料不足
  local material = gamemanager.GetData('global_data').gen_unlock_item[t.slotIndex]
  if material ~= nil then
    local own = 0
    if material.type == BaseResType.Item then
      own = gamemanager.GetModel('item_model').GetItemCountByItemID(material.id)
    else
      own = gamemanager.GetModel('game_model').GetBaseResourceValue(material.type)
    end
    if own < material.count then
      common_error_tips_view.Open(LocalizationController.instance:Get('ui.equipment_training_view.notEnoughMaterial'))
      return
    end
  end
  gamemanager.GetCtrl('equip_controller').InlayGemSlotUnlockReq(t.equipInfo.id,t.slotIndex-1)
 
end
-------------click event--------------------------
function t.ClickMaterialHandler(rewardIcon)
  local gameResData = rewardIcon.gameResData
   LuaInterface.LuaCsTransfer.OpenGoodsJumpPath(gameResData.type, gameResData.id,0)
end


---------------------------update by protocol---------------------
function t.UpdateTrainingByProtocol()
  local common_error_tips_view = require('ui/tips/view/common_error_tips_view')
  common_error_tips_view.Open(LocalizationController.instance:Get('ui.equipment_training_view.gem_insert.unlockSuccess'))
  t.Close()
end

return t