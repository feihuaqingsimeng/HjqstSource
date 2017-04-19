using UnityEngine;
using System.Collections.Generic;
using Logic.Protocol.Model;
using Logic.Activity.Model;
using Logic.Dungeon.Model;
using Logic.Fight.Model;
using Logic.Fight.Controller;

namespace Logic.Activity.Controller
{
	public class ActivityController : SingletonMono<ActivityController>
	{
		void Awake ()
		{
			instance = this;
		}

		void Start ()
		{
			Observers.Facade.Instance.RegisterObserver(((int)MSG.ActivityPveResp).ToString(), LOBBY2CLIENT_ACTIVITY_PVE_RESP);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.ActivityPveChallengeResp).ToString(), LOBBY2CLIENT_ACTIVITY_PVE_CHALLENGE_RESP);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.ActivityPveOverResp).ToString(), LOBBY2CLIENT_ACTIVITY_PVE_OVER_RESP);
//			Observers.Facade.Instance.RegisterObserver(((int)MSG.ActivityPveDrawResp).ToString(), LOBBY2CLIENT_ACTIVITY_PVE_DRAW_RESP);
			// wangxf
			Observers.Facade.Instance.RegisterObserver(((int)MSG.ActivityPveAwardResp).ToString(), LOBBY2CLIENT_ACTIVITY_PVE_AWARD_RESP);
		}

		void OnDestroy ()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver(((int)MSG.ActivityPveResp).ToString(), LOBBY2CLIENT_ACTIVITY_PVE_RESP);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.ActivityPveChallengeResp).ToString(), LOBBY2CLIENT_ACTIVITY_PVE_CHALLENGE_RESP);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.ActivityPveOverResp).ToString(), LOBBY2CLIENT_ACTIVITY_PVE_OVER_RESP);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.ActivityPveAwardResp).ToString(), LOBBY2CLIENT_ACTIVITY_PVE_AWARD_RESP);
			}
		}

		#region requests
		public void CLIENT2LOBBY_ACTIVITY_PVE_REQ ()
		{
			ActivityPveReq activityPveReq = new ActivityPveReq();
			Logic.Protocol.ProtocolProxy.instance.SendProtocol(activityPveReq);
		}

		public void CLIENT2LOBBY_ACTIVITY_PVE_CHALLENGE_REQ (int dungeonID,bool ispay)
		{
			ActivityPveChallengeReq activityPveChallengeReq = new ActivityPveChallengeReq();
			activityPveChallengeReq.dungeonId = dungeonID;
			activityPveChallengeReq.isPaid = ispay;
			DungeonInfo dungeonInfo = DungeonProxy.instance.GetDungeonInfo(dungeonID);
			Logic.Fight.Model.FightProxy.instance.ResetCurrentDungeonData(dungeonInfo.dungeonData);
			Logic.Protocol.ProtocolProxy.instance.SendProtocol(activityPveChallengeReq);
		}

		public void CLIENT2LOBBY_ACTIVITY_PVE_OVER_REQ (int dungeonID, int outDamage, int result,int deadNum,int passTime,int remaindHpPercent,int comboCount)
		{
			ActivityPveOverReq activityPveOverReq = new ActivityPveOverReq();
			activityPveOverReq.dungeonId = dungeonID;
			activityPveOverReq.outDamage = outDamage;
			activityPveOverReq.result = result;
            activityPveOverReq.dieNum = deadNum;
            activityPveOverReq.passTime = passTime;
            activityPveOverReq.remainHpPercent = remaindHpPercent;
            activityPveOverReq.combo = comboCount;
			Logic.Protocol.ProtocolProxy.instance.SendProtocol(activityPveOverReq);
		}

		public void CLIENT2LOBBY_ACTIVITY_PVE_AWARD_REQ (int multiple)
		{
			IntProto intProto = new IntProto();
			intProto.value = multiple;
			Logic.Protocol.ProtocolProxy.instance.SendProtocol((int)MSG.ActivityPveAwardReq, intProto);
		}

//		public void CLIENT2LOBBY_ACTIVITY_PVE_DRAW__REQ ()
//		{
//			ActivityPveDrawReq activityPveDrawReq = new ActivityPveDrawReq();
//			Logic.Protocol.ProtocolProxy.instance.SendProtocol(activityPveDrawReq);
//		}
		// wangxf
		public void CLIENT2LOBBY_ACTIVITY_PVE_DRAW__REQ ()
		{
			IntProto activityPveDrawReq = new IntProto();
			activityPveDrawReq.value = 0;
			Logic.Protocol.ProtocolProxy.instance.SendProtocol(activityPveDrawReq);
		}
		#endregion requests

		#region server callback
		private bool LOBBY2CLIENT_ACTIVITY_PVE_RESP (Observers.Interfaces.INotification note)
		{
			ActivityPveResp activityPveResp = note.Body as ActivityPveResp;
			ActivityProxy.instance.RefreshActivityInfos(activityPveResp.openPves);
			return true;
		}

		private bool LOBBY2CLIENT_ACTIVITY_PVE_CHALLENGE_RESP (Observers.Interfaces.INotification note)
		{
			ActivityPveChallengeResp activityPveChallengeResp = note.Body as ActivityPveChallengeResp;
			FightProxy.instance.SetData(activityPveChallengeResp.teamFightData);
			

			FightController.instance.fightType = Logic.Enums.FightType.DailyPVE;
			FightController.instance.PreReadyFight();
			return true;
		}

		private bool LOBBY2CLIENT_ACTIVITY_PVE_OVER_RESP (Observers.Interfaces.INotification note)
		{
			ActivityPveOverResp activityPveOverResp = note.Body as ActivityPveOverResp;
			FightProxy.instance.fightResultStar = 3;
			//ActivityProxy.instance.OnActivityChallengeOver(activityPveOverResp.fixedDrops, activityPveOverResp.drawDrops);
			// wangxf
			ActivityProxy.instance.OnActivityChallengeOver(activityPveOverResp.fixedDrops);
			return true;
		}

//		private bool LOBBY2CLIENT_ACTIVITY_PVE_DRAW_RESP (Observers.Interfaces.INotification note)
//		{
//			ActivityPveDrawResp activityPveDrawResp = note.Body as ActivityPveDrawResp;
//			ActivityProxy.instance.OnDrawCardSuccess(activityPveDrawResp.drawTimes);
//			return true;
//		}
		// wangxf
		private bool LOBBY2CLIENT_ACTIVITY_PVE_AWARD_RESP (Observers.Interfaces.INotification note)
		{
			ActivityProxy.instance.OnActivityPveAwardMultipled();
			return true;
		}
		#endregion server callback
	}
}
