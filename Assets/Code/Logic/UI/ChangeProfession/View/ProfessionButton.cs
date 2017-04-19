using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.ResMgr;
using Common.Localization;
using Logic.Player.Model;
using Logic.Game.Model;

namespace Logic.UI.ChangeProfession.View
{
	public class ProfessionButton : MonoBehaviour
	{
		private PlayerData _playerData;
		public PlayerData PlayerData
		{
			get
			{
				return _playerData;
			}
		}

		#region UI components
		public Text professionNameText;
		public Image professionPortraitImage;
		public Text activateStatusText;
		#endregion

		public void SetPlayerData (PlayerData playerData)
		{
			_playerData = playerData;
			professionNameText.text = Localization.Get(_playerData.heroData.name);
			professionPortraitImage.SetSprite(ResMgr.instance.Load<Sprite>(ResPath.GetPlayerPortraitPath(_playerData.portrait)));
			if (_playerData.Id == GameProxy.instance.PlayerInfo.playerData.Id)
			{
				activateStatusText.text = Localization.Get("ui.change_profession_view.is_selected");
			}
			else
			{
				if (PlayerProxy.instance.IsPlayerUnlocked((int)_playerData.Id))
				{
					activateStatusText.text = Localization.Get("ui.change_profession_view.is_activated");
				}
				else
				{
					activateStatusText.text = Localization.Get("ui.change_profession_view.is_not_activated");
				}
			}
		}
	}
}
