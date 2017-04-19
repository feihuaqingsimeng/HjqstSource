namespace Logic.Shop.Model
{
	public class ShopGoodsItemInfo
	{
		private ShopGoodsData _shopGoodsData;
		public ShopGoodsData ShopGoodsData
		{
			get
			{
				return _shopGoodsData;
			}
		}

		private int _remainPurchaseTimes = 0;
		public int RemainPurchaseTimes
		{
			get
			{
				return _remainPurchaseTimes;
			}
		}

		public ShopGoodsItemInfo (ShopGoodsData shopGoodsData)
		{
			_shopGoodsData = shopGoodsData;
		}

		public void UpdateInfo (int remainPurchaseTimes)
		{
			_remainPurchaseTimes = remainPurchaseTimes;
		}
	}
}
