using System.Collections.Generic;
using Common.Util;

namespace Logic.Shop.Model
{
	public class ShopData
	{
		private static Dictionary<int, ShopData> _shopDataDictionary;
		public static Dictionary<int, ShopData> GetShopDataDictionary ()
		{
			if (_shopDataDictionary == null)
			{
				_shopDataDictionary = CSVUtil.Parse<int, ShopData>("config/csv/shop", "id");
			}
			return _shopDataDictionary;
		}

		public static ShopData GetShopDataByID (int id)
		{
			ShopData shopData = null;
			GetShopDataDictionary().TryGetValue(id, out shopData);
			return shopData;
		}

		[CSVElement("id")]
		public int id;

		[CSVElement("shop_type")]
		public int shopType;

		[CSVElement("sheet_name")]
		public string name;

		[CSVElement("vip_limit")]
		public int vipLimit;
	}
}