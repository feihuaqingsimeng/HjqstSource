using UnityEngine;
using System.Collections;
using Logic.Protocol.Model;
using Logic.UI.IllustratedHandbook.Model;
using LuaInterface;

namespace Logic.UI.IllustratedHandbook.Controller
{
	public class IllustrationController : SingletonMono<IllustrationController> 
	{
		
		void Awake()
		{
			instance = this;
		}

		void Start()
		{
			Observers.Facade.Instance.RegisterObserver(((int)MSG.IllustrationResp).ToString(),LOBBY2CLIENT_IllustrationResp_handler);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.IllustrationSynResp).ToString(),LOBBY2CLIENT_IllustrationSynResp_handler);
			Observers.Facade.Instance.RegisterObserver(MSG.IllustrationResp.ToString(),Temp_UpdateIllustrationByProtocol);
			
		}
		void OnDestroy()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver(((int)MSG.IllustrationResp).ToString(),LOBBY2CLIENT_IllustrationResp_handler);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.IllustrationSynResp).ToString(),LOBBY2CLIENT_IllustrationSynResp_handler);
				Observers.Facade.Instance.RemoveObserver(MSG.IllustrationResp.ToString(),Temp_UpdateIllustrationByProtocol);
			}
		}


		#region client to server
		public void CLIENT2LOBBY_Illustration_REQ()
		{
			//IllustrationReq req = new IllustrationReq();
			//Protocol.ProtocolProxy.instance.SendProtocol(req);
			LuaTable illusCtrl = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl","illustration_ctrl")[0];
			illusCtrl.GetLuaFunction("IllustrationReq").Call();
		}
		#endregion

		#region sever
		//响应图鉴信息(C->S)
		private bool LOBBY2CLIENT_IllustrationResp_handler(Observers.Interfaces.INotification note)
		{
			IllustrationResp resp = note.Body as IllustrationResp ;
			IllustratedHandbookProxy.instance.UpdateIllustrationList(resp.heros,true);
			IllustratedHandbookProxy.instance.InitIllustrationByProtocol();
			return true;
		}
		//同步图鉴信息(C->S)
		private bool LOBBY2CLIENT_IllustrationSynResp_handler(Observers.Interfaces.INotification note)
		{
			IllustrationSynResp resp = note.Body as IllustrationSynResp;
			IllustratedHandbookProxy.instance.UpdateIllustrationList(resp.newHeros,false);
			IllustratedHandbookProxy.instance.UpdateIllustrationByProtocol();
			return true;
		}

		private bool Temp_UpdateIllustrationByProtocol(Observers.Interfaces.INotification note)
		{
			IllustratedHandbookProxy.instance.IllustrationDictionary.Clear();
			IllustratedHandbookProxy.instance.UpdateIllustrationByProtocol();
			return true;
		}
		#endregion
	}
}

