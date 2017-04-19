using UnityEngine;
using UnityEngine.UI;
using Common.Localization;
using Common.ResMgr;
using Logic.Game.Model;
using Logic.Enums;
using Logic.ConsumeTip.Model;

namespace Logic.UI.Tips.View
{
	public class ConfirmBuyShopItemTipsView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/tips/confirm_buy_shop_item_tips_view";

		public System.Action onClickBuyButtonHandler;

		#region UI components
		public Text titleText;
		public Text tipsText;
		public Text costTitleText;
		public Image costResourceIconImage;
		public Text costResourceCountText;
		public Text cancelText;
		public Text buyText;
		public Toggle toggleTip;
		#endregion UI components

		private ConsumeTipType _consumeTipType;

		public void SetInfo (string itemName, GameResData costGameResData, System.Action onClickBuyButtonHandler ,ConsumeTipType type)
		{
			titleText.text = Localization.Get("ui.confirm_buy_shop_item_tips_view.title");
			tipsText.text = string.Format(Localization.Get("ui.confirm_buy_shop_item_tips_view.tips"), itemName);
			costTitleText.text = Localization.Get("ui.confirm_buy_shop_item_tips_view.tip.cost_title");
			costResourceIconImage.SetSprite(ResMgr.instance.Load<Sprite>(UIUtil.GetBaseResIconPath(costGameResData.type)));
			costResourceIconImage.SetNativeSize();
			costResourceCountText.text = costGameResData.count.ToString();
			cancelText.text = Localization.Get("ui.confirm_buy_shop_item_tips_view.tip.cancel");
			buyText.text = Localization.Get("ui.confirm_buy_shop_item_tips_view.tip.buy");
			this.onClickBuyButtonHandler = onClickBuyButtonHandler;
			_consumeTipType = type;
			if(!ConsumeTipProxy.instance.HasConsumeTipKey(type))
			{
				toggleTip.gameObject.SetActive(false);
			}
		}

		public static ConfirmBuyShopItemTipsView Open (string itemName, GameResData costGameResData, System.Action onClickBuyButtonHandler,ConsumeTipType type = ConsumeTipType.None)
		{
			ConfirmBuyShopItemTipsView confirmBuyShopItemTipsView = UIMgr.instance.Open<ConfirmBuyShopItemTipsView>(PREFAB_PATH,EUISortingLayer.Tips);
			confirmBuyShopItemTipsView.SetInfo(itemName, costGameResData, onClickBuyButtonHandler,type);
			return confirmBuyShopItemTipsView;
		}

		public static ConfirmBuyShopItemTipsView Open (string title, string tips, GameResData costGameResData, System.Action onClickBuyButtonHandler, ConsumeTipType consumeTipType = ConsumeTipType.None)
		{
			ConfirmBuyShopItemTipsView confirmBuyShopItemTipsView = UIMgr.instance.Open<ConfirmBuyShopItemTipsView>(PREFAB_PATH,EUISortingLayer.Tips);
			confirmBuyShopItemTipsView.SetInfo("", costGameResData, onClickBuyButtonHandler, consumeTipType);
			confirmBuyShopItemTipsView.titleText.text = title;
			confirmBuyShopItemTipsView.tipsText.text = tips;
			return confirmBuyShopItemTipsView;
		}

		#region UI event handlers
		public void ClickCancelButtonHandler ()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}

		public void ClickBuyButtonHandler ()
		{
			ConsumeTipProxy.instance.SetConsumeTipEnable(_consumeTipType,!toggleTip.isOn);
			if (onClickBuyButtonHandler != null)
			{
				onClickBuyButtonHandler();
			}
			UIMgr.instance.Close(PREFAB_PATH);

		}
		#endregion UI event handlers
	}
}
