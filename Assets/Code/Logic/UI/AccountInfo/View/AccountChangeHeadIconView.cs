using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Common.UI.Components;
using Logic.UI.IllustratedHandbook.Controller;
using Logic.Protocol.Model;
using Logic.Role.Model;
using Logic.UI.IllustratedHandbook.Model;
using Logic.Game.Model;
using Logic.UI.Login.Controller;
using Logic.Player.Model;

namespace Logic.UI.AccountInfo.View
{
	public class AccountChangeHeadIconView : MonoBehaviour 
	{
		
		public const string PREFAB_PATH = "ui/account_info/account_change_headicon_view";
		public static AccountChangeHeadIconView Open()
		{
			AccountChangeHeadIconView view = UIMgr.instance.Open<AccountChangeHeadIconView>(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay);
			return view;
		}

		public ScrollContentExpand scrollContent;
		public GameObject heroPrefab;
		public GameObject noneGO;

		private List<RoleInfo> _roleList;
		private List<int> _headIconNoList = new List<int>();
		private List<string> _headIconPathList = new List<string>();

		void Start()
		{
			heroPrefab.SetActive(false);
			Init();

		}
		private void Init()
		{
			_roleList = IllustratedHandbookProxy.instance.GetIllustrationRoleList();
			RoleInfo _info;
			for(int i = 0 ,count = _roleList.Count;i<count;i++)
			{
				_info = _roleList[i];
				string headIcon = _info.HeadIcon;
				if(!_headIconPathList.Contains(headIcon))
				{
					_headIconPathList.Add(headIcon);
					_headIconNoList.Add(_info.heroData.id*10+_info.advanceLevel*1);
				}
//				if(_info is PlayerInfo)
//				{
//					PlayerInfo player = _info as PlayerInfo;
//					headIcon = player.PetHeadIcon;
//					if(!_headIconPathList.Contains(headIcon))
//					{
//						_headIconNoList.Add(player.heroData.id*10+player.advanceLevel*1);
//						_headIconPathList.Add(headIcon);
//					}
//				}
			}
			//add default
			string defaultIcon = GameProxy.instance.PlayerInfo.HeadIcon;
			if(!_headIconPathList.Contains(defaultIcon))
			{
				_headIconNoList.Add(GameProxy.instance.PlayerInfo.heroData.id*10+GameProxy.instance.PlayerInfo.advanceLevel*1);
				_headIconPathList.Add(defaultIcon);
			}
			scrollContent.Init(_headIconNoList.Count);
			noneGO.SetActive(_headIconNoList.Count == 0);
		}
		public void ClickSureBtnHandler()
		{
			ClickCloseBtnHandler();
		}
		public void ClickCloseBtnHandler()
		{
			UIMgr.instance.Close(PREFAB_PATH);
			AccountInfoView.Open();
		}
		public void ClickHeroHeadBtnHander(HeadIconChangeButton btn)
		{
			int No = btn.headNo;
			GameProxy.instance.AccountHeadIcon = UIUtil.ParseHeadIcon(No);
			scrollContent.RefreshAllContentItems();
			LoginController.instance.CLIENT2LOBBY_ROLE_HEAD_REQ(No);
		}

		public void OnResetItemHandler(GameObject go,int index)
		{
			HeadIconChangeButton btn = go.GetComponent<HeadIconChangeButton>();
			int no = _headIconNoList[index];
			string path = _headIconPathList[index];
			bool isSelect = path.Equals(GameProxy.instance.AccountHeadIcon);

			btn.Set(no,isSelect);
		}


	}
}

