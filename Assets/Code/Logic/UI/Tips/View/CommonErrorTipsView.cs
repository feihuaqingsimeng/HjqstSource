using UnityEngine;
using UnityEngine.UI;

namespace Logic.UI.Tips.View
{
	public class CommonErrorTipsView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/tips/common_error_tips_view";

		public Text tipsText;
		private System.Action _callback;
		public static CommonErrorTipsView Open(string tipsString, System.Action callback = null, EUISortingLayer uiSortingLayer = EUISortingLayer.Tips)
		{
			CommonErrorTipsView tips = UIMgr.instance.Open<CommonErrorTipsView>(PREFAB_PATH, uiSortingLayer);
			tips.SetTips(tipsString);
			tips._callback = callback;
			return tips;
		}

		public void SetTips (string tipsString)
		{
			tipsText.text = tipsString;
		}

		public void ClickCloseHandler ()
		{
			if (_callback != null)
			{
				_callback();
			}
			UIMgr.instance.Close(PREFAB_PATH);
		}
	}
}