using UnityEngine;
using Logic.Protocol.Model;
using Logic.VIP.Model;

namespace Logic.VIP.Controller
{
	public class VIPController : SingletonMono<VIPController>
	{
		private int _lastDrawBenefitsVIPLevel = -1;

		void Awake ()
		{
			instance = this;
		}

		/*
		void Start ()
		{
			Observers.Facade.Instance.RegisterObserver(((int)MSG.VipInfoResp).ToString(), S2C_VIPInfoResp);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.VipGiftBagResp).ToString(), S2C_VIPGiftBagResp);
		}

		void OnDestroy ()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver(((int)MSG.VipInfoResp).ToString(), S2C_VIPInfoResp);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.VipGiftBagResp).ToString(), S2C_VIPGiftBagResp);
			}
		}

		public void C2S_VIPInfoReq ()
		{
			VipInfoReq vipInfoReq = new VipInfoReq ();
			Logic.Protocol.ProtocolProxy.instance.SendProtocol(vipInfoReq);
		}

		public bool S2C_VIPInfoResp (Observers.Interfaces.INotification note)
		{
			VipInfoResp vipInfoResp = note.Body as VipInfoResp;
			VIPProxy.instance.UpdateVIPInfo(vipInfoResp.dailyGiftIsGet,vipInfoResp.totalRecharge, vipInfoResp.getGiftBagLvList);
			return true;
		}

		public void C2S_VIPGiftBagReq (int vipLevel)
		{
			VipGiftBagReq vipGiftBagReq = new VipGiftBagReq();
			vipGiftBagReq.vipLv = vipLevel;
			_lastDrawBenefitsVIPLevel = vipLevel;
			Logic.Protocol.ProtocolProxy.instance.SendProtocol(vipGiftBagReq);
		}

		public bool S2C_VIPGiftBagResp (Observers.Interfaces.INotification note)
		{
            LuaInterface.LuaScriptMgr.Instance.CallLuaFunctionVoid("ActivityRspVipGiftBag");
            VipGiftBagResp vipGiftBagResp = note.Body as VipGiftBagResp;
            if (VIPProxy.instance != null)
                VIPProxy.instance.onDrawVIPBenefitsSuccessDelegate(_lastDrawBenefitsVIPLevel);
			return true;
		}
		*/
	}
}
