using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Logic.UI.Mail.Model;
using Common.Localization;
using Common.ResMgr;
using Common.Util;
using Logic.UI.CommonReward.View;
using Logic.Game.Model;
using Logic.UI.Mail.Controller;
using Common.GameTime.Controller;
using Logic.UI.Tips.View;

namespace Logic.UI.Mail.View
{
	public class MailContentButton : MonoBehaviour 
	{

		#region ui component 
		public Text titleText;
		public Text contentText;
		public Text btnRewardText;
		public Image mailIcon;
		public RectTransform reward_root;
		public RectTransform root;
		public RectTransform contentBgRT;
		public GameObject btnRewardGO;
		public Text sendTimeText;
		#endregion

		public MailInfo mailInfo;

		private bool _isShowDetail;
		private Vector2 _defaultRootSize;
		private Vector2 _defaultTitleRootSize;
		private Vector2 _defaultRewardSize;
		private float _defaultContentHeight;
		private float _defaultContentWidth;
		void Awake()
		{
			_defaultRootSize = root.rect.size;
			_defaultTitleRootSize = contentBgRT.rect.size;
			_defaultRewardSize = reward_root.rect.size;
			_defaultContentHeight = contentText.preferredHeight;
			_defaultContentWidth = contentText.preferredWidth;
		}

		public void SetMailInfo(MailInfo info,bool isDetail)
		{
			mailInfo = info;
			_isShowDetail = isDetail;
			Refresh();
		}

		public void Refresh()
		{
			//root.sizeDelta = _defaultRootSize;
			//contentBgRT.sizeDelta = _defaultTitleRootSize;
			MailData data = mailInfo.mailData;
			if(mailInfo.titleParam != null && mailInfo.titleParam.Length != 0)
			{
				titleText.text = string.Format( Localization.Get( data.name),mailInfo.titleParam);
			}else
			{
				titleText.text = Localization.Get( data.name);

			}

			if(mailInfo.contentParam != null && mailInfo.contentParam.Length != 0)
			{
				contentText.text = string.Format( Localization.Get( data.des),mailInfo.contentParam);
			}else
			{
				contentText.text = Localization.Get( data.des);
			}
			string contentString = contentText.text;
			if(contentString.Length >= 17 && !_isShowDetail)
			{
				contentText.text = contentString.Substring(0,12)+"...";
			}

			btnRewardText.text = mailInfo.isGetReward ?  Localization.Get("ui.mail_view.alreadyGet") : Localization.Get("ui.mail_view.getmail");
			GameObject btn = btnRewardText.transform.parent.gameObject;
			btn.GetComponent<Button>().interactable = !mailInfo.isGetReward ;
			UIUtil.SetGray(btn ,mailInfo.isGetReward);

			if(mailIcon!= null)
			{
				string picPath = data.pic;
				if(!string.IsNullOrEmpty(mailInfo.picParam))
				{
					picPath = string.Format(Localization.Get(data.pic),mailInfo.picParam);
				}
				mailIcon.SetSprite(ResMgr.instance.Load<Sprite>(picPath));
				mailIcon.SetNativeSize();
			}
			if(sendTimeText != null)
			{
				System.DateTime time = TimeUtil.FormatTime((int)(mailInfo.createTime/1000));
				sendTimeText.text = string.Format(Localization.Get("ui.mail_view.time"),time);
			}
			TransformUtil.ClearChildren(reward_root,true);
			GameResData resData;
			int count = mailInfo.rewardList.Count;
			if(!_isShowDetail)
				count = count > 4 ? 4:count;
			for(int i = 0;i<count;i++)
			{
				resData = mailInfo.rewardList[i];
				CommonRewardIcon icon = CommonRewardIcon.Create(reward_root);
				icon.SetGameResData(resData);
				//if(_isShowDetail)
					//icon.ShowName(true);
			}

			if(count == 0)
			{
				btnRewardGO.SetActive(false);
			}else
			{
				btnRewardGO.SetActive(true);
//				if(_isShowDetail)
//				{
//					GridLayoutGroup grid = reward_root.GetComponent<GridLayoutGroup>();
//					if(grid != null)
//					{
//						int row = count/grid.constraintCount + (count%grid.constraintCount == 0? 0:1);
//						float h = row*grid.cellSize.y+(row-1)*grid.spacing.y;
//						float delta = h-_defaultRewardSize.y;
//						Vector2 deltaVec = new Vector2(0,delta);
//						root.sizeDelta += deltaVec;
//						contentBgRT.sizeDelta += deltaVec;
//					}
//
//				}
			}
		}
		public void OnClickShowDetailHandler()
		{
			if(! mailInfo.isRead)
				MailController.instance.CLIENT2LOBBY_MailReadReq_REQ(mailInfo.id);

			MailContentDetailView.Open(mailInfo);
		}
		public void OnClickGetRewardBtnHandler()
		{
			if(mailInfo.isGetReward)
				return;
			MailProxy.instance.GetOneMailRewardInfo = mailInfo;

			MailController.instance.CLIENT2LOBBY_GetRewardOneMail_REQ(mailInfo.id);
		}
		public void OnClickDeleteHandler()
		{
			if (mailInfo.isGetReward || mailInfo.rewardList.Count == 0)
			{
				MailController.instance.CLIENT2LOBBY_DelateAllReadMail_REQ(false,mailInfo.id);
			}else
			{
				CommonAutoDestroyTipsView.Open(Localization.Get("ui.mail_view.hasRewardTip"));
			}

		}

	}
}

