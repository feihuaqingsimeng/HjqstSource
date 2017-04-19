using UnityEngine;
using UnityEngine.UI;
using Logic.Game.Model;
using Common.Localization;
using Logic.Enums;
using Common.ResMgr;
using Logic.ConsumeTip.Model;

namespace Logic.UI.Tips.View
{
	public class ConfirmCostTipsView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/tips/confirm_cost_tips_view";
		
		private System.Action _clickCancelButtonDelegate;
		private System.Action _clickOKButtonDelegate;

		public Text titleText;
		public Text costTitleText;
		public Image costResourceIconImage;
		public Text costResourceAmountText;
		public Text contentText;
		public Text cancelText;
		public Text okText;
		public Toggle toggleTip;

		private ConsumeTipType _consumeTipType;

		private void SetInfo (GameResData costGameResData, string content, System.Action clickCancelButtonDelegate, System.Action clickOKButtonDelegate,ConsumeTipType type = ConsumeTipType.None)
		{
			titleText.text = Localization.Get("ui.confirm_cost_tip_view.title");
			costTitleText.text = Localization.Get("ui.confirm_cost_tip_view.cost_title");
			contentText.text = content;
			costResourceIconImage.SetSprite(ResMgr.instance.Load<Sprite>(UIUtil.GetBaseResIconPath(costGameResData.type)));
			costResourceAmountText.text = costGameResData.count.ToString();
			cancelText.text = Localization.Get("ui.confirm_cost_tip_view.cancel");
			okText.text = Localization.Get("ui.confirm_cost_tip_view.ok");
			_clickCancelButtonDelegate = clickCancelButtonDelegate;
			_clickOKButtonDelegate = clickOKButtonDelegate;
			_consumeTipType = type;
			if(!ConsumeTipProxy.instance.HasConsumeTipKey(type))
			{
				toggleTip.gameObject.SetActive(false);
			}
		}

		public static ConfirmCostTipsView Open (GameResData costGameResData, string content, System.Action clickCancelButtonDelegate, System.Action clickOKButtonDelegate,ConsumeTipType type = ConsumeTipType.None)
		{
			ConfirmCostTipsView confirmCostTipsView = UIMgr.instance.Open<ConfirmCostTipsView>(PREFAB_PATH,EUISortingLayer.Tips);
			confirmCostTipsView.SetInfo(costGameResData, content, clickCancelButtonDelegate, clickOKButtonDelegate,type);
			return confirmCostTipsView;
		}
		public static ConfirmCostTipsView Open (GameResData costGameResData,string title, string content,System.Action clickCancelButtonDelegate,System.Action clickOKButtonDelegate,ConsumeTipType type = ConsumeTipType.None)
		{
			ConfirmCostTipsView view = Open(costGameResData, content, clickCancelButtonDelegate, clickOKButtonDelegate,type);
			view.titleText.text = title;
			return view;
		}
		public void ClickCancelButton ()
		{
			if (_clickCancelButtonDelegate != null)
			{
				_clickCancelButtonDelegate();
			}
			UIMgr.instance.Close(PREFAB_PATH);
		}

		public void ClickOKButton ()
		{
			if(_consumeTipType!= ConsumeTipType.None)
				ConsumeTipProxy.instance.SetConsumeTipEnable(_consumeTipType,!toggleTip.isOn);

			if (_clickOKButtonDelegate != null)
			{
				_clickOKButtonDelegate();
			}
			UIMgr.instance.Close(PREFAB_PATH);
		}
	}
}