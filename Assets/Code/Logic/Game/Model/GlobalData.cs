using UnityEngine;
using System.Collections.Generic;
using Common.Util;

namespace Logic.Game.Model
{
    public class GlobalData
    {
        private static GlobalData _globalData;

        public static GlobalData GetGlobalData()
        {
            if (_globalData == null)
            {
                _globalData = CSVUtil.ParseClass<GlobalData>("config/csv/global");
            }
            return _globalData;
        }

        [CSVElement("largeIconWidth")]
        public int largeIconWidth;

        [CSVElement("largeIconHeight")]
        public int largeIconHeight;

        [CSVElement("midiumIconWidth")]
        public int midiumIconWidth;

        [CSVElement("midiumIconHeight")]
        public int midiumIconHeight;

        [CSVElement("equip_package_buy_a")]
        public int equipPackageBuyA;

        [CSVElement("equip_package_buy_b")]
        public int equipPackageBuyB;

        [CSVElement("hero_package_buy_a")]
        public int heroPackageBuyA;

        [CSVElement("hero_package_buy_b")]
        public int heroPackageBuyB;

        [CSVElement("hero_package_buy_num")]
        public int hero_package_buy_num;

        [CSVElement("equip_package_max_num")]
        public int equipPackageMaxNum;

        [CSVElement("hero_package_max_num")]
        public int heroPackageMaxNum;

        [CSVElement("player_lv_max")]
        public int playerLevelMax;

        [CSVElement("account_lv_max")]
        public int accountLevelMax;

        [CSVElement("physic_add")]
        public int pveActionRecoverTime;//体力恢复的时间间隔

        public List<int> initialPlayerDataIDList = new List<int>();
        [CSVElement("first_profession")]
        public string firstProfession
        {
            set
            {
                initialPlayerDataIDList = value.ToList<int>(CSVUtil.SYMBOL_SEMICOLON);
            }
        }

        [CSVElement("challengeTimesRecoverTime")]
        public int challengeTimesRecoverTime;//竞技场挑战次数回复时间
        [CSVElement("challengeListNum")]
        public int challengeListNum;//排行榜一页显示个数

        [CSVElement("challengeSuccPrize")]
        public int challengeSuccPrize;//竞技场胜利奖励荣誉点数、

        [CSVElement("challengeFailPrize")]
        public int challengeFailPrize;//竞技场失败奖励荣誉点数、


        public List<GameResData> payForCardList = new List<GameResData>();
        [CSVElement("payforCard")]
        public string payforCard//竞技场失败奖励荣誉点数、
        {
            set
            {
                payForCardList.Clear();
                string[] s = value.Split(CSVUtil.SYMBOL_SEMICOLON);
                for (int i = 0, count = s.Length; i < count; i++)
                {
                    payForCardList.Add(new GameResData(s[i]));
                }
            }
        }
        public List<string> loadingTipsIDs = new List<string>();
        [CSVElement("loading_tips")]
        public string loadTips
        {
            set
            {
                loadingTipsIDs = value.ToList(CSVUtil.SYMBOL_SEMICOLON);
            }
        }
        [CSVElement("basic")]
        public int powerBasic;//计算战力的一个基础战斗力常量

        public List<string> worldTreeBGPicNameList = new List<string>();
        [CSVElement("tree_pic")]
        public string treePic
        {
            set
            {
                worldTreeBGPicNameList = value.ToList(CSVUtil.SYMBOL_SEMICOLON);
            }
        }

        public List<Vector2> worldTreeDungeonPosList = new List<Vector2>();
        [CSVElement("tree_dungeon_pos")]
        public string treeDungeonPos
        {
            set
            {
                worldTreeDungeonPosList.Clear();
                List<string> worldTreeDungeonPosStringList = value.ToList(CSVUtil.SYMBOL_SEMICOLON);
                int worldTreeDungeonPosStringCount = worldTreeDungeonPosStringList.Count;
                Vector2 worldTreeDungeonPos;
                for (int i = 0; i < worldTreeDungeonPosStringCount; i++)
                {
                    worldTreeDungeonPos = worldTreeDungeonPosStringList[i].Replace("(", "").Replace(")", "").ToVector2(':');
                    worldTreeDungeonPosList.Add(worldTreeDungeonPos);
                }
            }
        }

        [CSVElement("max_tree")]
        public int worldTreeMaxFloor;

        [CSVElement("tree_attack_times")]
        public int worldTreeFruitRecoverLimit;

        [CSVElement("tree_max_key")]
        public int worldTreeFruitMax;

        [CSVElement("tree_key_refresh_time")]
        public int worldTreeFruitRecoverTime;

        [CSVElement("tree_buy_a")]
        public int worldTreeBuyFruitPriceParamA;

        [CSVElement("tree_buy_b")]
        public int worldTreeBuyFruitPriceParamB;

        [CSVElement("tree_buff")]
        public float worldTreeChallengeFailedWeakenBuff;

        [CSVElement("tree_buff_max")]
        public float worldTreeChallengeFailedWeakenBuffMax;

        public GameResData sweepTicket;
        [CSVElement("sweep_key")]
        public string sweep_key
        {
            set
            {
                sweepTicket = new GameResData(value);
            }
        }
		public GameResData buy_sweep_cost;
		[CSVElement("buy_sweep_cost")]
		public string buy_sweep_costStr
		{
			set{
				string[] s = value.ToArray(CSVUtil.SYMBOL_SEMICOLON);
				buy_sweep_cost = new GameResData( s[1]);
			}
		}

        public GameResData inspireCost;
        [CSVElement("inspire_cost")]
        public string inspireCostString
        {
            set
            {
                inspireCost = new GameResData(value);
            }
        }

        [CSVElement("inspire_effect")]
        public int inspireEffect;

        [CSVElement("inspire_max")]
        public int inspireMax;

        [CSVElement("world_boss_id")]
        public int worldBossID;

        [CSVElement("boss_fight_fail")]
        public long bossFightFail;

        [CSVElement("boss_fight_win")]
        public long bossFightWin;

        public GameResData bossFightRebornCost;
        [CSVElement("boss_fight_reborn_cost")]
        public string bossFightRebornCostStr
        {
            set
            {
                bossFightRebornCost = new GameResData(value);
            }
        }

        [CSVElement("boss_fight_reborn_time")]
        public long bossFightRebornTime;
        //boss1开启时间
        public System.DateTime boss_time1_fight_began;
        [CSVElement("boss_time1_fight_began")]
        public string boss_time1_fight_beganStr
        {
            set
            {
                int[] v = value.ToArray<int>(CSVUtil.SYMBOL_COLON);
                if (v.Length == 2)
                {
                    boss_time1_fight_began = System.DateTime.Today.AddHours(v[0]).AddMinutes(v[1]);
                }
            }
        }
        //boss2开启时间
        public System.DateTime boss_time2_fight_began;
        [CSVElement("boss_time2_fight_began")]
        public string boss_time2_fight_beganStr
        {
            set
            {
                int[] v = value.ToArray<int>(CSVUtil.SYMBOL_COLON);
                if (v.Length == 2)
                {
                    boss_time2_fight_began = System.DateTime.Today.AddHours(v[0]).AddMinutes(v[1]);
                }
            }
        }
        //boss持续时间
        [CSVElement("boss_fight_last_time")]
        public long bossFightLastTime;

        [CSVElement("boss_in_before")]
        public long bossInBefore;

        [CSVElement("boss_in_last")]
        public long bossInLast;

        [CSVElement("angry_value_everytime")]
        public uint angryValueEverytime;

        [CSVElement("chat_cd")]
        public int chat_cd;

        [CSVElement("gold_drop_probability")]
        public float gold_drop_probability;

        public int[] expSolutions;
        [CSVElement("exp_solution")]
        public string exp_solutionString   //经验药水获得经验（从大到小）
        {
            set
            {
                string[] s = value.Split(CSVUtil.SYMBOL_SEMICOLON);
                int count = s.Length;
                expSolutions = new int[count];
                for (int i = 0; i < count; i++)
                {
                    expSolutions[i] = s[i].ToInt32();
                }
            }
        }
        [CSVElement("friend_box_id")]
        public string friend_box_id;//好友礼包id

        [CSVElement("friend_box_count")]
        public int friendGiftNeedTimes;//领取好友的行动力N次获得1个礼包

        [CSVElement("friend_pveaction_add")]
        public int friendDonatePveActionPer;//每次赠送的体力数量

        [CSVElement("friend_pveaction_add_max")]
        public int friend_pveaction_add_max;//每天最大好友体力领取数量

        [CSVElement("equip_strength_lv_notice")]
        public int equipStrengthenLevelNotice;

        [CSVElement("for_coin_refresh_max")]
        public int formationTrainPointRecoverMax;//阵型点自动恢复上限

        [CSVElement("for_coin_max")]
        public int formationTrainPointStoreMax;//阵型点购买的存储上限

        [CSVElement("for_coin_buy_a")]
        public int formationTrainPointBuyA;//阵型点购买价格系数a  单位 钻石     price=a*n+b n为次数

        [CSVElement("for_coin_buy_b")]
        public int formationTrainPointBuyB;//阵型点购买价格系数b  单位 钻石    price=a*n+b   n为次数

        [CSVElement("for_coin_buy_num")]
        public int formationTrainPointBuyNum;//阵型点购买，每次购买的数量

        public List<GameResData> daily_dungeon_buy_List = new List<GameResData>();
        [CSVElement("daily_dungeon_buy")]
        public string daily_dungeon_buy//阵型点购买，每次购买的数量
        {
            set
            {
                daily_dungeon_buy_List.Clear();
                string[] buyS = value.Split(CSVUtil.SYMBOL_SEMICOLON);
                for (int i = 0, count = buyS.Length; i < count; i++)
                {
                    daily_dungeon_buy_List.Add(new GameResData(buyS[i]));
                }
            }
        }
        public GameResData faraway_buy_ResData;
        [CSVElement("faraway_bug")]
        public string faraway_buy//阵型点购买，每次购买的数量
        {
            set
            {
                faraway_buy_ResData = new GameResData(value);
            }
        }
        public GameResData renameCostResData;
        [CSVElement("rename_cost")]
        public string rename_cost//改名钻石消费
        {
            set
            {
                renameCostResData = new GameResData(value);
            }
        }
        
        [CSVElement("point_pvp_open_time")]
        public int point_pvp_open_time;

        [CSVElement("point_pvp_keep_time")]
        public int point_pvp_keep_time;


		public System.DateTime point_pvp_start_time;
        [CSVElement("point_pvp_start_time")]
        public string point_pvp_start_timeStr
        {
            set
            {
                int[] v = value.ToArray<int>(CSVUtil.SYMBOL_COLON);
                if (v.Length >= 2)
                {
                    point_pvp_start_time = System.DateTime.Today.AddHours(v[0]).AddMinutes(v[1]);
                }
            }
        }

        [CSVElement("map_arena")]
        public string mapArena;

        [CSVElement("map_expedition")]
        public string mapExpedition;

        [CSVElement("map_worldBoss")]
        public string mapWorldBoss;

        [CSVElement("map_skillDisplay")]
        public string mapSkillDisplay;

        [CSVElement("map_pvp")]
        public string mapPvp;

        [CSVElement("map_consortiaFight")]
        public string mapConsortiaFight;

		public float[] starAttrFactor = new float[6];

//		[CSVElement("star1Attr")]
//		public float star1Attr
//		{
//			set
//			{
//				starAttrFactor[0] = value;
//			}
//		}
//		[CSVElement("star2Attr")]
//		public float star2Attr
//		{
//			set
//			{
//				starAttrFactor[1] = value;
//			}
//		}
//		[CSVElement("star3Attr")]
//		public float star3Attr
//		{
//			set
//			{
//				starAttrFactor[2] = value;
//			}
//		}
//		[CSVElement("star4Attr")]
//		public float star4Attr
//		{
//			set
//			{
//				starAttrFactor[3] = value;
//			}
//		}
//		[CSVElement("star5Attr")]
//		public float star5Attr
//		{
//			set
//			{
//				starAttrFactor[4] = value;
//			}
//		}
//		[CSVElement("star6Attr")]
//		public float star6Attr
//		{
//			set
//			{
//				starAttrFactor[5] = value;
//			}
//		}

		public List<int> dailyDungeonAwardVIPLevelList = new List<int>();
		[CSVElement("daily_dungeon_award_vip")]
		public string dailyDungeonAwardVIP
		{
			set
			{
				string[] strList = value.Split(CSVUtil.SYMBOL_SEMICOLON);
				for (int i = 0, cout = strList.Length; i < cout; i++)
				{
					dailyDungeonAwardVIPLevelList.Add(strList[i].ToString().ToInt32());
				}
			}
		}

		public List<int> dailyDungeonAwardRateList = new List<int>();
		[CSVElement("daily_dungeon_award_rate")]
		public string dailyDungeonAwardRate
		{
			set
			{
				string[] strList = value.Split(CSVUtil.SYMBOL_SEMICOLON);
				for (int i = 0, count = strList.Length; i < count; i++)
				{
					dailyDungeonAwardRateList.Add(strList[i].ToString().ToInt32());
				}
			}
		}

		public List<GameResData> dailyDungeonAwardCostList = new List<GameResData>();
		[CSVElement("daily_dungeon_award_cost")]
		public string dailyDungeonAwardCost
		{
			set
			{
				string[] strList = value.Split(CSVUtil.SYMBOL_SEMICOLON);
				for (int i = 0, count = strList.Length; i < count; i++)
				{
					GameResData gameResData = new GameResData(strList[i]);
					dailyDungeonAwardCostList.Add(gameResData);
				}
			}
		}

        [CSVElement("Dungeon_Atk_CombatAdd")]
        public float dungeonAtkCombatAdd;

		public float[] starAttr = new float[7];
		[CSVElement("star1Attr")]
		public string star1Attr
		{
			set
			{
				starAttr[1] = value.ToFloat();
			}
		}
		[CSVElement("star2Attr")]
		public string star2Attr
		{
			set
			{
				starAttr[2] = value.ToFloat();
			}
		}
		[CSVElement("star3Attr")]
		public string star3Attr
		{
			set
			{
				starAttr[3] = value.ToFloat();
			}
		}
		[CSVElement("star4Attr")]
		public string star4Attr
		{
			set
			{
				starAttr[4] = value.ToFloat();
			}
		}
		[CSVElement("star5Attr")]
		public string star5Attr
		{
			set
			{
				starAttr[5] = value.ToFloat();
			}
		}
		[CSVElement("star6Attr")]
		public string star6Attr
		{
			set
			{
				starAttr[6] = value.ToFloat();
			}
		}
    }
}