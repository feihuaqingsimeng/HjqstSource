using System.Collections.Generic;
using Common.Util;
using Logic.Game.Model;
using Logic.Item.Model;
using Logic.Hero.Model;

namespace Logic.Shop.Model
{
	public class ShopGoodsData
	{
		private static Dictionary<int, ShopGoodsData> _shopGoodsDataDictionary;
		public static Dictionary<int, ShopGoodsData> GetShopGoodsData ()
		{
			if (_shopGoodsDataDictionary == null)
			{
				_shopGoodsDataDictionary = CSVUtil.Parse<int, ShopGoodsData>("config/csv/shop_goods", "id");
			}
			return _shopGoodsDataDictionary;
		}

		[CSVElement("id")]
		public int id;

		[CSVElement("sheet_num")]
		public int shopID;

		[CSVElement("buy_type")]
		public int buyType;

		public GameResData goodsGameResData;
		[CSVElement("item")]
		public string item
		{
			set
			{
				goodsGameResData = new GameResData(value);
			}
		}

		[CSVElement("name")]
		public string overrideName;
		public string name
		{
			get
			{
				string realName = overrideName;
				if (string.IsNullOrEmpty(realName))
				{
					if (goodsGameResData.type == Logic.Enums.BaseResType.Item)
					{
						realName = ItemData.GetItemDataByID(goodsGameResData.id).name;
					}
					else if (goodsGameResData.type == Logic.Enums.BaseResType.Hero)
					{
						realName = HeroData.GetHeroDataByID(goodsGameResData.id).name;
					}
				}
				return realName;
			}
		}

		[CSVElement("des")]
		public string overrideDescription;
		public string description
		{
			get
			{
				string realDescription = overrideDescription;
				if (string.IsNullOrEmpty(realDescription))
				{
					if (goodsGameResData.type == Logic.Enums.BaseResType.Item)
					{
						realDescription = ItemData.GetItemDataByID(goodsGameResData.id).des;
					}
					else if (goodsGameResData.type == Logic.Enums.BaseResType.Hero)
					{
						realDescription = HeroData.GetHeroDataByID(goodsGameResData.id).description;
					}
				}
				return realDescription;
			}
		}

		[CSVElement("pic")]
		public string pic;

		public GameResData costGameResData;
		[CSVElement("cost")]
		public string cost
		{
			set
			{
				costGameResData = new GameResData(value);
			}
		}

		[CSVElement("item_num")]
		public int itemNum;
	}
}