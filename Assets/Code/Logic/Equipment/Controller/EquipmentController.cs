using UnityEngine;
using System.Collections.Generic;
using Logic.Protocol.Model;
using Logic.Enums;
using Logic.Equipment.Model;
using LuaInterface;

namespace Logic.Equipment.Controller
{
	public class EquipmentController : SingletonMono<EquipmentController>
	{
		private static LuaTable _equipControllerLuaTable;

		public static LuaTable EquipControllerLuaTable
		{
			get
			{
				if (_equipControllerLuaTable == null)
					_equipControllerLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl", "equip_controller")[0];
				return _equipControllerLuaTable;
			}
		}

		void Awake ()
		{
			instance = this;
		}

		void Start ()
		{
            //Observers.Facade.Instance.RegisterObserver(((int)MSG.GetAllEquipResp).ToString(), LOBBY2CLIENT_GET_ALL_EUQIP_RESP_handler);
//			Observers.Facade.Instance.RegisterObserver(((int)MSG.EquipUpdateResp).ToString(), LOBBY2CLIENT_EQUIP_UPDATE_RESP_handler);
//			Observers.Facade.Instance.RegisterObserver(((int)MSG.EquipWearOffResp).ToString(), LOBBY2CLIENT_EQUIP_WEAR_OFF_REQ_handler);
//			Observers.Facade.Instance.RegisterObserver(((int)MSG.EquipAggrResp).ToString(), LOBBY2CLIENT_EQUIP_AGGR_RESP_handler);
		}

		void OnDesctroy ()
		{
			if (Observers.Facade.Instance != null)
			{
                //Observers.Facade.Instance.RemoveObserver(((int)MSG.GetAllEquipResp).ToString(), LOBBY2CLIENT_GET_ALL_EUQIP_RESP_handler);
				//Observers.Facade.Instance.RemoveObserver(((int)MSG.EquipUpdateResp).ToString(), LOBBY2CLIENT_EQUIP_UPDATE_RESP_handler);
				//Observers.Facade.Instance.RemoveObserver(((int)MSG.EquipWearOffResp).ToString(), LOBBY2CLIENT_EQUIP_WEAR_OFF_REQ_handler);
				//Observers.Facade.Instance.RemoveObserver(((int)MSG.EquipAggrResp).ToString(), LOBBY2CLIENT_EQUIP_AGGR_RESP_handler);
			}
		}

		#region requests
		public void CLIENT2LOBBY_GET_ALL_EQUIP_REQ ()
		{
//			GetAllEquipReq getAllEquipReq = new GetAllEquipReq();
//			Logic.Protocol.ProtocolProxy.instance.SendProtocol(getAllEquipReq);

			EquipControllerLuaTable.GetLuaFunction("GetAllEquipmentsReq").Call();
		}

		public void CLIENT2LOBBY_EQUIP_WEAR_OFF_REQ (int equipInstanceID, EquipmentWearOffType wearOffType, bool shouldDestroy, int heroInstanceID)
		{
//			EquipWearOffReq equipWearOffReq = new EquipWearOffReq();
//			equipWearOffReq.equipId = equipInstanceID;
//			equipWearOffReq.wearOrOff = (int)wearOffType;
//			equipWearOffReq.isDestroy = shouldDestroy;
//			equipWearOffReq.heroId = heroInstanceID;
//			Logic.Protocol.ProtocolProxy.instance.SendProtocol(equipWearOffReq);

			EquipControllerLuaTable.GetLuaFunction("EquipWearOffReq").Call(equipInstanceID, (int)wearOffType, shouldDestroy, heroInstanceID);
		}

		public void CLIENT2LOBBY_EQUIP_AGGR_REQ(int strengthenId,List<int> materials)
		{
//			EquipAggrReq req = new EquipAggrReq();
//			req.aggredId = strengthenId;
//			for(int i = 0,count = materials.Count;i<count;i++)
//			{
//				req.consumeIds.Add(materials[i]);
//			}
//			Logic.Protocol.ProtocolProxy.instance.SendProtocol(req);

			LuaTable materialsTable = (LuaTable)EquipControllerLuaTable.GetLuaFunction("GetEmptyTable").Call(null)[0];
			for (int index = 0, count = materials.Count; index < count; index++)
			{
				materialsTable[index + 1] = materials[index];
			}
			EquipControllerLuaTable.GetLuaFunction("EquipAggrReq").Call(strengthenId, materialsTable);
		}

		public void CLIENT2LOBBY_EQUIP_SELL_REQ (int equipmentInstanceID)
		{
//			EquipSellReq equipSellReq = new EquipSellReq();
//			equipSellReq.equipId = equipmentInstanceID;
//			Logic.Protocol.ProtocolProxy.instance.SendProtocol(equipSellReq);

			EquipControllerLuaTable.GetLuaFunction("EquipSellReq").Call(equipmentInstanceID);
		}
		public void CLIENT2LOBBY_EquipUpgradeResp(int equipmentInstanceID)
		{
			EquipControllerLuaTable.GetLuaFunction("EquipUpgradeReq").Call(equipmentInstanceID);
		}
		public void CLIENT2LOBBY_EquipRecastReq(int equipmentInstanceID)
		{
			EquipControllerLuaTable.GetLuaFunction("EquipRecastReq").Call(equipmentInstanceID);
		}
		public void CLIENT2LOBBY_EquipRecastAffirmReq(int equipmentInstanceID,bool isUsed)
		{
			EquipControllerLuaTable.GetLuaFunction("EquipRecastAffirmReq").Call(equipmentInstanceID,isUsed);
		}
		#endregion requests
		
		#region server callback handlers
        //private bool LOBBY2CLIENT_GET_ALL_EUQIP_RESP_handler (Observers.Interfaces.INotification note)
        //{
        //    GetAllEquipResp getAllEquipResp = note.Body as GetAllEquipResp;
        //    int equipmentCount = getAllEquipResp.equips.Count;
        //    for (int i = 0; i < getAllEquipResp.equips.Count; i++)
        //    {
        //        EquipmentInfo equipmentInfo = new EquipmentInfo(getAllEquipResp.equips[i]);
        //        EquipmentProxy.instance.AddEquipmentInfo(equipmentInfo, false);
        //    }
        //    return true;
        //}

//		private bool LOBBY2CLIENT_EQUIP_UPDATE_RESP_handler (Observers.Interfaces.INotification note)
//		{
//			EquipUpdateResp equipUpdateResp = note.Body as EquipUpdateResp;
//			int addEquipmentCount = equipUpdateResp.addEquips.Count;
//			for (int i = 0; i < addEquipmentCount; i++)
//			{
//				EquipmentInfo addEquipmentInfo = new EquipmentInfo(equipUpdateResp.addEquips[i]);
//				//EquipmentProxy.instance.AddEquipmentInfo(addEquipmentInfo, true);
//			}
//
//			int removeEquipmentCount = equipUpdateResp.delEquips.Count;
//			for (int i = 0; i < removeEquipmentCount; i++)
//			{
//				EquipmentProxy.instance.RemoveEquipmentInfo(equipUpdateResp.delEquips[i]);
//			}
//
//			int updateEquipmentCount = equipUpdateResp.updateEquips.Count;
//			for (int i = 0; i < updateEquipmentCount; i++)
//			{
//				EquipmentProxy.instance.UpdateEquipmentInfo(equipUpdateResp.updateEquips[i]);
//			}
//			EquipmentProxy.instance.OnEquipmentInfoListUpdate();
//			return true;
//		}
//
//		private bool LOBBY2CLIENT_EQUIP_WEAR_OFF_REQ_handler (Observers.Interfaces.INotification note)
//		{
//			EquipWearOffResp equipWearOffResp = note.Body as EquipWearOffResp;
//			EquipmentProxy.instance.OnEquipmentInfoListUpdate();
//			return true;
//		}
//
//		private bool LOBBY2CLIENT_EQUIP_AGGR_RESP_handler(Observers.Interfaces.INotification note)
//		{
//			EquipAggrResp resp = note.Body as EquipAggrResp;
//			Logic.UI.EquipmentsStrengthen.Model.EquipmentStrengthenProxy.instance.StrengthenSuccess();
//			return true;
//		}
//
//		private bool LOBBY2CLIENT_EQUIP_SELL_RESP_handler (Observers.Interfaces.INotification note)
//		{
////			EquipSellResp equipSellResp = note.Body as EquipSellResp;
////			EquipmentProxy.instance.RemoveEquipmentInfo(equipSellResp.equipId);
//			EquipmentProxy.instance.OnEquipmentInfoListUpdate();
//			return true;
//		}
		#endregion server callback handlers
	}
}
