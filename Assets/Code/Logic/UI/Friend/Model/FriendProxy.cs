using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Protocol.Model;
using Logic.Game.Model;
using Logic.FunctionOpen.Model;
using Logic.Enums;
using Logic.UI.RedPoint.Model;
using Logic.UI.Friend.Controller;
using LuaInterface;

namespace Logic.UI.Friend.Model
{
	public class FriendProxy : SingletonMono<FriendProxy> {
		


		public System.Action RefreshAllDelegate;
		public System.Action<int> InitCompleteRefreshDelegate;
		public System.Action<int> GetFriendPveActionRefreshDelegate;//int:行动力数量
		public System.Action<int> RefreshOneDelegate;
		public System.Action<List<GameResData>> GetFriendGiftBagRefreshDelegate;
		public System.Action<int > LookupFriendTeamDelegate;

		private LuaTable _friendModelLua;
		public LuaTable FriendModelLua
		{
			get{
				if (_friendModelLua == null)
				{
					_friendModelLua = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","friend_model")[0];
				}
				return _friendModelLua;
			}
		}

		private Dictionary<int,Dictionary<int,FriendInfo>> _FriendDic = new Dictionary<int, Dictionary<int, FriendInfo>>();
		public Dictionary<int,Dictionary<int,FriendInfo>> FriendDic
		{
			get
			{
				return _FriendDic;
			}
		}

		public Dictionary<int,FriendInfo> myFriendListDic
		{
			get
			{
				return _FriendDic[1];
			}
		}
		public Dictionary<int,FriendInfo> recommendFriendListDic
		{
			get
			{
				return _FriendDic[2];
			}
		}
		public Dictionary<int,FriendInfo> requestFriendListDic
		{
			get
			{
				return _FriendDic[3];
			}
		}
		///当前领取的体力值
		public int alreadyGetPveActionCount;
		///领取体力最大值
		public int maxGetPveActionCount 
		{
			get
			{
				return GlobalData.GetGlobalData().friend_pveaction_add_max;
			}
		}
		///当前累计次数
		public int getPveActionTotalTimes;
		///累计N次数获得礼包
		public int getRewardNeedTimes 
		{
			get
			{
				return GlobalData.GetGlobalData().friendGiftNeedTimes;
			}
		}
		///好友数量
		public int maxMyFriendCount = 1;

		void Awake()
		{
			instance = this;
			_FriendDic.Add(1,new Dictionary<int, FriendInfo>());
			_FriendDic.Add(2,new Dictionary<int, FriendInfo>());
			_FriendDic.Add(3,new Dictionary<int, FriendInfo>());
			Observers.Facade.Instance.RegisterObserver(Logic.UI.Main.View.MainView.PREFAB_PATH,Observer_MainView_handler);
		}
		void OnDestroy()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver(Logic.UI.Main.View.MainView.PREFAB_PATH,Observer_MainView_handler);
			}
		}
		public void Clear()
		{
			foreach(var value in _FriendDic)
			{
				value.Value.Clear();
			}
			FriendModelLua.GetLuaFunction("Clear").Call();
		}
		public List<FriendInfo> GetFriendListByType(int funcId)
		{
			if(!_FriendDic.ContainsKey(funcId))
				return null;
			Dictionary<int,FriendInfo> friendDic = _FriendDic[funcId];
			List<FriendInfo> friendList = friendDic.GetValues();
			friendList.Sort(FriendSort);
			return friendList;
		}
		public bool isFriend(int id)
		{
			return _FriendDic[1].ContainsKey(id);
		}
		private static int FriendSort(FriendInfo a,FriendInfo b)
		{
			int aGetReward = a.isGetPveAction ? 1 : 5;
			int bGetReward = b.isGetPveAction ? 1 : 5;
			if(aGetReward != bGetReward )
				return bGetReward-aGetReward;

			int aDonate = a.isDonate ? 1 : 5;
			int bDonate = b.isDonate ? 1 : 5;
			if(aDonate != bDonate)
				return bDonate - aDonate;
			if(a.level != b.level)
				return b.level-a.level;
			if(b.power != a.power)
				return b.power - a.power;

			return (int)(b.lastLoginTime-a.lastLoginTime);
		}

		public void AddAllFriendList(int funcId,List<FriendProtoData> dataList)
		{
			if(!_FriendDic.ContainsKey(funcId))
				return;
			_FriendDic[funcId].Clear();
			FriendModelLua.GetLuaFunction("ClearFriendByType").Call();
			FriendProtoData data;
			for(int i = 0,count = dataList.Count;i<count;i++)
			{
				data = dataList[i];
				AddFriend(funcId,data);
			}
		}

		public void AddFriend(int funcId,FriendProtoData data)
		{
			if(!_FriendDic.ContainsKey(funcId))
				return;
			if(data == null)
				return;
			if(_FriendDic[funcId].ContainsKey(data.id))
			   return;
			FriendInfo info = new FriendInfo(data);
			_FriendDic[funcId].Add(data.id,info);
			FriendModelLua.GetLuaFunction("AddFriendFromCSharp").Call(funcId,info);

		}
		public void AddFriend(int funcId,FriendInfo info)
		{
			if(!_FriendDic.ContainsKey(funcId))
				return ;
			if(info == null)
				return ;
			if(_FriendDic[funcId].ContainsKey(info.id))
				return ;
			_FriendDic[funcId].Add(info.id,info);
			FriendModelLua.GetLuaFunction("AddFriendFromCSharp").Call(funcId,info);
		}
		public void RemoveFriend(int funcId,int id)
		{
			if(_FriendDic.ContainsKey(funcId)){
				_FriendDic[funcId].Remove(id);
				FriendModelLua.GetLuaFunction("RemoveFriend").Call(funcId,id);
			}

		}
		#region red point tip
		private bool _newFriendListComing = false;
		private bool _newFriendRequestComing = false;
		public bool NewFriendListComing
		{
			set
			{
				bool old = _newFriendListComing;
				_newFriendListComing = value;
				if(old && !value)
				{
					RedPointProxy.instance.Refresh();
				}
			}
			get
			{
				return _newFriendListComing;
			}
		}
		public bool NewFriendRequestComing
		{
			set
			{
				bool old = _newFriendRequestComing;
				_newFriendRequestComing = value;
				if(old && !value)
				{
					RedPointProxy.instance.Refresh();
				}
			}
			get
			{
				return _newFriendRequestComing;
			}
		}
		public int GetNewFriendListCountByRedPoint()
		{
			if(!FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.MainView_Friend))
				return 0;
			if(_newFriendListComing)
				return 1;
			return 0;
		}
		public int GetNewFriendRequestCountByRedPoint()
		{
			if(!FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.MainView_Friend))
				return 0;
			if(_newFriendRequestComing)
				return 1;
			return 0;
		}
		#endregion
		#region refresh by server
		public void RefreshAllByProtocol()
		{
			if(RefreshAllDelegate!= null)
				RefreshAllDelegate();
		}
		public void InitCompleteRefreshByProtocol(int funcId)
		{
			if(InitCompleteRefreshDelegate!=null)
				InitCompleteRefreshDelegate(funcId);
		}
		public void RefreshOneByProtocol(int id)
		{
			if(RefreshOneDelegate!= null)
				RefreshOneDelegate(id);
		}
		public void GetFriendPveActionRefreshByProtocol(List<int> friendIdList)
		{
			if(GetFriendPveActionRefreshDelegate!= null)
			{
				int count = friendIdList.Count;
				alreadyGetPveActionCount += count;
				getPveActionTotalTimes += count;
				GetFriendPveActionRefreshDelegate(count);

			}
				
		}
		public void GetFriendGiftBagRefreshByProtocol()
		{
			if(GetFriendGiftBagRefreshDelegate!= null)
			{
				List<GameResData> dataList = new List<GameResData>();

				dataList.Add(new GameResData(GlobalData.GetGlobalData().friend_box_id));

				GetFriendGiftBagRefreshDelegate(dataList);
			}

		}
		public void LookupFriendTeamByProtocol(int id)
		{
			if(LookupFriendTeamDelegate!=null)
				LookupFriendTeamDelegate(id);
		}
		private bool Observer_MainView_handler(Observers.Interfaces.INotification note)
		{
			if("open".Equals( note.Type))
			{
				if (_FriendDic[1].Count == 0 && FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.MainView_Friend))
				{
					Friend.Controller.FriendController.instance.CLIENT2LOBBY_FriendListReq_REQ();
				}
			}
			return true;
		}
		#endregion
		
	}
}

