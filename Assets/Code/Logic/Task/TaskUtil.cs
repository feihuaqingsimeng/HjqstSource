using UnityEngine;
using Common.Localization;
using Logic.Enums;
using Logic.Game.Model;
using Logic.Dungeon.Model;
using Logic.Player.Model;
using Logic.Hero.Model;
using Logic.Item.Model;
using Logic.Task.Model;

namespace Logic.Task
{
	public static class TaskUtil
	{
		private static bool CheckAccountLevelRequirement (TaskConditionData taskConditionData)
		{
			if (taskConditionData.taskType != TaskType.AccountLevelRequirement)
			{
				return false;
			}
			return GameProxy.instance.AccountLevel >= taskConditionData.parameter1.ToInt32();
		}

		private static bool CheckPassDungeonTimes (TaskConditionData taskConditionData)
		{
			if (taskConditionData.taskType != TaskType.PassDungeonTimes)
			{
				return false;
			}
			return DungeonProxy.instance.GetDungeonInfo(taskConditionData.parameter1.ToInt32()).star > 0;
		}

		private static bool CheckProfessionIDAndLevel (TaskConditionData taskConditionData)
		{
			if (taskConditionData.taskType != TaskType.ProfessionIDAndLevel)
			{
				return false;
			}

			int playerDataID = taskConditionData.parameter1.ToInt32();
			int levelRequired = taskConditionData.parameter2.ToInt32();
			PlayerInfo playerInfo = PlayerProxy.instance.GetPlayerDataCorrespondingPlayerInfo(playerDataID);
			if (playerInfo != null)
			{
				if (playerInfo.level >= levelRequired)
				{
					return true;
				}
			}
			return false;
		}

		private static bool CheckVIPLevelRequirement (TaskConditionData taskConditionData)
		{
			if (taskConditionData.taskType != TaskType.VIPLevelRequirement)
			{
				return false;
			}
			return false;
		}

		private static bool CheckMultipleHeroLevelRequirement (TaskConditionData taskConditionData)
		{
			if (taskConditionData.taskType != TaskType.MultipleHeroLevelRequirement)
			{
				return false;
			}

			int requiredHeroCount = taskConditionData.parameter1.ToInt32();
			int requiredHeroLevel = taskConditionData.parameter2.ToInt32();
			if (HeroProxy.instance.GetHeroInfosLevelMoreThan(requiredHeroLevel).Count >= requiredHeroCount)
			{
				return true;
			}
			return false;
		}

		private static bool CheckMultipleHeroStarRequirement (TaskConditionData taskConditionData)
		{
			if (taskConditionData.taskType != TaskType.MultipleHeroStarRequirement)
			{
				return false;
			}

			int requiredHeroCount = taskConditionData.parameter1.ToInt32();
			int requiredHeroStar = taskConditionData.parameter2.ToInt32();
			if (HeroProxy.instance.GetHeroInfosStarMoreThan(requiredHeroStar).Count >= requiredHeroCount)
			{
				return true;
			}
			return false;
		}

		private static bool CheckOwnResource (TaskConditionData taskConditionData)
		{
			if (taskConditionData.taskType != TaskType.OwnResouce)
			{
				return false;
			}

			GameResData requiredGameResData = new GameResData(taskConditionData.parameter1);
			if (requiredGameResData.type == BaseResType.Gold)
			{
				return GameProxy.instance.BaseResourceDictionary[BaseResType.Gold] >= requiredGameResData.count;
			}
			else if (requiredGameResData.type == BaseResType.Diamond)
			{
				return GameProxy.instance.BaseResourceDictionary[BaseResType.Diamond] >= requiredGameResData.count;
			}
			else if (requiredGameResData.type == BaseResType.Crystal)
			{
				return GameProxy.instance.BaseResourceDictionary[BaseResType.Crystal] >= requiredGameResData.count;
			}
			else if (requiredGameResData.type == BaseResType.Honor)
			{
				return GameProxy.instance.BaseResourceDictionary[BaseResType.Honor] >= requiredGameResData.count;
			}
			else if (requiredGameResData.type == BaseResType.Item)
			{
				ItemInfo requiredItemInfo = ItemProxy.instance.GetItemInfoByItemID(requiredGameResData.id);
				if (requiredItemInfo != null)
				{
					if (requiredItemInfo.count >= requiredGameResData.count)
					{
						return true;
					}
				}
				return false;
			}
			else if (requiredGameResData.type == BaseResType.Hero)
			{
				if (HeroProxy.instance.GetHeroInfosByHeroDataID(requiredGameResData.id).Count >= requiredGameResData.count)
				{
					return true;
				}
				return false;
			}
			return false;
		}

		private static bool CheckFirstTopUpInTime (TaskConditionData taskConditionData)
		{
			if (taskConditionData.taskType != TaskType.FirstTopUpInTime)
			{
				return false;
			}
			return false;
		}

		private static bool CheckSingleTopUp (TaskConditionData taskConditionData)
		{
			if (taskConditionData.taskType != TaskType.SingleTopUp)
			{
				return false;
			}
			return false;
		}

		public static bool IsTaskConditionFinished (TaskConditionData taskConditionData)
		{
			switch (taskConditionData.taskType)
			{
				case TaskType.AccountLevelRequirement:
					return CheckAccountLevelRequirement(taskConditionData);
				case TaskType.PassDungeonTimes:
					return CheckPassDungeonTimes(taskConditionData);
				case TaskType.ProfessionIDAndLevel:
					return CheckProfessionIDAndLevel(taskConditionData);
				case TaskType.VIPLevelRequirement:
					return CheckVIPLevelRequirement(taskConditionData);
				case TaskType.MultipleHeroLevelRequirement:
					return CheckMultipleHeroLevelRequirement(taskConditionData);
				case TaskType.MultipleHeroStarRequirement:
					return CheckMultipleHeroStarRequirement(taskConditionData);
				case TaskType.OwnResouce:
					return CheckOwnResource(taskConditionData);
				case TaskType.FirstTopUpInTime:
					return CheckFirstTopUpInTime(taskConditionData);
				case TaskType.SingleTopUp:
					return CheckSingleTopUp(taskConditionData);
				default:
					break;
			}
			return false;
		}

		public static string GetTaskConditionDescription (TaskConditionData taskConditionData)
		{
			string taskConditionDescription = string.Empty;
			if (taskConditionData.taskType == TaskType.AccountLevelRequirement)
			{
				taskConditionDescription = Localization.Get("game.task_condition_description.account_level_requirement");
				taskConditionDescription = string.Format(taskConditionDescription, taskConditionData.parameter1);
			}
			else if (taskConditionData.taskType == TaskType.PassDungeonTimes)
			{
				taskConditionDescription = Localization.Get("game.task_condition_description.pass_dungeon_times");
				int dungeonID = taskConditionData.parameter1.ToInt32();
				DungeonInfo dungeonInfo = DungeonProxy.instance.GetDungeonInfo(dungeonID);
				string dungeonName = Localization.Get(dungeonInfo.dungeonData.name);
				taskConditionDescription = string.Format(taskConditionDescription, dungeonName, taskConditionData.parameter2);
			}
			else if (taskConditionData.taskType == TaskType.ProfessionIDAndLevel)
			{
				taskConditionDescription = Localization.Get("game.task_condition_description.profession_id_and_level");
				taskConditionDescription = string.Format(taskConditionDescription, taskConditionData.parameter1, taskConditionData.parameter2);
			}
			else if (taskConditionData.taskType == TaskType.VIPLevelRequirement)
			{
				taskConditionDescription = Localization.Get("game.task_condition_description.vip_level_requirement");
				taskConditionDescription = string.Format(taskConditionDescription, taskConditionData.parameter1);
			}
			else if (taskConditionData.taskType == TaskType.MultipleHeroLevelRequirement)
			{
				taskConditionDescription = Localization.Get("game.task_condition_description.multiple_hero_level_requirement");
				taskConditionDescription = string.Format(taskConditionDescription, taskConditionData.parameter1, taskConditionData.parameter2);
			}
			else if (taskConditionData.taskType == TaskType.MultipleHeroStarRequirement)
			{
				taskConditionDescription = Localization.Get("game.task_condition_description.multiple_hero_star_requirement");
				taskConditionDescription = string.Format(taskConditionDescription, taskConditionData.parameter1, taskConditionData.parameter2);
			}
			else if (taskConditionData.taskType == TaskType.OwnResouce)
			{
				GameResData requiredGameResData = new GameResData(taskConditionData.parameter1);
				if (requiredGameResData.type == BaseResType.Gold)
				{
					taskConditionDescription = Localization.Get("game.task_condition_description.own_gold");
					taskConditionDescription = string.Format(taskConditionDescription, requiredGameResData.count);
				}
				else if (requiredGameResData.type == BaseResType.Diamond)
				{
					taskConditionDescription = Localization.Get("game.task_condition_description.own_diamond");
					taskConditionDescription = string.Format(taskConditionDescription, requiredGameResData.count);
				}
				else if (requiredGameResData.type == BaseResType.Crystal)
				{
					taskConditionDescription = Localization.Get("game.task_condition_description.own_crystal");
					taskConditionDescription = string.Format(taskConditionDescription, requiredGameResData.count);
				}
				else if (requiredGameResData.type == BaseResType.Honor)
				{
					taskConditionDescription = Localization.Get("game.task_condition_description.own_honor");
					taskConditionDescription = string.Format(taskConditionDescription, requiredGameResData.count);
				}
				else if (requiredGameResData.type == BaseResType.Item)
				{
					ItemData itemData = ItemData.GetItemDataByID(requiredGameResData.id);
					taskConditionDescription = Localization.Get("game.task_condition_description.own_item");
					taskConditionDescription = string.Format(taskConditionDescription, requiredGameResData.count, Localization.Get(itemData.name));
				}
				else if (requiredGameResData.type == BaseResType.Hero)
				{
					HeroData heroData = HeroData.GetHeroDataByID(requiredGameResData.id);
					taskConditionDescription = Localization.Get("game.task_condition_description.own_hero");
					taskConditionDescription = string.Format(taskConditionDescription, requiredGameResData.count, Localization.Get(heroData.name));
				}
			}
			else if (taskConditionData.taskType == TaskType.FirstTopUpInTime)
			{
				taskConditionDescription = Localization.Get("game.task_condition_description.first_top_up_in_time");
				taskConditionDescription = string.Format(taskConditionDescription, taskConditionData.parameter1);
			}
			else if (taskConditionData.taskType == TaskType.SingleTopUp)
			{
				taskConditionDescription = Localization.Get("game.task_condition_description.single_top_up");
				taskConditionDescription = string.Format(taskConditionDescription, taskConditionData.parameter1);
			}
			return taskConditionDescription;
		}
		
		public static string GetTaskConditionDescriptionWithColor (TaskConditionData taskConditionData)
		{
			string finishedTaskDescripitonTemplate = "<color=#00ff00>{0}</color>";
			string unfinishedTaskDescripitonTemplate = "<color=#ff0000>{0}</color>";
			string taskConditionDescripitonWithColor = GetTaskConditionDescription(taskConditionData);
			bool isTaskConditionFinished = IsTaskConditionFinished(taskConditionData);
			if (isTaskConditionFinished)
			{
				taskConditionDescripitonWithColor = string.Format(finishedTaskDescripitonTemplate, taskConditionDescripitonWithColor);
			}
			else
			{
				taskConditionDescripitonWithColor = string.Format(unfinishedTaskDescripitonTemplate, taskConditionDescripitonWithColor);
			}
			return taskConditionDescripitonWithColor;
		}
	}
}
