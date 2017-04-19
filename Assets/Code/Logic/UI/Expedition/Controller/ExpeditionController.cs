using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.UI.Expedition.Model;
using Logic.Protocol.Model;
using Logic.Fight.Model;
using Logic.Game.Model;
using Logic.Player.Model;
using Logic.Hero.Model;
using Logic.Enums;
using Logic.Fight.Controller;
using LuaInterface;

namespace Logic.UI.Expedition.Controller
{
	public class ExpeditionController : SingletonMono<ExpeditionController> 
	{
		
		void Awake()
		{
			instance = this;
		}
		void Start ()
		{
			//Observers.Facade.Instance.RegisterObserver(((int)MSG.ExpeditionResp).ToString(),LOBBY2CLIENT_Expedition_RESP_handler);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.ExpeditionChallengeResp).ToString(),LOBBY2CLIENT_Expedition_Challenge_RESP_handler);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.ExpeditionSettleResp).ToString(),LOBBY2CLIENT_Expedition_Fight_Result_RESP_handler);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.GetExpeditionRewardResp).ToString(),LOBBY2CLIENT_Expedition_TreasureReward_RESP_handler);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.SynExpeditionResp).ToString(),LOBBY2CLIENT_Syn_Expedition_RESP_handler);

			Observers.Facade.Instance.RegisterObserver("LOBBY2CLIENT_Expedition_RESP_handler",LOBBY2CLIENT_Expedition_RESP_handler);
		}
		
		void OnDestroy ()
		{
			if (Observers.Facade.Instance != null)
			{
				//Observers.Facade.Instance.RemoveObserver(((int)MSG.ExpeditionResp).ToString(),LOBBY2CLIENT_Expedition_RESP_handler);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.ExpeditionChallengeResp).ToString(),LOBBY2CLIENT_Expedition_Challenge_RESP_handler);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.ExpeditionSettleResp).ToString(),LOBBY2CLIENT_Expedition_Fight_Result_RESP_handler);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.SynExpeditionResp).ToString(),LOBBY2CLIENT_Syn_Expedition_RESP_handler);

				Observers.Facade.Instance.RemoveObserver("LOBBY2CLIENT_Expedition_RESP_handler",LOBBY2CLIENT_Expedition_RESP_handler);
			}
		}
		#region  client to server
		//请求刷新远征界面
		public void CLIENT2LOBBY_Expedition_REQ()
		{
			ExpeditionReq req = new ExpeditionReq();
			Protocol.ProtocolProxy.instance.SendProtocol(req);
		}
		//请求战斗
		public void CLIENT2LOBBY_Expedition_Challenge_REQ(int dungeonId)
		{
			ExpeditionChallengeReq req = new ExpeditionChallengeReq();
			req.dungeonId = dungeonId;
			Protocol.ProtocolProxy.instance.SendProtocol(req);
		}
		//领取宝箱奖励
		public void CLIENT2LOBBY_Expedition_TreasureReward_REQ(int dungeonId)
		{
			GetExpeditionRewardReq req = new GetExpeditionRewardReq();
			req.id = dungeonId;
			Protocol.ProtocolProxy.instance.SendProtocol(req);
		}
		//请求重置刷新
		public void CLIENT2LOBBY_Expedition_Reset_REQ(bool isPay)
		{
			ResetExpeditionReq req = new ResetExpeditionReq();
			req.isPaid = isPay;
			Protocol.ProtocolProxy.instance.SendProtocol(req);
		}
		//阵型改变
		public void CLIENT2LOBBY_Expedition_Formation_Change_REQ()
		{
			//FormationController.instance.CLIENT2LOBBY_TeamChange_REQ(FormationTeamType.expeditionTeam);
//			TeamInfo teamInfo = new TeamInfo();
//			teamInfo.teamNo = 1;
//			SortedDictionary<FormationPosition,uint> formation = ExpeditionFormationProxy.instance.FormationsDictionary;
//			List<FormationPosition> keys = formation.GetKeys();
//			for(int i = 0,count = keys.Count;i<count;i++)
//			{
//				int pos = (int)keys[i];
//				PositionInfo posInfo = new PositionInfo();
//				posInfo.posIndex = pos;
//				posInfo.heroId = (int)formation[keys[i]];
//				teamInfo.posInfos.Add(posInfo);
//			}
//			ExpeditionTeamChangeReq req = new ExpeditionTeamChangeReq();
//			req.team = teamInfo;
//			Protocol.ProtocolProxy.instance.SendProtocol(req);
		}
		//请求战斗结算
		public void CLIENT2LOBBY_Expedition_Fight_Result_REQ(bool iswin,Dictionary<int,int> myHeros,Dictionary<int,int> enemyHeros)
		{
			ExpeditionSettleReq req = new ExpeditionSettleReq();
			req.dungeonId = ExpeditionProxy.instance.selectExpeditionDungeonInfo.id;
			req.result = iswin ? 1 : 0;

			ExpeditionHeroProto proto;
			List<int> keys = myHeros.GetKeys();

			int count = keys.Count;
			int key ;
			for(int i = 0;i<count;i++)
			{
				proto = new ExpeditionHeroProto();
				key = keys[i];
				proto.heroId = key;
				proto.hpPercent = myHeros[key];
				req.heros.Add(proto);
			}
			keys = enemyHeros.GetKeys();
			count = keys.Count;
			for(int i = 0;i<count;i++)
			{
				proto = new ExpeditionHeroProto();
				key = keys[i];
				proto.heroId = key;
				proto.hpPercent = enemyHeros[key];
				req.opponentHeros.Add(proto);
			}
			//ExpeditionFormationProxy.instance.UpdateExpeditionRole(myHeros);
			Protocol.ProtocolProxy.instance.SendProtocol(req);
		}
		#endregion

		#region sever
		//响应远征信息
		private bool LOBBY2CLIENT_Expedition_RESP_handler(Observers.Interfaces.INotification note)
		{
//			ExpeditionResp resp = note.Body as ExpeditionResp;
//			ExpeditionProxy.instance.resetCount = resp.remainRefreshTimes;
//			ExpeditionProxy.instance.expeditionVipBuyTimes = resp.vipBuyTimes;
//			ExpeditionProxy.instance.ResetDungeonInfoDictionary(resp.lastPassDungeon,resp.getRewardDungeonIds);
//			ExpeditionFormationProxy.instance.ResetFormation(resp.heros);
			LuaTable expeditionModelLua = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","expedition_model")[0];
			ExpeditionProxy.instance.resetCount = expeditionModelLua["resetCount"].ToString().ToInt32();
			ExpeditionProxy.instance.expeditionVipBuyTimes = expeditionModelLua["expeditionVipBuyTimes"].ToString().ToInt32();
			ExpeditionProxy.instance.ResetDungeonInfoDictionaryFromLua();
			ExpeditionFormationProxy.instance.UpdateExpeditionRoleFromLua();
			return true;
		}
		//响应挑战远征副本
		private bool LOBBY2CLIENT_Expedition_Challenge_RESP_handler(Observers.Interfaces.INotification note)
		{
			ExpeditionChallengeResp resp = note.Body as ExpeditionChallengeResp;
			//own
//			FightPlayerInfo ownFightPlayerInfo = null;
//			if(resp.selfTeamData.player != null)
//				ownFightPlayerInfo = new FightPlayerInfo(GameProxy.instance.PlayerInfo, resp.selfTeamData.player);
//			List<FightHeroInfo> ownFightHeroInfoList = new List<FightHeroInfo>();
//			HeroFightProtoData data;
//			int count = resp.selfTeamData.heros.Count;
//			
//			for(int i = 0;i<count;i++)
//			{
//				data = resp.selfTeamData.heros[i];
//				ownFightHeroInfoList.Add(new FightHeroInfo(HeroProxy.instance.GetHeroInfo((uint)data.id),data));
//			}
//			//enemy
//			PlayerFightProtoData opponentPlayerData =  resp.opponentTeamData.player;
//			PlayerInfo enemyPlayer = opponentPlayerData == null ? null :new PlayerInfo((uint)opponentPlayerData.id,(uint)opponentPlayerData.modelId,(uint)opponentPlayerData.hairCutId,(uint)opponentPlayerData.hairColorId,(uint)opponentPlayerData.faceId,"");
//			FightPlayerInfo enemyFightPlayerInfo = new FightPlayerInfo(enemyPlayer,opponentPlayerData);
//			List<FightHeroInfo> enemyFightHeroInfoList = new List<FightHeroInfo>();
//			count = resp.opponentTeamData.heros.Count;
//			for(int i = 0;i<count;i++)
//			{
//				data = resp.opponentTeamData.heros[i];
//				HeroInfo enemyHero = new HeroInfo((uint)data.id,data.modelId,1,0,data.star,1);
//				enemyFightHeroInfoList.Add(new FightHeroInfo(enemyHero,data));
//			}
			Observers.Facade.Instance.SendNotification("Expedition_Challenge_RESP_lua");
			//drop
			FightProxy.instance.SetData(resp.selfTeamData,resp.opponentTeamData,null);
			//fight
			FightController.instance.fightType = FightType.Expedition;
			FightController.instance.PreReadyFight();

			return true;
		}
		//请求领取远征奖励
		private bool LOBBY2CLIENT_Expedition_TreasureReward_RESP_handler(Observers.Interfaces.INotification note)
		{
			ExpeditionProxy.instance.UpdateGetRewardSuccessByProtocol();
			return true;
		}
		private bool LOBBY2CLIENT_Expedition_Refresh_RESP_handler(Observers.Interfaces.INotification note)
		{
			ExpeditionProxy.instance.UpdateResetSuccessByProtocol();
			return true;
		}
		//远征结算
		private bool LOBBY2CLIENT_Expedition_Fight_Result_RESP_handler(Observers.Interfaces.INotification note)
		{
			ExpeditionSettleResp resp = note.Body as ExpeditionSettleResp;
			FightProxy.instance.dropItems = new List<DropItem>(resp.dropItems);
			ExpeditionProxy.instance.UpdateExpeditionFightOverByProtocol();

			return true;
		}
		private bool LOBBY2CLIENT_Syn_Expedition_RESP_handler(Observers.Interfaces.INotification note)
		{
//			SynExpeditionResp resp = note.Body as SynExpeditionResp;
//			List<int > rewardId = null;
//			if(resp.newGetRewardDungeon != 0)
//			{
//				rewardId = new List<int>();
//				rewardId.Add(resp.newGetRewardDungeon);
//			}
//			ExpeditionProxy.instance.ResetDungeonInfoDictionary(resp.lastPassDungeon,rewardId);
			return true;
		}
		#endregion
	}

}
