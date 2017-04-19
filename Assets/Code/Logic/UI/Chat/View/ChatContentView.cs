using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Logic.UI.Chat.Model;
using System.Collections.Generic;
using Common.Util;
using Logic.Enums;
using Common.UI.Components;
using Logic.UI.Description.View;
using Logic.UI.CommonEquipment.View;
using Common.ResMgr;

namespace Logic.UI.Chat.View
{
	public class ChatContentView : MonoBehaviour 
	{
		#region ui component
		public Text leftTitleText;
		public Text leftContentText;
		public RectTransform leftContentBg;
		public GameObject leftRoot;

		public Text rightTitleText;
		public Text rightContentText;
		public RectTransform rightContentBg;
		public GameObject rightRoot;
		public RectTransform root;
		public float contentImgBgDefaultHeight = 10;
		#endregion


		public ChatInfo chatInfo;

		public void SetChatInfo(ChatInfo chatInfo)
		{
			this.chatInfo = chatInfo;
			Refresh();
		}
		private void Refresh()
		{
			Text titleText;
			Text contentText;
			RectTransform contentBg;
			leftRoot.SetActive(!chatInfo.isMe);
			rightRoot.SetActive(chatInfo.isMe);
			if(chatInfo.isMe)
			{
				titleText = rightTitleText;
				contentBg = rightContentBg;
				contentText = rightContentText;
			}else
			{
				titleText = leftTitleText;
				contentBg = leftContentBg;
				contentText = leftContentText;
			}

			titleText.text = string.Format("{0}",chatInfo.talkerName);


			contentText.text = chatInfo.content;
			contentText.name = "text";
				
			float h = contentText.preferredHeight;
			float w = contentText.preferredWidth;
			if(contentBg != null)
				contentBg.sizeDelta = new Vector2(contentBg.sizeDelta.x,h+contentImgBgDefaultHeight);
			float titleTextW = titleText.preferredWidth;
			float delta = contentText.rectTransform.sizeDelta.x-w;
			if (delta < 0 )
				delta = 0;
			contentText.transform.localPosition = new Vector3(titleText.transform.localPosition.x+ (chatInfo.isMe ? (-titleTextW-20+delta) : (titleTextW+20)),contentText.transform.localPosition.y,0);
			float rootH = -contentText.transform.localPosition.y + h;
			root.sizeDelta = new Vector2(root.sizeDelta.x,rootH);
		}

	}
}

