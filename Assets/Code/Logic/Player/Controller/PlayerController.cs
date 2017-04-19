using UnityEngine;
using System.Collections.Generic;
using Logic.Protocol.Model;
using Logic.Game.Model;
using Logic.Player.Model;
using Logic.UI.ManageHeroes.Model;
using Logic.Equipment.Model;
using Logic.Hero.Model;
using LuaInterface;
using System.Collections;

namespace Logic.Player.Controller
{
	public class PlayerController : SingletonMono<PlayerController>
	{
		void Awake ()
		{
			instance = this;
		}

		void Start ()
		{
			//Observers.Facade.Instance.RegisterObserver(((int)MSG.GetAllPlayerResp).ToString(), LOBBY2CLIENT_GET_ALL_PLAYER_RESP_handler);
			//Observers.Facade.Instance.RegisterObserver(((int)MSG.PlayerUpdateResp).ToString(), LOBBY2CLIENT_PLAYER_UPDATE_RESP_handler);
			//Observers.Facade.Instance.RegisterObserver(((int)MSG.PlayerTransferResp).ToString(), LOBBY2CLIENT_PLAYER_TRANSFER_RESP_handler);
			//Observers.Facade.Instance.RegisterObserver(((int)MSG.PlayerChangeResp).ToString(), LOBBY2CLIENT_PLAYER_CHANGE_RESP_handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.PlayerAggrResp).ToString(), LOBBY2CLIENT_PLAYER_AGGR_RESP_handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.PlayerBreakResp).ToString(), LOBBY2CLIENT_PLAYER_BREAK_RESP_handler);

			//Observers.Facade.Instance.RegisterObserver(((int)MSG.TalentSynResp).ToString(), LOBBY2CLIENT_TALENT_SYN_RESP_handler);

			Observers.Facade.Instance.RegisterObserver("GET_ALL_PLAYER_RESP_handler", LOBBY2CLIENT_GET_ALL_PLAYER_RESP_handler);
			Observers.Facade.Instance.RegisterObserver("PLAYER_UPDATE_RESP_handler", LOBBY2CLIENT_PLAYER_UPDATE_RESP_handler);
			Observers.Facade.Instance.RegisterObserver("LOBBY2CLIENT_PLAYER_TRANSFER_RESP_handler", LOBBY2CLIENT_PLAYER_TRANSFER_RESP_handler);
			Observers.Facade.Instance.RegisterObserver("LOBBY2CLIENT_PLAYER_CHANGE_RESP_handler", LOBBY2CLIENT_PLAYER_CHANGE_RESP_handler);

			Observers.Facade.Instance.RegisterObserver("TalentSynResp", LOBBY2CLIENT_TALENT_SYN_RESP_handler);
			Observers.Facade.Instance.RegisterObserver("TalentActivateResp", LOBBY2CLIENT_TALENT_ACTIVATE_RESP_handler);
			Observers.Facade.Instance.RegisterObserver("TalentUpgradeResp", LOBBY2CLIENT_TALENT_UPGRADE_RESP_handler);
			Observers.Facade.Instance.RegisterObserver("TalentChooseResp", LOBBY2CLIENT_TALENT_CHOOSE_RESP_handler);
		}

		void OnDestroy ()
		{
			if (Observers.Facade.Instance != null)
			{
				//Observers.Facade.Instance.RemoveObserver(((int)MSG.GetAllPlayerResp).ToString(), LOBBY2CLIENT_GET_ALL_PLAYER_RESP_handler);
				//Observers.Facade.Instance.RemoveObserver(((int)MSG.PlayerUpdateResp).ToString(), LOBBY2CLIENT_PLAYER_UPDATE_RESP_handler);
				//Observers.Facade.Instance.RemoveObserver(((int)MSG.PlayerTransferResp).ToString(), LOBBY2CLIENT_PLAYER_TRANSFER_RESP_handler);
				//Observers.Facade.Instance.RemoveObserver(((int)MSG.PlayerChangeResp).ToString(), LOBBY2CLIENT_PLAYER_CHANGE_RESP_handler);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.PlayerAggrResp).ToString(), LOBBY2CLIENT_PLAYER_AGGR_RESP_handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.PlayerBreakResp).ToString(), LOBBY2CLIENT_PLAYER_BREAK_RESP_handler);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.TalentActivateResp).ToString(), LOBBY2CLIENT_TALENT_ACTIVATE_RESP_handler);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.TalentUpgradeResp).ToString(), LOBBY2CLIENT_TALENT_UPGRADE_RESP_handler);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.TalentChooseResp).ToString(), LOBBY2CLIENT_TALENT_CHOOSE_RESP_handler);
				//Observers.Facade.Instance.RemoveObserver(((int)MSG.TalentSynResp).ToString(), LOBBY2CLIENT_TALENT_SYN_RESP_handler);

                Observers.Facade.Instance.RemoveObserver("GET_ALL_PLAYER_RESP_handler", LOBBY2CLIENT_GET_ALL_PLAYER_RESP_handler);
                Observers.Facade.Instance.RemoveObserver("PLAYER_UPDATE_RESP_handler", LOBBY2CLIENT_PLAYER_UPDATE_RESP_handler);
				Observers.Facade.Instance.RemoveObserver("LOBBY2CLIENT_PLAYER_TRANSFER_RESP_handler", LOBBY2CLIENT_PLAYER_TRANSFER_RESP_handler);
				Observers.Facade.Instance.RemoveObserver("LOBBY2CLIENT_PLAYER_CHANGE_RESP_handler", LOBBY2CLIENT_PLAYER_CHANGE_RESP_handler);

				Observers.Facade.Instance.RemoveObserver("TalentSynResp", LOBBY2CLIENT_TALENT_SYN_RESP_handler);
				Observers.Facade.Instance.RemoveObserver("TalentActivateResp", LOBBY2CLIENT_TALENT_ACTIVATE_RESP_handler);
				Observers.Facade.Instance.RemoveObserver("TalentUpgradeResp", LOBBY2CLIENT_TALENT_UPGRADE_RESP_handler);
				Observers.Facade.Instance.RemoveObserver("TalentChooseResp", LOBBY2CLIENT_TALENT_CHOOSE_RESP_handler);
				
				
			}
		}

		#region request to server
		public void CLIENT2LOBBY_GET_ALL_PLAYER_REQ ()
		{
			GetAllPlayerReq getAllPlayerReq = new GetAllPlayerReq();
			Logic.Protocol.ProtocolProxy.instance.SendProtocol(getAllPlayerReq);
		}
		//srcMaterialNoList: 使用通用材料代替的原始材料编号
		public void CLIENT2LOBBY_PLAYER_TRANSFER_REQ (int newPlayerDataID,List<int> srcMaterialNoList)
		{
			PlayerTransferReq playerTransferReq = new PlayerTransferReq();
			playerTransferReq.desProfessionId = newPlayerDataID;
			if(srcMaterialNoList != null)
			{
				for(int i = 0,count = srcMaterialNoList.Count;i<count;i++)
				{
					playerTransferReq.srcMaterialNos.Add(srcMaterialNoList[i]);
				}
			}
			Logic.Protocol.ProtocolProxy.instance.SendProtocol(playerTransferReq);
		}

		public void CLIENT2LOBBY_PLAYER_CHANGE_REQ (int playerInstanceID)
		{
			PlayerChangeReq playerChangeReq = new PlayerChangeReq();
			playerChangeReq.desInstanceId = playerInstanceID;
			Logic.Protocol.ProtocolProxy.instance.SendProtocol(playerChangeReq);
		}
		public void CLIENT2LOBBY_PLAYER_AGGR_REQ(int playerid,List<int> materialIds)
		{
			PlayerAggrReq req = new PlayerAggrReq();
			req.playerId = playerid;
			for(int i = 0,count = materialIds.Count;i<count;i++){
				req.consumeIds.Add(materialIds[i]);
			}
			Logic.Protocol.ProtocolProxy.instance.SendProtocol(req);
		}
        public void CLIENT2LOBBY_PLAYER_BREAK_REQ()
        {
            PlayerBreakReq req = new PlayerBreakReq();
            Logic.Protocol.ProtocolProxy.instance.SendProtocol(req);
        }
		///请求激活天赋
		public void CLIENT2LOBBY_TALENT_ACTIVATE_REQ(int id)
		{
//			TalentActivateReq req = new TalentActivateReq();
//			req.no = id;
//			Logic.Protocol.ProtocolProxy.instance.SendProtocol(req);
			LuaTable playerModel = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl","player_controller")[0];
			playerModel.GetLuaFunction("TalentActivateReq").Call(id);
		}
		///0x0116响应升级天赋
		public void CLIENT2LOBBY_TALENT_UPGRADE_REQ(int id)
		{
//			TalentUpgradeReq req = new TalentUpgradeReq();
//			req.no = id;
//			Logic.Protocol.ProtocolProxy.instance.SendProtocol(req);
			LuaTable playerModel = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl","player_controller")[0];
			playerModel.GetLuaFunction("TalentUpgradeResp").Call(id);
		}
		///请求变更主动天赋
		public void CLIENT2LOBBY_TALENT_CHOOSE_REQ(int passiveId,int summonId)
		{
//			TalentChooseRep req = new TalentChooseRep();
//			if( passiveId != 0 )
//				req.selectedTalnet.Add(passiveId);
//			if( summonId != 0)
//				req.selectedTalnet.Add(summonId);
//			Logic.Protocol.ProtocolProxy.instance.SendProtocol(req);
			LuaTable playerModel = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl","player_controller")[0];
			playerModel.GetLuaFunction("TalentChooseRep").Call(passiveId,summonId);
		}
		#endregion

		private void AllPlayerInitInLua()
		{
			PlayerProxy.instance.PlayerInfoDictionary.Clear();
			LuaTable playerModel = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","player_model")[0];
			LuaTable playerInfoTable = (LuaTable)playerModel["playerInfoTable"];
			foreach(DictionaryEntry kvp in playerInfoTable.ToDictTable())
			{
				LuaTable table = (LuaTable)kvp.Value;
				PlayerInfo playerInfo = new PlayerInfo(table);
				PlayerProxy.instance.AddPlayer(playerInfo);
			}
			LuaTable gameModel = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","game_model")[0];
			LuaTable curPlayerTable = (LuaTable)gameModel["playerInfo"];
			GameProxy.instance.PlayerInfo = PlayerProxy.instance.GetPlayerInfo(curPlayerTable["instanceID"].ToString().ToInt32());
		}
		

		#region server callback
		private bool LOBBY2CLIENT_GET_ALL_PLAYER_RESP_handler (Observers.Interfaces.INotification note)
		{
//			GetAllPlayerResp getAllPlayerResp = note.Body as GetAllPlayerResp;
//			int playerProtoDataCount = getAllPlayerResp.players.Count;
//			for (int i = 0; i < playerProtoDataCount; i++)
//			{
//				PlayerProtoData playerProtoData = getAllPlayerResp.players[i];
//				PlayerInfo playerInfo = new PlayerInfo(playerProtoData);
//				PlayerProxy.instance.AddPlayer(playerInfo);
//				PlayerProxy.instance.UpdatePlayerEquipments(playerProtoData.id, playerProtoData.wearEquips);
//				PlayerTalentProxy.instance.UpdateSkillTalent(playerProtoData.modelId,playerProtoData.talnets,playerProtoData.selectedTalnet);
//			}
//			GameProxy.instance.PlayerInfo = PlayerProxy.instance.GetPlayerInfo(getAllPlayerResp.currPlayerId);
			Debugger.Log("LOBBY2CLIENT_GET_ALL_PLAYER_RESP_handler");
			AllPlayerInitInLua();
			PlayerTalentProxy.instance.UpdateSkillTalentByLuaTable();
			return true;
		}
		
		private bool LOBBY2CLIENT_PLAYER_UPDATE_RESP_handler (Observers.Interfaces.INotification note)
		{
//			PlayerUpdateResp playerUpdateResp = note.Body as PlayerUpdateResp;
//			PlayerProtoData playerProtoData = playerUpdateResp.player;
//			PlayerInfo playerInfo = PlayerProxy.instance.GetPlayerInfo(playerProtoData.id);
//			playerInfo.SetPlayerInfo(playerProtoData);
//			PlayerProxy.instance.UpdatePlayerEquipments(playerProtoData.id, playerProtoData.wearEquips);
//			PlayerProxy.instance.OnPlayerInfoUpdate();
//			PlayerTalentProxy.instance.UpdateSkillTalent(playerProtoData.modelId,playerProtoData.talnets,playerProtoData.selectedTalnet);
			AllPlayerInitInLua();
			PlayerTalentProxy.instance.UpdateSkillTalentByLuaTable();
			PlayerProxy.instance.OnPlayerInfoUpdate();
			return true;
		}

		private bool LOBBY2CLIENT_PLAYER_TRANSFER_RESP_handler (Observers.Interfaces.INotification note)
		{
//			PlayerTransferResp playerTransferResp = note.Body as PlayerTransferResp;
//			PlayerInfo newPlayerInfo = new PlayerInfo(playerTransferResp.newPlayer);
//			newPlayerInfo.faceIndex = GameProxy.instance.PlayerInfo.faceIndex;
//			newPlayerInfo.hairCutIndex = GameProxy.instance.PlayerInfo.hairCutIndex;
//			newPlayerInfo.hairColorIndex = GameProxy.instance.PlayerInfo.hairColorIndex;
//			PlayerProxy.instance.AddPlayer(newPlayerInfo);
//			ManageHeroesProxy.instance.TransferPlayer((uint)newPlayerInfo.instanceID);
//			GameProxy.instance.PlayerInfo = PlayerProxy.instance.GetPlayerInfo((int)newPlayerInfo.instanceID);
//			PlayerProxy.instance.OnPlayerInfoUpdate();
			AllPlayerInitInLua();
			PlayerProxy.instance.OnPlayerInfoUpdate();
			return true;
		}

		private bool LOBBY2CLIENT_PLAYER_CHANGE_RESP_handler (Observers.Interfaces.INotification note)
		{
//			PlayerChangeResp playerChangeResp = note.Body as PlayerChangeResp;
//			ManageHeroesProxy.instance.TransferPlayer((uint)playerChangeResp.desInstanceId);
//			Logic.UI.Pvp.Model.PvpFormationProxy.instance.TransferPlayer((uint)playerChangeResp.desInstanceId);
//			Logic.UI.Expedition.Model.ExpeditionFormationProxy.instance.TransferPlayer((uint)playerChangeResp.desInstanceId);
//			GameProxy.instance.PlayerInfo = PlayerProxy.instance.GetPlayerInfo(playerChangeResp.desInstanceId);
//			PlayerProxy.instance.OnPlayerInfoUpdate();
			int id = note.Body.ToString().ToInt32();
			GameProxy.instance.PlayerInfo = PlayerProxy.instance.GetPlayerInfo(id);
			PlayerProxy.instance.OnPlayerInfoUpdate();
			return true;
		}
		private bool LOBBY2CLIENT_PLAYER_AGGR_RESP_handler (Observers.Interfaces.INotification note)
		{
			PlayerAggrResp resp = note.Body as PlayerAggrResp;
//			Logic.UI.HeroStrengthen.Model.HeroStrengthenProxy.instance.UpdateStrengthenSuccess( resp.isCrit);
			Logic.UI.HeroStrengthen.Model.HeroStrengthenProxy.HeroStrengthenModelLuaTable.GetLuaFunction("UpdateStrengthenSuccess").Call(resp.isCrit);
			return true;
		}
        private bool LOBBY2CLIENT_PLAYER_BREAK_RESP_handler(Observers.Interfaces.INotification note) 
        {
//            HeroProxy.instance.OnHeroBreakthroughSuccess();
			HeroProxy.instance.HeroModelLuaTable.GetLuaFunction("OnBreakthroughSuccess").Call();
            return true;
        }
		///响应激活天赋
		private bool LOBBY2CLIENT_TALENT_ACTIVATE_RESP_handler(Observers.Interfaces.INotification note) 
		{
//			TalentActivateResp resp = note.Body as TalentActivateResp;
//			PlayerSkillTalentInfo info = PlayerTalentProxy.instance.GetSkillTalentInfo(resp.no);
//			if(info != null)
//				info.Set(1,0);
//			PlayerTalentProxy.instance.UpdateTalentByProtocol(resp.no);
			Debugger.Log("响应激活天赋");
			int id = note.Body.ToString().ToInt32();
			PlayerTalentProxy.instance.UpdateSkillTalentByLuaTable();
			PlayerTalentProxy.instance.UpdateTalentByProtocol(id);
			return true;
		}
		///响应升级天赋
		private bool LOBBY2CLIENT_TALENT_UPGRADE_RESP_handler(Observers.Interfaces.INotification note) 
		{
			//TalentUpgradeResp resp = note.Body as TalentUpgradeResp;
			Debugger.Log("响应升级天赋");
			int id = note.Body.ToString().ToInt32();

			PlayerTalentProxy.instance.UpdateSkillTalentByLuaTable();
			PlayerTalentProxy.instance.UpdateTalentByProtocol(id);
			return true;
		}
		///响应变更主动天赋
		private bool LOBBY2CLIENT_TALENT_CHOOSE_RESP_handler(Observers.Interfaces.INotification note) 
		{
//			TalentChooseResp resp = note.Body as TalentChooseResp;
//			PlayerTalentProxy.instance.UpdateSkillTalent(GameProxy.instance.PlayerInfo.modelDataId,null,resp.selectedTalnet);
//			PlayerTalentProxy.instance.UpdateAllTalentByProtocol();
			Debugger.Log("响应变更主动天赋");
			PlayerTalentProxy.instance.UpdateSkillTalentByLuaTable();
			PlayerTalentProxy.instance.UpdateAllTalentByProtocol();

			return true;
		}
		///0x0120同步天赋变化
		private bool LOBBY2CLIENT_TALENT_SYN_RESP_handler(Observers.Interfaces.INotification note) 
		{
			Debugger.Log("同步天赋变化");
//			TalentSynResp resp = note.Body as TalentSynResp;
//			PlayerTalentProxy.instance.UpdateSkillTalent(GameProxy.instance.PlayerInfo.modelDataId, resp.talnets,null);
//			PlayerTalentProxy.instance.UpdateAllTalentByProtocol();
			PlayerTalentProxy.instance.UpdateSkillTalentByLuaTable();
			PlayerTalentProxy.instance.UpdateAllTalentByProtocol();
			return true;
		}
		#endregion
	}
}