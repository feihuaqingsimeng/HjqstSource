using UnityEngine;
using System.Collections.Generic;
using Common.Util;
using Logic.Game.Model;

namespace Logic.UI.Shop.Model
{
	public class ShopItemData
	{
		private static Dictionary<int, ShopItemData> _shopItemDataDictionary;

		public static Dictionary<int, ShopItemData> GetShopItemData()
		{
			if (_shopItemDataDictionary == null)
			{
				_shopItemDataDictionary = CSVUtil.Parse<int, ShopItemData>("config/csv/shop_item", "id");
			}
			return _shopItemDataDictionary;
		}

		public static ShopItemData GetShopItemDataByID (int shopItemDataID)
		{
			ShopItemData shopItemData = null;
			GetShopItemData().TryGetValue(shopItemDataID, out shopItemData);
			return shopItemData;
		}

		[CSVElement("id")]
		public int id;

		[CSVElement("shop_sheet")]
		public int shopSheet;

		[CSVElement("buy_type")]
		public int buy_type;

		[CSVElement("itemid")]
		public int itemID;

		[CSVElement("name")]
		public string name;

		[CSVElement("pic")]
		public string pic;

		[CSVElement("des")]
		public string des;

		public GameResData costGameResData;
		[CSVElement("cost")]
		public string cost
		{
			set
			{
				costGameResData = new GameResData(value);
			}
		}

		[CSVElement("free_time")]
		public int freeTime;

		[CSVElement("max")]
		public int max;
	}
}
