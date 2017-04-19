using UnityEngine;
using System.Collections.Generic;
using Common.Util;
using LuaInterface;
using Logic.Enums;
using System.Collections;
using Logic.Game.Model;

namespace Logic.Equipment.Model
{
	public class EquipmentData
	{
		private static Dictionary<int, EquipmentData> _equipmentDataDictionary;

        public static Dictionary<int, EquipmentData> GetEquipmentDatas()
        {
            if (_equipmentDataDictionary == null)
            {
                _equipmentDataDictionary = CSVUtil.Parse<int, EquipmentData>("config/csv/equip", "id");
            }
            return _equipmentDataDictionary;
        }

		public static Dictionary<int, EquipmentData> EquipmentDataDictionary
		{
			get
			{
				if (_equipmentDataDictionary == null)
				{
                    GetEquipmentDatas();
				}
				return _equipmentDataDictionary;
			}
		}

		public static EquipmentData GetEquipmentDataByID (int equipmentDataID)
		{
			EquipmentData equipmentData = null;
			if (EquipmentDataDictionary.ContainsKey(equipmentDataID))
			{
				equipmentData = EquipmentDataDictionary[equipmentDataID];
			}
			return equipmentData;
		}

		public EquipmentAttribute GetBaseAttr()
		{
			EquipmentAttrData data = EquipmentAttrData.GetFirstDataByAttrId(base_attr_id);
			if(data == null)
			{
				return null;
			}
			return new EquipmentAttribute((EquipmentAttributeType)data.attr_type,data.value_min);
		}

		[CSVElement("id")]
		public int id;

		[CSVElement("name")]
		public string name;

		[CSVElement("icon")]
		public string icon;

		public EquipmentType equipmentType = EquipmentType.None;
		[CSVElement("type")]
		public int type
		{
			set
			{
				equipmentType = (EquipmentType)value;
			}
		}

		public RoleType equipmentRoleType = RoleType.Invalid;
		[CSVElement("career")]
		public int career
		{
			set
			{
				equipmentRoleType = (RoleType)value;
			}
		}
		[CSVElement("quality")]
		public int quality ;
		public int useLv = 0;

		public GameResData unloadCost ;
		[CSVElement("unload_diamond")]
		public string unload_diamondStr
		{
			set{
				if (value .Equals("0"))
				{
					unloadCost = new GameResData(BaseResType.Diamond,0,0,0);
				}else
				{
					unloadCost = new GameResData(value);
				}

			}
		}
		
		public int star = 1;

		[CSVElement("description")]
		public string description;

		[CSVElement("price")]
		public int price;


		[CSVElement("special_hero")]
		public int special_hero;

		[CSVElement("base_attr_id")]
		public int base_attr_id;

	}
}
