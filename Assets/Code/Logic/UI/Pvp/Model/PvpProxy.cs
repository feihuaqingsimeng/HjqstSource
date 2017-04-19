using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Game.Model;
using Logic.UI.Pvp.Controller;
using Logic.Player.Model;
using Logic.Protocol.Model;
using Common.GameTime.Controller;
using LuaInterface;

namespace Logic.UI.Pvp.Model
{
    public class PvpProxy : SingletonMono<PvpProxy>
    {
        public delegate void UpdateTime(int remainTimeSec);

        public System.Action OnUpdateRefreshFighterTimeDelegate;
        public System.Action OnUpdatePvpInfoDelegate;
        //public System.Action OnUpdateMyRewardTimeDelegate;
        public System.Action OnUpdateGainRewardSuccessDelegate;
        public System.Action OnUpdateFighterDelegate;

        public delegate void UpdateTopHundredRankingDelegate(List<PvpFighterInfo> fighterList);
        public UpdateTopHundredRankingDelegate onUpdateTopHundredRankingSuccessDelegate;
        //战斗结束
        public System.Action onPvpFightOverDelegate;
        public System.Action onPvpRaceFightOverDelegate;

        public bool IsChangeFighterRecoverOver = true;

        public int pvpRace;
        public int pvpKeepWinTimes;

        private PvpInfo _pvpInfo = new PvpInfo();
        public PvpInfo PvpInfo
        {
            get
            {
                return _pvpInfo;
            }
        }

        public PvpFighterInfo ChallengeFighter
        {
            get;

            set;
        }
        void Awake()
        {
            instance = this;
        }

        public void UpdateChangeFighterTime()
        {
            if (_pvpInfo.remainRefreshTimes > 0)
            {
                if (_pvpInfo.refreshTimesCountDown > 0)
                {
                    IsChangeFighterRecoverOver = false;
                    _pvpInfo.UpdateRefreshTimesCountDown();
                    if (_pvpInfo.refreshTimesCountDown == 0)
                    {
                        IsChangeFighterRecoverOver = true;
                    }
                    if (OnUpdateRefreshFighterTimeDelegate != null)
                    {
                        OnUpdateRefreshFighterTimeDelegate();
                    }


                }
                else
                {
                    //					bool over = IsChangeFighterRecoverOver;

                    IsChangeFighterRecoverOver = true;
                    //					if(!over)
                    //					{
                    //						if(OnUpdateRefreshFighterTimeDelegate!= null)
                    //						{
                    //							OnUpdateRefreshFighterTimeDelegate();
                    //						}
                    //					}
                }
            }
            else
            {
                IsChangeFighterRecoverOver = true;
            }
        }
//        public void UpdateMyPvpRewardTime()
//        {
//            if (_pvpInfo.nextGetRewardTime > 0)
//            {
//                _pvpInfo.UpdateNextGetRewardCountDown();
//                if (OnUpdateMyRewardTimeDelegate != null)
//                {
//                    OnUpdateMyRewardTimeDelegate();
//                }
//                //				if(_pvpInfo.nextGetRewardTime <= 0)
//                //				{
//                //					RequestUpdatePvpInfo();
//                //				}
//            }
//        }

        //		private void RequestUpdatePvpInfo()
        //		{
        //			PvpController.instance.CLIENT2LOBBY_GET_RANK_ARENA_REQ();
        //		}

        #region update by server
        public void UpdatePvpArenaInfo(GetRankArenaResp resp)
        {

            _pvpInfo.rankNo = resp.rankNo;
            _pvpInfo.totalWinTimes = resp.totalWinTimes;
            //challenge
            _pvpInfo.challengeCoolingOverTime = resp.challengeCoolingOverTime;
            _pvpInfo.challengeMaxCount = resp.challengeTimesUp;
            _pvpInfo.remainChallengeTimes = resp.remainChallengeTimes;
            _pvpInfo.SetChallengeTimesNextRecoverTime(resp.challengeTimesNextRecoverTime);
            //refresh
            _pvpInfo.refreshMaxCount = resp.refreshListingTimesUp;
            _pvpInfo.remainRefreshTimes = resp.remainRefreshTimes;
            _pvpInfo.SetRefreshTimesCoolingOverTime(resp.refreshTimesCoolingOverTime);
            //reward
            _pvpInfo.SetNextGetRewardTime(resp.nextGetRewardTime);
            //last rank,gain reward use it
            _pvpInfo.lastRankNo = resp.lastSettleRankNo;
            //power
            _pvpInfo.fightingPower = resp.fightingPower;
			_pvpInfo.canUseWinTimes = resp.canUseWinTimes;
            //Debug.Log("rankNo:" + resp.rankNo);
			LuaTable arenaModelLua = (LuaTable) LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","arena_model")[0];
			arenaModelLua.GetLuaFunction("SetCanUseWinTimes").Call(resp.canUseWinTimes);
            GameProxy.instance.OnPvpActionInfoUpdate(_pvpInfo.remainChallengeTimes, _pvpInfo.challengeMaxCount, _pvpInfo.challengeTimesNextRecoverTime);

            UpdatePvpArenaFighters(resp.opponents, resp.remainRefreshTimes, resp.refreshTimesCoolingOverTime);
            if (OnUpdatePvpInfoDelegate != null)
            {
                OnUpdatePvpInfoDelegate();
            }
        }
        public void UpdatePvpArenaFighters(List<RankArenaOpponentProtoData> dataList, int remainRefreshTimes, long refreshTimesCoolingOverTime)
        {
            int count = dataList.Count;
            _pvpInfo.fighterInfoList.Clear();
            for (int i = 0; i < count; i++)
            {
                PvpFighterInfo info = new PvpFighterInfo(dataList[i]);
                _pvpInfo.fighterInfoList.Add(info);
               // Debugger.Log(info.ToString());

            }
            _pvpInfo.fighterInfoList.Sort(ComparePvpFighterInfo);
            _pvpInfo.remainRefreshTimes = remainRefreshTimes;
            _pvpInfo.SetRefreshTimesCoolingOverTime(refreshTimesCoolingOverTime);
        }
        private int ComparePvpFighterInfo(PvpFighterInfo a, PvpFighterInfo b)
        {
            return a.rank - b.rank;
        }
        //public void UpdatePvpGainRewardSuccess()
        //{
        //    if (OnUpdateGainRewardSuccessDelegate != null)
        //    {
        //        OnUpdateGainRewardSuccessDelegate();
        //    }
        //}

        public void UpdatePvpFightOverByProtocol()
        {
            if (onPvpFightOverDelegate != null)
            {
                onPvpFightOverDelegate();
            }
        }

        public void UpdatePvpRaceFightOverByProtocol(int race, int keepWinTimes)
        {
            pvpRace = race;
            pvpKeepWinTimes = keepWinTimes;
            if (onPvpRaceFightOverDelegate != null)
            {
                onPvpRaceFightOverDelegate();
            }
        }

        public void UpdateFighterByProtocol()
        {
            if (OnUpdateFighterDelegate != null)
            {
                OnUpdateFighterDelegate();
            }
        }
        #endregion

    }

}

