using UnityEngine;
using System.Collections;
using Logic.UI.BlackMarket.Model;
using System.Collections.Generic;
using Logic.Game.Model;
using Logic.Enums;
using Logic.Protocol.Model;

namespace Logic.UI.BlackMarket.Model
{
	public class BlackMarketProxy : SingletonMono<BlackMarketProxy> 
	{

		public System.Action onUpdateAllBlackMarketDelegate;
		public System.Action onUpdateBlackMarketDelegate;
		public System.Action onUpdatePurchaseGoodsDelegate;

		Dictionary<int,BlackMarketInfo> _blackMarketInfoDictionary = new Dictionary<int, BlackMarketInfo>();
        public bool isLimitActivityOpen = true;
		public long limitActiavityRefreshTime;//ms
		public Dictionary<int,BlackMarketInfo> BlackMarketInfoDictionary
		{
			get
			{
				return _blackMarketInfoDictionary;
			}
		}


		public BlackMarketInfo selectBlackMarketInfo;
		public BlackMarketInfo buyBlackMarketInfo;
		public BlackMarketType selectType = BlackMarketType.BlackMarket_Hero;
		public List<BlackMarketInfo> currentBalckMarketInfoList;
		
		void Awake()
		{
			instance = this;
		}

		public List<BlackMarketInfo> GetExchangeList()
		{
			List<BlackMarketInfo> allInfo = _blackMarketInfoDictionary.GetValues();
            List<BlackMarketInfo> exchangeList = new List<BlackMarketInfo>();

			BlackMarketInfo info ;
			for(int i = 0,count = allInfo.Count;i<count;i++)
			{
				info = allInfo[i];
				if(info.type == selectType)
				{
					exchangeList.Add(info);
				}
			}
			currentBalckMarketInfoList = exchangeList;
			return exchangeList;
		}
		public BlackMarketInfo GetBlackMarketInfo(int id)
		{
			if(BlackMarketInfoDictionary.ContainsKey(id))
			{
				return BlackMarketInfoDictionary[id];
			}
			return null;
		}
		public void UpdateAllBlackMarketInfo(List<BlackMarketGoodsProto> goodsList)
		{
			BlackMarketInfoDictionary.Clear();
			int count = goodsList.Count;
			BlackMarketGoodsProto proto;
			for(int i = 0;i<count;i++)
			{
				proto = goodsList[i];
				UpdateBlackMarketInfo(proto);
			}
		}
		public void UpdateBlackMarketInfo(BlackMarketGoodsProto proto)
		{
			if(BlackMarketInfoDictionary.ContainsKey(proto.marketId))
			{
				BlackMarketInfo info = BlackMarketInfoDictionary[proto.marketId];
				info.Set(proto.goodsNo,proto.remainBuyTimes);
				if(info.marketData == null || info.ruleData == null)
				{
					BlackMarketInfoDictionary.Remove(proto.marketId);
				}
			}else
			{
				BlackMarketInfo info = new BlackMarketInfo(proto.marketId,proto.goodsNo,proto.remainBuyTimes);
				if(info.marketData != null && info.ruleData != null)
					BlackMarketInfoDictionary.Add(proto.marketId,info);
			}
		}
		public void UpdateAllBlackMarketDelegateByProtocol()
		{
			if(onUpdateAllBlackMarketDelegate!= null)
			{
				onUpdateAllBlackMarketDelegate();
			}
		}
		public void UpdateBlackMarketDelegateByProtocol()
		{
			if(onUpdateBlackMarketDelegate!= null)
			{
				onUpdateBlackMarketDelegate();
			}
		}
		public void UpdatePurchaseGoodsDelegateByProtocol()
		{
			if(onUpdatePurchaseGoodsDelegate != null)
			{
				onUpdatePurchaseGoodsDelegate();
			}
		}
	}
}

