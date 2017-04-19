using UnityEngine;
using System.Collections;

namespace Logic.UI.HeroesPreview.View
{
	public class HeroesPreviewView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/heroes_preview_view/heroes_preview_view";

		#region ui event handlers
		public void ClickCloseHandler ()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
		#endregion
	}
}
