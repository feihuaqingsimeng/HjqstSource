using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;
using Logic.Enums;

namespace Logic.Item.Model
{
	public class ItemData 
	{
		private static Dictionary<int, ItemData> _itemDataDictionary;
		
		public static Dictionary<int, ItemData> GetItemDatas()
		{
			if (_itemDataDictionary == null)
			{
				_itemDataDictionary = CSVUtil.Parse<int, ItemData>("config/csv/item", "id");
			}
			return _itemDataDictionary;
		}
		
		public static Dictionary<int, ItemData> ItemDataDictionary
		{
			get
			{
				if (_itemDataDictionary == null)
				{
					GetItemDatas();
				}
				return _itemDataDictionary;
			}
		}
		public static ItemData GetItemDataByID (int itemtDataID)
		{
			ItemData equipmentData = null;
			if (ItemDataDictionary.ContainsKey(itemtDataID))
			{
				equipmentData = ItemDataDictionary[itemtDataID];
			}
			return equipmentData;
		}
		//基础资源类型
		public static ItemData GetBasicResItemByType(BaseResType type)
		{
			ItemData data;
			foreach(var value in ItemDataDictionary)
			{
				data = value.Value;
				if(data.type == (int)type)
					return data;
			}
			return null;
		}
		[CSVElement("id")]
		public int id;

		[CSVElement("type")]
		public int type;

		[CSVElement("name")]
		public string name;

		[CSVElement("icon")]
		public string icon;

		[CSVElement("des")]
		public string des;

		public ItemQuality itemQuality = ItemQuality.White;
		[CSVElement("quality")]
		public int quality
		{
			set
			{
				itemQuality = (ItemQuality)value;
			}
		}

		[CSVElement("money")]
		public int money;

		[CSVElement("prize_id")]
		public string prize_id;

		[CSVElement("jump_page")]
		public string jump_page;


		public List<RoleType> heroTypeList = new List<RoleType>();

		[CSVElement("hero_type")]
		public string hero_type
		{
			set
			{
				string[] types = value.Split(CSVUtil.SYMBOL_SEMICOLON);
				for(int i = 0,count = types.Length;i<count;i++)
				{
					int type = types[i].ToInt32();
					if( type != 0 )
						heroTypeList.Add((RoleType)(types[i].ToInt32()));
				}

			}
		}

		[CSVElement("count")]
		public int sort;	//在背包中的排序

		public int universal_id;
		[CSVElement("universal_id")]
		public string universal_idString
		{
			set
			{
				if(!string.IsNullOrEmpty(value))
					universal_id = value.ToInt32();
			}
		}
	}

}
