using UnityEngine;
using System.Collections.Generic;
using Common.Util;
using Logic.Game.Model;

namespace Logic.WorldBoss.Model
{
	public class WorldBossData
	{
		private static Dictionary<int, WorldBossData> _worldBossDataDictionary;

		public static Dictionary<int, WorldBossData> GetWorldBossData ()
		{
			if (_worldBossDataDictionary == null)
			{
				_worldBossDataDictionary = CSVUtil.Parse<int, WorldBossData>("config/csv/world_boss", "id");
			}
			return _worldBossDataDictionary;
		}

		public static List<WorldBossData> GetDamageRankWorldBossDataList ()
		{
			List<WorldBossData> damageRankWorldBossDataList = new List<WorldBossData>();
			List<WorldBossData> allWorldBossData = GetWorldBossData().GetValues();
			int allWorldBossDataCount = allWorldBossData.Count;
			WorldBossData worldBossData = null;
			for (int i = 0; i < allWorldBossDataCount; i++)
			{
				worldBossData = allWorldBossData[i];
				if (worldBossData.rankMin != 0 && worldBossData.rankMax != 0)
				{
					damageRankWorldBossDataList.Add(worldBossData);
				}
			}
			return damageRankWorldBossDataList;
		}

		public static WorldBossData GetKillWorldBossReward ()
		{
			// 击杀奖励id为11
			return GetWorldBossData()[11];
		}


		[CSVElement("id")]
		public int id;

		[CSVElement("rank_min")]
		public int rankMin;

		[CSVElement("rank_max")]
		public int rankMax;

		[CSVElement("reward_up")]
		public int rewardUp;

		[CSVElement("max_might")]
		public int maxMight;

		[CSVElement("boss_killed")]
		public int bossKilled;

		[CSVElement("player_damage")]
		public int playerDamage;

		[CSVElement("rank_num")]
		public int rankNum;

		[CSVElement("above_player_lv")]
		public int abovePlayerLevel;

		[CSVElement("player_number")]
		public int playerNumber;

		public List<GameResData> rewardList;
		[CSVElement("reward")]
		public string rewardString
		{
			set
			{
				rewardList = GameResData.ParseGameResDataList(value);
			}
		}

		public List<GameResData> rewardShowList;
		[CSVElement("reward_show")]
		public string rewardShowString
		{
			set
			{
				rewardShowList = GameResData.ParseGameResDataList(value);
			}
		}
	}
}
