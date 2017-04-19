using UnityEngine;
using UnityEngine.UI;
using Common.Localization;
using Logic.VIP.Model;

namespace Logic.UI.VIP.View
{
	public class VIPPrivilegeDetailItem : MonoBehaviour
	{
		#region UI components
		public Text vipPrivilegeDetailText;
		#endregion UI components

		public void SetVIPData (VIPData vipData)
		{
			vipPrivilegeDetailText.text = Localization.Get(vipData.des);
		}
	}
}