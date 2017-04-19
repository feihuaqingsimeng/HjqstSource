using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;
using Logic.Enums;

namespace Logic.UI.ServerList.Model
{
	public class ServerData 
	{
		private static Dictionary<int, ServerData> _serverDataDictionary;
		
		public static Dictionary<int, ServerData> GetServerDatas()
		{
			if (_serverDataDictionary == null)
			{
				_serverDataDictionary = CSVUtil.Parse<int, ServerData>("config/csv/server", "sever_id");
			}
			return _serverDataDictionary;
		}
		
		public static Dictionary<int, ServerData> ServerDataDictionary
		{
			get
			{
				if (_serverDataDictionary == null)
				{
					GetServerDatas();
				}
				return _serverDataDictionary;
			}
		}
		public static ServerData GetServerDataByID (int mailDataID)
		{
			ServerData data = null;
			if (ServerDataDictionary.ContainsKey(mailDataID))
			{
				data = ServerDataDictionary[mailDataID];
			}
			return data;
		}
		[CSVElement("sever_id")]
		public int sever_id;

		[CSVElement("server_name")]
		public string server_name;

		[CSVElement("server_ip")]
		public string server_ip;

		[CSVElement("server_port")]
		public int server_port;

		[CSVElement("inner")]
		public int inner;//是否是内网，0不是，1是

		[CSVElement("server_state")]
		public int server_state;//服务器状态：0新服，1火爆，2维护

		public Dictionary<int,bool> platformIdDictionary = new Dictionary<int,bool>();
		[CSVElement("platform_id")] 
		public string platform_idStr//指定渠道开放 0代表无渠道版本[参考enum PlatformType]
		{
			set{
				string[] idList =  value.Split(';');
				for(int i = 0,count = idList.Length;i<count;i++)
				{
					platformIdDictionary.Add(idList[i].ToInt32(),true);
				}

			}
		}
	}
}

