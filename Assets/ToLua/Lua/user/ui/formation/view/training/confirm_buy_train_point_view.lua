local t = {}
local PREFAB_PATH ='ui/train_formation/comfirm_buy_train_point_view'

local consume_tip_model = gamemanager.GetModel('consume_tip_model')

function t.Open(costDiamond ,buyCount,remaindCount,okCallback)
  local gameObject = UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay)
  t.transform = gameObject.transform
  
  t.costDiamond = costDiamond
  t.buyCount = buyCount
  t.remaindCount = remaindCount
  t.okCallback = okCallback
  
  t.InitComponent()
  t.Refresh()
  
end
function t.Close()
  UIMgr.instance:Close(PREFAB_PATH)
end
function t.InitComponent()
  local frame = t.transform:Find('core/imf_frame')
  t.textCostDiamond = frame:Find('tip/text_diamond_cost'):GetComponent(typeof(Text))
  t.textBuyCount = frame:Find('tip/text_buy_num'):GetComponent(typeof(Text))
  t.textRemindText = frame:Find('text_remaind_count'):GetComponent(typeof(Text))
  t.btnCancel = frame:Find('btn_close'):GetComponent(typeof(Button))
  t.btnCancel.onClick:AddListener(t.ClickCloseBtnHandler)
  t.btnCancel = frame:Find('btn_cancel'):GetComponent(typeof(Button))
  t.btnCancel.onClick:AddListener(t.ClickCloseBtnHandler)
  t.btnSure = frame:Find('btn_sure'):GetComponent(typeof(Button))
  t.btnSure.onClick:AddListener(t.ClickSureBtnHandler)
  t.toggleTip = frame:Find('Toggle_tip'):GetComponent(typeof(Toggle))
end

function t.Refresh()
  t.textCostDiamond.text = string.format(LocalizationController.instance:Get("ui.train_buy_point_view.diamondCost"),t.costDiamond)
  t.textBuyCount.text = string.format(LocalizationController.instance:Get("ui.train_buy_point_view.buyNum"),t.buyCount)
  t.textRemindText.text = string.format(LocalizationController.instance:Get("ui.train_buy_point_view.remaindTime"),t.remaindCount)
  --if not consume_tip_model.HasConsumeTipKey(ConsumeTipType.DiamondBuyFormationTrainPoint) then
    t.toggleTip.gameObject:SetActive(false)
 -- end

end

-----------------------------click event-----------------------
function t.ClickSureBtnHandler()
  print('ClickSureBtnHandler')
  consume_tip_model.SetConsumeTipEnable(ConsumeTipType.DiamondBuyFormationTrainPoint,not t.toggleTip.isOn)
  t.Close()
  
  if not gamemanager.GetModel('game_model').CheckBaseResEnoughByType(BaseResType.Diamond,t.costDiamond) then
    return
  end
  if t.okCallback then
    t.okCallback()
  end
  
end

function t.ClickCloseBtnHandler()
  t.Close()
end

return t