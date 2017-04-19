using Common.GameTime.Controller;

namespace Logic.Shop.Model
{
	public class ShopHeroRandomCardInfo
	{
		private ShopCardRandomData _shopCardRandomData;
		public ShopCardRandomData ShopCardRandomData
		{
			get
			{
				return _shopCardRandomData;
			}
		}

		private int _remainFreeTimes;
		public int RemainFreeTimes
		{
			get
			{
				return _remainFreeTimes;
			}
		}

		private long _nextFreeBuyTimeStamp;
		public long NextFreeBuyCountDownTime
		{
			get
			{
				return TimeController.instance.GetDiffTimeWithServerTimeInSecond(_nextFreeBuyTimeStamp);
			}
		}

		public ShopHeroRandomCardInfo (ShopCardRandomData shopCardRandomData)
		{
			_shopCardRandomData = shopCardRandomData;
		}

		public void setFreeInfo (int remainFreeTimes, long nextFreeBuyTimeStamp)
		{
			_remainFreeTimes = remainFreeTimes;
			_nextFreeBuyTimeStamp = nextFreeBuyTimeStamp;
		}
	}
}
