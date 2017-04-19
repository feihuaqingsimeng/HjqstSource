using UnityEngine;
using Logic.Protocol.Model;
using Logic.Fight.Model;
using Logic.Fight.Controller;
using Logic.Dungeon.Model;
using System.Collections.Generic;
using Logic.Enums;
using Logic.UI.DungeonDetail.Model;

namespace Logic.Dungeon.Controller
{
    public class DungeonController : SingletonMono<DungeonController>
    {
        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            //            Observers.Facade.Instance.RegisterObserver(((int)MSG.PveInfoResp).ToString(), LOBBY2CLIENT_PVE_INFO_RESP);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.PveFightResp).ToString(), LOBBY2CLIENT_PVE_FIGHT_RESP);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.PveFightOverResp).ToString(), LOBBY2CLIENT_PVE_FIGHT_OVER_RESP);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.PveMopUpResp).ToString(), LOBBY2CLIENT_PVE_MOP_UP_RESP);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.PveTenMopUpResp).ToString(), LOBBY2CLIENT_PVE_TEN_MOP_UP_RESP);
        }

        void OnDestroy()
        {
            if (Observers.Facade.Instance != null)
            {
                //                Observers.Facade.Instance.RemoveObserver(((int)MSG.PveInfoResp).ToString(), LOBBY2CLIENT_PVE_INFO_RESP);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.PveFightResp).ToString(), LOBBY2CLIENT_PVE_FIGHT_RESP);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.PveFightOverResp).ToString(), LOBBY2CLIENT_PVE_FIGHT_OVER_RESP);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.PveMopUpResp).ToString(), LOBBY2CLIENT_PVE_MOP_UP_RESP);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.PveTenMopUpResp).ToString(), LOBBY2CLIENT_PVE_TEN_MOP_UP_RESP);

            }
        }

        #region request
        public void CLIENT2LOBBY_PVE_INFO_REQ()
        {
            PveInfoReq pveInfoReq = new PveInfoReq();
            Logic.Protocol.ProtocolProxy.instance.SendProtocol(pveInfoReq);
        }

        public void CLIENT2LOBBY_PVE_FIGHT_REQ(int dungeonID)
        {
            PveFightReq pveFightReq = new PveFightReq();
            pveFightReq.dungeonId = dungeonID;
            Logic.TalkingData.Controller.TalkingDataController.instance.TDGAMissionOnBegin(dungeonID.ToString(), Logic.TalkingData.Controller.TalkDataMissionType.Level);
            DungeonInfo dungeonInfo = DungeonProxy.instance.GetDungeonInfo(dungeonID);
            DungeonDetailProxy.instance.DungeonInfo = dungeonInfo;
            Logic.Fight.Model.FightProxy.instance.ResetCurrentDungeonData(dungeonInfo.dungeonData);
            Logic.Protocol.ProtocolProxy.instance.SendProtocol(pveFightReq);
        }

        public void CLIENT2LOBBY_PVE_FIGHT_OVER(int dungeonID, int passTime, int result, int outDamage, int deadNum, int remaindHpPercent, int comboCount, List<int> dieHeroIDs)
        {
            PveFightOverReq pveFightOverReq = new PveFightOverReq();
            pveFightOverReq.dungeonId = dungeonID;
            pveFightOverReq.passTime = passTime;
            pveFightOverReq.result = result;
            pveFightOverReq.dieNum = deadNum;
            pveFightOverReq.outDamage = outDamage;
            pveFightOverReq.remainHpPercent = remaindHpPercent;
            pveFightOverReq.combo = comboCount;
            pveFightOverReq.dieHeroIds.AddRange(dieHeroIDs);
            Logic.Protocol.ProtocolProxy.instance.SendProtocol(pveFightOverReq);
        }
        /// <summary>
        /// 扫荡1
        /// </summary>
        public void CLIENT2LOBBY_PVE_MOP_UP_REQ(int dungeonid)
        {
            PveMopUpReq req = new PveMopUpReq();
            req.dungeonId = dungeonid;
            Logic.Protocol.ProtocolProxy.instance.SendProtocol(req);
        }
        /// <summary>
        /// 扫荡10
        /// </summary>
        public void CLIENT2LOBBY_PveTenMopUp_REQ(int dungeonid,int times)
        {
            PveTenMopUpReq req = new PveTenMopUpReq();
            req.dungeonId = dungeonid;
			req.times = times;
            Logic.Protocol.ProtocolProxy.instance.SendProtocol(req);
        }

		public void CLIENT2LOBBY_RefreshDayTimesReq (int dungeonID)
		{
			IntProto intProto = new IntProto();
			intProto.value = dungeonID;
			Logic.Protocol.ProtocolProxy.instance.SendProtocol((int)MSG.RefreshDayTimesReq, intProto);
		}
        #endregion request

        #region server callback handlers
        //        private bool LOBBY2CLIENT_PVE_INFO_RESP(Observers.Interfaces.INotification note)
        //        {
        //            PveInfoResp pveInfoResp = note.Body as PveInfoResp;
        //            DungeonProxy.instance.UpdateDungeonInfos(pveInfoResp);
        //			Logic.UI.SoftGuide.Model.SoftGuideProxy.instance.UpdateSoftGuide();
        //			Logic.UI.ManageHeroes.Model.ManageHeroesProxy.instance.CurrentSelectFormationTeamType = (FormationTeamType)pveInfoResp.pveTeamNo;
        //            return true;
        //        }

        private bool LOBBY2CLIENT_PVE_FIGHT_RESP(Observers.Interfaces.INotification note)
        {

			Observers.Facade.Instance.SendNotification("EnterFight","pve");
            PveFightResp pveFightResp = note.Body as PveFightResp;
            FightProxy.instance.SetData(pveFightResp.teamFightData, pveFightResp.dropItems);

            FightController.instance.fightType = Logic.Enums.FightType.PVE;
            FightController.instance.PreReadyFight();
            return true;
        }

        private bool LOBBY2CLIENT_PVE_FIGHT_OVER_RESP(Observers.Interfaces.INotification note)
        {
            PveFightOverResp pveFightOverResp = note.Body as PveFightOverResp;
            FightProxy.instance.fightResultStar = pveFightOverResp.evaluateStar;
            Logic.TalkingData.Controller.TalkingDataController.instance.TDGAMissionOnCompleted(FightProxy.instance.CurrentDungeonData.dungeonID.ToString(), Logic.TalkingData.Controller.TalkDataMissionType.Level);
            DungeonProxy.instance.OnPveFightOver();
            return true;
        }
        private bool LOBBY2CLIENT_PVE_MOP_UP_RESP(Observers.Interfaces.INotification note)
        {
            PveMopUpResp resp = note.Body as PveMopUpResp;
            Logic.UI.DungeonDetail.Model.DungeonDetailProxy.instance.UpdateMopUpSuccessByProtocol(resp.dungeonId, resp.dropItems);
            return true;

        }
        private bool LOBBY2CLIENT_PVE_TEN_MOP_UP_RESP(Observers.Interfaces.INotification note)
        {
            PveTenMopUpResp resp = note.Body as PveTenMopUpResp;
            Logic.UI.DungeonDetail.Model.DungeonDetailProxy.instance.UpdateMopUpSuccessByProtocol(resp.dungeonId, resp.dropItems);
            return true;

        }
        #endregion server callback handlers
    }
}
