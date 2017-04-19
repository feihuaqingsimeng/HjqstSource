local t = {}
local PREFAB_PATH = "ui/tips/common_expand_bag_tips_view"

local game_model = gamemanager.GetModel('game_model')
local global_data = gamemanager.GetData('global_data')

--bagType 1.heroBag 2.equipBag
function t.Open(bagType,cost,okCallback,consumeTipType)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.Tips, UIOpenMode.Overlay)
  t.transform = gameObject.transform
  t.bagType = bagType
  t.cost= cost
  t.okCallback = okCallback
  t.consumeTipType = consumeTipType
  
  local bg = t.transform:Find('core/common_tips_root/img_background')
  t.textCost = bg:Find('text_cost'):GetComponent(typeof(Text))
  t.textDes = bg:Find('text_description'):GetComponent(typeof(Text))
  t.toggleTip = bg:Find('Toggle_tip'):GetComponent(typeof(Toggle))
  t.toggleTip.gameObject:SetActive(false)
  t.btnCancel = bg:Find('btn_cancel'):GetComponent(typeof(Button))
  t.btnCancel.onClick:AddListener(t.Close)
  t.btnOk = bg:Find('btn_ok'):GetComponent(typeof(Button))
  t.btnOk.onClick:AddListener(t.ClickOkHandler)
  
  if game_model.GetBaseResourceValue(BaseResType.Diamond) < t.cost then
    t.textCost.text = ui_util.FormatToRedText(cost)
  else
    t.textCost.text = cost
  end
  
  if bagType == 1 then
    t.textDes.text = string.format(LocalizationController.instance:Get('ui.common_expand_bag_tips_view.not_enough_bag_description2'), global_data.hero_package_buy_num)
  elseif bagType == 2 then
    t.textDes.text = string.format(LocalizationController.instance:Get('ui.common_expand_bag_tips_view.not_enough_bag_description2'), global_data.equip_package_buy_num)
  end
end
function t.Open2(bagType,cost)
  t.Open(bagType,cost,nil,ConsumeTipType.None)
end
function t.Close()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end
function t.ClickOkHandler()
  local common_error_tips_view = require('ui/tips/view/common_error_tips_view')
  if t.bagType == 1 and game_model.heroCellNum >= global_data.hero_package_max_num then
    common_error_tips_view.Open(LocalizationController.instance:Get('ui.common_tips.hero_bag__reach_max'))
    return
  end
  if t.bagType == 2 and game_model.equipCellNum >=  global_data.equip_package_max_num then
    common_error_tips_view.Open(LocalizationController.instance:Get('ui.common_tips.equipment_bag_reach_max'))
    return
  end
  if game_model.GetBaseResourceValue(BaseResType.Diamond) < t.cost then
    common_error_tips_view.Open(LocalizationController.instance:Get("ui.common_tips.not_enough_diamond"))
    return
  end
  
  gamemanager.GetCtrl('game_controller').BuyPackCellReq(t.bagType)
  
  if t.okCallback then
    t.okCallback()
  end
  t.Close()
end

return t