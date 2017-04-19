
namespace Common.ResMgr
{
    public enum EResPlatform
    {
        unknown = 0,
        standalonewindows = 1,
        iOS = 2,
        android = 3,
        webplayer = 4,
    }

    public static class ResConf
    {
        //E_RES_PLATFORM_NAME 只是为了方便在editor中显示，而且又定义到了一块所以放到了这边
#if UNITY_IPHONE
#if UNITY_EDITOR
        public const string E_RES_PLATFORM_NAME = "(for iOS)";
#endif
        public static EResPlatform eResPlatform = EResPlatform.iOS;
#elif UNITY_ANDROID
#if UNITY_EDITOR
        public const string E_RES_PLATFORM_NAME = "(for android)";
#endif
        public static EResPlatform eResPlatform = EResPlatform.android;
#elif UNITY_STANDALONE_WIN
#if UNITY_EDITOR
        public const string E_RES_PLATFORM_NAME = "(for standalonewindows)";
#endif
        public static EResPlatform eResPlatform = EResPlatform.standalonewindows;
#elif UNITY_WEBPLAYER
#if UNITY_EDITOR
        public const string E_RES_PLATFORM_NAME = "(for webplayer)";
#endif
        public static EResPlatform eResPlatform = EResPlatform.webplayer;
#else
#if UNITY_EDITOR
        public const string E_RES_PLATFORM_NAME = "(for unknown)";
#endif
        public static EResPlatform eResPlatform = EResPlatform.unknown;
#endif
    }
}
