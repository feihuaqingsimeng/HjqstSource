using UnityEngine;
using System.Collections;

namespace Logic.UI.Achievements.View
{
	public class AchievementsView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/achievements/achievements_view";

		#region ui event handlers
		public void ClickCloseHandler ()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
		#endregion
	}
}
