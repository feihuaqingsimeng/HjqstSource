using System.Collections.Generic;
using Common.Util;
using Logic.Game.Model;

namespace Logic.Shop.Model
{
	public class ShopCardRandomData
	{
		private static Dictionary<int, ShopCardRandomData> _shopCardRandomDataDictionary;
		public static Dictionary<int, ShopCardRandomData> GetShopCardRandomDataDictionary ()
		{
			if (_shopCardRandomDataDictionary == null)
			{
				_shopCardRandomDataDictionary = CSVUtil.Parse<int, ShopCardRandomData>("config/csv/shop_card_random", "id");
			}
			return _shopCardRandomDataDictionary;
		}

		public static ShopCardRandomData GetShopCardRandomDataByID (int id)
		{
			ShopCardRandomData shopCardRandomData = null;
			GetShopCardRandomDataDictionary().TryGetValue(id, out shopCardRandomData);
			return shopCardRandomData;
		}

		public static List<ShopCardRandomData> GetHeroRandomItemDataList ()
		{
			List<ShopCardRandomData> heroRandomItemDataList = new List<ShopCardRandomData>();
			List<ShopCardRandomData> allCardRandomItemDataList = GetShopCardRandomDataDictionary().GetValues();
			int cardRandomItemDataCount = allCardRandomItemDataList.Count;
			for (int i = 0; i < cardRandomItemDataCount; i++)
			{
				if (allCardRandomItemDataList[i].shopID == 11)
				{
					heroRandomItemDataList.Add(allCardRandomItemDataList[i]);
				}
			}
			return heroRandomItemDataList;
		}

		public static List<ShopCardRandomData> GetEquipmentRandomDataList ()
		{
			List<ShopCardRandomData> equipmentRandomItemDataList = new List<ShopCardRandomData>();
			List<ShopCardRandomData> allCardRandomItemDataList = GetShopCardRandomDataDictionary().GetValues();
			int cardRandomItemDataCount = allCardRandomItemDataList.Count;
			for (int i = 0; i < cardRandomItemDataCount; i++)
			{
				if (allCardRandomItemDataList[i].shopID == 12)
				{
					equipmentRandomItemDataList.Add(allCardRandomItemDataList[i]);
				}
			}
			return equipmentRandomItemDataList;
		}

		[CSVElement("id")]
		public int id;

		[CSVElement("sheet_num")]
		public int shopID;

		[CSVElement("buy_type")]
		public int buyType;

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

		[CSVElement("free_time")]
		public int freeTime;

		[CSVElement("max")]
		public int max;
	}
}