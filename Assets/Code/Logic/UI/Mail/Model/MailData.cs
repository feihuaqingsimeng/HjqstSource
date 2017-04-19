using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;
using Logic.Game.Model;

namespace Logic.UI.Mail.Model
{
	public class MailData 
	{
		private static Dictionary<int, MailData> _mailDataDictionary;
		
		public static Dictionary<int, MailData> GetMailDatas()
		{
			if (_mailDataDictionary == null)
			{
				_mailDataDictionary = CSVUtil.Parse<int, MailData>("config/csv/mail", "id");
			}
			return _mailDataDictionary;
		}
		
		public static Dictionary<int, MailData> MailDataDictionary
		{
			get
			{
				if (_mailDataDictionary == null)
				{
					GetMailDatas();
				}
				return _mailDataDictionary;
			}
		}
		public static MailData GetMailDataByID (int mailDataID)
		{
			MailData data = null;
			if (MailDataDictionary.ContainsKey(mailDataID))
			{
				data = MailDataDictionary[mailDataID];
			}
			return data;
		}

		[CSVElement("id")]
		public int id;

		[CSVElement("type")]
		public string type;

		[CSVElement("pic")]
		public string pic;

		[CSVElement("name")]
		public string name;

		[CSVElement("des")]
		public string des;

	}
}

