local t = {}
local PREFAB_PATH = 'ui/equipments/decompose/equip_decompose_view'

local common_equip_icon = require 'ui/common_icon/common_equip_icon'
local common_item_icon = require 'ui/common_icon/common_item_icon'
local global_data = gamemanager.GetData('global_data')
local equip_model = gamemanager.GetModel('equip_model')
local equip_controller = gamemanager.GetCtrl('equip_controller')
local item_data = gamemanager.GetData('item_data')
local game_model = gamemanager.GetModel('game_model')
local equip_decompose_data = gamemanager.GetData('equip_decompose_data')
local confirm_tip_view = require('ui/tips/view/confirm_tip_view')

--equipInfo装备,itemInfo装备碎片 赋值一个就可
function t.Open(equipInfo,itemInfo)
  uimanager.RegisterView(PREFAB_PATH,t)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  t.BindDelegate()
  t.InitComponent()
  
  t.gainSoul = 0
  t.gainMoney = 0
  t.decomposeData = nil
  
  if equipInfo then
    t.SetEquipInfo(equipInfo)
  else
    t.SetEquipPiece(itemInfo)
  end
  
end

function t.BindDelegate()
  equip_controller.equipDecomposeSucDelegate:AddListener(t.EquipDecomposeSucByProtocol)
end

function t.UnbindDelegate()
  equip_controller.equipDecomposeSucDelegate:RemoveListener(t.EquipDecomposeSucByProtocol)
end

function t.InitComponent()
  t.heroIconRoot = t.transform:Find('core/img_frame/img_inner_frame/icon_root')
  t.heroNameText = t.transform:Find('core/img_frame/img_inner_frame/text_name'):GetComponent(typeof(Text))
  t.toggleTip = t.transform:Find('core/img_frame/toggle_tip'):GetComponent(typeof(Toggle))
  t.toggleTip.onValueChanged:AddListener(t.ClickToggleHandler)
  t.cancelButton = t.transform:Find("core/img_frame/btn_cancel"):GetComponent(typeof(Button))
  t.cancelButton.onClick:AddListener(t.ClickCloseButtonHandler)
  t.confirmDecomposeButton = t.transform:Find("core/img_frame/btn_confirm_decompose"):GetComponent(typeof(Button))
  t.confirmDecomposeButton.onClick:AddListener(t.ClickConfirmDecomposeButtonHandler)

  t.textGainAward = ui_util.FindComp(t.transform,'core/img_frame/img_inner_frame/text_gain_award',Text)
end

function t.SetEquipInfo (equipInfo)
  t.equipInfo = equipInfo
  t.equipIcon = common_equip_icon.New(t.heroIconRoot)
  t.equipIcon:SetEquipInfo(t.equipInfo, false)
  t.heroNameText.text = LocalizationController.instance:Get(equipInfo.data.name)
  
  t.RefreshGainAward()
end

function t.SetEquipPiece(itemInfo)
  t.itemInfo = itemInfo
  t.itemIcon = common_item_icon.New(t.heroIconRoot)
  t.itemIcon:SetItemInfo(itemInfo)
  t.heroNameText.text = LocalizationController.instance:Get(itemInfo.itemData.name)
  t.RefreshGainAward()
end

function t.RefreshGainAward()
  --器魂
  local id = 0
  local baseResType = nil
  local count = 1
  if t.equipInfo then
    id = t.equipInfo.data.id
    baseResType = BaseResType.Equipment
    count = 1
  else
    id = t.itemInfo.itemData.id
    baseResType = BaseResType.Item
    count = t.itemInfo.count
  end
  t.decomposeData = equip_decompose_data.GetDataByTypeAndItemId(baseResType,id)
  t.gainSoul = t.decomposeData.soul*count
  local tip = string.format(LocalizationController.instance:Get('ui.equip_decompose_view.gainEquipSoulAward'),t.gainSoul)
  
  --金币
  if t.equipInfo then
    t.gainMoney = equip_model.GetStrengthenMoney(t.equipInfo.data.strengthen_type,0,t.equipInfo.strengthenLevel)
    if t.gainMoney > 0 then
      tip = tip .. string.format(LocalizationController.instance:Get('ui.equip_decompose_view.gainMoneyAward'),t.gainMoney)
    end
    
  end
  
  --去除回车建
  tip = string.sub(tip,0,-2)
  t.textGainAward.text = tip
end

function t.Close()
  t.transform = nil
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
end
-----------------------------click event----------------------------
function t.ClickCloseButtonHandler ()
  t.Close()
end
function t.ClickToggleHandler(value)
  gamemanager.GetModel('consume_tip_model').SetConsumeTipEnable(ConsumeTipType.EquipDecompose,not value)
end
function t.ClickConfirmDecomposeButtonHandler ()
  
  equip_controller.EquipDeComposeReqByEquipInfo(t.equipInfo)
  equip_controller.EquipDeComposeReqByEquipPiece(t.itemInfo)
end
--------------------------update by protocol------------------------
function t.EquipDecomposeSucByProtocol()
  t.Close()
end

return t