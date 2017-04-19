namespace Logic.Shop.Model
{
	public class ShopDiamondItemInfo
	{
		private ShopDiamondData _shopDiamondData;
		public ShopDiamondData ShopDiamondData
		{
			get
			{
				return _shopDiamondData;
			}
		}

		public ShopDiamondItemInfo (ShopDiamondData shopDiamondData)
		{
			_shopDiamondData = shopDiamondData;
		}
	}
}
