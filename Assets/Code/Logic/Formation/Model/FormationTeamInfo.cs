using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Enums;
using Logic.Hero.Model;
using Logic.Game.Model;
using LuaInterface;

namespace Logic.Formation.Model
{
	public class FormationTeamInfo  
	{
		public int formationId;
		public FormationTeamType type;
		public SortedDictionary<FormationPosition, uint> teamPosDictionary = new SortedDictionary<FormationPosition, uint>();

		private LuaTable _formationTeamInfoLuaTable;
		private LuaTable FormationTeamInfoLuaTable 
		{
			get
			{
				if(_formationTeamInfoLuaTable == null)
				{
					LuaTable formationModel = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","formation_model")[0];
					_formationTeamInfoLuaTable = (LuaTable)formationModel.GetLuaFunction("GetFormationTeam").Call((int)type)[0];
				}
				return _formationTeamInfoLuaTable;
			}
		}
		public FormationInfo formationInfo
		{
			get
			{
				if(FormationProxy.instance.FormationInfoDictionary.ContainsKey(formationId))
				{
					return FormationProxy.instance.FormationInfoDictionary[formationId];
				}
				return null;
			}
		}

		public void ClearTeam()
		{
			teamPosDictionary.Clear();
			FormationTeamInfoLuaTable.GetLuaFunction("ClearTeam").Call(FormationTeamInfoLuaTable);
		}
				
		public void SetFormationId(int id)
		{
			formationId = id;
			FormationTeamInfoLuaTable.GetLuaFunction("SetFormationId").Call(FormationTeamInfoLuaTable,id);
		}
		public FormationTeamInfo(FormationTeamType type, int formationId,SortedDictionary<FormationPosition, uint> teamPosDictionary)
		{
			this.formationId = formationId;
			this.type = type;
			this.teamPosDictionary = teamPosDictionary;
		}

		public bool IsHeroInFormation (uint heroInstanceID)
		{
			return teamPosDictionary.ContainsValue(heroInstanceID);
		}

		public HeroInfo GetHeroAt (FormationPosition formationPosition)
		{
			uint heroInstanceID = uint.MinValue;
			HeroInfo heroInfo = null;
			if (teamPosDictionary.ContainsKey(formationPosition))
			{
				heroInstanceID = teamPosDictionary.GetValue(formationPosition);
				heroInfo = HeroProxy.instance.GetHeroInfo(heroInstanceID);
			}
			return heroInfo;
		}

		public FormationPosition GetHeroFormationPosition (uint heroInstanceID)
		{
			FormationPosition formationPosition = FormationPosition.Invalid_Position;

			if (teamPosDictionary.ContainsValue(heroInstanceID))
			{
				foreach(var value in teamPosDictionary)
				{
					if(value.Value == heroInstanceID)
					{
						formationPosition = value.Key;
						break;
					}
				}
			}
			return formationPosition;
		}

		public bool RemoveHeroFromFormation (FormationPosition formationPosition)
		{
			bool result = false;
			if (IsPositionEmpty(formationPosition))
			{
				return result;
			}
			result = teamPosDictionary.Remove(formationPosition);
			FormationTeamInfoLuaTable.GetLuaFunction("RemoveHeroByFormationPosition").Call(FormationTeamInfoLuaTable,(int)formationPosition);
			return result;
		}

		public bool RemoveHeroFromFormation (uint heroInstanceID)
		{
			FormationPosition formationPosition = GetHeroFormationPosition(heroInstanceID);
			return RemoveHeroFromFormation(formationPosition);
		}

		public bool IsPositionEmpty (FormationPosition formationPosition)
		{
			return !teamPosDictionary.ContainsKey(formationPosition) ;
		}

		public void AddHeroToFormaiton (FormationPosition formationPosition, uint heroInstanceID)
		{
			FormationPosition oldFormationPosition = GetHeroFormationPosition(heroInstanceID);//新英雄旧位置
			teamPosDictionary.Remove(oldFormationPosition);
			
			if (!IsPositionEmpty(formationPosition))//新位置有英雄
			{
				uint newPositionOldCharacterInstanceID = teamPosDictionary.GetValue(formationPosition);
				if (oldFormationPosition != FormationPosition.Invalid_Position)//新英雄已在队伍中
				{
					teamPosDictionary[oldFormationPosition] = newPositionOldCharacterInstanceID;
					teamPosDictionary[formationPosition] = heroInstanceID;
				}
				else
				{
					teamPosDictionary[formationPosition] = heroInstanceID;
				}
			}
			else if (teamPosDictionary.Count < 5)
			{
				teamPosDictionary.Add(formationPosition, heroInstanceID);
			}
			FormationTeamInfoLuaTable.GetLuaFunction("AddHeroToFormaiton").Call(FormationTeamInfoLuaTable,(int)formationPosition,(int)heroInstanceID);
		}

		public bool CanAddToFormationPosition (FormationPosition formationPosition, uint addCharacterInstanceID,bool playerCanLeaveTeam = false)
		{
			if(!formationInfo.formationData.GetPosEnalbe(formationPosition))
				return false;

			FormationTeamInfoLuaTable.GetLuaFunction("CanAddToFormationPosition").Call(FormationTeamInfoLuaTable,(int)formationPosition,addCharacterInstanceID,playerCanLeaveTeam);

			uint oldId = teamPosDictionary.GetValue(formationPosition);
			bool can = true;
			bool isPosEmpty = IsPositionEmpty(formationPosition);
			bool playerFlag = playerCanLeaveTeam ? true : !GameProxy.instance.IsPlayer(oldId);//

			if (IsHeroInFormation(addCharacterInstanceID))//新英雄在阵中
			{
				if (!isPosEmpty && oldId == addCharacterInstanceID)
				{
					can = false;
				}
			}
			else
			{
				HeroInfo heroInfo = HeroProxy.instance.GetHeroInfo(addCharacterInstanceID);
				bool hasSame = GameProxy.instance.IsPlayer(addCharacterInstanceID) ? false : HasSameHeroInFormation(heroInfo.heroData.id);
				if (hasSame)//有同类型英雄
				{
					if (!isPosEmpty)
					{

						if (playerFlag && HeroProxy.instance.GetHeroInfo(oldId).heroData.id == heroInfo.heroData.id)
						{
							can = true;
						}
						else
						{
							can = false;
						}
					}
					else
					{
						can = false;
					}
				}
				else
				{
					if (!isPosEmpty  && !playerFlag)
					{
						can = false;
					}
					else
					{
						if (!HasEmptyFormationPosition())
						{
							if (!isPosEmpty)
							{
								can = true;
							}
							else
							{
								can = false;
							}
						}
						else
						{
							can = true;
						}
					}
				}
			}
			return can;
		}

		public bool HasSameHeroInFormation (int heroDataID)
		{
			foreach(var value in teamPosDictionary)
			{
				if (!GameProxy.instance.IsPlayer(value.Value))
				{
					HeroInfo heroInfo = HeroProxy.instance.GetHeroInfo(value.Value);
					if (heroInfo.heroData.id == heroDataID)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool HasEmptyFormationPosition ()
		{
			return teamPosDictionary.Count < 5;
		}

		public FormationPosition GetPlayerPosition ()
		{
			foreach(var value in teamPosDictionary)
			{
				if(GameProxy.instance.IsPlayer(value.Value))
				{
					return value.Key;
				}
			}
			return  FormationPosition.Invalid_Position;
		}

		public void TransferPlayer (uint newPlayerInstanceID)
		{
			FormationTeamInfoLuaTable.GetLuaFunction("TransferPlayer").Call(FormationTeamInfoLuaTable,newPlayerInstanceID);

			foreach(var value in teamPosDictionary)
			{
				if(GameProxy.instance.IsPlayer(value.Value))
				{
					teamPosDictionary[value.Key] = newPlayerInstanceID;
					break;
				}
			}
		}

		public int Power
		{
			get
			{
				int combatCapability = 0;
//				foreach(var value in teamPosDictionary)
//				{
//					if(GameProxy.instance.IsPlayer(value.Value))
//					{
//						combatCapability += GameProxy.instance.PlayerInfo.Power;
//					}else
//					{
//						combatCapability += HeroProxy.instance.GetHeroInfo(value.Value).Power;
//					}
//				}
//				FormationInfo info = formationInfo;
//				if(info != null)
//					combatCapability = combatCapability * (info.Power+1);
//				return (int)combatCapability;
				combatCapability =  FormationTeamInfoLuaTable.GetLuaFunction("Power").Call(FormationTeamInfoLuaTable)[0].ToString().ToInt32();
				return combatCapability;
			}
		}
	}
}

