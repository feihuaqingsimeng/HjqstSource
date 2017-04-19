using UnityEngine;
using System.Collections.Generic;
using Logic.Protocol.Model;
using LuaInterface;
using System.Collections;
using Logic.Enums;

namespace Logic.Equipment.Model
{
	public class EquipmentProxy : SingletonMono<EquipmentProxy>
	{
		private static LuaTable _equipModelLuaTable;
		public static LuaTable EquipModelLuaTable
		{
			get
			{
				if (_equipModelLuaTable == null)
					_equipModelLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "equip_model")[0];
				return _equipModelLuaTable;
			}
		}

		private Dictionary<int, EquipmentInfo> _equipmentsDictionary = new Dictionary<int, EquipmentInfo>();
		private Dictionary<int, EquipmentInfo> _newEquipmentMarksDictionary = new Dictionary<int, EquipmentInfo>();

		public delegate void OnEquipmentInfoUpdateDelegate (int equipmentInstanceID);
		public OnEquipmentInfoUpdateDelegate onEquipmentInfoUpdateDelegate;

		public delegate void OnEquipmentInfoListUpdateDelegate ();
		public OnEquipmentInfoListUpdateDelegate onEquipmentInfoListUpdateDelegate;

		public delegate void OnNewEquipmentMarksChangedDelegate ();
		public OnNewEquipmentMarksChangedDelegate onNewEquipmentMarksChangedDelegate;

		//装备培养
		public System.Action UpdateTrainingDelegate;
		
		//是否重铸了
		public bool isRecast
		{
			get
			{
				return EquipModelLuaTable["isRecast"].ToString().ToBoolean();
			}

		}
		public List<EquipmentAttribute> equipRecastAttrList
		{
			get
			{
				List<EquipmentAttribute> attrList = new List<EquipmentAttribute>();
				LuaTable attrTable = (LuaTable)EquipModelLuaTable["equipRecastAttrList"];
				if(attrTable != null)
				{

					foreach(DictionaryEntry kvp in attrTable.ToDictTable())
					{
						LuaTable table = kvp.Value as LuaTable;
						if(table != null)
						{
							EquipmentAttributeType type = (EquipmentAttributeType)table["type"].ToString().ToInt32();
							attrList.Add(new EquipmentAttribute(type, table["value"].ToString().ToFloat()));
						}
					}
				}
				return attrList;
			}
		}


		void Awake ()
		{
			instance = this;
		}

		void Start ()
		{
			Observers.Facade.Instance.RegisterObserver("OnEquipmentInfoUpdate", OnEquipmentInfoUpdate);
			Observers.Facade.Instance.RegisterObserver("OnEquipmentInfoListUpdate", OnEquipmentInfoListUpdate);
			Observers.Facade.Instance.RegisterObserver("OnNewEquipmentMarksChanged", OnNewEquipmentMarksChanged);
			Observers.Facade.Instance.RegisterObserver("UpdateEquipTrainingByProtocol", UpdateTrainingByProtocol);
		}

		void OnDestroy ()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver("OnEquipmentInfoUpdate", OnEquipmentInfoUpdate);
				Observers.Facade.Instance.RemoveObserver("OnEquipmentInfoListUpdate", OnEquipmentInfoListUpdate);
				Observers.Facade.Instance.RemoveObserver("OnNewEquipmentMarksChanged", OnNewEquipmentMarksChanged);
				Observers.Facade.Instance.RemoveObserver("UpdateEquipTrainingByProtocol", UpdateTrainingByProtocol);
			}
		}

		public Dictionary<int, EquipmentInfo> GetAllEquipmentInfoDictioary ()
		{
//			return _equipmentsDictionary;

			Dictionary<int, EquipmentInfo> allEquipmentInfoDictionary = null;
			LuaFunction getAllEquipmentInfoDictionaryLuaFunction = EquipModelLuaTable.GetLuaFunction("GetAllEquipmentInfoDictionary");
			LuaTable allEquipmentInfoLuaTable = (LuaTable)getAllEquipmentInfoDictionaryLuaFunction.Call(null)[0];
			if (allEquipmentInfoLuaTable != null)
			{
				allEquipmentInfoDictionary = new Dictionary<int, EquipmentInfo>();
				foreach (DictionaryEntry kvp in allEquipmentInfoLuaTable.ToDictTable())
				{
					int equipmentInstanceID = kvp.Key.ToString().ToInt32();
					LuaTable equipmentInfoLuaTable = (LuaTable)kvp.Value;
					EquipmentInfo equipmentInfo = new EquipmentInfo(equipmentInfoLuaTable);
					allEquipmentInfoDictionary.Add(equipmentInstanceID, equipmentInfo);
				}
			}
			return allEquipmentInfoDictionary;
		}

		public List<EquipmentInfo> GetAllEquipmentInfoList ()
		{
//			return new List<EquipmentInfo>(_equipmentsDictionary.Values);

			return GetAllEquipmentInfoDictioary().GetValues();
		}

		public int GetAllEquipmentCount ()
		{
//			return _equipmentsDictionary.Count;

			return EquipModelLuaTable.GetLuaFunction("GetAllEquipmentCount").Call(null)[0].ToString().ToInt32();
		}

		public List<EquipmentInfo> GetFreeEquipmentInfoList ()
		{
//			List<EquipmentInfo> allEquipmentInfoList = GetAllEquipmentInfoList();
//			List<EquipmentInfo> allFreeEquipmentInfoList = new List<EquipmentInfo>();
//			int allEquipmentCount = allEquipmentInfoList.Count;
//			for (int i = 0; i < allEquipmentCount; i++)
//			{
//				if (!allEquipmentInfoList[i].isInUse)
//				{
//					allFreeEquipmentInfoList.Add(allEquipmentInfoList[i]);
//				}
//			}
//			return allFreeEquipmentInfoList;

			List<EquipmentInfo> freeEquipmentInfoList = new List<EquipmentInfo>();
			LuaTable equipModelLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "equip_model")[0];
			LuaFunction getFreeEquipmentInfoListLuaFunction = equipModelLuaTable.GetLuaFunction("GetFreeEquipmentInfoList");
			LuaTable freeEquipmentInfoListLuaTable = (LuaTable)getFreeEquipmentInfoListLuaFunction.Call(null)[0];
			foreach (DictionaryEntry kvp in freeEquipmentInfoListLuaTable.ToDictTable())
			{
				EquipmentInfo equipmentInfo = new EquipmentInfo((LuaTable)kvp.Value);
				freeEquipmentInfoList.Add(equipmentInfo);
			}
			return freeEquipmentInfoList;
		}

		public EquipmentInfo GetEquipmentInfoByInstanceID (int instanceID)
		{
//			EquipmentInfo equipmentInfo = null;
//			if (_equipmentsDictionary.ContainsKey(instanceID))
//			{
//				equipmentInfo = _equipmentsDictionary[instanceID];
//			}
//			return equipmentInfo;

			EquipmentInfo equipmentInfo = null;
			object[] objects = LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "equip_model");
			LuaTable equipModelTabel = (LuaTable)objects[0];
			LuaFunction luaFunction = equipModelTabel.GetLuaFunction("GetEquipmentInfoByInstanceID");
			object[] objects2 = luaFunction.Call(instanceID);
			LuaTable equipmentInfoLuaTable = (LuaTable)objects2[0];

			if (equipmentInfoLuaTable != null)
			{
				equipmentInfo = new EquipmentInfo(equipmentInfoLuaTable);
			}
			return equipmentInfo;
		}

//		public bool AddEquipmentInfo (EquipmentInfo equipmentInfo, bool isNew)
//		{
//			if (_equipmentsDictionary.ContainsKey(equipmentInfo.instanceID))
//			{
//				return false;
//			}
//			_equipmentsDictionary.Add(equipmentInfo.instanceID, equipmentInfo);
//			if (isNew)
//			{
//				_newEquipmentMarksDictionary.Add(equipmentInfo.instanceID, equipmentInfo);
//			}
//			return true;
//		}

//		public bool RemoveEquipmentInfo (int instanceID)
//		{
//			return _equipmentsDictionary.Remove(instanceID);
//		}

//		public void UpdateEquipmentInfo (Equip equip)
//		{
//			GetEquipmentInfoByInstanceID(equip.id).Update(equip);
//			if (onEquipmentInfoUpdateDelegate != null)
//			{
//				onEquipmentInfoUpdateDelegate(equip.id);
//			}
//		}

		public bool HasNewEquipment ()
		{
//			return _newEquipmentMarksDictionary.Count > 0;

			return EquipModelLuaTable.GetLuaFunction("HasNewEquipment").Call(null)[0].ToString().ToBoolean();
		}

		public bool IsNewEquipment (int instanceID)
		{
//			return _newEquipmentMarksDictionary.ContainsKey(instanceID);

			return EquipModelLuaTable.GetLuaFunction("IsNewEquipment").Call(instanceID)[0].ToString().ToBoolean();
		}

		public void SetEquipmentAsChecked (int equipmentInstanceID)
		{
//			if (_newEquipmentMarksDictionary.ContainsKey(equipmentInstanceID))
//			{
//				_newEquipmentMarksDictionary.Remove(equipmentInstanceID);
//				if (onNewEquipmentMarksChangedDelegate != null)
//				{
//					onNewEquipmentMarksChangedDelegate();
//				}
//			}

			EquipModelLuaTable.GetLuaFunction("SetEquipmentAsChecked").Call(equipmentInstanceID);
		}

		public void ClearNewEquipmentMarks ()
		{
//			_newEquipmentMarksDictionary.Clear();
//			if (onNewEquipmentMarksChangedDelegate != null)
//			{
//				onNewEquipmentMarksChangedDelegate();
//			}

			EquipModelLuaTable.GetLuaFunction("ClearNewEquipmentMarks").Call();
		}

		public bool OnEquipmentInfoUpdate (Observers.Interfaces.INotification note)
		{
			int equipmentInstanceID = note.Body.ToString().ToInt32();
			if (onEquipmentInfoUpdateDelegate != null)
				onEquipmentInfoUpdateDelegate(equipmentInstanceID);
			return true;
		}

		public bool OnEquipmentInfoListUpdate (Observers.Interfaces.INotification note)
		{
			if (onEquipmentInfoListUpdateDelegate != null)
				onEquipmentInfoListUpdateDelegate();
			return true;
		}

		public bool OnNewEquipmentMarksChanged (Observers.Interfaces.INotification note)
		{
			if (onNewEquipmentMarksChangedDelegate != null)
				onNewEquipmentMarksChangedDelegate();
			return true;
		}
		public bool UpdateTrainingByProtocol (Observers.Interfaces.INotification note)
		{
			if (UpdateTrainingDelegate != null)
				UpdateTrainingDelegate();
			return true;
		}
	}
}