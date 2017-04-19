using UnityEngine;
using UnityEngine.UI;
using Common.Localization;
using Logic.Enums;
using Logic.ConsumeTip.Model;

namespace Logic.UI.Tips.View
{
    public class ConfirmTipsView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/tips/confirm_tips_view";

        public System.Action onClickOKButtonHandler;
        public System.Action onClickCancelButtonHandler;

        #region UI components
        public Text tipsTitleText;
        public Text tipsDescriptionText;
        public Text cancelText;
        public Text okText;
		public Toggle toggleTip;
        #endregion UI components

		private ConsumeTipType _consumeTipType;

		public void SetInfo(string tipsDescription, System.Action onClickOKButtonHandler,ConsumeTipType type = ConsumeTipType.None)
        {
            tipsTitleText.text = Localization.Get("confirm_tips_view.tips_title");
            cancelText.text = Localization.Get("confirm_tips_view.cancel");
            okText.text = Localization.Get("confirm_tips_view.ok");
            tipsDescriptionText.text = tipsDescription;
            this.onClickOKButtonHandler = onClickOKButtonHandler;
			_consumeTipType = type;
			if(!ConsumeTipProxy.instance.HasConsumeTipKey(type))
			{
				toggleTip.gameObject.SetActive(false);
			}
        }

        public static void Open(string tipsDescription, System.Action onClickOKButtonHandler,ConsumeTipType type = ConsumeTipType.None)
        {
            ConfirmTipsView confirmTipsView = UIMgr.instance.Open<ConfirmTipsView>(PREFAB_PATH,EUISortingLayer.Tips);
            confirmTipsView.SetInfo(tipsDescription, onClickOKButtonHandler,type);
        }

		public static void Open(string tipsDescription, System.Action onClickOKButtonHandler, System.Action onClickCancelButtonHandler, ConsumeTipType type = ConsumeTipType.None)
		{
			ConfirmTipsView confirmTipsView = UIMgr.instance.Open<ConfirmTipsView>(PREFAB_PATH,EUISortingLayer.Tips);
			confirmTipsView.SetInfo(tipsDescription, onClickOKButtonHandler,type);
			confirmTipsView.onClickCancelButtonHandler = onClickCancelButtonHandler;
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
			if(_consumeTipType != ConsumeTipType.None)
				ConsumeTipProxy.instance.SetConsumeTipEnable(_consumeTipType,!toggleTip.isOn);

            if (onClickOKButtonHandler != null)
            {
                onClickOKButtonHandler();
            }
            UIMgr.instance.Close(PREFAB_PATH);
        }
        #endregion UI event handlers
    }
}
