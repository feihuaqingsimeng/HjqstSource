using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common.GameTime.Controller;

namespace Logic.UI.LoginNoticeBoard.View
{
	public class LoginNoticeBoardView : MonoBehaviour 
	{
		public const string PREFAB_PATH = "ui/login_notice_board/login_notice_board_view";

		public static LoginNoticeBoardView Open(string content)
		{
			LoginNoticeBoardView view = UIMgr.instance.Open<LoginNoticeBoardView> (PREFAB_PATH, EUISortingLayer.Tips);
			view.SetData(content);
			return view;
		}

		public Text contentText;
		public RectTransform contentRoot;
		public Toggle toggle;
		void Start()
		{

		}

		private void SetData(string content)
		{
			contentText.text = content;
			contentRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,contentText.preferredHeight+10);
		}
		public void ClickCloseHandler()
		{
			PlayerPrefs.SetInt("tipLoginNoticeBoardDay",toggle.isOn ? System.DateTime.Now.DayOfYear : -1);
			UIMgr.instance.Close(PREFAB_PATH);
		}
	}
}

