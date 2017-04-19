namespace Logic.Shop.Model
{
	public class ShopGoldItemInfo
	{
		private ShopLimitItemData _shopLimitItemData;
		public ShopLimitItemData ShopLimitItemData
		{
			get
			{
				return _shopLimitItemData;
			}
		}

		private bool _isOpen = false;
		public bool IsOpen
		{
			get
			{
				return _isOpen;
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

		public ShopGoldItemInfo (ShopLimitItemData shopLimitItemData)
		{
			_shopLimitItemData = shopLimitItemData;
		}

		public void UpdateInfo (bool isOpen, int remainPurchaseTimes)
		{
			_isOpen = isOpen;
			_remainPurchaseTimes = remainPurchaseTimes;
		}
	}
}
