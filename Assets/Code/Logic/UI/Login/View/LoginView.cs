using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common.Localization;
using Logic.UI.Login.Controller;
using Logic.Game.Controller;
using Logic.UI.Login.Model;
using Logic.Game;
using Common.GameTime.Controller;
using Common.Util;
using Logic.Audio.Controller;
using Common.ResMgr;

namespace Logic.UI.Login.View
{
    public class LoginView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/login/login_view";
        private static bool _showOurLogin = true;
        public static void Open()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            if (PlatformProxy.instance.GetPlatformId() != 0)
                _showOurLogin = false;
#endif
            if (_showOurLogin)
            {
                UI.UIMgr.instance.Open<LoginView>(PREFAB_PATH);
            }
            else
            {
                ServerList.View.ServerView.Open();
            }
        }
        public float showUIDelay = 2f;
        public float showUITime = 0.8f;

        #region ui components
        public RectTransform bgCoreRectTransform;
        public RectTransform bgRectTransform;

        public CanvasGroup coreCanvasGroup;

        public Text loginGameTitleText;
        public Text serverListText;
        public Text startGameText;
        public Text guestLoginText;

        public Text accountTitleText;
        public Text passwordTitletext;
        public Text gameVersionText;
        public Text configVersionText;
        public InputField nameInputField;
        public InputField passwordInputField;

        public Text registerText;
        public Text forgotPasswordtext;
        public RawImage bgRawImg;
        public string bgRawImgPath;
        #endregion

        void Awake()
        {
            Init();
        }

        void Start()
        {
            
            AudioController.instance.PlayBGMusic(AudioController.LOGIN);

            DelayFadeInUI(showUIDelay);
        }

        void Update()
        {
            Vector3 dir = Vector3.zero;
            dir.x = Input.acceleration.x;
            dir.y = Input.acceleration.y;
            if (dir.sqrMagnitude > 1)
                dir.Normalize();
            dir *= Time.deltaTime * 100;

            float idealAnchoredX = bgRectTransform.anchoredPosition.x + dir.x;
            float idealAnchoredY = bgRectTransform.anchoredPosition.y + dir.y;

            float minAnchoredX = Mathf.Min((bgCoreRectTransform.rect.width - bgRectTransform.rect.width) * 0.5f, 0);
            float maxAnchoredX = Mathf.Max((bgRectTransform.rect.width - bgCoreRectTransform.rect.width) * 0.5f, 0);
            float minAnchoredY = Mathf.Min((bgCoreRectTransform.rect.height - bgRectTransform.rect.height) * 0.5f, 0);
            float maxAnchoredY = Mathf.Max((bgRectTransform.rect.height - bgCoreRectTransform.rect.height) * 0.5f, 0);

            idealAnchoredX = Mathf.Clamp(idealAnchoredX, minAnchoredX, maxAnchoredX);
            idealAnchoredY = Mathf.Clamp(idealAnchoredY, minAnchoredY, maxAnchoredY);

            bgRectTransform.anchoredPosition = new Vector2(idealAnchoredX, idealAnchoredY);
        }

        private void Init()
        {
            coreCanvasGroup.alpha = 0;

            loginGameTitleText.text = Localization.Get("ui.login_view.login_game_title");
            serverListText.text = Localization.Get("ui.login_view.server_list");
            startGameText.text = Localization.Get("ui.login_view.start_game");
            guestLoginText.text = Localization.Get("ui.login_view.guest_login");
            accountTitleText.text = Localization.Get("ui.login_view.account_title");
            passwordTitletext.text = Localization.Get("ui.login_view.password_title");
            registerText.text = Localization.Get("ui.login_view.register");
            forgotPasswordtext.text = Localization.Get("ui.login_view.forgot_password");
            gameVersionText.text = GameConfig.instance.gameVesrion;
            configVersionText.text = GameConfig.instance.csvVersion + "&" + GameConfig.instance.luaVersion;
            nameInputField.text = PlayerPrefs.GetString("account");
            bgRawImg.texture = ResMgr.instance.Load<Texture>(bgRawImgPath);
        }

        private void DelayFadeInUI(float delay)
        {
            coreCanvasGroup.alpha = 0;
            LeanTween.delayedCall(gameObject, delay, FadeInUI);
        }

        private void FadeInUI()
        {
            LTDescr ltDescr = LeanTween.value(gameObject, 0, 1, showUITime);
            ltDescr.setIgnoreTimeScale(true);
            ltDescr.setOnUpdate(OnUpdateUIAlpha);
            StartCoroutine("ShowNoticeView");
        }
        private IEnumerator ShowNoticeView()
        {
            int day = PlayerPrefs.GetInt("tipLoginNoticeBoardDay");
            day = System.DateTime.Now.DayOfYear - day;
            if (GameDataCenter.instance.isTipLoginNotice && day > 0)
            {
				WWW www = new WWW(ResUtil.GetRemoteStaticPathByCdn("notice.txt"));
                yield return www;

                string noticeStr = www.text;
                Logic.UI.LoginNoticeBoard.View.LoginNoticeBoardView.Open(noticeStr);
                GameDataCenter.instance.isTipLoginNotice = false;
                www.Dispose();
                www = null;
            }
        }
        private void OnUpdateUIAlpha(float alpha)
        {
            coreCanvasGroup.alpha = alpha;
        }

        #region ui event handlers
        public void ClickStartGameHandler()
        {
            LoginProxy.instance.cachedAccount = nameInputField.text;
            LoginProxy.instance.cachedPassword = passwordInputField.text;
            ServerList.View.ServerView.Open();
            //LoginController.instance.Login(name, password);
        }
        public void ClickServerListBtnHandler()
        {
            //ServerList.View.ServerListView.Open();
        }
        #endregion
    }
}