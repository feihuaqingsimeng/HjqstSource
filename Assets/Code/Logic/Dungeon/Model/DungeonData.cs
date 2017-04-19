using System.Collections.Generic;
using Common.Util;
using Logic.Enums;
using Logic.Game.Model;
using Logic.Hero.Model;
using Logic.Team.Model;
using Common.Localization;

namespace Logic.Dungeon.Model
{
    public class DungeonData
    {
        private static Dictionary<int, DungeonData> _dungeonDataDictionary;

        public static Dictionary<int, DungeonData> GetDungeonDatas()
        {
            if (_dungeonDataDictionary == null)
            {
                _dungeonDataDictionary = CSVUtil.Parse<int, DungeonData>("config/csv/dungeon", "id");
            }
            return _dungeonDataDictionary;
        }

        public static Dictionary<int, DungeonData> DungeonDataDictionary
        {
            get
            {
                if (_dungeonDataDictionary == null)
                {
                    GetDungeonDatas();
                }
                return _dungeonDataDictionary;
            }
        }

        public static List<DungeonData> GetAllDungeonDataList()
        {
            return new List<DungeonData>(DungeonDataDictionary.Values);
        }

        public static DungeonData GetDungeonDataByID(int dungeonID)
        {
            DungeonData dungeonData = null;
            if (DungeonDataDictionary.ContainsKey(dungeonID) && DungeonDataDictionary[dungeonID] != null)
            {
                dungeonData = DungeonDataDictionary[dungeonID];
            }
			else
			{
				Debugger.LogError("can not find dungeon data ,id:"+dungeonID);
			}
            return dungeonData;
        }

		public static List<DungeonData> GetDungeonDataListByType (DungeonType dungeonType)
		{
			List<DungeonData> result = new List<DungeonData>();
			List<DungeonData> allDungeonDataList = GetAllDungeonDataList();
			int allDungeonDataCount = allDungeonDataList.Count;
			DungeonData dungeonData = null;
			for (int i = 0; i < allDungeonDataCount; i++)
			{
				dungeonData = allDungeonDataList[i];
				if (dungeonData.dungeonType == dungeonType)
				{
					result.Add(dungeonData);
				}
			}
			return result;
		}
		public string GetDungeonTypeName()
		{
			if( dungeonType == DungeonType.Easy)
			{
				return Localization.Get("ui.select_chapter_view.easy_type");
			}else if(dungeonType == DungeonType.Normal)
			{
				return Localization.Get("ui.select_chapter_view.normal_type");
			}else if(dungeonType == DungeonType.Hard)
			{
				return Localization.Get("ui.select_chapter_view.hard_type");
			}
			return "";
		}
		public string GetOrderName()
		{
			return Localization.Get(order_name);
		}
		public string GetDungeonName()
		{
			return Localization.Get(name);
		}
		
        [CSVElement("id")]
        public int dungeonID;

        [CSVElement("name")]
        public string name;

        [CSVElement("description")]
        public string description;

        [CSVElement("dungeon_map")]
        public string dungeonMap;

		[CSVElement("unlock_lv")]
        public int unlockLevel;

		public DungeonBossType dungeonBossType;
		[CSVElement("is_boss")]
		public int isBoss
		{
			set
			{
				dungeonBossType = (DungeonBossType)value;
			}
		}

		[CSVElement("unlock_dungeon_id_pre1")]
		public int unlockDungeonIDPre1;

		[CSVElement("unlock_dungeon_id_pre2")]
		public int unlockDungeonIDPre2;

		[CSVElement("unlock_dungeon_id_next1")]
		public int unlockDungeonIDNext1;

		[CSVElement("unlock_dungeon_id_next2")]
		public int unlockDungeonIDNext2;

        public List<uint> teamIDs;
		public List<TeamData> teamDataList;
        [CSVElement("teams")]
        public string teamID
        {
            set
            {
				teamIDs = value.ToList<uint>(CSVUtil.SYMBOL_SEMICOLON);
				teamDataList = new List<TeamData>();
				int teamIDCount = teamIDs.Count;
				TeamData teamData = null;
				for (int teamIDIndex = 0; teamIDIndex < teamIDCount; teamIDIndex++)
				{
					uint teamID = teamIDs[teamIDIndex];
					teamData = TeamData.GetTeamDataByID(teamID);
					teamDataList.Add(teamData);
				}
            }
        }
		public int[] each_loot_id;
		[CSVElement("each_loot_id")]
		public string each_loot_idStr
		{
			set
			{
				if(!string.IsNullOrEmpty(value))
				{
					each_loot_id = value.ToArray<int>(CSVUtil.SYMBOL_SEMICOLON);
				}
			}
		}
		public int[] special_loot_id;
		[CSVElement("special_loot_id")]
		public string special_loot_idStr
		{
			set
			{
				if(!string.IsNullOrEmpty(value))
				{
					special_loot_id = value.ToArray<int>(CSVUtil.SYMBOL_SEMICOLON);
				}
			}
		}

		public GameResData showFirstPassRewardGameResData = null;
		[CSVElement("show_first_loot_id")]
		public string showFirstLootID
		{
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					showFirstPassRewardGameResData = new GameResData(value);
				}
			}
		}

        public List<GameResData> eachLootPresent = new List<GameResData>();
        [CSVElement("each_loot_present")]
        public string eachLootPresentStr
        {
            set
            {
				string[] gameResDataStrs = value.ToArray(CSVUtil.SYMBOL_SEMICOLON);
				int gameResDataStrsCount = gameResDataStrs.Length;
				for (int i = 0; i < gameResDataStrsCount; i++)
				{
					GameResData gameResData = new GameResData(gameResDataStrs[i]);
					eachLootPresent.Add(gameResData);
				}
            }
        }
		//副本怪物展示
		private List<HeroInfo> _heroPresentList = null;
		public List<HeroInfo> heroPresentList 
		{
			get
			{
				if (_heroPresentList == null)
				{
					Dictionary<int,HeroInfo> monstersDic = new Dictionary<int,HeroInfo>();
					Dictionary<int,HeroInfo> bossDic = new Dictionary<int,HeroInfo>();
					
					Dictionary<FormationPosition,HeroData> teamDictionary  = null;
					for(int i = 0,count = teamDataList.Count;i<count;i++)
					{
						TeamData tData = teamDataList[i];
						foreach(var value in tData.teamDictionary)
						{
							if(tData.IsBoss((int)value.Key))
							{
								if (!bossDic.ContainsKey(value.Value.id))
									bossDic.Add(value.Value.id,new HeroInfo(value.Value));
							}else if (!monstersDic.ContainsKey(value.Value.id))
							{
								monstersDic.Add(value.Value.id,new HeroInfo(value.Value));
							}
						}
					}
					List<HeroInfo> monstersList = new List<HeroInfo>();
					int index = bossDic.Count;
					foreach(var value in monstersDic)
					{
						monstersList.Add(value.Value);
						index ++;
						if (index >= 4)
						{
							break;
						}
					}
					foreach(var value in bossDic)
					{
						monstersList.Add(value.Value);
					}
					_heroPresentList = monstersList;
				}
				return _heroPresentList;
			}
		}
		

        [CSVElement("rate_method")]
        public uint rateMethod;

        [CSVElement("action_need")]
        public uint actionNeed;

        [CSVElement("power_fix")]
        public float powerFix;

		public DungeonType dungeonType = DungeonType.Invalid;
		[CSVElement("type")]
		public uint type
		{
			set
			{
				dungeonType = (DungeonType)value;
			}
		}

		[CSVElement("dungeon_show")]
		public string order_name;

		[CSVElement("tree_show")]
		public string worldTreeRewardIconPath;

		[CSVElement("day_times")]
		public int dayChallengeTimes;

		[CSVElement("combat")]
		public int combat;


		public DungeonType unlockStarDungeonType = DungeonType.Normal;
		public int unlockStarCount = 0;
		[CSVElement("unlock_star")]
		public string ubnlockStar
		{
			set
			{
				if (value != null && value != string.Empty)
				{
					string unlcokStarStrs = value.Split(CSVUtil.SYMBOL_SEMICOLON)[0];
					string[] unlockStarItemStrs = unlcokStarStrs.Split(CSVUtil.SYMBOL_COLON);
					unlockStarDungeonType = (DungeonType)(unlockStarItemStrs[0].ToInt32());
					unlockStarCount = unlockStarItemStrs[1].ToInt32();
				}
			}
		}

		public GameResData itemCostGameResData = null;
		[CSVElement("item_cost")]
		public string itemCost
		{
			set
			{
				if (value != "-1")
				{
					itemCostGameResData = new GameResData(value);
				}
			}
		}
    }
}