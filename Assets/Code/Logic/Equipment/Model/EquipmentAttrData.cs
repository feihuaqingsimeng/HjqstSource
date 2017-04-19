using UnityEngine;
using System.Collections.Generic;
using Common.Util;
using Logic.Enums;

namespace Logic.Equipment.Model
{
	public class EquipmentAttrData
	{
		private static Dictionary<int, EquipmentAttrData> _equipmentAttrDataDictionary;

		public static Dictionary<int, EquipmentAttrData> GetEquipmentAttrDatas()
        {
            if (_equipmentAttrDataDictionary == null)
            {
				_equipmentAttrDataDictionary = CSVUtil.Parse<int, EquipmentAttrData>("config/csv/equip_attr", "id");
            }
            return _equipmentAttrDataDictionary;
        }

		public static Dictionary<int, EquipmentAttrData> EquipmentAttrDataDictionary
		{
			get
			{
				if (_equipmentAttrDataDictionary == null)
				{
                    GetEquipmentAttrDatas();
				}
				return _equipmentAttrDataDictionary;
			}
		}

		public static EquipmentAttrData GetEquipmentAttrDataByID (int equipmentDataID)
		{
			EquipmentAttrData equipmentData = null;
			if (EquipmentAttrDataDictionary.ContainsKey(equipmentDataID))
			{
				equipmentData = EquipmentAttrDataDictionary[equipmentDataID];
			}
			return equipmentData;
		}
		public static EquipmentAttrData GetFirstDataByAttrId(int attrid)
		{
			foreach(var value in EquipmentAttrDataDictionary) 
			{
				if(value.Value.attr_id == attrid)
				{
					return value.Value;
				}
			}
			return null;
		}
		[CSVElement("id")]
		public int id;

		[CSVElement("attr_id")]
		public int attr_id;

		[CSVElement("describe")]
		public string describe;

		[CSVElement("attr_type")]
		public int attr_type;

		[CSVElement("value_min")]
		public int value_min;

		[CSVElement("value_max")]
		public int value_max;

		[CSVElement("weight")]
		public int weight;

		[CSVElement("attr_comat_const")]
		public int attr_comat_const;

	}
}
