using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;

namespace Logic.Equipment.Model
{
	public class EquipmentStrengthenNeedData  
	{
		
		private static Dictionary<int, EquipmentStrengthenNeedData> _equipmentStrengthenNeedDataDictionary;
		public static Dictionary<int, EquipmentStrengthenNeedData> GetEquipmentStrengthenNeedDatas()
		{
			if (_equipmentStrengthenNeedDataDictionary == null)
			{
				_equipmentStrengthenNeedDataDictionary = CSVUtil.Parse<int, EquipmentStrengthenNeedData>("config/csv/equip_aggr_need", "aggr_lv");
			}
			return _equipmentStrengthenNeedDataDictionary;
		}
		
		public static Dictionary<int, EquipmentStrengthenNeedData> EquipmentStrengthenNeedDataDictionary
		{
			get
			{
				if (_equipmentStrengthenNeedDataDictionary == null)
				{
					GetEquipmentStrengthenNeedDatas();
				}
				return _equipmentStrengthenNeedDataDictionary;
			}
		}
		public static List<EquipmentStrengthenNeedData > getStrengthenNeedDataList()
		{
			return new List<EquipmentStrengthenNeedData>(EquipmentStrengthenNeedDataDictionary.Values);
		}
		public static EquipmentStrengthenNeedData GetStrengthenNeedDataByLv(int lv)
		{
			EquipmentStrengthenNeedData data = null;
			if(EquipmentStrengthenNeedDataDictionary.ContainsKey(lv))
			{
				data = EquipmentStrengthenNeedDataDictionary[lv];
			}
			return data;
		}
		/// <summary>
		/// Gets the level total exp.
		/// </summary>
		public static int GetStrengthenTotalExp(int level){
			List<EquipmentStrengthenNeedData >  dataList = getStrengthenNeedDataList();
			EquipmentStrengthenNeedData data;
			int needTotal = 0;
			for(int i = 0,count = dataList.Count;i<count;i++)
			{
				data = dataList[i];
				if(level<= data.aggr_lv)
					break;
				needTotal += data.exp_need;

			}
			return needTotal;
		}
		public static EquipmentStrengthenNeedData GetStrengthenNeedDataByExp(int expTotal)
		{
			List<EquipmentStrengthenNeedData >  dataList = getStrengthenNeedDataList();
			EquipmentStrengthenNeedData data = null;
			int needTotal = 0;
			for(int i = 0,count = dataList.Count;i<count;i++)
			{
				data = dataList[i];
				needTotal += data.exp_need;
				if(expTotal<needTotal){
					return data;
				}
			}
			return null;
		}
		public static EquipmentStrengthenNeedData LastNeedData()
		{
			List<EquipmentStrengthenNeedData> dataList = getStrengthenNeedDataList();
			EquipmentStrengthenNeedData data = dataList[dataList.Count-1];
			return data;
		}
		public static bool IsMaxLevel(int level)
		{
			int lv = LastNeedData().aggr_lv;
			if(level>lv)
				return true;
			return false;
			
		}


		public int GetGoldNeedByStar(int star)
		{
			int gold = 0;
			switch(star){
			case 1:
				gold = gold_need_1;
				break;
			case 2:
				gold = gold_need_2;
				break;
			case 3:
				gold = gold_need_3;
				break;
			case 4:
				gold = gold_need_4;
				break;
			case 5:
				gold = gold_need_5;
				break;
			case 6:
				gold = gold_need_6;
				break;
			}
			return gold;
		}
		public float GetBaseAttrAdd(){
			return base_attr_percent/100.0f;
		}
		[CSVElement("aggr_lv")]
		public int aggr_lv;

		[CSVElement("exp_need")]
		public int exp_need;			

		[CSVElement("gold_need_1")]
		public int gold_need_1;		

		[CSVElement("gold_need_2")]
		public int gold_need_2;		

		[CSVElement("gold_need_3")]
		public int gold_need_3;		

		[CSVElement("gold_need_4")]
		public int gold_need_4;		

		[CSVElement("gold_need_5")]
		public int gold_need_5;		

		[CSVElement("gold_need_6")]
		public int gold_need_6;	


		[CSVElement("base_attr_percent")]
		public int base_attr_percent;	
	}
}

