using UnityEngine;
using UnityEngine.UI;
using Common.Localization;
using Logic.Enums;
using Logic.Game.Model;
using Logic.ConsumeTip.Model;
using LuaInterface;

namespace Logic.UI.Tips.View
{
	public class CommonExpandBagTipsView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/tips/common_expand_bag_tips_view";

		public static CommonExpandBagTipsView Open(BagType bagType, int cost, System.Action confirmAction,ConsumeTipType type)
		{
			CommonExpandBagTipsView view = UIMgr.instance.Open<CommonExpandBagTipsView>(PREFAB_PATH, EUISortingLayer.Tips);
			view.SetTypeAndCost(bagType,cost,confirmAction,type);
			return view;
		}

		[NoToLua]
		public static CommonExpandBagTipsView Open(int bagTypeValue, int cost, System.Action confirmAction, int ConsumeTipTypeValue)
		{
			CommonExpandBagTipsView view = UIMgr.instance.Open<CommonExpandBagTipsView>(PREFAB_PATH, EUISortingLayer.Tips);
			view.SetTypeAndCost((BagType)bagTypeValue, cost, confirmAction, (ConsumeTipType)ConsumeTipTypeValue);
			return view;
		}

		private BagType _bagType;
		private int _cost;

		#region UI components
		public Text titleText;
		public Text costTitleText;
		public Text costText;
		public Text descriptionText;

		public Text cancelText;
		public Text okText;
		public Toggle toggleTip;
		#endregion UI components

		public System.Action action;
		private ConsumeTipType _consumeTipType;
		void Start ()
		{
			titleText.text = Localization.Get("ui.common_expand_bag_tips_view.not_enough_bag_title");
			descriptionText.text = string.Format(Localization.Get("ui.common_expand_bag_tips_view.not_enough_bag_description"), GlobalData.GetGlobalData().hero_package_buy_num);
			cancelText.text = Localization.Get("ui.common_expand_bag_tips_view.cancel");
			okText.text = Localization.Get("ui.common_expand_bag_tips_view.expand_bag");
		}

		public void SetTypeAndCost (BagType bagType, int cost, System.Action confirmAction,ConsumeTipType type = ConsumeTipType.None)
		{
			_bagType = bagType;
			_cost = cost;
			costText.text = cost.ToString();
			action = confirmAction;
			_consumeTipType = type;
			if(!ConsumeTipProxy.instance.HasConsumeTipKey(type))
			{
				toggleTip.gameObject.SetActive(false);
			}
		}

		#region UI event handlers
		public void ClickCloseHandler ()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}

		public void ClickCancelHandler ()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}

		public void ClickOKHandler ()
		{
			if (_bagType == BagType.HeroBag && GameProxy.instance.HeroCellNum >= GlobalData.GetGlobalData().heroPackageMaxNum)
			{
				CommonErrorTipsView.Open(Localization.Get("ui.common_tips.hero_bag__reach_max"));
				return;
			}

			if (_bagType == BagType.EquipmentBag && GameProxy.instance.EquipCellNum >= GlobalData.GetGlobalData().equipPackageMaxNum)
			{
				CommonErrorTipsView.Open(Localization.Get("ui.common_tips.equipment_bag_reach_max"));
				return;
			}

			if (GameProxy.instance.BaseResourceDictionary[BaseResType.Diamond] < _cost)
			{
				CommonErrorTipsView.Open(Localization.Get("ui.common_tips.not_enough_diamond"));
				return;
			}
			if (action != null) 
				action();
			UIMgr.instance.Close(PREFAB_PATH);

			ConsumeTipProxy.instance.SetConsumeTipEnable(_consumeTipType,!toggleTip.isOn);
		}
		#endregion UI event handers
	}
}