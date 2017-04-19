using System.Collections.Generic;
using Common.Util;
using Logic.Enums;
using Logic.Game.Model;

namespace Logic.Shop.Model
{
	public class ShopLimitItemData
	{
		private static Dictionary<int, ShopLimitItemData> _shopLimitItemDataDictionary;
		public static Dictionary<int, ShopLimitItemData> GetShopLimitItemDataDictionary ()
		{
			if (_shopLimitItemDataDictionary == null)
			{
				_shopLimitItemDataDictionary = CSVUtil.Parse<int, ShopLimitItemData>("config/csv/shop_limit_item", "id");
			}
			return _shopLimitItemDataDictionary;
		}

		public static List<ShopLimitItemData> GetActionItemDatalist ()
		{
			List<ShopLimitItemData> actionItemDataList = new List<ShopLimitItemData>();
			List<ShopLimitItemData> shopLimitItemDataList = GetShopLimitItemDataDictionary().GetValues();
			int shopLimitItemDataCount = shopLimitItemDataList.Count;
			for (int i = 0; i < shopLimitItemDataCount; i++)
			{
				if (shopLimitItemDataList[i].shopID == 14)
				{
					actionItemDataList.Add(shopLimitItemDataList[i]);
				}
			}
			return actionItemDataList;
		}

		public static List<ShopLimitItemData> GetGoldItemDataList ()
		{
			List<ShopLimitItemData> goldItemDataList = new List<ShopLimitItemData>();
			List<ShopLimitItemData> shopLimitItemDataList = GetShopLimitItemDataDictionary().GetValues();
			int shopLimitItemDataCount = shopLimitItemDataList.Count;
			for (int i = 0; i < shopLimitItemDataCount; i++)
			{
				if (shopLimitItemDataList[i].shopID == 15)
				{
					goldItemDataList.Add(shopLimitItemDataList[i]);
				}
			}
			return goldItemDataList;
		}

		[CSVElement("id")]
		public int id;
		
		[CSVElement("sheet_num")]
		public int shopID;
		
		[CSVElement("buy_type")]
		public int buyType;

		public BaseResType baseResType;
		[CSVElement("type")]
		public int type
		{
			set
			{
				baseResType = (BaseResType)value;
			}
		}
		public GameResData resourseGameResData;
		[CSVElement("resource")]
		public string resource
		{
			set
			{
				resourseGameResData = new GameResData(value);
			}
		}

		[CSVElement("name")]
		public string name;

		[CSVElement("pic")]
		public string pic;

		[CSVElement("des")]
		public string description;

		public GameResData costGameResData;
		[CSVElement("cost")]
		public string cost
		{
			set
			{
				costGameResData = new GameResData(value);
			}
		}

		[CSVElement("pre_item")]
		public int preItem;

		[CSVElement("item_num")]
		public int itemNum;
	}	
}
