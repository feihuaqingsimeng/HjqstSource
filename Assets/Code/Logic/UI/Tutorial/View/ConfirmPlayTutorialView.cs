using UnityEngine;
using System.Collections;

namespace Logic.UI.Tutorial.View
{
	public class ConfirmPlayTutorialView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/tutorial/confirm_play_tutorial_view";

		private System.Action _clickPlayDelegate;
		private System.Action _clickSkipDelegate;

		public static ConfirmPlayTutorialView Open (System.Action clickPlayDelegate, System.Action clickSkipDelegate)
		{
			ConfirmPlayTutorialView confirmPlayTutorialView = UIMgr.instance.Open<ConfirmPlayTutorialView>(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay);
			confirmPlayTutorialView._clickPlayDelegate = clickPlayDelegate;
			confirmPlayTutorialView._clickSkipDelegate = clickSkipDelegate;
			return confirmPlayTutorialView;
		}

		public void ClickPlayButtonHandler ()
		{
			if (_clickPlayDelegate != null)
			{
				_clickPlayDelegate();
			}
			UIMgr.instance.Close(PREFAB_PATH);
		}

		public void ClickSkipButtonHandler ()
		{
			/*
			Logic.Tutorial.Controller.TutorialController.instance.SkipWholeTutorial();
			if (_clickSkipDelegate != null)
			{
				_clickSkipDelegate();
			}
			UIMgr.instance.Close(PREFAB_PATH);
			*/

			Logic.UI.Tips.View.ConfirmTipsView.Open(Common.Localization.Localization.Get("ui.confirm_play_tutorial_view.twice_tips"), ClickConfirmSkipButtonHandler);
		}

		public void ClickConfirmSkipButtonHandler ()
		{
			Logic.Tutorial.Controller.TutorialController.instance.SkipWholeTutorial();
			if (_clickSkipDelegate != null)
			{
				_clickSkipDelegate();
			}
			UIMgr.instance.Close(PREFAB_PATH);
		}
	}
}