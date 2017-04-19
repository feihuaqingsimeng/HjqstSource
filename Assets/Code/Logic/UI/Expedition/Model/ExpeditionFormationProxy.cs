using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Enums;
using Logic.Hero.Model;
using Logic.Game.Model;
using Logic.Protocol.Model;
using Logic.UI.Expedition.Controller;
using Logic.Role;
using Logic.Formation.Model;
using LuaInterface;

namespace Logic.UI.Expedition.Model
{
	public class ExpeditionFormationProxy : SingletonMono<ExpeditionFormationProxy> 
	{

		public FormationTeamInfo FormationTeamInfo
		{
			get
			{
				return  FormationProxy.instance.GetFormationTeam(FormationTeamType.expeditionTeam);
			}
		}

		private Dictionary<uint ,ExpeditionHeroInfo> _expeditionRoleDictionary = new Dictionary<uint, ExpeditionHeroInfo>();

		//private SortedDictionary<FormationPosition,uint> _formationsDictionary = new SortedDictionary<FormationPosition, uint>();
		public SortedDictionary<FormationPosition, uint> FormationsDictionary
		{
			get
			{
				return FormationTeamInfo.teamPosDictionary;
			}
		}

		public List<ExpeditionHeroInfo> currentExpeditionRoleList;
		public int ExpeditionPower
		{
			get
			{
//				int power = 0;
//				List<uint> formationRoleList = FormationsDictionary.GetValues();
//				for(int i = 0,count = formationRoleList.Count;i<count;i++)
//				{
//					ExpeditionHeroInfo info = GetExpeditionHeroInfo(formationRoleList[i]);
//					power += info.roleInfo.Power;
//				}
//				return power;
				return FormationTeamInfo.Power;
			}
		}

		void Awake()
		{
			instance = this;
		}

		public void ResetFormation(List<ExpeditionHeroProto> heros)
		{
//			FormationsDictionary.Clear();
//			List<PositionInfo> posList = teams.posInfos;
//			int count = posList.Count;
//			for(int i = 0;i<count;i++)
//			{
//				PositionInfo posInfo = posList[i];
//				FormationsDictionary[(FormationPosition)posInfo.posIndex] = (uint)posInfo.heroId;
//			}
			_expeditionRoleDictionary.Clear();
			int count = heros.Count;
			ExpeditionHeroProto heroProto;
			for(int i = 0;i<count;i++)
			{
				heroProto = heros[i];
				if(GameProxy.instance.IsPlayer((uint)heroProto.heroId))
				{
					ExpeditionHeroInfo expeditionInfo = new ExpeditionHeroInfo(GameProxy.instance.PlayerInfo,heroProto.hpPercent/10000.0f);
					_expeditionRoleDictionary.Add((uint)heroProto.heroId,expeditionInfo);
				}
				HeroInfo info = HeroProxy.instance.GetHeroInfo((uint)heroProto.heroId);
				if(info != null)
				{
					ExpeditionHeroInfo expeditionInfo = new ExpeditionHeroInfo(info,heroProto.hpPercent/10000.0f);
					_expeditionRoleDictionary.Add((uint)heroProto.heroId, expeditionInfo);
				}
			}
		}
		public void UpdateExpeditionRole(Dictionary<int,int> roleInfoDic)
		{
			List<int> keys = roleInfoDic.GetKeys();
			
			int count = keys.Count;
			int id ;
			for(int i = 0;i<count;i++)
			{
				id = keys[i];
				ExpeditionHeroInfo info = GetExpeditionHeroInfo((uint)id);
				if(info != null)
				{
					info.hpRate = roleInfoDic[id]/10000.0f;
				}
			}
		}
		public void UpdateExpeditionRoleFromLua()
		{
			_expeditionRoleDictionary.Clear();
			LuaTable expeditionModelLua = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","expedition_model")[0];
			LuaTable roleDic = (LuaTable) expeditionModelLua["expeditionRoleDictionary"];
			LuaTable roleDicData = (LuaTable)roleDic["data"];
			int instanceId = 0;
			float hpRate = 0;
			foreach(DictionaryEntry kvp in roleDicData.ToDictTable())
			{
				LuaTable v = (LuaTable)kvp.Value;
				instanceId = v["instanceID"].ToString().ToInt32();
				hpRate = v["hpRate"].ToString().ToFloat();
				ExpeditionHeroInfo info = GetExpeditionHeroInfo((uint)instanceId);
				if(info != null)
				{
					info.hpRate = hpRate;
                }
			}
		}
		
		public void deleteExpeditionRole(List<int> idList)
		{
			for(int i = 0,count = idList.Count;i<count;i++)
			{
				uint id = (uint)idList[i];
				if(_expeditionRoleDictionary.ContainsKey(id))
				{
					_expeditionRoleDictionary.Remove(id);
				}
			}
		}
		//死亡下阵
		public void CheckDeadHeroAtFormation()
		{
			LuaTable formationModelLua = (LuaTable) LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","formation_model")[0];
			LuaTable formationTeamInfoLua = (LuaTable)formationModelLua.GetLuaFunction("GetFormationTeam").Call((int)FormationTeamType.expeditionTeam)[0];


			List<FormationPosition> posList = FormationsDictionary.GetKeys();
			int count = posList.Count;
			bool hasDead = false;
			for(int i = 0;i<count;i++)
			{
				FormationPosition key = posList[i];
				ExpeditionHeroInfo info = GetExpeditionHeroInfo(FormationsDictionary[key]);
				if(info.hpRate <= 0)
				{
					hasDead = true;
					FormationsDictionary.Remove(key);
					formationTeamInfoLua.GetLuaFunction("RemoveHeroByFormationPosition").Call(formationTeamInfoLua,(int)key);
				}
			}
			if(hasDead)
			{
				ExpeditionController.instance.CLIENT2LOBBY_Expedition_Formation_Change_REQ();
			}
		}
		public List<ExpeditionHeroInfo> GetEnableExpeditionHeroList()
		{

			Dictionary<int, ExpeditionHeroInfo> expeditionHeroDic = new Dictionary<int,ExpeditionHeroInfo>();
			uint intanceId = GameProxy.instance.PlayerInfo.instanceID;
			expeditionHeroDic.Add(GameProxy.instance.PlayerInfo.modelDataId, GetExpeditionHeroInfo(intanceId));

			List<HeroInfo> heroList = HeroProxy.instance.GetAllHeroInfoList();
			HeroInfo heroInfo;
			for(int i = 0,count = heroList.Count;i<count;i++)
			{
				heroInfo = heroList[i];
				if(expeditionHeroDic.ContainsKey(heroInfo.modelDataId))
				{
					if(expeditionHeroDic[heroInfo.modelDataId].roleInfo.Power<heroInfo.Power)
					{
						expeditionHeroDic[heroInfo.modelDataId] = GetExpeditionHeroInfo(heroInfo.instanceID);
					}
				}else
				{
					expeditionHeroDic.Add(heroInfo.modelDataId,GetExpeditionHeroInfo(heroInfo.instanceID));
				}
			}
			List<ExpeditionHeroInfo> expeditionInfoList = expeditionHeroDic.GetValues();
//			if (type == HeroSortType.QualityDesc)
//			{
//				expeditionInfoList.Sort(CompareHeroByQualityDesc);
//			}
//			else if (type == HeroSortType.QualityAsc)
//			{
//				expeditionInfoList.Sort(CompareHeroByQualityAsc);
//			}

			currentExpeditionRoleList = expeditionInfoList;
			return expeditionInfoList;
		}
		public static int CompareHeroByQualityDesc(ExpeditionHeroInfo info1,ExpeditionHeroInfo info2)
		{
			int compare =  -RoleUtil.CompareRoleDataPower(info1.roleInfo,info2.roleInfo);
			if(compare!= 0)
				return compare;
			return RoleUtil.CompareRoleByQualityDesc(info1.roleInfo,info2.roleInfo);
		}
		public static int CompareHeroByQualityAsc(ExpeditionHeroInfo info1,ExpeditionHeroInfo info2)
		{
			return  RoleUtil.CompareRoleDataPower(info1.roleInfo,info2.roleInfo);
		}

		public ExpeditionHeroInfo GetExpeditionHeroInfo(uint instanceId)
		{
			if(_expeditionRoleDictionary.ContainsKey(instanceId))
			{
				return _expeditionRoleDictionary[instanceId];
			}
			ExpeditionHeroInfo expeditionInfo ;
			if(GameProxy.instance.IsPlayer(instanceId))
			{
				expeditionInfo = new ExpeditionHeroInfo(GameProxy.instance.PlayerInfo,1);
				_expeditionRoleDictionary.Add(instanceId,expeditionInfo);
				return expeditionInfo;
			}
			HeroInfo info = HeroProxy.instance.GetHeroInfo(instanceId);
			if(info != null)
			{
				expeditionInfo = new ExpeditionHeroInfo(info,1);
				_expeditionRoleDictionary.Add(instanceId, expeditionInfo);
				return expeditionInfo;
			}
			return null;
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
		//	return FormationsDictionary.ContainsValue(heroInstanceID);
			return FormationTeamInfo.IsHeroInFormation(heroInstanceID);
		}
//		public bool HasSameHeroInFormation (int heroDataID)
//		{
//			List<uint> characterInstanceIDs = new List<uint>(FormationsDictionary.Values);
//			int characterInstanceIDCount = characterInstanceIDs.Count;
//			for (int i = 0; i < characterInstanceIDCount; i++)
//			{
////				if (!GameProxy.instance.IsPlayer(characterInstanceIDs[i]))
////				{
//					ExpeditionHeroInfo heroInfo = GetExpeditionHeroInfo(characterInstanceIDs[i]);
//					if (heroInfo.roleInfo.modelDataId == heroDataID)
//					{
//						return true;
//					}
////				}
//			}
//			return false;
//		}
//		public bool HasEmptyFormationPosition ()
//		{
//			return GetFormationHeroCount() < 5;
//		}
//		public int GetFormationHeroCount ()
//		{
//			return FormationsDictionary.Count;
//		}
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
//						break;
//					}
//				}
//			}
//			return formationPosition;
			return FormationTeamInfo.GetHeroFormationPosition(heroInstanceID);
		}
		public bool RemoveHeroFromFormation (uint heroInstanceID)
		{
//			FormationPosition formationPosition = GetHeroCurrentFormationPosition(heroInstanceID);
//
//			return RemoveHeroFromFormationAt(formationPosition);
			return FormationTeamInfo.RemoveHeroFromFormation(heroInstanceID);
		}
		public bool RemoveHeroFromFormationAt (FormationPosition formationPosition)
		{
//			bool result = false;
//			if (IsPositionEmpty(formationPosition))
//			{
//				return result;
//			}
//			List<uint> tempList = FormationsDictionary.GetValues();
//			result = FormationsDictionary.Remove(formationPosition);
//			List<uint> tempList2 = FormationsDictionary.GetValues();
//			return result;
			return FormationTeamInfo.RemoveHeroFromFormation(formationPosition);
		}
		public void AddHeroToFormaiton (FormationPosition formationPosition, uint heroInstanceID)
		{
//			FormationPosition oldFormationPosition = GetHeroCurrentFormationPosition(heroInstanceID);
//
////			if (GameProxy.instance.IsPlayer(heroInstanceID))
////			{
////				FormationsDictionary.Remove(oldFormationPosition);
////			}
////			else
////			{
//				RemoveHeroFromFormation(heroInstanceID);
////			}
//			
//			if (!IsPositionEmpty(formationPosition))
//			{
//				uint newPositionOldCharacterInstanceID = GetCharacterInstanceIDAt(formationPosition);
//				if (oldFormationPosition != FormationPosition.Invalid_Position)
//				{
////					if (GameProxy.instance.IsPlayer(newPositionOldCharacterInstanceID))
////					{
////						FormationsDictionary.Remove(formationPosition);
////					}
////					else
////					{
//						RemoveHeroFromFormationAt(formationPosition);
////					}
//					FormationsDictionary.Add(oldFormationPosition, newPositionOldCharacterInstanceID);
//					FormationsDictionary.Add(formationPosition, heroInstanceID);
//				}
//				else
//				{
////					if (!GameProxy.instance.IsPlayer(newPositionOldCharacterInstanceID))
////					{
//						RemoveHeroFromFormationAt(formationPosition);
//						FormationsDictionary.Add(formationPosition, heroInstanceID);
////					}
//				}
//			}
//			else
//			{
//				if (FormationsDictionary.Count < 5)
//				{
//					FormationsDictionary.Add(formationPosition, heroInstanceID);
//				}
//			}
			FormationTeamInfo.AddHeroToFormaiton(formationPosition, heroInstanceID);
		}
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
//				ExpeditionHeroInfo expeditionInfo = GetExpeditionHeroInfo(addCharacterInstanceID);
////				HeroInfo heroInfo = HeroProxy.instance.GetHeroInfo(addCharacterInstanceID);
//				if (HasSameHeroInFormation(expeditionInfo.roleInfo.modelDataId))
//				{
//					if (!IsPositionEmpty(formationPosition))
//					{
//						uint characterInstanceID = GetCharacterInstanceIDAt(formationPosition);
//						ExpeditionHeroInfo oldInfo = GetExpeditionHeroInfo(characterInstanceID);
//						//if (!GameProxy.instance.IsPlayer(characterInstanceID)&&
//						if( oldInfo.roleInfo.modelDataId == expeditionInfo.roleInfo.modelDataId)
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
////					if (!IsPositionEmpty(formationPosition) && GameProxy.instance.IsPlayer((GetCharacterInstanceIDAt(formationPosition))))
////					{
////						can = false;
////					}
////					else
////					{
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
////					}
//				}
//			}
//			return can;
			return FormationTeamInfo.CanAddToFormationPosition(formationPosition, addCharacterInstanceID);
		}
		public void TransferPlayer (uint newPlayerInstanceID)
		{
//			FormationPosition playerPosition = GetPlayerPosition();
//			if(playerPosition != FormationPosition.Invalid_Position)
//				FormationsDictionary[playerPosition] = newPlayerInstanceID;
			FormationTeamInfo.TransferPlayer(newPlayerInstanceID);
		}
		public FormationPosition GetPlayerPosition ()
		{
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
			return FormationTeamInfo.GetPlayerPosition();
		}
	}
}

