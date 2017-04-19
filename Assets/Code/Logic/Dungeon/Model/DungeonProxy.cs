using UnityEngine;
using System.Collections.Generic;
using Logic.Protocol.Model;
using Logic.Enums;
using Logic.Chapter.Model;
using Logic.Team.Model;
using Logic.Hero.Model;
using LuaInterface;
using System.Collections;

namespace Logic.Dungeon.Model
{
    public class DungeonProxy : SingletonMono<DungeonProxy>
    {
		public System.Action onDungeonInfosUpdateDelegate;

		private LuaTable _dungeonModelLuaTable;
		public LuaTable DungeonModelLuaTable
		{
			get
			{
				if (_dungeonModelLuaTable == null)
					_dungeonModelLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "dungeon_model")[0];
				return _dungeonModelLuaTable;
			}
		}

		public System.Action onDungeonInfoUpdateDelegate;

		private int _lastUnlockEasyDungeonID = 0;
        public int LastUnlockEasyDungeonID
        {
            get
            {
				return _lastUnlockEasyDungeonID;
            }
        }

		private int _lastUnlockNormalDungeonID = 0;
        public int LastUnlockNormalDungeonID
        {
            get
            {
				return _lastUnlockNormalDungeonID;
            }
        }

		private int _lastUnlockHardDungeonID = 0;
        public int LastUnlockHardDungeonID
        {
            get
            {
				return _lastUnlockHardDungeonID;
            }
        }

        private Dictionary<int, DungeonInfo> _dungeonInfoDictionary = new Dictionary<int, DungeonInfo>();
        public Dictionary<int, DungeonInfo> DungeonInfoDictionary
        {
            get
            {
				return _dungeonInfoDictionary;
            }
        }

        private Dictionary<int, DungeonInfo> _easyDungeonInfoDictionary = new Dictionary<int, DungeonInfo>();
        public Dictionary<int, DungeonInfo> EasyDungeonInfoDictionary
        {
			get
			{
				return _easyDungeonInfoDictionary;
			}
        }

        private Dictionary<int, DungeonInfo> _normalDungeonInfoDictionary = new Dictionary<int, DungeonInfo>();
        public Dictionary<int, DungeonInfo> NormalDungeonInfoDictionary
        {
			get
			{
				return _normalDungeonInfoDictionary;
			}
        }

        private Dictionary<int, DungeonInfo> _hardDungeonInfoDictionary = new Dictionary<int, DungeonInfo>();
        public Dictionary<int, DungeonInfo> HardDungeonInfoDictionary
        {
			get
			{
				Dictionary<int, DungeonInfo> hardDungeonInfoDictionary = new Dictionary<int, DungeonInfo>();
				LuaTable hardDungeonInfoListLuaTable = DungeonModelLuaTable["hardDungeonInfoList"] as LuaTable;
				foreach (DictionaryEntry kvp in hardDungeonInfoListLuaTable.ToDictTable())
				{
					int dungeonID = kvp.Key.ToString().ToInt32();
					LuaTable dungeonInfoLuaTable = kvp.Value as LuaTable;
					hardDungeonInfoDictionary.Add(dungeonID, new DungeonInfo(dungeonInfoLuaTable));
				}
				return hardDungeonInfoDictionary;
			}
        }

        public Dictionary<int, DungeonInfo> GetDungeonInfoDictionaryByType(DungeonType type)
        {

            Dictionary<int, DungeonInfo> dic = null;

            switch (type)
            {
                case DungeonType.Easy:
                    dic = EasyDungeonInfoDictionary;
                    break;
                case DungeonType.Normal:
                    dic = NormalDungeonInfoDictionary;
                    break;
                case DungeonType.Hard:
                    dic = HardDungeonInfoDictionary;
                    break;

            }
            return dic;
        }

        #region delegates
        public delegate void OnPveFightOverDelegate();
        public OnPveFightOverDelegate onPveFightOverDelegate;
        #endregion delegates

        void Awake()
        {
            instance = this;
			Observers.Facade.Instance.RegisterObserver("OnUpdateDungeonInfos", UpdateDungeonInfos);
        }

		void OnDestroy ()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver("OnUpdateDungeonInfos", UpdateDungeonInfos);
			}
		}

        public DungeonInfo GetDungeonInfo(int dungeonID)
        {
            DungeonInfo dungeonInfo = null;
            DungeonInfoDictionary.TryGetValue(dungeonID, out dungeonInfo);
            return dungeonInfo;
        }

        public List<DungeonInfo> GetAllDungeonInfos()
        {
            return new List<DungeonInfo>(DungeonInfoDictionary.Values);
        }

        public List<DungeonInfo> GetDungeonInfos(int[] dungeonIDs)
        {
            List<DungeonInfo> dungeonInfos = new List<DungeonInfo>();
            int dungeonIDsCount = dungeonIDs.Length;
            for (int i = 0; i < dungeonIDsCount; i++)
            {
                DungeonInfo dungeonInfo = null;
                DungeonInfoDictionary.TryGetValue(dungeonIDs[i], out dungeonInfo);
                if (dungeonInfo != null)
                {
                    dungeonInfos.Add(dungeonInfo);
                }
            }
            return dungeonInfos;
        }

        public int GetLastUnlockDungeonID(DungeonType dungeonType)
        {
            int lastUnlockDungeonID = -1;
            switch (dungeonType)
            {
                case DungeonType.Easy:
                    lastUnlockDungeonID = LastUnlockEasyDungeonID;
                    break;
                case DungeonType.Normal:
                    lastUnlockDungeonID = LastUnlockNormalDungeonID;
                    break;
                case DungeonType.Hard:
                    lastUnlockDungeonID = LastUnlockHardDungeonID;
                    break;
                default:
                    break;
            }
            return lastUnlockDungeonID;
        }

		public DungeonType LastSelectPVEDungeonType
		{
			get
			{
				DungeonType lastSelectPVEDungeonType = DungeonType.Invalid;
				string key = string.Format("{0}:{1}", Game.Model.GameProxy.instance.AccountId, "LastSelectPVEDungeonType");
				lastSelectPVEDungeonType = (DungeonType)PlayerPrefs.GetInt(key);
				if (lastSelectPVEDungeonType == DungeonType.Invalid)
				{
					lastSelectPVEDungeonType = DungeonType.Easy;
				}
				return lastSelectPVEDungeonType;
			}
			set
			{
				string key = string.Format("{0}:{1}", Game.Model.GameProxy.instance.AccountId, "LastSelectPVEDungeonType");
				PlayerPrefs.SetInt(key, (int)value);
				PlayerPrefs.Save();
			}
		}

		public int GetTotalStarCountOfChapterOfDungeonType (DungeonType dungeonType, int chapterID)
		{
			ChapterData chapterData = ChapterData.GetChapterDataById(chapterID);
			List<int> dungeonIDList = chapterData.GetChapterDungeonIDListOfDungeonType(dungeonType);
			return dungeonIDList.Count * 3;
		}

		public int GetPlayerGainStarCountOfChapterOfDungeonType (DungeonType dungeonType, int chapterID)
		{
			int playerGainStarCountOfChpaterOfDungeonType = 0;
			ChapterData chapterData = ChapterData.GetChapterDataById(chapterID);
			List<int> dungeonIDList = chapterData.GetChapterDungeonIDListOfDungeonType(dungeonType);
			DungeonInfo dungeonInfo = null;
			for (int i = 0; i < dungeonIDList.Count; i++)
			{
				playerGainStarCountOfChpaterOfDungeonType += GetDungeonInfo(dungeonIDList[i]).star;
			}
			return playerGainStarCountOfChpaterOfDungeonType;
		}

		public int GetTotalStarCountOfDungeonType (DungeonType dungeonType)
		{
			return DungeonModelLuaTable.GetLuaFunction("GetTotalStarCountOfDungeonType").Call((int)dungeonType)[0].ToString().ToInt32();
		}

		public int GetDungeonTotalStarCount ()
		{
			return DungeonModelLuaTable.GetLuaFunction("GetAllDungeonTotalStarCount").Call(null)[0].ToString().ToInt32();
		}

		private bool UpdateDungeonInfos (Observers.Interfaces.INotification note)
		{
			_lastUnlockEasyDungeonID = DungeonModelLuaTable["lastUnlockEasyDungeonID"].ToString().ToInt32();
			_lastUnlockNormalDungeonID = DungeonModelLuaTable["lastUnlcokNormalDungeonID"].ToString().ToInt32();
			_lastUnlockHardDungeonID = DungeonModelLuaTable["lastUnlockHardDungeonID"].ToString().ToInt32();

			/* ALL DUNGEON INFOS DICTIONARY */
			LuaTable allDungeonInfoListLuaTable = DungeonModelLuaTable["allDungeonInfoList"] as LuaTable;
			foreach (DictionaryEntry kvp in allDungeonInfoListLuaTable.ToDictTable())
			{
				int dungeonID = kvp.Key.ToString().ToInt32();
				LuaTable dungeonInfoLuaTable = kvp.Value as LuaTable;
				if (_dungeonInfoDictionary.ContainsKey(dungeonID) && _dungeonInfoDictionary[dungeonID] != null)
					_dungeonInfoDictionary[dungeonID].UpdateBy(dungeonInfoLuaTable);
				else
					_dungeonInfoDictionary.Add(dungeonID, new DungeonInfo(dungeonInfoLuaTable));
			}
			/* ALL DUNGEON INFOS DICTIONARY */

			/* EASY DUNGEON INFOS DICTIONARY */
			LuaTable easyDungeonInfoListLuaTable = DungeonModelLuaTable["easyDungeonInfoList"] as LuaTable;
			foreach (DictionaryEntry kvp in easyDungeonInfoListLuaTable.ToDictTable())
			{
				int dungeonID = kvp.Key.ToString().ToInt32();
				LuaTable dungeonInfoLuaTable = kvp.Value as LuaTable;
				if (_easyDungeonInfoDictionary.ContainsKey(dungeonID) && _easyDungeonInfoDictionary[dungeonID] != null)
					_easyDungeonInfoDictionary[dungeonID].UpdateBy(dungeonInfoLuaTable);
				else
					_easyDungeonInfoDictionary.Add(dungeonID, new DungeonInfo(dungeonInfoLuaTable));
			}
			/* EASY DUNGEON INFOS DICTIONARY */

			/* NORMAL DUNGEON INFOS DICTIONARY */
			LuaTable normalDungeonInfoListLuaTable = DungeonModelLuaTable["normalDungeonInfoList"] as LuaTable;
			foreach (DictionaryEntry kvp in normalDungeonInfoListLuaTable.ToDictTable())
			{
				int dungeonID = kvp.Key.ToString().ToInt32();
				LuaTable dungeonInfoLuaTable = kvp.Value as LuaTable;
				if (_normalDungeonInfoDictionary.ContainsKey(dungeonID) && _normalDungeonInfoDictionary[dungeonID] != null)
					_normalDungeonInfoDictionary[dungeonID].UpdateBy(dungeonInfoLuaTable);
				else
					_normalDungeonInfoDictionary.Add(dungeonID, new DungeonInfo(dungeonInfoLuaTable));
			}
			/* NORMAL DUNGEON INFOS DICTIONARY */

			/* HARD DUNGEON INFOS DICTIONARY */
			LuaTable hardDungeonInfoListLuaTable = DungeonModelLuaTable["hardDungeonInfoList"] as LuaTable;
			foreach (DictionaryEntry kvp in hardDungeonInfoListLuaTable.ToDictTable())
			{
				int dungeonID = kvp.Key.ToString().ToInt32();
				LuaTable dungeonInfoLuaTable = kvp.Value as LuaTable;
				if (_hardDungeonInfoDictionary.ContainsKey(dungeonID) && _hardDungeonInfoDictionary[dungeonID] != null)
					_hardDungeonInfoDictionary[dungeonID].UpdateBy(dungeonInfoLuaTable);
				else
					_hardDungeonInfoDictionary.Add(dungeonID, new DungeonInfo(dungeonInfoLuaTable));
			}
			/* HARD DUNGEON INFOS DICTIONARY */

			if (onDungeonInfosUpdateDelegate != null)
			{
				onDungeonInfosUpdateDelegate();
			}
			return true;
		}

        public void OnPveFightOver()
        {
            if (onPveFightOverDelegate != null)
            {
                onPveFightOverDelegate();
            }
        }
    }
}
