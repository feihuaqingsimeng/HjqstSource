using UnityEngine;
using System.Collections.Generic;
using Logic.Enums;
using Logic.Protocol.Model;
using Logic.Shop.Model;
using Logic.Hero.Model;
using Logic.Equipment.Model;
using Logic.UI.Shop.View;
using LuaInterface;

namespace Logic.UI.Shop.Model
{
	public class ShopProxy : SingletonMono<ShopProxy>
	{
		private LuaTable _shopModelLuaTable;
		public LuaTable ShopModelLuaTable
		{
			get
			{
				if (_shopModelLuaTable == null)
					_shopModelLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "shop_model")[0];
				return _shopModelLuaTable;
			}
		}

		public delegate void OnShopHeroRandomCardInfoListUpdateDelegate();
		public delegate void OnShopEquipmentRandomCardInfoListUpdateDelegate();
		public delegate void OnShopDiamondItemInfoListUpdateDelegate();
		public delegate void OnShopActionItemInfoListUpdateDelegate();
		public delegate void OnShopGoldItemInfoListUpdateDelegate();
		public delegate void OnShopGoodsItemInfoListUpdateDelegate();

		public OnShopHeroRandomCardInfoListUpdateDelegate onShopHeroRandomCardInfoListUpdateDelegate;
		public OnShopEquipmentRandomCardInfoListUpdateDelegate onShopEquipmentRandomCardInfoListUpdateDelegate;
		public OnShopDiamondItemInfoListUpdateDelegate onShopDiamondItemInfoListUpdateDelegate;
		public OnShopActionItemInfoListUpdateDelegate onShopActionItemInfoListUpdateDelegate;
		public OnShopGoldItemInfoListUpdateDelegate onShopGoldItemInfoListUpdateDelegate;
		public OnShopGoodsItemInfoListUpdateDelegate onShopGoodsItemInfoListUpdateDelegate;

		private Dictionary<int, ShopHeroRandomCardInfo> _shopHeroRandomCardInfoDic = new Dictionary<int, ShopHeroRandomCardInfo>();
		public Dictionary<int, ShopHeroRandomCardInfo> ShopHeroRandomCardInfoDic
		{
			get
			{
				return _shopHeroRandomCardInfoDic;
			}
		}
		private Dictionary<int, ShopEquipmentRandomCardInfo> _shopEquipmentRandomCardInfoDic = new Dictionary<int, ShopEquipmentRandomCardInfo>();
		public Dictionary<int, ShopEquipmentRandomCardInfo> ShopEquipmentRandomCardInfoDic
		{
			get
			{
				return _shopEquipmentRandomCardInfoDic;
			}
		}
		private Dictionary<int, ShopDiamondItemInfo> _shopDiamondItemInfoDic = new Dictionary<int, ShopDiamondItemInfo>();
		public Dictionary<int, ShopDiamondItemInfo> ShopDiamondItemInfoDic
		{
			get
			{
				return _shopDiamondItemInfoDic;
			}
		}
		private Dictionary<int, ShopActionItemInfo> _shopActionItemInfoDic = new Dictionary<int, ShopActionItemInfo>();
		public Dictionary<int, ShopActionItemInfo> ShopActionItemInfoDic
		{
			get
			{
				return _shopActionItemInfoDic;
			}
		}
		private Dictionary<int, ShopGoldItemInfo> _shopGoldItemInfoDic = new Dictionary<int, ShopGoldItemInfo>();
		public Dictionary<int, ShopGoldItemInfo> ShopGoldItemInfoDic
		{
			get
			{
				return _shopGoldItemInfoDic;
			}
		}
		private Dictionary<int, ShopGoodsItemInfo> _shopGoodsItemInfoDic = new Dictionary<int, ShopGoodsItemInfo>();
		public Dictionary<int, ShopGoodsItemInfo> ShopGoodsItemInfoDic
		{
			get
			{
				return _shopGoodsItemInfoDic;
			}
		}

		void Awake ()
		{
			instance = this;
			LoadShopHeroRandomCardInfoDic();
			LoadShopEquipmentRandomCardInfoDic();
			LoadShopDiamondItemInfo();
			LoadShopActionItemInfo();
			LoadShopGoldItemInfo();
			LoadShopGoodsItemInfo();
		}

		private void LoadShopHeroRandomCardInfoDic ()
		{
			List<ShopCardRandomData> shopHeroRandomCardDataList = ShopCardRandomData.GetHeroRandomItemDataList();
			int heroRandomCardDataCount = shopHeroRandomCardDataList.Count;
			for (int i = 0; i < heroRandomCardDataCount; i++)
			{
				ShopHeroRandomCardInfo shopHeroRandomCardInfo = new ShopHeroRandomCardInfo(shopHeroRandomCardDataList[i]);
				_shopHeroRandomCardInfoDic.Add(shopHeroRandomCardDataList[i].id, shopHeroRandomCardInfo);
			}
		}

		private void LoadShopEquipmentRandomCardInfoDic ()
		{
			List<ShopCardRandomData> shopEquipmentRandomCardDataList = ShopCardRandomData.GetEquipmentRandomDataList();
			int equipmentRandomCardDataCount = shopEquipmentRandomCardDataList.Count;
			for (int i = 0; i < equipmentRandomCardDataCount; i++)
			{
				ShopEquipmentRandomCardInfo shopEquipmentRandomCardInfo = new ShopEquipmentRandomCardInfo(shopEquipmentRandomCardDataList[i]);
				_shopEquipmentRandomCardInfoDic.Add(shopEquipmentRandomCardDataList[i].id, shopEquipmentRandomCardInfo);
			}
		}

		public void LoadShopDiamondItemInfo ()
		{
			List<ShopDiamondData> shopDiamondDataList = ShopDiamondData.GetShopDiamondData().GetValues();
			int shopDiamondDataCount = shopDiamondDataList.Count;
			for (int i = 0; i < shopDiamondDataCount; i++)
			{
				ShopDiamondItemInfo diamondItemInfo = new ShopDiamondItemInfo(shopDiamondDataList[i]);
				_shopDiamondItemInfoDic.Add(shopDiamondDataList[i].id, diamondItemInfo);
			}
		}

		public void LoadShopActionItemInfo ()
		{
			List<ShopLimitItemData> shopActionLimitDataList = ShopLimitItemData.GetActionItemDatalist();
			int shopActionLimitDataCount = shopActionLimitDataList.Count;
			for (int i = 0; i < shopActionLimitDataCount; i++)
			{
				ShopActionItemInfo shopActionItemInfo = new ShopActionItemInfo(shopActionLimitDataList[i]);
				_shopActionItemInfoDic.Add(shopActionLimitDataList[i].id, shopActionItemInfo);
			}
		}

		public void LoadShopGoldItemInfo ()
		{
			List<ShopLimitItemData> shopGoldLimitDataList = ShopLimitItemData.GetGoldItemDataList();
			int shopGoldLimitDataCount = shopGoldLimitDataList.Count;
			for (int i = 0; i < shopGoldLimitDataCount; i++)
			{
				ShopGoldItemInfo shopGoldItemInfo = new ShopGoldItemInfo(shopGoldLimitDataList[i]);
				_shopGoldItemInfoDic.Add(shopGoldLimitDataList[i].id, shopGoldItemInfo);
			}
		}

		public void LoadShopGoodsItemInfo ()
		{
			List<ShopGoodsData> shopGoodsDataList = ShopGoodsData.GetShopGoodsData().GetValues();
			int shopGoodsDataCount = shopGoodsDataList.Count;
			for (int i = 0; i < shopGoodsDataCount; i++)
			{
				ShopGoodsItemInfo shopGoodsItemInfo = new ShopGoodsItemInfo(shopGoodsDataList[i]);
				_shopGoodsItemInfoDic.Add(shopGoodsDataList[i].id, shopGoodsItemInfo);
			}
		}

		public void OnShopHeroRandomCardInfoListUpdate ()
		{
			if (onShopHeroRandomCardInfoListUpdateDelegate != null)
			{
				onShopHeroRandomCardInfoListUpdateDelegate();
			}
		}

		public void OnShopEquipmentRandomCardInfoListUpdate ()
		{
			if (onShopEquipmentRandomCardInfoListUpdateDelegate != null)
			{
				onShopEquipmentRandomCardInfoListUpdateDelegate();
			}
		}

		public void OnShopDiamondItemInfoListUpdate ()
		{
			if (onShopDiamondItemInfoListUpdateDelegate != null)
			{
				onShopDiamondItemInfoListUpdateDelegate();
			}
		}

		public void OnShopActionItemInfoListUpdate()
		{
			if (onShopActionItemInfoListUpdateDelegate != null)
			{
				onShopActionItemInfoListUpdateDelegate();
			}
		}

		public void OnShopGoldItemInfoListUpdate ()
		{
			if (onShopGoldItemInfoListUpdateDelegate != null)
			{
				onShopGoldItemInfoListUpdateDelegate();
			}
		}

		public void OnShopGoodsItemInfoListUpdate()
		{
			if (onShopGoodsItemInfoListUpdateDelegate != null)
			{
				onShopGoodsItemInfoListUpdateDelegate();
			}
		}

		public int GetCurrentFreeItemsCount ()
		{
			return ShopModelLuaTable.GetLuaFunction("GetCurrentFreeItemsCount").Call(null)[0].ToString().ToInt32();
		}
	}
}
