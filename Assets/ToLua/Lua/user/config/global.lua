-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'name','value','des',
}

t.t = {
	['largeIconWidth']={'largeIconWidth','136','?',nil},
	['largeIconHeight']={'largeIconHeight','132','?',nil},
	['midiumIconWidth']={'midiumIconWidth','88','?',nil},
	['midiumIconHeight']={'midiumIconHeight','85','?',nil},
	['player_lv_max']={'player_lv_max','60','职业英雄等级上限',nil},
	['account_lv_max']={'account_lv_max','60','职业等级（账号等级）上限',nil},
	['equip_package_buy_a']={'equip_package_buy_a','30','装备格子购买系数a',nil},
	['equip_package_buy_b']={'equip_package_buy_b','0','装备格子购买系数b',nil},
	['equip_package_buy_num']={'equip_package_buy_num','5','装备格子每次购买数量',nil},
	['equip_package_max_num']={'equip_package_max_num','300','装备格子上限（数量）',nil},
	['hero_package_buy_a']={'hero_package_buy_a','20','英雄格子购买系数a',nil},
	['hero_package_buy_b']={'hero_package_buy_b','0','英雄格子购买系数b',nil},
	['hero_package_buy_num']={'hero_package_buy_num','10','英雄格子每次购买数量',nil},
	['hero_package_max_num']={'hero_package_max_num','300','英雄格子上限（数量）',nil},
	['dailyRefreshTime']={'dailyRefreshTime','0','每日刷新时间点',nil},
	['physic_add']={'physic_add','360','体力恢复的时间间隔（秒）',nil},
	['physic_max']={'physic_max','999','体力上限',nil},
	['dungeon_id_skip']={'dungeon_id_skip','10000','不同难度副本ID差值',nil},
	['first_profession']={'first_profession','200','初始可以选择的职业ID',nil},
	['challengeTimesRecoverTime']={'challengeTimesRecoverTime','17280','竞技场挑战次数回复时间（s）',nil},
	['challengeSuccCoolingTime']={'challengeSuccCoolingTime','1','竞技场挑战成功冷却时间（s）',nil},
	['challengeFailCoolingTime']={'challengeFailCoolingTime','1','竞技场挑战失败冷却时间（s）',nil},
	['refreshListingCoolingTime']={'refreshListingCoolingTime','180','竞技场刷新挑战列表冷却时间（s）',nil},
	['reportNum']={'reportNum','10','竞技场保存战报个数',nil},
	['challengeListNum']={'challengeListNum','20','排行榜一页显示个数',nil},
	['challengeSuccPrize']={'challengeSuccPrize','25','竞技场胜利奖励荣誉点数、',nil},
	['challengeFailPrize']={'challengeFailPrize','5','竞技场失败奖励荣誉点数、',nil},
	['arena_winning_streak_num']={'arena_winning_streak_num','5','竞技场累计胜利次数',nil},
	['arena_winning_streak_award']={'arena_winning_streak_award','14:0:100:0;8:0:1000:0','竞技场累计胜利奖励',nil},
	['payforCard']={'payforCard','9:0:30:0;9:0:100:0;9:0:300:0','钻石翻牌消耗',nil},
	['basic']={'basic','10','计算战力的一个基础战斗力常量',nil},
	['faraway_comat_const_M']={'faraway_comat_const_M','3','远征匹配不到战力区间的时候，最小值百分比和最大值百分比每次修正的系数。min=min-m',nil},
	['faraway_comat_const_N']={'faraway_comat_const_N','100','远征匹配不到战力区间的时候，修正系数的最大值。当M>=N的时候，为匹配不到。此时匹配自己',nil},
	['loading_tips']={'loading_tips','tips1;tips2;tips3;tips4;tips5;tips6;tips7;tips8;tips9;tips10','载入随机',nil},
	['faraway_times']={'faraway_times','1','每日远征重置次数',nil},
	['tree_pic']={'tree_pic','tree01;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02;tree02','世界树图片顺序',nil},
	['tree_dungeon_pos']={'tree_dungeon_pos','(0:45.714);(0:137.143);(0:228.571);(0:320);(0:411.428);(0:502.856);(0:594.28436)','世界树上点的位置',nil},
	['max_tree']={'max_tree','50200','世界树最大层数',nil},
	['tree_attack_times']={'tree_attack_times','5','恢复上限',nil},
	['tree_max_key']={'tree_max_key','99','最大存储数量',nil},
	['tree_key_refresh_time']={'tree_key_refresh_time','3600','世界树回复间隔（秒）',nil},
	['tree_buy_a']={'tree_buy_a','20','世界树购买重置的价格系数a  单位 钻石     price=a*n+b n为次数',nil},
	['tree_buy_b']={'tree_buy_b','10','世界树购买重置的价格系数b  单位 钻石    price=a*n+b   n为次数',nil},
	['tree_buff']={'tree_buff','0.02','世界树每次失败，对面小怪叠加的负面buff',nil},
	['tree_buff_max']={'tree_buff_max','0.2','世界树每次失败，对面小怪叠加的负面buff最大值',nil},
	['sweep_key']={'sweep_key','4:10500:1:0','扫荡消耗的扫荡卷ID',nil},
	['mail_delete']={'mail_delete','2592000','自动删除邮件的时间（秒）',nil},
	['inspire_cost']={'inspire_cost','9:0:10:0','每次鼓舞消耗',nil},
	['inspire_effect']={'inspire_effect','20','鼓舞效果，攻击增加(20%,40%……)',nil},
	['inspire_max']={'inspire_max','10','最大鼓舞次数',nil},
	['world_boss_id']={'world_boss_id','52','世界bossid',nil},
	['boss_fight_fail']={'boss_fight_fail','1800','世界boss挑战失败时间（秒）',nil},
	['boss_fight_win']={'boss_fight_win','1020','世界boss挑战成功时间（秒）',nil},
	['boss_fight_reborn_cost']={'boss_fight_reborn_cost','9:0:10:0','世界boss复活价格',nil},
	['boss_fight_reborn_time']={'boss_fight_reborn_time','120','死亡等待时间（秒）',nil},
	['boss_time1_fight_began']={'boss_time1_fight_began','1899/12/31 12:00:00','世界boss1战斗开始',nil},
	['boss_time2_fight_began']={'boss_time2_fight_began','1899/12/31 21:00:00','世界boss2战斗开始',nil},
	['boss_fight_last_time']={'boss_fight_last_time','1800','战斗持续时间（秒）',nil},
	['boss_in_before']={'boss_in_before','1800','提前开始时间（秒）',nil},
	['boss_in_last']={'boss_in_last','1800','结束持续时间（秒）',nil},
	['boss_hp_const']={'boss_hp_const','1.6','世界boss每次血量变化的常量',nil},
	['boss_hp_min_const']={'boss_hp_min_const','0.05','世界boss血量最小系数。HP>=hp(dayHp)*boss_hp_min_const',nil},
	['angry_value_everytime']={'angry_value_everytime','5','每次受击增加怒气值',nil},
	['whisper_message_count']={'whisper_message_count','20','私聊需要保存的消息数量',nil},
	['chat_cd']={'chat_cd','10','聊天间隔（秒）',nil},
	['gold_drop_probability']={'gold_drop_probability','0.3','金币掉落概率',nil},
	['exp_solution']={'exp_solution','30000;3000;300','经验药水获得经验（从大到小）',nil},
	['friend_box_count']={'friend_box_count','5','领取好友的行动力N次获得1个礼包',nil},
	['friends_max']={'friends_max','30','最大好友数量',nil},
	['friend_pveaction_add']={'friend_pveaction_add','2','每次赠送的体力数量',nil},
	['friend_pveaction_add_max']={'friend_pveaction_add_max','20','每天最大好友体力领取数量',nil},
	['friend_box_id']={'friend_box_id','4:10134:1:0','好友礼包id',nil},
	['vip_max_lv']={'vip_max_lv','15','vip最大等级数量',nil},
	['friend_range']={'friend_range','15','随机出的好友战力上下区间百分比，向上向下各15%',nil},
	['friend_recommend_num']={'friend_recommend_num','10','每次随机出的好友人数',nil},
	['equip_strength_lv_notice']={'equip_strength_lv_notice','5','装备强化提示（此等级以上会提示）',nil},
	['for_coin_refresh_max']={'for_coin_refresh_max','10','阵型点自动恢复上限',nil},
	['for_coin_max']={'for_coin_max','100','阵型点购买的存储上限',nil},
	['for_coin_refresh_time']={'for_coin_refresh_time','1200','阵型点回复间隔（秒）',nil},
	['for_coin_buy_a']={'for_coin_buy_a','10','阵型点购买价格系数a  单位 钻石     price=a*n+b n为次数',nil},
	['for_coin_buy_b']={'for_coin_buy_b','10','阵型点购买价格系数b  单位 钻石    price=a*n+b   n为次数',nil},
	['for_coin_buy_num']={'for_coin_buy_num','3','阵型点购买，每次购买的数量',nil},
	['daily_dungeon_buy']={'daily_dungeon_buy','9:0:30:0;9:0:60:0;9:0:100:0;9:0:200:0;9:0:300:0','每日副本购买次数的价格。四段式，分号分开。超过次数取最后一次',nil},
	['faraway_bug']={'faraway_bug','9:0:180:0','远征购买次数的价格。四段式，分号分开。超过次数取最后一次',nil},
	['daily_dungeon_show_pic']={'daily_dungeon_show_pic','4:10015:1:0;4:10025:1:0;4:10066:1:0;4:10072:1:0','每日副本说明显示图片',nil},
	['rename_cost']={'rename_cost','9:0:50:0','改名钻石消费',nil},
	['shop_npc_head']={'shop_npc_head','1;2;3;4;5;6','商店NPC头像',nil},
	['task_npc_head']={'task_npc_head','11;12;13;14;15;16','任务NPC头像',nil},
	['newbie_npc_head']={'newbie_npc_head','21;22;23;24;25;26','新手NPC头像',nil},
	['max_scroll_lv']={'max_scroll_lv','4','最大符文卷轴等级',nil},
	['max_gen_lv']={'max_gen_lv','8','最大宝石等级',nil},
	['gen_unlock_item1']={'gen_unlock_item1','9:0:50:0','解锁第一个宝石孔道具',nil},
	['gen_unlock_item2']={'gen_unlock_item2','9:0:80:0','解锁第二个宝石孔道具',nil},
	['scroll_readom']={'scroll_readom','20','符文镶嵌浮动值',nil},
	['gen_star1']={'gen_star1','40001;1;4','升星宝石对应概率',nil},
	['gen_star2']={'gen_star2','40002;4;8','升星宝石对应概率',nil},
	['gen_star3']={'gen_star3','40003;8;12','升星宝石对应概率',nil},
	['gen_star4']={'gen_star4','40004;12;15','升星宝石对应概率',nil},
	['gen_back_star']={'gen_back_star','9:0:0:0','退星索要的钻石',nil},
	['explore_refresh']={'explore_refresh','9:0:80:0','委派刷新任务价格',nil},
	['explore_number']={'explore_number','8','委派任务数量',nil},
	['comat_const']={'comat_const','75','战力判定系数',nil},
	['comat_PVP_min']={'comat_PVP_min','0.28','战力PVP判定系数。详情见策划相关/防外挂验证',nil},
	['comat_PVP_max']={'comat_PVP_max','0.8','战力PVP判定系数。详情见策划相关/防外挂验证',nil},
	['all_rank_number']={'all_rank_number','30','总排行榜显示多少名',nil},
	['equipment_star_max']={'equipment_star_max','6','装备最大升星等级',nil},
	['guild_creat_lv']={'guild_creat_lv','15','创建公会等级',nil},
	['guild_creat_cost']={'guild_creat_cost','9:0:800:0','创建公会钻石',nil},
	['guild_mark']={'guild_mark','icon_g_01;icon_g_02;icon_g_03;icon_g_04;icon_g_05;icon_g_06;icon_g_07;icon_g_08;icon_g_09;icon_g_10','公会创建选择标志',nil},
	['guild_people_number']={'guild_people_number','30','公会基本人数',nil},
	['guild_people_number_up']={'guild_people_number_up','5','公会升级后人数增加',nil},
	['application_list_number']={'application_list_number','30','申请列表人数',nil},
	['guild_message_max']={'guild_message_max','30','公会最大日志数量',nil},
	['guild_fire_people']={'guild_fire_people','5','每日开除人数',nil},
	['guild_join_cd']={'guild_join_cd','86400','退出公会后再次加入时间',nil},
	['guild_application_join_cd']={'guild_application_join_cd','3600','再次请求加入公会CD',nil},
	['guild_donate_max']={'guild_donate_max','5','每日可捐献次数',nil},
	['guild_request_information']={'guild_request_information','5','玩家最多请求加入公会的数量',nil},
	['daily_key_time_keep']={'daily_key_time_keep','3600','每次体力活动持续秒',nil},
	['guild_shop_refresh']={'guild_shop_refresh','8;15;21','商店刷新时间',nil},
	['guild_pvp_time1']={'guild_pvp_time1','1,0','PVP开始报名时间，周一 0点',nil},
	['guild_pvp_time2']={'guild_pvp_time2','2,20;5,20','PVP开始匹配时间多个分号隔开',nil},
	['guild_pvp_time3']={'guild_pvp_time3','3,20;6,20','PVP开始战斗时间多个分号隔开',nil},
	['guild_pvp_integral']={'guild_pvp_integral','1,3;2,3;3,3;4,3;5,3;6,3;7,3;8,3;9,3;','每一路对应的积分',nil},
	['point_pvp_open_time']={'point_pvp_open_time','600','PVP开始前提示时间',nil},
	['point_pvp_keep_time']={'point_pvp_keep_time','1800','每次PVP持续时间',nil},
	['point_pvp_start_week']={'point_pvp_start_week','1;2;3;4;5;6;0','积分赛每周开启日期0是星期天',nil},
	['point_pvp_start_time']={'point_pvp_start_time','1899/12/31 20:00:00','积分赛每日开启时间',nil},
	['point_pvp_daily_time']={'point_pvp_daily_time','5','积分赛每日可匹配次数',nil},
	['point_pvp_win_point']={'point_pvp_win_point','120','积分赛胜利奖励积分',nil},
	['point_pvp_lose_point']={'point_pvp_lose_point','60','积分赛失败奖励积分',nil},
	['point_pvp_keep_win_point']={'point_pvp_keep_win_point','10','积分赛连胜获取积分',nil},
	['buy_sweep_cost']={'buy_sweep_cost','4:10500:1:0;9:0:1:0','购买扫荡卷价格 ID,钻石价格',nil},
	['map_arena']={'map_arena','map_103',nil,nil},
	['map_expedition']={'map_expedition','map_101',nil,nil},
	['map_worldBoss']={'map_worldBoss','map_104',nil,nil},
	['map_skillDisplay']={'map_skillDisplay','map_101',nil,nil},
	['map_pvp']={'map_pvp','map_101',nil,nil},
	['map_consortiaFight']={'map_consortiaFight','map_101',nil,nil},
	['event_sort']={'event_sort','30;10;70;140;50;40;80;100;60;20','活动类型排序',nil},
	['bbs_addres']={'bbs_addres','https://sojump.com/jq/9315783.aspx','问答跳转网址',nil},
	['boss_list_first']={'boss_list_first','60001','boss列表的第一个副本id',nil},
	['aggr_inherit_per']={'aggr_inherit_per','0.8','强化继承的经验',nil},
	['daily_dungeon_award_vip']={'daily_dungeon_award_vip','0;5','每日副本额外获得奖励VIP等级',nil},
	['daily_dungeon_award_rate']={'daily_dungeon_award_rate','2;3','每日副本额外获得奖励倍数',nil},
	['daily_dungeon_award_cost']={'daily_dungeon_award_cost','9:0:100:0;9:0:300:0','每日副本额外获得奖励所需价格',nil},
	['plunder_random_min']={'plunder_random_min','10','矿战最小掠夺资源百分比',nil},
	['plunder_random_max']={'plunder_random_max','30','矿战最大掠夺资源百分比',nil},
	['star1Attr']={'star1Attr','1','1星英雄属性系数',nil},
	['star2Attr']={'star2Attr','1.15','2星英雄属性系数',nil},
	['star3Attr']={'star3Attr','1.3','3星英雄属性系数',nil},
	['star4Attr']={'star4Attr','1.45','4星英雄属性系数',nil},
	['star5Attr']={'star5Attr','1.6','5星英雄属性系数',nil},
	['star6Attr']={'star6Attr','1.75','6星英雄属性系数',nil},
	['main_hand']={'main_hand','10','指关卡手指新手阶段用',nil},
	['blackmarket_double']={'blackmarket_double','1;1','综合黑市购买物品倍率',nil},
	['card_random_award']={'card_random_award','50;5','钻石抽卡累计次数；prizeID',nil},
	['handgold_crit']={'handgold_crit','2,5000;5,1000','点金手暴击概率（总概率10000）',nil},
	['hero_decomposition_cost']={'hero_decomposition_cost','50','英雄分解保留道具花费',nil},
	['Dungeon_Atk_CombatAdd']={'Dungeon_Atk_CombatAdd','2E-05','副本战力伤害加成系数',nil},
	['equip_decom_gold_sale']={'equip_decom_gold_sale','0.7','分解装备返还金币折损',nil},
	['open_gift_modulus']={'open_gift_modulus','20','等级礼包购买人数随机最大值',nil},
	['commGift']={'commGift','HJQST2017-4:9999:1:0','论坛礼包',nil},
	['black_market_sort']={'black_market_sort','5;1;2;3;4;6;7','黑市商店页签排序',nil}
}

function t.ForEach(func)
	if not func then return end
	local ky = nil
	local v = nil
	for i,j in pairs(t.t) do
		local r={}
		ky=tonumber(i)
		for k=1,#indexs do
			v=indexs[k]
			if v and v ~= '' then
				r[v]=j[k]
			end
		end

		if ky then func(ky,r) 
		else func(i,r) end
	end
end

function t.GetItem(id)
	id=tostring(id)
    local item=t[id]
	if item then return item end
	item=t.t[id]
	local result = {}
	local v = nil
	for i=1,#indexs do
		v=indexs[i]
		if v and v ~= '' then
			result[v]=item[i]
		end
	end
	t[id]=result
	return result
end

return t
