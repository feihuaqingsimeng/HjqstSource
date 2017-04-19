using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Logic.UI.Friend.Model;
using Logic.Hero.Model;
using Common.ResMgr;
using Common.Localization;
using Logic.Role.Model;
using Logic.UI.CommonHeroIcon.View;
using Logic.UI.Description.View;

namespace Logic.UI.Friend.View
{
	public class FriendDetailView : MonoBehaviour 
	{

		public const string PREFAB_PATH = "ui/friend/friend_detail_view";
		public static FriendDetailView Open(FriendInfo info)
		{
			FriendDetailView view = UIMgr.instance.Open<FriendDetailView>(PREFAB_PATH, EUISortingLayer.Tips);
			view.SetFriendInfo(info);
			return view;
		}

		#region ui component
		public Image imgIcon;
		public Text textLevel;
		public Text textName;
		public Text textPower;
		public Transform heroRoot;
		#endregion

		private FriendInfo _friendInfo;
		private void SetFriendInfo(FriendInfo info)
		{
			_friendInfo = info;
			Refresh();
		}
		private void Refresh()
		{

			imgIcon.SetSprite(ResMgr.instance.Load<Sprite>(_friendInfo.headIcon));
			textLevel.text = string.Format(Localization.Get("ui.friendView.friendLv"),_friendInfo.level);
			textName.text = _friendInfo.name;
			textPower.text = string.Format(Localization.Get("ui.friendView.friendPower"),_friendInfo.power);

			RoleInfo roleInfo;
			for(int i = 0,count = _friendInfo.formationHeroList.Count;i<count;i++)
			{
				roleInfo = _friendInfo.formationHeroList[i];
				CommonHeroIcon.View.CommonHeroIcon icon = CommonHeroIcon.View.CommonHeroIcon.Create(heroRoot);
				icon.SetRoleInfo(roleInfo);
				RoleDesButton des = icon.gameObject.AddComponent<RoleDesButton>();
				des.SetRoleInfo(roleInfo);
			}
		}
		public void OnClickCloseBtnHandler()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}

	}
}

