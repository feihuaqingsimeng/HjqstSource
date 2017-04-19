using UnityEngine;
using UnityEngine.UI;
using Common.ResMgr;
using Common.Localization;
using Logic.Shop.Model;
using Logic.UI.Shop.Controller;
using Logic.Equipment.Model;
using Logic.Enums;
using Logic.Game.Model;
using Logic.UI.Tips.View;
using Logic.Game;
using Logic.ConsumeTip.Model;
using Logic.Item.Model;
using Logic.UI.CommonItem.View;

namespace Logic.UI.Shop.View
{
	public class DrawEquipmentResultView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/shop/draw_equipment_result_view";

		private ShopEquipmentRandomCardInfo _shopEquipmentRandomCardInfo;
				
		#region UI components
		public DrawEquipmentItem drawEquipmentItem;
		public Image cardBackImage;
		public CommonItemIcon drawItemIcon;
		public Text backText;
		public Image costIconImage;
		public Text againText;
		#endregion UI components

		public void SetNewEquipmentInfo (ShopEquipmentRandomCardInfo shopEquipmentCardInfo, EquipmentInfo equipmentInfo)
		{
			_shopEquipmentRandomCardInfo = shopEquipmentCardInfo;
			drawEquipmentItem.SetEquipmentInfo(equipmentInfo);
			drawEquipmentItem.TurnOverAfter(0.2f);

			backText.text = Localization.Get("ui.draw_equipment_result_view.back");
			costIconImage.SetSprite(ResMgr.instance.Load<Sprite>(UIUtil.GetBaseResIconPath(_shopEquipmentRandomCardInfo.ShopCardRandomData.costGameResData.type)));
			costIconImage.SetNativeSize();
			againText.text = string.Format(Localization.Get("ui.draw_equipment_result_view.again"), _shopEquipmentRandomCardInfo.ShopCardRandomData.costGameResData.count);
		}

		public void SetNewItemInfo (ShopEquipmentRandomCardInfo shopEquipmentRandomCardInfo, ItemInfo itemInfo)
		{
			_shopEquipmentRandomCardInfo = shopEquipmentRandomCardInfo;
			drawItemIcon.SetItemInfo(itemInfo);
			drawItemIcon.ShowCount();

			LeanTween.cancel(drawItemIcon.gameObject);
			LeanTween.cancel(cardBackImage.gameObject);

			drawItemIcon.transform.localScale = new Vector3 (0, 1, 1);
			cardBackImage.transform.localScale = Vector3.one;

			LTDescr ltDescr = LeanTween.scaleX (cardBackImage.gameObject, 0, 0.2f);
			ltDescr.setOnComplete(OnCardBackTurnOverComplete);

			backText.text = Localization.Get("ui.draw_equipment_result_view.back");
			costIconImage.SetSprite(ResMgr.instance.Load<Sprite>(UIUtil.GetBaseResIconPath(_shopEquipmentRandomCardInfo.ShopCardRandomData.costGameResData.type)));
			costIconImage.SetNativeSize();
			againText.text = string.Format(Localization.Get("ui.draw_equipment_result_view.again"), _shopEquipmentRandomCardInfo.ShopCardRandomData.costGameResData.count);
		}

		private void OnCardBackTurnOverComplete ()
		{
			LTDescr ltDescr = LeanTween.scaleX (drawItemIcon.gameObject, 1, 0.2f);
		}

		#region UI event handlers
		public void ClickAgainHandler ()
		{
			int shopID = 0;
			int shopItemID = 0;
			string shopItemName = string.Empty;
			GameResData costGameResData = null;
			int costType = 1;
			if (_shopEquipmentRandomCardInfo != null)
			{
				shopID = _shopEquipmentRandomCardInfo.ShopCardRandomData.shopID;
				shopItemID = _shopEquipmentRandomCardInfo.ShopCardRandomData.id;
				shopItemName = Localization.Get(_shopEquipmentRandomCardInfo.ShopCardRandomData.name);
				costGameResData = _shopEquipmentRandomCardInfo.ShopCardRandomData.costGameResData;
				if (_shopEquipmentRandomCardInfo.RemainFreeTimes > 0 && _shopEquipmentRandomCardInfo.NextFreeBuyCountDownTime <= 0)
				{
					costType = 0;
				}
			}
			
			if (costType == 0)   //免费
			{
				ShopController.instance.CLIENT2LOBBY_PURCHASE_GOODS_REQ(shopID, shopItemID, costType);
			}
			else if (costType == 1)   //非免费
			{
				if (GameResUtil.IsBaseRes(costGameResData.type))
				{
					if (GameProxy.instance.BaseResourceDictionary[costGameResData.type] < costGameResData.count)
					{
						CommonErrorTipsView.Open(string.Format(Localization.Get("ui.common_tips.not_enough_resource"), GameResUtil.GetBaseResName(costGameResData.type)));
						return;
					}
					
					if (costGameResData.type == Logic.Enums.BaseResType.Diamond)
					{
//						if(ConsumeTipProxy.instance.GetConsumeTipEnable(ConsumeTipType.DiamondSingleLotteryEquip))
//							ConfirmBuyShopItemTipsView.Open(shopItemName, costGameResData, ClickConfirmBuyHandler,ConsumeTipType.DiamondSingleLotteryEquip);
//						else 
//							ClickConfirmBuyHandler();
					}
					else
					{
						ShopController.instance.CLIENT2LOBBY_PURCHASE_GOODS_REQ(shopID, shopItemID, costType);
					}
				}
				else
				{
					ShopController.instance.CLIENT2LOBBY_PURCHASE_GOODS_REQ(shopID, shopItemID, costType);
				}
			}
		}

		public void ClickConfirmBuyHandler ()
		{
			int shopID = 0;
			int shopItemID = 0;
			GameResData costGameResData = null;
			int costType = 1;

			if (_shopEquipmentRandomCardInfo != null)
			{
				shopID = _shopEquipmentRandomCardInfo.ShopCardRandomData.shopID;
				shopItemID = _shopEquipmentRandomCardInfo.ShopCardRandomData.id;
				costGameResData = _shopEquipmentRandomCardInfo.ShopCardRandomData.costGameResData;
				if (_shopEquipmentRandomCardInfo.RemainFreeTimes > 0 && _shopEquipmentRandomCardInfo.NextFreeBuyCountDownTime <= 0)
				{
					costType = 0;
				}
			}
			ShopController.instance.CLIENT2LOBBY_PURCHASE_GOODS_REQ(shopID, shopItemID, costType);
		}

		public void ClickCloseHandler ()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}		
		#endregion
	}
}
