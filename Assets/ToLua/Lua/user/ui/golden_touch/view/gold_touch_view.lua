local t = {}
local PREFAB_PATH = 'ui/golden_touch/golden_touch_view'
local name = PREFAB_PATH

local golden_touch_data = gamemanager.GetData('golden_touch_data')
local game_model = gamemanager.GetModel('game_model')
local golden_touch_model = gamemanager.GetModel('golden_touch_model')
local golden_touch_controller = gamemanager.GetCtrl('golden_touch_controller')
local vip_model = gamemanager.GetModel('vip_model')
local auto_destroy_tips_view = require('ui/tips/view/auto_destroy_tip_view')

function t.Open()
  if uimanager.GetView(name) then
    return
  end
  uimanager.RegisterView(name,t)
  
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  t.BindDelegate()
  t.InitComponent()
  golden_touch_controller.GoldHandInfoReq()
  
  t.InitScrollContent()
  t.Refresh()
end

function t.Close()
  t.transform = nil
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
end

function t.BindDelegate()
  golden_touch_controller.UpdateGoldenInfoListDelegate:AddListener(t.InitByProtocol)
  golden_touch_controller.UpdateGoldenSuccessDelegate:AddListener(t.GoldenSuccessByProtocol)
  game_model.onUpdateBaseResourceDelegate:AddListener(t.Refresh)
  vip_model.OnVIPInfoUpdateDelegate:AddListener(t.Refresh)
end

function t.UnbindDelegate()
  golden_touch_controller.UpdateGoldenInfoListDelegate:RemoveListener(t.InitByProtocol)
  golden_touch_controller.UpdateGoldenSuccessDelegate:RemoveListener(t.GoldenSuccessByProtocol)
  game_model.onUpdateBaseResourceDelegate:RemoveListener(t.Refresh)
  vip_model.OnVIPInfoUpdateDelegate:RemoveListener(t.Refresh)
end

function t.InitComponent()
  local img_frame = t.transform:Find('core/img_frame')
  t.btnClose = ui_util.FindComp(img_frame,'btn_close',Button)
  t.btnClose.onClick:AddListener(t.ClickCloseHandler)
  t.btnCancel = ui_util.FindComp(img_frame,'btn_cancel',Button)
  t.btnCancel.onClick:AddListener(t.ClickCloseHandler)
  t.btnUse = ui_util.FindComp(img_frame,'btn_ok',Button)
  t.btnUse.onClick:AddListener(t.ClickUseHandler)
  t.scrollContent = ui_util.FindComp(img_frame,'Scroll View/Viewport/Content',ScrollContentExpand)
  t.scrollContent:AddResetItemListener(t.OnResetItemHandler)
  --vip
  t.useRoot = img_frame:Find('use_bg')
  t.textUseTip = ui_util.FindComp(t.useRoot,'text_use_tip',Text)
  t.textDiamondCount = ui_util.FindComp(t.useRoot,'text_diamond_count',Text)
  t.textGoldCount = ui_util.FindComp(t.useRoot,'text_gold_count',Text)
  t.textNone = ui_util.FindComp(img_frame,'text_none',Text)
  
end

function t.Refresh()
  local isEnough = golden_touch_model.GetUseTimes() < golden_touch_model.GetMaxTimes()
  if isEnough then
    
    t.useRoot.gameObject:SetActive(true)
    t.textNone.gameObject:SetActive(false)
    t.textDiamondCount.text = golden_touch_data.GetDiamondCostByTimes(golden_touch_model.GetUseTimes())
    t.textGoldCount.text = golden_touch_data.GetGoldByAccountLevel(game_model.accountLevel,golden_touch_model.GetUseTimes())
    t.textUseTip.text = string.format(LocalizationController.instance:Get('ui.golden_touch_view.useTip'),vip_model.vipLevel,golden_touch_model.GetMaxTimes(),golden_touch_model.GetUseTimes(),golden_touch_model.GetMaxTimes())
  else
    t.useRoot.gameObject:SetActive(false)
    t.textNone.gameObject:SetActive(true)
    local vip,extraCount = golden_touch_model.GetNextVipAndExtraTimes()
    if vip > vip_model.vipLevel then --还可以提升vip哟
      t.textNone.text = string.format(LocalizationController.instance:Get('ui.golden_touch_view.needUpVipTip'),vip,extraCount)
    else
      t.textNone.text = LocalizationController.instance:Get('ui.golden_touch_view.noTimesTip')
    end
  end
  local ownDiamond = game_model.GetBaseResourceValue(BaseResType.Diamond)
  local isGray = not isEnough or ownDiamond < golden_touch_data.GetDiamondCostByTimes(golden_touch_model.GetUseTimes())
  ui_util.SetGrayChildren(t.btnUse.transform,isGray ,true)
end



function t.InitScrollContent()
  t.scrollContent:Init(golden_touch_model.useLogList.Count,false,0)
  t.scrollContent:ScrollToBottom(0.01)
end
function t.RefeshScrollContent()
  t.scrollContent:RefreshCount(golden_touch_model.useLogList.Count,-1)
  t.scrollContent:ScrollToBottom(0.3)
end
----------------------click event----------------------------
function t.ClickCloseHandler()
  t.Close()
end

t.isClickUse = false
function t.ClickUseHandler()
  if t.isClickUse then
    Debugger.Log('please hold on second ')
    return
  end
  t.isClickUse = true
  coroutine.start(function()
      coroutine.wait(0.3)
      t.isClickUse = false
    end)
  if golden_touch_model.GetUseTimes() >= golden_touch_model.GetMaxTimes() then
    return
  end
  if not game_model.CheckBaseResEnoughByType(BaseResType.Diamond,golden_touch_data.GetDiamondCostByTimes(golden_touch_model.GetUseTimes())) then
    return 
  end
  golden_touch_controller.GoldHandUseReq()
end

function t.OnResetItemHandler(go,index)
  local logInfo = golden_touch_model.useLogList:Get(index+1)
  go.transform:Find('item_root/text_item'):GetComponent(typeof(Text)).text = logInfo:ToString()
  local img_crit = go.transform:Find('item_root/img_crit'):GetComponent(typeof(Image))
  if logInfo.crit <= 1 then
    img_crit.gameObject:SetActive(false)
  else
    img_crit.gameObject:SetActive(true)
    img_crit.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/icon_baoji_'..logInfo.crit)
  end
  
  local mod = index-math.floor(index/2)*2
  go.transform:Find('bg1').gameObject:SetActive(mod == 0)
  go.transform:Find('bg2').gameObject:SetActive(mod ~= 0)
end
-----------------------------update by protocol --------------------
function t.InitByProtocol()
  Debugger.Log('响应点金手信息')
  t.InitScrollContent()
  t.Refresh()
end

function t.GoldenSuccessByProtocol()
  Debugger.Log('响应点金手')
  auto_destroy_tips_view.Open('点金成功')
  t.Refresh()
  t.RefeshScrollContent()
end

return t