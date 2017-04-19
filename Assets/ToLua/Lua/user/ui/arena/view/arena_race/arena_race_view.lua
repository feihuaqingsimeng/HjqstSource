local t = {}
local PREFAB_PATH = 'ui/pvp/pvp_race_view'
local arena_model = gamemanager.GetModel('arena_model')
local common_hero_icon = require('ui/common_icon/common_hero_icon')
local global_data = gamemanager.GetData('global_data')

function t.Open()
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  t.InitEvent()
  
  local common_top_bar = require ('ui/common_top_bar/common_top_bar')
  common_top_bar = common_top_bar:Create(t.transform:Find('core'))
  common_top_bar.transform:SetAsFirstSibling()
  common_top_bar:SetAsCommonStyle(LocalizationController.instance:Get('ui.pvp_race_view.text_race_title'),t.OnClickBackBtnHandler,true,true,true,false,false,false,false)
  
  local front = t.transform:Find('core/front/')
  t.textPower = front:Find('text_power_title/text_power'):GetComponent(typeof(Text))
  t.textRemindTimeTitle = t.transform:Find('core/text_remind_time_title'):GetComponent(typeof(Text))
  t.textRemindTime = t.transform:Find('core/text_remind_time'):GetComponent(typeof(Text))
  --t.textCurRank = front:Find('text_cur_rank_title/text_cur_rank'):GetComponent(typeof(Text))
  t.textCurScore = front:Find('text_cur_score_title/text_cur_score'):GetComponent(typeof(Text))
  t.textLastTime = t.transform:Find('core/front/txt_last_time'):GetComponent(typeof(Text))
  
  t.formation_root ={}
  t.formation_root[1] = t.transform:Find('core/front/formation_root/formation_1/role_icon_root')
  t.formation_root[2] = t.transform:Find('core/front/formation_root/formation_2/role_icon_root')
  t.formation_root[3] = t.transform:Find('core/front/formation_root/formation_3/role_icon_root')
  t.formation_root[4] = t.transform:Find('core/front/formation_root/formation_4/role_icon_root')
  t.formation_root[5] = t.transform:Find('core/front/formation_root/formation_5/role_icon_root')
  t.formation_root[6] = t.transform:Find('core/front/formation_root/formation_6/role_icon_root')
  t.formation_root[7] = t.transform:Find('core/front/formation_root/formation_7/role_icon_root')
  t.formation_root[8] = t.transform:Find('core/front/formation_root/formation_8/role_icon_root')
  t.formation_root[9] = t.transform:Find('core/front/formation_root/formation_9/role_icon_root')
  
  local fight_root = front:Find('fight_root')
  t.textWinCount = fight_root:Find('text_win_count'):GetComponent(typeof(Text))
  t.textLoseCount = fight_root:Find('text_lose_count'):GetComponent(typeof(Text))
  t.endline = t.transform:Find('core/endline')
  t.endline.gameObject:SetActive(false)
  t.textMatchTips = t.transform:Find('core/endline/txt_match_tips'):GetComponent(typeof(Text))
  
  t.scrollContent = fight_root:Find('Scroll View/Viewport/Content'):GetComponent(typeof(Common.UI.Components.ScrollContent))
  t.scrollContent.onResetItem:AddListener(t.OnResetItemHandler)
  
  --formation
  
  t.formationTeamType = gamemanager.GetModel('formation_model').CurrentPVESelectFormationTeamType
  t.RefreshEmbattle(t.formationTeamType)
  --t.formationGridView = dofile('ui/formation/view/formation_grid_view')
  --t.formationGridView.Init(t.transform:Find('core/formation_root'),t.formationTeamType,false)
  t.RefreshPower()  
  t.Refresh()
  uimanager.RegisterView(PREFAB_PATH,t)
end

function t.Tick()
  if(arena_model.isOpen) then
    t.textRemindTimeTitle.text = LocalizationController.instance:Get('ui.pvp_race_view.text_remind_time')
  else
    t.textRemindTimeTitle.text = LocalizationController.instance:Get('ui.pvp_race_view.text_next_time')
  end
  while(true) do
    local time = arena_model.time - TimeController.instance.ServerTimeTicksSecond
    t.textRemindTime.text = TimeUtil.FormatSecondToHour(time)
    coroutine.wait(1)
  end
end

function t.Refresh()
  t.textCurScore.text = arena_model.race
  --t.textWinLose.text = string.format(LocalizationController.instance:Get('ui.pvp_race_view.text_win_lose'),arena_model.winTimes,arena_model.loseTimes)
  
  t.textWinCount.text = tostring(arena_model.winTimes)
  t.textLoseCount.text = tostring(arena_model.loseTimes)
  
  t.lastTime = global_data.point_pvp_daily_time - arena_model.useTimes
  t.textLastTime.text = string.format(LocalizationController.instance:Get('ui.pvp_race_view.txt_last_time'),t.lastTime,global_data.point_pvp_daily_time)
  t.scrollContent:Init(#arena_model.rec, playAnimation, 0)
  coroutine.stop(t.countDownCoroutine)
  t.countDownCoroutine = coroutine.start(t.Tick)  
end

function t.RefreshPower()    
    local formationTeamInfo = gamemanager.GetModel('formation_model').GetFormationTeam(t.formationTeamType)
    t.textPower.text = formationTeamInfo:Power()
 end 

function t.InitEvent()
  local btnRank = t.transform:Find('core/front/btn_rank'):GetComponent(typeof(Button))
  btnRank.onClick:AddListener(t.OnClickRankBtnHandler)
  
  local btnFormation = t.transform:Find('core/front/btn_formation'):GetComponent(typeof(Button))
  btnFormation.onClick:AddListener(t.OnClickFormationBtnHandler)
  
  t.btnPrepare = t.transform:Find('core/front/btn_prepare'):GetComponent(typeof(Button))
  t.btnPrepare.onClick:AddListener(t.OnClickPrepareBtnHandler)
  --t.textPrepare = t.transform:Find('core/front/btn_prepare/text_prepare'):GetComponent(typeof(Text))
  
  t.btnCancel = t.transform:Find('core/endline/btn_cancel'):GetComponent(typeof(Button))
  t.btnCancel.onClick:AddListener(t.OnClickCancelBtnHandler)
  --t.textCancel = t.transform:Find('core/endline/btn_prepare/txt_cancel'):GetComponent(typeof(Text))
end

function t.OnResetItemHandler(gameObject,index)
  local text_fight_opponent = gameObject.transform:Find('text_fight_opponent'):GetComponent(typeof(Text))
  local text_fight_result = gameObject.transform:Find('text_fight_result'):GetComponent(typeof(Text))
  local text_fight_race = gameObject.transform:Find('text_fight_race'):GetComponent(typeof(Text))
  local bgGameObject = gameObject.transform:Find('bg').gameObject
  local r = arena_model.rec[index + 1]
  text_fight_opponent.text = r.name
  if(r.result > 0) then
    text_fight_result.text = LocalizationController.instance:Get('ui.pvp_race_view.win')
    text_fight_race.text = string.format(LocalizationController.instance:Get('ui.pvp_race_view.win_score'),r.race)
  else
    text_fight_result.text = LocalizationController.instance:Get('ui.pvp_race_view.fail')
    text_fight_race.text = string.format(LocalizationController.instance:Get('ui.pvp_race_view.lose_score'),r.race)
  end 
  bgGameObject:SetActive(index % 2 == 1)
end

function t.RefreshEmbattle(formationTeamType)
  t.formationTeamType = formationTeamType
  t.RefreshPower()
  local gameModel = gamemanager.GetModel('game_model')
  local heroModel = gamemanager.GetModel('hero_model')
  local formation_model = gamemanager.GetModel('formation_model')
  local formationTeamTable = formation_model.GetFormationTeam(formationTeamType)
  for k,v in pairs(t.formation_root) do
    ui_util.ClearChildren(v,true)
  end
  for k,v in pairs(formationTeamTable.teamPosTable) do
    local parentTrans = t.formation_root[k]
    if gameModel.IsPlayer(v) then
      local roleInfo = gameModel.playerInfo
      local heroIcon =  common_hero_icon.New(parentTrans)
      heroIcon:SetRoleInfo(roleInfo,true)
    else
      local roleInfo = heroModel.GetHeroInfo(v)
      local heroIcon =  common_hero_icon.New(parentTrans)
      heroIcon:SetRoleInfo(roleInfo,false)
      heroIcon:SetRoleInfo(roleInfo,false)
    end
  end
  
  --t.formationGridView.RefreshView(formationTeamType)
end

function t.Matching()
  local delay = UnityEngine.Random.Range(5,20)
  local cost = 0
  local last = delay - cost
  while(cost <= delay) do
    t.textMatchTips.text =string.format(LocalizationController.instance:Get('ui.pvp_race_view.txt_match_tips'),TimeUtil.FormatSecondToHour(last))
    coroutine.wait(1)
    cost = cost + 1
    last = delay - cost
  end
  gamemanager.GetCtrl('arena_controller').PointPvpChallengeReq()
end

function t.Close()  
  coroutine.stop(t.countDownCoroutine)
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end

-- --------------------click event---------------------------------------
function t.OnClickBackBtnHandler()  
  uimanager.CloseView(PREFAB_PATH)
end

function t.OnClickRankBtnHandler()
  gamemanager.GetCtrl('arena_controller').OpenRaceRankView()
end

function t.OnClickFormationBtnHandler()
  gamemanager.GetCtrl('arena_controller').OpenRaceEmbattleView(t.RefreshEmbattle)
end

function t.OnClickPrepareBtnHandler()  
  local time = arena_model.time - TimeController.instance.ServerTimeTicksSecond  
  if(time < 30) then
    local confirm_tip_view = require('ui/tips/view/common_error_tips_view')
    confirm_tip_view.Open(LocalizationController.instance:Get('ui.pvp_race_view.out_of_time'))
  elseif(t.lastTime > 0) then
    t.endline.gameObject:SetActive(true)
    t.matchCoroutine = coroutine.start(t.Matching)  
  else
    local confirm_tip_view = require('ui/tips/view/common_error_tips_view')
    confirm_tip_view.Open(LocalizationController.instance:Get('ui.pvp_race_view.less_of_match_time'))
  end
  --gamemanager.GetCtrl('arena_controller').PointPvpChallengeReq()
end

function t.OnClickCancelBtnHandler()
  t.endline.gameObject:SetActive(false)
  coroutine.stop(t.matchCoroutine)
end
return t