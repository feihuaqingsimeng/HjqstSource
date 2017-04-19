using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Logic.UI.Mail.Model;

namespace Logic.UI.Mail.View
{
	public class MailContentDetailView : MonoBehaviour 
	{
		
		public const string PREFAB_PATH = "ui/mail/mail_content_detail_view";

		private MailInfo _mailInfo;
		public static MailContentDetailView Open(MailInfo info)
		{
			MailContentDetailView view = UIMgr.instance.Open<MailContentDetailView>(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay);
			view.SetMailInfo(info);
			return view;
		}

		public MailContentButton contentButton;
		public RectTransform contentBgTran;
		public RectTransform scrollRectTran;

		void Awake()
		{
			MailProxy.instance.onUpdateGetOneMailRewardSuccessDelegate += RefreshOneItem;
		}
		void OnDestroy()
		{
			MailProxy.instance.onUpdateGetOneMailRewardSuccessDelegate -= RefreshOneItem;
			
		}

		public void SetMailInfo(MailInfo info)
		{
			_mailInfo = info;
			Refresh();
		}
		public void Refresh()
		{
			if(_mailInfo.rewardList.Count == 0)
			{
				scrollRectTran.sizeDelta = new Vector2(scrollRectTran.sizeDelta.x,contentBgTran.sizeDelta.y);
			}
			contentButton.SetMailInfo(_mailInfo,true);

		}
		public void RefreshOneItem(int id )
		{
			Refresh();
		}
		#region ui event handler
		public void OnClickCloseBtnHandler()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
		#endregion
	}

}

