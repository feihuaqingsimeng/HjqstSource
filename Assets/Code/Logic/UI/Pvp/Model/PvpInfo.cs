using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.GameTime.Controller;

namespace Logic.UI.Pvp.Model
{
	public class PvpInfo 
	{
		// 当前排名
		public int rankNo;
		// 总省场数
		public int totalWinTimes;
		// 挑战冷却时间(ms)
		public long challengeCoolingOverTime;
		// 每日挑战次数上限
		public int challengeMaxCount;
		// 剩余挑战次数
		public int remainChallengeTimes;
		//挑战次数下次恢复时间(ms)
		public long challengeTimesNextRecoverTime ;
		// 挑战次数下次恢复时间剩余(ms)
		public float challengeTimesCountDown ;
		// 每日刷新次数上限
		public int refreshMaxCount;
		// 剩余刷新次数
		public int remainRefreshTimes;
		//刷新冷却结束时间(ms)
		public long refreshTimesCoolingOverTime ;
		// 刷新冷却时间剩余(ms)
		public float refreshTimesCountDown;
		// 下次领取奖励时间(ms)
		public long nextGetRewardTime;
		//下次领取奖励时间剩余(ms)
		public float nextGetRewardCountDown;
		// 上次结算排名
		public int lastRankNo;
		// 战斗力
		public int fightingPower;
		//竞技场可用胜利场数
		public int canUseWinTimes = 0;
		// 列表玩家信息
		public List<PvpFighterInfo> fighterInfoList = new List<PvpFighterInfo>();

		public void SetChallengeTimesNextRecoverTime(long time)
		{
			challengeTimesNextRecoverTime = time;
			UpdateChallengeTimesCountDown();
		}

		public void SetRefreshTimesCoolingOverTime(long time)
		{
			refreshTimesCoolingOverTime = time;
			UpdateRefreshTimesCountDown();
		}

		public void SetNextGetRewardTime(long time)
		{
			nextGetRewardTime = time;
			UpdateNextGetRewardCountDown();
		}
		public void UpdateChallengeTimesCountDown()
		{
			challengeTimesCountDown = Mathf.Max( challengeTimesNextRecoverTime-TimeController.instance.ServerTimeTicksSecond*1000,0);
		}
		public void UpdateRefreshTimesCountDown()
		{
			refreshTimesCountDown = Mathf.Max( refreshTimesCoolingOverTime-TimeController.instance.ServerTimeTicksSecond*1000,0);
		}
		public void UpdateNextGetRewardCountDown()
		{
			nextGetRewardCountDown = Mathf.Max(nextGetRewardTime-TimeController.instance.ServerTimeTicksSecond*1000,0);
		}
	}

}
