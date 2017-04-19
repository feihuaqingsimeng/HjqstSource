using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;
using Logic.Game.Model;
using Logic.Enums;
using Logic.Dungeon.Model;

namespace Logic.UI.GoodsJump.Model
{
	public class DropMessageData  
	{
//		private static Dictionary<int, DropMessageData> _dropMsgDataDictionary;
		private static Dictionary<string, DropMessageData> _dropMsgDataDictionary;
		
		private static Dictionary<string, DropMessageData> GetDropMsgDatas()
		{
			if (_dropMsgDataDictionary == null)
			{
				Dictionary<int, DropMessageData> dropMsgDataDic = CSVUtil.Parse<int, DropMessageData>("config/csv/drop_message", "id");

				string s ;
				GameResData resData;
				_dropMsgDataDictionary = new Dictionary<string, DropMessageData>();
				foreach(var value in dropMsgDataDic)
				{
					resData = value.Value.itemData;
					s = string.Format("{0},{1},{2},{3}",(int)resData.type,resData.id,resData.count,resData.star);
					_dropMsgDataDictionary[s] = value.Value;
				}
			}
			return _dropMsgDataDictionary;
		}
		
		private static Dictionary<string, DropMessageData> DropMsgDataDictionary
		{
			get
			{
				if (_dropMsgDataDictionary == null)
				{
					GetDropMsgDatas();
				}
				return _dropMsgDataDictionary;
			}
		}
		public static DropMessageData GetDropMsgDataByResData(GameResData resData,bool includeDefault = false)
		{
			DropMessageData data =  GetDropMsgDataByResData((int)resData.type,resData.id,resData.count,resData.star);
			if (data == null && includeDefault)
			{
				data =  GetDropMsgDataByResData((int)resData.type,resData.id,0,0);
			}
			return data;
		}
		public static DropMessageData GetDropMsgDataByResData(int type,int modelId,int count = 0,int star = 0)
		{
			string s = string.Format("{0},{1},{2},{3}",type,modelId,count,star);
			if(DropMsgDataDictionary.ContainsKey(s))
			{
				return DropMsgDataDictionary[s];
			}
			return null;
		}


		public static List<PathData> GetPathDatas(GameResData gameResData,int limitCount)
		{
			List<PathData> pathDataList = new List<PathData>();
			DropMessageData data = DropMessageData.GetDropMsgDataByResData(gameResData,true);
			if(data == null)
			{
				return null;
			}else
			{
				Dictionary<GoodsJumpType, List<int>> jumpDic = data.JumpDic;
				int index = 0;
				foreach(var value in jumpDic)
				{
					if(value.Key == GoodsJumpType.Jump_Dungeon)
					{
						List<int > idList = value.Value;
						int id;
						for(int i = 0;i < idList.Count ;i++)
						{
							id = idList[i];
							int remaindCount = limitCount-index;
							DungeonInfo dInfo = DungeonProxy.instance.GetDungeonInfo(id);
							if(dInfo == null)
							{
								Debugger.LogError("dungeon can not find in jump path, id:"+id);
							}else if(!dInfo.isLock||i < remaindCount)
							{
								pathDataList.Add(new PathData(value.Key,id));
								index++;
								if(index == limitCount)
									break;
							}
						}
					}else
					{
						List<int > idList = value.Value;
						int id;
						for(int i = 0;i < idList.Count ;i++)
						{
							id = idList[i];
							pathDataList.Add(new PathData(value.Key,id));
							index++;
							if(index == limitCount)
								break;
						}

					}
				}
			}
			return pathDataList;
		}

		private Dictionary<GoodsJumpType,List<int>> _jumpDic;

		public Dictionary<GoodsJumpType,List<int>> JumpDic
		{
			get
			{
				if(_jumpDic == null)
				{
					_jumpDic = new Dictionary<GoodsJumpType, List<int>>();
					int len = jump_page.Length;
					int type;
					List<int> pageList;
					for(int i = 0;i<len;i++)
					{
						type = jump_type[i];
						if(type != 0)
						{
							string[] page = jump_page[i].Split(';');
							pageList = new List<int>();
							for(int j = 0,count = page.Length;j<count;j++)
							{
								pageList.Add(page[j].ToInt32());
							}
							if(_jumpDic.ContainsKey((GoodsJumpType)type))
							{
								//Debugger.Log(string.Format("[error],drop_message表 id:{0}, jump path has same key:{1},     please 策划同学 <color=#ff0000>干掉它</color> !!!!!!!!!",id,(GoodsJumpType)type));
								_jumpDic[(GoodsJumpType)type].AddRange(pageList);
							}else
							{
								_jumpDic.Add((GoodsJumpType)type,pageList);
							}
						}
					}
				}
				return _jumpDic;
			}
		}

		[CSVElement("id")]
		public int id;

		public GameResData itemData;
		[CSVElement("item")]
		public string itemString
		{
			set
			{
				itemData = new GameResData(value);
			}
		}

		[CSVElement("des")]
		public string des;

		private int[] jump_type = new int[4];
		private string[] jump_page = new string[4];

		[CSVElement("jump_type1")]
		public string jump_type1
		{
			set
			{
				jump_type[0] = string.IsNullOrEmpty(value) ? 0 : value.ToInt32();
			}
		}

		[CSVElement("jump_page1")]
		public string jump_page1
		{
			set
			{
				jump_page[0] = value;
			}
		}

		[CSVElement("jump_type2")]
		public string jump_type2
		{
			set
			{
				jump_type[1] = string.IsNullOrEmpty(value) ? 0 : value.ToInt32();
			}
		}
		
		[CSVElement("jump_page2")]
		public string jump_page2
		{
			set
			{
				jump_page[1] = value;
			}
		}

		[CSVElement("jump_type3")]
		public string jump_type3
		{
			set
			{
				jump_type[2] = string.IsNullOrEmpty(value) ? 0 : value.ToInt32();
			}
		}
		
		[CSVElement("jump_page3")]
		public string jump_page3
		{
			set
			{
				jump_page[2] = value;
			}
		}

		[CSVElement("jump_type4")]
		public string jump_type4
		{
			set
			{
				jump_type[3] = string.IsNullOrEmpty(value) ? 0 : value.ToInt32();
			}
		}
		
		[CSVElement("jump_page4")]
		public string jump_page4
		{
			set
			{
				jump_page[3] = value;
			}
		}
	}
	public class PathData
	{
		public GoodsJumpType type;
		public int id;
		public PathData(GoodsJumpType type,int id)
		{
			this.type = type;
			this.id = id;
		}
	}
}

