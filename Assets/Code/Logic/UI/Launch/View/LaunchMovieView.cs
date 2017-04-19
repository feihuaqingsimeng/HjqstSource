using UnityEngine;
using System.Collections;
namespace Logic.UI.Launch.View
{
    public class LaunchMovieView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/launch/launch_movie_view";
        public float duration;
        public Transform mediaTrans;

        public delegate void OnTimeOutDelegate();
        public OnTimeOutDelegate onTimeOutDelegate;

        public static LaunchMovieView Open()
        {
            return UIMgr.instance.Open<LaunchMovieView>(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Replace);
        }

        public void SetOnTimeOutDelegate(OnTimeOutDelegate onTimeOutDelegate)
        {
            this.onTimeOutDelegate = onTimeOutDelegate;
        }

        void Start()
        {
            Vector3 scale = mediaTrans.transform.localScale;
            scale.x /= transform.localScale.x;
            scale.y /= transform.localScale.y;
            mediaTrans.transform.localScale = scale;
            LTDescr ltDescr = LeanTween.delayedCall(this.gameObject, duration, OnTimeOut);
            ltDescr.setIgnoreTimeScale(true);
            EasyTouch.On_SimpleTap += On_SimpleTapHanlder;
        }

        private void OnTimeOut()
        {
            if (onTimeOutDelegate != null)
                onTimeOutDelegate();
			this.onTimeOutDelegate = null;
            UIMgr.instance.Close(PREFAB_PATH);
        }

        void OnDestroy() 
        {
            EasyTouch.On_SimpleTap -= On_SimpleTapHanlder;
        }

        #region ui event handler
        private void On_SimpleTapHanlder(Gesture gesture)
        {
            OnSkipBtnClickHandler();
        }

        public void OnSkipBtnClickHandler()
        {
            LeanTween.cancel(this.gameObject);
            OnTimeOut();
        }
        #endregion
    }
}