using UnityEngine;
using System.Collections.Generic;
using Logic.Protocol.Model;
using Logic.UI.WorldTree.Model;
using Logic.Fight.Model;
using Logic.Fight.Controller;

namespace Logic.UI.WorldTree.Controller
{
	public class WorldTreeController : SingletonMono<WorldTreeController>
	{
		void Awake ()
		{
			instance = this;
		}

		void Start ()
		{
			Observers.Facade.Instance.RegisterObserver(((int)MSG.WorldTreeResp).ToString(), LOBBY2CLIENT_WORLD_TREE_RESP);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.WorldTreeFruitSynResp).ToString(), LOBBY2CLIENT_WORLD_TREE_FRUIT_SYN_RESP);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.WorldTreeChallengeResp).ToString(), LOBBY2CLIENT_WORLD_TREE_CHALLENGE_RESP);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.WorldTreeSettleResp).ToString(), LOBBY2CLIENT_WORLD_TREE_SETTLE_RESP);
		}

		void OnDestroy ()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver(((int)MSG.WorldTreeResp).ToString(), LOBBY2CLIENT_WORLD_TREE_RESP);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.WorldTreeFruitSynResp).ToString(), LOBBY2CLIENT_WORLD_TREE_FRUIT_SYN_RESP);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.WorldTreeChallengeResp).ToString(), LOBBY2CLIENT_WORLD_TREE_CHALLENGE_RESP);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.WorldTreeSettleResp).ToString(), LOBBY2CLIENT_WORLD_TREE_SETTLE_RESP);
			}
		}

		public void CLIENT2LOBBY_WORLD_TREE_REQ ()
		{
			WorldTreeReq worldTreeReq = new WorldTreeReq();
			Logic.Protocol.ProtocolProxy.instance.SendProtocol(worldTreeReq);
		}

		public void CLIENT2LOBBY_WORLD_TREE_FRUIT_SYN_REQ ()
		{
			WorldTreeFruitSynReq worldTreeFruitSynReq = new WorldTreeFruitSynReq();
			Logic.Protocol.ProtocolProxy.instance.SendProtocol(worldTreeFruitSynReq);
		}

		public void CLIENT2LOBBY_WORLD_TREE_CHALLENGE_REQ (int dungeonID)
		{
			WorldTreeChallengeReq worldTreeChallengeReq = new WorldTreeChallengeReq();
			worldTreeChallengeReq.dungeonId = dungeonID;
			Logic.Fight.Model.FightProxy.instance.ResetCurrentDungeonData(WorldTreeProxy.instance.GetWorldTreeInfoByID(dungeonID).dungeonData);
			Logic.Protocol.ProtocolProxy.instance.SendProtocol(worldTreeChallengeReq);
		}

		public void CLIENT2LOBBY_WORLD_TREE_SETTLE_REQ (int dungeonID, int result, List<int> diedHeroIDList)
		{
			WorldTreeSettleReq worldTreeSettleReq = new WorldTreeSettleReq();
			worldTreeSettleReq.dungeonId = dungeonID;
			worldTreeSettleReq.result = result;
			worldTreeSettleReq.diedHeroIds.AddRange(diedHeroIDList);
			Logic.Protocol.ProtocolProxy.instance.SendProtocol(worldTreeSettleReq);
		}

		private bool LOBBY2CLIENT_WORLD_TREE_RESP (Observers.Interfaces.INotification note)
		{
			WorldTreeResp worldTreeResp = note.Body as WorldTreeResp;
			WorldTreeProxy.instance.OnWorldTreeDungeonInfosUpdate(worldTreeResp.lastPassDungeon, worldTreeResp.fruitPurchasedTimes, worldTreeResp.failTimes);
			return true;
		}

		private bool LOBBY2CLIENT_WORLD_TREE_FRUIT_SYN_RESP (Observers.Interfaces.INotification note)
		{
			WorldTreeFruitSynResp worldTreeFruitSynResp = note.Body as WorldTreeFruitSynResp;
			WorldTreeProxy.instance.OnWorldTreeFruitUpdate(worldTreeFruitSynResp.fruit, worldTreeFruitSynResp.fruitUpperLimit, worldTreeFruitSynResp.nextRecoverTime);
			return true;
		}
		
		private bool LOBBY2CLIENT_WORLD_TREE_CHALLENGE_RESP (Observers.Interfaces.INotification note)
		{
			WorldTreeChallengeResp worldTreeChallengeResp = note.Body as WorldTreeChallengeResp;
			FightProxy.instance.SetData(worldTreeChallengeResp.teamFightData);

			FightController.instance.fightType = Logic.Enums.FightType.WorldTree;
			FightController.instance.PreReadyFight();
			return true;
		}
		
		private bool LOBBY2CLIENT_WORLD_TREE_SETTLE_RESP (Observers.Interfaces.INotification note)
		{
			WorldTreeSettleResp worldTreeSettleResp = note.Body as WorldTreeSettleResp;
			FightProxy.instance.dropItems = new List<DropItem>(worldTreeSettleResp.dropItems);
			WorldTreeProxy.instance.OnWolrdTreeFightOver();
			return true;
		}
	}
}