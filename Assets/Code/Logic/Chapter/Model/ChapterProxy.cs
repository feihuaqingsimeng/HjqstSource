using UnityEngine;
using System.Collections.Generic;
using Logic.Enums;
using Logic.Dungeon.Model;
using LuaInterface;

namespace Logic.Chapter.Model
{
    public class ChapterProxy : SingletonMono<ChapterProxy>		
    {
		private static LuaTable _chapterModelLuaTable;
		public static LuaTable ChapterModelLuaTable
		{
			get
			{
				if (_chapterModelLuaTable == null)
					_chapterModelLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl", "chapter_model")[0];
				return _chapterModelLuaTable;
			}
		}

		private Dictionary<int, ChapterInfo> _chapterInfoDictionary;

		public System.Action onDungeonStarBoxInfoUpdateDelegate;

		public Dictionary<int, ChapterInfo> ChapterInfoDictionary
		{
			get
			{
				if (_chapterInfoDictionary == null)
				{
					_chapterInfoDictionary = new Dictionary<int, ChapterInfo>();
					Dictionary<int, ChapterData> chapterDataDictionary = ChapterData.GetChapterDatas();
					List<int> chapterIDList = new List<int>(chapterDataDictionary.Keys);
					int chapterIDCount = chapterIDList.Count;
					for (int i = 0; i < chapterIDCount; i++)
					{
						int chapterDataID = chapterIDList[i];
						ChapterInfo chapterInfo = new ChapterInfo(chapterDataID);
						_chapterInfoDictionary.Add(chapterDataID, chapterInfo);
					}
				}
				return _chapterInfoDictionary;
			}
		}

		public DungeonType _lastSelectedDungeonType;
		public DungeonType LastSelectedDungeonType
		{
			get
			{
				return _lastSelectedDungeonType;
			}
		}

		public int _lastSelectedChapterID;
		public int LastSelectedChapterID
		{
			get
			{
				return _lastSelectedChapterID;
			}
		}

		public void SetLastSelect (DungeonType dungeonType, int chapterID)
		{
			_lastSelectedDungeonType = dungeonType;
			_lastSelectedChapterID = chapterID;
		}

		public ChapterInfo GetChapterInfo (int chapterID)
		{
			ChapterInfo chapterInfo = null;
			ChapterInfoDictionary.TryGetValue(chapterID, out chapterInfo);
			return chapterInfo;
		}

		public List<int> GetAllChapterIDs ()
		{
			return new List<int>(ChapterInfoDictionary.Keys);
		}

		public List<ChapterInfo> GetAllChapterInfos ()
		{
			return new List<ChapterInfo>(ChapterInfoDictionary.Values);
		}

        public int currentChapterID;

        void Awake() 
        {
            instance = this;
			Observers.Facade.Instance.RegisterObserver("OnDungeonStarBoxInfoUpdate", OnDungeonStarBoxInfoUpdate);
        }

		void OnDestroy ()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RegisterObserver("OnDungeonStarBoxInfoUpdate", OnDungeonStarBoxInfoUpdate);
			}
		}

        void Start() 
        {
			_lastSelectedDungeonType = DungeonType.Invalid;
			_lastSelectedChapterID = -1;
            currentChapterID = GetAllChapterIDs()[0];
        }

        public List<ChapterInfo> GetChapterInfos()
        {
            List<ChapterInfo> result = new List<ChapterInfo>();

            return result;
        }

		public int GetChapterIdByDungeonInfo (DungeonInfo dungeonInfo)
		{
			if(dungeonInfo == null)
			{
				return 0;
			}

			List<ChapterInfo> chapterList =  ChapterProxy.instance.GetAllChapterInfos();
			ChapterInfo cinfo;
			int[] dungeonIds;
			for(int i = 0,count = chapterList.Count;i<count;i++)
			{
				cinfo = chapterList[i];
				dungeonIds = cinfo.chapterData.GetChapterDungeonIDListOfDungeonType(dungeonInfo.dungeonData.dungeonType).ToArray();
				for(int j = 0,count2 = dungeonIds.Length;j<count2;j++)
				{
					if(dungeonInfo.id == dungeonIds[j])
					{
						return cinfo.chapterId;
					}
				}
			}
			return 0;
		}

		public bool HasUnreceivedDungeonStarBox (DungeonType dungeonType)
		{
			return ChapterModelLuaTable.GetLuaFunction("HasUnreceivedDungeonStarBox").Call((int)dungeonType)[0].ToString().ToBoolean();
		}

		bool OnDungeonStarBoxInfoUpdate (Observers.Interfaces.INotification note)
		{
			if (onDungeonStarBoxInfoUpdateDelegate != null)
			{
				onDungeonStarBoxInfoUpdateDelegate();
			}
			return true;
		}
    }
}