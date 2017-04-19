using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.GameTime.Controller;
using Logic.UI.SignIn.View;
using Logic.Game.Model;
using LuaInterface;

namespace Logic.UI.SignIn.Model
{
	public class SignInProxy : SingletonMono<SignInProxy>
	{
		
		void Awake()
		{
			instance = this;
		}
		void Start()
		{
			Observers.Facade.Instance.RegisterObserver(Logic.UI.Main.View.MainView.PREFAB_PATH,Observer_MainView_handler);
		}
		void OnDestroy()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver(Logic.UI.Main.View.MainView.PREFAB_PATH,Observer_MainView_handler);
            }
        }
        public System.Action<int> RefreshSignDelegate;
		//public System.Action  UpdateSignedTodayDelegate;//今日点击签到刷新

		public int lastSignInId;//上次签到id
		public int curSignInId;//当前可签到id
		public bool isSignInToday;//今天是否已签到

		private List<SignInInfo> _SignInList = new List<SignInInfo>();
		public List<SignInInfo> SignInList
		{
			get
			{
				if(_SignInList.Count == 0)
				{
					Dictionary<int, SignInData> signDataDic = SignInData.GetSignInDatas();
					foreach(var data in signDataDic)
					{
						_SignInList.Add(new SignInInfo(data.Key,false));
					}
				}
				return _SignInList;
			}
		}

		public void SetSignInDataByProtocol(int lastId,bool isSignToday)
		{

			this.lastSignInId = lastId;
			this.isSignInToday = isSignToday;

			SignInInfo lastInfo = SignInList[SignInList.Count-1];
			if(lastSignInId == lastInfo.id && !isSignInToday)
				lastSignInId = 0;
			curSignInId = isSignToday ? lastSignInId : lastSignInId + 1;
			SignInInfo info;
			for(int i = 0,count = SignInList.Count;i<count;i++)
			{
				info = SignInList[i];
				info.isSign = info.id <= lastSignInId;
			}
			if(RefreshSignDelegate != null)
			{
				RefreshSignDelegate(lastId);
			}
//			if(isSignToday && UpdateSignedTodayDelegate!= null)
//				UpdateSignedTodayDelegate();
			LuaTable gameModelLua  = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","sign_in_model")[0];
			gameModelLua.GetLuaFunction("SetIsSign").Call(this.lastSignInId,this.curSignInId,this.isSignInToday);
		}

		private bool Observer_MainView_handler(Observers.Interfaces.INotification note)
		{
			if("open".Equals( note.Type))
			{
				string mark = string.Format("openSignDay{0}",GameProxy.instance.AccountId);
				int day = PlayerPrefs.GetInt(mark);
				int nowDay = TimeController.instance.ServerTime.DayOfYear;
				bool isOpen = FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(Logic.Enums.FunctionOpenType.MainView_SignIn);
				if( isOpen && nowDay != day && !isSignInToday)
				{
					Debugger.Log(string.Format("{0}sign is open :{1},preDay:{2},nowDay:{3}",GameProxy.instance.AccountId,isOpen,day,nowDay));
					SignInView.Open();
				}
			}
            return true;
        }
	}
}

