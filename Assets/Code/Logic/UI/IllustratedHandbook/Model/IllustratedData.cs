using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;
using Logic.Game.Model;
using Logic.Enums;
using LuaInterface;

namespace Logic.UI.IllustratedHandbook.Model
{
	public class IllustratedData 
	{
		
		private static Dictionary<int, IllustratedData> _illustratedDataDictionary;
		public static Dictionary<int, IllustratedData> GetIllustratedDataDicionary ()
		{
			if (_illustratedDataDictionary == null)
			{
				_illustratedDataDictionary = CSVUtil.Parse<int, IllustratedData>("config/csv/atlas", "id");
//				_illustratedDataDictionary = new Dictionary<int, IllustratedData>();
//				LuaTable luaTable = LuaScriptMgr.Instance.GetLuaTable("gamemanager");
//				object[] objs = LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetData","illustration_data");
//				LuaTable datas = objs[0] as LuaTable;
//				foreach(DictionaryEntry kvp in datas.ToDictTable())
//				{
//					LuaTable data = kvp.Value as LuaTable;
//					if(data == null)
//						continue;
//					IllustratedData illusData = new IllustratedData();
//					illusData.id = data["id"].ToString().ToInt32();
//					illusData.type = data["type"].ToString().ToInt32();
//					illusData.type_name = data["type_name"].ToString();
//					illusData.sheet = data["sheet"].ToString().ToInt32();
//					illusData.sheet_name = data["sheet_name"].ToString();
//					illusData.hero_id = data["hero_id"].ToString().ToInt32();
//					illusData.hero_star = data["hero_star"].ToString().ToInt32();
//					_illustratedDataDictionary.Add(illusData.id,illusData);
//				}

			}
			return _illustratedDataDictionary;
		}
		public static Dictionary<int, IllustratedData> IllustratedDataDictionary
		{
			get
			{
				return GetIllustratedDataDicionary();
			}
		}
		
		public static IllustratedData GetIllustratedDataByID(int id)
		{
			if(IllustratedDataDictionary!= null && IllustratedDataDictionary.ContainsKey(id))
			{
				return IllustratedDataDictionary[id];
			}
			return null;
		}

		public int type; // 已弃用
		public string type_name; //已弃用

		[CSVElement("id")]
		public int id;

		[CSVElement("atlas_type")]
		public int atlas_type;
		

		[CSVElement("sheet")]
		public int sheet;

		[CSVElement("sheet_name")]
		public string sheet_name;

		public GameResData resData = new GameResData();

		[CSVElement("hero_id")]
		public int hero_id
		{
			set
			{
				resData.type = BaseResType.Hero;
				resData.id = value;
			}
		}
	}
}

