using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace Logic.UI.WifiTips.View
{
    public class WifiTipsView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/wifi_tips/wifi_tips_view";

        public System.Action onClickOKButtonHandler;
        public System.Action onClickCancelButtonHandler;

        public static void Open(System.Action onClickOKButtonHandler, System.Action onClickCancelButtonHandler)
        {
            WifiTipsView wifiTipsView = UIMgr.instance.Open<WifiTipsView>(PREFAB_PATH, EUISortingLayer.Tips);
            wifiTipsView.onClickOKButtonHandler = onClickOKButtonHandler;
            wifiTipsView.onClickCancelButtonHandler = onClickCancelButtonHandler;
        }

        #region UI event handlers
        public void ClickCloseButtonHandler()
        {
            if (onClickCancelButtonHandler != null)
            {
                onClickCancelButtonHandler();
            }
            UIMgr.instance.Close(PREFAB_PATH);
        }

        public void ClickOKButtonHandler()
        {
            if (onClickOKButtonHandler != null)
            {
                onClickOKButtonHandler();
            }
            UIMgr.instance.Close(PREFAB_PATH);
        }
        #endregion UI event handlers
    }
}