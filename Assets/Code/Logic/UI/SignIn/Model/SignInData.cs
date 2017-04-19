using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;
using Logic.Game.Model;

namespace Logic.UI.SignIn.Model
{
	public class SignInData 
	{
		private static Dictionary<int, SignInData> _SignInDataDictionary;
		
		public static Dictionary<int, SignInData> GetSignInDatas()
		{
			if (_SignInDataDictionary == null)
			{
				_SignInDataDictionary = CSVUtil.Parse<int, SignInData>("config/csv/sign", "sign_id");
			}
			return _SignInDataDictionary;
		}
		
		public static Dictionary<int, SignInData> SignInDataDictionary
		{
			get
			{
				if (_SignInDataDictionary == null)
				{
					GetSignInDatas();
				}
				return _SignInDataDictionary;
			}
		}
		public static SignInData GetSignInDataByID (int id)
		{
			SignInData data = null;
			if (SignInDataDictionary.ContainsKey(id))
			{
				data = SignInDataDictionary[id];
			}
			return data;
		}
		[CSVElement("sign_id")]
		public int sign_id;

		[CSVElement("vip_lv")]
		public int vip_lv;

		[CSVElement("vip_multiple")]
		public int vip_multiple;

		public List<GameResData> awardItemList = new List<GameResData>();
		[CSVElement("award_item")]
		public string award_item
		{
			set
			{
				awardItemList.Clear();
				string[] s = value.Split(CSVUtil.SYMBOL_SEMICOLON);
				for(int i = 0,count = s.Length;i<count;i++)
				{
					awardItemList.Add(new GameResData(s[i]));
				}
			}
		}

	}
}

