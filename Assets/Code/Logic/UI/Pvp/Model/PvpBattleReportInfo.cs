using UnityEngine;
using System.Collections;
using Common.Util;
using Common.GameTime.Controller;
using System;
using Common.Localization;

namespace Logic.UI.Pvp.Model
{
	public class PvpBattleReportInfo 
	{
		public int id;
		public DateTime time;
		public bool isWin;
		public bool isChallenger;
		public string fighterName;
		public bool isRankChange;
		public int rank;

		public PvpBattleReportInfo( int id,DateTime time,bool isWin,bool isChallenger ,string fighterName,bool isRankChange,int rank)
		{
			this.id = id;
			this.time = time;
			this.isWin = isWin;
			this.isChallenger = isChallenger;
			this.fighterName = fighterName;
			this.isRankChange = isRankChange;
			this.rank = rank;
		}
		public override string ToString ()
		{
			return string.Format("{0}{1}{2}",GetTimeString(),GetFightWho(),GetRankChange());
		}

		private string GetTimeString()
		{
			TimeSpan deltaTime = TimeController.instance.ServerTime-time;
			if(deltaTime.Days>=1)
			{
				return string.Format(Localization.Get("ui.pvp_battle_report.info_time4"),deltaTime.Days);
			}
			if(deltaTime.Hours>=1)
			{
				return string.Format(Localization.Get("ui.pvp_battle_report.info_time3"),deltaTime.Hours);
			}
			if(deltaTime.Minutes >= 5)
			{
				return string.Format(Localization.Get("ui.pvp_battle_report.info_time2"),deltaTime.Minutes);
			}
			return Localization.Get("ui.pvp_battle_report.info_time1");
		}
		private string GetFightWho()
		{
			if(isChallenger)
			{
				if(isWin)
				{
					return string.Format(Localization.Get("ui.pvp_battle_report.info_fighter2"),fighterName);
				}else{
					return string.Format(Localization.Get("ui.pvp_battle_report.info_fighter1"),fighterName);
                }
			}else
			{
				if(isWin)
				{
					return string.Format(Localization.Get("ui.pvp_battle_report.info_fighter4"),fighterName);
				}else{
					return string.Format(Localization.Get("ui.pvp_battle_report.info_fighter3"),fighterName);
                }
            }
            
        }
        private string GetRankChange()
		{
			if(isRankChange)
			{
				if(isWin)
				{
					return string.Format(Localization.Get("ui.pvp_battle_report.info_rank2"),rank);
				}else{
					return string.Format(Localization.Get("ui.pvp_battle_report.info_rank1"),rank);
				}
			}else{
				return Localization.Get("ui.pvp_battle_report.info_rank3");//未变化
			}
		}
	}

}
