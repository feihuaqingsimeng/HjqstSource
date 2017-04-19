using UnityEngine;
using System.Collections;
using Logic.Protocol.Model;
using Logic.UI.BlackMarket.Model;

namespace Logic.UI.BlackMarket.Controller
{
	public class BlackMarketController : SingletonMono<BlackMarketController> 
	{
		void Awake()
		{
			instance = this;
		}
		void Start ()
		{
			Observers.Facade.Instance.RegisterObserver(((int)MSG.BlackMarketResp).ToString(),LOBBY2CLIENT_BlackMarket_RESP_handler);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.BlackMarketUpdateResp).ToString(),LOBBY2CLIENT_BlackMarket_Update_RESP_handler);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.PurchaseBlackGoodsResp).ToString(),LOBBY2CLIENT_Purchase_BlackGoods_RESP_handler);
		}
		
		void OnDestroy ()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver(((int)MSG.BlackMarketResp).ToString(),LOBBY2CLIENT_BlackMarket_RESP_handler);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.BlackMarketUpdateResp).ToString(),LOBBY2CLIENT_BlackMarket_Update_RESP_handler);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.PurchaseBlackGoodsResp).ToString(),LOBBY2CLIENT_Purchase_BlackGoods_RESP_handler);
				
			}
		}
		#region  client to server
		//请求黑市商品信息
		public void CLIENT2LOBBY_BlackMarket_REQ()
		{
			BlackMarketReq req = new BlackMarketReq();
			Protocol.ProtocolProxy.instance.SendProtocol(req);
		}
		//请求交换黑市商品
		public void CLIENT2LOBBY_BlackMarket_Exchange_REQ(int marketId)
		{
			PurchaseBlackGoodsReq req = new PurchaseBlackGoodsReq();
			req.marketId = marketId;
			Protocol.ProtocolProxy.instance.SendProtocol(req);
			
		}

		#endregion

		#region sever
		//响应黑市商品信息
		private bool LOBBY2CLIENT_BlackMarket_RESP_handler(Observers.Interfaces.INotification note)
		{
//			BlackMarketResp resp = note.Body as BlackMarketResp;
//			BlackMarketProxy.instance.UpdateAllBlackMarketInfo(resp.goods);
//			BlackMarketProxy.instance.limitActiavityRefreshTime = resp.limitGoodsNextRefreshTime;
//			BlackMarketProxy.instance.UpdateAllBlackMarketDelegateByProtocol();
			return true;
		}
		//黑市商品信息更新
		private bool LOBBY2CLIENT_BlackMarket_Update_RESP_handler(Observers.Interfaces.INotification note)
		{
//			BlackMarketUpdateResp resp = note.Body as BlackMarketUpdateResp;
//			BlackMarketProxy.instance.UpdateBlackMarketInfo(resp.goods);
//			BlackMarketProxy.instance.UpdateBlackMarketDelegateByProtocol();
			return true;
		}
		private bool LOBBY2CLIENT_Purchase_BlackGoods_RESP_handler(Observers.Interfaces.INotification note)
		{
			BlackMarketProxy.instance.UpdatePurchaseGoodsDelegateByProtocol();
			return true;
		}
		#endregion
	}
}

