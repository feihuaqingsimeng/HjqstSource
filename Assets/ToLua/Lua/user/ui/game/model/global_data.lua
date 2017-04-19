
local t = {}
local game_res_data = require('ui/game/model/game_res_data')

local function Start()
	local origin = dofile('global')
	t.largeIconWidth = origin.t['largeIconWidth'][2] 
	t.largeIconHeight = origin.t['largeIconHeight'][2] 
	t.midiumIconWidth = origin.t['midiumIconWidth'][2] 
	t.midiumIconHeight = origin.t['midiumIconHeight'][2] 
	t.player_lv_max = tonumber(origin.t['player_lv_max'][2]) 
	t.account_lv_max = tonumber(origin.t['account_lv_max'][2]) 
	t.equip_package_buy_a = tonumber(origin.t['equip_package_buy_a'][2] )
	t.equip_package_buy_b = tonumber(origin.t['equip_package_buy_b'][2] )
	t.equip_package_buy_num = tonumber(origin.t['equip_package_buy_num'][2] )
	t.equip_package_max_num = tonumber(origin.t['equip_package_max_num'][2] )
	t.hero_package_buy_a = tonumber(origin.t['hero_package_buy_a'][2] )
	t.hero_package_buy_b = tonumber(origin.t['hero_package_buy_b'][2] )
	t.hero_package_buy_num = tonumber(origin.t['hero_package_buy_num'][2] )
	t.hero_package_max_num = tonumber(origin.t['hero_package_max_num'][2] )
	t.dailyRefreshTime = origin.t['dailyRefreshTime'][2] 
	t.physic_add = origin.t['physic_add'][2] 
	t.physic_max = origin.t['physic_max'][2] 
	t.dungeon_id_skip = origin.t['dungeon_id_skip'][2] 
	t.first_profession = origin.t['first_profession'][2] 
	t.challengeTimesRecoverTime = origin.t['challengeTimesRecoverTime'][2] 
	t.challengeSuccCoolingTime = origin.t['challengeSuccCoolingTime'][2] 
	t.challengeFailCoolingTime = origin.t['challengeFailCoolingTime'][2] 
	t.refreshListingCoolingTime = origin.t['refreshListingCoolingTime'][2] 
	t.reportNum = origin.t['reportNum'][2] 
	t.challengeListNum = origin.t['challengeListNum'][2] 
	t.challengeSuccPrize = origin.t['challengeSuccPrize'][2] 
	t.challengeFailPrize = origin.t['challengeFailPrize'][2]
  t.arena_winning_streak_num = tonumber(origin.t['arena_winning_streak_num'][2])
  t.arena_winning_streak_award = game_res_data.ParseGameResDataList(origin.t['arena_winning_streak_award'][2])
	t.payforCard = origin.t['payforCard'][2] 
	t.powerBasic = tonumber(origin.t['basic'][2]) 
	t.faraway_comat_const_M = origin.t['faraway_comat_const_M'][2] 
	t.faraway_comat_const_N = origin.t['faraway_comat_const_N'][2] 
	t.loading_tips = origin.t['loading_tips'][2] 
	t.faraway_times = origin.t['faraway_times'][2] 
	t.tree_pic = origin.t['tree_pic'][2] 
	t.tree_dungeon_pos = origin.t['tree_dungeon_pos'][2] 
	t.max_tree = origin.t['max_tree'][2] 
	t.tree_attack_times = origin.t['tree_attack_times'][2] 
	t.tree_max_key = origin.t['tree_max_key'][2] 
	t.tree_key_refresh_time = origin.t['tree_key_refresh_time'][2] 
	t.tree_buy_a = origin.t['tree_buy_a'][2] 
	t.tree_buy_b = origin.t['tree_buy_b'][2] 
	t.tree_buff = origin.t['tree_buff'][2] 
	t.tree_buff_max = origin.t['tree_buff_max'][2] 
	t.sweep_key = origin.t['sweep_key'][2] 
	t.mail_delete = origin.t['mail_delete'][2] 
	t.inspire_cost = origin.t['inspire_cost'][2] 
	t.inspire_effect = origin.t['inspire_effect'][2] 
	t.inspire_max = origin.t['inspire_max'][2] 
	t.world_boss_id = origin.t['world_boss_id'][2] 
	t.boss_fight_fail = origin.t['boss_fight_fail'][2] 
	t.boss_fight_win = origin.t['boss_fight_win'][2] 
	t.boss_fight_reborn_cost = origin.t['boss_fight_reborn_cost'][2] 
	t.boss_fight_reborn_time = origin.t['boss_fight_reborn_time'][2] 
	t.boss_time1_fight_began = origin.t['boss_time1_fight_began'][2] 
	t.boss_time2_fight_began = origin.t['boss_time2_fight_began'][2] 
	t.boss_fight_last_time = tonumber(origin.t['boss_fight_last_time'][2] )
	t.boss_in_before = tonumber(origin.t['boss_in_before'][2] )
	t.boss_in_last = tonumber(origin.t['boss_in_last'][2] )
	t.boss_hp_const = origin.t['boss_hp_const'][2] 
	t.angry_value_everytime = origin.t['angry_value_everytime'][2] 
	t.whisper_message_count = origin.t['whisper_message_count'][2] 
	t.chat_cd = origin.t['chat_cd'][2] 
	t.gold_drop_probability = origin.t['gold_drop_probability'][2] 
	
  t.expSolutions = {}
  local expSolution = string.split(origin.t['exp_solution'][2] ,';')
  for k,v in pairs(expSolution) do
    t.expSolutions[k] = tonumber(v)
  end
  
	t.friend_box_count = origin.t['friend_box_count'][2] 
	t.friends_max = origin.t['friends_max'][2] 
	t.friend_pveaction_add = origin.t['friend_pveaction_add'][2] 
	t.friend_pveaction_add_max = origin.t['friend_pveaction_add_max'][2] 
	t.friend_box_id = origin.t['friend_box_id'][2] 
	t.vip_max_lv = origin.t['vip_max_lv'][2] 
	t.friend_range = origin.t['friend_range'][2] 
	t.friend_recommend_num = origin.t['friend_recommend_num'][2] 
	t.equip_strength_lv_notice = origin.t['equip_strength_lv_notice'][2] 
	t.for_coin_refresh_max = origin.t['for_coin_refresh_max'][2] 
	t.for_coin_max = tonumber(origin.t['for_coin_max'][2] )
	t.for_coin_refresh_time = tonumber(origin.t['for_coin_refresh_time'][2] )
	t.for_coin_buy_a = tonumber(origin.t['for_coin_buy_a'][2] )
	t.for_coin_buy_b = tonumber(origin.t['for_coin_buy_b'][2] )
	t.for_coin_buy_num = tonumber(origin.t['for_coin_buy_num'][2] )
	t.daily_dungeon_buy = origin.t['daily_dungeon_buy'][2] 
	t.faraway_bug = origin.t['faraway_bug'][2] 
	t.daily_dungeon_show_pic = origin.t['daily_dungeon_show_pic'][2] 
	t.rename_cost = origin.t['rename_cost'][2] 
	t.shop_npc_head = origin.t['shop_npc_head'][2] 
	t.task_npc_head = origin.t['task_npc_head'][2] 
	t.newbie_npc_head = origin.t['newbie_npc_head'][2] 
	t.max_scroll_lv = origin.t['max_scroll_lv'][2] 
	t.max_gen_lv = tonumber(origin.t['max_gen_lv'][2] )
  
  t.gen_unlock_item = {}
	t.gen_unlock_item[1] = origin.t['gen_unlock_item1'][2] 
	t.gen_unlock_item[2] = origin.t['gen_unlock_item2'][2]
  for k,v in pairs(t.gen_unlock_item) do
    if v == '0' then
      t.gen_unlock_item[k] = nil
    else
      t.gen_unlock_item[k]= game_res_data.NewByString(t.gen_unlock_item[k])
    end
  end
  
	t.scroll_readom = origin.t['scroll_readom'][2]
  t.gen_star = {}
  for i = 1 ,4 do
    t.gen_star[i] = {}
    local value = origin.t['gen_star'..i][2] 
    value = string.split(value,';')
    t.gen_star[i].item = game_res_data.New(BaseResType.Item, tonumber( value[1]),1,0)
    t.gen_star[i].min = tonumber(value[2])
    t.gen_star[i].max = tonumber(value[3])
  end
	t.gen_back_star = game_res_data.NewByString(origin.t['gen_back_star'][2] )
	t.explore_refresh = origin.t['explore_refresh'][2] 
	t.explore_number = origin.t['explore_number'][2] 
  t.comat_const = origin.t['comat_const'][2] 
  t.all_rank_number = origin.t['all_rank_number'][2] 
  t.equipment_star_max = tonumber(origin.t['equipment_star_max'][2] )
  t.guild_people_number = origin.t['guild_people_number'][2] 
	t.guild_people_number_up = origin.t['guild_people_number_up'][2] 
	t.application_list_number = origin.t['application_list_number'][2] 
  --公会
  t.guild_creat_lv = origin.t['guild_creat_lv'][2] 
	t.guild_creat_cost = game_res_data.NewByString(origin.t['guild_creat_cost'][2] )
	t.guild_mark = string.split(origin.t['guild_mark'][2] ,';')
	t.guild_people_number = origin.t['guild_people_number'][2] 
	t.guild_people_number_up = origin.t['guild_people_number_up'][2] 
	t.application_list_number = origin.t['application_list_number'][2] 
	t.guild_message_max = origin.t['guild_message_max'][2] 
	t.guild_fire_people = origin.t['guild_fire_people'][2] 
	t.guild_join_cd = origin.t['guild_join_cd'][2] 
	t.guild_application_join_cd = origin.t['guild_application_join_cd'][2] 
	t.guild_donate_max = origin.t['guild_donate_max'][2] 
	t.guild_request_information = origin.t['guild_request_information'][2] 
  t.daily_key_time_keep = origin.t['daily_key_time_keep'][2] 
	t.guild_shop_refresh = origin.t['guild_shop_refresh'][2] 
	t.guild_pvp_time1 = origin.t['guild_pvp_time1'][2] 
	t.guild_pvp_time2 = origin.t['guild_pvp_time2'][2] 
	t.guild_pvp_time3 = origin.t['guild_pvp_time3'][2] 
	t.guild_pvp_integral = origin.t['guild_pvp_integral'][2]
  --pvp race
	t.point_pvp_open_time = tonumber(origin.t['point_pvp_open_time'][2])
	t.point_pvp_keep_time = tonumber(origin.t['point_pvp_keep_time'][2]) 
  t.point_pvp_start_week = string.split2number(origin.t['point_pvp_start_week'][2],';') 
  local time = string.gsub(origin.t['point_pvp_start_time'][2],'1899/12/31 ','')
  time = string.split2number(time ,':')
  t.point_pvp_start_time = System.DateTime.Today:AddHours(time[1]):AddMinutes(time[2])
	t.point_pvp_daily_time = tonumber(origin.t['point_pvp_daily_time'][2] )
	t.point_pvp_win_point = origin.t['point_pvp_win_point'][2] 
	t.point_pvp_lose_point = origin.t['point_pvp_lose_point'][2] 
	t.point_pvp_keep_win_point = origin.t['point_pvp_keep_win_point'][2] 
  
	t.buy_sweep_cost = origin.t['buy_sweep_cost'][2] 
	t.map_arena = origin.t['map_arena'][2] 
	t.map_expedition = origin.t['map_expedition'][2] 
	t.map_worldBoss = origin.t['map_worldBoss'][2] 
	t.map_skillDisplay = origin.t['map_skillDisplay'][2] 
	t.map_pvp = origin.t['map_pvp'][2] 
	t.map_consortiaFight = origin.t['map_consortiaFight'][2] 
	t.event_sort = origin.t['event_sort'][2] 
	t.bbs_addres = origin.t['bbs_addres'][2] 
  t.boss_list_first = origin.t['boss_list_first'][2] 
  t.aggr_inherit_per = tonumber(origin.t['aggr_inherit_per'][2] )
  t.daily_dungeon_award_vip = origin.t['daily_dungeon_award_vip'][2] 
	t.daily_dungeon_award_rate = origin.t['daily_dungeon_award_rate'][2] 
	t.daily_dungeon_award_cost = origin.t['daily_dungeon_award_cost'][2] 
	t.plunder_random_min = origin.t['plunder_random_min'][2] 
	t.plunder_random_max = origin.t['plunder_random_max'][2] 
  t.starAttr = {}
	t.starAttr[1] = tonumber(origin.t['star1Attr'][2] )
	t.starAttr[2] = tonumber(origin.t['star2Attr'][2] )
	t.starAttr[3] = tonumber(origin.t['star3Attr'][2] )
	t.starAttr[4] = tonumber(origin.t['star4Attr'][2] )
	t.starAttr[5] = tonumber(origin.t['star5Attr'][2] )
	t.starAttr[6] = tonumber(origin.t['star6Attr'][2] )
  t.startFightIndicatorShowMaxLevel = tonumber(origin.t['main_hand'][2])
  t.blackmarket_double = string.split2number(origin.t['blackmarket_double'][2],';')
  t.accumulatedExpMax = string.split2number(origin.t['card_random_award'][2], ';')[1]
  t.hero_decomposition_cost = tonumber(origin.t['hero_decomposition_cost'][2])
  
  local black_market_sort = origin.t['black_market_sort'][2]
  t.black_market_sort_list = string.split2number(black_market_sort,';')
end
--星级属性计算系数
function t.GetStarAttr(star)
  local attr = t.starAttr[star]
  if attr then
    return attr
  end
  return 0
end

Start()
return t