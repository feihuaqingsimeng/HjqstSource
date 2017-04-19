using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;
using Logic.Game.Model;
using Logic.Enums;

namespace Logic.Player.Model
{
	public class PlayerSkillTalentData 
	{
		private static Dictionary<int, PlayerSkillTalentData> _skillTalentDataDictionary;
		
		public static Dictionary<int, PlayerSkillTalentData> GetSkillTalentDatas()
		{
			if (_skillTalentDataDictionary == null)
			{
				_skillTalentDataDictionary = CSVUtil.Parse<int, PlayerSkillTalentData>("config/csv/pet_talent", "id");
			}
			return _skillTalentDataDictionary;
		}
		
		public static Dictionary<int, PlayerSkillTalentData> SkillTalentDataDictionary
		{
			get
			{
				if (_skillTalentDataDictionary == null)
				{
					GetSkillTalentDatas();
				}
				return _skillTalentDataDictionary;
			}
		}
		public static PlayerSkillTalentData GetSkillTalentDataByID(int id)
		{
			if(SkillTalentDataDictionary.ContainsKey(id))
				return SkillTalentDataDictionary[id];
			return null;
		}

		public static List<PlayerSkillTalentData> GetDatasByPlayerId(int playerModelId)
		{
			List<PlayerSkillTalentData> dataList = new List<PlayerSkillTalentData>();
			foreach(var data in SkillTalentDataDictionary)
			{
				if(data.Value.pet_id == playerModelId)
				{
					dataList.Add(data.Value);
				}
			}
			return dataList;
		}
		public List<GameResData> GetCostResData(int level)
		{
			if(costResData.ContainsKey(level))
				return costResData[level];
			return null;
		}
        public string GetDesByLevel(int level)
        {
            if (level == 0)
                level = 1;
            if (des.Length < level)
                return "";
            return des[level - 1];
        }
        public string GetNameByLevel(int level) 
        {
            if (level == 0)
                level = 1;
            if (name.Length < level)
                return "";
            return name[level - 1];
        }
		private List<GameResData> ParseCostResData(string value)
		{
			List<GameResData> resList = new List<GameResData>();
			if(! string.IsNullOrEmpty(value))
			{
				string[] s = value.ToArray(CSVUtil.SYMBOL_SEMICOLON);
				for(int i = 0,count = s.Length;i<count;i++)
				{
					resList.Add(new GameResData(s[i]));
				}
			}
			return resList;
		}

		[CSVElement("id")]
		public int id;

		[CSVElement("pet_id")]
		public int pet_id;

		[CSVElement("xp_need")]
		public int exp_need;

		[CSVElement("effect")]
		public int effect;

		[CSVElement("pre_id")]
		public int pre_id;

		[CSVElement("max_lv")]
		public int max_lv;

		public PlayerSkillTalentType  groupType;
		
		[CSVElement("group")]
		public int group
		{
			set
			{
				groupType = (PlayerSkillTalentType) value;
			}
		}

		[CSVElement("icon")]
		public string icon;

        private string[] name;
        [CSVElement("name")]
        public string nameString 
        {
            set
            {
                name = value.Split(CSVUtil.SYMBOL_SEMICOLON);
            }
        }

        private string[] des;

        [CSVElement("des")]
        public string desString 
        {
            set 
            {
                des = value.Split(CSVUtil.SYMBOL_SEMICOLON);
            }
        }
		private Dictionary<int,List<GameResData>> costResData = new Dictionary<int,List<GameResData>>();

		[CSVElement("lv1")]
		public string lv1
		{
			set
			{
				costResData.Add(1,ParseCostResData(value));
			}
		}
		[CSVElement("lv2")]
		public string lv2
		{
			set
			{
				costResData.Add(2,ParseCostResData(value));
			}
		}
		[CSVElement("lv3")]
		public string lv3
		{
			set
			{
				costResData.Add(3,ParseCostResData(value));
			}
		}
		[CSVElement("lv4")]
		public string lv4
		{
			set
			{
				costResData.Add(4,ParseCostResData(value));
			}
		}
		[CSVElement("lv5")]
		public string lv5
		{
			set
			{
				costResData.Add(5,ParseCostResData(value));
			}
		}
		[CSVElement("lv6")]
		public string lv6
		{
			set
			{
				costResData.Add(6,ParseCostResData(value));
			}
		}
		[CSVElement("lv7")]
		public string lv7
		{
			set
			{
				costResData.Add(7,ParseCostResData(value));
			}
		}
		[CSVElement("lv8")]
		public string lv8
		{
			set
			{
				costResData.Add(8,ParseCostResData(value));
			}
		}
		[CSVElement("lv9")]
		public string lv9
		{
			set
			{
				costResData.Add(9,ParseCostResData(value));
			}
		}
		[CSVElement("lv10")]
		public string lv10
		{
			set
			{
				costResData.Add(10,ParseCostResData(value));
			}
		}
	}

}
