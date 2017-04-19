using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Logic.Role.Model;
using Logic.Hero.Model;
using Logic.Player.Model;
using Common.ResMgr;
using Logic.UI.CommonHeroIcon.View;
using Logic.UI.IllustratedHandbook.Model;

namespace Logic.UI.IllustratedHandbook.View
{
	public class IllustratedHeroButton : MonoBehaviour 
	{
		
		#region ui component
		public Image headImage;
		public GameObject[] stars;
		public GameObject maskGO;
		public Image heroTypeImage;

		#endregion

		private Sprite _starNormalSprite;
		private Sprite _starDefaultSprite;
		private RoleInfo _roleInfo;

		void Awake()
		{
			_starNormalSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/icon_star");
			_starDefaultSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/icon_star2_big_disable");
		}
		public void SetRoleInfo( RoleInfo info)
		{
			_roleInfo = info;
			Refresh();
			ShowMask(false);
		}

		private void Refresh()
		{
			HeroInfo heroInfo = _roleInfo as HeroInfo;
			PlayerInfo playerInfo = _roleInfo as PlayerInfo ;
			if(heroInfo != null)
			{
				headImage.SetSprite(ResMgr.instance.Load<Sprite>(heroInfo.HeadIcon));
			}else if(playerInfo != null)
			{
				headImage.SetSprite(ResMgr.instance.Load<Sprite>( playerInfo.PetHeadIcon));
			}
			for (int i = 0,starCount = stars.Length; i < starCount; i++)
			{
				stars[i].GetComponent<Image>().SetSprite(i < _roleInfo.advanceLevel ? _starNormalSprite : _starDefaultSprite);
				stars[i].SetActive(i < _roleInfo.MaxAdvanceLevel);
			}
			if (heroTypeImage != null)
			{
				heroTypeImage.SetSprite( UIUtil.GetRoleTypeSmallIconSprite(_roleInfo.heroData.roleType));
			}
		}
		public void ShowMask(bool value)
		{
			maskGO.SetActive(value);
		}

		public void OnClickRoleButtonHandler()
		{
			IllustratedHandbookProxy.instance.CheckedDetailRoleInfo = _roleInfo;
			IllustrationDetailView.Open(_roleInfo);
		}
	}

}

