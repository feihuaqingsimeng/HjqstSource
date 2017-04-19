using UnityEngine;
using Logic.Protocol.Model;
using Logic.WorldBoss.Model;
using Logic.Fight.Model;
using Logic.Fight.Controller;
using Logic.Hero.Model;
using System.Collections.Generic;
using Logic.Game.Model;
using LuaInterface;

namespace Logic.WorldBoss.Controller
{
	public class WorldBossController : SingletonMono<WorldBossController>
	{
		public bool isWin = false;
		public bool isWorldBossActivityEnd = false;
		public bool isWorldBossKilledByOther = false;

		void Awake ()
		{
			instance = this;
		}

		void Start ()
		{
			Observers.Facade.Instance.RegisterObserver(((int)MSG.WorldBossTimeResp).ToString(), LOBBY2CLIENT_WorldBossTimeResp);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.WorldBossResp).ToString(), LOBBY2CLIENT_WorldBossResp);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.WorldBossChallengeResp).ToString(), LOBBY2CLIENT_WorldBossChallengeResp);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.WorldBossInspireResp).ToString(), LOBBY2CLIENT_WorldBossInspireResp);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.WorldBossReviveResp).ToString(), LOBBY2CLIENT_WorldBossReviveResp);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.WorldBossHurtSynResp).ToString(), LOBBY2CLIENT_WorldBossHurtSynResp);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.WorldBossKilledResp).ToString(), LOBBY2CLIENT_WorldBossKilledResp);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.WorldBossActivityEndResp).ToString(), LOBBY2CLIENT_WorldBossActivityEndResp);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.WorldBossSettleResp).ToString(), LOBBY2CLIETN_WorldBossSettleResp);
		}

		void OnDestroy ()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver(((int)MSG.WorldBossTimeResp).ToString(), LOBBY2CLIENT_WorldBossTimeResp);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.WorldBossResp).ToString(), LOBBY2CLIENT_WorldBossResp);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.WorldBossChallengeResp).ToString(), LOBBY2CLIENT_WorldBossChallengeResp);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.WorldBossInspireResp).ToString(), LOBBY2CLIENT_WorldBossInspireResp);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.WorldBossReviveResp).ToString(), LOBBY2CLIENT_WorldBossReviveResp);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.WorldBossHurtSynResp).ToString(), LOBBY2CLIENT_WorldBossHurtSynResp);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.WorldBossKilledResp).ToString(), LOBBY2CLIENT_WorldBossKilledResp);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.WorldBossActivityEndResp).ToString(), LOBBY2CLIENT_WorldBossActivityEndResp);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.WorldBossSettleResp).ToString(), LOBBY2CLIETN_WorldBossSettleResp);
			}
		}

		public bool LOBBY2CLIENT_WorldBossTimeResp (Observers.Interfaces.INotification note)
		{
			WorldBossTimeResp worldBossTimeResp = note.Body as WorldBossTimeResp;
			WorldBossProxy.instance.OnWorldBossStatusChanged(worldBossTimeResp.isOpen, worldBossTimeResp.openTime, worldBossTimeResp.overTime);

			return true;
		}

		public void CLIENT2LOBBY_WorldBossReq ()
		{
			WorldBossReq worldBossReq = new WorldBossReq();
			Logic.Protocol.ProtocolProxy.instance.SendProtocol(worldBossReq);
		}

		public bool LOBBY2CLIENT_WorldBossResp (Observers.Interfaces.INotification note)
		{
			WorldBossResp worldBossResp = note.Body as WorldBossResp;
			WorldBossProxy.instance.OnWorldBossInfoUpdate(worldBossResp);
			return true;
		}

		public void CLIENT2LOBBY_WorldBossChallengeReq ()
		{
			WorldBossChallengeReq worldBossChallengeReq = new WorldBossChallengeReq();
			Logic.Protocol.ProtocolProxy.instance.SendProtocol(worldBossChallengeReq);
		}

		public bool LOBBY2CLIENT_WorldBossChallengeResp (Observers.Interfaces.INotification note)
		{
			WorldBossChallengeResp worldBossChallengeResp = note.Body as WorldBossChallengeResp;
			WorldBossProxy.instance.OnWorldBossFightStart(worldBossChallengeResp.boss);
			FightProxy.instance.SetData(worldBossChallengeResp.teamFightData);
			FightController.instance.fightType = Logic.Enums.FightType.WorldBoss;
			FightController.instance.PreReadyFight();
			return true;
		}

		public void CLIENT2LOBBY_WorldBossInspireReq ()
		{
			WorldBossInspireReq worldBossInspireReq = new WorldBossInspireReq();
			Logic.Protocol.ProtocolProxy.instance.SendProtocol(worldBossInspireReq);
		}

		public bool LOBBY2CLIENT_WorldBossInspireResp (Observers.Interfaces.INotification note)
		{
			WorldBossInspireResp worldBossInspireResp = note.Body as WorldBossInspireResp;
			WorldBossProxy.instance.OnWorldBossInspireTimesUpdate(worldBossInspireResp.inspireTimes);
			return true;
		}

		public void CLIENT2LOBBY_WorldBossReviveReq ()
		{
			WorldBossReviveReq worldBossReviveReq = new WorldBossReviveReq();
			Logic.Protocol.ProtocolProxy.instance.SendProtocol(worldBossReviveReq);
		}

		public bool LOBBY2CLIENT_WorldBossReviveResp (Observers.Interfaces.INotification note)
		{
			WorldBossReviveResp worldBossReviveResp = note.Body as WorldBossReviveResp;
			WorldBossProxy.instance.OnReviveSuccess();
			return true;
		}

		public void CLIENT2LOBBY_WorldBossHurtSynReq (int hurt)
		{
			WorldBossHurtSynReq worldBossHurtSynReq = new WorldBossHurtSynReq();
			worldBossHurtSynReq.hurt = hurt;
			Logic.Protocol.ProtocolProxy.instance.SendProtocol(worldBossHurtSynReq);
		}

		public bool LOBBY2CLIENT_WorldBossHurtSynResp (Observers.Interfaces.INotification note)
		{
			WorldBossHurtSynResp worldBossHurtSynResp = note.Body as WorldBossHurtSynResp;
			WorldBossProxy.instance.OnWorldBossHurtSyn(worldBossHurtSynResp.bossRemainHp);
			return true;
		}

		public bool LOBBY2CLIENT_WorldBossKilledResp (Observers.Interfaces.INotification note)
		{
			WorldBossKilledResp worldBossKilledResp = note.Body as WorldBossKilledResp;
			isWorldBossKilledByOther = true;
			WorldBossProxy.instance.OnWorldBossKilledByOther();
			return true;
		}

		public bool LOBBY2CLIENT_WorldBossActivityEndResp (Observers.Interfaces.INotification note)
		{
			WorldBossActivityEndResp worldBossActivityEndResp = note.Body as WorldBossActivityEndResp;
			isWorldBossActivityEnd = true;
			WorldBossProxy.instance.OnWorldBossActivityEnd();
			return true;
		}

		public void CLIENT2LOBBY_WorldBossSettleReq (int hurt)
		{
			WorldBossSettleReq worldBossSettleReq = new WorldBossSettleReq();
			worldBossSettleReq.hurt = hurt;
			Logic.Protocol.ProtocolProxy.instance.SendProtocol(worldBossSettleReq);
		}

		public bool LOBBY2CLIETN_WorldBossSettleResp (Observers.Interfaces.INotification note)
		{
			WorldBossSettleResp worldBossSettleResp = note.Body as WorldBossSettleResp;
			WorldBossProxy.instance.OnWorldBossFightOver();
			return true;
		}

		public void Mock_WorldBossSettleReq (bool isWin, int hurt)
		{
			this.isWin = isWin;
			if (!isWorldBossActivityEnd && !isWin)
			{
				CLIENT2LOBBY_WorldBossSettleReq(hurt);
			}
			else
			{
				WorldBossProxy.instance.OnWorldBossReallyFightOver();
			}
		}
	}
}