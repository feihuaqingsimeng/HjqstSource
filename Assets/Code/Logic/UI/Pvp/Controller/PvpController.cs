using UnityEngine;
using System.Collections;
using Logic.UI.Pvp.Model;
using Logic.Protocol.Model;
using System.Collections.Generic;
using Logic.Enums;
using Logic.Protocol;
using Logic.Fight.Model;
using Logic.Fight.Controller;
using Logic.Game.Model;
using Logic.Hero.Model;
using Logic.UI.FightResult.Model;

namespace Logic.UI.Pvp.Controller
{
    public class PvpController : SingletonMono<PvpController>
    {

        void Awake()
        {
            instance = this;
        }
        void Start()
        {
            Observers.Facade.Instance.RegisterObserver(((int)MSG.GetRankArenaResp).ToString(), LOBBY2CLIENT_GET_RANK_ARENA_RESP_handler);
            //Observers.Facade.Instance.RegisterObserver(((int)MSG.RankArenaTeamChangeResp).ToString(),LOBBY2CLIENT_RANK_ARENA_TEAM_CHANGE_RESP_handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.GetRankArenaReportResp).ToString(), LOBBY2CLIENT_GET_RANK_ARENA_REPORT_RESP_handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.RankArenaReportUpdateResp).ToString(), LOBBY2CLIENT_RANK_ARENA_REPORT_UPDATE_RESP_handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.RefreshOpponentsResp).ToString(), LOBBY2CLIENT_RANK_ARENA_REFRESH_OPPONENTS_RESP_handler);
            //Observers.Facade.Instance.RegisterObserver(((int)MSG.GetRankArenaRewardResp).ToString(), LOBBY2CLIENT_RANK_ARENA_REWARD_RESP_handler);
            //Observers.Facade.Instance.RegisterObserver(((int)MSG.GetRankingListResp).ToString(),LOBBY2CLIENT_GET_RANKING_LIST_RESP_handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.RankArenaChallengeResp).ToString(), LOBBY2CLIENT_RANK_ARENA_CHALLENGE_RESP_handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.RankArenaChallengeOverResp).ToString(), LOBBY2CLIENT_RANK_ARENA_CHALLENGE_OVER_RESP_handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.PointPvpChallengeResp).ToString(), LOBBY2CLIENT_POINT_PVP_CHALLENGE_RESP_handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.PointPvpSettleResp).ToString(), LOBBY2CLIENT_POINT_PVP_SETTLE_RESP_handler);
            //Observers.Facade.Instance.RegisterObserver(((int)MSG.RankArenaTeamResp ).ToString(),LOBBY2CLIENT_RANK_ARENA_TEAM_RESP_handler);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.SynPvpActionResp).ToString(), LOBBY2CLIENT_SynPvpActionResp_handler);
			
        }

        void OnDestroy()
        {
            if (Observers.Facade.Instance != null)
            {
                Observers.Facade.Instance.RemoveObserver(((int)MSG.GetRankArenaResp).ToString(), LOBBY2CLIENT_GET_RANK_ARENA_RESP_handler);
                //Observers.Facade.Instance.RemoveObserver(((int)MSG.RankArenaTeamChangeResp).ToString(),LOBBY2CLIENT_RANK_ARENA_TEAM_CHANGE_RESP_handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.GetRankArenaReportResp).ToString(), LOBBY2CLIENT_GET_RANK_ARENA_REPORT_RESP_handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.RankArenaReportUpdateResp).ToString(), LOBBY2CLIENT_RANK_ARENA_REPORT_UPDATE_RESP_handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.RefreshOpponentsResp).ToString(), LOBBY2CLIENT_RANK_ARENA_REFRESH_OPPONENTS_RESP_handler);
                //Observers.Facade.Instance.RemoveObserver(((int)MSG.GetRankArenaRewardResp).ToString(), LOBBY2CLIENT_RANK_ARENA_REWARD_RESP_handler);
                //Observers.Facade.Instance.RemoveObserver(((int)MSG.GetRankingListResp).ToString(),LOBBY2CLIENT_GET_RANKING_LIST_RESP_handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.RankArenaChallengeResp).ToString(), LOBBY2CLIENT_RANK_ARENA_CHALLENGE_RESP_handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.RankArenaChallengeOverResp).ToString(), LOBBY2CLIENT_RANK_ARENA_CHALLENGE_OVER_RESP_handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.PointPvpChallengeResp).ToString(), LOBBY2CLIENT_POINT_PVP_CHALLENGE_RESP_handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.PointPvpSettleResp).ToString(), LOBBY2CLIENT_POINT_PVP_SETTLE_RESP_handler);
                //	Observers.Facade.Instance.RemoveObserver(((int)MSG.RankArenaTeamResp ).ToString(),LOBBY2CLIENT_RANK_ARENA_TEAM_RESP_handler);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.SynPvpActionResp).ToString(), LOBBY2CLIENT_SynPvpActionResp_handler);
				

            }
        }


        #region client to server
        public void CLIENT2LOBBY_GET_RANK_ARENA_REQ()
        {
            GetRankArenaReq req = new GetRankArenaReq();
            Logic.Protocol.ProtocolProxy.instance.SendProtocol(req);
        }
        public void CLIENT2LOBBY_RANK_ARENA_TEAM_CHANGE_REQ()
        {
           // FormationController.instance.CLIENT2LOBBY_TeamChange_REQ(FormationTeamType.pvpTeam);
            //			RankArenaTeamChangeReq req = new RankArenaTeamChangeReq();
            //			SortedDictionary<FormationPosition, uint> formation = PvpFormationProxy.instance.FormationsDictionary;
            //			List<FormationPosition> positions = new List<FormationPosition>(formation.Keys);
            //			int count = positions.Count;
            //			TeamInfo teamInfo = new TeamInfo();
            //			teamInfo.teamNo = 1;
            //			for(int i = 0;i<count;i++)
            //			{
            //				FormationPosition pos = positions[i];
            //				PositionInfo info = new PositionInfo();
            //				info.heroId = (int)formation[pos];
            //				info.posIndex = (int)pos;
            //				teamInfo.posInfos.Add(info);
            //			}
            //			req.teams.Add(teamInfo);
            //			Logic.Protocol.ProtocolProxy.instance.SendProtocol(req);
        }
        public void CLIENT2LOBBY_GET_RANK_ARENA_REPORT_REQ()
        {
            GetRankArenaReportReq req = new GetRankArenaReportReq();
            Logic.Protocol.ProtocolProxy.instance.SendProtocol(req);
        }
        //请求刷新对手
        public void CLIENT2LOBBY_REFRESH_OPPONENTS_REQ()
        {
            RefreshOpponentsReq req = new RefreshOpponentsReq();
            Logic.Protocol.ProtocolProxy.instance.SendProtocol(req);
        }
        public void CLIENT2LOBBY_GET_RANK_ARENA_REWARD_REQ()
        {
            GetRankArenaRewardReq req = new GetRankArenaRewardReq();
            Logic.Protocol.ProtocolProxy.instance.SendProtocol(req);
        }
        //请求排行榜数据
        //public void CLIENT2LOBBY_GET_RANKING_LIST_REQ()
        //{
        //    GetRankingListReq req = new GetRankingListReq();
        //    ProtocolProxy.instance.SendProtocol(req);
        //}
        //请求挑战
        public void CLIENT2LOBBY_RANK_ARENA_CHANLLENGE_REQ(PvpFighterInfo fighter)
        {
            RankArenaChallengeReq req = new RankArenaChallengeReq();
            req.opponentId = (int)fighter.id;
            req.opponentRankNo = fighter.rank;
            ProtocolProxy.instance.SendProtocol(req);
        }
        //请求挑战结算
        public void CLIENT2LOBBY_RANK_ARENA_CHALLENGE_OVER_REQ(int result)
        {
            RankArenaChallengeOverReq req = new RankArenaChallengeOverReq();
            req.result = result;
            ProtocolProxy.instance.SendProtocol(req);
        }

        //请求积分赛结算
        public void CLIENT2LOBBY_PointPvpSettleReq(int result)
        {
            PointPvpSettleReq req = new PointPvpSettleReq();
            req.result = result;
            ProtocolProxy.instance.SendProtocol(req);
        }
        #endregion

        #region sever
        private bool LOBBY2CLIENT_GET_RANK_ARENA_RESP_handler(Observers.Interfaces.INotification note)
        {
            GetRankArenaResp resp = note.Body as GetRankArenaResp;
            PvpProxy.instance.UpdatePvpArenaInfo(resp);
            return true;
        }
        //		private bool LOBBY2CLIENT_RANK_ARENA_TEAM_CHANGE_RESP_handler(Observers.Interfaces.INotification note)
        //		{
        //			//RankArenaTeamChangeResp resp = note.Body as RankArenaTeamChangeResp;
        //			return true;
        //		}
        private bool LOBBY2CLIENT_GET_RANK_ARENA_REPORT_RESP_handler(Observers.Interfaces.INotification note)
        {
            GetRankArenaReportResp resp = note.Body as GetRankArenaReportResp;
            PvpBattleReportProxy.instance.AddPvpBattleReport(resp.reports);
            PvpBattleReportProxy.instance.UpdateBattleReportUI();
            return true;
        }
        private bool LOBBY2CLIENT_RANK_ARENA_REPORT_UPDATE_RESP_handler(Observers.Interfaces.INotification note)
        {
            RankArenaReportUpdateResp resp = note.Body as RankArenaReportUpdateResp;
            PvpBattleReportProxy.instance.AddPvpBattleReport(resp.newReport);
            PvpBattleReportProxy.instance.RemovePvpBattleReport(resp.delReportId);
            PvpBattleReportProxy.instance.UpdateBattleReportUI();
            return true;
        }
        private bool LOBBY2CLIENT_RANK_ARENA_REFRESH_OPPONENTS_RESP_handler(Observers.Interfaces.INotification note)
        {
            RefreshOpponentsResp resp = note.Body as RefreshOpponentsResp;
            PvpProxy.instance.UpdatePvpArenaFighters(resp.opponents, resp.remainRefreshTimes, resp.refreshTimesCoolingOverTime);
            PvpProxy.instance.UpdateFighterByProtocol();
            return true;
        }
        //private bool LOBBY2CLIENT_RANK_ARENA_REWARD_RESP_handler(Observers.Interfaces.INotification note)
        //{
        //    GetRankArenaRewardResp resp = note.Body as GetRankArenaRewardResp;
        //    //PvpProxy.instance.UpdatePvpGainRewardSuccess();
        //    return true;
        //}
        //private bool LOBBY2CLIENT_GET_RANKING_LIST_RESP_handler(Observers.Interfaces.INotification note)
        //{
        //    GetRankingListResp resp = note.Body as GetRankingListResp;
        //    PvpProxy.instance.UpdateTopHundredRankingListSuccess(resp.list);
        //    return true;
        //}
        //pvp挑战玩家
        private bool LOBBY2CLIENT_RANK_ARENA_CHALLENGE_RESP_handler(Observers.Interfaces.INotification note)
        {
            RankArenaChallengeResp resp = note.Body as RankArenaChallengeResp;
            //own
            //			FightPlayerInfo ownFightPlayerInfo = new FightPlayerInfo(GameProxy.instance.PlayerInfo, resp.selfTeamData.player);
            //			List<FightHeroInfo> ownFightHeroInfoList = new List<FightHeroInfo>();
            //			TeamHeroProtoData data;
            //			int count = resp.selfTeamData.heros.Count;
            //
            //			for(int i = 0;i<count;i++)
            //			{
            //				data = resp.selfTeamData.heros[i];
            //				ownFightHeroInfoList.Add(new FightHeroInfo(HeroProxy.instance.GetHeroInfo((uint)data.id),data));
            //			}
            //			//enemy
            //			FightPlayerInfo enemyFightPlayerInfo = new FightPlayerInfo(PvpProxy.instance.ChallengeFighter.playerInfo,resp.opponentTeamData.player);
            //			List<FightHeroInfo> enemyFightHeroInfoList = new List<FightHeroInfo>();
            //			count = resp.opponentTeamData.heros.Count;
            //			for(int i = 0;i<count;i++)
            //			{
            //				data = resp.opponentTeamData.heros[i];
            //
            //				enemyFightHeroInfoList.Add(new FightHeroInfo(PvpProxy.instance.ChallengeFighter.GetHeroInfo(data.id),data));
            //			}
            //drop
			Observers.Facade.Instance.SendNotification("RANK_ARENA_CHALLENGE_RESP_LUA");
            List<DropItem> dropList = new List<DropItem>();
            DropItem dropItem = new DropItem();
            dropItem.itemType = (int)BaseResType.Honor;
            dropItem.itemNum = GlobalData.GetGlobalData().challengeSuccPrize;
            dropList.Add(dropItem);
            FightProxy.instance.SetData(resp.selfTeamData, resp.opponentTeamData, dropList);
            FightController.instance.fightType = FightType.Arena;
            FightController.instance.PreReadyFight();
            return true;
        }
        private bool LOBBY2CLIENT_RANK_ARENA_CHALLENGE_OVER_RESP_handler(Observers.Interfaces.INotification note)
        {
            RankArenaChallengeOverResp resp = note.Body as RankArenaChallengeOverResp;
            PvpProxy.instance.UpdatePvpFightOverByProtocol();
            return true;
        }
        //		//阵型
        //		private bool LOBBY2CLIENT_RANK_ARENA_TEAM_RESP_handler(Observers.Interfaces.INotification note)
        //		{
        //			RankArenaTeamResp resp = note.Body as RankArenaTeamResp;
        //			PvpFormationProxy.instance.UpdateTeamInfosByProtocol(resp.teams);
        //			return true;
        //		}

        //响应匹配积分赛
        private bool LOBBY2CLIENT_POINT_PVP_CHALLENGE_RESP_handler(Observers.Interfaces.INotification note)
        {
            PointPvpChallengeResp resp = note.Body as PointPvpChallengeResp;
            FightProxy.instance.SetData(resp.selfTeamData, resp.opponentTeamData);
            //LuaInterface.LuaScriptMgr.Instance.GetLuaTable("game")



            FightController.instance.fightType = FightType.PVP;
            FightController.instance.PreReadyFight();
            return true;
        }

        //响应积分赛结算(S->C)
        private bool LOBBY2CLIENT_POINT_PVP_SETTLE_RESP_handler(Observers.Interfaces.INotification note)
        {
            PointPvpSettleResp resp = note.Body as PointPvpSettleResp;
            PvpProxy.instance.UpdatePvpRaceFightOverByProtocol(resp.point, resp.keepWinTimes);
            return true;
        }
		private bool LOBBY2CLIENT_SynPvpActionResp_handler(Observers.Interfaces.INotification note)
		{
			IntProto resp = note.Body as IntProto;
			GameProxy.instance.PvpAction = resp.value;
			GameProxy.instance.onPvpActionInfoUpdateDelegateByProtocol();
			return true;
		}
        #endregion
    }

}

