local t = {}
local PREFAB_PATH = 'ui/pvp/pvp_race_intro_view'

local arena_model = gamemanager.GetModel('arena_model')

function t.Open()
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Replace)
  t.BindDelegate()
  t.transform = gameObject:GetComponent(typeof(Transform))
  local frame = t.transform:Find('core/frame')
  t.textRule = frame:Find('text_rule'):GetComponent(typeof(Text))
  t.textCountDownTitle = frame:Find('text_count_down_title'):GetComponent(typeof(Text))
  t.textCountDown = frame:Find('text_count_down'):GetComponent(typeof(Text))
  t.textCurScore = frame:Find('text_cur_score'):GetComponent(typeof(Text))
  t.textLastRank = frame:Find('text_last_rank'):GetComponent(typeof(Text))
  t.btnEnter = frame:Find('btn_enter'):GetComponent(typeof(Button))
  t.textBtnEnter = frame:Find('btn_enter/text_enter'):GetComponent(typeof(Text))
  
  t.InitEvent()  
   
  local common_top_bar = require ('ui/common_top_bar/common_top_bar')
  common_top_bar = common_top_bar:Create(t.transform:Find('core'))
  common_top_bar.transform:SetAsFirstSibling()
  common_top_bar:SetAsCommonStyle(LocalizationController.instance:Get('ui.pvp_race_view.text_race_title'),t.OnClickBackBtnHandler,true,true,true,false,false,false,false)
   
  uimanager.RegisterView(PREFAB_PATH,t)
end

function t.RefreshInfo()
  if(arena_model.isOpen) then 
    t.btnEnter.gameObject:SetActive(true)
    t.textCountDownTitle.text = LocalizationController.instance:Get('ui.pvp_race_intro_view.text_count_down_end_title')
  else
    t.btnEnter.gameObject:SetActive(false)
    t.textCountDownTitle.text = LocalizationController.instance:Get('ui.pvp_race_intro_view.text_count_down_start_title')
  end
  t.textCurScore.text = arena_model.race  
  coroutine.stop(t.countDownCoroutine)
  t.countDownCoroutine = coroutine.start(t.Tick)  
end

function t.Tick()
  while(true) do
    local time = arena_model.time - TimeController.instance.ServerTimeTicksSecond
    if(time <= 0) then
      gamemanager.GetCtrl('arena_controller').GetPvpInfoReq()
      break
    end
    t.textCountDown.text = TimeUtil.FormatSecondToHour(time)
    coroutine.wait(1)
  end
end

function t.InitEvent()
  t.btnEnter.onClick:AddListener(t.OnClickEnterBtnHander)
end

function t.BindDelegate()
  gamemanager.GetModel('arena_model').UpdatePvpInfoDelegate:AddListener(t.UpdatePvpInfoByProtocol)
end

function t.UnbindDelegate()
  gamemanager.GetModel('arena_model').UpdatePvpInfoDelegate:RemoveListener(t.UpdatePvpInfoByProtocol)
end

function t.Close()
  coroutine.stop(t.countDownCoroutine)
  --t.btnEnter.onClick:RemoveListener(t.OnClickEnterBtnHander)
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
end
-- --------------------click event---------------------------------------
function t.OnClickEnterBtnHander()
  gamemanager.GetCtrl('arena_controller').OpenRaceView()
end

function t.OnClickBackBtnHandler()
  --t.Close()
  uimanager.CloseView(PREFAB_PATH)
end

------------------------update by protocol----------------- 

function t.UpdatePvpInfoByProtocol()
  t.RefreshInfo()
end

return t