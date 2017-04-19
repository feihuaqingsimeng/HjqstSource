using UnityEngine;
using System.Collections.Generic;
using Logic.Enums;
using Logic.Protocol.Model;
using Logic.UI.ManageHeroes.Model;
using Logic.Formation.Model;

namespace Logic.UI.ManageHeroes.Controller
{
	public class ManageHeroesController : SingletonMono<ManageHeroesController>
	{
		void Awake ()
		{
			instance = this;
		}

		void Start ()
		{
			//Observers.Facade.Instance.RegisterObserver(((int)MSG.TeamResp).ToString(), LOBBY2CLIENT_TEAM_RESP_handler);
			//Observers.Facade.Instance.RegisterObserver(((int)MSG.PveTeamChangeResp).ToString(), LOBBY2CLIENT_PVE_TEAM_CHANGE_RESP_handler);
		}

		void OnDestroy ()
		{
			if (Observers.Facade.Instance != null)
			{
				//Observers.Facade.Instance.RemoveObserver(((int)MSG.TeamResp).ToString(), LOBBY2CLIENT_TEAM_RESP_handler);
				//Observers.Facade.Instance.RemoveObserver(((int)MSG.PveTeamChangeResp).ToString(), LOBBY2CLIENT_PVE_TEAM_CHANGE_RESP_handler);
			}
		}

		public void AddHeroToFormaiton (FormationPosition formationPosition, uint heroInstanceID)
		{
			ManageHeroesProxy.instance.AddHeroToFormaiton(formationPosition, heroInstanceID);
		}
		
		public void RemoveHeroFromFomation (uint heroInstanceID)
		{
			ManageHeroesProxy.instance.RemoveHeroFromFormation(heroInstanceID);
		}

		public void RemoveHeroFromFormationAt (FormationPosition formationPosition)
		{
			ManageHeroesProxy.instance.RemoveHeroFromFormationAt(formationPosition);
		}

		public void FinishEmbattle ()
		{
			ManageHeroesProxy.instance.OnEmbattleSuccess();
		}

		#region server
		public bool LOBBY2CLIENT_TEAM_RESP_handler (Observers.Interfaces.INotification note)
		{
//			TeamResp teamResp = note.Body as TeamResp;
//			ManageHeroesProxy.instance.UpdateTeamInfos(teamResp);
			return true;
		}

		public void CLIENT2LOBBY_PVE_TEAM_CHANGE_REQ ()
		{
			//FormationController.instance.CLIENT2LOBBY_TeamChange_REQ(ManageHeroesProxy.instance.CurrentSelectFormationTeamType);
//			PveTeamChangeReq pveTeamChangeReq = new PveTeamChangeReq();
//			pveTeamChangeReq.selectPveTeamNo = ManageHeroesProxy.instance.CurrentSelectedFormationNo;
//
//			Dictionary<FormationTeamType ,FormationTeamInfo> teamDic = ManageHeroesProxy.instance.formationTeamDictionary;
//			int teamNo = 1;
//			foreach(var team in teamDic)
//			{
//				TeamInfo teamInfo = new TeamInfo();
//				teamInfo.teamNo = teamNo;
//				SortedDictionary<FormationPosition, uint> formationDictionary = team.Value.teamPosDictionary;
//				List<FormationPosition> formationPositions = new List<FormationPosition>(formationDictionary.Keys);
//				int formationPositionCount = formationPositions.Count;
//				for (int j = 0; j < formationPositionCount; j++)
//				{
//					FormationPosition formationPosition = formationPositions[j];
//					PositionInfo positionInfo = new PositionInfo();
//					positionInfo.posIndex = (int)formationPosition;
//					positionInfo.heroId = (int)formationDictionary[formationPosition];
//					teamInfo.posInfos.Add(positionInfo);
//				}
//				pveTeamChangeReq.pveChangeTeams.Add(teamInfo);
//				teamNo++;
//			}
//			Logic.Protocol.ProtocolProxy.instance.SendProtocol(pveTeamChangeReq);

//			List<int> keys = new List<int>(formationsDictionary.Keys);
//			int keysCount = keys.Count;
//			for (int i = 0; i < keysCount; i++)
//			{
//				int teamNo = keys[i];
//				TeamInfo teamInfo = new TeamInfo();
//				teamInfo.teamNo = teamNo;
//
//				SortedDictionary<FormationPosition, uint> formationDictionary = formationsDictionary[teamNo];
//				List<FormationPosition> formationPositions = new List<FormationPosition>(formationDictionary.Keys);
//				int formationPositionCount = formationPositions.Count;
//				for (int j = 0; j < formationPositionCount; j++)
//				{
//					FormationPosition formationPosition = formationPositions[j];
//					PositionInfo positionInfo = new PositionInfo();
//					positionInfo.posIndex = (int)formationPosition;
//					positionInfo.heroId = (int)formationDictionary[formationPosition];
//					teamInfo.posInfos.Add(positionInfo);
//				}
//				pveTeamChangeReq.pveChangeTeams.Add(teamInfo);
//			}
//
//			Logic.Protocol.ProtocolProxy.instance.SendProtocol(pveTeamChangeReq);
		}

//		public bool LOBBY2CLIENT_PVE_TEAM_CHANGE_RESP_handler (Observers.Interfaces.INotification note)
//		{
//			//PveTeamChangeResp pveTeamChangeResp = note.Body as PveTeamChangeResp;
//			return true;
//		}
		#endregion server
	}
}
