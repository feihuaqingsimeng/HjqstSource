using UnityEngine;
using Logic.Tutorial.Model;
using Logic.UI;
using Logic.UI.Tutorial.View;
using Logic.Protocol.Model;
using LuaInterface;

namespace Logic.Tutorial.Controller
{
	public class TutorialController : SingletonMono<TutorialController>
	{
		public const string TUTORIAL_STEP_COMPLETE_MSG = "TutorialStepComplete";

		public bool IsTutorialOpen
		{
			get
			{
				LuaScriptMgr.Instance.DoFile("user/tutorial_test");
				LuaTable tutorialChaptersLuaTable = LuaScriptMgr.Instance.GetLuaTable("tutorial_data");
				return tutorialChaptersLuaTable["IsTutorialOpen"].ToString().ToBoolean();
			}
		}
		private bool _initialized = false;

		void Awake ()
		{
			instance = this;
		}

		void Start ()
		{
			Observers.Facade.Instance.RegisterObserver(((int)MSG.LoginResp).ToString(), LOBBY2CLIENT_LOGIN_SUCCESS_Handler);

			UI.UIMgr.instance.onUIViewOpenDelegate += OnUIViewOpenHandler;
			Logic.Player.Model.PlayerProxy.instance.onPlayerInfoUpdateDelegate += OnPlayerInfoUpdateHandler;
			Logic.Dungeon.Model.DungeonProxy.instance.onPveFightOverDelegate += OnPveFightOverHandler;
			Logic.UI.SoftGuide.Model.SoftGuideProxy.instance.FunctionOpenDelegate += OnFunctionOpenHandler;
			Logic.UI.Task.Model.TaskProxy.instance.onTaskCompleteDelegate += OnTaskCompleteHandler;
			Logic.Dungeon.Model.DungeonProxy.instance.onDungeonInfoUpdateDelegate += OnDungeonInfoUpdateHandler;
		}

		void OnDestroy ()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver(((int)MSG.LoginResp).ToString(), LOBBY2CLIENT_LOGIN_SUCCESS_Handler);
			}
			UI.UIMgr.instance.onUIViewOpenDelegate -= OnUIViewOpenHandler;
			Logic.Player.Model.PlayerProxy.instance.onPlayerInfoUpdateDelegate -= OnPlayerInfoUpdateHandler;
			Logic.Dungeon.Model.DungeonProxy.instance.onPveFightOverDelegate -= OnPveFightOverHandler;
			Logic.UI.SoftGuide.Model.SoftGuideProxy.instance.FunctionOpenDelegate -= OnFunctionOpenHandler;
			Logic.UI.Task.Model.TaskProxy.instance.onTaskCompleteDelegate += OnTaskCompleteHandler;
			Logic.Dungeon.Model.DungeonProxy.instance.onDungeonInfoUpdateDelegate -= OnDungeonInfoUpdateHandler;
		}

		private bool LOBBY2CLIENT_LOGIN_SUCCESS_Handler (Observers.Interfaces.INotification note)
		{
			if (IsTutorialOpen)
			{
				LoginResp loginResp = note.Body as LoginResp;
				int lastTutorialChapterID = loginResp.guideNo > 0 ? loginResp.guideNo : TutorialChapterData.GetFirstTutorialChapterData().id;
				TutorialProxy.instance.Init(lastTutorialChapterID);
				_initialized = true;
			}
			return true;
		}

		private bool CheckIfChapterIsQualified (TutorialChapterData tutorialChapterData)
		{
			if (tutorialChapterData == null)
			{
				TutorialView.Close();
				return false;
			}
			if (!_initialized)
			{
				TutorialView.Close();
				return false;
			}
			if (!TutorialProxy.instance.IsFirstStep())
			{
				return false;
			}
			if (tutorialChapterData == null)
			{
				TutorialView.Close();
				return false;
			}
			if (Game.Model.GameProxy.instance.PlayerInfo == null)
			{
				TutorialView.Close();
				return false;
			}
			// check player level
			if (Game.Model.GameProxy.instance.PlayerInfo.level < tutorialChapterData.playerLevel)
			{
				TutorialView.Close();
				return false;
			}
			// check pass dungeon
			if (tutorialChapterData.passDungeon > 0)
			{
				Dungeon.Model.DungeonInfo requiredPassDungeonInfo = Dungeon.Model.DungeonProxy.instance.GetDungeonInfo(tutorialChapterData.passDungeon);
				if (requiredPassDungeonInfo == null || requiredPassDungeonInfo.star <= 0)
				{
					TutorialView.Close();
					return false;
				}
			}

			// check if task complete
			if (tutorialChapterData.taskID > 0
			    && !Logic.UI.Task.Model.TaskProxy.instance.IsTaskComplete(tutorialChapterData.taskID))
			{
				TutorialView.Close();
				return false;
			}
			// check dungeon total star count
			if (tutorialChapterData.dungeonTotalStarCount > 0
			    && Logic.Dungeon.Model.DungeonProxy.instance.GetDungeonTotalStarCount() < tutorialChapterData.dungeonTotalStarCount)
			{TutorialView.Close();
				return false;
			}
			// check active ui panel path
			if (tutorialChapterData.atUIViewPath != null
			    && UI.UIMgr.instance.LastOpenedUIPath != tutorialChapterData.atUIViewPath)
			{
				TutorialView.Close();
				return false;
			}
			if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen((Enums.FunctionOpenType)tutorialChapterData.functionOpenID))
			{
				TutorialView.Close();
				return false;
			}
			return true;
		}

		private bool CheckIfShouldOpenCurrentChapter ()
		{
			Debugger.Log("===== CheckIfShouldOpenCurrentChapter =====");

			int currentTutorialChapterID = TutorialProxy.instance.CurrentTutorialChapterData.id;
			TutorialChapterData currentTutorialChapterData = TutorialChapterData.GetTutorialChapterData(currentTutorialChapterID);
			TutorialProxy.instance.MoveToChapter(currentTutorialChapterData);

			// check if should ignore chapter
			TutorialChapterData tutorialChapterData = currentTutorialChapterData;
			TutorialChapterData latestCanNotIgnoredChapterData = null;
			while (tutorialChapterData != null && tutorialChapterData.canIgnore)
			{
				tutorialChapterData = tutorialChapterData.GetNextTutorialChapterData();
				if (tutorialChapterData != null && !tutorialChapterData.canIgnore)
				{
					latestCanNotIgnoredChapterData = tutorialChapterData;
				}
			}

			if (latestCanNotIgnoredChapterData != null && CheckIfChapterIsQualified(latestCanNotIgnoredChapterData))
			{
				TutorialProxy.instance.MoveToChapter(latestCanNotIgnoredChapterData);
				currentTutorialChapterData = TutorialProxy.instance.CurrentTutorialChapterData;
			}
			// check if should ignore chapter

			if (currentTutorialChapterData == null)
			{
				if (UIMgr.instance.IsOpening(TutorialView.PREFAB_PATH))
					TutorialView.Close();
				return false;
			}
			if (!_initialized)
			{
				if (UIMgr.instance.IsOpening(TutorialView.PREFAB_PATH))
					TutorialView.Close();
				return false;
			}
			if (!TutorialProxy.instance.IsFirstStep())
			{
				return false;
			}
			if (currentTutorialChapterData == null)
			{
				if (UIMgr.instance.IsOpening(TutorialView.PREFAB_PATH))
					TutorialView.Close();
				return false;
			}
			if (Game.Model.GameProxy.instance.PlayerInfo == null)
			{
				if (UIMgr.instance.IsOpening(TutorialView.PREFAB_PATH))
					TutorialView.Close();
				return false;
			}
			// check player level
			if (Game.Model.GameProxy.instance.PlayerInfo.level < currentTutorialChapterData.playerLevel)
			{
				if (UIMgr.instance.IsOpening(TutorialView.PREFAB_PATH))
					TutorialView.Close();
				return false;
			}
			// check pass dungeon
			if (currentTutorialChapterData.passDungeon > 0)
			{
				Dungeon.Model.DungeonInfo requiredPassDungeonInfo = Dungeon.Model.DungeonProxy.instance.GetDungeonInfo(currentTutorialChapterData.passDungeon);
				if (requiredPassDungeonInfo == null || requiredPassDungeonInfo.star <= 0)
				{
					if (UIMgr.instance.IsOpening(TutorialView.PREFAB_PATH))
						TutorialView.Close();
					return false;
				}
			}
			// check if task complete
			if (currentTutorialChapterData.taskID > 0
			    && !Logic.UI.Task.Model.TaskProxy.instance.IsTaskComplete(currentTutorialChapterData.taskID))
			{
				if (UIMgr.instance.IsOpening(TutorialView.PREFAB_PATH))
					TutorialView.Close();
				return false;
			}
			// check dungeon total star count
			if (currentTutorialChapterData.dungeonTotalStarCount > 0
			    && Logic.Dungeon.Model.DungeonProxy.instance.GetDungeonTotalStarCount() < currentTutorialChapterData.dungeonTotalStarCount)
			{
				if (UIMgr.instance.IsOpening(TutorialView.PREFAB_PATH))
					TutorialView.Close();
				return false;
			}
			// check active ui panel path
			if (currentTutorialChapterData.atUIViewPath != null
				&& UI.UIMgr.instance.LastOpenedUIPath != currentTutorialChapterData.atUIViewPath)
			{
				if (UIMgr.instance.IsOpening(TutorialView.PREFAB_PATH))
					TutorialView.Close();
				return false;
			}
			if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen((Enums.FunctionOpenType)currentTutorialChapterData.functionOpenID))
			{
				if (UIMgr.instance.IsOpening(TutorialView.PREFAB_PATH))
					TutorialView.Close();
				return false;
			}
//			if (!UIMgr.instance.IsOpening(TutorialView.PREFAB_PATH))
				TutorialView.Open(TutorialProxy.instance.CurrentTutorialChapterData ,TutorialProxy.instance.CurrentTutorialStepData);
			Debugger.Log("=====[Turotial]::[OpenChapter]::[" + (TutorialProxy.instance.CurrentTutorialChapterData.isBackup ? "Backup:" : "Main:") + TutorialProxy.instance.CurrentTutorialChapterData.id + "]");
			TutorialProxy.instance.IsCurrentTurorialChapterOpened = true;
			Debugger.Log("===== CheckIfShouldOpenCurrentChapter =====");
			return true;
		}

		private bool CheckIfShouldOpenCurrentBackupChapter ()
		{
			Debugger.Log("===== CheckIfShouldOpenCurrentBackupChapter =====");
			int currentTutorialChapterDataID = TutorialProxy.instance.CurrentTutorialChapterData.id;
			TutorialChapterData currentBackupChapter = TutorialChapterData.GetBackupTutorialChapterData(currentTutorialChapterDataID);
			if (currentBackupChapter != null)
			{
				if (CheckIfChapterIsQualified(currentBackupChapter))
				{
					TutorialProxy.instance.MoveToChapter(currentBackupChapter);
//					if (!UIMgr.instance.IsOpening(TutorialView.PREFAB_PATH))
						TutorialView.Open(TutorialProxy.instance.CurrentTutorialChapterData ,TutorialProxy.instance.CurrentTutorialStepData);
					Debugger.Log("======[Turotial]::[OpenChapter]::[" + (TutorialProxy.instance.CurrentTutorialChapterData.isBackup ? "Backup:" : "Main:") + TutorialProxy.instance.CurrentTutorialChapterData.id + "]");
					TutorialProxy.instance.IsCurrentTurorialChapterOpened = true;
				}
			}
			Debugger.Log("===== CheckIfShouldOpenCurrentBackupChapter =====");
			return false;
		}

		private bool CheckIfShouldOpenCurrentChapterOrCurrentBackupChapter ()
		{
			if (TutorialProxy.instance.CurrentTutorialChapterData != null)
			{
				int currentTutorialChapterID = TutorialProxy.instance.CurrentTutorialChapterData.id;
				if (TutorialProxy.instance.IsFirstStep())
				{
					TutorialChapterData currentTutorialChapterData = TutorialChapterData.GetTutorialChapterData(currentTutorialChapterID);
					if (CheckIfChapterIsQualified(currentTutorialChapterData))
					{
						if (CheckIfShouldOpenCurrentChapter())
						{
							return true;
						}
					}
					else if (currentTutorialChapterData.HasBackupTutorialChapterData())
					{
						TutorialChapterData backupTutorialChapterData = currentTutorialChapterData.GetBackupTutorialChapterData();
						if (CheckIfChapterIsQualified(backupTutorialChapterData))
						{
							return CheckIfShouldOpenCurrentBackupChapter();
						}
					}
				}
			}
			return false;
		}

		void OnUIViewOpenHandler (string uiPath)
		{
			CheckIfShouldOpenCurrentChapterOrCurrentBackupChapter();
		}

		void OnPlayerInfoUpdateHandler ()
		{
			CheckIfShouldOpenCurrentChapterOrCurrentBackupChapter();
		}

		void OnPveFightOverHandler ()
		{
			CheckIfShouldOpenCurrentChapterOrCurrentBackupChapter();
		}

		void OnFunctionOpenHandler ()
		{
			CheckIfShouldOpenCurrentChapterOrCurrentBackupChapter();
		}

		void OnTaskCompleteHandler (int taskID)
		{
			CheckIfShouldOpenCurrentChapterOrCurrentBackupChapter();
		}

		void OnDungeonInfoUpdateHandler ()
		{
			CheckIfShouldOpenCurrentChapterOrCurrentBackupChapter();
		}

		public void ExecuteStepComplete (TutorialStepData stepData)
		{
			if (stepData.id == TutorialProxy.instance.CurrentTutorialStepData.id)
			{
				if (TutorialProxy.instance.IsCompleteStep())
				{
					int nextChapterID = TutorialProxy.instance.CurrentTutorialChapterData.nextID;
					GuideReq guideReq = new GuideReq();
					guideReq.no = nextChapterID;
					guideReq.guideNo2 = 0;
					Logic.Protocol.ProtocolProxy.instance.SendProtocol(guideReq);
				}
				else if (!TutorialProxy.instance.IsAfterCompleteStep())
				{
					GuideReq guideReq = new GuideReq();
					guideReq.no = TutorialProxy.instance.CurrentTutorialChapterData.id;
					guideReq.guideNo2 = TutorialProxy.instance.CurrentTutorialChapterData.isBackup ? stepData.id + 100000 : stepData.id;
					Logic.Protocol.ProtocolProxy.instance.SendProtocol(guideReq);
				}

				if (TutorialProxy.instance.HasNextStep())
				{
					TutorialProxy.instance.MoveToNextStep(stepData);
					TutorialView.Open(TutorialProxy.instance.CurrentTutorialChapterData, TutorialProxy.instance.CurrentTutorialStepData);
					Observers.Facade.Instance.SendNotification(TUTORIAL_STEP_COMPLETE_MSG);
				}
				else if (TutorialProxy.instance.HasNextChapter())
				{
					TutorialProxy.instance.MoveToNextChapter();
					CheckIfShouldOpenCurrentChapterOrCurrentBackupChapter();
				}
			}
		}

		public void ExecuteStepComplete ()
		{
			ExecuteStepComplete(TutorialProxy.instance.CurrentTutorialStepData);
		}

		public void SkipCurrentTutorialChapter ()
		{
			if (UIMgr.instance.IsOpening(TutorialView.PREFAB_PATH))
				TutorialView.Close();

			int skipChapterDataID = TutorialProxy.instance.CurrentTutorialChapterData.id;

			GuideReq guidReq = new GuideReq();
			guidReq.no = skipChapterDataID + 1;
			Logic.Protocol.ProtocolProxy.instance.SendProtocol(guidReq);

			if (TutorialProxy.instance.HasNextChapter())
				TutorialProxy.instance.MoveToNextChapter();

			if (TutorialProxy.instance.onSkipCurrentChapterDelegate != null)
				TutorialProxy.instance.onSkipCurrentChapterDelegate(skipChapterDataID);
		}

		public void SkipWholeTutorial ()
		{
			TutorialProxy.instance.MoveToChapter(null);
			GuideReq guideReq = new GuideReq();
			guideReq.no = int.MaxValue;
			guideReq.guideNo2 = int.MaxValue;
			Logic.Protocol.ProtocolProxy.instance.SendProtocol(guideReq);
		}
	}
}