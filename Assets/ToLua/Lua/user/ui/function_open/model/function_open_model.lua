local t = {}
local name = 'function_open_model'

local funtion_open_data = gamemanager.GetData('function_open_data')
local auto_destroy_tip_view = require('ui/tips/view/auto_destroy_tip_view')
local dungeon_data = gamemanager.GetData('dungeon_data')

local function Start ()
  gamemanager.RegisterModel(name, t)
end

function t.IsFunctionOpen(functionOpenType,isShowTip)
 
  t.data = funtion_open_data.GetDataById(functionOpenType)
  if not t.data then
    return true
  end
  local isOpen = false
  t.subTip = {}--如果有多个条件采用此提示
  local isVipOk = t.IsVipOk()  
  local isDungeonOk = t.IsDungeonPassOk()
  local isAccountLevelOk = t.IsAccountLevelOk()
  if isVipOk and isDungeonOk and isAccountLevelOk then
    isOpen = true
  end 
 
  if not isOpen and isShowTip then
    local count = table.count(t.subTip)
    --auto_destroy_tip_view.Open(t.tip)
    if count == 0 then
      auto_destroy_tip_view.Open(t.tip)
    else
      local s = ''
      local name = LocalizationController.instance:Get(t.data.function_name)
      for k,v in pairs(t.subTip) do
        s = s .. v..'/'
      end
      s = string.sub(s,0,-2) .. string.format(LocalizationController.instance:Get('ui.function_open.can_unlock'),name)
      auto_destroy_tip_view.Open(s)
    end
    
  end
  --print('IsFunctionOpen',functionOpenType,isOpen,t.data.player_level,t.data.dungeon_pass,t.data.vip)
  return isOpen
end

function t.IsAccountLevelOk()
  if t.data.player_level == 0 then
    return true
  end
  if t.data.player_level == 99 then --功能未开启
     t.tip = LocalizationController.instance:Get("ui.function_open_view.notOpen")
    return false
  end
  
  local flag = gamemanager.GetModel('game_model').accountLevel >= t.data.player_level
  if not flag then
    t.tip = string.format(LocalizationController.instance:Get("ui.function_open.not_enough_account_level"), t.data.player_level)
    t.subTip[1] = string.format(LocalizationController.instance:Get("ui.function_open.not_enough_account_level2"), t.data.player_level)
  end
  return flag
end

function t.IsDungeonPassOk()
  local dungeon_model = gamemanager.GetModel('dungeon_model')
  local isDungeonPassed = dungeon_model.IsDungeonPassed(t.data.dungeon_pass)
  local dungeonData = dungeon_data.GetDataById(t.data.dungeon_pass)
  if isDungeonPassed == false then
    t.tip = string.format(LocalizationController.instance:Get("ui.function_open.dungeon_not_pass"), dungeonData:GetDungeonTypeNamme(), dungeonData:GetOrderName(), dungeonData:GetDungeonName())
    t.subTip[2] = string.format(LocalizationController.instance:Get("ui.function_open.dungeon_not_pass2"), dungeonData:GetDungeonTypeNamme(), dungeonData:GetOrderName(), dungeonData:GetDungeonName())
  end
  return isDungeonPassed
end

function t.IsVipOk()
  local vip_model = gamemanager.GetModel('vip_model')
  local flag = vip_model.vipLevel >= t.data.vip
  if not flag then
    t.tip = string.format(LocalizationController.instance:Get("ui.function_open.vip_not_enough"), t.data.vip)
    t.subTip[3] = string.format(LocalizationController.instance:Get("ui.function_open.vip_not_enough2"), t.data.vip)
  end
  return flag
end

function t.OpenFunctionByCSharp(functionOpenType,...)
  local param = {...}
  local paramTable = {}
  local index = 1
  for k,v in pairs( param) do
    if v ~= nil then
      for i = 1,v.Length do
        paramTable[index] = v[i-1]
        index = index + 1
      end
    end
  end
  if not t.IsFunctionOpen(functionOpenType,true) then
    return 
  end
  t.DoOpenFunction (functionOpenType,paramTable)
end

function t.OpenFunction(functionOpenType,...)
  local paramTable = {...}
  if not t.IsFunctionOpen(functionOpenType,true) then
    return 
  end
  t.DoOpenFunction (functionOpenType,paramTable)
end

function t.DoOpenFunction (functionOpenType,paramTable)
  local ranking_controller = gamemanager.GetCtrl('ranking_controller')
  local equip_model = gamemanager.GetModel('equip_model')
  local hero_controller = gamemanager.GetCtrl('hero_controller')
  local shop_controller = gamemanager.GetCtrl('shop_controller')
  
  if functionOpenType == FunctionOpenType.MainView_Task then                -- 主界面 任务按钮
  elseif functionOpenType == FunctionOpenType.MainView_Mail then            -- 主界面 邮件按钮
  elseif functionOpenType == FunctionOpenType.MainView_Friend then          -- 主界面 好友按钮
  elseif functionOpenType == FunctionOpenType.MainView_SignIn then          -- 主界面 签到
elseif functionOpenType == FunctionOpenType.MainView_Activity then          -- 主界面 活动    
     gamemanager.GetCtrl('activity_controller').OpenActivity()
  elseif functionOpenType == FunctionOpenType.MainView_Ranking then         -- 主界面 排行榜
    ranking_controller.OpenRankingView(paramTable[1])
  
  elseif functionOpenType == FunctionOpenType.MainView_Player then          -- 主界面 主角
  elseif functionOpenType == FunctionOpenType.PlayerChoice then             -- 使魔选择
  elseif functionOpenType == FunctionOpenType.PlayerStrengthen then         -- 使魔强化
  elseif functionOpenType == FunctionOpenType.PlayerTraining then           -- 使魔培养
  elseif functionOpenType == FunctionOpenType.PlayerBreakthrough then       -- 使魔突破
  elseif functionOpenType == FunctionOpenType.PlayerWearEquip then          -- 使魔穿戴
    equip_model.OpenRoleEquipView(paramTable[1],paramTable[2],paramTable[3])
    
  elseif functionOpenType == FunctionOpenType.MainView_Hero then            -- 主界面 培养按钮
    hero_controller.OpenHeroInfoView()
    
  elseif functionOpenType == FunctionOpenType.HeroStrengthen then           -- 英雄强化
    hero_controller.OpenHeroStrengthenView(paramTable[1])
  elseif functionOpenType == FunctionOpenType.HeroAdvance then              -- 英雄升星
    hero_controller.OpenHeroAdvanceView(paramTable[1])
  elseif functionOpenType == FunctionOpenType.HeroBreakthrough then         -- 英雄突破
    hero_controller.OpenHeroBreakthroughView(paramTable[1])
  elseif functionOpenType == FunctionOpenType.MainView_FormationBtn then    -- 主界面 阵型按钮
    gamemanager.GetCtrl('pve_embattle_controller').OpenPveEmbattleView()
  elseif functionOpenType == FunctionOpenType.FormationTraining then        -- 更换阵型
  elseif functionOpenType == FunctionOpenType.HeroSecondTeam then           -- 英雄二队
  elseif functionOpenType == FunctionOpenType.HeroThirdTeam then            -- 英雄三队
  elseif functionOpenType == FunctionOpenType.MainView_Equipment then       -- 主界面 装备按钮
    gamemanager.GetModel('equip_model').OpenEquipBrowseView()
  
  elseif functionOpenType == FunctionOpenType.EquipTraining             -- 装备培养
  or functionOpenType == FunctionOpenType.EquipRecast               -- 装备重铸
  or functionOpenType == FunctionOpenType.EquipEnchant              -- 装备附魔
  or functionOpenType == FunctionOpenType.EquipUpStar               -- 装备升星
  or functionOpenType == FunctionOpenType.EquipGemInsert then           -- 装备镶嵌
    equip_model.OpenTrainingView(paramTable[1],paramTable[2])
  elseif functionOpenType == FunctionOpenType.MainView_Shop then            -- 主界面 商店
    shop_controller.OpenShopView(paramTable[1])
  elseif functionOpenType == FunctionOpenType.MainView_BlackMarket      -- 主界面 黑市
  or functionOpenType == FunctionOpenType.BlackMarket_Arena         -- 黑市 竞技场黑市
  or functionOpenType == FunctionOpenType.BlackMarket_Expedition    -- 黑市 远征黑市
  or functionOpenType == FunctionOpenType.BlackMarket_WorldBoss then    -- 黑市 世界boss黑市
    gamemanager.GetCtrl('black_market_controller').OpenBlackMarketView(paramTable[1])
  elseif functionOpenType == FunctionOpenType.MainView_illustrate then      -- 主界面 图鉴
    gamemanager.GetCtrl('illustration_ctrl').OpenIllustrationView()
  elseif functionOpenType == FunctionOpenType.MainView_illustrate_hero_detial_view then --  英雄图鉴详情界面
    gamemanager.GetCtrl('illustration_ctrl').OpenIllustrationHeroDetailView(paramTable[1])
  elseif functionOpenType == FunctionOpenType.MainView_Consortia then       -- 主界面 公会按钮
    gamemanager.GetCtrl('consortia_controller').OpenConsortiaView()
    
  elseif functionOpenType == FunctionOpenType.MainView_Explore then         -- 主界面 探险按钮
    CallExplore()
  
  elseif functionOpenType == FunctionOpenType.MainView_FightCenter then     -- 主界面 战斗中心按钮
  elseif functionOpenType == FunctionOpenType.FightCenter_Arena then        -- 战斗中心 竞技场
  elseif functionOpenType == FunctionOpenType.FightCenter_WorldTree then    -- 战斗中心 世界树
  elseif functionOpenType == FunctionOpenType.FightCenter_Expedition then   -- 战斗中心 远征
  elseif functionOpenType == FunctionOpenType.FightCenter_PvpRace then      -- 战斗中心 竞技场积分赛
      gamemanager.GetCtrl('arena_controller').OpenRaceIntroView()
  elseif functionOpenType == FunctionOpenType.FightCenter_PvpRaceMatch then --积分赛匹配界面
    local arena_model = gamemanager.GetModel('arena_model')
    if(arena_model.time) then
      if(arena_model.isOpen and TimeController.instance.ServerTimeTicksSecond < arena_model.time) then
        gamemanager.GetCtrl('arena_controller').OpenRaceView()
      end
    end
  elseif functionOpenType == FunctionOpenType.FightCenter_MineBattle then
    gamemanager.GetCtrl('mine_controller').OpenMineMapView()
  elseif functionOpenType == FunctionOpenType.MainView_compose then
     gamemanager.GetCtrl('compose_controller').OpenEquipCompose()
  elseif functionOpenType == FunctionOpenType.MainView_DailyDungeon then    -- 主界面 每日副本
  elseif functionOpenType == FunctionOpenType.MainView_PVE then             -- 主界面 pve副本
  elseif functionOpenType == FunctionOpenType.PVE_Normal then               -- pve副本 普通难度
  elseif functionOpenType == FunctionOpenType.PVE_Hard then                 -- pve副本 困难难度
  elseif functionOpenType == FunctionOpenType.SingleSweep then              -- pve副本 单次扫荡
  elseif functionOpenType == FunctionOpenType.TenSweep then                 -- pve副本 十次扫荡
  elseif functionOpenType == FunctionOpenType.DoubleSpeed then              -- pve副本 两倍速
  elseif functionOpenType == FunctionOpenType.AutoFight then                -- pve 自动战斗
  elseif functionOpenType == FunctionOpenType.MainView_WorldBoss then       -- 主界面世界boss
  elseif functionOpenType == FunctionOpenType.WorldBoss_inspireBtn then     -- 世界boss鼓舞按钮
  elseif functionOpenType == FunctionOpenType.MainView_Chat then            -- 主界面 聊天按钮
   OpenChat()
  elseif functionOpenType == FunctionOpenType.MainView_Pack then            -- 主界面 背包
    gamemanager.GetCtrl('pack_controller').OpenPackView()
  elseif functionOpenType == FunctionOpenType.Boss_List_View then           -- 副本选择界面 Boss列表按钮
    gamemanager.GetCtrl('chapter_controller').OpenBossDungeonListView()
  elseif functionOpenType == FunctionOpenType.GoldenTouchView then          --点金手
    gamemanager.GetCtrl('golden_touch_controller').OpenGoldenTouchView()
  end
end

Start()
return t