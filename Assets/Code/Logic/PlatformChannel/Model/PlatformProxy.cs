using UnityEngine;
using System.Collections;
using LitJson;
using System.Runtime.InteropServices;
public class PlatformProxy : MonoBehaviour
{
    private static PlatformProxy _instance;
    public static PlatformProxy instance
    {
        get
        {
            return _instance;
        }
    }

#if UNITY_IOS
    private const string _productIdPrefix = "com.dowan.hjqst.shunwang_";
#endif
    public bool isU8
    {
        get
        {
            return GetIsU8();
        }
    }
    void Awake()
    {
        _instance = this;
    }

    #region android platform
#if UNITY_ANDROID
    private AndroidJavaClass _ndroidJavaClass = null;
    private AndroidJavaObject _androidJavaObj;
#endif
    private void CallActivityFunc(string functionName, params string[] param)
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            Debugger.Log("不是android 平台----------------");
            return;
        }
        try
        {
#if UNITY_ANDROID
            if (_ndroidJavaClass == null)
            {
                _ndroidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            }
            _androidJavaObj = _ndroidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
            _androidJavaObj.Call("runOnUiThread", new AndroidJavaRunnable(() => { _androidJavaObj.Call(functionName, param); }));
#endif
        }
        catch (UnityException e)
        {
            Debugger.LogError(e.StackTrace);
        }
    }

	void ProcessAuthentication (bool success)
	{
		if (success)
		{
			string loginString = string.Format("{0},{1},{2}", Social.localUser.id, "", (int)Logic.Enums.PlatformType.iOS);
			PlatformResultProxy.instance.PlatformLoginSuccess(loginString);
			Debugger.Log("Authenticated.");
		}
		else
			Debugger.Log("Failed to authenticate.");
		Debugger.Log("=====[ProcessAuthentication::success:{0}]", success.ToString());
		Debugger.Log("=====[ProcessAuthentication::UnityEngine.Social.localUser.authenticated:{0}]=====", UnityEngine.Social.localUser.authenticated.ToString());
		Debugger.Log("=====[ProcessAuthentication::UnityEngine.Social.localUser.state:{0}]=====", UnityEngine.Social.localUser.state.ToString());
	}

    public void ShowSdkLogin(string loginVerifyUrl)
    {
#if UNITY_IOS
		UnityEngine.Social.localUser.Authenticate(ProcessAuthentication);
		Debugger.Log("=====[ShowSdkLogin::UnityEngine.Social.localUser.authenticated:{0}]=====", UnityEngine.Social.localUser.authenticated.ToString());
		Debugger.Log("=====[ShowSdkLogin::UnityEngine.Social.localUser.state:{0}]=====", UnityEngine.Social.localUser.state.ToString());
#else
		CallActivityFunc("ShowSDKLogin", loginVerifyUrl);
#endif

/*
#if UNITY_IOS
		AuthenticateLocalUser();
#else
		CallActivityFunc("ShowSDKLogin", loginVerifyUrl);
#endif
*/
    }
    public void ShowSdkPay(string payJson)
    {
        Debugger.Log("json:{0}", payJson);
#if UNITY_ANDROID
        JsonData jsonData = JsonMapper.ToObject(payJson);
        CallActivityFunc("Pay", payJson);
        string orderId = (string)jsonData["id"];
        string productName = (string)jsonData["productName"];
        string priceStr = (string)jsonData["price"];
        int price = priceStr.ToInt32();
        Logic.Enums.PlatformType platformType = (Logic.Enums.PlatformType)GetPlatformId();
        Logic.TalkingData.Controller.TalkingDataController.instance.TDGAVirtualCurrencyOnChargeRequest(orderId, productName, price, "CNY", price, platformType.ToString());
#elif UNITY_IOS
        JsonData jsonData = JsonMapper.ToObject(payJson);
        string idStr = (string)jsonData["itemid"];
        int id = 0;
        int.TryParse(idStr, out id);
        if (id > 0)
        {
            string productCode = string.Format("{0}{1}", _productIdPrefix, id);
            Pay(productCode);
        }
#endif
    }

    public void showSdkLogout()
    {
        CallActivityFunc("Logout");
    }

    public void SendGameInfo(string s)
    {
        Debugger.Log("-------------------------SendGameInfo:{0}", s);
        CallActivityFunc("SendGameInfo", s);
    }

    public void InitShunWangSdk()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        CallActivityFunc("InitPlatformShunWangSdk");
#else
        PlatformResultProxy.instance.PlatformInitOk();
#endif
    }

    public void InitSdk()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        CallActivityFunc("InitPlatformSdk");
#else
		
#endif
    }

    //发送额外数据
    public void SendExternalData(string s)
    {
        Debugger.Log("SendExternalData:{0}----------------", s);
        CallActivityFunc("SendExternalData", s);
    }

    public string GetResUrl()
    {
#if UNITY_ANDROID
        try
        {
            string url = string.Empty;
            if (_ndroidJavaClass == null)
            {
                _ndroidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            }
            _androidJavaObj = _ndroidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
            url = _androidJavaObj.Call<string>("GetResUrl");
            Debugger.Log("-------------------------url:" + url);
            return url;
        }
        catch (UnityException e)
        {
            Debugger.LogError(e.StackTrace);
            return string.Empty;
        }
#endif
        return string.Empty;
    }

    public int GetPlatformId()
    {
#if UNITY_ANDROID
        if (Application.platform != RuntimePlatform.Android)
        {
            Debugger.Log("不是android 平台----------------");
            return 0;
        }

        try
        {
            if (_ndroidJavaClass == null)
            {
                _ndroidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            }
            _androidJavaObj = _ndroidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
            int id = _androidJavaObj.Call<int>("GetPlatformId");
            Debugger.Log("-------------------------GetPlatformId:" + id);
            return id;
        }
        catch (UnityException e)
        {
            Debugger.LogError(e.StackTrace);
            return 0;
        }
#elif UNITY_IOS
        return (int)Logic.Enums.PlatformType.iOS;
#endif
        return 0;
    }

    public string GetGameVersion()
    {
#if UNITY_ANDROID
        if (Application.platform != RuntimePlatform.Android)
        {
            Debugger.Log("不是android 平台----------------");
            return string.Empty;
        }

        try
        {
            if (_ndroidJavaClass == null)
            {
                _ndroidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            }
            _androidJavaObj = _ndroidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
            string gameVersion = _androidJavaObj.Call<string>("GetGameVersion");
            Debugger.Log("-------------------------GetGameVersion:" + gameVersion);
            return gameVersion;
        }
        catch (UnityException e)
        {
            Debugger.LogError(e.StackTrace);
            return string.Empty;
        }
#endif
        return string.Empty;
    }

    private bool GetIsU8()
    {
#if UNITY_ANDROID
        if (Application.platform != RuntimePlatform.Android)
        {
            Debugger.Log("不是android 平台----------------");
            return false;
        }

        try
        {
            if (_ndroidJavaClass == null)
            {
                _ndroidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            }
            _androidJavaObj = _ndroidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
            bool isU8 = _androidJavaObj.Call<bool>("GetIsU8");
            Debugger.Log("-------------------------GetIsU8:" + isU8);
            return isU8;
        }
        catch (UnityException e)
        {
            Debugger.LogError(e.StackTrace);
            return false;
        }
#endif
        return false;
    }

    public void ClearNotifications()
    {
        CallActivityFunc("ClearNotifications");
    }

    public void AddLocalNotification(string title, string content, string date, string hour, string min)
    {
        Debugger.Log("AddLocalNotification:" + date + "-" + hour + "-" + min + "," + title + "," + content);
        CallActivityFunc("AddLocalNotification", title, content, date, hour, min);
    }
    #endregion

    #region iOS platform
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void InitPayment();
    [DllImport("__Internal")]
    private static extern void IOSPayment(string productCode);
    [DllImport("__Internal")]
    private static extern void VerifySuccess();

	[DllImport("__Internal")]
	private static extern void iOS_InitAdTracking();
	[DllImport("__Internal")]
	private static extern void iOS_AdTracking_OnRegister(string accountID);
	[DllImport("__Internal")]
	private static extern void iOS_AdTracking_OnLogin(string accountID);
	[DllImport("__Internal")]
	private static extern void iOS_AdTracking_OnPay(string accountID, string orderID, int amount, string currencyType, string payType);

	[DllImport("__Internal")]
	private static extern void authenticateLocalUser ();
#endif

    public void InitIOSPayment()
    {
#if UNITY_IOS && !UNITY_EDITOR
        InitPayment();
#endif
    }

    public void Pay(string productCode)
    {
#if UNITY_IOS && !UNITY_EDITOR
        IOSPayment(productCode);
        Debugger.Log("pay :" + productCode);
#endif
    }

    public void PaymentCallback(string s)
    {
#if UNITY_IOS && !UNITY_EDITOR
        JsonData jsonData = JsonMapper.ToObject(s);
        string base64Encoding = (string)jsonData["base64Encoding"];
        string productIdentifier = (string)jsonData["productIdentifier"];
        string transactionIdentifier = (string)jsonData["transactionIdentifier"];
        string[] strs = productIdentifier.Split('_');
        int id = 0;
        if (strs.Length > 1)
        {
            id = strs[1].ToInt32();
        }

		/****** 此段为修复客户端表与app store商品ID不一致的问题所加 ******/
		if (id == 26)
		{
			id = 105;
		}
		else if (id == 27)
		{
			id = 106;
		}
		/****** 此段为修复客户端表与app store商品ID不一致的问题所加 ******/

        Debugger.Log("base64Encoding:" + base64Encoding);
        Debugger.Log("id:" + id);
        LuaInterface.LuaTable onlineGiftController = (LuaInterface.LuaTable)LuaInterface.LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl", "shop_controller")[0];
        onlineGiftController.GetLuaFunction("AppStoreVerifyReq").Call(id, base64Encoding, transactionIdentifier);
#endif
    }

    public void PaySuccess()
    {
#if UNITY_IOS && !UNITY_EDITOR
        VerifySuccess();
        Debugger.Log("VerifySuccess");
#endif
    }
    #endregion

	public void InitAdTracking ()
    {
#if UNITY_IOS && !UNITY_EDITOR
		iOS_InitAdTracking();
#endif
    }

	public void AdTracking_OnRegister (string accountID)
    {
#if UNITY_IOS && !UNITY_EDITOR
		iOS_AdTracking_OnRegister(accountID);
#endif
    }

	public void AdTracking_OnLogin (string accountID)
    {
#if UNITY_IOS && !UNITY_EDITOR
		iOS_AdTracking_OnLogin(accountID);
#endif
    }

	public void AdTracking_OnPay (string accountID, string orderID, int amount, string currencyType, string payType)
    {
#if UNITY_IOS && !UNITY_EDITOR
		iOS_AdTracking_OnPay(accountID, orderID, amount, currencyType, payType);
#endif
    }

	public void AuthenticateLocalUser ()
	{
#if UNITY_IOS && !UNITY_EDITOR
		authenticateLocalUser();
#endif
	}
}
