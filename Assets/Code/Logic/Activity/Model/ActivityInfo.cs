using UnityEngine;
using System.Collections.Generic;
using Logic.Protocol.Model;
using Logic.VIP.Model;
using Logic.Dungeon.Model;

namespace Logic.Activity.Model
{
	public class ActivityInfo
	{
		public const string LastChallengedLevelKeyTemplate = "AccountID[{0}]::Activity[{1}]::LastChallengedLevel";
		private string LastChallengedLevelKey
		{
			get
			{
				return string.Format(LastChallengedLevelKeyTemplate, Logic.Game.Model.GameProxy.instance.AccountId, ActivityData.id);
			}
		}
		public int LastChallengedLevel
		{
			get
			{
				int lastChallengedLevel = 0;
				lastChallengedLevel = PlayerPrefs.GetInt(LastChallengedLevelKey);
				return lastChallengedLevel;
			}
			set
			{
				PlayerPrefs.SetInt(LastChallengedLevelKey, value);
				PlayerPrefs.Save();
			}
		}

		public const string ChallengedHighestLevelKeyTemplate = "AccountID[{0}]::Activity[{1}]::ChallengedHighestLevel";
		private string ChallengedHighestLevelKey
		{
			get
			{
				return string.Format(ChallengedHighestLevelKeyTemplate, Logic.Game.Model.GameProxy.instance.AccountId, ActivityData.id);
			}
		}
		public int ChallengedHighestLevel
		{
			get
			{
				int challengedHighestLevel = 0;
				challengedHighestLevel = PlayerPrefs.GetInt(ChallengedHighestLevelKey);
				return challengedHighestLevel;
			}
			set
			{
				PlayerPrefs.SetInt(ChallengedHighestLevelKey, value);
				PlayerPrefs.Save();
			}
		}

		public int UnlockedHighestLevel
		{
			get
			{
				int unlockedHighestLevel = 0;
				for (int i = 0, count = ActivityData.DungeonIDList.Count; i < count; i++)
				{
					DungeonData dungeonData = DungeonData.GetDungeonDataByID(ActivityData.DungeonIDList[i]);
					if (Logic.Game.Model.GameProxy.instance.AccountLevel > dungeonData.unlockLevel)
					{
						unlockedHighestLevel = i;
					}
				}
				return unlockedHighestLevel;
			}
		}

		private int _activityID;
		public int ActivityID
		{
			get
			{
				return _activityID;
			}
		}

		private ActivityData _activitydata;
		public ActivityData ActivityData
		{
			get
			{
				return _activitydata;
			}
		}

		public int remainChallengeTimes;
		public int lastChallengeDungeon;
		public List<int> passDungeonID;
		public bool isOpen = false;
		//vip已购买挑战次数
		public int buyActivityChanllengeTimes ;

		public int remainBuyActivityTimes
		{
			get
			{
				return VIPProxy.instance.VIPData.dailyDungeonBuyTimes - buyActivityChanllengeTimes ;
			}
		}
		public bool hasBuyActivityTimesByMaxVip
		{
			get
			{
				return (VIPData.VIPDataDictionary.Last().Value.dailyDungeonBuyTimes-buyActivityChanllengeTimes) > 0;
			}
		}

		public ActivityInfo (ActivityData activityData)
		{
			_activityID = activityData.id;
			_activitydata = activityData;
		}

		public void Reset ()
		{
			remainChallengeTimes = 0;
			lastChallengeDungeon = 0;
			passDungeonID = null;
			isOpen = false;
		}

		public void Refresh (ActivityPveProto activityPveProto)
		{
			if (activityPveProto.id == _activityID)
			{
				remainChallengeTimes = activityPveProto.remainChallengeTimes;
				lastChallengeDungeon = activityPveProto.lastChallengeDungeon;
				passDungeonID = activityPveProto.passDungeonId;
				isOpen = true;
			}
			buyActivityChanllengeTimes = activityPveProto.vipBuyTimes;
		}
	}
}