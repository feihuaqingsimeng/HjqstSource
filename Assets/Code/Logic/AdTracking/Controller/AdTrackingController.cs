using UnityEngine;
using System.Collections;

namespace Logic.AdTracking.Controller
{
	public class AdTrackingController : SingletonPersistent<AdTrackingController>
	{
		public void InitAdTracking ()
		{
			PlatformProxy.instance.InitAdTracking();
			Debugger.Log("AdTrackingController.InitAdTracking()");
		}
		
		public void AdTracking_OnRegister (string accountID)
		{
			PlatformProxy.instance.AdTracking_OnRegister(accountID);
			Debugger.Log(string.Format("AdTrackingController.AdTracking_OnRegister({0})", accountID));
		}
		
		public void AdTracking_OnLogin (string accountID)
		{
			PlatformProxy.instance.AdTracking_OnLogin(accountID);
			Debugger.Log(string.Format("AdTrackingController.AdTracking_OnLogin({0})", accountID));
		}
		
		public void AdTracking_OnPay (string accountID, string orderID, int amount, string currencyType, string payType)
		{
			PlatformProxy.instance.AdTracking_OnPay(accountID, orderID, amount, currencyType, payType);
			Debugger.Log("AdTrackingController.AdTracking_OnPay({0}{1}{2}{3}{4}{5})", accountID, orderID, amount, currencyType, payType);
		}
	}
}