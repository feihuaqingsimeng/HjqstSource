using UnityEngine;

namespace Logic.UI.Login.Model
{
    public class LoginProxy : SingletonMono<LoginProxy>
    {
        public string cachedAccount;//渠道账号
        public string cachedPassword;
        public string cachedToken;//渠道token
        public int cachedPlatformId;//渠道id
		public string cachedU8userID;
        void Awake()
        {
            instance = this;
			#if UNITY_IOS
			cachedPlatformId = (int)Enums.PlatformType.iOS;
			#endif
        }
        public void Clear()
        {
            cachedAccount = string.Empty;
            cachedPassword = string.Empty;
        }
    }
}
