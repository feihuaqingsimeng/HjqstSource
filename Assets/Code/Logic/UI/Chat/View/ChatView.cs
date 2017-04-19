using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.UI.Components;
using Logic.UI.Chat.Model;
using Logic.Enums;
using Logic.Equipment.Model;
using Logic.Game.Model;
using Common.Localization;
using System.Collections;
using Logic.UI.Chat.Controller;
using Logic.UI.Tips.View;
using Logic.Audio.Controller;

namespace Logic.UI.Chat.View
{
    public class ChatView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/chat/chat_view";
		public static ChatView Open()
		{
			ChatView view = UIMgr.instance.Open<ChatView>(PREFAB_PATH, Logic.UI.EUISortingLayer.MainUI, UIOpenMode.Overlay);
			return view;
		}
		#region ui component
		public ScrollContentExpand scrollContent;
		public Transform systemNoticeRoot;
		public InputField inputText;
		public Toggle[] toggles;
		#endregion


		private Toggle _currentToggle;


		void Awake()
		{
			BindDelegate();
		}
		void Start()
		{
			Init();
			if(!ChatProxy.instance.isSendChatListReq)
			{
				ChatProxy.instance.isSendChatListReq = true;
				ChatController.instance.CLIENT2LOBBY_ChatList_REQ();
			}
//			StartCoroutine(TestCoroutine());
		}
		void OnDestroy()
		{
			ClearEditData();
			UnbindDelegate();
		}
		private void BindDelegate()
		{
			ChatProxy.instance.onUpdateByComeMessageDelegate += UpdateMessageByProtocol;
		}
		private void UnbindDelegate()
		{
			ChatProxy.instance.onUpdateByComeMessageDelegate -= UpdateMessageByProtocol;
		}

		private void Init()
		{
//			for(int i = 0,count = toggles.Length;i<count;i++)
//			{
//				if(i == 0)
//					toggles[i].isOn = true;
//				else 
//					toggles[i].isOn = false;
//				toggles[i].GetComponent<ToggleContent>().Set(i,Localization.Get("ui.chat_view.toggle"+i));
//
//			}
			SystemNoticeView.Create(systemNoticeRoot);
			Refresh();
		}
//		private IEnumerator TestCoroutine()
//		{
//			while(true)
//			{
//				yield return new WaitForSeconds(3);
//				if(EquipmentProxy.instance.GetAllEquipmentInfoDictioary().Count > 0)
//				{
//					ChatInfo info = ChatProxy.instance.CreateChatInfoByEquip(EquipmentProxy.instance.GetAllEquipmentInfoDictioary().First().Value);
//					info.isMe = false;
//					ChatProxy.instance.AddChatInfo(info);
//					UpdateMessageByProtocol(false);
//				}
//			}
//		}
		private void Refresh()
		{
			scrollContent.Init(ChatProxy.instance.currentChatInfoList.Count);
			scrollContent.ScrollToBottom(0.1f);
		}
		private void ClearEditData()
		{
			inputText.text = string.Empty;
		}
		#region ui event handler
		public void OnCloseBtnHandler()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
		public void OnResetItemHandler(GameObject go,int index)
		{
			ChatContentView view = go.GetComponent<ChatContentView>();
			if(view != null)
			{
				ChatInfo info = ChatProxy.instance.currentChatInfoList[index];
				view.SetChatInfo(info);
			}
		}
		public void OnClickToggleBtnHandler(Toggle toggle)
		{
			if(toggle == _currentToggle)
				return;
			if(toggle.isOn)
			{
				_currentToggle = toggle;
				int id = toggle.GetComponent<ToggleContent>().id;
				ChatProxy.instance.currentChatType = (ChatType)(id+1);
				Refresh();

			}
		}
		public void OnClickSendBtnHandler()
		{

			if(!string.IsNullOrEmpty( inputText.text))
			{

				if(ChatProxy.instance.canSendMessage)
				{
					string s = Common.Util.BlackListWordUtil.WordsFilter( inputText.text);
					ChatInfo info = ChatProxy.instance.CreateChatInfoByInputString(s,ChatProxy.instance.currentChatType);
					ClearEditData();
					ChatController.instance.CLIENT2LOBBY_SendMessage_REQ(info);
				}else
				{
					CommonAutoDestroyTipsView.Open("操作太频繁啦！");
				}

			}
		}
		#endregion

		#region update by server
		private void UpdateMessageByProtocol(bool isMeSend)
		{
			float scrollValue = scrollContent.scrollRect.verticalScrollbar.value;
			scrollContent.RefreshCount(ChatProxy.instance.currentChatInfoList.Count);
			if(isMeSend)
				scrollContent.ScrollToBottom(0.3f);
			else
			{
				if(scrollValue<=0.1f)
				{
					scrollContent.ScrollToBottom(0.3f);
				}
			}
		}
		#endregion
    }
}