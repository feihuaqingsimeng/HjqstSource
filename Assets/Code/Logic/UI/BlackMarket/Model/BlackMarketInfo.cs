using UnityEngine;
using System.Collections;
using Logic.Hero.Model;
using Logic.Game.Model;
using System.Collections.Generic;
using Logic.Enums;
using Logic.Equipment.Model;
using Logic.Item.Model;
using Common.Localization;

namespace Logic.UI.BlackMarket.Model
{
	public class BlackMarketInfo 
	{
		public int id;
		public BlackMarketData marketData;
		public BlackMarketRuleData ruleData;
		public int remaindCount;

		public BlackMarketInfo (int marketDataId,int ruleDataId,int remainCount)
		{
			id = marketDataId;
			marketData = BlackMarketData.GetBlackMarketDataById(marketDataId);
			if(marketData == null)
				Debugger.LogError("[BlackMarketInfo]marketData is null ,id:"+marketDataId);
			Set(ruleDataId,remainCount);
		}
		public void Set(int ruleDataId,int remainCount)
		{
			ruleData = BlackMarketRuleData.GetBlackMarketRuleDataById(ruleDataId);
			if(ruleData == null)
				Debugger.LogError(string.Format("[BlackMarketInfo]ruleData is null ,marketid:{0},ruleid:{1}",id,ruleDataId));
			this.remaindCount = remainCount;
		}
		public string remaindCountString
		{
			get
			{
				if(limitType == BlackMarketLimitType.BlackMarketLimit_Person)
				{
					return string.Format(Localization.Get("ui.black_market_view.remainCount"),remaindCount);
				}
				if(limitType == BlackMarketLimitType.BlackMarketLimit_Server)
				{
					return string.Format(Localization.Get("ui.black_market_view.serverRemainCount"),remaindCount);
				}
				return string.Empty;
			}
		}

		public GameResData itemData
		{
			get
			{
				return ruleData.itemData;
				//return new GameResData("2:5:1:3");
			}
		}

		public List<GameResData> materials
		{
			get
			{
				return ruleData.materials;
//				List<GameResData> mat = new List<GameResData>();
//				mat.Add(new GameResData("4:10011:100:0"));
//				mat.Add(new GameResData("4:10012:100:0"));
//				return mat;
			}
		}
		public BlackMarketLimitType limitType
		{
			get
			{
				return (BlackMarketLimitType)marketData.limit_type;
				//return BlackMarketLimitType.BlackMarketLimit_Server;
			}
		}
		public BlackMarketType type
		{
			get
			{
				return (BlackMarketType)marketData.item_type;
				//return BlackMarketType.BlackMarket_Hero;
			}
		}
		public int limitLv
		{
			get
			{
				return marketData.limit_lv;
				//return 0;
			}
		}
		public bool onSell
		{
			get
			{
				return marketData.on_sell == 1? true:false;
				//return true;
			}
		}
	}
}

