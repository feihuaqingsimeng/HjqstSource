using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Protocol.Model;
using Logic.Item.Model;
using LuaInterface;

namespace Logic.Item.Controller
{
	public class ItemController : SingletonMono<ItemController> 
	{
		
		void Awake()
		{
			instance = this;
		}

		void Start()
		{
			//Observers.Facade.Instance.RegisterObserver(((int)MSG.GetAllItemResp).ToString(),LOBBY2CLIENT_GET_ALL_ITEM_RESP_handler);
			//Observers.Facade.Instance.RegisterObserver(((int)MSG.ItemUpdateResp).ToString(),LOBBY2CLIENT_ITEM_UPDATE_RESP_handler);
			//Observers.Facade.Instance.RegisterObserver(((int)MSG.ExpPotionResp).ToString(),LOBBY2CLIENT_ExpPotion_RESP_handler);
			Observers.Facade.Instance.RegisterObserver("LOBBY2CLIENT_GET_ALL_ITEM_RESP_handler",LOBBY2CLIENT_GET_ALL_ITEM_RESP_handler);
			Observers.Facade.Instance.RegisterObserver("LOBBY2CLIENT_ITEM_UPDATE_RESP_handler",LOBBY2CLIENT_ITEM_UPDATE_RESP_handler);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.BuySweepCouponsResp).ToString(),LOBBY2CLIENT_BuySweepCouponsResp_RESP_handler);
			
		}
		void OnDestroy()
		{
			if (Observers.Facade.Instance != null)
			{
				//Observers.Facade.Instance.RemoveObserver(((int)MSG.GetAllItemResp).ToString(),LOBBY2CLIENT_GET_ALL_ITEM_RESP_handler);
				//Observers.Facade.Instance.RemoveObserver(((int)MSG.ItemUpdateResp).ToString(),LOBBY2CLIENT_ITEM_UPDATE_RESP_handler);
				//Observers.Facade.Instance.RemoveObserver(((int)MSG.ExpPotionResp).ToString(),LOBBY2CLIENT_ExpPotion_RESP_handler);
				Observers.Facade.Instance.RemoveObserver("LOBBY2CLIENT_GET_ALL_ITEM_RESP_handler",LOBBY2CLIENT_GET_ALL_ITEM_RESP_handler);
				Observers.Facade.Instance.RemoveObserver("LOBBY2CLIENT_ITEM_UPDATE_RESP_handler",LOBBY2CLIENT_ITEM_UPDATE_RESP_handler);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.BuySweepCouponsResp).ToString(),LOBBY2CLIENT_BuySweepCouponsResp_RESP_handler);
				
			}
		}

		public void CLIENT2LOBBY_ExpPotion_REQ(int heroid,int potionNo,int num)
		{
			ExpPotionReq req = new ExpPotionReq();
			req.heroId = heroid;
			req.potionNo = potionNo;
			req.num = num;
			Protocol.ProtocolProxy.instance.SendProtocol(req);
		}
		public void CLIENT2LOBBY_BuySweepCouponsReq(int num)
		{
			IntProto req = new IntProto();
			req.value = num;
			Protocol.ProtocolProxy.instance.SendProtocol((int)MSG.BuySweepCouponsReq, req);
		}
		private bool LOBBY2CLIENT_GET_ALL_ITEM_RESP_handler(Observers.Interfaces.INotification note)
		{
//			GetAllItemResp resp = note.Body as GetAllItemResp;
//			int count = resp.items.Count;
//			for(int i = 0;i<count;i++){
//				ItemInfo item = new ItemInfo(resp.items[i]);
//				ItemProxy.instance.AddItemInfo(item);
//			}
			LuaTable itemModel = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","item_model")[0];
			LuaTable itemsTable = (LuaTable)itemModel["itemsTable"];
			foreach(DictionaryEntry kvp in itemsTable.ToDictTable())
			{
				LuaTable itemInfoTable = (LuaTable)kvp.Value;
				int id = itemInfoTable["instanceId"].ToString().ToInt32();
				int count = itemInfoTable["count"].ToString().ToInt32();
				LuaTable itemDataTable = (LuaTable)itemInfoTable["itemData"];
				if(itemDataTable != null)
				{
					int modelId = itemDataTable["id"].ToString().ToInt32();
					ItemProxy.instance.AddItemInfo(new ItemInfo(id,modelId,count));
				}
			}
			Logic.UI.RandomGift.Model.RandomGiftProxy.instance.UpdateRandomGift();
			return true;
		}
		private bool LOBBY2CLIENT_ITEM_UPDATE_RESP_handler(Observers.Interfaces.INotification note)
		{
//			ItemUpdateResp resp = note.Body as ItemUpdateResp;
//			//add
//			int addCount = resp.addItems.Count;
//			for(int i = 0;i<addCount;i++)
//			{
//				ItemInfo item = new ItemInfo(resp.addItems[i]);
//				ItemProxy.instance.AddItemInfo(item);
//			}
//			//remove
//			int removeCount = resp.delItems.Count;
//			for(int i = 0;i<removeCount;i++)
//			{
//				ItemProxy.instance.RemoveItemInfo(resp.delItems[i]);
//			}
//			//update
//			int updateCount = resp.updateItems.Count;
//			for(int i = 0;i<updateCount;i++)
//			{
//				ItemProxy.instance.UpdateItemInfo(resp.updateItems[i]);
//			}
			ItemProxy.instance.GetAllItemInfoDictioary().Clear();

			LuaTable itemModel = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","item_model")[0];
			LuaTable itemsTable = (LuaTable)itemModel["itemsTable"];
			foreach(DictionaryEntry kvp in itemsTable.ToDictTable())
			{
				LuaTable itemInfoTable = (LuaTable)kvp.Value;
				int id = itemInfoTable["instanceId"].ToString().ToInt32();
				int count = itemInfoTable["count"].ToString().ToInt32();
				LuaTable itemDataTable = (LuaTable)itemInfoTable["itemData"];
				if(itemDataTable != null)
				{
					int modelId = itemDataTable["id"].ToString().ToInt32();
					ItemProxy.instance.AddItemInfo(new ItemInfo(id,modelId,count));
				}
			}

			ItemProxy.instance.OnItemInfoListUpdate();

			//if(addCount != 0 || removeCount != 0)
				Logic.UI.RandomGift.Model.RandomGiftProxy.instance.UpdateRandomGift();
			return true;
		}
		private bool LOBBY2CLIENT_ExpPotion_RESP_handler(Observers.Interfaces.INotification note)
		{
			return true;
		}
		private bool LOBBY2CLIENT_BuySweepCouponsResp_RESP_handler(Observers.Interfaces.INotification note)
		{
			Logic.UI.DungeonDetail.Model.DungeonDetailProxy.instance.OnBuySweepCouponsSuccess();
			return true;
		}
	}
}

