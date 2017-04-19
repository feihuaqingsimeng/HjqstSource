using UnityEngine;
using System.Collections.Generic;
using Logic.Protocol.Model;
using Common.GameTime.Controller;
using Logic.Enums;
using Logic.WorldBoss.Controller;
using Common.Localization;
using Logic.Fight.Controller;
using LuaInterface;

namespace Logic.WorldBoss.Model
{
    public class WorldBossProxy : SingletonMono<WorldBossProxy>
    {
        public delegate void OnWorldBossStatusChangedDelegate();
        public OnWorldBossStatusChangedDelegate onWorldBossStatusChangedDelegate;

        public delegate void OnWorldBossInfoUpdateDelegate();
        public OnWorldBossInfoUpdateDelegate onWorldBossInfoUpdateDelegate;

        public delegate void OnWorldBossInspireTimesUpdateDelegate();
        public OnWorldBossInspireTimesUpdateDelegate onWorldBossInspireTimesUpdateDelegate;

        public delegate void OnWorldBossCurrHPChangedDelegate(int bossCurrHP);
        public OnWorldBossCurrHPChangedDelegate onWorldBossCurrHPChangedDelegate;


        public delegate void OnWorldBossOpenCountDownTimeUpdateDelegate(int second);
        public OnWorldBossOpenCountDownTimeUpdateDelegate onWorldBossOpenCountDownTimeUpdateDelegate;

        public delegate void OnWorldBossOverCountDownTimeUpdateDelegate(int second);
        public OnWorldBossOverCountDownTimeUpdateDelegate onWorldBossOverCountDownTimeUpdateDelegate;

        public delegate void OnWorldBossFightOverDelegate();
        public OnWorldBossFightOverDelegate onWorldBossFightOverDelegate;

        public delegate void OnWorldBossKilledByOthersDelegate();
        public OnWorldBossKilledByOthersDelegate onWorldBossKilledByOthersDelegate;

        public delegate void OnWorldBossActivityEndDelegate();
        public OnWorldBossActivityEndDelegate onWorldBossActivityEndDelegate;

        #region world boss status info
        private bool _isOpen;
        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
        }

        private long _openTime;
        public long OpenTime
        {
            get
            {
                return _openTime;
            }
        }

        private long _overTime;
        public long OverTime
        {
            get
            {
                return _overTime;
            }
        }

		public int OpenDiffTimeWithServerTimeInSecond
		{
			get
			{
				return (int)TimeController.instance.GetDiffTimeWithServerTimeInSecond(WorldBossProxy.instance.OpenTime);
			}
		}

		public int OverDiffTimeWithServerTimeInSecond
		{
			get
			{
				return (int)TimeController.instance.GetDiffTimeWithServerTimeInSecond(WorldBossProxy.instance.OverTime);
			}
		}
        #endregion world boss status info

        #region world boss info
        private List<WorldBossHurtRankProto> _worldBossHurtRankProtoList;
        public List<WorldBossHurtRankProto> WorldBossHurtRankProtoList
        {
            get
            {
                return _worldBossHurtRankProtoList;
            }
        }

        private int _bossID;
        public int BossID
        {
            get
            {
                return _bossID;
            }
        }

        private int _bossLevel;
        public int BossLevel
        {
            get
            {
                return _bossLevel;
            }
        }

        private int _bossHPUpperLimit;
        public int BossHPUpperLimit
        {
            get
            {
                return _bossHPUpperLimit;
            }
        }

        private int _bossCurrHP;
        public int BossCurrHP
        {
            get
            {
                return _bossCurrHP;
            }
        }

        private int _totalHurt;
        public int TotalHurt
        {
            get
            {
                return _totalHurt;
            }
        }

        private int _hurtPercent;
        public int HurtPercent
        {
            get
            {
                return _hurtPercent;
            }
        }

        private int _hurtRankNo;
        public int HurtRankNo
        {
            get
            {
                return _hurtRankNo;
            }
        }

        private int _inspireTimes;
        public int InspireTimes
        {
            get
            {
                return _inspireTimes;
            }
        }

        private long _reviveCoolingEndTime;
        public long ReviveCoolingEndTime
        {
            get
            {
                return _reviveCoolingEndTime;
            }
        }
        #endregion world boss info

        #region world boss fight info
        private WorldBossFightProto _worldBossFightProto;
        public WorldBossFightProto WorldBossFightProto
        {
            get
            {
                return _worldBossFightProto;
            }
        }
        #endregion world boss fight info

        void Awake()
        {
            instance = this;
        }

        void Update()
        {
//            if (!_isOpen)
//            {
//                if (_openTime > 0)
//                {
//                    if (onWorldBossOpenCountDownTimeUpdateDelegate != null)
//                    {
//                        int diffTimeWithServerTimeInSecond = (int)TimeController.instance.GetDiffTimeWithServerTimeInSecond(WorldBossProxy.instance.OpenTime);
//                        onWorldBossOpenCountDownTimeUpdateDelegate(diffTimeWithServerTimeInSecond);
//                    }
//                }
//            }
//            else
//            {
//                if (_overTime > 0)
//                {
//                    if (onWorldBossOverCountDownTimeUpdateDelegate != null)
//                    {
//                        int diffTimeWithServerTimeInSecond = (int)TimeController.instance.GetDiffTimeWithServerTimeInSecond(WorldBossProxy.instance.OverTime);
//                        onWorldBossOverCountDownTimeUpdateDelegate(diffTimeWithServerTimeInSecond);
//                    }
//                }
//            }

			if (_overTime > 0)
			{
				if (onWorldBossOverCountDownTimeUpdateDelegate != null)
				{
					int diffTimeWithServerTimeInSecond = (int)TimeController.instance.GetDiffTimeWithServerTimeInSecond(WorldBossProxy.instance.OverTime);
					onWorldBossOverCountDownTimeUpdateDelegate(diffTimeWithServerTimeInSecond);
				}
			}
			else if (_openTime > 0)
			{
				if (onWorldBossOpenCountDownTimeUpdateDelegate != null)
				{
					int diffTimeWithServerTimeInSecond = (int)TimeController.instance.GetDiffTimeWithServerTimeInSecond(WorldBossProxy.instance.OpenTime);
					onWorldBossOpenCountDownTimeUpdateDelegate(diffTimeWithServerTimeInSecond);
				}
			}
        }

        public void OnWorldBossStatusChanged(bool isOpen, long openTime, long overTime)
        {
            _isOpen = isOpen;
            _openTime = openTime;
            _overTime = overTime;
            if (onWorldBossStatusChangedDelegate != null)
            {
                onWorldBossStatusChangedDelegate();
            }
			LuaTable worldBossModel = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","world_boss_model")[0];
			worldBossModel.GetLuaFunction("OnWorldBossStatusChanged").Call(isOpen,openTime,overTime);
        }

        public void OnWorldBossInfoUpdate(WorldBossResp worldBossResp)
        {
            _worldBossHurtRankProtoList = worldBossResp.rankList;
            _bossID = worldBossResp.bossId;
            _bossLevel = worldBossResp.bossLv;
            _bossHPUpperLimit = worldBossResp.bossHpUpperLimit;
            _bossCurrHP = worldBossResp.bossCurrHp;
            _totalHurt = worldBossResp.totalHurt;
            _hurtPercent = worldBossResp.hurtPercent;
            _hurtRankNo = worldBossResp.hurtRankNo;
            _inspireTimes = worldBossResp.inspireTimes;
            _reviveCoolingEndTime = worldBossResp.reviveCoolingEndTime;
            Debugger.Log("********** World Boss Revive Time Test **********");
            Debugger.Log(string.Format("Current Server Time:{0}", TimeController.instance.ServerTimeTicksSecond));
            Debugger.Log(string.Format("Next revive time stamp:{0}", _reviveCoolingEndTime));
            int reviveDiffTimeWithServer = (int)TimeController.instance.GetDiffTimeWithServerTimeInSecond(_reviveCoolingEndTime);
            Debugger.Log(string.Format("Revive Diff Time With Server In Second:{0}", reviveDiffTimeWithServer));
            Debugger.Log("********** World Boss Revive Time Test **********");
            if (onWorldBossInfoUpdateDelegate != null)
            {
                onWorldBossInfoUpdateDelegate();
            }
        }

        public void OnWorldBossInspireTimesUpdate(int inspireTimes)
        {
            _inspireTimes = inspireTimes;
            if (onWorldBossInspireTimesUpdateDelegate != null)
            {
                onWorldBossInspireTimesUpdateDelegate();
            }
        }

        public void OnReviveSuccess()
        {
            _reviveCoolingEndTime = 0;
            if (onWorldBossInfoUpdateDelegate != null)
            {
                onWorldBossInfoUpdateDelegate();
            }
        }

        public void OnWorldBossFightStart(WorldBossFightProto worldBossFightProto)
        {
            _worldBossFightProto = worldBossFightProto;
        }

        public void OnWorldBossHurtSyn(int bossCurrHP)
        {
            _bossCurrHP = bossCurrHP;
            if (onWorldBossCurrHPChangedDelegate != null)
            {
                onWorldBossCurrHPChangedDelegate(_bossCurrHP);
            }
        }

        public void OnWorldBossKilledByOther()
        {
            if (onWorldBossKilledByOthersDelegate != null)
            {
                onWorldBossKilledByOthersDelegate();
            }
        }

        public void OnWorldBossActivityEnd()
        {
            if (onWorldBossActivityEndDelegate != null)
            {
                onWorldBossActivityEndDelegate();
            }
        }

        public void OnWorldBossFightOver()
        {
            if (onWorldBossFightOverDelegate != null)
            {
                onWorldBossFightOverDelegate();
            }
        }

        public void OnWorldBossReallyFightOver()
        {
            if (WorldBossController.instance.isWin)
            {
                if (WorldBossController.instance.isWorldBossKilledByOther)
                {
                    On_FightResult_WorldBossKilledByOther();
                }
                else
                {
                    On_FightResult_WorldBossKilledByMe();
                }
            }
            else
            {
                if (WorldBossController.instance.isWorldBossActivityEnd)
                {
                    On_FightResult_WorldBossActivityEnd();
                }
                else
                {
                    On_FightResult_PlayerKilledByBoss();
                }
            }
        }

        public void On_FightResult_PlayerKilledByBoss()
        {
            Fight.Controller.FightController.instance.QuitFight(true, PreQuitFight);
        }

        private void PreQuitFight()
        {
            Logic.UI.LoadGame.Controller.LoadGameController.instance.SetDelayTime(FightController.FIGHT_TRANSITION_TIME, LoadFinishedHandler);
        }

        private void LoadFinishedHandler()
        {
            FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_WorldBoss, null, true);
        }

        public void On_FightResult_WorldBossKilledByMe()
        {
			Logic.UI.Tips.View.CommonErrorTipsView.Open(Localization.Get("ui.world_boss_fight.boss_was_killed_by_me"), OnClickQuitWorldBossFight);
        }

        public void On_FightResult_WorldBossKilledByOther()
        {
			Logic.UI.Tips.View.CommonErrorTipsView.Open(Localization.Get("ui.world_boss_fight.boss_was_killed_by_other"), OnClickQuitWorldBossFight);
        }

        public void On_FightResult_WorldBossActivityEnd()
        {
            Logic.UI.Tips.View.ConfirmTipsView.Open(Localization.Get("ui.world_boss_fight.boss_activity_end"), OnClickQuitWorldBossFight);
        }

        public void OnClickQuitWorldBossFight()
        {
            Fight.Controller.FightController.instance.QuitFight(true, PreQuitFight);
        }
    }
}