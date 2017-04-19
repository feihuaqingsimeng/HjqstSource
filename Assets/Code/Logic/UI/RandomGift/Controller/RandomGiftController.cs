using UnityEngine;
using System.Collections;
using Logic.Protocol.Model;
using Logic.UI.RandomGift.Model;

namespace Logic.UI.RandomGift.Controller
{
	public class RandomGiftController : SingletonMono<RandomGiftController>
	{

		void Awake()
		{
			instance = this;
		}

		void Start () {
			Observers.Facade.Instance.RegisterObserver(((int)MSG.OpenGiftBagResp).ToString(),LOBBY2CLIENT_OpenGiftBag_RESP_handler);
			
		}
		void OnDestroy ()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver(((int)MSG.OpenGiftBagResp).ToString(),LOBBY2CLIENT_OpenGiftBag_RESP_handler);
				
			}
		}

		#region client to server
		public void CLIENT2LOBBY_OPEN_RANDOM_GIFT_REQ(int itemid)
		{
			OpenGiftBagReq req = new OpenGiftBagReq();
			req.giftBagNo = itemid;
			Protocol.ProtocolProxy.instance.SendProtocol(req);
		}
		#endregion

		#region server to client
		private bool LOBBY2CLIENT_OpenGiftBag_RESP_handler(Observers.Interfaces.INotification note)
		{
			OpenGiftBagResp resp = note.Body as OpenGiftBagResp;
			RandomGiftProxy.instance.RandomGiftRefreshByProtocol(resp.dropItems);
			return true;
		}
		#endregion
	}
}

