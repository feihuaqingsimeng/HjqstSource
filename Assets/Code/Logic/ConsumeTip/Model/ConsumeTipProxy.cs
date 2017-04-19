using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Enums;
using Common.Util;
using System.Text;
using Logic.Game.Model;
using LuaInterface;

namespace Logic.ConsumeTip.Model
{
	public class ConsumeTipProxy : SingletonMono<ConsumeTipProxy>
	{
		
		void Awake()
		{
			instance = this;
		}
		private Dictionary<ConsumeTipType , bool> _consumeTipDictionary = new Dictionary<ConsumeTipType, bool>();

		private Dictionary<ConsumeTipType , bool> ConsumeTipDictionary
		{
			get
			{
				if(_consumeTipDictionary.Count == 0)
				{
					Dictionary<int, ConsumeTipData> dataDic = ConsumeTipData.ConsumeTipDataDictionary;
					foreach(var data in dataDic)
					{
						_consumeTipDictionary.Add((ConsumeTipType)data.Key,true);
					}
					
					string s = PlayerPrefs.GetString(string.Format("consumeTip{0}",GameProxy.instance.AccountName));
					//Debugger.Log(string.Format("c#consumeTip{0},{1}",GameProxy.instance.AccountName,s));
					string[] consumeDataString = s.Split(CSVUtil.SYMBOL_SEMICOLON);
					for(int i = 0,count = consumeDataString.Length;i<count;i++)
					{
						int[] consume = consumeDataString[i].ToArray<int>(CSVUtil.SYMBOL_FIELD);
						if(consume.Length == 2)
						{
							ConsumeTipType type =  (ConsumeTipType)consume[0];
							if(_consumeTipDictionary.ContainsKey(type))
							{
								_consumeTipDictionary[type] = consume[1] != 0 ;
							}
						}
					}
				}
				return _consumeTipDictionary;
			}

		}
		private void Save()
		{
			LuaTable table = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "consume_tip_model")[0];
			table.GetLuaFunction("Save").Call();
//			StringBuilder s = new StringBuilder();
//			foreach(var data in _consumeTipDictionary)
//			{
//				s.Append(";");
//				s.Append((int)data.Key);
//				s.Append(",");
//				s.Append(data.Value ? 1 : 0);
//			}
//			s.Remove(0,1);
//			PlayerPrefs.SetString(string.Format("consumeTip{0}",GameProxy.instance.AccountName),s.ToString());
		}
		public bool HasConsumeTipKey(ConsumeTipType type)
		{
			return ConsumeTipDictionary.ContainsKey(type);
		}
		public bool GetConsumeTipEnable(ConsumeTipType type)
		{
		
			if(ConsumeTipDictionary.ContainsKey(type))
			{
				return ConsumeTipDictionary[type];
			}
			return true;
		}
		public void SetConsumeTipEnable(ConsumeTipType type,bool value)
		{
			if(type == ConsumeTipType.None)
				return;
			LuaTable table = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "consume_tip_model")[0];
			table.GetLuaFunction("SetConsumeTipEnable").Call((int)type,value);

			if(ConsumeTipDictionary.ContainsKey(type))
			{
				ConsumeTipDictionary[type] = value;
				//Save();
			}

		}
	}
}

