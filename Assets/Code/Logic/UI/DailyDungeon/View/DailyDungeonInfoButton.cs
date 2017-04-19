using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Localization;
using Common.ResMgr;
using Logic.Activity.Model;

namespace Logic.UI.DailyDungeon.View
{
	public class DailyDungeonInfoButton : MonoBehaviour
	{
		private ActivityInfo _activityInfo;

		public ActivityInfo ActivityInfo
		{
			get
			{
				return _activityInfo;
			}
		}

		#region UI components
		public Text dailyDungeonNameText;
		public Image dailyDungeonCardImage;
		public GameObject lockMaskGameObject;
		public Text openDateText;
		#endregion UI components

		public void SetActivityInfo (ActivityInfo activityInfo)
		{
			_activityInfo = activityInfo;
			dailyDungeonNameText.text = Localization.Get(_activityInfo.ActivityData.name);
			dailyDungeonCardImage.SetSprite(ResMgr.instance.Load<Sprite>(ResPath.GetActivityIconPath(activityInfo.ActivityData.pic)));
			lockMaskGameObject.SetActive(!_activityInfo.isOpen);

			string openDaysString = string.Empty;
			if (_activityInfo.ActivityData.openDayList.Count < 7)
			{
				openDaysString = UIUtil.GetWeekdayListString(_activityInfo.ActivityData.openDayList);
				openDaysString = string.Format(Localization.Get("ui.daily_dungeon_view.open_date"), openDaysString);
			}
			else
			{
				openDaysString = Localization.Get("ui.daily_dungeon_view.open_every_day");
			}
			openDateText.text = openDaysString;

			if (!_activityInfo.isOpen)
			{
				SetToGray();
			}
		}

		private void SetToGray ()
		{
			dailyDungeonCardImage.SetGray(true);
		}

		#region UI event handlers
		public void ClickHandler ()
		{
//			if (!_activityInfo.isOpen)
//			{
//				return;
//			}

			DailyDungeonInfoView dailyDungeonInfoView = UIMgr.instance.Open<DailyDungeonInfoView>(DailyDungeonInfoView.PREFAB_PATH);
			dailyDungeonInfoView.SetActivityInfo(_activityInfo);
		}
		#endregion UI event handlers
	}
}