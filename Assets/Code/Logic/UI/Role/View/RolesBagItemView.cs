using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Localization;
using Common.ResMgr;
using Logic.Role.Model;
using Logic.Hero.Model;
using Logic.Player.Model;
using Logic.Role;
using Logic.Game.Model;

namespace Logic.UI.Role.View
{
	public class RolesBagItemView : MonoBehaviour
	{
		private uint _roleInstanceID;
		public RoleInfo RoleInfo
		{
			set
			{
				_roleInstanceID = 0;
				if (value != null)
					_roleInstanceID = value.instanceID;
			}
			get
			{
				RoleInfo roleInfo = null;
				if (_roleInstanceID == GameProxy.instance.PlayerInfo.instanceID)
					roleInfo = GameProxy.instance.PlayerInfo;
				else
					roleInfo = HeroProxy.instance.GetHeroInfo(_roleInstanceID);
				return roleInfo;
			}
		}

		#region UI components
		public Button expandRolesBagButton;
		public GameObject contentRoot;
		public Image roleHeadIconImage;
		public Text levelText;
		public Text strengthenLevelText;
		public List<Image> starImages;
		public Image inFormationMarkImage;
		public Image selectMarkImage;
		public Image newRoleMark;

		public Image roleTypeIconImage;

		private Sprite _starNormalSprite;
		public Sprite StarNormalSprite
		{
			get
			{
				if (_starNormalSprite == null)
					_starNormalSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/icon_star");
				return _starNormalSprite;
			}
		}

		private Sprite _starDefaultSprite;
		public Sprite StarDefaultSprite
		{
			get
			{
				if (_starDefaultSprite == null)
					_starDefaultSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/icon_star2_big_disable");
				return _starDefaultSprite;
			}
		}

		#endregion UI components
		public void SetRoleInfo (RoleInfo roleInfo)
		{
			if (expandRolesBagButton.gameObject.activeSelf == true)
				expandRolesBagButton.gameObject.SetActive(false);

			this.RoleInfo = roleInfo;
			if (this.RoleInfo == null)
			{
				if (contentRoot.activeSelf == true)
					contentRoot.SetActive(false);
				return;
			}
			else if (this.RoleInfo is HeroInfo)
			{
				contentRoot.SetActive(true);
				HeroInfo heroInfo = this.RoleInfo as HeroInfo;
				roleHeadIconImage.SetSprite(ResMgr.instance.Load<Sprite>(heroInfo.HeadIcon));
				levelText.text = string.Format(Localization.Get("common.role_icon.role_lv"), heroInfo.level);
				strengthenLevelText.text = RoleUtil.GetStrengthenAddShowValueString(heroInfo);
				int starImageCount = starImages.Count;
				bool shouldActiveStar = true;
				for (int i = 0; i < starImageCount; i++)
				{
					shouldActiveStar = heroInfo.advanceLevel > i;
					starImages[i].SetSprite(shouldActiveStar ? StarNormalSprite : StarDefaultSprite) ;
					starImages[i].gameObject.SetActive(	i < heroInfo.MaxAdvanceLevel);

				}
				newRoleMark.gameObject.SetActive(HeroProxy.instance.IsNewHero(this.RoleInfo.instanceID));
				roleTypeIconImage.SetSprite(UIUtil.GetRoleTypeSmallIconSprite(heroInfo.heroData.roleType));
			}
			SetInFormation(false);
			SetSelect(false);
		}

		public void SetAsExpandRolesBagButton()
		{
			if (expandRolesBagButton.gameObject.activeSelf == false)
				expandRolesBagButton.gameObject.SetActive(true);
			if (contentRoot.activeSelf == true)
				contentRoot.SetActive(false);
		}

		public void SetInFormation (bool inFormation)
		{
			if (inFormationMarkImage == null)
				return;
			if (inFormationMarkImage.gameObject.activeSelf != inFormation)
				inFormationMarkImage.gameObject.SetActive(inFormation);
		}

		public void SetSelect (bool select)
		{
			if (selectMarkImage == null)
				return;
			if (selectMarkImage.gameObject.activeSelf != select)
				selectMarkImage.gameObject.SetActive(select);
		}
	}
}