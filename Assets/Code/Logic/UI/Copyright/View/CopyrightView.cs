using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Logic.UI.Copyright.View
{
    public class CopyrightView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/copyright/copyright_view";

        public Text copyrightText;
        public float crossFadeDelay;
        public float crossFadeDuration;

        public delegate void OnTimeOutDelegate();
        public OnTimeOutDelegate onTimeOutDelegate;

        public static CopyrightView Open()
        {
            return UIMgr.instance.Open<CopyrightView>(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Replace);
        }

        public void SetOnTimeOutDelegate(OnTimeOutDelegate onTimeOutDelegate)
        {
            this.onTimeOutDelegate = onTimeOutDelegate;
        }

        void Awake()
        {
            LTDescr ltDescr = LeanTween.delayedCall(crossFadeDelay + crossFadeDuration + crossFadeDuration, OnTimeOut);
            ltDescr.setIgnoreTimeScale(true);
        }

        void Start()
        {
            //copyrightText.CrossFadeAlpha(1, crossFadeDuration, true);
            LTDescr ltdescr = LeanTween.value(copyrightText.gameObject, 0, 1, crossFadeDuration);
            ltdescr.setOnUpdate(SetUpdateTextColor);

            LTDescr crossFadeLTDescr = LeanTween.delayedCall(crossFadeDelay + crossFadeDuration, StartCrossFade);
            crossFadeLTDescr.setIgnoreTimeScale(true);
        }

        private void SetUpdateTextColor(float alpha)
        {
            Color color = copyrightText.color;
            color.a = alpha;
            copyrightText.color = color;
        }


        private void StartCrossFade()
        {
            copyrightText.CrossFadeAlpha(0, crossFadeDuration, true);
        }

        private void OnTimeOut()
        {
            if (onTimeOutDelegate != null)
                onTimeOutDelegate();
            UIMgr.instance.Close(PREFAB_PATH);
        }
    }
}
