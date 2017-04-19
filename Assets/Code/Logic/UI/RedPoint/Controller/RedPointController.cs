using UnityEngine;
using System.Collections;
using Logic.Protocol.Model;
using Logic.UI.RedPoint.Model;

namespace Logic.UI.RedPoint.Controller
{
	public class RedPointController : SingletonMono<RedPointController> 
	{
		void Awake()
		{
			instance = this;
		}
		void Start ()
		{
			Observers.Facade.Instance.RegisterObserver(((int)MSG.RedPointPromptResp ).ToString(),LOBBY2CLIENT_RedPointPromptResp_handler);
		
			
		}
		
		void OnDestroy ()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver(((int)MSG.RedPointPromptResp).ToString(),LOBBY2CLIENT_RedPointPromptResp_handler);
			}
		}
		#region server
		//红点提示(S->C)
		private bool LOBBY2CLIENT_RedPointPromptResp_handler(Observers.Interfaces.INotification note)
		{
			RedPointPromptResp resp = note.Body as RedPointPromptResp;
			Debugger.Log(string.Format("红点提示啦：funcid:{0},sub:{1}",resp.funcId ,resp.subFuncId));
			RedPointProxy.instance.RefreshByProtocol(resp.funcId,resp.subFuncId);
			return true;
		}
		#endregion
		
	}
}

