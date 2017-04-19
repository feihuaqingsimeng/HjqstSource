using UnityEngine;
using System.Collections;
using Logic.Enums;
using Logic.UI.Mail.Model;
using Logic.UI.Friend.Model;
using LuaInterface;

namespace Logic.UI.RedPoint.Model
{
	public class RedPointProxy : SingletonMono<RedPointProxy> 
	{

		public System.Action OnRefreshDelegate;
		//刷新特定的红点type
		public System.Action<int> OnRefreshSpecificDelegate;
		void Awake()
		{
			instance = this;
			Observers.Facade.Instance.RegisterObserver("OnRedPointChange", OnRedPointChange);
		}

		void OnDestroy ()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver("OnRedPointChange", OnRedPointChange);
			}
		}

		public void Refresh()
		{
			if(OnRefreshDelegate!= null)
			{
				OnRefreshDelegate();
			}
		}
		public void RefreshSpecific(int type)
		{
			if (OnRefreshSpecificDelegate!= null)
				OnRefreshSpecificDelegate(type);
		}
		public void RefreshByProtocol(int funcId,int subFuncId)
		{
			//蛋疼的策划非得不统一客户端、服务端id
			FunctionOpenType type = (FunctionOpenType) (funcId/10*10);
			int sub = subFuncId-funcId;
			switch(type)
			{
			case FunctionOpenType.MainView_Mail:
				MailProxy.instance.hasNewMailComing = true;

				break;
			case FunctionOpenType.MainView_Friend:
				if(sub == 1)
					FriendProxy.instance.NewFriendListComing = true;
				else if(sub == 3)
					FriendProxy.instance.NewFriendRequestComing = true;
				break;
			default:
				{
					LuaTable model = (LuaTable)LuaInterface.LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","red_point_model")[0];
					model.GetLuaFunction("UpdateRedPointByProtocol").Call((int)type,sub);
				}
				break;
			}
			RefreshSpecific((int)type);
		}
		public void RefreshDelay(float delayTime = 0.2f)
		{
			CancelInvoke("Refresh");
			Invoke("Refresh",delayTime);
		}

		public bool OnRedPointChange (Observers.Interfaces.INotification note)
		{
			if (note.Body == null){
				Refresh();
			}else
			{
				int type = note.Body.ToString().ToInt32();

				RefreshSpecific(type);

			}
				
			return true;
		}
	}
}

