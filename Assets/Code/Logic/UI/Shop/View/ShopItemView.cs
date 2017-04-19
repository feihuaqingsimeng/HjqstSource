using System;
using UnityEngine;
using UnityEngine.UI;
using Common.Localization;
using Common.ResMgr;
using Common.GameTime.Controller;
using Logic.Game;
using Logic.Game.Model;
using Logic.Shop.Model;
using Logic.VIP.Model;
using Logic.UI.Shop.Model;
using Logic.Item.Model;
using Logic.Hero.Model;
using Logic.UI.Shop.Controller;
using Logic.UI.Tips.View;
using Common.Util;
using Logic.Enums;
using Logic.ConsumeTip.Model;

namespace Logic.UI.Shop.View
{
	public class ShopItemView : MonoBehaviour
	{
		private bool _isCountingDown = false;

		#region cached data
		private ShopHeroRandomCardInfo _shopHeroRandomCardInfo;
		private ShopEquipmentRandomCardInfo _shopEquipmentRandomCardInfo;
		private ShopDiamondItemInfo _shopDiamondItemInfo;
		private ShopActionItemInfo _shopActionItemInfo;
		private ShopGoldItemInfo _shopGoldItemInfo;
		private ShopGoodsItemInfo _shopGoodsItemInfo;
		#endregion cached data

		#region UI components
		public Text nameText;
		public Text descriptionText;
		public Image itemIconImage;
		public Slider freeCountDownSlider;
		public Text freeCountDownText;

		public GameObject freeTimesRoot;
		public Text freeTimesTitleText;
		public Text freeTimesText;
		public GameObject limitTimesRoot;
		public Text limitTimesTitleText;
		public Text limitTimesText;

		public Button buyButton;
		public Image costResourceIcon;
		public Text costResourceCountText;
		public Button soldOutButton;
		public Text soldOutText;
		#endregion UI components

		void Awake ()
		{
			if (soldOutText != null)
			{
				freeTimesTitleText.text = Localization.Get("ui.shop_view.free_times_title");
				limitTimesTitleText.text = Localization.Get("ui.shop_view.limit_times_title");
				soldOutText.text = Localization.Get("ui.shop_view.sold_out");
			}
		}

		void Update ()
		{
			if (_isCountingDown)
			{
				if (_shopHeroRandomCardInfo != null)
				{
					if (_shopHeroRandomCardInfo.RemainFreeTimes > 0 && _shopHeroRandomCardInfo.NextFreeBuyCountDownTime > 0)
					{
//						TimeSpan timeSpan = TimeSpan.FromSeconds(_shopHeroRandomCardInfo.NextFreeBuyCountDownTime);
//						string nextFreeBuyCountDownTimeStr = string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
						string nextFreeBuyCountDownTimeStr = TimeUtil.FormatSecondToHour((int)_shopHeroRandomCardInfo.NextFreeBuyCountDownTime);
						freeCountDownText.text = nextFreeBuyCountDownTimeStr;
						freeCountDownSlider.value = _shopHeroRandomCardInfo.NextFreeBuyCountDownTime * 1.0f / _shopHeroRandomCardInfo.ShopCardRandomData.freeTime;
					}
					else
					{
						SetShopHeroRandomCardInfo(_shopHeroRandomCardInfo);
						_isCountingDown = false;
					}
				}
				else if (_shopEquipmentRandomCardInfo != null)
				{
					if (_shopEquipmentRandomCardInfo.RemainFreeTimes > 0 && _shopEquipmentRandomCardInfo.NextFreeBuyCountDownTime > 0)
					{
//						TimeSpan timeSpan = TimeSpan.FromSeconds(_shopEquipmentRandomCardInfo.NextFreeBuyCountDownTime);
//						string nextFreeBuyCountDownTimeStr = string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
						string nextFreeBuyCountDownTimeStr = TimeUtil.FormatSecondToHour((int)_shopEquipmentRandomCardInfo.NextFreeBuyCountDownTime);
						freeCountDownText.text = _shopEquipmentRandomCardInfo.NextFreeBuyCountDownTime.ToString();
						freeCountDownSlider.value = _shopEquipmentRandomCardInfo.NextFreeBuyCountDownTime * 1.0f / _shopEquipmentRandomCardInfo.ShopCardRandomData.freeTime;
					}
					else
					{
						SetShopEquipmentRandomCardInfo(_shopEquipmentRandomCardInfo);
						_isCountingDown = false;
					}
				}
			}
		}

		public void SetShopHeroRandomCardInfo (ShopHeroRandomCardInfo shopHeroRandomCardInfo)
		{
			_shopHeroRandomCardInfo = shopHeroRandomCardInfo;
			nameText.text = Localization.Get(_shopHeroRandomCardInfo.ShopCardRandomData.name);
			descriptionText.text = Localization.Get(_shopHeroRandomCardInfo.ShopCardRandomData.description);
			itemIconImage.SetSprite(ResMgr.instance.Load<Sprite>(ResPath.GetShopItemIconPath(_shopHeroRandomCardInfo.ShopCardRandomData.pic)));
			itemIconImage.SetNativeSize();
			costResourceIcon.SetSprite(ResMgr.instance.Load<Sprite>(UIUtil.GetBaseResIconPath(_shopHeroRandomCardInfo.ShopCardRandomData.costGameResData.type)));
			costResourceIcon.SetNativeSize();
			costResourceCountText.text = _shopHeroRandomCardInfo.ShopCardRandomData.costGameResData.count.ToString();

//			freeTimesRoot.gameObject.SetActive(_shopHeroRandomCardInfo.ShopCardRandomData.freeTime > 0);
//			freeTimesText.text = string.Format(Localization.Get("common.value/max"), _shopHeroRandomCardInfo.RemainFreeTimes, _shopHeroRandomCardInfo.ShopCardRandomData.max);

			//免费抽卡取消每日次数限制，遂不在显示次数
			freeTimesRoot.gameObject.SetActive(false);

			if (_shopHeroRandomCardInfo.RemainFreeTimes > 0)
			{
				if (_shopHeroRandomCardInfo.NextFreeBuyCountDownTime > 0)
				{
					costResourceCountText.text = _shopHeroRandomCardInfo.ShopCardRandomData.costGameResData.count.ToString();
					freeCountDownSlider.gameObject.SetActive(true);
					_isCountingDown = true;
				}
				else
				{
					costResourceCountText.text = Localization.Get("ui.shop_view.free");
					freeCountDownSlider.gameObject.SetActive(false);
				}
			}
			limitTimesRoot.SetActive(false);
			buyButton.gameObject.SetActive(true);
			soldOutButton.gameObject.SetActive(false);
		}

		public void SetShopEquipmentRandomCardInfo (ShopEquipmentRandomCardInfo shopEquipmentRandomCardInfo)
		{
			_shopEquipmentRandomCardInfo = shopEquipmentRandomCardInfo;
			nameText.text = Localization.Get(_shopEquipmentRandomCardInfo.ShopCardRandomData.name);
			descriptionText.text = Localization.Get(_shopEquipmentRandomCardInfo.ShopCardRandomData.description);
			itemIconImage.SetSprite( ResMgr.instance.Load<Sprite>(ResPath.GetShopItemIconPath(_shopEquipmentRandomCardInfo.ShopCardRandomData.pic)));
			itemIconImage.SetNativeSize();
			costResourceIcon.SetSprite(ResMgr.instance.Load<Sprite>(UIUtil.GetBaseResIconPath(_shopEquipmentRandomCardInfo.ShopCardRandomData.costGameResData.type)));
			costResourceIcon.SetNativeSize();
			costResourceCountText.text = _shopEquipmentRandomCardInfo.ShopCardRandomData.costGameResData.count.ToString();

//			freeTimesRoot.gameObject.SetActive(_shopEquipmentRandomCardInfo.ShopCardRandomData.freeTime > 0);
//			freeTimesText.text = string.Format(Localization.Get("common.value/max"), _shopEquipmentRandomCardInfo.RemainFreeTimes, _shopEquipmentRandomCardInfo.ShopCardRandomData.max);

			//免费抽卡取消每日次数限制，遂不在显示次数
			freeTimesRoot.gameObject.SetActive(false);

			if (_shopEquipmentRandomCardInfo.RemainFreeTimes > 0)
			{
				if (_shopEquipmentRandomCardInfo.NextFreeBuyCountDownTime > 0)
				{
					costResourceCountText.text = _shopEquipmentRandomCardInfo.ShopCardRandomData.costGameResData.count.ToString();
					freeCountDownSlider.gameObject.SetActive(true);
					_isCountingDown = true;
				}
				else
				{
					costResourceCountText.text = Localization.Get("ui.shop_view.free");
					freeCountDownSlider.gameObject.SetActive(false);
				}
			}
			limitTimesRoot.SetActive(false);
			buyButton.gameObject.SetActive(true);
			soldOutButton.gameObject.SetActive(false);
		}

		public void SetShopDiamondItemInfo (ShopDiamondItemInfo shopDiamondItemInfo)
		{
			_shopDiamondItemInfo = shopDiamondItemInfo;
			nameText.text = Localization.Get(_shopDiamondItemInfo.ShopDiamondData.name);
			descriptionText.text = Localization.Get(_shopDiamondItemInfo.ShopDiamondData.description);
			itemIconImage.SetSprite(ResMgr.instance.Load<Sprite>(ResPath.GetShopItemIconPath(_shopDiamondItemInfo.ShopDiamondData.pic)));
			itemIconImage.SetNativeSize();

			freeTimesRoot.SetActive(false);
			limitTimesRoot.SetActive(false);
			freeCountDownSlider.gameObject.SetActive(false);

			costResourceIcon.SetSprite(ResMgr.instance.Load<Sprite>(UIUtil.GetBaseResIconPath(_shopDiamondItemInfo.ShopDiamondData.costGameResData.type)));
			costResourceIcon.SetNativeSize();
			costResourceCountText.text = _shopDiamondItemInfo.ShopDiamondData.costGameResData.count.ToString();

			buyButton.gameObject.SetActive(true);
			soldOutButton.gameObject.SetActive(false);
		}

		public void SetShopActionItemInfo (ShopActionItemInfo shopActionItemInfo)
		{
			_shopActionItemInfo = shopActionItemInfo;
			nameText.text = Localization.Get(_shopActionItemInfo.ShopLimitItemData.name);
			descriptionText.text = Localization.Get(_shopActionItemInfo.ShopLimitItemData.description);
			itemIconImage.SetSprite( ResMgr.instance.Load<Sprite>(ResPath.GetShopItemIconPath(_shopActionItemInfo.ShopLimitItemData.pic)));
			itemIconImage.SetNativeSize();

			freeTimesRoot.SetActive(false);
			limitTimesRoot.SetActive(false);
			freeCountDownSlider.gameObject.SetActive(false);

			costResourceIcon.SetSprite(ResMgr.instance.Load<Sprite>(UIUtil.GetBaseResIconPath(_shopActionItemInfo.ShopLimitItemData.costGameResData.type)));
			costResourceIcon.SetNativeSize();
			costResourceCountText.text = _shopActionItemInfo.ShopLimitItemData.costGameResData.count.ToString();

			if (_shopActionItemInfo.RemainPurchaseTimes > 0)
			{
				buyButton.gameObject.SetActive(true);
				soldOutButton.gameObject.SetActive(false);
			}
			else
			{
				buyButton.gameObject.SetActive(false);
				soldOutButton.gameObject.SetActive(true);
			}
		}

		public void SetShopGoldItemInfo (ShopGoldItemInfo shopGoldItemInfo)
		{
			_shopGoldItemInfo = shopGoldItemInfo;
			nameText.text = Localization.Get(_shopGoldItemInfo.ShopLimitItemData.name);
			descriptionText.text = Localization.Get(_shopGoldItemInfo.ShopLimitItemData.description);
			itemIconImage.SetSprite(ResMgr.instance.Load<Sprite>(ResPath.GetShopItemIconPath(_shopGoldItemInfo.ShopLimitItemData.pic)));
			itemIconImage.SetNativeSize();

			freeTimesRoot.SetActive(false);
			limitTimesRoot.SetActive(false);
			freeCountDownSlider.gameObject.SetActive(false);

			costResourceIcon.SetSprite(ResMgr.instance.Load<Sprite>(UIUtil.GetBaseResIconPath(_shopGoldItemInfo.ShopLimitItemData.costGameResData.type)));
			costResourceIcon.SetNativeSize();
			costResourceCountText.text = _shopGoldItemInfo.ShopLimitItemData.costGameResData.count.ToString();

			if (_shopGoldItemInfo.RemainPurchaseTimes > 0)
			{
				buyButton.gameObject.SetActive(true);
				soldOutButton.gameObject.SetActive(false);
			}
			else
			{
				buyButton.gameObject.SetActive(false);
				soldOutButton.gameObject.SetActive(true);
			}
		}

		public void SetShopGoodItemInfo (ShopGoodsItemInfo shopGoodsItemInfo)
		{
			_shopGoodsItemInfo = shopGoodsItemInfo;

			GameResData goodsGameResData = _shopGoodsItemInfo.ShopGoodsData.goodsGameResData;

			nameText.text = Localization.Get(_shopGoodsItemInfo.ShopGoodsData.name);
			descriptionText.text = Localization.Get(_shopGoodsItemInfo.ShopGoodsData.description);

			string itemIconSpritePath = string.Empty;
			if (!string.IsNullOrEmpty(_shopGoodsItemInfo.ShopGoodsData.pic))
			{
				itemIconSpritePath = ResPath.GetShopItemIconPath(_shopGoodsItemInfo.ShopGoodsData.pic);
			}
			else
			{
				if (goodsGameResData.type == Logic.Enums.BaseResType.Item)
				{
					ItemData itemData = ItemData.GetItemDataByID(goodsGameResData.id);
					itemIconSpritePath = ResPath.GetItemIconPath(itemData.icon);
				}
				else if (goodsGameResData.type == Logic.Enums.BaseResType.Hero)
				{
					HeroData heroData = HeroData.GetHeroDataByID(goodsGameResData.id);
					itemIconSpritePath = ResPath.GetCharacterHeadIconPath(heroData.headIcons[heroData.starMin - 1]);
				}
			}
			itemIconImage.SetSprite( ResMgr.instance.Load<Sprite>(itemIconSpritePath));
			itemIconImage.SetNativeSize();

			costResourceIcon.SetSprite(ResMgr.instance.Load<Sprite>(UIUtil.GetBaseResIconPath(_shopGoodsItemInfo.ShopGoodsData.costGameResData.type)));
			costResourceIcon.SetNativeSize();
			costResourceCountText.text = _shopGoodsItemInfo.ShopGoodsData.costGameResData.count.ToString();

			freeTimesRoot.SetActive(false);
			freeCountDownSlider.gameObject.SetActive(false);
			
			if (_shopGoodsItemInfo.ShopGoodsData.itemNum > 0)
			{
				if (_shopGoodsItemInfo.RemainPurchaseTimes > 0)
				{
					buyButton.gameObject.SetActive(true);
					soldOutButton.gameObject.SetActive(false);
				}
				else
				{
					buyButton.gameObject.SetActive(false);
					soldOutButton.gameObject.SetActive(true);
				}
				limitTimesText.text = string.Format(Localization.Get("common.value/max"), _shopGoodsItemInfo.RemainPurchaseTimes, _shopGoodsItemInfo.ShopGoodsData.itemNum);
				limitTimesRoot.SetActive(true);
			}
			else
			{
				limitTimesRoot.SetActive(false);
				buyButton.gameObject.SetActive(true);
				soldOutButton.gameObject.SetActive(false);
			}
		}

		#region UI event handlers
		public void ClickBuyHandler ()
		{
			int shopID = 0;
			int shopItemID = 0;
			string shopItemName = string.Empty;
			GameResData costGameResData = null;
			int costType = 1;
			ConsumeTipType consumTipType = ConsumeTipType.None;//提示
			if (_shopHeroRandomCardInfo != null)
			{
				shopID = _shopHeroRandomCardInfo.ShopCardRandomData.shopID;
				shopItemID = _shopHeroRandomCardInfo.ShopCardRandomData.id;
				shopItemName = Localization.Get(_shopHeroRandomCardInfo.ShopCardRandomData.name);
				costGameResData = _shopHeroRandomCardInfo.ShopCardRandomData.costGameResData;
				if (_shopHeroRandomCardInfo.RemainFreeTimes > 0 && _shopHeroRandomCardInfo.NextFreeBuyCountDownTime <= 0)
				{
					costType = 0;
				}
				consumTipType = _shopHeroRandomCardInfo.ShopCardRandomData.buyType == 1 ? ConsumeTipType.DiamondDrawSingleHero : ConsumeTipType.DiamondDrawTenHeroes;
			}
			else if (_shopEquipmentRandomCardInfo != null)
			{
				shopID = _shopEquipmentRandomCardInfo.ShopCardRandomData.shopID;
				shopItemID = _shopEquipmentRandomCardInfo.ShopCardRandomData.id;
				shopItemName = Localization.Get(_shopEquipmentRandomCardInfo.ShopCardRandomData.name);
				costGameResData = _shopEquipmentRandomCardInfo.ShopCardRandomData.costGameResData;
				if (_shopEquipmentRandomCardInfo.RemainFreeTimes > 0 && _shopEquipmentRandomCardInfo.NextFreeBuyCountDownTime <= 0)
				{
					costType = 0;
				}
//				consumTipType = _shopEquipmentRandomCardInfo.ShopCardRandomData.buyType == 1 ? ConsumeTipType.DiamondSingleLotteryEquip : ConsumeTipType.None;
			}
			else if (_shopDiamondItemInfo != null)
			{
				shopID = _shopDiamondItemInfo.ShopDiamondData.shopID;
				shopItemID = _shopDiamondItemInfo.ShopDiamondData.id;
				shopItemName = Localization.Get(_shopDiamondItemInfo.ShopDiamondData.name);
				costGameResData = _shopDiamondItemInfo.ShopDiamondData.costGameResData;
			}
			else if (_shopActionItemInfo != null)
			{
				shopID = _shopActionItemInfo.ShopLimitItemData.shopID;
				shopItemID = _shopActionItemInfo.ShopLimitItemData.id;
				shopItemName = Localization.Get(_shopActionItemInfo.ShopLimitItemData.name);
				costGameResData = _shopActionItemInfo.ShopLimitItemData.costGameResData;
				BaseResType resourseResType = _shopActionItemInfo.ShopLimitItemData.resourseGameResData.type;
				if(resourseResType == BaseResType.TowerAction)
					consumTipType = ConsumeTipType.DiamondBuyWorldTreeCount;
				else if(resourseResType == BaseResType.PvpAction)
					consumTipType = ConsumeTipType.DiamondBuyPvpCount;
				else if(resourseResType == BaseResType.PveAction)
					consumTipType = ConsumeTipType.DiamondBuyPveAction;
			}
			else if (_shopGoldItemInfo != null)
			{
				shopID = _shopGoldItemInfo.ShopLimitItemData.shopID;
				shopItemID = _shopGoldItemInfo.ShopLimitItemData.id;
				shopItemName = Localization.Get(_shopGoldItemInfo.ShopLimitItemData.name);
				costGameResData = _shopGoldItemInfo.ShopLimitItemData.costGameResData;
				consumTipType = ConsumeTipType.DiamondBuyCoin;
			}
			else if (_shopGoodsItemInfo != null)
			{
				shopID = _shopGoodsItemInfo.ShopGoodsData.shopID;
				shopItemID = _shopGoodsItemInfo.ShopGoodsData.id;
				shopItemName = Localization.Get(_shopGoodsItemInfo.ShopGoodsData.name);
				costGameResData = _shopGoodsItemInfo.ShopGoodsData.costGameResData;
			}

			// 对于限制购买次数的物品，先判断是否还有购买次数
			if (_shopActionItemInfo != null|| _shopGoldItemInfo != null)
			{
				ShopLimitItemData shopLimitItemData = null;
				if (_shopActionItemInfo != null)
				{
					shopLimitItemData = _shopActionItemInfo.ShopLimitItemData;
					if (shopLimitItemData.baseResType == BaseResType.PveAction)
					{
						if (shopLimitItemData.id > VIPProxy.instance.VIPData.pveActionBuyTimes)
						{
							ConfirmTipsView.Open(Localization.Get("ui.shop_view.not_enough_remain_times_tips"), ClickGoToBuyDiamond);
							return;
						}
					}
					else if (shopLimitItemData.baseResType == BaseResType.TowerAction)
					{
						if (shopLimitItemData.id > VIPProxy.instance.VIPData.worldTreeActionBuyTimes)
						{
							ConfirmTipsView.Open(Localization.Get("ui.shop_view.not_enough_remain_times_tips"), ClickGoToBuyDiamond);
							return;
						}
					}
				}
				else if (_shopGoldItemInfo != null)
				{
					shopLimitItemData = _shopGoldItemInfo.ShopLimitItemData;
					if (shopLimitItemData.id > VIPProxy.instance.VIPData.goldBuyTimes)
					{
						ConfirmTipsView.Open(Localization.Get("ui.shop_view.not_enough_remain_times_tips"), ClickGoToBuyDiamond);
						return;
					}
				}
			}
			// 对于限制购买次数的物品，先判断是否还有购买次数

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
						if (costGameResData.type == BaseResType.Diamond)
						{
							string diamondNotEnoughTipsString = Localization.Get("ui.common_tips.not_enough_diamond_and_go_to_buy");
							ConfirmTipsView.Open(diamondNotEnoughTipsString, ClickGoToBuyDiamond, ConsumeTipType.None);
						}
						else
						{
							CommonErrorTipsView.Open(string.Format(Localization.Get("ui.common_tips.not_enough_resource"), GameResUtil.GetBaseResName(costGameResData.type)));
						}
						return;
					}

					if (costGameResData.type == Logic.Enums.BaseResType.Diamond)
					{ 
						if(ConsumeTipProxy.instance.GetConsumeTipEnable(consumTipType))
							ConfirmBuyShopItemTipsView.Open(shopItemName, costGameResData, ClickConfirmBuyHandler,consumTipType);
						else 
							ClickConfirmBuyHandler();
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
				Debugger.Log(string.Format("[ShopItemView]shopid:{0},shopitemid:{1},costtype:{2}",shopID,shopItemID,costType));
			}
		}

		public void ClickConfirmBuyHandler ()
		{
			int shopID = 0;
			int shopItemID = 0;
			GameResData costGameResData = null;
			int costType = 1;
			if (_shopHeroRandomCardInfo != null)
			{
				shopID = _shopHeroRandomCardInfo.ShopCardRandomData.shopID;
				shopItemID = _shopHeroRandomCardInfo.ShopCardRandomData.id;
				costGameResData = _shopHeroRandomCardInfo.ShopCardRandomData.costGameResData;
				if (_shopHeroRandomCardInfo.RemainFreeTimes > 0 && _shopHeroRandomCardInfo.NextFreeBuyCountDownTime <= 0)
				{
					costType = 0;
				}
			}
			else if (_shopEquipmentRandomCardInfo != null)
			{
				shopID = _shopEquipmentRandomCardInfo.ShopCardRandomData.shopID;
				shopItemID = _shopEquipmentRandomCardInfo.ShopCardRandomData.id;
				costGameResData = _shopEquipmentRandomCardInfo.ShopCardRandomData.costGameResData;
				if (_shopEquipmentRandomCardInfo.RemainFreeTimes > 0 && _shopEquipmentRandomCardInfo.NextFreeBuyCountDownTime <= 0)
				{
					costType = 0;
				}
			}
			else if (_shopDiamondItemInfo != null)
			{
				shopID = _shopDiamondItemInfo.ShopDiamondData.shopID;
				shopItemID = _shopDiamondItemInfo.ShopDiamondData.id;
				costGameResData = _shopDiamondItemInfo.ShopDiamondData.costGameResData;
			}
			else if (_shopActionItemInfo != null)
			{
				shopID = _shopActionItemInfo.ShopLimitItemData.shopID;
				shopItemID = _shopActionItemInfo.ShopLimitItemData.id;
				costGameResData = _shopActionItemInfo.ShopLimitItemData.costGameResData;
			}
			else if (_shopGoldItemInfo != null)
			{
				shopID = _shopGoldItemInfo.ShopLimitItemData.shopID;
				shopItemID = _shopGoldItemInfo.ShopLimitItemData.id;
				costGameResData = _shopGoldItemInfo.ShopLimitItemData.costGameResData;
			}
			else if (_shopGoodsItemInfo != null)
			{
				shopID = _shopGoodsItemInfo.ShopGoodsData.shopID;
				shopItemID = _shopGoodsItemInfo.ShopGoodsData.id;
				costGameResData = _shopGoodsItemInfo.ShopGoodsData.costGameResData;
			}
			ShopController.instance.CLIENT2LOBBY_PURCHASE_GOODS_REQ(shopID, shopItemID, costType);
		}

		public void ClickGoToBuyDiamond ()
		{
			if (FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(Logic.Enums.FunctionOpenType.Shop_Diamond))
			{
				FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction(Logic.Enums.FunctionOpenType.Shop_Diamond);
			}
		}
		#endregion UI event handlers
	}
}
