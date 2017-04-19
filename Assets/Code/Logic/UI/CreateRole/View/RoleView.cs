using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Logic.Player.Model;

namespace Logic.UI.CreateRole.View
{
	public class RoleView : MonoBehaviour
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
		public Image image;
		#endregion

		public void Init (PlayerData playerData)
		{
			_playerData = playerData;
		}

		public void SetAsSelect ()
		{
			image.CrossFadeAlpha(1, 0.4f, true);
		}

		public void SetAsUnselect ()
		{
			image.CrossFadeAlpha(1, 0, true);
			image.CrossFadeAlpha(0.2f, 0.4f, true);
		}
	}
}