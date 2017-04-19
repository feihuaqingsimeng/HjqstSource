using UnityEngine;
using System.Collections.Generic;
using Common.Util;
using Logic.Enums;
namespace Logic.Chapter.Model
{
    public class ChapterData
    {
        private static Dictionary<int, ChapterData> _chapterDataDic;
        private static Dictionary<int, ChapterData> _dungeonIDChapterDataDic;

        public static Dictionary<int, ChapterData> GetChapterDatas()
        {
            if (_chapterDataDic == null)
            {
                _chapterDataDic = CSVUtil.Parse<int, ChapterData>("config/csv/chapter", "id");
            }
            return _chapterDataDic;
        }

        private static Dictionary<int, ChapterData> DungeonIDChapterDataDic
        {
            get
            {
                if (_dungeonIDChapterDataDic == null)
                {
                    _dungeonIDChapterDataDic = new Dictionary<int, ChapterData>();
                    Dictionary<int, ChapterData> chapterDataDic = GetChapterDatas();
                    List<int> chapterIDList = chapterDataDic.GetKeys();
                    for (int i = 0; i < chapterIDList.Count; i++)
                    {
                        ChapterData chapterData = GetChapterDataById(chapterIDList[i]);
                        for (int easyDungeonIDIndex = 0; easyDungeonIDIndex < chapterData.easyDungeonIDList.Count; easyDungeonIDIndex++)
                        {
                            _dungeonIDChapterDataDic.Add(chapterData.easyDungeonIDList[easyDungeonIDIndex], chapterData);
                        }

                        for (int noramlDungeonIDIndex = 0; noramlDungeonIDIndex < chapterData.normalDungeonIDList.Count; noramlDungeonIDIndex++)
                        {
                            _dungeonIDChapterDataDic.Add(chapterData.normalDungeonIDList[noramlDungeonIDIndex], chapterData);
                        }

                        for (int hardDungeonIDIndex = 0; hardDungeonIDIndex < chapterData.hardDungeonIDList.Count; hardDungeonIDIndex++)
                        {
                            _dungeonIDChapterDataDic.Add(chapterData.hardDungeonIDList[hardDungeonIDIndex], chapterData);
                        }
                    }
                }
                return _dungeonIDChapterDataDic;
            }
        }

        public static ChapterData GetChapterDataById(int id)
        {
            if (_chapterDataDic == null)
                GetChapterDatas();
            if (_chapterDataDic.ContainsKey(id))
                return _chapterDataDic[id];
            Debugger.Log("[Warning]=====[can't find chapter id:" + id + "]");
            return null;
        }

        public static ChapterData GetChapterDataContainsDungeon(int dungeonID)
        {
            ChapterData chapterData = null;
            DungeonIDChapterDataDic.TryGetValue(dungeonID, out chapterData);
            return chapterData;
        }

        #region field
        [CSVElement("id")]
        public int Id;

        [CSVElement("name")]
        public string name;

        public Vector2 chapterPosition;
        [CSVElement("chapter_position")]
        public string chapterPositionStr
        {
            set
            {
                chapterPosition = value.ToVector2('|');
            }
        }

        [CSVElement("chapter_bg")]
        public string chapterBG;

        [CSVElement("chapter_line_bg")]
        public string chapterLineBG;

        [CSVElement("chapter_name")]
        public string chapterName;

        public List<Vector2> positions;
        [CSVElement("positions")]
        public string positionsStr
        {
            set
            {
                positions = new List<Vector2>();
                string[] strs = value.ToArray(CSVUtil.SYMBOL_SEMICOLON);
                for (int i = 0, count = strs.Length; i < count; i++)
                {
                    float[] vectors = strs[i].ToArray<float>(CSVUtil.SYMBOL_PIPE);
                    Vector2 v2 = new Vector2(vectors[0], vectors[1]);
                    positions.Add(v2);
                }
            }
        }

        public List<int> easyDungeonIDList;
        [CSVElement("dungeons_easy")]
        public string dungeonsEasyStr
        {
            set
            {
                easyDungeonIDList = value.ToList<int>(CSVUtil.SYMBOL_SEMICOLON);
            }
        }

        public List<int> normalDungeonIDList;
        [CSVElement("dungeons_normal")]
        public string dungeonsNormalStr
        {
            set
            {
                normalDungeonIDList = value.ToList<int>(CSVUtil.SYMBOL_SEMICOLON);
            }
        }

        public List<int> hardDungeonIDList;
        [CSVElement("dungeons_hard")]
        public string dungeonsHardStr
        {
            set
            {
                hardDungeonIDList = value.ToList<int>(CSVUtil.SYMBOL_SEMICOLON);
            }
        }
        #endregion

        public List<int> GetChapterDungeonIDListOfDungeonType(DungeonType dungeonType)
        {
            if (dungeonType == DungeonType.Easy)
            {
                return easyDungeonIDList;
            }
            else if (dungeonType == DungeonType.Normal)
            {
                return normalDungeonIDList;
            }
            else if (dungeonType == DungeonType.Hard)
            {
                return hardDungeonIDList;
            }
            return null;
        }

        public ChapterData PreviousChapterData
        {
            get
            {
                return GetChapterDataById(this.Id - 1);
            }
        }

        public ChapterData NextChapterData
        {
            get
            {
                return GetChapterDataById(this.Id + 1);
            }
        }
    }
}