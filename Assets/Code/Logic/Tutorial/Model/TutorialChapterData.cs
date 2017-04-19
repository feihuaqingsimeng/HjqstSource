using UnityEngine;
using System.Collections.Generic;
using LuaInterface;
using System.Collections;

namespace Logic.Tutorial.Model
{
	public class TutorialChapterData
	{
		private static List<int> _tutorialChapterDataIDList;
		public static List<int> TutorialChapterDataIDList
		{
			get
			{
				if (_tutorialChapterDataIDList == null)
				{
					LuaScriptMgr.Instance.DoFile("user/tutorial_test");
					LuaTable tutorialChapterIDListLuaTable = LuaScriptMgr.Instance.GetLuaTable("tutorial_cahpter_id_list");
					_tutorialChapterDataIDList = new List<int>();
					foreach (object o in tutorialChapterIDListLuaTable.ToArray())
					{
						_tutorialChapterDataIDList.Add(o.ToString().ToInt32());
					}

				}
				return _tutorialChapterDataIDList;
			}
		}

		private static Dictionary<int, int> _tutorialChapterNextIDDic;
		private static Dictionary<int, int> TutorialChapterNextIDDic
		{
			get
			{
				if (_tutorialChapterNextIDDic == null)
				{
					_tutorialChapterNextIDDic = new Dictionary<int, int>();
					LuaScriptMgr.Instance.DoFile("user/tutorial_test");
					LuaTable tutorialChapterIDListLuaTable = LuaScriptMgr.Instance.GetLuaTable("tutorial_cahpter_id_list");
					List<int> tutorialChapterDataIDList = new List<int>();
					foreach (object o in tutorialChapterIDListLuaTable.ToArray())
					{
						tutorialChapterDataIDList.Add(o.ToString().ToInt32());
					}

					for (int i = 0, count = tutorialChapterDataIDList.Count; i < count - 1; i++)
					{
						_tutorialChapterNextIDDic.Add(tutorialChapterDataIDList[i], tutorialChapterDataIDList[i + 1]);
					}
				}
				return _tutorialChapterNextIDDic;
			}
		}

		private static SortedDictionary<int, TutorialChapterData> _tutorialChapterDataSortedDic;
		public static SortedDictionary<int, TutorialChapterData> TutorialChapterDataSortedDic
		{
			get
			{
				if (_tutorialChapterDataSortedDic == null)
				{
					LuaScriptMgr.Instance.DoFile("user/tutorial_test");
					LuaTable tutorialChaptersLuaTable = LuaScriptMgr.Instance.GetLuaTable("tutorial_chapter");
                    //LuaArrayTable tutorialChaptersLuaArrayTable = tutorialChaptersLuaTable.ToArrayTable();
					TutorialChapterData tutorialChapterData = null;
					LuaDictTable tutorialChapterLuaDictTable = tutorialChaptersLuaTable.ToDictTable();
					_tutorialChapterDataSortedDic = new SortedDictionary<int, TutorialChapterData>();
					foreach (DictionaryEntry kvp in tutorialChapterLuaDictTable)
					{
						int tutorialChapterID = kvp.Key.ToString().ToInt32();
						LuaTable tutorialChapterLuaTable = kvp.Value as LuaTable;
						tutorialChapterData = TutorialChapterData.CreateFromLuaTable(tutorialChapterID, tutorialChapterLuaTable, false);
						int nextChapterID = 0;
						TutorialChapterNextIDDic.TryGetValue(tutorialChapterData.id, out nextChapterID);
						tutorialChapterData.nextID = nextChapterID;
						_tutorialChapterDataSortedDic.Add(tutorialChapterID, tutorialChapterData);
					}
				}
				return _tutorialChapterDataSortedDic;
			}
		}

		private static SortedDictionary<int, TutorialChapterData> _backupTutorialChapterDataSortedDic;
		private static SortedDictionary<int, TutorialChapterData> BackupTutorialChapterDataSortedDic
		{
			get
			{
				if (_backupTutorialChapterDataSortedDic == null)
				{
					LuaScriptMgr.Instance.DoFile("user/tutorial_backup");
					LuaTable backupTutorialChaptersLuaTable = LuaScriptMgr.Instance.GetLuaTable("backup_tutorial_chapter");
					TutorialChapterData tutorialChapterData = null;
					LuaDictTable tutorialChapterLuaDictTable = backupTutorialChaptersLuaTable.ToDictTable();
					_backupTutorialChapterDataSortedDic = new SortedDictionary<int, TutorialChapterData>();
					foreach (DictionaryEntry kvp in tutorialChapterLuaDictTable)
					{
						int tutorialChapterID = kvp.Key.ToString().ToInt32();
						LuaTable tutorialChapterLuaTable = kvp.Value as LuaTable;
						tutorialChapterData = TutorialChapterData.CreateFromLuaTable(tutorialChapterID, tutorialChapterLuaTable, true);
						int nextChapterID = 0;
						TutorialChapterNextIDDic.TryGetValue(tutorialChapterData.id, out nextChapterID);
						tutorialChapterData.nextID = nextChapterID;
						_backupTutorialChapterDataSortedDic.Add(tutorialChapterID, tutorialChapterData);
					}
				}
				return _backupTutorialChapterDataSortedDic;
			}
		}

		public int id;
		public int nextID = 0;
		public int playerLevel = 0;
		public int passDungeon = 0;
		public int taskID = 0;
		public int dungeonTotalStarCount = 0;
		public string atUIViewPath;
		public int functionOpenID = 0;

		public int completeStepID = 0;
		public bool isSkippable = false;
		public bool canIgnore = false;

		public bool isBackup = false;
		private SortedDictionary<int, TutorialStepData> _stepsDic = new SortedDictionary<int, TutorialStepData>();

		public static TutorialChapterData CreateFromLuaTable (int tutorialChapterID, LuaTable luaTable, bool isBackup)
		{
			TutorialChapterData tutorialChapterData = new TutorialChapterData(tutorialChapterID, luaTable);
			tutorialChapterData.isBackup = isBackup;
			return tutorialChapterData;
		}

		public TutorialChapterData (int tutorialChapterID, LuaTable luaTable)
		{
			id = tutorialChapterID;
			if (luaTable["player_level"] != null)
				playerLevel = luaTable["player_level"].ToString().ToInt32();
			if (luaTable["pass_dungeon"] != null)
				passDungeon = luaTable["pass_dungeon"].ToString().ToInt32();
			if (luaTable["task_id"] != null)
				taskID = luaTable["task_id"].ToString().ToInt32();
			if (luaTable["dungeon_total_star_count"] != null)
				dungeonTotalStarCount = luaTable["dungeon_total_star_count"].ToString().ToInt32();
			if (luaTable["at_ui_view_path"] != null)
				atUIViewPath = luaTable["at_ui_view_path"].ToString();
			if (luaTable["function_open_id"] != null)
				functionOpenID = luaTable["function_open_id"].ToString().ToInt32();
			if (luaTable["complete_step_id"] != null)
				completeStepID = luaTable["complete_step_id"].ToString().ToInt32();
			if (luaTable["is_skippable"] != null)
				isSkippable = luaTable["is_skippable"].ToString().ToBoolean();
			if (luaTable["can_ignore"] != null)
				canIgnore = luaTable["can_ignore"].ToString().ToBoolean();

			LuaTable stepsLuaTable = (LuaTable)luaTable["steps"];
			foreach (DictionaryEntry kvp in stepsLuaTable.ToDictTable())
			{
				LuaTable stepLuaTable = kvp.Value as LuaTable;
				_stepsDic.Add(stepLuaTable["id"].ToString().ToInt32(), TutorialStepData.CreateFromLuaTable(stepLuaTable));
			}
		}

		public static TutorialChapterData GetFirstTutorialChapterData ()
		{
			return GetTutorialChapterData(TutorialChapterDataIDList[0]);
		}

		public static TutorialChapterData GetTutorialChapterData (int id)
		{
			TutorialChapterData tutorialChapterData = null;
			TutorialChapterDataSortedDic.TryGetValue(id, out tutorialChapterData);
			return tutorialChapterData;
		}

		public static TutorialChapterData GetBackupTutorialChapterData (int id)
		{
			TutorialChapterData backupTutorialChapterData = null;
			BackupTutorialChapterDataSortedDic.TryGetValue(id, out backupTutorialChapterData);
			return backupTutorialChapterData;
		}

		public bool HasBackupTutorialChapterData ()
		{
			return BackupTutorialChapterDataSortedDic.ContainsKey(id);
		}

		public TutorialChapterData GetBackupTutorialChapterData ()
		{
			return BackupTutorialChapterDataSortedDic[id];
		}

		public bool HasNextChapter ()
		{
			return TutorialChapterDataSortedDic.ContainsKey(nextID);
		}

		public TutorialChapterData GetNextTutorialChapterData ()
		{
			TutorialChapterData nextTutorialChapterData = null;
			TutorialChapterDataSortedDic.TryGetValue(GetTutorialChapterData(id).nextID, out nextTutorialChapterData);
			return nextTutorialChapterData;
		}

		public TutorialStepData GetStepData (int id)
		{
			TutorialStepData tutorialStepData = null;
			_stepsDic.TryGetValue(id, out tutorialStepData);
			return tutorialStepData;
		}

		public TutorialStepData GetNextStepData (TutorialStepData tutorialStepData)
		{
			TutorialStepData nextStepData = null;
			_stepsDic.TryGetValue(tutorialStepData.nextID, out nextStepData);
			return nextStepData;
		}

		public bool IsFirstStep (int stepID)
		{
			return stepID == _stepsDic.First().Key;
		}

		public bool IsLastStep (int stepID)
		{
			return stepID == _stepsDic.Last().Key;
		}

		public bool IsComleteStep (int stepID)
		{
			return stepID == completeStepID;
		}

		public bool IsAfterCompleteStep (int stepID)
		{
			return stepID > completeStepID;
		}
	}
}
