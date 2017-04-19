using UnityEngine;
using System.Collections.Generic;
using Logic.Protocol.Model;
using Logic.Enums;
using Logic.Game.Model;
using Logic.Hero.Model;
using Logic.Formation.Model;

namespace Logic.UI.ManageHeroes.Model
{
	public class ManageHeroesProxy : SingletonMono<ManageHeroesProxy>
	{
		private int _currentSelectedFormationNo = 0;
		public int CurrentSelectedFormationNo
		{
			get
			{
				LuaInterface.LuaTable formationModelLuaTable = LuaInterface.LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "formation_model")[0] as LuaInterface.LuaTable;
				_currentSelectedFormationNo = formationModelLuaTable["CurrentPVESelectFormationTeamType"].ToString().ToInt32() - (int)FormationTeamType.pveFirstTeam + 1;
				return _currentSelectedFormationNo;
			}
			set
			{
                _currentSelectedFormationNo = value;
                LuaInterface.LuaTable luaTable = LuaInterface.LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "formation_model")[0] as LuaInterface.LuaTable;
                luaTable.GetLuaFunction("SetCurrentPVESelectFormationTeamType").Call((int)CurrentSelectFormationTeamType);
			}
		}
		public FormationTeamType CurrentSelectFormationTeamType
		{
			get
			{
				return (FormationTeamType)(CurrentSelectedFormationNo-1+(int)FormationTeamType.pveFirstTeam);
			}
			set
			{
                CurrentSelectedFormationNo = (int)value - (int)FormationTeamType.pveFirstTeam + 1;
			}
		}

		private Dictionary<FormationTeamType ,FormationTeamInfo> _formationTeamDictionary = new Dictionary<FormationTeamType, FormationTeamInfo>();
		public Dictionary<FormationTeamType ,FormationTeamInfo> formationTeamDictionary
		{
			get
			{
				if(_formationTeamDictionary.Count == 0)
				{
					FormationTeamInfo info = FormationProxy.instance.GetFormationTeam(FormationTeamType.pveFirstTeam);
					if(info != null)
						_formationTeamDictionary.Add(FormationTeamType.pveFirstTeam,info);
					info = FormationProxy.instance.GetFormationTeam(FormationTeamType.pveSecondTeam);
					if(info != null)
						_formationTeamDictionary.Add(FormationTeamType.pveSecondTeam,info);
					info = FormationProxy.instance.GetFormationTeam(FormationTeamType.pveThirdTeam);
					if(info != null)
						_formationTeamDictionary.Add(FormationTeamType.pveThirdTeam,info);
				}
				return _formationTeamDictionary;
			}
		}
		public void ClearFormationTeamDic()
		{
			_formationTeamDictionary.Clear();
		}

		public delegate void OnFormationUpdateHandler ();
		public OnFormationUpdateHandler onFormationUpdateHandler;

		public delegate void OnEmbattleSuccessHandler ();
		public OnEmbattleSuccessHandler onEmbattleSuccessHandler;

		void Awake ()
		{
			instance = this;
		}

		public FormationTeamInfo CurrentFormationTeamInfo
		{
			get
			{
				FormationTeamType type = CurrentSelectFormationTeamType;
				if(formationTeamDictionary.ContainsKey(type))
				{
					return formationTeamDictionary[type];
				}
				return null;
			}
		}
		public SortedDictionary<FormationPosition, uint> CurrentFormationDictionary
		{
			get
			{
				return CurrentFormationTeamInfo.teamPosDictionary;
			}
		}

		public bool IsPositionEmpty (FormationPosition formationPosition)
		{
			return CurrentFormationTeamInfo.IsPositionEmpty(formationPosition);
		}

		public bool IsTeamUnlocked (int teamIndex)
		{
			FormationTeamType type = (FormationTeamType)(teamIndex-1+(int)FormationTeamType.pveFirstTeam);
			if(!formationTeamDictionary.ContainsKey(type))
				return false;
			FormationTeamInfo info = formationTeamDictionary[type];

			return info.teamPosDictionary.Count != 0 ;
		}

		public uint GetCharacterInstanceIDAt (FormationPosition formationPosition)
		{
			uint characterInstanceID = 0;
			CurrentFormationDictionary.TryGetValue(formationPosition, out characterInstanceID);
			return characterInstanceID;
		}

		public FormationPosition GetPlayerPosition (SortedDictionary<FormationPosition, uint> formationDictionary)
		{
			return CurrentFormationTeamInfo.GetPlayerPosition();
		}

		public bool IsHeroInFormation (uint heroInstanceID)
		{
			return CurrentFormationTeamInfo.IsHeroInFormation(heroInstanceID);
		}

		public bool IsHeroInAnyFormation (uint heroInstanceID)
		{
			foreach(var value in _formationTeamDictionary)
			{
				if(value.Value.teamPosDictionary.ContainsValue(heroInstanceID))
				{
					return true;
				}
			}
			return false;
		}

		public HeroInfo GetHeroAt (FormationPosition formationPosition)
		{
			return CurrentFormationTeamInfo.GetHeroAt(formationPosition);
		}

		public FormationPosition GetHeroCurrentFormationPosition (uint heroInstanceID)
		{
			return CurrentFormationTeamInfo.GetHeroFormationPosition(heroInstanceID);
		}

		public void AddHeroToFormaiton (FormationPosition formationPosition, uint heroInstanceID)
		{
			CurrentFormationTeamInfo.AddHeroToFormaiton(formationPosition, heroInstanceID);
			if (onFormationUpdateHandler != null)
			{
				onFormationUpdateHandler();
			}
		}

		public bool RemoveHeroFromFormationAt (FormationPosition formationPosition)
		{
			bool result = CurrentFormationTeamInfo.RemoveHeroFromFormation(formationPosition);
			if (onFormationUpdateHandler != null)
			{
				onFormationUpdateHandler();
			}
			return result ;
		}
		
		public bool RemoveHeroFromFormation (uint heroInstanceID)
		{
			FormationPosition formationPosition = GetHeroCurrentFormationPosition(heroInstanceID);
			return RemoveHeroFromFormationAt(formationPosition);
		}

		public bool CanAddToFormationPosition (FormationPosition formationPosition, uint addCharacterInstanceID)
		{
			return CurrentFormationTeamInfo.CanAddToFormationPosition(formationPosition,addCharacterInstanceID);
		}

		public void TransferPlayer (uint newPlayerInstanceID)
		{
			CurrentFormationTeamInfo.TransferPlayer(newPlayerInstanceID);
		}

		public void OnEmbattleSuccess ()
		{
			if (onEmbattleSuccessHandler != null)
			{
				onEmbattleSuccessHandler();
			}
		}
	}
}
