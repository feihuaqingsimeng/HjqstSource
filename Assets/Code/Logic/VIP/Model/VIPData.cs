using System.Collections.Generic;
using Common.Util;
using Logic.Game.Model;
using Common.Localization;

namespace Logic.VIP.Model
{
    public class VIPData
    {
        private static Dictionary<int, VIPData> _vipDataDictionary;

        public static Dictionary<int, VIPData> VIPDataDictionary
        {
            get
            {
                if (_vipDataDictionary == null)
                {
                    _vipDataDictionary = CSVUtil.Parse<int, VIPData>("config/csv/vip_config", "id");
                }
                return _vipDataDictionary;
            }
        }

        public static VIPData GetVIPData(int vipLevel)
        {
            VIPData vipData = null;
            VIPDataDictionary.TryGetValue(vipLevel, out vipData);
            return vipData;
        }

        public VIPData GetNextLevelVIPData()
        {
            VIPData nextLevelVIPData = null;
            VIPDataDictionary.TryGetValue(id + 1, out nextLevelVIPData);
            return nextLevelVIPData;
        }

        public static List<VIPData> GetAllVIPDataList()
        {
            return VIPDataDictionary.GetValues();
        }

        public bool IsMinLevelVIPData()
        {
            return this.id == VIPDataDictionary.First().Key;
        }

        public bool IsMaxLevelVIPData()
        {
            return this.id == VIPDataDictionary.Last().Key;
        }

        public string ShortName
        {
            get
            {
                return string.Format(Localization.Get("common.vip_short_name"), id);
            }
        }


        //		public static VIPData DoubleSpeedFightOpenVIPData
        //		{
        //			get
        //			{
        //				VIPData vipData = null;
        //				List<VIPData> vipDataList = GetAllVIPDataList();
        //				for (int i = 0, vipDataCount = vipDataList.Count; i < vipDataCount; i++)
        //				{
        //					vipData = vipDataList[i];
        //					if (vipData.canFightInDoubleSpeed)
        //					{
        //						break;
        //					}
        //				}
        //				return vipData;
        //			}
        //		}

        /// <summary>
        /// VIP ID.
        /// </summary>
        [CSVElement("id")]
        public int id;

        /// <summary>
        /// VIP显示名称.
        /// </summary>
        [CSVElement("name")]
        public string name;

        /// <summary>
        /// VIP权限描述.
        /// </summary>
        [CSVElement("des")]
        public string des;

        /// <summary>
        /// VIP福利物品列表.
        /// </summary>
        public List<GameResData> benefitItemList;
        [CSVElement("rewards")]
        public string rewards
        {
            set
            {
                benefitItemList = GameResData.ParseGameResDataList(value);
            }
        }
        public GameResData dailyWelfareSweep;
        [CSVElement("daily_welfare")]
        public string daily_welfare
        {
            set
            {
                dailyWelfareSweep = new GameResData(value);
            }
        }
        /// <summary>
        /// 相应VIP等级所需经验.
        /// </summary>
        [CSVElement("exp")]
        public int exp;

        /// <summary>
        /// 购买远征次数.
        /// </summary>
        [CSVElement("faraway")]
        public int BuyExpeditionTimes;

        /// <summary>
        /// 体力上限点数.
        /// </summary>
        [CSVElement("pve_action_add")]
        public int pveActionAdd;

        /// <summary>
        /// 每日副本购买次数.
        /// </summary>
        [CSVElement("daily_dungeon_buytimes")]
        public int dailyDungeonBuyTimes;

        /// <summary>
        /// PVE行动力购买次数.
        /// </summary>
        [CSVElement("pve_action_buytimes")]
        public int pveActionBuyTimes;

        /// <summary>
        /// 金币购买次数.
        /// </summary>
        [CSVElement("gold_buytimes")]
        public int goldBuyTimes;

        /// <summary>
        /// 竞技场刷新次数.
        /// </summary>
        [CSVElement("pvp_refresh")]
        public int pvpRefreshTimes;

        /// <summary>
        /// 竞技场每日可打次数.
        /// </summary>
        [CSVElement("pvp_add")]
        public int pvpAdd;

        /// <summary>
        /// 世界树行动力上限.
        /// </summary>
        [CSVElement("world_tree_add")]
        public int worldTreeActionAdd;

        /// <summary>
        /// 世界树行动力购买次数.
        /// </summary>
        [CSVElement("world_tree_buytimes")]
        public int worldTreeActionBuyTimes;

        /// <summary>
        /// 普通副本额外金币收益.
        /// </summary>
        [CSVElement("dungeon_gold_add")]
        public int dungeonExtraGoldAmount;

        /// <summary>
        /// 每日副本额外翻牌次数.
        /// </summary>
        [CSVElement("daily_dungeon_add")]
        public int dailyDungeonExtraDrawCardTimes;

        /// <summary>
        /// 阵型培养点购买次数.
        /// </summary>
        [CSVElement("formation_add")]
        public int formation_add;

        /// <summary>
        /// 三倍速.
        /// </summary>
        [CSVElement("three_speed")]
        public bool three_speed;

		[CSVElement("dayRefreshTimes")]
		public int dayRefreshTimes;
    }
}
