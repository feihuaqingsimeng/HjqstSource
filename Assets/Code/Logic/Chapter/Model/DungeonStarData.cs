using Common.Util;
using System.Collections.Generic;
using Logic.Enums;

namespace Logic.Chapter.Model
{
	public class DungeonStarData
	{
		private const string COMPOSITE_KEY_TEMPLATE_STRING = "[{0}][{1}][{2}]";

		private static Dictionary<int, DungeonStarData> _dungeonStarDataDictionary;
		private static Dictionary<string, DungeonStarData> _compositeKeyDungeonStarDataDictionary;
		
		public static Dictionary<int, DungeonStarData> GetDungeonStarDataDictionary()
		{
			if (_dungeonStarDataDictionary == null)
			{
				_dungeonStarDataDictionary = CSVUtil.Parse<int, DungeonStarData>("config/csv/dungeon_star", "id");
			}
			return _dungeonStarDataDictionary;
		}
		
		public static Dictionary<int, DungeonStarData> DungeonStarDataDictionary
		{
			get
			{
				if (_dungeonStarDataDictionary == null)
				{
					GetDungeonStarDataDictionary();
				}
				return _dungeonStarDataDictionary;
			}
		}

		public static Dictionary<string, DungeonStarData> CompositeKeyDungeonStarDataDictionary
		{
			get
			{
				if (_compositeKeyDungeonStarDataDictionary == null)
				{
					_compositeKeyDungeonStarDataDictionary = new Dictionary<string, DungeonStarData>();
					List<int> idList = DungeonStarDataDictionary.GetKeys();
					DungeonStarData dungeonStarData = null;
					string compositeKey = string.Empty;
					for (int i = 0; i < idList.Count; i++)
					{
						dungeonStarData = DungeonStarDataDictionary[idList[i]];
						compositeKey = string.Format(COMPOSITE_KEY_TEMPLATE_STRING, dungeonStarData.dungeonType.ToString(), dungeonStarData.chapterID, dungeonStarData.chestPosition);
						_compositeKeyDungeonStarDataDictionary.Add(compositeKey, dungeonStarData);
						compositeKey = string.Empty;
					}
				}
				return _compositeKeyDungeonStarDataDictionary;
			}
		}

		public static DungeonStarData GetDungeonStarData (int id)
		{
			DungeonStarData dungeonStarData = null;
			DungeonStarDataDictionary.TryGetValue(id, out dungeonStarData);
			return dungeonStarData;
		}

		public static DungeonStarData GetDungeonStarData (DungeonType dungeonType, int chapterID, int chestPosition)
		{
			DungeonStarData dungeonStarData = null;
			string compositeKey = string.Format(COMPOSITE_KEY_TEMPLATE_STRING, dungeonType.ToString(), chapterID, chestPosition);
			CompositeKeyDungeonStarDataDictionary.TryGetValue(compositeKey, out dungeonStarData);
			return dungeonStarData;
		}

		[CSVElement("id")]
		public int id;

		[CSVElement("chapterId")]
		public int chapterID;

		public DungeonType dungeonType = DungeonType.Invalid;
		[CSVElement("dungeon_type")]
		public int dungeonTypeIntValue
		{
			get
			{
				return (int)dungeonType;
			}
			set
			{
				dungeonType = (DungeonType)value;
			}
		}

		[CSVElement("chest_position")]
		public int chestPosition;

		[CSVElement("star_number")]
		public int starNumber;

		[CSVElement("award")]
		public string awardStr
		{
			set
			{

			}
		}
	}
}
