using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Logic.UI.Friend.Controller;
using Common.Localization;
using Logic.UI.Friend.Model;
using Logic.UI.Tips.View;
using System.Collections.Generic;

namespace Logic.UI.Friend.View
{
	public class FriendAddView : MonoBehaviour 
	{

		public const string PREFAB_PATH = "ui/friend/friend_add_view";
		public static FriendAddView Open()
		{
			FriendAddView view = UIMgr.instance.Open<FriendAddView>(PREFAB_PATH, EUISortingLayer.Tips);
			return view;
		}

		public InputField inputField;

		public void OnClickCloseBtnHandler()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
		public void OnClickAddBtnHandler()
		{
			string text = inputField.text;
			if(string.IsNullOrEmpty(text))
			{
				Logic.UI.Tips.View.CommonAutoDestroyTipsView.Open(Localization.Get("ui.friendAddView.not_null"));
				return;
			}
			if(FriendProxy.instance.myFriendListDic.Count >= FriendProxy.instance.maxMyFriendCount)
			{
				CommonAutoDestroyTipsView.Open(Localization.Get("ui.friendView.reachMyFriendLimit"));
				return;
			}
			Dictionary<int,FriendInfo> friendDic = FriendProxy.instance.myFriendListDic;
			bool exist = false;
			foreach(var value in friendDic)
			{
				if(value.Value.name.Equals(text))
				{
					exist = true;
					break;
				}
			}
			if(exist)
			{
				CommonAutoDestroyTipsView.Open(Localization.Get("ui.friendAddView.alreadyFriend"));
				return;
			}

			FriendController.instance.CLIENT2LOBBY_FriendAddReq_REQ(text,0);
		}
		public void OnClickCancelBtnHandler()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
	}
}

