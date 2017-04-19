using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;

namespace Logic.Equipment.Model{
	public class EquipmentStrengthenData  {
		
		private static Dictionary<int, EquipmentStrengthenData> _equipmentStrengthenDataDictionary;
		public static Dictionary<int, EquipmentStrengthenData> GetEquipmentStrengthenDatas()
		{
			if (_equipmentStrengthenDataDictionary == null)
			{
				_equipmentStrengthenDataDictionary = CSVUtil.Parse<int, EquipmentStrengthenData>("config/csv/equip_aggr", "star");
			}
			return _equipmentStrengthenDataDictionary;
		}
		
		public static Dictionary<int, EquipmentStrengthenData> EquipmentStrengthenDataDictionary
		{
			get
			{
				if (_equipmentStrengthenDataDictionary == null)
				{
					GetEquipmentStrengthenDatas();
				}
				return _equipmentStrengthenDataDictionary;
			}
		}
		public static EquipmentStrengthenData GetStrengthenDataByStar(int star){
			EquipmentStrengthenData data = null;
			if(EquipmentStrengthenDataDictionary.ContainsKey(star)){
				data = EquipmentStrengthenDataDictionary[star];
			}
			return data;
		}

		[CSVElement("star")]
		public int star;

		[CSVElement("exp_provide")]
		public int exp_provide;			//提供的经验

		[CSVElement("off_need_type")]
		public int off_need_type;		//脱下装备需要的货币种类

		[CSVElement("off_need_num")]
		public int off_need_num;		//脱下装备需要的货币数量
	}
}

