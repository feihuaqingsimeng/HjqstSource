using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Logic.UI.ServerList.Model;
using Common.Localization;
using Logic.Enums;

namespace Logic.UI.ServerList.View
{
	public class ServerButton : MonoBehaviour 
	{
		
		public Text nameText;


		public ServerInfo serverInfo;

		public void SetServerInfo(ServerInfo info)
		{
			serverInfo = info;
			Refresh();
		}
		private void Refresh()
		{
			nameText.text = serverInfo.description;
		}



	}
}

