using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;
using Logic.Protocol.Model;
using Logic.Enums;
using LuaInterface;

namespace Logic.Game.Model
{
	public class GameResData
	{
		public BaseResType type;
		public int id;
		public int count;
		public int star;

		public GameResData(){}

		public GameResData (string gameResStr)
		{

			int[] values = gameResStr.ToArray<int>(CSVUtil.SYMBOL_COLON);
			if(values!= null && values.Length ==4 )
			{
				type = (BaseResType)values[0];
				id = values[1];
				count = values[2];
				star = values[3];
			}else
			{
				Debugger.LogError("[error]indexOutOfRangeException,gameResStr:"+gameResStr);
			}

		}
		public GameResData(BaseResType type,int id,int resCount,int star){
			set (type,id,resCount,star);
		}
		public GameResData(int type,int id,int resCount,int star){
			set ((BaseResType)type,id,resCount,star);
		}
		public void set(BaseResType type,int id,int resCount,int star){
			this.type = type;
			this.id = id;
			this.count = resCount;
			this.star = star;
		}
		[NoToLua]
		public GameResData (DropItem dropItem)
		{
			set((BaseResType)dropItem.itemType, dropItem.itemNo, dropItem.itemNum, dropItem.heroStar);
		}
		[NoToLua]
		public static List<GameResData> ParseGameResDataList(string gameResDataListStr)
		{
			List<GameResData> gameResDataList = new List<GameResData>();
			List<string> gameResDataStrList = gameResDataListStr.ToList(CSVUtil.SYMBOL_SEMICOLON);
			int gameResDataCount = gameResDataStrList.Count;
			for (int i = 0; i < gameResDataCount; i++)
			{
				gameResDataList.Add(new GameResData(gameResDataStrList[i]));
			}
			return gameResDataList;
		}
		public override string ToString ()
		{
			return string.Format ("[GameResData]type:{0},id:{1},count:{2},star:{3}",type,id,count,star);
		}
	}
}