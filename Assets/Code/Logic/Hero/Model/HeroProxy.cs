using UnityEngine;
using System.Collections.Generic;
using Logic.UI.ManageHeroes.Model;
using Logic.Equipment.Model;
using Logic.UI.Pvp.Model;
using Logic.UI.Expedition.Model;
using Logic.Role.Model;
using LuaInterface;
using System.Collections;

namespace Logic.Hero.Model
{
    public class HeroProxy : SingletonMono<HeroProxy>
    {
		private LuaTable _heroModelLuaTable;
		public LuaTable HeroModelLuaTable
		{
			get
			{
				if (_heroModelLuaTable == null)
					_heroModelLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "hero_model")[0];
				return _heroModelLuaTable;
			}
		}

        private Dictionary<uint, HeroInfo> _heroInfoDictionary = new Dictionary<uint, HeroInfo>();
		private Dictionary<uint, HeroInfo> _newHeroMarkDictionary = new Dictionary<uint, HeroInfo>();

		public delegate void OnHeroInfoListUpdateDelegate();
		public OnHeroInfoListUpdateDelegate onHeroInfoListUpdateDelegate;

		public delegate void OnHeroInfoUpdateDelegate(uint heroInstanceID);
		public OnHeroInfoUpdateDelegate onHeroInfoUpdateDelegate;

		public delegate void OnHeroBreakthroughSuccessDelegate();
		public OnHeroBreakthroughSuccessDelegate onHeroBreakthroughSuccessDelegate;

		public delegate void OnHeroStrengthenSuccessDelegate (uint heroInstanceID);
		public OnHeroStrengthenSuccessDelegate onHeroStrengthenSuccessDelegate;

		public delegate void OnNewHeroMarksChangedDelegate ();
		public OnNewHeroMarksChangedDelegate onNewHeroMarksChangedDelegate;

        void Awake()
        {
            instance = this;
        }

		void Start ()
		{
			Observers.Facade.Instance.RegisterObserver("OnUpdateHeroInfoList", OnUpdateHeroInfoList);
			Observers.Facade.Instance.RegisterObserver("OnUpdateHero", OnUpdateHero);
			Observers.Facade.Instance.RegisterObserver("OnNewHeroMarksChanged", OnNewHeroMarksChanged);
			Observers.Facade.Instance.RegisterObserver("OnBreakthroughSuccess", OnBreakthroughSuccess);
			Observers.Facade.Instance.RegisterObserver("OnStrengthenSuccess", OnStrengthenSuccess);
		}

		void OnDestroy ()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver("OnUpdateHeroInfoList", OnUpdateHeroInfoList);
				Observers.Facade.Instance.RemoveObserver("OnUpdateHero", OnUpdateHero);
				Observers.Facade.Instance.RemoveObserver("OnNewHeroMarksChanged", OnNewHeroMarksChanged);
				Observers.Facade.Instance.RemoveObserver("OnBreakthroughSuccess", OnBreakthroughSuccess);
				Observers.Facade.Instance.RemoveObserver("OnStrengthenSuccess", OnStrengthenSuccess);
			}
		}

        public List<HeroInfo> GetAllHeroInfoList()
        {
//            return new List<HeroInfo>(_heroInfoDictionary.Values);

			/* from lua method */
			List<HeroInfo> allHeroInfoList = new List<HeroInfo>();
			LuaFunction getAllHeroInfoListLuaFunction = HeroModelLuaTable.GetLuaFunction("GetAllHeroInfoList");
			LuaTable allHeroInfoLuaTable = (LuaTable)getAllHeroInfoListLuaFunction.Call(null)[0];
			if (allHeroInfoLuaTable != null)
			{
				foreach (DictionaryEntry kvp in allHeroInfoLuaTable.ToDictTable())
				{
                    //int heroInstanceID = kvp.Key.ToString().ToInt32();
					LuaTable heroInfoLuaTable = (LuaTable)kvp.Value;
					HeroInfo heroInfo = new HeroInfo(heroInfoLuaTable);
					allHeroInfoList.Add(heroInfo);
				}
			}
			return allHeroInfoList;
			/* from lua method */
        }

		public int GetAllHeroCount ()
		{
//			return _heroInfoDictionary.Count;

			/* from lua method */
			int allHeroInfoCount = HeroModelLuaTable.GetLuaFunction("GetAllHeroCount").Call(null)[0].ToString().ToInt32();
			return allHeroInfoCount;
			/* from lua method */
		}

		public List<HeroInfo> GetAllInBagHeroInfoList ()
		{
			List<HeroInfo> allInBagHeroInfoList = new List<HeroInfo>();

			List<HeroInfo> allHeroInfoList = GetAllHeroInfoList();
			int allHeroCount = allHeroInfoList.Count;
			for (int i = 0; i < allHeroCount; i++)
			{
				if (!ManageHeroesProxy.instance.IsHeroInFormation(allHeroInfoList[i].instanceID))
				{
					allInBagHeroInfoList.Add(allHeroInfoList[i]);
				}

			}
			return allInBagHeroInfoList;
		}
		/// <summary>
		/// 获得不在阵型中（pve、pvp、远征）的英雄列表
		/// </summary>
		public List<HeroInfo> GetNotInAnyTeamHeroInfoList ()
		{
			List<HeroInfo> allInBagHeroInfoList = new List<HeroInfo>();
			
			List<HeroInfo> allHeroInfoList = GetAllHeroInfoList();
			int allHeroCount = allHeroInfoList.Count;
			uint id ;
			for (int i = 0; i < allHeroCount; i++)
			{
				id = allHeroInfoList[i].instanceID;
				if (!ManageHeroesProxy.instance.IsHeroInAnyFormation(id) && !PvpFormationProxy.instance.IsHeroInFormation(id)&& !ExpeditionFormationProxy.instance.IsHeroInFormation(id))
				{
					allInBagHeroInfoList.Add(allHeroInfoList[i]);
				}

			}
			return allInBagHeroInfoList;
		}

        public HeroInfo GetHeroInfo(uint heroInstanceID)
        {
//            HeroInfo heroInfo = null;
//            _heroInfoDictionary.TryGetValue(heroInstanceID, out heroInfo);
//            return heroInfo;

			/* from lua method */
			HeroInfo heroInfo = null;
			LuaTable heroInfoLuaTable = (LuaTable)HeroModelLuaTable.GetLuaFunction("GetHeroInfo").Call(heroInstanceID)[0];
			if (heroInfoLuaTable != null)
				heroInfo = new HeroInfo(heroInfoLuaTable);
			return heroInfo;
			/* from lua method */
        }

		public List<HeroInfo> GetHeroInfosByHeroDataID (int heroDataID)
		{
//			List<HeroInfo> heroInfoList = GetAllHeroInfoList();
//			List<HeroInfo> result = new List<HeroInfo>();
//			int heroInfoCount = heroInfoList.Count;
//			for (int i = 0; i < heroInfoCount; i++)
//			{
//				if (heroInfoList[i].heroData.id == heroDataID)
//				{
//					result.Add(heroInfoList[i]);
//				}
//			}
//			return result;

			/* from lua method */
			List<HeroInfo> heroInfoList = new List<HeroInfo>();
			LuaTable heroInfosLuaTable = (LuaTable)HeroModelLuaTable.GetLuaFunction("GetHeroInfosByHeroDataID").Call(heroDataID)[0];
			if (heroInfosLuaTable != null)
			{
				foreach (DictionaryEntry kvp in heroInfosLuaTable.ToDictTable())
				{
					LuaTable heroInfoLuaTable = (LuaTable)kvp.Value;
					HeroInfo heroInfo = new HeroInfo(heroInfoLuaTable);
					heroInfoList.Add(heroInfo);
				}
			}
			return heroInfoList;
			/* from lua method */
		}

		public List<HeroInfo> GetHeroInfosLevelMoreThan (int level)
		{
//			List<HeroInfo> heroInfoList = GetAllHeroInfoList();
//			List<HeroInfo> result = new List<HeroInfo>();
//			int heroInfoCount = heroInfoList.Count;
//			for (int i = 0; i < heroInfoCount; i++)
//			{
//				if (heroInfoList[i].level >= level)
//				{
//					result.Add(heroInfoList[i]);
//				}
//			}
//			return result;

			/* from lua method */
			List<HeroInfo> heroInfoList = new List<HeroInfo>();
			LuaTable heroInfosLuaTable = (LuaTable)HeroModelLuaTable.GetLuaFunction("GetHeroInfosLevelMoreThan").Call(level)[0];
			if (heroInfosLuaTable != null)
			{
				foreach (DictionaryEntry kvp in heroInfosLuaTable.ToDictTable())
				{
					LuaTable heroInfoLuaTable = (LuaTable)kvp.Value;
					HeroInfo heroInfo = new HeroInfo(heroInfoLuaTable);
					heroInfoList.Add(heroInfo);
				}
			}
			return heroInfoList;
			/* from lua method */
		}

		public List<HeroInfo> GetHeroInfosStarMoreThan (int star)
		{
//			List<HeroInfo> heroInfoList = GetAllHeroInfoList();
//			List<HeroInfo> result = new List<HeroInfo>();
//			int heroInfoCount = heroInfoList.Count;
//			for (int i = 0; i < heroInfoCount; i++)
//			{
//				if (heroInfoList[i].advanceLevel >= star)
//				{
//					result.Add(heroInfoList[i]);
//				}
//			}
//			return result;

			/* from lua method */
			List<HeroInfo> heroInfoList = new List<HeroInfo>();
			LuaTable heroInfosLuaTable = (LuaTable)HeroModelLuaTable.GetLuaFunction("GetHeroInfosStarMoreThan").Call(3)[0];
			if (heroInfosLuaTable != null)
			{
				foreach (DictionaryEntry kvp in heroInfosLuaTable.ToDictTable())
				{
					LuaTable heroInfoLuaTable = (LuaTable)kvp.Value;
					HeroInfo heroInfo = new HeroInfo(heroInfoLuaTable);
					heroInfoList.Add(heroInfo);
				}
			}
			return heroInfoList;
			/* from lua method */
		}

		public int GetBreakthroughMaterialStarRequirements (RoleInfo breakthroughRoleInfo)
		{
			int breakthroughMaterialStarRequirements = Mathf.Min((int)(breakthroughRoleInfo.heroData.starMin + breakthroughRoleInfo.breakthroughLevel - 1), (int)breakthroughRoleInfo.heroData.starMax);
			return breakthroughMaterialStarRequirements;
		}

		public List<RoleInfo> GetUsableBreakthroughMaterialHeroList (HeroInfo breakthroughHeroInfo)
		{
            List<RoleInfo> usableBreakthroughMaterialHeroList = new List<RoleInfo>();
			List<HeroInfo> allInBagHeroInfoList = GetNotInAnyTeamHeroInfoList();
			int allInBagHeroInfoCount = allInBagHeroInfoList.Count;
			for (int i = 0; i < allInBagHeroInfoCount; i++)
			{
				HeroInfo heroInfo = allInBagHeroInfoList[i];
				if (heroInfo.instanceID != breakthroughHeroInfo.instanceID
				    && heroInfo.heroData.id == breakthroughHeroInfo.heroData.id
				    && heroInfo.advanceLevel >= GetBreakthroughMaterialStarRequirements(breakthroughHeroInfo))
				{
					usableBreakthroughMaterialHeroList.Add(heroInfo);
				}
			}
			return usableBreakthroughMaterialHeroList;
		}

		/*
		public void UpdateHeroes (List<HeroInfo> heroes)
		{
			int heroCount = heroes.Count;
			for (int i = heroCount; i < heroCount; i++)
			{
				OnUpdateHero(heroes[i].instanceID);
			}
		}
		*/

		/*
		public bool AddHero (HeroInfo heroInfo, bool isNew)
		{
			if (_heroInfoDictionary.ContainsKey(heroInfo.instanceID))
			{
				Debugger.Log("The instanceID of new hero is already exist in the current hero info dictionary.");
				return false;
			}
			_heroInfoDictionary.Add(heroInfo.instanceID, heroInfo);
			if (isNew)
			{
				_newHeroMarkDictionary.Add(heroInfo.instanceID, heroInfo);
			}
			return true;
		}
		*/

		/*
		public bool RemoveHero (uint heroInstanceID)
		{
			if (_heroInfoDictionary.ContainsKey(heroInstanceID))
			{
				_heroInfoDictionary.Remove(heroInstanceID);
				return true;
			}
			return false;
		}
		*/

		/*
		public bool RemoveHeroes (List<int> heroInstancesIDs)
		{
			int removeHeroIDCount = heroInstancesIDs.Count;
			for (int i = 0; i < removeHeroIDCount; i++)
			{
				RemoveHero((uint)heroInstancesIDs[i]);
			}
			return true;
		}
		*/

		public bool HasNewHero ()
		{
//			return _newHeroMarkDictionary.Count > 0;

			/* call lua method */

			return HeroModelLuaTable.GetLuaFunction("HasNewHero").Call(null)[0].ToString().ToBoolean();
			/* call lua method */
		}

		public bool IsNewHero (uint heroInstanceID)
		{
//			return _newHeroMarkDictionary.ContainsKey(heroInstanceID);

			/* call lua method */
			return HeroModelLuaTable.GetLuaFunction("IsNewHero").Call(heroInstanceID)[0].ToString().ToBoolean();
			/* call lua method */
		}

		/*
		public void SetHeroAsChecked (uint heroInstanceID)
		{
			if (_newHeroMarkDictionary.ContainsKey(heroInstanceID))
			{
				_newHeroMarkDictionary.Remove(heroInstanceID);
				if (onNewHeroMarksChangedDelegate != null)
					onNewHeroMarksChangedDelegate();
			}
		}
		*/

		/*
		public void ClearNewHeroMarks ()
		{
			_newHeroMarkDictionary.Clear();
			if (onNewHeroMarksChangedDelegate != null)
				onNewHeroMarksChangedDelegate();
		}
		*/

		public bool OnUpdateHeroInfoList (Observers.Interfaces.INotification note)
		{
			if (onHeroInfoListUpdateDelegate != null)
				onHeroInfoListUpdateDelegate();
			return true;
		}
		
		public bool OnUpdateHero (Observers.Interfaces.INotification note)
		{
			uint heroInstanceID = note.Body.ToString().ToUInt32();
			if (onHeroInfoUpdateDelegate != null)
				onHeroInfoUpdateDelegate(heroInstanceID);
			return true;
		}

		public bool OnNewHeroMarksChanged (Observers.Interfaces.INotification note)
		{
			if (onNewHeroMarksChangedDelegate != null)
				onNewHeroMarksChangedDelegate();
			Logic.UI.RedPoint.Model.RedPointProxy.instance.Refresh();
			return true;
		}

//		public void OnHeroBreakthroughSuccess ()
//		{
//			if (onHeroBreakthroughSuccessDelegate != null)
//				onHeroBreakthroughSuccessDelegate();
//		}
//
//		public void OnHeroStrengthenSuccess (uint heroInstanceID)
//		{
//			if (onHeroStrengthenSuccessDelegate != null)
//				onHeroStrengthenSuccessDelegate(heroInstanceID);
//		}

		public bool OnBreakthroughSuccess (Observers.Interfaces.INotification note)
		{
			if (onHeroBreakthroughSuccessDelegate != null)
				onHeroBreakthroughSuccessDelegate();
			return true;
		}

		public bool OnStrengthenSuccess (Observers.Interfaces.INotification note)
		{
			uint heroInstanceID = note.Body.ToString().ToUInt32();
			if (onHeroStrengthenSuccessDelegate != null)
				onHeroStrengthenSuccessDelegate(heroInstanceID);
			return true;
		}

		/*
		public void PutOnEquipment (uint heroInstanceID, int equipmentInstanceID)
		{
			HeroInfo heroInfo = HeroProxy.instance.GetHeroInfo(heroInstanceID);
			EquipmentInfo equipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(equipmentInstanceID);
			equipmentInfo.ownerId = (int)heroInstanceID;
			LuaTable equipmentInfoLuaTable = (LuaTable)EquipmentProxy.EquipModelLuaTable.GetLuaFunction("GetEquipmentInfoByInstanceID").Call(equipmentInstanceID)[0];
			if (equipmentInfo.equipmentData.equipmentType == Logic.Enums.EquipmentType.PhysicalWeapon
			    || equipmentInfo.equipmentData.equipmentType == Logic.Enums.EquipmentType.MagicalWeapon)
			{
				EquipmentInfo oldEquipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(heroInfo.weaponID);
					
				LuaTable oldEquipmentInfoLuaTable = (LuaTable)EquipmentProxy.EquipModelLuaTable.GetLuaFunction("GetEquipmentInfoByInstanceID").Call(heroInfo.weaponID)[0];
				if (oldEquipmentInfo != null)
				{
					oldEquipmentInfo.ownerId = 0;
					oldEquipmentInfoLuaTable.GetLuaFunction("SetOwnerId").Call(oldEquipmentInfoLuaTable, 0);
					
				}
				heroInfo.weaponID = equipmentInstanceID;

			}
			else if (equipmentInfo.equipmentData.equipmentType == Logic.Enums.EquipmentType.Armor)
			{
				EquipmentInfo oldEquipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(heroInfo.armorID);

					
				LuaTable oldEquipmentInfoLuaTable = (LuaTable)EquipmentProxy.EquipModelLuaTable.GetLuaFunction("GetEquipmentInfoByInstanceID").Call(heroInfo.armorID)[0];
				if (oldEquipmentInfo != null)
				{
					oldEquipmentInfoLuaTable.GetLuaFunction("SetOwnerId").Call(oldEquipmentInfoLuaTable, 0);
					oldEquipmentInfo.ownerId = 0;
				}
					
				heroInfo.armorID = equipmentInstanceID;
			}
			else if (equipmentInfo.equipmentData.equipmentType == Logic.Enums.EquipmentType.Accessory)
			{
				EquipmentInfo oldEquipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(heroInfo.accessoryID);

				LuaTable oldEquipmentInfoLuaTable = (LuaTable)EquipmentProxy.EquipModelLuaTable.GetLuaFunction("GetEquipmentInfoByInstanceID").Call(heroInfo.accessoryID)[0];
				if (oldEquipmentInfo != null)
				{
					oldEquipmentInfoLuaTable.GetLuaFunction("SetOwnerId").Call(oldEquipmentInfoLuaTable, 0);
					
					oldEquipmentInfo.ownerId = 0;
				}
				heroInfo.accessoryID = equipmentInstanceID;
			}
			equipmentInfoLuaTable.GetLuaFunction("SetOwnerId").Call(equipmentInfoLuaTable, heroInstanceID);
		}
		*/

		/*
		public void PutOffEquipment (uint heroInstanceID, int equipmentInstanceID)
		{
			HeroInfo heroInfo = HeroProxy.instance.GetHeroInfo(heroInstanceID);
			EquipmentInfo equipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(equipmentInstanceID);
			equipmentInfo.ownerId = 0;
			LuaTable equipmentInfoLuaTable = (LuaTable)EquipmentProxy.EquipModelLuaTable.GetLuaFunction("GetEquipmentInfoByInstanceID").Call(equipmentInstanceID)[0];
			if (equipmentInfo != null)
			{
				if (equipmentInfo.equipmentData.equipmentType == Logic.Enums.EquipmentType.PhysicalWeapon
				    || equipmentInfo.equipmentData.equipmentType == Logic.Enums.EquipmentType.MagicalWeapon)
					heroInfo.weaponID = 0;
				else if (equipmentInfo.equipmentData.equipmentType == Logic.Enums.EquipmentType.Armor)
					heroInfo.armorID = 0;
				else if (equipmentInfo.equipmentData.equipmentType == Logic.Enums.EquipmentType.Accessory)
					heroInfo.accessoryID = 0;
				equipmentInfoLuaTable.GetLuaFunction("SetOwnerId").Call(equipmentInfoLuaTable, 0);
			}
		}
		*/

		/*
		public void UpdateHeroEquipments (int heroInstanceID, List<int> equipmentIDs)
		{
			HeroInfo heroInfo = GetHeroInfo((uint)heroInstanceID);

			int weaponID = 0;
			int armorID = 0;
			int accessoryID = 0;

			int equipmentIDsCount = equipmentIDs.Count;
			if (equipmentIDsCount == 0) //装备列表为空，表示无需更新装备
			{
				return;
			}

			if (equipmentIDs.Contains(-1))
			{
				PutOffEquipment((uint)heroInstanceID, heroInfo.weaponID);
				PutOffEquipment((uint)heroInstanceID, heroInfo.armorID);
				PutOffEquipment((uint)heroInstanceID, heroInfo.accessoryID);
				return;
			}

			for (int i = 0; i < equipmentIDsCount; i++)
			{
				EquipmentInfo equipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(equipmentIDs[i]);
				if (equipmentInfo.equipmentData.equipmentType == Logic.Enums.EquipmentType.PhysicalWeapon
				    || equipmentInfo.equipmentData.equipmentType == Logic.Enums.EquipmentType.MagicalWeapon)
				{
					weaponID = equipmentIDs[i];
				}
				else if (equipmentInfo.equipmentData.equipmentType == Logic.Enums.EquipmentType.Armor)
				{
					armorID = equipmentIDs[i];
				}
				else if (equipmentInfo.equipmentData.equipmentType == Logic.Enums.EquipmentType.Accessory)
				{
					accessoryID = equipmentIDs[i];
				}
			}
			if (heroInfo.weaponID != weaponID)
			{
				if (weaponID > 0)
				{
					PutOnEquipment((uint)heroInstanceID ,weaponID);
				}
				else
				{
					PutOffEquipment((uint)heroInstanceID, heroInfo.weaponID);
				}
			}
			if (heroInfo.armorID != armorID)
			{
				if (armorID > 0)
				{
					PutOnEquipment((uint)heroInstanceID, armorID);
				}
				else
				{
					PutOffEquipment((uint)heroInstanceID, heroInfo.armorID);
				}
			}
			if (heroInfo.accessoryID != accessoryID)
			{
				if (accessoryID > 0)
				{
					PutOnEquipment((uint)heroInstanceID, accessoryID);
				}
				else
				{
					PutOffEquipment((uint)heroInstanceID, heroInfo.accessoryID);
				}
			}
		}
		*/
    }
}