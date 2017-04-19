local t = {}
local PREFAB_PATH = 'ui/pvp/pvp_challenge_reward_view'
local name = PREFAB_PATH

local global_data = gamemanager.GetData('global_data')
local common_reward_icon = require('ui/common_icon/common_reward_icon')
local arena_model = gamemanager.GetModel('arena_model')
local arena_controller = gamemanager.GetCtrl('arena_controller')
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
  
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegaete()
end

function t.BindDelegate()
  arena_controller.GainArenaChallengeRewardSucDelegate:AddListener(t.GainArenaChallengeRewardSucByProtocol)
end

function t.UnbindDelegaete()
  arena_controller.GainArenaChallengeRewardSucDelegate:RemoveListener(t.GainArenaChallengeRewardSucByProtocol)

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
  for k,v in pairs(global_data.arena_winning_streak_award) do
    local icon = common_reward_icon.New(t.tranRewardRoot,v)
  end
  t.RefreshTimes()
end
function t.RefreshTimes()
  local isEnough = arena_model.canUseWinTimes >= global_data.arena_winning_streak_num
  t.textTip.text = string.format(LocalizationController.instance:Get('ui.pvp_view.totoalSucChallengeTimeTip'),arena_model.canUseWinTimes,global_data.arena_winning_streak_num)
  ui_util.SetGrayChildren(t.btnReceive.transform,not isEnough,true)
end
----------------------click event-----------------
function t.ClickCloseBtnHandler()
  uimanager.CloseView(name)
end
function t.ClickReceiveBtnHandler()
  arena_controller.GetWinTimesAwardReq()
end
------------------update by protocol
function t.GainArenaChallengeRewardSucByProtocol()
  t.RefreshTimes()
  --reward_auto_destroy_tip_view.OpenByList(global_data.arena_winning_streak_award,false)
   gamemanager.GetModel("tips_model").GetTipView('common_reward_tips_view').Create(global_data.arena_winning_streak_award)
end

return t