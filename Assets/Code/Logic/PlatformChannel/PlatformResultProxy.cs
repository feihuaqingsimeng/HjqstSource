using UnityEngine;
using System.Collections;
using Logic.Game;

public class PlatformResultProxy : SingletonMono<PlatformResultProxy>
{
	public System.Action<string, string, string, int> onLoginSuccessDelegate;
    public System.Action onPlatformLogoutSuccessDelegate;
    public System.Action onPlatformInitOkDelegate;
    public System.Action onPlatformShunWangInitOkDelegate;
    public System.Action<string> onPlatformPaySuccess;

    


   
    void Start()
    {
        instance = this;
    }

    public void PlatformLoginSuccess(string s)
    {
        Debugger.LogError("PlatformLoginSuccess:{0}", s);
        string[] param = s.Split(',');
        if (param.Length == 3)
        {
            GameDataCenter.instance.sdkId = param[0];
            GameDataCenter.instance.u8id = "";
            GameDataCenter.instance.token = param[1];
            GameDataCenter.instance.platformId = param[2].ToInt32();
        }

        else if (param.Length == 4)
        {
            GameDataCenter.instance.sdkId = param[0];
            GameDataCenter.instance.u8id = param[1];
            GameDataCenter.instance.token = param[2];
            GameDataCenter.instance.platformId = param[3].ToInt32();

        }
        if (onLoginSuccessDelegate != null)
		{
            onLoginSuccessDelegate(GameDataCenter.instance.sdkId, GameDataCenter.instance.u8id, GameDataCenter.instance.token, GameDataCenter.instance.platformId);
        }
        GameDataCenter.instance.isSdkLogin = true;
    }

    public void PlatformPaySuccess(string s)
    {
        Debugger.LogError("PlatformPaySuccess:{0}", s);
        if (onPlatformPaySuccess != null)
            onPlatformPaySuccess(s);
    }
    public void PlatformLogoutSuccess(string s)
    {
        GameDataCenter.instance.isSdkLogin = false;
        Debugger.LogError("PlatformLogoutSuccess:{0}", s);
        if (onPlatformLogoutSuccessDelegate != null)
            onPlatformLogoutSuccessDelegate();
    }
    public void PlatformInitOk()
    {
        if (onPlatformInitOkDelegate != null)
            onPlatformInitOkDelegate();
    }
    public void PlatformShunWangInitOk()
    {
        if(onPlatformShunWangInitOkDelegate != null)
        {
            onPlatformShunWangInitOkDelegate();
        }
    }
}
