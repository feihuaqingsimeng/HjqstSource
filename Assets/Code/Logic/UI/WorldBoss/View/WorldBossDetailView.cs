using UnityEngine;
using UnityEngine.UI;
using Common.Localization;

namespace Logic.UI.WorldBoss.View
{
	public class WorldBossDetailView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/world_boss/world_boss_detail_view";

		public GameObject core;

		#region UI components
		public Text titleText;
		public Text summaryTitleText;
		public Text summaryText;
		public Text weaknessTitleText;
		public Text weaknessText;
		#endregion UI components

		void Awake ()
		{
			titleText.text = Localization.Get("ui.world_boss_detail_view.second_title");
			summaryTitleText.text = Localization.Get("ui.world_boss_detail_view.summary_title");
			summaryText.text = Localization.Get("ui.world_boss_detail_view.summary");
			weaknessTitleText.text = Localization.Get("ui.world_boss_detail_view.weakness_title");
			weaknessText.text = Localization.Get("ui.world_boss_detail_view.weakness");
		}

		#region UI event handlers
		public void ClickCloseHandler ()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
		#endregion UI event handlers
	}
}