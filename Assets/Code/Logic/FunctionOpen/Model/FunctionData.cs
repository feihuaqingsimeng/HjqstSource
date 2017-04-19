using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;
using Logic.Enums;

namespace Logic.FunctionOpen.Model{
	public class FunctionData 
	{
		
		private static Dictionary<int,FunctionData> _FunctionDataDictionary;
		
		public static Dictionary<int,FunctionData> GetFunctionDatas(){
			if(_FunctionDataDictionary == null){
				_FunctionDataDictionary = CSVUtil.Parse<int,FunctionData>("config/csv/newfunction","id");
			}
			return _FunctionDataDictionary;
		}
		public static Dictionary<int,FunctionData> FunctionDataDictionary{
			get{
				if(_FunctionDataDictionary == null)
					GetFunctionDatas();
				return _FunctionDataDictionary;
			}
		}
		
		public static List<FunctionData> GetAllFunctionDataList(){
			
			return new List<FunctionData>(FunctionDataDictionary.Values);
			
		}
		public static FunctionData GetFunctionDataByID(int id){
			FunctionData data = null;
			if(FunctionDataDictionary.ContainsKey(id) && FunctionDataDictionary[id] != null){
				data = FunctionDataDictionary[id];
			}
			return data;
		}
		public static FunctionData GetFunctionDataByID(FunctionOpenType type){
			int id = (int)type;
			return GetFunctionDataByID(id);
		}
		[CSVElement("id")]
		public int id;
		
		[CSVElement("name")]
		public string name;
		
		[CSVElement("player_level")]
		public int player_level;
		
		
		[CSVElement("dungeon_pass")]
		public int dungeon_pass;
		
		[CSVElement("task_get")]
		public int task_get;
		
		[CSVElement("task_finish")]
		public int task_finish;
		
		[CSVElement("notice")]
		public int notice;
		
		[CSVElement("vip")]
		public int vip;
		[CSVElement("is_show_light")]
		public bool is_show_light;//是否显示光圈

		//0 不处理 1 是否显示隐藏，2显隐and是否有动画（2为主界面专用）
		[CSVElement("is_show_animation")]
		public int show_animation_status;
		[CSVElement("show_main_position")]
		public string show_main_position;

		[CSVElement("show_sheet_position")]
		public string show_sheet_position;

		[CSVElement("show_new_pic")]

		public string show_new_pic;

		[CSVElement("show_new_des")]
		public string show_new_des;

	}
}
