using UnityEngine;
using System.Collections.Generic;
using Common.Localization;
using Logic.Protocol;
using Logic.Protocol.Model;
using Logic.Enums;
using Logic.Game.Model;
using Logic.Hero.Model;
using Logic.Equipment.Model;
using Logic.Shop.Model;
using Logic.UI.Shop.Model;
using Logic.UI.Tips.View;
using Logic.Item.Model;

namespace Logic.UI.Shop.Controller
{
	public class ShopController : SingletonMono<ShopController>
	{
		void Awake ()
		{
			instance = this;
		}

		void Start ()
		{
			Observers.Facade.Instance.RegisterObserver(((int)MSG.PurchaseGoodsResp).ToString(), LOBBY2CLIENT_PURCHASE_GOODS_RESP);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.PurchaseDrawCardGoodsResp).ToString(), LOBBY2CLIENT_PURCHASE_DRAW_CARD_GOODS_RESP);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.DrawCardGoodsResp).ToString(), LOBBY2CLIENT_DRAW_CARD_GOODS_RESP);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.DrawCardGoodsUpdateResp).ToString(), LOBBY2CLIENT_DRAW_CARD_GOODS_UPDATE_RESP);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.LimitGoodsResp).ToString(), LOBBY2CLIENT_LIMIT_GOODS_RESP);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.LimitGoodsUpdateResp).ToString(), LOBBY2CLIENT_LIMIT_GOODS_UPDATE_RESP);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.OtherGoodsResp).ToString(), LOBBY2CLIENT_OTHER_GOODS_RESP);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.OtherGoodsUpdateResp).ToString(), LOBBY2CLIENT_OTEHR_GOODS_UPDATE_RESP);
		}

		void OnDestroy ()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver(((int)MSG.PurchaseGoodsResp).ToString(), LOBBY2CLIENT_PURCHASE_GOODS_RESP);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.PurchaseDrawCardGoodsResp).ToString(), LOBBY2CLIENT_PURCHASE_DRAW_CARD_GOODS_RESP);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.DrawCardGoodsResp).ToString(), LOBBY2CLIENT_DRAW_CARD_GOODS_RESP);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.DrawCardGoodsUpdateResp).ToString(), LOBBY2CLIENT_DRAW_CARD_GOODS_UPDATE_RESP);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.LimitGoodsResp).ToString(), LOBBY2CLIENT_LIMIT_GOODS_RESP);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.LimitGoodsUpdateResp).ToString(), LOBBY2CLIENT_LIMIT_GOODS_UPDATE_RESP);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.OtherGoodsResp).ToString(), LOBBY2CLIENT_OTHER_GOODS_RESP);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.OtherGoodsUpdateResp).ToString(), LOBBY2CLIENT_OTEHR_GOODS_UPDATE_RESP);
			}
		}

		#region purchase goods
		public void CLIENT2LOBBY_PURCHASE_GOODS_REQ (int shopID, int shopItemID, int costType)
		{
			PurchaseGoodsReq purchaseGoodsReq = new PurchaseGoodsReq();
			purchaseGoodsReq.goodsType = shopID;
			purchaseGoodsReq.goodsNo = shopItemID;
			purchaseGoodsReq.costType = costType;
			ProtocolProxy.instance.SendProtocol(purchaseGoodsReq);
		}

		public bool LOBBY2CLIENT_PURCHASE_GOODS_RESP (Observers.Interfaces.INotification note)
		{
			PurchaseGoodsResp purchaseGoodsResp = note.Body as PurchaseGoodsResp;
			int shopID = purchaseGoodsResp.goodsType;
			int shopItemID = purchaseGoodsResp.goodsNo;
			int costType = purchaseGoodsResp.costType;
			string shopItemName = string.Empty;
			if (shopID == 13)
			{
				ShopDiamondItemInfo shopDiamondInfo = null;
				ShopProxy.instance.ShopDiamondItemInfoDic.TryGetValue(shopItemID, out shopDiamondInfo);
				shopItemName = Localization.Get(shopDiamondInfo.ShopDiamondData.name);
			}
			else if (shopID == 14)
			{
				ShopActionItemInfo shopActionItemInfo = null;
				ShopProxy.instance.ShopActionItemInfoDic.TryGetValue(shopItemID, out shopActionItemInfo);
				shopItemName = Localization.Get(shopActionItemInfo.ShopLimitItemData.name);
			}
			else if (shopID == 15)
			{
				ShopGoldItemInfo shopGoldItemInfo = null;
				ShopProxy.instance.ShopGoldItemInfoDic.TryGetValue(shopItemID, out shopGoldItemInfo);
				shopItemName = Localization.Get(shopGoldItemInfo.ShopLimitItemData.name);
			}
			else if (shopID == 16)
			{
				ShopGoodsItemInfo shopGoodsItemInfo = null;
				ShopProxy.instance.ShopGoodsItemInfoDic.TryGetValue(shopItemID, out shopGoodsItemInfo);
				shopItemName = Localization.Get(shopGoodsItemInfo.ShopGoodsData.name);
			}
			CommonAutoDestroyTipsView.Open(string.Format(Localization.Get("ui.shop_view.buy_success_tips"), shopItemName));
			return true;
		}

		public bool LOBBY2CLIENT_PURCHASE_DRAW_CARD_GOODS_RESP (Observers.Interfaces.INotification note)
		{
			PurchaseDrawCardGoodsResp purchaseDrawCardGoodsResp = note.Body as PurchaseDrawCardGoodsResp;
			int cardRandomID = purchaseDrawCardGoodsResp.goodsNo;
			ShopHeroRandomCardInfo shopHeroRandomCardInfo = null;
			ShopEquipmentRandomCardInfo shopEquipmentRandomCardInfo = null;
			ShopProxy.instance.ShopHeroRandomCardInfoDic.TryGetValue(cardRandomID, out shopHeroRandomCardInfo);
			ShopProxy.instance.ShopEquipmentRandomCardInfoDic.TryGetValue(cardRandomID, out shopEquipmentRandomCardInfo);
			if (shopHeroRandomCardInfo != null)
			{
				if (shopHeroRandomCardInfo.ShopCardRandomData.buyType == 1)
				{
					HeroInfo rewardHeroInfo = new HeroInfo(purchaseDrawCardGoodsResp.commonGoods[0]);
					Logic.UI.Shop.View.RecruitOneHeroResultView recruitOneHeroResultView = UIMgr.instance.Open<Logic.UI.Shop.View.RecruitOneHeroResultView>(Logic.UI.Shop.View.RecruitOneHeroResultView.PREFAB_PATH);
					recruitOneHeroResultView.SetInfo(shopHeroRandomCardInfo, rewardHeroInfo);
				}
				else if (shopHeroRandomCardInfo.ShopCardRandomData.buyType == 2)
				{
					List<HeroInfo> rewardCommonHeroList = new List<HeroInfo>();
					int commonGoodsCount = purchaseDrawCardGoodsResp.commonGoods.Count;
					for (int i = 0; i < commonGoodsCount; i++)
					{
						HeroInfo heroInfo = new HeroInfo(purchaseDrawCardGoodsResp.commonGoods[i]);
						rewardCommonHeroList.Add(heroInfo);
					}
					HeroInfo giftHeroInfo = new HeroInfo(purchaseDrawCardGoodsResp.specialGoods);
					Logic.UI.Shop.View.RecruitResultView recruitResultView = UIMgr.instance.Open<Logic.UI.Shop.View.RecruitResultView>(Logic.UI.Shop.View.RecruitResultView.PREFAB_PATH);
					recruitResultView.SetNewHeroInfoList(shopHeroRandomCardInfo ,rewardCommonHeroList, giftHeroInfo);
				}
			}
			else if (shopEquipmentRandomCardInfo != null)
			{
				Logic.UI.Shop.View.DrawEquipmentResultView drawEquipmentResultView = UIMgr.instance.Open<Logic.UI.Shop.View.DrawEquipmentResultView>(Logic.UI.Shop.View.DrawEquipmentResultView.PREFAB_PATH);
//				EquipmentInfo equipmentInfo = new EquipmentInfo(purchaseDrawCardGoodsResp.commonGoods[0]);
//				drawEquipmentResultView.SetNewEquipmentInfo(shopEquipmentRandomCardInfo, equipmentInfo);

				ItemInfo itemInfo = new ItemInfo(purchaseDrawCardGoodsResp.commonGoods[0]);
				drawEquipmentResultView.SetNewItemInfo(shopEquipmentRandomCardInfo, itemInfo);
			}
			return true;
		}
		#endregion purchase goods

		#region draw		
		public void CLIENT2LOBBY_DRAW_CARD_GOODS_REQ ()
		{
			DrawCardGoodsReq drawCardGoodsReq = new DrawCardGoodsReq();
			ProtocolProxy.instance.SendProtocol(drawCardGoodsReq);
		}

		public bool LOBBY2CLIENT_DRAW_CARD_GOODS_RESP (Observers.Interfaces.INotification note)
		{
			DrawCardGoodsResp drawCardGoodsResp = note.Body as DrawCardGoodsResp;
			List<DrawCardGoodsProto> drawCardGoodsProtoList = drawCardGoodsResp.goods;
			int drawCardGoodsProtoCount = drawCardGoodsProtoList.Count;
			for (int i = 0; i < drawCardGoodsProtoCount; i++)
			{
				DrawCardGoodsProto drawCardGoodsProto = drawCardGoodsProtoList[i];
				ShopHeroRandomCardInfo shopHeroRandomCardInfo = null;
				ShopEquipmentRandomCardInfo shopEquipmentRandomCardInfo = null;
				ShopProxy.instance.ShopHeroRandomCardInfoDic.TryGetValue(drawCardGoodsProto.goodsNo, out shopHeroRandomCardInfo);
				ShopProxy.instance.ShopEquipmentRandomCardInfoDic.TryGetValue(drawCardGoodsProto.goodsNo, out shopEquipmentRandomCardInfo);
				if (shopHeroRandomCardInfo != null)
				{
					shopHeroRandomCardInfo.setFreeInfo(drawCardGoodsProto.remainFreeTimes, drawCardGoodsProto.freeDrawCoolingOverTime);
				}
				else if (shopEquipmentRandomCardInfo != null)
				{
					shopEquipmentRandomCardInfo.setFreeInfo(drawCardGoodsProto.remainFreeTimes, drawCardGoodsProto.freeDrawCoolingOverTime);
				}
			}
			ShopProxy.instance.OnShopHeroRandomCardInfoListUpdate();
			ShopProxy.instance.OnShopEquipmentRandomCardInfoListUpdate();
			return true;
		}

		public bool LOBBY2CLIENT_DRAW_CARD_GOODS_UPDATE_RESP (Observers.Interfaces.INotification note)
		{
			DrawCardGoodsUpdateResp drawCardGoodsUpdateResp = note.Body as DrawCardGoodsUpdateResp;
			DrawCardGoodsProto drawCardGoodsProto = drawCardGoodsUpdateResp.updateGoods;
			if (drawCardGoodsProto != null)
			{
				ShopHeroRandomCardInfo shopHeroRandomCardInfo = null;
				ShopEquipmentRandomCardInfo shopEquipmentRandomCardInfo = null;
				ShopProxy.instance.ShopHeroRandomCardInfoDic.TryGetValue(drawCardGoodsProto.goodsNo, out shopHeroRandomCardInfo);
				ShopProxy.instance.ShopEquipmentRandomCardInfoDic.TryGetValue(drawCardGoodsProto.goodsNo, out shopEquipmentRandomCardInfo);
				if (shopHeroRandomCardInfo != null)
				{
					shopHeroRandomCardInfo.setFreeInfo(drawCardGoodsProto.remainFreeTimes, drawCardGoodsProto.freeDrawCoolingOverTime);
				}
				else if (shopEquipmentRandomCardInfo != null)
				{
					shopEquipmentRandomCardInfo.setFreeInfo(drawCardGoodsProto.remainFreeTimes, drawCardGoodsProto.freeDrawCoolingOverTime);
				}
				ShopProxy.instance.OnShopHeroRandomCardInfoListUpdate();
				ShopProxy.instance.OnShopEquipmentRandomCardInfoListUpdate();
			}
			return true;
		}
		#endregion draw

		#region limit goods
		public void CLIENT2LOBBY_LIMIT_GOODS_REQ ()
		{
			LimitGoodsReq limitGoodsReq = new LimitGoodsReq();
			ProtocolProxy.instance.SendProtocol(limitGoodsReq);
		}

		public bool LOBBY2CLIENT_LIMIT_GOODS_RESP (Observers.Interfaces.INotification note)
		{
			LimitGoodsResp limitGoodsResp = note.Body as LimitGoodsResp;

			List<ShopActionItemInfo> shopActionItemInfoList = ShopProxy.instance.ShopActionItemInfoDic.GetValues();
			int shopActionItemInfoCount = shopActionItemInfoList.Count;
			for (int i = 0; i < shopActionItemInfoCount; i++)
			{
				shopActionItemInfoList[i].UpdateInfo(false, 0);
			}
			List<ShopGoldItemInfo> shopGoldItemInfoList = ShopProxy.instance.ShopGoldItemInfoDic.GetValues();
			int shopGoldItemInfoCount = shopGoldItemInfoList.Count;
			for (int i = 0; i < shopGoldItemInfoCount; i++)
			{
				shopGoldItemInfoList[i].UpdateInfo(false, 0);
			}


			List<LimitGoodsProto> limitGoodsProtoList = limitGoodsResp.goods;
			int limitGoodsProtoCount = limitGoodsProtoList.Count;
			for (int i = 0; i < limitGoodsProtoCount; i++)
			{
				int limitItemID = limitGoodsProtoList[i].goodsNo;
				ShopActionItemInfo shopActionItemInfo = null;
				ShopGoldItemInfo shopGoldItemInfo = null;
				ShopProxy.instance.ShopActionItemInfoDic.TryGetValue(limitItemID, out shopActionItemInfo);
				ShopProxy.instance.ShopGoldItemInfoDic.TryGetValue(limitItemID, out shopGoldItemInfo);

				if (shopActionItemInfo != null)
				{
					shopActionItemInfo.UpdateInfo(true, limitGoodsProtoList[i].remianPurchaseTimes);
				}
				else if (shopGoldItemInfo != null)
				{
					shopGoldItemInfo.UpdateInfo(true, limitGoodsProtoList[i].remianPurchaseTimes);
				}
			}
			ShopProxy.instance.OnShopActionItemInfoListUpdate();
			ShopProxy.instance.OnShopGoldItemInfoListUpdate();
			return true;
		}

		public bool LOBBY2CLIENT_LIMIT_GOODS_UPDATE_RESP (Observers.Interfaces.INotification note)
		{
			LimitGoodsUpdateResp limitGoodsUpdateResp = note.Body as LimitGoodsUpdateResp;
			List<LimitGoodsProto> addLimitGoodsProtoList = limitGoodsUpdateResp.addGoods;
			int addLimitGoodsProtoCount = addLimitGoodsProtoList.Count;
			for (int i = 0; i < addLimitGoodsProtoCount; i++)
			{
				int shopItemID = addLimitGoodsProtoList[i].goodsNo;
				ShopActionItemInfo shopActionItemInfo = null;
				ShopGoldItemInfo shopGoldItemInfo = null;
				ShopProxy.instance.ShopActionItemInfoDic.TryGetValue(shopItemID, out shopActionItemInfo);
				ShopProxy.instance.ShopGoldItemInfoDic.TryGetValue(shopItemID, out shopGoldItemInfo);

				if (shopActionItemInfo != null)
				{
					shopActionItemInfo.UpdateInfo(true, addLimitGoodsProtoList[i].remianPurchaseTimes);
				}
				else if (shopGoldItemInfo != null)
				{
					shopGoldItemInfo.UpdateInfo(true, addLimitGoodsProtoList[i].remianPurchaseTimes);
				}
			}

			int deleteShopItemID = limitGoodsUpdateResp.delGoods;
			ShopActionItemInfo deleteShopActionItemInfo = null;
			ShopGoldItemInfo deleteShopGoldItemInfo = null;
			ShopProxy.instance.ShopActionItemInfoDic.TryGetValue(deleteShopItemID, out deleteShopActionItemInfo);
			ShopProxy.instance.ShopGoldItemInfoDic.TryGetValue(deleteShopItemID, out deleteShopGoldItemInfo);
			
			if (deleteShopActionItemInfo != null)
			{
				deleteShopActionItemInfo.UpdateInfo(false, 0);
			}
			else if (deleteShopGoldItemInfo != null)
			{
				deleteShopGoldItemInfo.UpdateInfo(false, 0);
			}

			LimitGoodsProto updateLimitGoodsProto = limitGoodsUpdateResp.updateGoods;
			if (updateLimitGoodsProto != null)
			{
				ShopActionItemInfo updateShopActionItemInfo = null;
				ShopGoldItemInfo updateShopGoldItemInfo = null;
				ShopProxy.instance.ShopActionItemInfoDic.TryGetValue(updateLimitGoodsProto.goodsNo, out updateShopActionItemInfo);
				ShopProxy.instance.ShopGoldItemInfoDic.TryGetValue(updateLimitGoodsProto.goodsNo, out updateShopGoldItemInfo);
				if (updateShopActionItemInfo != null)
				{
					updateShopActionItemInfo.UpdateInfo(updateShopActionItemInfo.IsOpen, updateLimitGoodsProto.remianPurchaseTimes);
				}
				else if (updateShopGoldItemInfo != null)
				{
					updateShopGoldItemInfo.UpdateInfo(updateShopGoldItemInfo.IsOpen, updateLimitGoodsProto.remianPurchaseTimes);
				}
			}

			ShopProxy.instance.OnShopActionItemInfoListUpdate();
			ShopProxy.instance.OnShopGoldItemInfoListUpdate();
			return true;
		}
		#endregion limit goods

		#region other goods
		public void CLIENT2LOBBY_OTHER_GOODS_REQ ()
		{
			OtherGoodsReq otherGoodsReq = new OtherGoodsReq();
			ProtocolProxy.instance.SendProtocol(otherGoodsReq);
		}

		public bool LOBBY2CLIENT_OTHER_GOODS_RESP (Observers.Interfaces.INotification note)
		{
			OtherGoodsResp otherGoodsResp = note.Body as OtherGoodsResp;

			List<ShopGoodsItemInfo> shopGoodsItemInfoList = ShopProxy.instance.ShopGoodsItemInfoDic.GetValues();
			int shopGoodsItemInfoCount = shopGoodsItemInfoList.Count;
			for (int i = 0; i < shopGoodsItemInfoCount; i++)
			{
				shopGoodsItemInfoList[i].UpdateInfo(0);
			}

			List<OtherGoodsProto> otherGoodsProtoList = otherGoodsResp.goods;
			int otherGoodsProtoCount = otherGoodsProtoList.Count;
			ShopGoodsItemInfo shopGoodsItemInfo = null;
			for (int i = 0; i < otherGoodsProtoCount; i++)
			{
				shopGoodsItemInfo = null;
				ShopProxy.instance.ShopGoodsItemInfoDic.TryGetValue(otherGoodsProtoList[i].goodsNo, out shopGoodsItemInfo);

				if (shopGoodsItemInfo != null)
				{
					shopGoodsItemInfo.UpdateInfo(otherGoodsProtoList[i].remianPurchaseTimes);
				}
			}
			ShopProxy.instance.OnShopGoodsItemInfoListUpdate();
			return true;
		}

		public bool LOBBY2CLIENT_OTEHR_GOODS_UPDATE_RESP (Observers.Interfaces.INotification note)
		{
			OtherGoodsUpdateResp otherGoodsUpdateResp = note.Body as OtherGoodsUpdateResp;
			OtherGoodsProto otherGoodsProto = otherGoodsUpdateResp.updateGoods;
			ShopGoodsItemInfo shopGoodsItemInfo = null;
			ShopProxy.instance.ShopGoodsItemInfoDic.TryGetValue(otherGoodsProto.goodsNo, out shopGoodsItemInfo);
			if (shopGoodsItemInfo != null)
			{
				shopGoodsItemInfo.UpdateInfo(otherGoodsProto.remianPurchaseTimes);
				ShopProxy.instance.OnShopGoodsItemInfoListUpdate();
			}
			return true;
		}
		#endregion other goods
	}
}
