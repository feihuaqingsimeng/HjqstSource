using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Enums;
using Logic.Equipment.Model;
using Common.Localization;
using System.Text;
using Logic.Equipment;
using Common.Util;
using Common.ResMgr;

namespace Logic.UI.Chat.Model
{


	public class ChatInfo {

		public int id;
		public string talkerName;
		public int talkTime;
		public ChatType chatType;
		public bool isMe ;
		//public List<ChatContentInfo> contentList = new List<ChatContentInfo>();
		public string content;
		private static int _id = 0;

		public ChatInfo()
		{
			id = _id;
			_id ++;
		}
	}
}

