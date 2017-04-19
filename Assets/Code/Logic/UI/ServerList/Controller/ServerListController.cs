using UnityEngine;
using System.Collections;

namespace Logic.UI.ServerList.Controller
{
	public class ServerListController : SingletonMono<ServerListController> 
	{
		void Awake()
		{
			instance = this;
		}
	}
}

