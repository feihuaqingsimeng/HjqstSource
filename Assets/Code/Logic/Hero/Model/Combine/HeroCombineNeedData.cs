using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;

namespace Logic.Hero.Model.Combine{
	public class HeroCombineNeedData {
		
		private static Dictionary<int, HeroCombineNeedData> _heroCombineNeedDataDictionary;

		public static Dictionary<int,HeroCombineNeedData> GetHeroCombineNeedDatas(){
			
			if(_heroCombineNeedDataDictionary == null)
				_heroCombineNeedDataDictionary = CSVUtil.Parse<int,HeroCombineNeedData>("config/csv/compose_need","star");
			return _heroCombineNeedDataDictionary;
		}
		public static Dictionary<int,HeroCombineNeedData> HeroCombineNeedDataDictionary{
			get{
				if(_heroCombineNeedDataDictionary == null)
					GetHeroCombineNeedDatas();
				return _heroCombineNeedDataDictionary;
			}
		}

		public static HeroCombineNeedData GetHeroCombineNeedDataByStar(int star){
			if(HeroCombineNeedDataDictionary.ContainsKey(star)){
				return HeroCombineNeedDataDictionary[star];
			}
			return null;
		}
		public int GetHeroCombineRate(int num){

			if(num == 2)
				return rate1;
			else if(num == 3)
				return rate2;
			else if(num == 4)
				return rate3;
			return 0;

		}

		[CSVElement("star")]
		public int star;

		[CSVElement("gold")]
		public int gold;

		[CSVElement("level")]
		public int level;

		[CSVElement("rate1")]
		public int rate1;

		[CSVElement("rate2")]
		public int rate2;

		[CSVElement("rate3")]
		public int rate3;

	}
}

