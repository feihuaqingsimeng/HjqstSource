using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace Logic.UI.NetErrorTips.View
{
    public class NetErrorTipsView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/net_error_tips/net_error_tips_view";

        public System.Action onClickOKButtonHandler;

        public static void Open(System.Action onClickOKButtonHandler)
        {
            NetErrorTipsView wifiTipsView = UIMgr.instance.Open<NetErrorTipsView>(PREFAB_PATH, EUISortingLayer.Tips);
            wifiTipsView.onClickOKButtonHandler = onClickOKButtonHandler;
        }

        #region UI event handlers
        public void ClickCloseButtonHandler()
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