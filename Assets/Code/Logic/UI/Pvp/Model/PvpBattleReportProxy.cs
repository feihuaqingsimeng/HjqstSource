using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Protocol.Model;
using Common.GameTime.Controller;
using Common.Util;

namespace Logic.UI.Pvp.Model
{
	public class PvpBattleReportProxy : SingletonMono<PvpBattleReportProxy> 
	{
		public System.Action OnUpdateBattleReportDelegate;

		private Dictionary<int,PvpBattleReportInfo> _battleReportDictionary = new Dictionary<int,PvpBattleReportInfo>();
		public  List<PvpBattleReportInfo> BattleReportList
		{
			get
			{
				List<PvpBattleReportInfo> reportList = _battleReportDictionary.GetValues();
				reportList.Sort(ComparePvpBattleReportInfo);
				return reportList;
			}
		}

		void Awake()
		{
			instance = this;
		}
		public static int ComparePvpBattleReportInfo(PvpBattleReportInfo a,PvpBattleReportInfo b)
		{
			return (int)((a.time.Ticks-b.time.Ticks+0.0)/10000000L);
		}
		public void UpdateBattleReportUI()
		{
			if(OnUpdateBattleReportDelegate!= null)
			{
				OnUpdateBattleReportDelegate();
			}
		}
		#region update from server
		public void AddPvpBattleReport(List<RankArenaReportProtoData> reportData)
		{
			_battleReportDictionary.Clear();

			int count = reportData.Count;
			for(int i = 0;i<count;i++)
			{
				AddPvpBattleReport(reportData[i]);
			}

		}
		public void AddPvpBattleReport(RankArenaReportProtoData data)
		{
			System.DateTime time = TimeUtil.FormatTime((int)(data.fightTime/1000));
			PvpBattleReportInfo info = new PvpBattleReportInfo(data.id,time,data.isWin,data.isChallenger, data.opponentRoleName,data.srcRankNo != data.desRankNo,data.desRankNo);
			
			_battleReportDictionary.Add(info.id,info);
		}
		public void RemovePvpBattleReport(int id)
		{
			_battleReportDictionary.Remove(id);
		}
		#endregion
	}
}

