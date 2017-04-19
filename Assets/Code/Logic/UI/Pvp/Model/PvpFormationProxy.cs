using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Enums;
using Logic.Protocol.Model;
using Logic.Hero.Model;
using Logic.Game.Model;
using Logic.Hero;
using Logic.UI.ManageHeroes.Model;
using Logic.Formation.Model;

namespace Logic.UI.Pvp.Model
{
	public class PvpFormationProxy : SingletonMono<PvpFormationProxy> 
	{
		public FormationTeamInfo FormationTeamInfo
		{
			get
			{
				//return FormationProxy.instance.GetFormationTeam(FormationTeamType.pvpTeam);
				return ManageHeroesProxy.instance.CurrentFormationTeamInfo;
			}
		}
		public SortedDictionary<FormationPosition, uint> FormationsDictionary
		{
			get
			{
				return FormationTeamInfo.teamPosDictionary;
			}
		}
		public List<HeroInfo> allHeroInfoList = new List<HeroInfo>();
		void Awake()
		{
			instance = this;
		}

		public bool IsPositionEmpty (FormationPosition formationPosition)
		{
//			bool isPositionEmpty = true;
//			isPositionEmpty = !FormationsDictionary.ContainsKey(formationPosition);
//			return isPositionEmpty;
			return FormationTeamInfo.IsPositionEmpty(formationPosition);
		}

		public uint GetCharacterInstanceIDAt (FormationPosition formationPosition)
		{
			uint characterInstanceID = 0;
			FormationsDictionary.TryGetValue(formationPosition, out characterInstanceID);
			return characterInstanceID;
		}
		public bool IsHeroInFormation (uint heroInstanceID)
		{
			//return FormationsDictionary.ContainsValue(heroInstanceID);
			return FormationTeamInfo.IsHeroInFormation(heroInstanceID);
			
		}
		
		public int GetPower()
		{
			return FormationTeamInfo.Power;
		}
		
		public HeroInfo GetHeroAt (FormationPosition formationPosition)
		{
//			uint heroInstanceID = uint.MinValue;
//			HeroInfo heroInfo = null;
//			if (FormationsDictionary.ContainsKey(formationPosition))
//			{
//				heroInstanceID = FormationsDictionary.GetValue(formationPosition);
//				heroInfo = HeroProxy.instance.GetHeroInfo(heroInstanceID);
//			}
//			return heroInfo;
			return FormationTeamInfo.GetHeroAt(formationPosition);
		}
		public FormationPosition GetHeroCurrentFormationPosition (uint heroInstanceID)
		{
//			FormationPosition formationPosition = FormationPosition.Invalid_Position;
//			if (FormationsDictionary.ContainsValue(heroInstanceID))
//			{
//				List<FormationPosition> keys = FormationsDictionary.GetKeys();
//				int keysCount = keys.Count;
//				for (int keyIndex = 0; keyIndex < keysCount; keyIndex++)
//				{
//					FormationPosition key = keys[keyIndex];
//					if (FormationsDictionary[key] == heroInstanceID)
//					{
//						formationPosition = key;
//					}
//				}
//			}
//			return formationPosition;
			return FormationTeamInfo.GetHeroFormationPosition(heroInstanceID);
		}

		public void AddHeroToFormaiton (FormationPosition formationPosition, uint heroInstanceID)
		{
//			FormationPosition oldFormationPosition = GetHeroCurrentFormationPosition(heroInstanceID);
//			
//			if (GameProxy.instance.IsPlayer(heroInstanceID))
//			{
//				FormationsDictionary.Remove(oldFormationPosition);
//			}
//			else
//			{
//				RemoveHeroFromFormation(heroInstanceID);
//			}
//			
//			if (!IsPositionEmpty(formationPosition))
//			{
//				uint newPositionOldCharacterInstanceID = GetCharacterInstanceIDAt(formationPosition);
//				if (oldFormationPosition != FormationPosition.Invalid_Position)
//				{
//					if (GameProxy.instance.IsPlayer(newPositionOldCharacterInstanceID))
//					{
//						FormationsDictionary.Remove(formationPosition);
//					}
//					else
//					{
//						RemoveHeroFromFormationAt(formationPosition);
//					}
//					FormationsDictionary.Add(oldFormationPosition, newPositionOldCharacterInstanceID);
//					FormationsDictionary.Add(formationPosition, heroInstanceID);
//				}
//				else
//				{
//					if (!GameProxy.instance.IsPlayer(newPositionOldCharacterInstanceID))
//					{
//						RemoveHeroFromFormationAt(formationPosition);
//						FormationsDictionary.Add(formationPosition, heroInstanceID);
//					}
//				}
//			}
//			else
//			{
//				if (FormationsDictionary.Count < 5)
//				{
//					FormationsDictionary.Add(formationPosition, heroInstanceID);
//				}
//			}
			FormationTeamInfo.AddHeroToFormaiton(formationPosition,  heroInstanceID);
		}
		public bool RemoveHeroFromFormationAt (FormationPosition formationPosition)
		{
//			bool result = false;
//			if (IsPositionEmpty(formationPosition))
//			{
//				return result;
//			}
//			
//			if (!GameProxy.instance.IsPlayer(GetCharacterInstanceIDAt(formationPosition)))
//			{
//				result = FormationsDictionary.Remove(formationPosition);
//			}
//
//			return result;
			if (!GameProxy.instance.IsPlayer(GetCharacterInstanceIDAt(formationPosition)))
			{
				return FormationTeamInfo.RemoveHeroFromFormation(formationPosition);
			}
			return false;
		}
		public bool RemoveHeroFromFormation (uint heroInstanceID)
		{
//			FormationPosition formationPosition = GetHeroCurrentFormationPosition(heroInstanceID);
//			return RemoveHeroFromFormationAt(formationPosition);
			return FormationTeamInfo.RemoveHeroFromFormation(heroInstanceID);
		}
		
//		public int GetFormationHeroCount ()
//		{
//			return FormationsDictionary.Count;
//		}
		
//		public bool HasEmptyFormationPosition ()
//		{
//			return GetFormationHeroCount() < 5;
//		}
//		public bool HasSameHeroInFormation (int heroDataID)
//		{
//			List<uint> characterInstanceIDs = new List<uint>(FormationsDictionary.Values);
//			int characterInstanceIDCount = characterInstanceIDs.Count;
//			for (int i = 0; i < characterInstanceIDCount; i++)
//			{
//				if (!GameProxy.instance.IsPlayer(characterInstanceIDs[i]))
//				{
//					HeroInfo heroInfo = HeroProxy.instance.GetHeroInfo(characterInstanceIDs[i]);
//					if (heroInfo.heroData.id == heroDataID)
//					{
//						return true;
//					}
//				}
//			}
//			return false;
//		}

		public bool CanAddToFormationPosition (FormationPosition formationPosition, uint addCharacterInstanceID)
		{
//			bool can = true;
//			if (IsHeroInFormation(addCharacterInstanceID))
//			{
//				if (!IsPositionEmpty(formationPosition)
//				    && GetCharacterInstanceIDAt(formationPosition) == addCharacterInstanceID)
//				{
//					can = false;
//				}
//			}
//			else
//			{
//				HeroInfo heroInfo = HeroProxy.instance.GetHeroInfo(addCharacterInstanceID);
//				if (HasSameHeroInFormation(heroInfo.heroData.id))
//				{
//					if (!IsPositionEmpty(formationPosition))
//					{
//						uint characterInstanceID = GetCharacterInstanceIDAt(formationPosition);
//						if (!GameProxy.instance.IsPlayer(characterInstanceID)
//						    && HeroProxy.instance.GetHeroInfo(characterInstanceID).heroData.id == heroInfo.heroData.id)
//						{
//							can = true;
//						}
//						else
//						{
//							can = false;
//						}
//					}
//					else
//					{
//						can = false;
//					}
//				}
//				else
//				{
//					if (!IsPositionEmpty(formationPosition) && GameProxy.instance.IsPlayer((GetCharacterInstanceIDAt(formationPosition))))
//					{
//						can = false;
//					}
//					else
//					{
//						if (!HasEmptyFormationPosition())
//						{
//							if (!IsPositionEmpty(formationPosition))
//							{
//								can = true;
//							}
//							else
//							{
//								can = false;
//							}
//						}
//						else
//						{
//							can = true;
//						}
//					}
//				}
//			}
//			return can;
			return FormationTeamInfo.CanAddToFormationPosition(formationPosition, addCharacterInstanceID);
		}
		public List<HeroInfo> GetAllHeroInfoBySortType(HeroSortType type)
		{
			List<HeroInfo> heroInfoList = HeroProxy.instance.GetAllHeroInfoList();

			if (type == HeroSortType.QualityDesc)
			{
				heroInfoList.Sort(HeroUtil.CompareHeroByQualityDesc);
			}
			else if (type == HeroSortType.QualityAsc)
			{
				heroInfoList.Sort(HeroUtil.CompareHeroByQualityAsc);
			}
			allHeroInfoList = heroInfoList;
			return allHeroInfoList;
		}
		public void TransferPlayer (uint newPlayerInstanceID)
		{
//			FormationPosition playerPosition = GetPlayerPosition();
//			FormationsDictionary[playerPosition] = newPlayerInstanceID;
			FormationTeamInfo.TransferPlayer(newPlayerInstanceID);

		}
//		public FormationPosition GetPlayerPosition ()
//		{
//			List<FormationPosition> keys = FormationsDictionary.GetKeys();
//			int keyCount = keys.Count;
//			for (int i = 0; i < keyCount; i++)
//			{
//				FormationPosition formationPosition = keys[i];
//				if (GameProxy.instance.IsPlayer(FormationsDictionary[formationPosition]))
//				{
//					return formationPosition;
//				}
//			}
//			return FormationPosition.Invalid_Position;
//		}
		#region update by server
//		public void UpdateTeamInfosByProtocol (List<TeamInfo> teams)
//		{
//
//			FormationsDictionary.Clear();
//			int teamCount = teams.Count;
//
//			TeamInfo teamInfo = teams[0];
//			
//			int positionInfoCount = teamInfo.posInfos.Count;
//			for (int j = 0; j < positionInfoCount; j++)
//			{
//				PositionInfo positionInfo = teamInfo.posInfos[j];
//				FormationsDictionary.Add((FormationPosition)positionInfo.posIndex, (uint)positionInfo.heroId);
//			}
//
//		}
		#endregion
	}
}

