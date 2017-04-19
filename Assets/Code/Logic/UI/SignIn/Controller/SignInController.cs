using UnityEngine;
using System.Collections;
using Logic.Protocol.Model;
using Logic.UI.SignIn.Model;

namespace Logic.UI.SignIn.Controller
{
	public class SignInController : SingletonMono<SignInController>
	{
		
		void Awake()
		{
			instance = this;
		}
		void Start ()
		{
			Observers.Facade.Instance.RegisterObserver(((int)MSG.SignInResp).ToString(),LOBBY2CLIENT_SignInResp_handler);
			
		}
		
		void OnDestroy ()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver(((int)MSG.SignInResp).ToString(),LOBBY2CLIENT_SignInResp_handler);
			}
		}

		public void CLIENT2LOBBY_SignInReq()
		{
			SignInReq req = new SignInReq();
			Protocol.ProtocolProxy.instance.SendProtocol(req);
		}


		#region server
		//响应签到(S->C)
		private bool LOBBY2CLIENT_SignInResp_handler(Observers.Interfaces.INotification note)
		{
			SignInResp resp = note.Body as SignInResp;
			SignInProxy.instance.SetSignInDataByProtocol(resp.lastSignInNo,resp.isSignIn);
            return true;
        }
        #endregion
	}
}

