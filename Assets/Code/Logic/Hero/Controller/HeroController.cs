using UnityEngine;
using System.Collections.Generic;
using Logic.Protocol.Model;
using Logic.CommonGameData;
using Logic.Game.Model;
using Logic.Player.Model;
using Logic.Hero.Model;
using Logic.Equipment.Model;
using Logic.UI.HeroBreakthrough.View;
using LuaInterface;

namespace Logic.Hero.Controller
{
	public class HeroController : SingletonMono<HeroController>
	{
		private static LuaTable _heroControllerLuaTable;
		
		public static LuaTable HeroControllerLuaTable
		{
			get
			{
				if (_heroControllerLuaTable == null)
					_heroControllerLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl", "hero_controller")[0];
				return _heroControllerLuaTable;
			}
		}

		private int _strengthenInstanceId = 0;

		void Awake ()
		{
			instance = this;
		}

		void Start ()
		{
//			Observers.Facade.Instance.RegisterObserver(((int)MSG.GetAllHeroResp).ToString(), LOBBY2CLIENT_GET_ALL_HERO_RESP_Handler);
//			Observers.Facade.Instance.RegisterObserver(((int)MSG.HeroUpdateResp).ToString(), LOBBY2CLIENT_HERO_UPDATE_RESP_Handler);
//			Observers.Facade.Instance.RegisterObserver(((int)MSG.HeroAggrResp).ToString(), LOBBY2CLIENT_HERO_AGGR_RESP_handler);
//			Observers.Facade.Instance.RegisterObserver(((int)MSG.HeroBreakResp).ToString(), LOBBY2CLIENT_HERO_BREAK_RESP);
//			Observers.Facade.Instance.RegisterObserver(((int)MSG.HeroComposeResp).ToString(), LOBBY2CLIENT_HERO_COMPOSE_RESP);
//			Observers.Facade.Instance.RegisterObserver(((int)MSG.HeroAdvanceResp).ToString(), LOBBY2CLIENT_HERO_ADVANCE_RESP);

		}

		void OnDestroy ()
		{
//			if (Observers.Facade.Instance != null)
//			{
//				Observers.Facade.Instance.RemoveObserver(((int)MSG.GetAllHeroResp).ToString(), LOBBY2CLIENT_GET_ALL_HERO_RESP_Handler);
//				Observers.Facade.Instance.RemoveObserver(((int)MSG.HeroUpdateResp).ToString(), LOBBY2CLIENT_HERO_UPDATE_RESP_Handler);
//				Observers.Facade.Instance.RemoveObserver(((int)MSG.HeroAggrResp).ToString(),LOBBY2CLIENT_HERO_AGGR_RESP_handler);
//				Observers.Facade.Instance.RemoveObserver(((int)MSG.HeroBreakResp).ToString(), LOBBY2CLIENT_HERO_BREAK_RESP);
//				Observers.Facade.Instance.RemoveObserver(((int)MSG.HeroComposeResp).ToString(), LOBBY2CLIENT_HERO_COMPOSE_RESP);
//				Observers.Facade.Instance.RemoveObserver(((int)MSG.HeroAdvanceResp).ToString(), LOBBY2CLIENT_HERO_ADVANCE_RESP);
//			}
		}

		#region C2S requests
		public void CLIENT2LOBBY_HERO_BREAK_REQ (int breakHeroInstanceID, int breakMaterialHeroInstanceID)
		{
//			HeroBreakReq heroBreakReq = new HeroBreakReq();
//			heroBreakReq.breakedId = breakHeroInstanceID;
//			heroBreakReq.consumeId = breakMaterialHeroInstanceID;
//			Logic.Protocol.ProtocolProxy.instance.SendProtocol(heroBreakReq);

			/* call lua function */
			HeroControllerLuaTable.GetLuaFunction("HeroBreakReq").Call(breakHeroInstanceID, breakMaterialHeroInstanceID);
			/* call lua function */
		}

		public void CLIENT2LOBBY_HERO_AGGR_REQ (int heroInstanceID, List<int> materialHeroInstanceIDs)
		{
//			HeroAggrReq req = new HeroAggrReq();
//			req.aggredId = heroInstanceID;
//			_strengthenInstanceId = heroInstanceID;
//			for(int i = 0,count = materialHeroInstanceIDs.Count;i<count;i++){
//				req.consumeIds.Add(materialHeroInstanceIDs[i]);
//			}
//			Logic.Protocol.ProtocolProxy.instance.SendProtocol(req);

			/* call lua function */
			LuaTable materialHeroInstanceIDsTable = (LuaTable)HeroControllerLuaTable.GetLuaFunction("GetEmptyTable").Call(null)[0];
			for (int index = 0, count = materialHeroInstanceIDs.Count; index < count; index++)
			{
				materialHeroInstanceIDsTable[index + 1] = materialHeroInstanceIDs[index];
			}
			HeroControllerLuaTable.GetLuaFunction("HeroAggrReq").Call(heroInstanceID, materialHeroInstanceIDsTable);
			/* call lua function */
		}

		// because of the hero compose req function was deprecated, I think we may remove it later
		public void CLIENT2LOBBY_HERO_COMPOSE_REQ(List<int> composeIds){
//			HeroComposeReq req = new HeroComposeReq();
//			for(int i = 0,count = composeIds.Count;i<count;i++){
//				req.composeHeros.Add(composeIds[i]);
//			}
//			Logic.Protocol.ProtocolProxy.instance.SendProtocol(req);
		}

		public void CLIENT2LOBBY_HERO_ADVANCE_REQ(int advanceHeroId,int materialType)
		{
//			HeroAdvanceReq req = new HeroAdvanceReq();
//			req.heroId = advanceHeroId;
//			req.materialType = materialType;
//			Logic.Protocol.ProtocolProxy.instance.SendProtocol(req);

			/* call lua function */
			HeroControllerLuaTable.GetLuaFunction("HeroAdvanceReq").Call(advanceHeroId, materialType);
			/* call lua function */
		}
		#endregion C2S requests

		#region server callback handlers
//		public bool LOBBY2CLIENT_GET_ALL_HERO_RESP_Handler (Observers.Interfaces.INotification note)
//		{
//			GetAllHeroResp getAllHeroResp = note.Body as GetAllHeroResp;
//			List<HeroProtoData> heroProtoDataList = getAllHeroResp.heros;
//			int heroesCount = heroProtoDataList.Count;
//			for (int i = 0; i < heroesCount; i++)
//			{
//				HeroProtoData heroProtoData = heroProtoDataList[i];
//				HeroInfo heroInfo = new HeroInfo(heroProtoData);
//				HeroProxy.instance.AddHero(heroInfo, false);
//				HeroProxy.instance.UpdateHeroEquipments(heroProtoData.id, heroProtoData.wearEquips);
//			}
//			return true;
//		}
//
//		public bool LOBBY2CLIENT_HERO_UPDATE_RESP_Handler (Observers.Interfaces.INotification note)
//		{
//			HeroUpdateResp heroUpdateResp = note.Body as HeroUpdateResp;
//
//			// add heroes
//			int addHeroCount = heroUpdateResp.addHeros.Count;
//			HeroProtoData newHeroProtoData = null;
//			HeroInfo newHeroInfo = null;
//			for (int i = 0; i < addHeroCount; i++)
//			{
//				newHeroProtoData = heroUpdateResp.addHeros[i];
//				newHeroInfo = new HeroInfo(newHeroProtoData);
//				HeroProxy.instance.AddHero(newHeroInfo, true);
//				HeroProxy.instance.UpdateHeroEquipments(newHeroProtoData.id, newHeroProtoData.wearEquips);
//				newHeroInfo = null;
//			}
//
//			// update heroes
//			List<HeroInfo> updateHeroes = new List<HeroInfo>();
//			int updateHeroCount = heroUpdateResp.updateHeros.Count;
//			HeroProtoData updateHeroProtoData = null;
//			HeroInfo updateHeroInfo = null;
//			for (int i = 0; i < updateHeroCount; i++)
//			{
//				updateHeroProtoData = heroUpdateResp.updateHeros[i];
//				updateHeroInfo = HeroProxy.instance.GetHeroInfo((uint)updateHeroProtoData.id);
//				if(updateHeroInfo!= null)
//				{
//					updateHeroInfo.SetHeroInfo(updateHeroProtoData);
//					HeroProxy.instance.UpdateHeroEquipments(updateHeroProtoData.id, updateHeroProtoData.wearEquips);
//				}
//				HeroProxy.instance.OnUpdateHero(updateHeroInfo.instanceID);
//			}
//
//			// remove heroes
//			HeroProxy.instance.RemoveHeroes(heroUpdateResp.delHeros);
//			Logic.UI.Expedition.Model.ExpeditionFormationProxy.instance.deleteExpeditionRole(heroUpdateResp.delHeros);
//			HeroProxy.instance.OnUpdateHeroInfoList();
//			return true;
//		}
//		//强化
//		public bool LOBBY2CLIENT_HERO_AGGR_RESP_handler (Observers.Interfaces.INotification note)
//		{
//			HeroAggrResp resp = note.Body as HeroAggrResp;
//			
//			Logic.UI.HeroStrengthen.Model.HeroStrengthenProxy.instance.UpdateStrengthenSuccess( resp.isCrit);
//			return true;
//		}
//
//		public bool LOBBY2CLIENT_HERO_BREAK_RESP (Observers.Interfaces.INotification note)
//		{
//			HeroBreakResp heroBreakResp = note.Body as HeroBreakResp;
//			HeroProxy.instance.OnHeroBreakthroughSuccess();
////			HeroBreakthroughSuccessExhibitionView.Open(HeroProxy.instance.GetHeroInfo(heroBreakResp.));
//			return true;
//		}
//		public bool LOBBY2CLIENT_HERO_COMPOSE_RESP (Observers.Interfaces.INotification note)
//		{
//			HeroComposeResp resp = note.Body as HeroComposeResp;
//
//
//			Logic.UI.HeroCombine.Model.HeroCombineProxy.instance.RefreshCombineByProtocol(resp.result == 1?true:false,resp.newHero,resp.isSpecial);
//			return true;
//		}
//		public bool LOBBY2CLIENT_HERO_ADVANCE_RESP(Observers.Interfaces.INotification note)
//		{
//			HeroAdvanceResp resp = note.Body as HeroAdvanceResp;
//			Logic.UI.HeroAdvance.Model.HeroAdvanceProxy.instance.UpdateHeroAdvanceByProtocol(resp.result == 1 ? true:false);
//			return true;
//		}
		#endregion server callback handlers
	}
}
