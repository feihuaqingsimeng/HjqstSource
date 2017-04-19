local t = {}
local PREFAB_PATH = 'ui/main/main_view'
local name = PREFAB_PATH

local game_model = gamemanager.GetModel('game_model')
local hero_controller = gamemanager.GetCtrl('hero_controller')
local hero_model = gamemanager.GetModel('hero_model')
local function_open_model = gamemanager.GetModel('function_open_model')
local vip_model = gamemanager.GetModel('vip_model')
local global_data = gamemanager.GetData('global_data')
local world_boss_model = gamemanager.GetModel('world_boss_model')
local formation_model = gamemanager.GetModel('formation_model')
local player_model = gamemanager.GetModel('player_model')
local online_gift_model = gamemanager.GetModel('online_gift_model')
local sign_in_model = gamemanager.GetModel('sign_in_model')
local arena_controller = gamemanager.GetCtrl('arena_controller')
local activity_model = gamemanager.GetModel('activity_model')

function t.Open()
  if uimanager.GetView(name) then
    Debugger.LogError('main view is already open------------------------')
    return 
  end
  uimanager.RegisterView(name,t)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  local common_top_bar = require ('ui/common_top_bar/common_top_bar')
  common_top_bar = common_top_bar:Create(t.transform:Find('core'))
  common_top_bar:SetAsMainViewStyle()
  
  t.characters = {}
  t.isLeftFunctionButtonsFold = false
  t.buttonFoldSprite = ResMgr.instance:LoadSprite('sprite/main_ui/btn_arrow_04_2')
  t.buttonUnfoldSprite = ResMgr.instance:LoadSprite('sprite/main_ui/btn_arrow_04')
  
  t.BindDelegate()
  t.InitComponent()
  t.Refresh()
  
  LuaCsTransfer.SystemNoticeCreate(t.systemNoticeRoot)
  
  AudioController.instance:PlayBGMusic(AudioController.MAINSCENE)
  AudioController.instance:SetBGMusicState(AudioController.instance.isOpenAudioBg)
  
  t.UpdateCoroutine = coroutine.start(t.UpdateView)
end
function t.Close()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
  coroutine.stop(t.UpdateCoroutine)
  t.DespawnCharacter()
end
function t.BindDelegate()
  vip_model.OnVIPInfoUpdateDelegate:AddListener(t.RefreshVipLevelHandler)
  game_model.onPveActionInfoUpdateDelegate:AddListener(t.RefreshPveActionNextRecoverTime)
  hero_model.onHeroInfoListUpdateDelegate:AddListener(t.RefreshHeroModels)
  player_model.OnPlayerInfoUpdateDelegate:AddListener(t.RefreshHeroModels)
  formation_model.FormationChangeDelegate:AddListener(t.RefreshHeroModels)
  formation_model.FormationUpgradeDelegate:AddListener(t.RefreshAccount)
  game_model.UpdateAccountLevelAndExpDelegate:AddListener(t.RefreshAccount)
  sign_in_model.onSignInUpdateDelegate:AddListener(t.RefreshSign)
  gamemanager.GetCtrl('illustration_ctrl').UpdatePowerDelegate:AddListener(t.RefreshAccount)
  activity_model.DelegateClose:AddListener(t.OnActivityClose)
end
function t.UnbindDelegate()
  vip_model.OnVIPInfoUpdateDelegate:RemoveListener(t.RefreshVipLevelHandler)
  game_model.onPveActionInfoUpdateDelegate:RemoveListener(t.RefreshPveActionNextRecoverTime)
  hero_model.onHeroInfoListUpdateDelegate:RemoveListener(t.RefreshHeroModels)
  player_model.OnPlayerInfoUpdateDelegate:RemoveListener(t.RefreshHeroModels)
  formation_model.FormationChangeDelegate:RemoveListener(t.RefreshHeroModels)
  formation_model.FormationUpgradeDelegate:RemoveListener(t.RefreshAccount)
  game_model.UpdateAccountLevelAndExpDelegate:RemoveListener(t.RefreshAccount)
  sign_in_model.onSignInUpdateDelegate:RemoveListener(t.RefreshSign)
  gamemanager.GetCtrl('illustration_ctrl').UpdatePowerDelegate:RemoveListener(t.RefreshAccount)
  activity_model.DelegateClose:RemoveListener(t.OnActivityClose)
end

function t.InitComponent()
  t.systemNoticeRoot = t.transform:Find('core/systemNoticeRoot')
  --账号相关
  local top_left_anchor = t.transform:Find('core/top_left_anchor')
  t.player_avatar_frame = top_left_anchor:Find('player_avatar_frame')
  t.player_avatar_frame:GetComponent(typeof(Button)).onClick:AddListener(t.ClickAccountInfoHandler)
  t.textAccountLevel = t.player_avatar_frame:Find('text_account_level'):GetComponent(typeof(Text))
  t.textAccountName = t.player_avatar_frame:Find('text_account_name'):GetComponent(typeof(Text))
  t.sliderAccountExp = t.player_avatar_frame:Find('slider_account_exp'):GetComponent(typeof(Slider))
  t.imgAccountHeadIcon = t.player_avatar_frame:Find('img_head'):GetComponent(typeof(Image))
  t.foldUnfoldFunctionsButton = t.player_avatar_frame:Find('btn_fold_unfold_functons'):GetComponent(typeof(Button))
  t.foldUnfoldFunctionsButton.onClick:AddListener(t.ClickFoldUnfoldFunctionsButton)
  t.foldUnfoldFunctionsButton:GetComponent(typeof(Image)).sprite = t.buttonFoldSprite
  t.textPower = t.player_avatar_frame:Find('text_power'):GetComponent(typeof(Text))
  --top
  local btnVip = top_left_anchor:Find('btn_vip'):GetComponent(typeof(Button))
  btnVip.onClick:AddListener(t.ClickVipBtnHandler)
  t.textVipLevel = btnVip.transform:Find('text_vip_level'):GetComponent(typeof(Text))
  if (t.transform:Find('core/text_pve_action_next_recover_time') ~= nil) then
    t.textPveActionNextRecoverTime = t.transform:Find('core/text_pve_action_next_recover_time'):GetComponent(typeof(Text))
  else
    
    t.textPveActionNextRecoverTime = t.transform:Find('core/top_left_anchor/text_pve_action_next_recover_time'):GetComponent(typeof(Text))
  end
  
  
  
  --pvp race
  t.pvpRaceButton = t.transform:Find('core/top_mid_anchor/pvp_race_root/btn_pvp_race'):GetComponent(typeof(Button))
  t.pvpRaceButton.onClick:AddListener(t.ClickPvpRaceButtonHandler)
  t.pvpRaceOpenTimeText = t.pvpRaceButton.transform:Find('text_pvp_race_open_time'):GetComponent(typeof(Text))
  --world boss
  t.worldBossButton = t.transform:Find('core/top_mid_anchor/world_boss_root/btn_world_boss'):GetComponent(typeof(Button))
  t.worldBossButton.onClick:AddListener(t.ClickWorldBossButtonHandler)
  t.worldBossOpenTimeText = t.worldBossButton.transform:Find('text_world_boss_open_time'):GetComponent(typeof(Text))
  --left
  local left = top_left_anchor:Find('left')
  t.btnTask = left:Find('btn_task'):GetComponent(typeof(Button))
  t.btnTask.onClick:AddListener(t.ClickTaskBtnHandler)
  t.btnMail = left:Find('btn_mail'):GetComponent(typeof(Button))
  t.btnMail.onClick:AddListener(t.ClickMailBtnHandler)
  t.btnFriend = left:Find('btn_friend'):GetComponent(typeof(Button))
  t.btnFriend.onClick:AddListener(t.ClickFriendBtnHandler)
  t.btnRank = left:Find('btn_ranking'):GetComponent(typeof(Button))
  t.btnRank.onClick:AddListener(t.ClickRankListBtnHandler)
  --玩家手册
  t.btn_player_manual = left:Find('btn_player_manual/btn_player_manual'):GetComponent(typeof(Button))
  t.btn_player_manual.onClick:AddListener(t.ClickPlayerManualBtnHanlder)
  t.btnSignIn= left:Find('btn_sign'):GetComponent(typeof(Button))
  t.btnSignIn.onClick:AddListener(t.ClickSignInBtnHandler)
  
  t.btnTaskInitialLocalPosition = t.btnTask.transform.localPosition
  t.btnMailInitialLocalPosition = t.btnMail.transform.localPosition
  t.btnFriendInitialLocalPosition = t.btnFriend.transform.localPosition
  t.btnRankInitialLocalPosition = t.btnRank.transform.localPosition
  t.btn_player_manual_InitialLocalPosition = t.btn_player_manual.transform.localPosition
  t.btnSignInInitialLocalPosition = t.btnSignIn.transform.localPosition
  --bottom
  local bottom = t.transform:Find('core/bottom_left_anchor/bottom')
  bottom:Find('btn_hero'):GetComponent(typeof(Button)).onClick:AddListener(t.ClickHeroTrainBtnHandler)
  bottom:Find('btn_pve_embattle'):GetComponent(typeof(Button)).onClick:AddListener(t.ClickPveEmbattleBtnHandler)
  bottom:Find('btn_shop'):GetComponent(typeof(Button)).onClick:AddListener(t.ClickShopBtnHandler)
  bottom:Find('btn_black_market'):GetComponent(typeof(Button)).onClick:AddListener(t.ClickBalckMarketBtnHandler)
  bottom:Find('btn_illustration'):GetComponent(typeof(Button)).onClick:AddListener(t.ClickIllustrationBtnHandler)
  bottom:Find('btn_explore'):GetComponent(typeof(Button)).onClick:AddListener(t.ClickExploreBtnHandler)
  bottom:Find('btn_consortia'):GetComponent(typeof(Button)).onClick:AddListener(t.ClickConsortiaBtnHandler)
  bottom:Find('btn_pack/btn_pack_pic'):GetComponent(typeof(Button)).onClick:AddListener(t.PackBtnHandler)
  bottom:Find('btn_hero_combine'):GetComponent(typeof(Button)).onClick:AddListener(t.HeroCombineBtnHandler)
  --top right
  local top_right_anchor = t.transform:Find('core/top_right_anchor')
  --online gift
  t.btn_online_gift_root = top_right_anchor:Find('btn_online_gift')
  t.btn_online_gift = top_right_anchor:Find('btn_online_gift/btn_online_gift_pic')
  t.btn_online_gift:GetComponent(typeof(Button)).onClick:AddListener(t.ClickOnlineGiftBtnHanlder)
  t.text_online_gift_name = top_right_anchor:Find('btn_online_gift/text_online_gift_name'):GetComponent(typeof(Text))
  t.btn_online_gift_animator = t.btn_online_gift:GetComponent(typeof(Animator))
  t.btn_online_gift_animator.enabled = false
  t.online_gift_red_point = t.btn_online_gift:Find('img_red_point')
  
  --first charge
  t.btn_first_charge = top_right_anchor:Find('btn_first_charge/btn_first_charge_pic')
  t.btn_first_charge:GetComponent(typeof(Button)).onClick:AddListener(t.ClickFirstChargeBtnHanlder)  
  t.btn_first_charge_animator = t.btn_first_charge:GetComponent(typeof(Animator))
  
  --seven hilarity
   t.btn_seven_hilarity = top_right_anchor:Find('btn_seven_hilarity/img_Icon'):GetComponent(typeof(Button))
   t.btn_seven_hilarity.onClick:AddListener(t.ClickSevenHilarityBtnHanlder) 
   t.seven_hilarity_red_pont = top_right_anchor:Find('btn_seven_hilarity/img_point').gameObject
   --大转盘
   t.btn_turntable = t.transform:Find('core/top_right_anchor/btn_turntable/btn_turntable'):GetComponent(typeof(Button))
   t.btn_turntable.onClick:AddListener(t.ClickTurntableBtnHanlder)
   
   -- 活动
   top_right_anchor:Find('btn_activity'):GetComponent(typeof(Button)).onClick:AddListener(t.ClickActivityBtnHandler)
  --right
  local bottom_right_anchor = t.transform:Find('core/bottom_right_anchor')
  t.startFightIndicatorGameObject = bottom_right_anchor:Find('btn_start_fight/start_fight_indicator').gameObject
  bottom_right_anchor:Find('btn_start_fight'):GetComponent(typeof(Button)).onClick:AddListener(t.ClickSelectChapterBtnHandler)
  bottom_right_anchor:Find('btn_daily_activity'):GetComponent(typeof(Button)).onClick:AddListener(t.ClickDailyDungeonBtnHandler)
  bottom_right_anchor:Find('btn_multiple'):GetComponent(typeof(Button)).onClick:AddListener(t.ClickFightCenterBtnHandler)
  t.btn_daily_task = bottom_right_anchor:Find('btn_daily_task'):GetComponent(typeof(Button))
  t.btn_daily_task.onClick:AddListener(t.ClickTaskDailyBtnHandler)
  --t.btn_daily_task.transform:Find('img_lock').gameObject:SetActive(not function_open_model.IsFunctionOpen(FunctionOpenType.MainView_Task,false))
  
  --models
  local formationRoot = t.transform:Find('core/formation')
  t.formationPositionTable = {}
  for i = 0,4 do
    local table = {}
    table.root = formationRoot:Find(i)
    table.modelRoot = table.root:Find('hero_model_root_'..i)
    table.titleRoot = table.root:Find('common_role_title_'..i)
    table.textLevel = table.titleRoot:Find('text_level'):GetComponent(typeof(Text))
    table.textName = table.titleRoot:Find('text_name'):GetComponent(typeof(Text))
    table.imgRoleType = table.titleRoot:Find('img_role_type_icon'):GetComponent(typeof(Image))
    table.btnClickTrigger = table.root:Find('btn_click'):GetComponent(typeof(EventTriggerDelegate))
    table.btnClickTrigger.onClick:AddListener(t.ClickHeroHandler)
    table.goStarTable = {}
    for k = 1,6 do
      table.goStarTable[k] = table.titleRoot:Find('stars_root/img_star_'..k).gameObject
    end
    t.formationPositionTable[i+1] = table
    
  end
 
end
function t.DespawnCharacter()
  for k,v in pairs(t.characters) do
    if(v ~= nil) then
      if v.isRole then
        PoolController.instance:SetPoolForever(v.petEntity.name, false);
      end
      PoolController.instance:SetPoolForever(v.name, false);
      PoolController.instance:Despawn(v.name,v)
    end
  end
  t.characters = {}
end
function t.Refresh()
  t.RefreshAccount()
  t.RefreshVipLevelHandler()
  t.RefreshPveActionNextRecoverTime()
  t.RefreshHeroModels()
  t.RefreshSign()
  t.RefreshStartFightIndicator()
  t.RefreshFirstCharge()
  t.RefreshSevenHilarity()
end
--刷新英雄模型
function t.RefreshHeroModels()
  
  t.textPower.text = formation_model.GetCurPveFormationTeam():Power()
  
  t.DespawnCharacter()
  local teamPosTable = formation_model.GetCurPveFormationTeam().teamPosTable
  t.heroInstanceIDList = {}
  local index = 1
  local playerIndex = 0
  for k,v in pairs(teamPosTable) do
    t.heroInstanceIDList[index] = v
    if game_model.IsPlayer(v) then
      playerIndex = index
      
    end
    index = index + 1
  end
 -- 主角要一直站中间
  if index ~= 0 then
    local temp = t.heroInstanceIDList[1]
    t.heroInstanceIDList[1] = t.heroInstanceIDList[playerIndex]
    t.heroInstanceIDList[playerIndex] = temp
  end
  local id
  for k,v in pairs(t.formationPositionTable) do
    id = t.heroInstanceIDList[k]
    if id and id ~= 0 then
      v.root.gameObject:SetActive(true)
      v.titleRoot.gameObject:SetActive(true)
      if game_model.IsPlayer(id) then
        t.CreatePlayerEntityAsUIElementAsyn(game_model.playerInfo,v.modelRoot)
        v.modelRoot.localRotation = Quaternion.Euler(game_model.playerInfo.heroData.home_rotation)
        t.RefreshModelInfo(game_model.playerInfo,v)
      else
        t.CreateHeroEntityAsUIElementAsyn(id,v.modelRoot)
        local heroInfo = hero_model.GetHeroInfo(id)
        v.modelRoot.localRotation = Quaternion.Euler(heroInfo.heroData.home_rotation)
        t.RefreshModelInfo(heroInfo,v)
      end
    else
      v.root.gameObject:SetActive(false)
    end
    
  end
end
--异步
function t.CreatePlayerEntityAsUIElementAsyn(playerInfo,parent)
  local playerEntity = Logic.Character.CharacterEntity.CreatePlayerEntityAsUIElementAsyn(playerInfo.instanceID, parent,false,false,System.Action_Logic_Character_PlayerEntity(function(playerEntity)
      t.characters[playerInfo.instanceID] = playerEntity
      PoolController.instance:SetPoolForever(playerInfo:ModelName(), true)
        
      end),System.Action_Logic_Character_PetEntity(function(petEntity)
        
        if  petEntity then
          PoolController.instance:SetPoolForever(petEntity.name, true);
        end
      end))
end
--异步
function t.CreateHeroEntityAsUIElementAsyn(instanceId,parent)
  local heroEntity = Logic.Character.CharacterEntity.CreateHeroEntityAsUIElementAsyn(instanceId,parent,false,false,System.Action_Logic_Character_HeroEntity(function(heroEntity)
        local heroInfo = hero_model.GetHeroInfo(instanceId)
        PoolController.instance:SetPoolForever(heroInfo:ModelName(), true)
        t.characters[instanceId] = heroEntity
      end))
end
--刷新模型属性
function t.RefreshModelInfo(roleInfo,formationPosRoot)
  local table = formationPosRoot
    table.textLevel.text = roleInfo.level
    table.textName.text = roleInfo.heroData:GetNameWithQualityColor()
    table.imgRoleType.sprite = ui_util.GetRoleTypeBigIconSprite(roleInfo.heroData.roleType)
    for k ,v in pairs(table.goStarTable) do
        v:SetActive(k <= roleInfo.advanceLevel)
    end
end
--刷新签到按钮
function t.RefreshSign()
  t.btnSignIn.gameObject:SetActive(not sign_in_model.isSignInToday)
end
--刷新账号相关
function t.RefreshAccount()
  t.textAccountLevel.text = string.format('Lv%d',game_model.accountLevel)
  t.textAccountName.text = game_model.accountName
  t.sliderAccountExp.value = game_model.AccountExpPercent()
  t.imgAccountHeadIcon.sprite = ResMgr.instance:LoadSprite(ui_util.ParseHeadIcon(game_model.accountHeadNo))
  t.textPower.text = formation_model.GetCurPveFormationTeam():Power()
  t.RefreshStartFightIndicator()
end
--刷新vip
function t.RefreshVipLevelHandler()
  t.textVipLevel.text = vip_model.vipLevel
end
--刷新pve action
function t.RefreshPveActionNextRecoverTime()
  if game_model.pveAction >= game_model.pveActionMax then
    t.textPveActionNextRecoverTime.gameObject:SetActive(false)
  else
    t.textPveActionNextRecoverTime.gameObject:SetActive(true)
    t.textPveActionNextRecoverTime.text = TimeUtil.FormatSecondToMinute(game_model.pveActionNextRecoverTime)
  end
end
-- [[ Refresh Start Fight Indicator ]] --
function t.RefreshStartFightIndicator ()
  local showStartFightIndicator = (gamemanager.GetModel('game_model').accountLevel <= gamemanager.GetData('global_data').startFightIndicatorShowMaxLevel)
  t.startFightIndicatorGameObject:SetActive(showStartFightIndicator)
end
-- [[ Refresh Start Fight Indicator ]] --
--指定时间更新
function t.UpdateView()
  while(true) do
    t.RefreshWorldBoss()
    t.RefreshPVPRace()
    t.RefreshOnlineGift()
    t.RefreshFirstCharge()
    t.RefreshSevenHilarity()
    t.RefreshTurntable()
    coroutine.wait(1)
  end
end
--大转盘更新
function t.RefreshTurntable()
  local eventData = gamemanager.GetData('event_data').GetTurntableData()
    local start1 = System.DateTime.Parse(eventData.event_timestart)
    local end1 = System.DateTime.Parse(eventData.event_timeover)
    local serverTime = TimeController.instance.ServerTime
    start1 = (serverTime-start1).TotalSeconds
    --策划同学说要往后延迟一天再关闭（恩，你懂得）
    end1 = (end1-serverTime).TotalSeconds + 24*3600
    if start1 >= 0 and end1 >= 0  and eventData.open == '1' then
      t.btn_turntable.gameObject:SetActive( true)
    else
      t.btn_turntable.gameObject:SetActive( false)
    end
end
--世界boss 时间刷新
function t.RefreshWorldBoss()
  local shouldShowWorldBossButton = false
  --print(world_boss_model.isOpen,world_boss_model.openTime,world_boss_model.overTime,TimeUtil.FormatTimeToString(world_boss_model.openTime/1000,''),TimeUtil.FormatTimeToString(world_boss_model.overTime/1000,''))
  if world_boss_model.isOpen then--开始
    t.worldBossOpenTimeText.gameObject:SetActive(true)
    local diffTime = TimeController.instance:GetDiffTimeWithServerTimeInSecond(world_boss_model.overTime)
    t.worldBossOpenTimeText.text = TimeUtil.FormatSecondToHour(diffTime)
    shouldShowWorldBossButton = true
  else
      if world_boss_model.openTime > 0 then
        local diffTime = TimeController.instance:GetDiffTimeWithServerTimeInSecond(world_boss_model.openTime)
        if diffTime <= global_data.boss_in_before then--开始倒计时
          shouldShowWorldBossButton = true
          t.worldBossOpenTimeText.gameObject:SetActive(true)
          t.worldBossOpenTimeText.text = TimeUtil.FormatSecondToHour(diffTime)
        else
          diffTime = TimeController.instance:GetDiffTimeWithServerTimeInSecond(world_boss_model.overTime)
          
          if diffTime >= global_data.boss_in_last * -1 then--结束倒计时
            shouldShowWorldBossButton = true
            t.worldBossOpenTimeText.gameObject:SetActive(false)
            --t.worldBossOpenTimeText.text = TimeUtil.FormatSecondToHour(global_data.boss_in_last-math.abs(diffTime))
          end
          
        end
      end
      
    if (t.worldBossButton.transform.parent.gameObject.activeSelf ~= shouldShowWorldBossButton) then
      t.worldBossButton.transform.parent.gameObject:SetActive(shouldShowWorldBossButton)
    end
  end
  
end

--pvp 积分赛时间刷新
function t.RefreshPVPRace()
  local serverTime = TimeController.instance.ServerTime
  local startTime = global_data.point_pvp_start_time
  local diffTime = TimeUtil.GetDiffTime(serverTime, startTime)
  --print(serverTime:ToString(),startTime:ToString(),diffTime)
  local isOpen = arena_controller.IsOpenRace()
  local show = false
  if isOpen then
    if diffTime <= global_data.point_pvp_open_time and diffTime > 0 then--开放
      t.pvpRaceOpenTimeText.gameObject:SetActive(true)
      t.pvpRaceOpenTimeText.text = TimeUtil.FormatSecondToHour(diffTime)
      show = true
    elseif diffTime <= 0 and (math.abs(diffTime) < global_data.point_pvp_keep_time) then--延时关闭
      t.pvpRaceOpenTimeText.gameObject:SetActive(false)
      show = true
    else
      show = false
    end
  end
  if (t.pvpRaceButton.transform.parent.gameObject.activeSelf ~= show) then
    --t.pvpRaceButton.gameObject:SetActive(show)
    t.pvpRaceButton.transform.parent.gameObject:SetActive(show)
  end
end

--online gift update time
function t.RefreshOnlineGift()
  if(online_gift_model.GetCurrentGiftId() == 0) then
    t.btn_online_gift_root.gameObject:SetActive(false)
    t.text_online_gift_name.gameObject:SetActive(false)
  else
    if(t.btn_online_gift.gameObject.activeInHierarchy == false) then
      t.btn_online_gift_root.gameObject:SetActive(true)
    end
    if(t.text_online_gift_name.gameObject.activeInHierarchy == false) then
      t.text_online_gift_name.gameObject:SetActive(true)
    end
    local endTime = online_gift_model.GetEndTime()
    if(online_gift_model.CanGetOnlineGift(endTime)) then
      --t.btn_online_gift_animator.enabled = true
      --t.btn_online_gift_animator:Play('Normal')
      t.online_gift_red_point.gameObject:SetActive(true)
      t.text_online_gift_name.text = '' --LocalizationController.instance:Get('ui.main_view.onlineGift')
    else
      t.text_online_gift_name.text =  TimeUtil.FormatSecondToHour(endTime)
      --t.btn_online_gift_animator.enabled = false
      t.online_gift_red_point.gameObject:SetActive(false)
    end
  end
end

--first charge Update
function t.RefreshFirstCharge()  
  local btype = activity_model:GetFirstChargeType()
  
  local activityData = activity_model:GetActivityByType(btype)
  if nil == activityData then
    t.btn_first_charge.parent.gameObject:SetActive(false)
    do return end 
  end
  
  local IsAward = activityData:IsAlreadyAward()
  
  t.btn_first_charge.parent.gameObject:SetActive(not IsAward)
  
  if IsAward then do return end end
  local isFirstChagerAward = activity_model:IsActivityCanAwardByType(btype) 
  t.btn_first_charge_animator.enabled = isFirstChagerAward
  if isFirstChagerAward then t.btn_first_charge_animator:Play('Normal') end  
end

--seven hilarity Update
function t.RefreshSevenHilarity()
  local  IsSevenHilarityOverTime = activity_model:IsSevenHilarityOverTime()
  
  if not IsSevenHilarityOverTime then 
    local IsCanAward = activity_model:IsHaveHilarityCanAward()
    if IsCanAward then      
      t.seven_hilarity_red_pont:SetActive(true) 
      end
    t.btn_seven_hilarity.transform.parent.gameObject:SetActive(true)
  else t.btn_seven_hilarity.transform.parent.gameObject:SetActive(false)  end
end

---------------------click event----------------------------
--点击账号信息
function t.ClickAccountInfoHandler()
  LuaCsTransfer.OpenAccountInfoView()
end
--点击任务按钮
function t.ClickTaskBtnHandler()
  LuaCsTransfer.OpenTaskView()
end
--点击邮件
function t.ClickMailBtnHandler()
  LuaCsTransfer.OpenMailView()
end
--点击好友
function t.ClickFriendBtnHandler()
  LuaCsTransfer.OpenFriendView()
end
--点击排行榜
function t.ClickRankListBtnHandler()
  function_open_model.OpenFunction(FunctionOpenType.MainView_Ranking,1)
end
--点击签到
function t.ClickSignInBtnHandler()
  LuaCsTransfer.OpenSignInView()
end
--点击英雄培养
function t.ClickHeroTrainBtnHandler()
  function_open_model.OpenFunction(FunctionOpenType.MainView_Hero)
end
--点击阵型
function t.ClickPveEmbattleBtnHandler()
  function_open_model.OpenFunction(FunctionOpenType.MainView_FormationBtn)
end
--点击商店
function t.ClickShopBtnHandler()
  function_open_model.OpenFunction(FunctionOpenType.MainView_Shop,1)
end
--点击黑市
function t.ClickBalckMarketBtnHandler()
  function_open_model.OpenFunction(FunctionOpenType.MainView_BlackMarket)
end
--点击图鉴
function t.ClickIllustrationBtnHandler()
  function_open_model.OpenFunction(FunctionOpenType.MainView_illustrate)
end
--探险
function t.ClickExploreBtnHandler()
  function_open_model.OpenFunction(FunctionOpenType.MainView_Explore)
end
--公会
function t.ClickConsortiaBtnHandler()
  function_open_model.OpenFunction(FunctionOpenType.MainView_Consortia)
end
--点击背包
function t.PackBtnHandler()
  function_open_model.OpenFunction(FunctionOpenType.MainView_Pack)
end

--点击开始冒险
function t.ClickSelectChapterBtnHandler()
  LuaCsTransfer.OpenSelectChapter()
end
--点击每日副本
function t.ClickDailyDungeonBtnHandler()
  LuaCsTransfer.OpenDailyDungeonView()
end
---点击战斗中心
function t.ClickFightCenterBtnHandler()
  LuaCsTransfer.OpenFightCenterView()
end
--活动
function t.ClickActivityBtnHandler()
  local bTime = TimeController.instance.ServerTimeTicksMillisecond
  function_open_model.OpenFunction(FunctionOpenType.MainView_Activity)
end
---vip
function t.ClickVipBtnHandler()
  gamemanager.GetCtrl('vip_controller').OpenVIPView()
end
--pvp 积分赛
function t.ClickPvpRaceButtonHandler()
  function_open_model.OpenFunction(FunctionOpenType.FightCenter_PvpRace)
end
--world boss 
function t.ClickWorldBossButtonHandler()
  LuaCsTransfer.OpenWorldBossView()
end
--每日任务
function t.ClickTaskDailyBtnHandler()
  LuaCsTransfer.OpenTaskDailyView()
end
--online gift
function t.ClickOnlineGiftBtnHanlder()
  gamemanager.GetCtrl('online_gift_controller').OpenOnlineGiftRewardView()
end

--first charge
function t.ClickFirstChargeBtnHanlder()
  gamemanager.GetCtrl('activity_controller').OpenFirstCharge()
end

--seven hilarity
function t.ClickSevenHilarityBtnHanlder()
  gamemanager.GetCtrl('activity_controller').OpenSevenHilarity()
end
--大转盘
function t.ClickTurntableBtnHanlder()
  dofile('ui/activity/view/turntable/turntable_view').Open()
end

--玩家手册
function t.ClickPlayerManualBtnHanlder()
  local player_manual_controller = gamemanager.GetCtrl('player_manual_controller')
  player_manual_controller.OpenPlayerManualView ()
end

--活动界面关闭
function t.OnActivityClose()
  t.seven_hilarity_red_pont:SetActive(false)
end

--英雄合成
function t.HeroCombineBtnHandler()
   --hero_controller.OpenHeroComposeView()
   function_open_model.OpenFunction(FunctionOpenType.MainView_compose)
end
--点击英雄播音and 胜利动画
function t.ClickHeroHandler(go)
  local index = 0
  for k,v in pairs(t.formationPositionTable) do
    if v.btnClickTrigger.gameObject == go then
      index = k
      break
    end
  end
  local id = t.heroInstanceIDList[index]
  if id then
    if game_model.playerInfo.instanceID == id then
      gamemanager.GetModel('audio_model').PlayRandomAudioInView(AudioViewType.mainView,game_model.playerInfo.heroData.id)
    else
      gamemanager.GetModel('audio_model').PlayRandomAudioInView(AudioViewType.mainView,hero_model.GetHeroInfo(id).heroData.id)
    end
      AnimatorUtil.CrossFade(t.characters[id].anim,AnimatorUtil.VICOTRY_ID,0.3)
  end
  
  print(index,id)
end
function t.FoldTopLeftFunctionButtons ()
  LeanTween.moveY(t.btnTask.gameObject, t.imgAccountHeadIcon.transform.position.y, 0.1).tweenType = LeanTweenType.easeInOutSine
  LeanTween.moveY(t.btnMail.gameObject, t.imgAccountHeadIcon.transform.position.y, 0.2).tweenType = LeanTweenType.easeInOutSine
  LeanTween.moveY(t.btnFriend.gameObject, t.imgAccountHeadIcon.transform.position.y, 0.3).tweenType = LeanTweenType.easeInOutSine
  LeanTween.moveY(t.btnRank.gameObject, t.imgAccountHeadIcon.transform.position.y, 0.4).tweenType = LeanTweenType.easeInOutSine
  LeanTween.moveY(t.btn_player_manual.gameObject, t.imgAccountHeadIcon.transform.position.y, 0.5).tweenType = LeanTweenType.easeInOutSine
  LeanTween.moveY(t.btnSignIn.gameObject, t.imgAccountHeadIcon.transform.position.y, 0.6).tweenType = LeanTweenType.easeInOutSine
end

function t.UnfoldTopLeftFunctionButtons ()
  LeanTween.moveLocalY(t.btnTask.gameObject, t.btnTaskInitialLocalPosition.y, 0.1):setDelay(0.5).tweenType = LeanTweenType.easeInOutSine
  LeanTween.moveLocalY(t.btnMail.gameObject, t.btnMailInitialLocalPosition.y, 0.2):setDelay(0.4).tweenType = LeanTweenType.easeInOutSine
  LeanTween.moveLocalY(t.btnFriend.gameObject, t.btnFriendInitialLocalPosition.y, 0.3):setDelay(0.3).tweenType = LeanTweenType.easeInOutSine
  LeanTween.moveLocalY(t.btnRank.gameObject, t.btnRankInitialLocalPosition.y, 0.4):setDelay(0.2).tweenType = LeanTweenType.easeInOutSine
  LeanTween.moveLocalY(t.btn_player_manual.gameObject, t.btn_player_manual_InitialLocalPosition.y, 0.5):setDelay(0.1).tweenType = LeanTweenType.easeInOutSine
  LeanTween.moveLocalY(t.btnSignIn.gameObject, t.btnSignInInitialLocalPosition.y, 0.6):setDelay(0).tweenType = LeanTweenType.easeInOutSine
end

function t.ClickFoldUnfoldFunctionsButton ()
  if t.isLeftFunctionButtonsFold then
    t.UnfoldTopLeftFunctionButtons()
    t.foldUnfoldFunctionsButton:GetComponent(typeof(Image)).sprite = t.buttonFoldSprite
  else
    t.FoldTopLeftFunctionButtons()
    t.foldUnfoldFunctionsButton:GetComponent(typeof(Image)).sprite = t.buttonUnfoldSprite
  end
  t.isLeftFunctionButtonsFold = not t.isLeftFunctionButtonsFold
end
return t
