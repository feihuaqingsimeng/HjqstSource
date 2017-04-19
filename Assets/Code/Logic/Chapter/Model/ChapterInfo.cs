using UnityEngine;
using System.Collections;
using Logic.Enums;
using Logic.Dungeon.Model;

namespace Logic.Chapter.Model
{
    public class ChapterInfo
    {
        public int chapterId;
        public ChapterData chapterData;

        public ChapterInfo(int chapterId)
        {
            this.chapterId = chapterId;
            chapterData = ChapterData.GetChapterDataById(chapterId);
        }

		public bool IsLock (DungeonType dungeonType)
		{
			if (dungeonType == DungeonType.Easy)
			{
				for (int i = 0, count = chapterData.easyDungeonIDList.Count; i < count; i++)
				{
					if (!DungeonProxy.instance.GetDungeonInfo(chapterData.easyDungeonIDList[i]).isLock)
					{
						return false;
					}
				}
				return true;
			}
			else if (dungeonType == DungeonType.Normal)
			{
				for (int i = 0, count = chapterData.normalDungeonIDList.Count; i < count; i++)
				{
					if (!DungeonProxy.instance.GetDungeonInfo(chapterData.normalDungeonIDList[i]).isLock)
					{
						return false;
					}
				}
				return true;
			}
			else if (dungeonType == DungeonType.Hard)
			{
				for (int i = 0, count = chapterData.hardDungeonIDList.Count; i < count; i++)
				{
					if (!DungeonProxy.instance.GetDungeonInfo(chapterData.hardDungeonIDList[i]).isLock)
					{
						return false;
					}
				}
				return true;
			}
			return true;
		}
    }
}