using UnityEngine;
using UnityEngine.UI;
using Common.Localization;
using Logic.Player.Model;
using Logic.Hero.Model;
using Logic.Role;

namespace Logic.UI.CommonRoleTitle.View
{
	public class CommonRoleTitleView : MonoBehaviour
	{
		private PlayerInfo _playerInfo;
		private HeroInfo _heroInfo;

		public Text levelText;
		public Text nameText;
		public Image roleTypeIconImage;
		public GameObject[] stars;

		public void SetPlayerInfo (PlayerInfo playerInfo)
		{
			_playerInfo = playerInfo;
			levelText.text = _playerInfo.level.ToString();
//			nameText.text = RoleUtil.GetRoleNameWithStrengthenLevel(_playerInfo);
//			nameText.text = Logic.Game.Model.GameProxy.instance.PlayerInfo.name;
			nameText.text = Logic.Game.Model.GameProxy.instance.AccountName;
			roleTypeIconImage.SetSprite(UIUtil.GetRoleTypeBigIconSprite(_playerInfo.heroData.roleType));
			int starCount = stars.Length;
			for (int i = 0; i < starCount; i++)
			{
				stars[i].SetActive(i < _playerInfo.advanceLevel);
			}
		}

		public void SetHeroInfo (HeroInfo heroInfo)
		{
			_heroInfo = heroInfo;
			levelText.text = _heroInfo.level.ToString();
			nameText.text = RoleUtil.GetRoleNameWithStrengthenLevel(_heroInfo);
			roleTypeIconImage.SetSprite(UIUtil.GetRoleTypeBigIconSprite(_heroInfo.heroData.roleType));
			int starCount = stars.Length;
			for (int i = 0; i < starCount; i++)
			{
				stars[i].SetActive(i < _heroInfo.advanceLevel);
			}
		}
	}
}
