using UnityEngine;
using System.Collections;
using Logic.UI.Chat.Model;
using Logic.Protocol.Model;
using Logic.Enums;
using Logic.Game.Model;


namespace Logic.UI.Chat.Controller
{
	public class ChatController : SingletonMono<ChatController> {
		
		void Awake()
		{
			instance = this;
		}
		void Start ()
		{
			Observers.Facade.Instance.RegisterObserver(((int)MSG.ChatResp).ToString(),LOBBY2CLIEN_Chat_RESP_handler);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.ChatInfoResp).ToString(),LOBBY2CLIEN_ChatList_RESP_handler);
		}
		
		void OnDestroy ()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver(((int)MSG.ChatResp).ToString(),LOBBY2CLIEN_Chat_RESP_handler);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.ChatInfoResp).ToString(),LOBBY2CLIEN_ChatList_RESP_handler);
			}
		}

		#region client to server
		public void CLIENT2LOBBY_ChatList_REQ()
		{
			ChatInfoReq req = new ChatInfoReq();
			Protocol.ProtocolProxy.instance.SendProtocol(req);
		}
		public void CLIENT2LOBBY_SendMessage_REQ(ChatInfo info)
		{
			ChatReq req = new ChatReq();
			req.chatType = (int)info.chatType;
			req.revRoleName = info.talkerName;
			req.content = info.content;
			//ChatProxy.instance.AddChatInfo(info);
			//ChatProxy.instance.UpdateChatByProtocol(info.isMe );
			Protocol.ProtocolProxy.instance.SendProtocol(req);
		}
		#endregion

		#region server to client
		public bool LOBBY2CLIEN_Chat_RESP_handler(Observers.Interfaces.INotification note)
		{
			ChatResp resp = note.Body as ChatResp;
			if(resp.chatType == (int)ChatType.World)
			{
				ChatInfo info = new ChatInfo();
				info.chatType = ChatType.World;
				info.content = resp.content;
				info.talkerName = resp.sendRoleName;
//				info.isMe = GameProxy.instance.PlayerInfo.name.Equals(resp.sendRoleName);
				info.isMe = GameProxy.instance.AccountName.Equals(resp.sendRoleName);
				ChatProxy.instance.AddChatInfo(info);
				ChatProxy.instance.UpdateChatByProtocol(info.isMe );
			}else if(resp.chatType == (int)ChatType.System)
			{
				string s = resp.content;
				if(resp.noticeNo!= 0)
				{
					SystemNoticeProxy.instance.AddSystemNotice(resp.noticeNo,resp.noticParams);
				}else
				{
					SystemNoticeProxy.instance.AddSystemNotice(s);
				}
			}
			return true;
		}
		public bool LOBBY2CLIEN_ChatList_RESP_handler(Observers.Interfaces.INotification note)
		{
			ChatInfoResp resp = note.Body as ChatInfoResp;

			ChatProxy.instance.AddChatInfoList(resp.chatList);
			return true;
		}
		#endregion
	}
}

