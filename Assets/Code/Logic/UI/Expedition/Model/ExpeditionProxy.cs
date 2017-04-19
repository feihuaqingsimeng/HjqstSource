using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Protocol.Model;
using Logic.Game.Model;
using Logic.Enums;
using Logic.UI.Expedition.Controller;
using Logic.VIP.Model;
using LuaInterface;

namespace Logic.UI.Expedition.Model
{
	public class ExpeditionProxy : SingletonMono<ExpeditionProxy> 
	{
		
		public System.Action onUpdateGetRewardSucDelegate;
		public System.Action onExpeditionFightOverDelegate;
		public System.Action onUpdateResetSucDelegate;
		//剩余重置次数
		public int resetCount = 1;
		public int expeditionVipBuyTimes;

		public int expeditionRemaindBuyTimes
		{
			get
			{
				return VIPProxy.instance.VIPData.BuyExpeditionTimes - ExpeditionProxy.instance.expeditionVipBuyTimes;
			}
		}
		public int nextVipExtraBuyTimes
		{
			get
			{
				Dictionary<int, VIPData> vipdic = VIPData.VIPDataDictionary;
				foreach(var data in vipdic)
				{
					if(data.Value.BuyExpeditionTimes > ExpeditionProxy.instance.expeditionVipBuyTimes)
						return data.Value.id;
				}
				return 0;
			}
		}
		public int CurrentExpeditionDungeonId;
		public int CurrentExpeditionChapterId = 1;

		private Dictionary<int,ExpeditionDungeonInfo> _dungeonInfoDictionary;
		public Dictionary<int,ExpeditionDungeonInfo> DungeonInfoDictionary
		{
			get
			{
				if(_dungeonInfoDictionary == null)
				{
					_dungeonInfoDictionary = new Dictionary<int, ExpeditionDungeonInfo>();
					List<ExpeditionData> dataList = ExpeditionData.ExpeditionDataDictionary.GetValues();
					int count = dataList.Count;
					ExpeditionData data;
					for(int i = 0;i<count;i++)
					{
						data = dataList[i];
						_dungeonInfoDictionary.Add(data.id,new ExpeditionDungeonInfo(data.id,false,false,false));
					}
				}
				return _dungeonInfoDictionary;
			}
		}
		public ExpeditionDungeonInfo selectExpeditionDungeonInfo;

		//自己主动刷新界面
		private bool _needRefresh = true;
		private System.DateTime _needRefreshTime;

		private LuaTable _expeditionModelLua;
		public LuaTable ExpeditionModelLua
		{
			get
			{
				if(_expeditionModelLua == null)
				{
					_expeditionModelLua = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","expedition_model")[0];
				}
				return _expeditionModelLua;
			}
		}

		void Awake()
		{
			instance = this;
		}
		public void Clear()
		{
			_needRefresh = true;
		}
		public void CheckRefresh()
		{
			if(_needRefresh )
			{
				_needRefreshTime = Common.GameTime.Controller.TimeController.instance.ServerTime;
				ExpeditionController.instance.CLIENT2LOBBY_Expedition_REQ();
				_needRefresh = false;

			}else{
				if(_needRefreshTime.DayOfYear < Common.GameTime.Controller.TimeController.instance.ServerTime.DayOfYear)
				{
					_needRefreshTime = Common.GameTime.Controller.TimeController.instance.ServerTime;
					ExpeditionController.instance.CLIENT2LOBBY_Expedition_REQ();
				}
			}

		}
		public void ResetDungeonInfoDictionaryFromLua()
		{
			LuaTable infoDic = (LuaTable)ExpeditionModelLua["expeditionDungeonInfoDic"];
			LuaTable valueList = (LuaTable)infoDic["data"];
			int id = 0;
			bool isFinished = false;
			bool isGetReward = false;
			bool isUnlocked = false;
			ExpeditionDungeonInfo info;
			foreach(DictionaryEntry kvp in valueList.ToDictTable())
			{
				LuaTable v = (LuaTable)kvp.Value;
				id = v["id"].ToString().ToInt32();
				isFinished = v["isFinished"].ToString().ToBoolean();
				isGetReward = v["isGetReward"].ToString().ToBoolean();
				isUnlocked = v["isUnlocked"].ToString().ToBoolean();
				info = DungeonInfoDictionary[id];
				info.isFinished = isFinished;
				info.isGetReward = isGetReward;
				info.isUnlocked = isUnlocked;
			}
			CurrentExpeditionDungeonId = ExpeditionModelLua["currentExpeditionDungeonId"].ToString().ToInt32();
			if(onUpdateResetSucDelegate!=null)
			{
				onUpdateResetSucDelegate();
			}
		}
		public void ResetDungeonInfoDictionary(int lastDungeonId,List<int> getRewardDungeonIds)
		{
			//
			List<ExpeditionDungeonInfo> infoList = DungeonInfoDictionary.GetValues();
			int count = infoList.Count;
            //ExpeditionData data;
			ExpeditionDungeonInfo info;
			for(int i = 0;i<count;i++)
			{
				info = infoList[i];
				info.SetFinished(info.id<=lastDungeonId);
				info.SetUnlocked(info.id<=lastDungeonId);
				info.SetGetReward(info.id <= lastDungeonId);
			}
			//unlock
			if(lastDungeonId== -1)
			{
				ExpeditionData firstData = ExpeditionData.ExpeditionDataDictionary.First().Value;
				CurrentExpeditionDungeonId = firstData.id;
				DungeonInfoDictionary[firstData.id].SetUnlocked(true);
			}else{
				ExpeditionData nextData = ExpeditionData.GetNextExpeditionData(lastDungeonId);
				if(nextData!=null)
				{
					CurrentExpeditionDungeonId = nextData.id;
					DungeonInfoDictionary[nextData.id].SetUnlocked(true);
					if(nextData.type !=(int)ExpeditionDungeonType.Expedition_Normal)
					{
						DungeonInfoDictionary[nextData.id].SetFinished(true);
					}
				}else{
					CurrentExpeditionDungeonId = lastDungeonId;
				}
			}
			//reward
			if(getRewardDungeonIds != null)
			{
				count = getRewardDungeonIds.Count;
				int rewardId;
				for(int i = 0;i<count;i++)
				{
					rewardId = getRewardDungeonIds[i];
					if(DungeonInfoDictionary.ContainsKey(rewardId))
					{
						DungeonInfoDictionary[rewardId].SetGetReward(true);
					}
				}
			}

			if(onUpdateResetSucDelegate!=null)
			{
				onUpdateResetSucDelegate();
			}
		}
		public ExpeditionDungeonInfo GetDungeonInfo(int id)
		{
			ExpeditionDungeonInfo info = DungeonInfoDictionary.TryGet(id);
			return info;
		}
		public List<ExpeditionDungeonInfo> GetDungeonInfoListByChapter(int chapterId)
		{
			List<ExpeditionDungeonInfo> infoList = DungeonInfoDictionary.GetValues();
			List<ExpeditionDungeonInfo> infoTempList = new List<ExpeditionDungeonInfo>();

			int count = infoList.Count;
			ExpeditionDungeonInfo data;
			for(int i = 0;i<count;i++)
			{
				data = infoList[i];
				if(data.data.chapter == chapterId)
				{
					infoTempList.Add(data);
				}
			}
			return infoTempList;
		}
		public List<ExpeditionDungeonInfo> GetCurrentDungeonInfoListByChapter()
		{
			return GetDungeonInfoListByChapter(CurrentExpeditionChapterId); 
		}
		public string GetCurrentMapByChapter()
		{
			return "ui/Expedition/map/chapter1";
		}
		#region update by server
		public void UpdateGetRewardSuccessByProtocol()
		{

			if(onUpdateGetRewardSucDelegate!=null)
			{
				onUpdateGetRewardSucDelegate();
			}
		}
		public void UpdateResetSuccessByProtocol()
		{
			if(onUpdateResetSucDelegate!=null)
			{
				onUpdateResetSucDelegate();
			}
		}
		public void UpdateExpeditionFightOverByProtocol()
		{
			if(onExpeditionFightOverDelegate!= null)
			{
				onExpeditionFightOverDelegate();
			}
		}
		#endregion
	}
}

