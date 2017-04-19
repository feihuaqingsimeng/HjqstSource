using UnityEngine;
using System.Collections.Generic;
using Common.Util;
using Common.ResMgr;
using Logic.Game.Model;
using Logic.Role.Model;

namespace Logic.CommonGameData
{
	public class BreakthroughData
	{
		private static Dictionary<int, BreakthroughData> _breakthroughDataDictionary;

        public static Dictionary<int, BreakthroughData> GetBreakthroughDatas()
        {
            if (_breakthroughDataDictionary == null)
            {
                _breakthroughDataDictionary = CSVUtil.Parse<int, BreakthroughData>(ResPath.GetConfigFilePath("breakthrough"), "id");
            }
            return _breakthroughDataDictionary;
        }

		public static Dictionary<int, BreakthroughData> BreakthroughDataDictionary
		{
			get
			{
				if (_breakthroughDataDictionary == null)
                {
                    GetBreakthroughDatas();
				}
				return _breakthroughDataDictionary;
			}
		}

		public static BreakthroughData GetRoleCurrentBreakthroughData (RoleInfo roleInfo)
		{
			BreakthroughData roleCurrentBreakthroughData = null;
			List<BreakthroughData> breakthroughDataList = BreakthroughDataDictionary.GetValues();
			for (int i = 0, count = breakthroughDataList.Count; i < count; i++)
			{
				if (breakthroughDataList[i].quality == (int)roleInfo.heroData.roleQuality
				    && breakthroughDataList[i].layer == roleInfo.breakthroughLevel)
				{
					roleCurrentBreakthroughData = breakthroughDataList[i];
				}
			}
			return roleCurrentBreakthroughData;
		}

		[CSVElement("id")]
		public int id;

		[CSVElement("layer")]
		public int layer;

		[CSVElement("quality")]
		public int quality;

		[CSVElement("lock_lev")]
		public int levelMin;

		[CSVElement("lock_release")]
		public int levelMax;

		public GameResData costGoldGameResData = null;
		[CSVElement("cost_1")]
		public string cost1
		{
			set
			{
				if (value.ToInt32() != 0)
				{
					costGoldGameResData = new GameResData(value);
				}
			}
		}

		public Logic.Game.Model.GameResData costItemGameResData = null;
		[CSVElement("cost_2")]
		public string cost2
		{
			set
			{
				if (value.ToInt32() != 0)
				{
					costItemGameResData = new GameResData(value);
				}
			}
		}
	}
}
