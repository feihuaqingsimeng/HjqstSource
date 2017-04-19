using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Localization;
using Logic.Activity.Model;
using Logic.UI.CommonAnimations;

namespace Logic.UI.DailyDungeon.View
{
	public class DailyDungeonView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/daily_dungeon/daily_dungeon_view";

		#region UI components
		public Transform core;
	
		public Text descriptionText;
		public Transform dailyDungeonInfoButtonsRootTransform;
		public DailyDungeonInfoButton dailyDungeonInfoButtonPrefab;
		#endregion

		void Awake ()
		{
			Init();
		}

		private void Init ()
		{
			CommonTopBar.View.CommonTopBarView commonTopBarView = CommonTopBar.View.CommonTopBarView.CreateNewAndAttachTo(core.transform);
			commonTopBarView.SetAsCommonStyle(Localization.Get("ui.daily_dungeon_view.daily_dungeon_title"), ClickCloseHandler, true, true, true, false);

			descriptionText.text = Localization.Get("ui.daily_dungeon_view.description");

			List<ActivityInfo> enabledActivityInfoList = ActivityProxy.instance.GetEnabledActivityInfoList();
			int activityCount = enabledActivityInfoList.Count;
			dailyDungeonInfoButtonPrefab.gameObject.SetActive(false);
			for (int i = 0; i < activityCount; i++)
			{
				DailyDungeonInfoButton dailyDungeonInfoButton = GameObject.Instantiate<DailyDungeonInfoButton>(dailyDungeonInfoButtonPrefab);
				dailyDungeonInfoButton.SetActivityInfo(enabledActivityInfoList[i]);
				dailyDungeonInfoButton.gameObject.name = enabledActivityInfoList[i].isOpen ? "open_dungeon" : "closed_dungeon";
				dailyDungeonInfoButton.transform.SetParent(dailyDungeonInfoButtonsRootTransform, false);
				dailyDungeonInfoButton.gameObject.SetActive(true);
				CommonScaleInAnimation.Get(dailyDungeonInfoButton.gameObject).Set(0.2f,i*0.05f,new Vector3(0,1,1),Vector3.one);
			}
			Observers.Facade.Instance.SendNotification(string.Format("{0}::{1}", PREFAB_PATH, "OnViewReady"));
		}

		#region UI event handlers
		public void ClickCloseHandler ()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}

		public void ClickDailyDungeonButtonHandler (Button button)
		{
			UIMgr.instance.Open<DailyDungeonInfoView>(DailyDungeonInfoView.PREFAB_PATH);
		}

		public void ClickRecommendedBuddies ()
		{
			UIMgr.instance.Open(RecommendedBuddiesView.PREFAB_PATH);
		}

		public void ClickDescriptionHandler ()
		{
			DailyDungeonDescriptionView.Open();
		}
		#endregion
	}
}
