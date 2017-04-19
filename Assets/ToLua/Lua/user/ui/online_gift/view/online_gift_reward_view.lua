local t = {}
local PREFAB_PATH = 'ui/online_gift/online_gift_reward_view'
local name = PREFAB_PATH

local common_reward_icon = require('ui/common_icon/common_reward_icon')
local online_gift_model = gamemanager.GetModel('online_gift_model')
local online_gift_controller = gamemanager.GetCtrl('online_gift_controller')
local reward_auto_destroy_tip_view = require('ui/tips/view/reward_auto_destroy_tip_view')

function t.Open()
  uimanager.RegisterView(name,t)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  t.BindDelegate()
  t.InitComponent()
  
  t.Refresh()
end
function t.Close()  
  coroutine.stop(t.UpdateCoroutine)
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegaete()
end

function t.BindDelegate()
  online_gift_model.GetOnlineGiftRewardSucDelegate:AddListener(t.GetOnlineGiftRewardSucByProtocol)
end

function t.UnbindDelegaete()
  online_gift_model.GetOnlineGiftRewardSucDelegate:RemoveListener(t.GetOnlineGiftRewardSucByProtocol)

end

function t.InitComponent()
  t.btnClose = t.transform:Find('core/btn_close'):GetComponent(typeof(Button))
  t.btnClose.onClick:AddListener(t.ClickCloseBtnHandler)
  t.tranRewardRoot = t.transform:Find('core/img_frame/reward_icons_root')
  t.textTip = t.transform:Find('core/text_tip'):GetComponent(typeof(Text))
  t.btnReceive = t.transform:Find('core/btn_receive'):GetComponent(typeof(Button))
  t.btnReceive.onClick:AddListener(t.ClickReceiveBtnHandler)
end

function t.Refresh()
  ui_util.ClearChildren(t.tranRewardRoot,true)
  local currentRewardId = online_gift_model.GetCurrentGiftId()
  local giftData = gamemanager.GetData('online_gift_data').GetDataById(currentRewardId)
  if(giftData ~= nil) then
    for k,v in pairs(giftData.award) do
      local icon = common_reward_icon.New(t.tranRewardRoot,v)
    end
  end
  coroutine.stop(t.UpdateCoroutine)
  t.UpdateCoroutine = coroutine.start(t.RefreshOnlineGift)
end

function t.RefreshOnlineGift()
  while(true) do
    local endTime = online_gift_model.GetEndTime()
    if(endTime > 0) then
      t.textTip.text =  TimeUtil.FormatSecondToHour(endTime)
    else
      t.textTip.text = ''
    end
    local canGetReward = online_gift_model.CanGetOnlineGift(endTime)
    ui_util.SetGrayChildren(t.btnReceive.transform,not canGetReward,true) 
    coroutine.wait(1)
  end
end

----------------------click event-----------------
function t.ClickCloseBtnHandler()
  uimanager.CloseView(name)
end
function t.ClickReceiveBtnHandler()
    local endTime = online_gift_model.GetEndTime()
    local canGetReward = online_gift_model.CanGetOnlineGift(endTime)
    if(canGetReward) then
      online_gift_controller.OnlineGiftReq(online_gift_model.GetCurrentGiftId())
    end
end
------------------update by protocol
function t.GetOnlineGiftRewardSucByProtocol(id) 
  local giftData = gamemanager.GetData('online_gift_data').GetDataById(id)
  --reward_auto_destroy_tip_view.OpenByList(giftData.award,false)
  gamemanager.GetModel("tips_model").GetTipView('common_reward_tips_view').Create(giftData.award)
  t.ClickCloseBtnHandler()
end

return t