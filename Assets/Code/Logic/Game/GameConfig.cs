//#define banshu
#define develop
//#define beta
//#define beta2
//#define test
//#define performanceTest
using UnityEngine;
using System.Collections;
namespace Logic.Game
{
    public class GameConfig : SingletonMono<GameConfig>
    {
        public static bool ableDebug
        {
            get
            {
#if UNITY_EDITOR
                return true;
#endif
                bool result = Logic.Game.Model.InfoData.GetInfoData().ableDebug;
                if (!result)
                    Debugger.Log("debug was disable !");
                return result;
            }
        }

        public static bool outLog
        {
            get
            {
#if UNITY_EDITOR
                return true;
#endif
                return Logic.Game.Model.InfoData.GetInfoData().outLog;
            }
        }

        public static bool assetBundle
        {
            get
            {
#if UNITY_EDITOR
                return GameConfig.instance.loadAssetBundle;
#endif
                return Logic.Game.Model.InfoData.GetInfoData().assetBundle;
            }
        }
#if UNITY_EDITOR
        public bool skipLaunchProcess;
        public bool showEnemySkillButton;
        public bool loadAssetBundle;
        public bool csvRemote;
        public bool luaRemote;
#endif
        public bool loadCSVRemote
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    return false;
                return GameConfig.instance.csvRemote;
#endif
#if banshu
                return false;
#endif
                return true;
            }
        }

        public bool loadLuaRemote
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    return false;
                return GameConfig.instance.luaRemote;
#endif
#if banshu
                return false;
#endif
                return true;
            }
        }
        //单方列距离与双方阵容距离之比2/5.5
        public const float timePercent = 0.13f;
#if UNITY_EDITOR
        public string innerGameServerHost
        {
            get;
            set;
        }
        public int innerGameServerPort
        {
            get;
            set;
        }
        public string assetbundleVersion = "1.0.1";
#endif
        public static string gameServerHost = "120.132.58.75";
        public static int gameServerPort = 90;
        public string url
        {
            get
            {
#if UNITY_EDITOR_WIN
                return "http://192.168.3.103:8080/ABs/" + assetbundleVersion;
#elif UNITY_EDITOR_OSX
                return "http://192.168.2.110:8080/ABs/" + assetbundleVersion;
#endif
#if UNITY_ANDROID
                string result = PlatformProxy.instance.GetResUrl();
                if (!string.IsNullOrEmpty(result))
                    return result;
#endif
#if develop
				return "http://cdn.huanji.yileweb.com/huanji/channel_official";
                //return "http://120.132.58.75:8080/develop_468";
#elif performanceTest
				return "http://120.132.58.75:8080/performanceTest";
#elif beta
                return "http://cdn.huanji.yileweb.com/huanji/beta";
#elif beta2
                return "http://cdn.huanji.yileweb.com/huanji/beta2";
#elif test
                return "http://120.132.58.75:8080/test";
#else
                return "http://120.132.58.75:8080";
#endif
            }
        }

        public string staticUrl
        {
            get
            {
#if UNITY_EDITOR_WIN
                return "http://192.168.3.103:8080/ABs/" + assetbundleVersion;
#elif UNITY_EDITOR_OSX
                return "http://192.168.2.110:8080/ABs/" + assetbundleVersion;
#elif develop
#if UNITY_ANDROID

                return  PlatformProxy.instance.GetResUrl();
#elif UNITY_IOS
				return "http://cdn.huanji.yileweb.com/huanji/channel_official";
#endif
#elif beta
                return "http://120.132.58.75:8080/beta";
#else
                return "http://120.132.58.75:8080";
#endif
            }
        }

        private bool _playMovie = true;
        public bool playMovie
        {
            get { return _playMovie; }
            set { _playMovie = value; }
        }

        private const string _gameVersion = "v1.2.1";
        public string gameVesrion
        {
            get
            {
#if UNITY_EDITOR
                return _gameVersion;
#endif
#if UNITY_ANDROID
                return PlatformProxy.instance.GetGameVersion();
#endif
                return _gameVersion;
            }
        }
        public string csvVersion { get; set; }
        public string luaVersion { get; set; }

        private string _cdnVersion = string.Empty;
        public string cdnVersion
        {
            get
            {
                return _cdnVersion;
            }
            set
            {
                Debugger.Log(value);
                _cdnVersion = value;
            }
        }

        public string param
        {
            get
            {
                if (string.IsNullOrEmpty(cdnVersion))
                    return string.Empty;
                return string.Format("?v={0}", cdnVersion);
            }
        }

        public static string GetServerListUrl()
        {
#if UNITY_ANDROID
            return PayCallbackData.GetPayCallbackDataByID(300).callback;
#elif UNITY_IOS
			return PayCallbackData.GetPayCallbackDataByID(200).callback;
#elif UNITY_EDITOR_WIN
			return PayCallbackData.GetPayCallbackDataByID(400).callback;
#endif
            return "error";
        }


        public const string getServerListKey = "6*M6657*=V]kZ4Z($5ftlEzmB%xN+(";
        public static string encryptKey = "happyday";
        [System.NonSerialized]
        public bool encrypt = false;
        [System.NonSerialized]
        public string aesEncryptKey = "1234567812345678";
        public static int excursion = 10;
        public static string TALKING_DATA_APP_ID = "CE0A646E4864F157FC38C15CCB2EB9AE";
        public static string CHANNEL_ID
        {
            get
            {
                int platformId = PlatformProxy.instance.GetPlatformId();
                Enums.PlatformType platformType = (Enums.PlatformType)platformId;
                return platformType.ToString();
            }
        }
        void Awake()
        {
            instance = this;
        }
        /*
        void OnGUI()
        {
#if UNITY_EDITOR
            _version = UnityEditor.PlayerSettings.bundleVersion;
#endif
            GUI.Label(new Rect(0, 20, 150, 20), "version:" + _version);
#if UNITY_EDITOR
            GUI.Label(new Rect(0, 40, 150, 20), "host:" + innerGameServerHost);
            GUI.Label(new Rect(0, 60, 150, 20), "port:" + innerGameServerPort);
#else            
            GUI.Label(new Rect(0, 40, 150, 20), "host:" + gameServerHost);
            GUI.Label(new Rect(0, 60, 150, 20), "port:" + gameServerPort);
#endif
        }*/
    }
}