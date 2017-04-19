using System.Collections.Generic;
using Common.Util;
using Logic.Game.Model;

namespace Logic.Shop.Model
{
	public class ShopDiamondData
	{
		private static Dictionary<int, ShopDiamondData> _shopDiamondDataDictionary;
		public static Dictionary<int, ShopDiamondData> GetShopDiamondData ()
		{
			if (_shopDiamondDataDictionary == null)
			{
				_shopDiamondDataDictionary = CSVUtil.Parse<int, ShopDiamondData>("config/csv/shop_diamond", "id");
			}
			return _shopDiamondDataDictionary;
		}

        public static ShopDiamondData GetShopDiamondDataById(int id) 
        {
            if (_shopDiamondDataDictionary == null)
                GetShopDiamondData();
            return _shopDiamondDataDictionary[id];
        }

		[CSVElement("id")]
		public int id;

		[CSVElement("sheet_num")]
		public int shopID;

		[CSVElement("buy_type")]
		public int buyType;

		[CSVElement("resource")]
		public string resource;

		[CSVElement("name")]
		public string name;

        [CSVElement("name1")]
        public string name1;

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

		public GameResData firstAwardGameResData;
		[CSVElement("first_award")]
		public string first_award
		{
			set
			{
//				firstAwardGameResData = new GameResData(value);
			}
		}

		public GameResData buyAwardGameResData;
		[CSVElement("buy_award")]
		public string buy_award
		{
			set
			{
//				buyAwardGameResData = new GameResData(value);
			}
		}

		[CSVElement("double_base")]
		public int double_base;
	}
}
