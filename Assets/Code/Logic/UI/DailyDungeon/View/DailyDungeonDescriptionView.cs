using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common.Localization;

namespace Logic.UI.DailyDungeon.View
{
	public class DailyDungeonDescriptionView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/daily_dungeon/daily_dungeon_description_view";
		public static DailyDungeonDescriptionView Open()
		{
			DailyDungeonDescriptionView dailyDungeonInfoView = UIMgr.instance.Open<DailyDungeonDescriptionView>(DailyDungeonDescriptionView.PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay);
			return dailyDungeonInfoView;
		}
		#region UI components
		public Text dailyDungeonDescriptionTitleText;
		public Text dailyDungeonDescriptionText;
		#endregion

		void Awake ()
		{
			Init();
		}

		private void Init ()
		{
			dailyDungeonDescriptionTitleText.text = Localization.Get("ui.daily_dungeon_description_view.daily_dungeon_description_title");
			dailyDungeonDescriptionText.text = Localization.Get("ui.daily_dungeon_description_view.daily_dungeon_description");
		}

		#region UI event handlers
		public void ClickCloseHandler ()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
		#endregion
	}
}
