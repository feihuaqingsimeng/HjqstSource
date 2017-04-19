using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Enums;
using Logic.Game.Model;
using Logic.Protocol.Model;
using Logic.Equipment.Model;

namespace Logic.UI.Chat.Model
{
	public class ChatProxy : SingletonMono<ChatProxy> 
	{


		public delegate void UpdateByComeMessageDelegate(bool isMeSend);
		public UpdateByComeMessageDelegate onUpdateByComeMessageDelegate;

		void Awake()
		{
			instance = this;
		}

		private SortedDictionary<int,ChatInfo> _chatInfoDictionary = new SortedDictionary<int, ChatInfo>();

		public SortedDictionary<int,ChatInfo> ChatInfoDictionary
		{
			get
			{
				return _chatInfoDictionary;
			}
		}

		public ChatType currentChatType = ChatType.World;
		public List<ChatInfo> currentChatInfoList = new List<ChatInfo>();
		public bool canSendMessage = true;
		public bool isSendChatListReq = false;

		public void AddChatInfo(ChatInfo info)
		{
			if(info == null)
				return;
			if(_chatInfoDictionary.ContainsKey(info.id))
				return;
			_chatInfoDictionary.Add(info.id,info);
			currentChatInfoList.Add(info);

			if(_chatInfoDictionary.Count > 50)
			{
				DeleteChatInfo(currentChatInfoList[0].id);
			}
		}
		public void AddChatInfoList(List<ChatProtoData> protoData)
		{
		//	_chatInfoDictionary.Clear();
			//currentChatInfoList.Clear();
			ChatProtoData data;
			for(int i = 0,count = protoData.Count;i<count;i++)
			{
				data = protoData[i];
				if(data.chatType == (int)ChatType.World)
				{
					ChatInfo info = new ChatInfo();
					info.chatType = ChatType.World;
					info.content = data.content;
					info.talkerName = data.sendRoleName;
//					info.isMe = GameProxy.instance.PlayerInfo.name.Equals(data.sendRoleName);
					info.isMe = GameProxy.instance.AccountName.Equals(data.sendRoleName);
					ChatProxy.instance.AddChatInfo(info);
					ChatProxy.instance.UpdateChatByProtocol(info.isMe );
				}else if(data.chatType == (int)ChatType.System)
				{
					string s = data.content;
					if(data.noticeNo != 0)
					{
						SystemNoticeProxy.instance.AddSystemNotice(data.noticeNo,data.noticParams);
					}else
					{
						SystemNoticeProxy.instance.AddSystemNotice(s);
					}
				}
			}

		}
		public void DeleteChatInfo(int id)
		{
			if(_chatInfoDictionary.ContainsKey(id))
			{
				currentChatInfoList.Remove(_chatInfoDictionary[id]);
				_chatInfoDictionary.Remove(id);
			}

		}
		public ChatInfo CreateChatInfoByInputString(string s,ChatType type)
		{
			canSendMessage = true;
			if (type == ChatType.World)
				StartCoroutine(ChatSendCDCoroutine());
			if(s.Length>140)
			{
				s = s.Substring(0,140);
			}
			ChatInfo info = new ChatInfo();
			info.talkerName = GameProxy.instance.AccountName;
			info.chatType = type;
			info.isMe = true;
			info.content = s;
			return info;
		}
		private IEnumerator ChatSendCDCoroutine()
		{
			canSendMessage = false;
			yield return new WaitForSeconds(GlobalData.GetGlobalData().chat_cd);
			canSendMessage = true;
		}
		public void UpdateChatByProtocol(bool isMeSend)
		{
			if( onUpdateByComeMessageDelegate!= null)
				onUpdateByComeMessageDelegate(isMeSend);
		}
	}
}

