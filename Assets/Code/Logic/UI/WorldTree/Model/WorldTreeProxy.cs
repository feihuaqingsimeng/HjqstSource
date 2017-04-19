using System.Collections.Generic;
using Logic.Dungeon.Model;
using Logic.Enums;
using Logic.Game.Model;
using UnityEngine;
using Common.GameTime.Controller;
using Logic.UI.WorldTree.Controller;

namespace Logic.UI.WorldTree.Model
{
	public class WorldTreeProxy : SingletonMono<WorldTreeProxy>
	{
		public delegate void OnWorldTreeDungeonInfosUpdateDelegate();
		public OnWorldTreeDungeonInfosUpdateDelegate onWorldTreeDungeonInfosUpdateDelegate;

		public delegate void OnWorldTreeFruitUpdateDelegate();
		public OnWorldTreeFruitUpdateDelegate onWorldTreeFruitUpdateDelegate;

		public delegate void OnWorldTreeFruitNextRecoverCountDownDelegate(int countDownTime);
		public OnWorldTreeFruitNextRecoverCountDownDelegate onWorldTreeFruitNextRecoverCountDownDelegate;

		public System.Action onWorldTreeFightOverDelegate;

		private bool _isInited = false;
		private bool _isWaitingWorldTreeFruitUpdate = false;

		private int _worldTreeFruitPurchasedTimes;
		public int WorldTreeFruitPurchasedTimes
		{
			get
			{
				return _worldTreeFruitPurchasedTimes;
			}
		}

		private int _faildTimes;
		public int FailedTimes
		{
			get
			{
				return _faildTimes;
			}
		}

		private int _worldTreeFruit;
		public int WorldTreeFruit
		{
			get
			{
				return _worldTreeFruit;
			}
		}

		private int _worldTreeFruitUpperLimit;
		public int WorldTreeFruitUpperLimit
		{
			get
			{
				return _worldTreeFruitUpperLimit;
			}
		}

		private long _worldTreeNextRecoverTime;
		public long WorldTreeNextRecoverTime
		{
			get
			{
				return _worldTreeNextRecoverTime;
			}
		}

		private WorldTreeDungeonInfo _unlockedWorldTreeDungeonInfo;
		public WorldTreeDungeonInfo UnlockedWorldTreeDungeonInfo
		{
			get
			{
				return _unlockedWorldTreeDungeonInfo;
			}
		}

		private Dictionary<int, WorldTreeDungeonInfo> _worldTreeDungeonInfoDictionary;
		public Dictionary<int, WorldTreeDungeonInfo> WorldTreeDungeonInfoDictionary
		{
			get
			{
				if (_worldTreeDungeonInfoDictionary == null)
				{
					_worldTreeDungeonInfoDictionary = new Dictionary<int, WorldTreeDungeonInfo>();
					List<DungeonData> worldTreeDungeonDataList = DungeonData.GetDungeonDataListByType(DungeonType.WorldTree);
					DungeonData dungeonData = null;
					for (int i = 0; i < worldTreeDungeonDataList.Count; i++)
					{
						dungeonData = worldTreeDungeonDataList[i];
						WorldTreeDungeonInfo worldTreeDungeonInfo = new WorldTreeDungeonInfo(dungeonData, i + 1);
						_worldTreeDungeonInfoDictionary.Add(worldTreeDungeonInfo.dungeonID, worldTreeDungeonInfo);
					}
				}
				return _worldTreeDungeonInfoDictionary;
			}
		}

		public List<WorldTreeDungeonInfo> GetAllWorldTreeDungeonInfoList ()
		{
			return WorldTreeDungeonInfoDictionary.GetValues();
		}

		public WorldTreeDungeonInfo GetWorldTreeInfoByID (int id)
		{
			WorldTreeDungeonInfo worldTreeDungeonInfo = null;
			WorldTreeDungeonInfoDictionary.TryGetValue(id, out worldTreeDungeonInfo);
			return worldTreeDungeonInfo;
		}

		public int GetWorldTreeDungeonIndexByID (int worldTreeDungeonID)
		{
			return WorldTreeDungeonInfoDictionary.GetKeys().IndexOf(worldTreeDungeonID);
		}

		public WorldTreeDungeonInfo GetNextWorldTreeDungeonInfo (int currentWorldTreeDungeonID)
		{
			WorldTreeDungeonInfo nextWorldTreeDungeonInfo = null;
			List<WorldTreeDungeonInfo> allWorldTreeDungeonInfoList = GetAllWorldTreeDungeonInfoList();
			for (int i = 0, count = allWorldTreeDungeonInfoList.Count; i < count - 1; i++)
			{
				if (allWorldTreeDungeonInfoList[i].dungeonID == currentWorldTreeDungeonID)
				{
					nextWorldTreeDungeonInfo = allWorldTreeDungeonInfoList[i + 1];
				}
			}
			return nextWorldTreeDungeonInfo;
		}

		public float GetWorldTreeChallengeFailedWeakenValue ()
		{
			float weakenValue = (float)System.Math.Round(WorldTreeProxy.instance.FailedTimes * GlobalData.GetGlobalData().worldTreeChallengeFailedWeakenBuff, 2);
			weakenValue = Mathf.Min(weakenValue, GlobalData.GetGlobalData().worldTreeChallengeFailedWeakenBuffMax);
			return weakenValue;
		}

		void Awake ()
		{
			instance = this;
		}

		void Update ()
		{
			if (_isInited)
			{
				if (_worldTreeFruit <= 0)
				{
					int worldTreeFruitNextRecoverCountDownTime = (int)TimeController.instance.GetDiffTimeWithServerTimeInSecond(_worldTreeNextRecoverTime);
					if (worldTreeFruitNextRecoverCountDownTime > 0)
					{
						if (onWorldTreeFruitNextRecoverCountDownDelegate != null)
						{
							onWorldTreeFruitNextRecoverCountDownDelegate(worldTreeFruitNextRecoverCountDownTime);
						}
					}
					else if (!_isWaitingWorldTreeFruitUpdate)
					{
						WorldTreeController.instance.CLIENT2LOBBY_WORLD_TREE_FRUIT_SYN_REQ();
						_isWaitingWorldTreeFruitUpdate = true;
					}
				}
			}
		}

		public void OnWorldTreeDungeonInfosUpdate(int lastPassDungeon, int fruitPurchasedTimes, int failedTimes)
		{
			_worldTreeFruitPurchasedTimes = fruitPurchasedTimes;
			_faildTimes = failedTimes;
			List<WorldTreeDungeonInfo> allWorldTreeDungeonInfoList = GetAllWorldTreeDungeonInfoList();
			int allWorldTreeDungeonInfoCount = allWorldTreeDungeonInfoList.Count;
			int lastPassDungeonIndex = GetWorldTreeDungeonIndexByID(lastPassDungeon);
			WorldTreeDungeonInfo worldTreeDungeonInfo = null;
			for(int i = 0; i < allWorldTreeDungeonInfoCount; i++)
			{
				worldTreeDungeonInfo = allWorldTreeDungeonInfoList[i];
				if (i <= lastPassDungeonIndex)
				{
					worldTreeDungeonInfo.worldTreeDungeonStatus = WorldTreeDungeonStatus.Passed;
				}
				else if (i == lastPassDungeonIndex + 1)
				{
					worldTreeDungeonInfo.worldTreeDungeonStatus = WorldTreeDungeonStatus.Unlocked;
					_unlockedWorldTreeDungeonInfo = worldTreeDungeonInfo;
				}
				else
				{
					worldTreeDungeonInfo.worldTreeDungeonStatus = WorldTreeDungeonStatus.Locked;
				}
			}

			if (onWorldTreeDungeonInfosUpdateDelegate != null)
			{
				onWorldTreeDungeonInfosUpdateDelegate();
			}
		}

		public void OnWorldTreeFruitUpdate (int worldTreeFruit, int worldTreeFruitUpperLimit, long worldTreeNextRecoverTime)
		{
			_worldTreeFruit = worldTreeFruit;
			_worldTreeFruitUpperLimit = worldTreeFruitUpperLimit;
			_worldTreeNextRecoverTime = worldTreeNextRecoverTime;

			if (onWorldTreeFruitUpdateDelegate != null)
			{
				onWorldTreeFruitUpdateDelegate();
			}

			_isInited = true;
			_isWaitingWorldTreeFruitUpdate = false;
		}

		public void OnWolrdTreeFightOver ()
		{
			if (onWorldTreeFightOverDelegate != null)
			{
				onWorldTreeFightOverDelegate();
			}
		}
	}
}