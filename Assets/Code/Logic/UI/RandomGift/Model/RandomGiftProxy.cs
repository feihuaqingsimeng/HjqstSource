using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Item.Model;
using Logic.Game.Model;
using Logic.Enums;
using Logic.Protocol.Model;

namespace Logic.UI.RandomGift.Model
{
	public class RandomGiftProxy : SingletonMono<RandomGiftProxy> 
	{
		
		void Awake()
		{
			instance = this;
		}
		
		public System.Action< List<GameResData> > GetRandomGiftRefreshDelegate; 
		public System.Action RefreshDelegate;
		private Dictionary<int, ItemInfo> _randomGiftDictionary = new Dictionary<int, ItemInfo>();
		
		public Dictionary<int, ItemInfo> RandomGiftDictionary
		{
			get
			{
				return _randomGiftDictionary;
			}
		}

		public void UpdateRandomGift()
		{
			_randomGiftDictionary.Clear();
			Dictionary<int,ItemInfo> itemDic = ItemProxy.instance.GetAllItemInfoDictioary();
			foreach(var value in itemDic)
			{
				if(value.Value.itemData.type == (int)ITEM_TYPE.RandomGift)
				{
					_randomGiftDictionary.Add(value.Key,value.Value);
				}
			}
			RefreshGiftByProtocol();
		}
		private void RefreshGiftByProtocol()
		{
			if(RefreshDelegate!= null)
				RefreshDelegate();
		}
		public void RandomGiftRefreshByProtocol(List<DropItem> dataList)
		{
			if(GetRandomGiftRefreshDelegate!= null)
			{
				List<GameResData> resList = new List<GameResData>();
				for(int i = 0,count = dataList.Count;i<count;i++)
				{
					resList.Add(new GameResData(dataList[i]));
				}
				GetRandomGiftRefreshDelegate(resList);
			}
		}
	}
}

