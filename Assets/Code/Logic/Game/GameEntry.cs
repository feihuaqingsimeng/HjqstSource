using UnityEngine;
using System.Collections.Generic;
using Logic.Game.Controller;
namespace Logic.Game
{
    public class GameEntry : SingletonMono<GameEntry>
    {
        void Awake()
        {
            instance = this;
            //UnityEngine.GameObject.DontDestroyOnLoad(transform.parent.gameObject);
            InitBugly();
        }

        void InitBugly()
        {
#if UNITY_IPHONE || UNITY_IOS
//			// 开启SDK的⽇志打印，发布版本请务必关闭
//			BuglyAgent.ConfigDebugMode(true);
//			// 注册⽇志回调，替换使⽤ 'Application.RegisterLogCallback(Application.LogCallback)'注册⽇志回调的⽅式
//			//BuglyAgent.RegisterLogCallback(Application.logMessageReceived);
//
//            BuglyAgent.InitWithAppId("900007872");
//
//			// 如果你确认已在对应的iOS⼯程或Android⼯程中初始化SDK，那么在脚本中只需启动C#异常捕获上报功能即可
//			BuglyAgent.EnableExceptionHandler();
#elif UNITY_ANDROID
			// 开启SDK的⽇志打印，发布版本请务必关闭
			BuglyAgent.ConfigDebugMode(true);
			// 注册⽇志回调，替换使⽤ 'Application.RegisterLogCallback(Application.LogCallback)'注册⽇志回调的⽅式
			//BuglyAgent.RegisterLogCallback(Application.logMessageReceived);

            BuglyAgent.InitWithAppId("900008181");

			// 如果你确认已在对应的iOS⼯程或Android⼯程中初始化SDK，那么在脚本中只需启动C#异常捕获上报功能即可
			BuglyAgent.EnableExceptionHandler();
#endif
        }

        void Start()
        {
            Startup();
        }

        void Startup()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            //GameSetting.instance.speedMode = GameSpeedMode.Normal;
            GameSetting.instance.frameType = GameFrameType.UI;
            GameStart();
        }

        void CopyStreamingAssets2persistentDataPath()
        {
            //#if UNITY_ANDROID && !UNITY_EDITOR
            //            if (!Common.ResMgr.ResUtil.ExistConfigMD5InLocal())
            //            {
            //                string sourceDir = string.Format(@"{0}/{1}", Application.streamingAssetsPath, Common.ResMgr.ResConf.eResPlatform);
            //                string targetDir = string.Format(@"{0}/{1}", Application.persistentDataPath, Common.ResMgr.ResConf.eResPlatform);
            //                Debugger.Log(sourceDir);
            //                Debugger.Log(targetDir);
            //                Common.ResMgr.ResUtil.CopyFiles(sourceDir, targetDir, "*.md5");
            //                Common.ResMgr.ResUtil.CopyFiles(sourceDir, targetDir, "*.txt");
            //                Common.ResMgr.ResUtil.CopyFiles(sourceDir, targetDir, "*.csv");
            //            }
            //#endif
            GameStart();
        }

        void GameStart()
        {
            gameObject.AddComponent<GameController>();
            Debugger.Log("开始游戏");
        }
    }
}