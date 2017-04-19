using UnityEngine;

namespace Logic.UI.TouchIndicator.View
{
    public class TouchIndicatorView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/touch_indicator/touch_indicator_view";

        public Transform indicatorTransform;

        void Start()
        {
            indicatorTransform.gameObject.SetActive(false);
            EasyTouch.On_TouchStart += EasyTouch_On_TouchStart;
        }

        void OnDestroy()
        {
            EasyTouch.On_TouchStart -= EasyTouch_On_TouchStart;
        }

        void EasyTouch_On_TouchStart(Gesture gesture)
        {
            indicatorTransform.position = UIMgr.instance.uiCamera.ScreenToWorldPoint(gesture.position);
            indicatorTransform.localPosition = new Vector3(indicatorTransform.localPosition.x, indicatorTransform.localPosition.y, 0);
            indicatorTransform.gameObject.SetActive(false);
            indicatorTransform.gameObject.SetActive(true);
        }
    }
}
