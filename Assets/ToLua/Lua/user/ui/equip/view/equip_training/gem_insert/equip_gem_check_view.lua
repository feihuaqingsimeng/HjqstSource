local t = {}
local PREFAB_PATH = 'ui/equipments/gem_insert/equip_gem_check_view'

local equip_model = gamemanager.GetModel('equip_model')
local equip_controller = gamemanager.GetCtrl('equip_controller')
local item_model = gamemanager.GetModel('item_model')
local item_data = gamemanager.GetData('item_data')
local gem_attr_data = gamemanager.GetData('gem_attr_data')
local auto_destroy_tip_view = require('ui/tips/view/auto_destroy_tip_view')
--slotIndex 从1开始
function t.Open(equipInfo,selectData)
  
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.Tips,UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  t.equipInfo = equipInfo
  t.selectData = selectData
  
  t.InitComponent()
  t.BindDelegate()
  t.Refresh()
end

function t.Close()
 
  t.UnbindDelegate()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end

function t.BindDelegate()
  equip_model.UpdateTrainingDelegate:AddListener(t.UpdateTrainingByProtocol)
end

function t.UnbindDelegate()
  equip_model.UpdateTrainingDelegate:RemoveListener(t.UpdateTrainingByProtocol)
end

function t.InitComponent()
  local frame = t.transform:Find('core/frame')
  frame:Find('title/btn_close'):GetComponent(typeof(Button)).onClick:AddListener(t.ClickCloseBtnHandler)
  t.imgIcon = frame:Find('img_icon'):GetComponent(typeof(Image))
  t.textTip = frame:Find('text_tip'):GetComponent(typeof(Text))
  t.textTitle = frame:Find('title/text_title'):GetComponent(typeof(Text))
  t.textCount = frame:Find('img_icon/text_count'):GetComponent(typeof(Text))
  --
  local attr_root = frame:Find('attr_root')
  t.tranAttrTable = {}
  t.tranAttrTable.textName = attr_root:Find('text_name'):GetComponent(typeof(Text))
  t.tranAttrTable.textAttrName = attr_root:Find('text_attr_name'):GetComponent(typeof(Text))
  t.tranAttrTable.textAttrValue = attr_root:Find('text_attr_value'):GetComponent(typeof(Text))
  --
  t.btnInsert = frame:Find('btn/btn_insert'):GetComponent(typeof(Button))
  t.btnInsert.onClick:AddListener(t.ClickInsertBtnHandler)
  t.btnCompose = frame:Find('btn/btn_compose'):GetComponent(typeof(Button))
  t.btnCompose.onClick:AddListener(t.ClickComposeBtnHandler)
  t.btnUnload = frame:Find('btn_unload'):GetComponent(typeof(Button))
  t.btnUnload.onClick:AddListener(t.ClickUnloadBtnHandler)
end

function t.Refresh()
  
  t.btnInsert.gameObject:SetActive(false)
  t.btnCompose.gameObject:SetActive(false)
  t.btnUnload.gameObject:SetActive(false)
  
  local itemInfo = nil
  
  if t.selectData.selectState == GemSelectState.SelectEquipSlot then
    itemInfo = item_model.GetItemInfoByItemID( t.equipInfo.gemInsertIds[t.selectData.selectSlotIndex])
    t.btnUnload.gameObject:SetActive(true)
    t.textCount.text = ''
    t.textTitle.text = LocalizationController.instance:Get('ui.equipment_training_view.gem_insert.slotAlreadyInsert')
  elseif t.selectData.selectState == GemSelectState.SelectUnUseGem then
    itemInfo = item_model.GetItemInfoByInstanceID(t.selectData.selectGemId)
    t.textTitle.text = LocalizationController.instance:Get('ui.equipment_training_view.gem_insert.slotCanInsert')
    t.textCount.text = itemInfo:Count()
  end
  --选中的宝石道具（背包槽中或未镶嵌宝石）
 
  local itemData = item_data.GetDataById(itemInfo.itemData.id)
  local attrData = gem_attr_data.GetDataById(itemInfo.itemData.id)
  t.imgIcon.sprite = ResMgr.instance:LoadSprite(itemData:IconPath())
  t.tranAttrTable.textName.text = LocalizationController.instance:Get(itemData.name)
  t.tranAttrTable.textAttrName.text = attrData.equipAttr:GetName()
  t.tranAttrTable.textAttrValue.text = string.format('+%s',attrData.equipAttr:GetValueString())
  --是否可以合成
  local canCombine,ownCount,materialCount = item_model.CheckGemCombine(itemData.id)
  if materialCount == 0 then
    t.textTip.text = LocalizationController.instance:Get('ui.equipment_training_view.gem_insert.maxLevelCannotCombine')
  else
    t.textTip.text = string.format(LocalizationController.instance:Get('ui.equipment_training_view.gem_insert.sameGemCombine'),materialCount)
  end
  --是否可镶嵌
  local canInsert = false
  local findType = false
  for k,v in pairs(t.equipInfo.data.gem) do
    if v ~= -1 and v == attrData.gen_type then -- 槽开放
      findType = true
      canInsert = true
      break
    end
  end
  
  if t.selectData.selectState == GemSelectState.SelectUnUseGem then
    t.btnInsert.gameObject:SetActive(canInsert)
    t.btnCompose.gameObject:SetActive(canCombine)
    if not findType then
      t.textTitle.text = LocalizationController.instance:Get('ui.equipment_training_view.gem_insert.slotTypeWrong')
    end
  end
end

-------------------------------------click event----------------------------
function t.ClickCloseBtnHandler()
  t.Close()
end
--镶嵌
function t.ClickInsertBtnHandler()
  
  local itemInfo = item_model.GetItemInfoByInstanceID(t.selectData.selectGemId)
  local attrData = gem_attr_data.GetDataById(itemInfo.itemData.id)
  local slot = 0
  local minValueSlot = 0 
  local minValue = 0
  for k,v in pairs(t.equipInfo.data.gem) do
    if v == attrData.gen_type then

      if t.equipInfo.gemInsertIds[k] == 0 then--空槽
        slot = k
        break
      end
      if t.equipInfo.gemInsertIds[k] > 0 then
        local value = gem_attr_data.GetDataById(t.equipInfo.gemInsertIds[k]).equipAttr.value
        if minValue == 0 or value < minValue then --找到属性最小的槽 替换之
          minValueSlot = k
          minValue = value
        end
      end
    end
  end
  print(slot,minValueSlot)
  if slot == 0 and minValueSlot == 0 then--槽都锁住
    auto_destroy_tip_view.Open(LocalizationController.instance:Get('ui.equipment_training_view.gem_insert.slotLock'))
    return
  end
  if slot == 0 then --没有空槽
    slot = minValueSlot
  end
  t.slotParticleIndex = slot
  equip_controller.EquipInlayGemReq(t.equipInfo.id,slot-1,itemInfo.itemData.id)
  t.ClickCloseBtnHandler()
end
--合成
function t.ClickComposeBtnHandler()
  if t.selectData.selectState == GemSelectState.SelectUnUseGem then
    local itemInfo = gamemanager.GetModel('item_model').GetItemInfoByInstanceID(t.selectData.selectGemId)
    equip_controller.InlayGemComposeReq(itemInfo.itemData.id,0,0)
  end
end
--卸载
function t.ClickUnloadBtnHandler()
  equip_controller.EquipInlayGemReq(t.equipInfo.id,t.selectData.selectSlotIndex-1,0)
  t.ClickCloseBtnHandler()
end
-------------------------------------update by protocol----------------------------
function t.UpdateTrainingByProtocol()
  if t.selectData.selectState == GemSelectState.SelectUnUseGem then
    auto_destroy_tip_view.Open(LocalizationController.instance:Get('ui.equipment_training_view.gem_insert.CombineSucTip'))
    local itemInfo = item_model.GetItemInfoByInstanceID(t.selectData.selectGemId)
    if itemInfo then
      local canCombine,ownCount,materialCount = item_model.CheckGemCombine(itemInfo.itemData.id)
      if canCombine then
        t.Refresh()
        return
      end
    end
  end
  t.ClickCloseBtnHandler()
end
return t