using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Logic.Enums;


namespace Logic.UI.Launch.View
{
    public class LogoView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/launch/logo_view";

        public Image bgImg;
        public Image shunLogo;
        public Image ourLogo;

        public delegate void OnTimeOutDelegate();
        public OnTimeOutDelegate onTimeOutDelegate;

        private int platformId;
        public static LogoView Open()
        {
            return UIMgr.instance.Open<LogoView>(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Replace);
        }

        public void SetOnTimeOutDelegate(OnTimeOutDelegate onTimeOutDelegate)
        {
            this.onTimeOutDelegate = onTimeOutDelegate;
        }

        void Awake()
        {
            ourLogo.CrossFadeAlpha(0, 0, true);
            PlatformResultProxy.instance.onPlatformInitOkDelegate += ShunWangPlatformInitOk;
            PlatformProxy.instance.InitSdk();
        }
        void OnDestroy()
        {
            PlatformResultProxy.instance.onPlatformInitOkDelegate -= ShunWangPlatformInitOk;
        }
        void Start()
        {
            platformId = PlatformProxy.instance.GetPlatformId();
            if (platformId == (int)PlatformType.shunwang)
            {
                shunLogo.gameObject.SetActive(false);
                PlatformProxy.instance.InitShunWangSdk();
            }
            else
            {
                LeanTween.delayedCall(0.5f, FadeOutShunLogo);
            }

        }

        void FadeOutShunLogo()
        {
            shunLogo.CrossFadeAlpha(0, 1f, true);
            LeanTween.delayedCall(1.5f, FadeInOurLogo);
        }

        void FadeInOurLogo()
        {
            ourLogo.CrossFadeAlpha(1, 0.5f, true);
            LeanTween.delayedCall(1f, FadeOutOurLogo);
        }

        void FadeOutOurLogo()
        {
            ourLogo.CrossFadeAlpha(0, 0.5f, true);
            LeanTween.delayedCall(0.5f, OnTimeOut);
        }

        private void OnTimeOut()
        {
            if (onTimeOutDelegate != null)
                onTimeOutDelegate();
            UIMgr.instance.Close(PREFAB_PATH);
        }

        private void ShunWangPlatformInitOk()
        {
            FadeOutShunLogo();
        }
    }
}