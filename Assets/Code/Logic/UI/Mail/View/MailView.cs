using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.UI.Components;
using Logic.UI.Mail.Model;
using Logic.UI.Mail.Controller;
using Logic.Game.Model;
using Logic.UI.Tips.View;
using Logic.UI.CommonTopBar.View;
using Common.Localization;
using LuaInterface;

namespace Logic.UI.Mail.View
{
	public class MailView : MonoBehaviour 
	{
		public const string PREFAB_PATH = "ui/mail/mail_view";
		public static MailView Open()
		{
			MailView view = UIMgr.instance.Open<MailView>(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay);
			return view;
		}

		#region ui component 
		public ScrollContentExpand scrollContent;
		public Transform core;
		public Text getAllMailText;
		public Text deleteMailText;
		private bool _isFirstEnter = true;
		#endregion

		void Awake()
		{
			CommonTopBarView barView = CommonTopBarView.CreateNewAndAttachTo(core);
			barView.SetAsCommonStyle(Localization.Get("ui.mail_view.title"),OnClickCloseHandler,true,true,true,false);
		}
		void Start()
		{
			MailProxy.instance.hasNewMailComing = false;
			MailController.instance.CLIENT2LOBBY_Mail_REQ();
			Init();
			BindDelegate();
		}
		void OnDestroy()
		{
			MailProxy.instance.hasNewMailComing = false;
			UnbindDelegate();
		}
		private void Init()
		{
			getAllMailText.text = Localization.Get("ui.mail_view.getAllMail");
			deleteMailText.text = Localization.Get("ui.mail_view.deleteMail");
		}
		private void BindDelegate()
		{
			MailProxy.instance.onRefreshAllDelegate += Refresh;
			MailProxy.instance.onUpdateGetAllMailRewardSuccessDelegate += GetAllRewardItemByProtocol;
			MailProxy.instance.onUpdateGetOneMailRewardSuccessDelegate += GetOneItemRewardByProtocol;
		}
		private void UnbindDelegate()
		{
			MailProxy.instance.onRefreshAllDelegate -= Refresh;
			MailProxy.instance.onUpdateGetAllMailRewardSuccessDelegate -= GetAllRewardItemByProtocol;
			MailProxy.instance.onUpdateGetOneMailRewardSuccessDelegate -= GetOneItemRewardByProtocol;
			
		}


		private void Refresh()
		{
			MailProxy.instance.GetMailInfoListByType();
			scrollContent.Init(MailProxy.instance.CurrentMailInfoList.Count,_isFirstEnter,0.2f);
			_isFirstEnter = false;

		}
		private void RefreshScrollContent()
		{
			MailProxy.instance.GetMailInfoListByType();
			scrollContent.RefreshCount(MailProxy.instance.CurrentMailInfoList.Count);
		}
		#region ui event handler
		public void OnClickCloseHandler()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}

		private List<GameResData> _getRewardAllList = new List<GameResData>();
		public void OnClickGetRewardAllMailHandler()
		{
			_getRewardAllList.Clear();
			Dictionary<int,MailInfo> mailDic = MailProxy.instance.MailInfoDictionary;
			if(mailDic.Count == 0)
			{
				CommonAutoDestroyTipsView.Open(Localization.Get("ui.mail_view.noMail"));
				return;
			}
			bool hasReward = false;
			foreach(KeyValuePair<int,MailInfo> value in mailDic)
			{
				if(!value.Value.isGetReward && value.Value.rewardList.Count != 0)
				{
					hasReward = true;
					_getRewardAllList.AddRange(value.Value.rewardList);

				}
			}
			if(hasReward){

				MailController.instance.CLIENT2LOBBY_GetRewardAllMail_REQ();
			}else {
				CommonAutoDestroyTipsView.Open(Localization.Get("ui.mail_view.noMail"));
			}
		}
		public void OnClickDeleteAllReadMailHandler()
		{
			bool hasDelete = false;
			Dictionary<int,MailInfo> mailDic = MailProxy.instance.MailInfoDictionary;
			foreach(KeyValuePair<int,MailInfo> value in mailDic)
			{
				if(value.Value.isGetReward || value.Value.rewardList.Count == 0)
				{
					hasDelete = true;
					break;
				}
			}
			if(hasDelete){
				ConfirmTipsView.Open(Localization.Get( "ui.mail_view.confirmDelete"),ConfirmDeleteMail);

			}else 
				CommonAutoDestroyTipsView.Open(Localization.Get("ui.mail_view.noMailDelete"));
		}
		private void ConfirmDeleteMail()
		{
			MailController.instance.CLIENT2LOBBY_DelateAllReadMail_REQ();
		}
		public void OnResetItemHandler(GameObject go,int index)
		{
			MailContentButton btn = go.GetComponent<MailContentButton>();
			btn.SetMailInfo(MailProxy.instance.CurrentMailInfoList[index],false);
		}

		#endregion
		#region server refresh
		public void GetAllRewardItemByProtocol()
		{
			RefreshScrollContent();
			OpenRewardTipView(UIUtil.CombineGameResList(_getRewardAllList));
		}
		public void GetOneItemRewardByProtocol(int id)
		{

			RefreshScrollContent();
			List<MailInfo> infoList = MailProxy.instance.CurrentMailInfoList;
			MailInfo info ;
			for(int i = 0,count = infoList.Count;i<count;i++)
			{
				info = infoList[i];
				if(info.id == id)
				{
					//scrollContent.RefreshContentItem(i);

					OpenRewardTipView(info.rewardList);
					break;
				}
			}
		}

		public void OpenRewardTipView(List<GameResData> rewardList)
		{
			LuaCsTransfer.OpenRewardTipsView(rewardList);
		}
		#endregion
	}
}

