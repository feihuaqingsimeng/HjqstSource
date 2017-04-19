using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common.Localization;

namespace Logic.UI.DailyDungeon.View
{
	public class RecommendedBuddiesView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/daily_dungeon/recommended_buddies_view";

		#region UI components
		public Text recommendedBuddiesTitleText;

		public Text recommended01Text;
		public Text recommended02Text;
		public Text recommended03Text;
		#endregion

		void Awake ()
		{
			Init();
		}

		private void Init ()
		{
			recommendedBuddiesTitleText.text = Localization.Get("ui.recommended_buddies_view.recommended_buddies_view_title");

			recommended01Text.text = Localization.Get("ui.recommended_buddies_view.recommended");
			recommended02Text.text = Localization.Get("ui.recommended_buddies_view.recommended");
			recommended03Text.text = Localization.Get("ui.recommended_buddies_view.recommended");
		}

		#region UI event Handlers
		public void ClickClose ()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
		#endregion
	}
}
