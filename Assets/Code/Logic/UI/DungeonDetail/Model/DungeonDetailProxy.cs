using UnityEngine;
using System.Collections;
using Logic.Dungeon.Model;
using System.Collections.Generic;
using Logic.Hero.Model;
using Logic.Enums;
using Logic.Chapter.Model;
using Logic.Game.Model;
using Common.Localization;
using Logic.Equipment.Model;
using Logic.Protocol.Model;
using Logic.UI.Tips.View;
using Logic.Item.Model;
using Logic.Item.Controller;
using LuaInterface;
using Logic.Team.Model;
using Logic.VIP.Model;

namespace Logic.UI.DungeonDetail.Model{
	public class DungeonDetailProxy : SingletonMono<DungeonDetailProxy> {


		public delegate void UpdateMopUpSuccessDelegate(int dungeonId,Dictionary<int, List<GameResData>> rewardDic);
		public UpdateMopUpSuccessDelegate onUpdateMopUpSuccessDelegate;

		public System.Action onBuySweepCouponsSuccessDelegate;

		private int _dungeonId;
		public DungeonInfo DungeonInfo
		{
			get
			{
				LuaTable infoLua = (LuaTable)DungeonProxy.instance.DungeonModelLuaTable.GetLuaFunction("GetDungeonInfo").Call(_dungeonId)[0];
				return new DungeonInfo(infoLua);
			}
			set{
				_dungeonId = value.id;
			}
		}
		private List<HeroInfo> _monstersList;

		private List<GameResData> _dropRewardList;

		public SweepType lastCheckSweepType = SweepType.Single;

		void Awake(){
			instance = this;
			_monstersList = new List<HeroInfo>();
			_dropRewardList = new List<GameResData>();
		}

		public bool canStartFight()
		{

			if(GameProxy.instance.PveAction-DungeonDetailProxy.instance.DungeonInfo.dungeonData.actionNeed < 0 )
			{
				string tipString = Localization.Get("ui.common_tips.not_enough_pve_action_and_go_to_buy");
				ConfirmTipsView.Open(tipString, GoToBuyAction);
				return false;
			}
			if (!DungeonDetailProxy.instance.DungeonInfo.hasChallengeTimes())
			{
				CommonErrorTipsView.Open(Localization.Get("ui.common_tips.not_enough_fight_times"));
				return false;
			}
			return true;
		}

		public void GoToBuyAction ()
		{
			FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_Action);
		}

		public bool StartFight(){
			if(canStartFight()){

				Logic.Dungeon.Controller.DungeonController.instance.CLIENT2LOBBY_PVE_FIGHT_REQ(DungeonDetailProxy.instance.DungeonInfo.dungeonData.dungeonID);
				return true;
			}else{
				return false;
			}


		}
		public List<HeroInfo> GetMonstersList(){

			return DungeonInfo.dungeonData.heroPresentList;

		}
		public List<GameResData> GetDropRewardList(){

			_dropRewardList = DungeonInfo.dungeonData.eachLootPresent;

			return _dropRewardList;
		}
		public int GetSweepTimes()
		{
			DungeonInfo info = DungeonDetailProxy.instance.DungeonInfo;
			int count = 10;
			int countByAction = 0;
			if (info.dungeonData.actionNeed == 0) {
				countByAction = 99;
			}else
			{
				countByAction = (int)(GameProxy.instance.PveAction/info.dungeonData.actionNeed);
			}
			int countByLimit = info.GetRemindChallengeTimes();
			count = Mathf.Min(count,countByAction);
			count = Mathf.Min(count,countByLimit);
			return count;
		}

		public void GoToBuyDiamond ()
		{
			Logic.FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_Diamond);
		}

		public bool CheckSweep(bool isTen)
		{
			lastCheckSweepType = isTen ? SweepType.Ten : SweepType.Single;
			if(isTen)
			{
				if(!Logic.FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.TenSweep,false))
				{
					FunctionOpen.Model.FunctionData sweepTenOpenData = FunctionOpen.Model.FunctionData.GetFunctionDataByID((int)FunctionOpenType.TenSweep);
					Logic.VIP.Model.VIPData vipData = Logic.VIP.Model.VIPData.GetVIPData(sweepTenOpenData.vip);
					if (VIPProxy.instance.VIPLevel < sweepTenOpenData.vip)
					{
						string tipsString = Localization.Format("ui.dungeon_detail_view.sweep_ten_times_locked_tips", vipData.ShortName);
						ConfirmTipsView.Open(tipsString, GoToBuyDiamond);
						return false;
					}

				}
			}else
			{
				if(!Logic.FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.SingleSweep,true))
				{
					return false;
				}
			}



			if(DungeonDetailProxy.instance.DungeonInfo.star < 3)
			{
				CommonAutoDestroyTipsView.Open(Localization.Get("ui.dungeon_detail_view.text_sweep_not_enough_star"));
				return false;
			}
			if(GameProxy.instance.CheckPackFull())
			{
				return false;
			}

			DungeonInfo info = DungeonDetailProxy.instance.DungeonInfo;
			int count = 1;//默认等于10改为1下面判断是扫荡10次才变值
			int countByAction = (int)(GameProxy.instance.PveAction/info.dungeonData.actionNeed);
			if(countByAction == 0)
			{
				string tipString = Localization.Get("ui.common_tips.not_enough_pve_action_and_go_to_buy");
				ConfirmTipsView.Open(tipString, GoToBuyAction);
				return false;
			}
			int countByLimit = info.GetRemindChallengeTimes();
			if(countByLimit == 0)
			{
				CommonAutoDestroyTipsView.Open(Localization.Get("ui.dungeon_detail_view.challengeTimesNotEnoughTip"));
				return false;
			}
			count = Mathf.Min(count,countByAction);
			count = Mathf.Min(count,countByLimit);
			if (isTen){
				count = count <= 1 ? 10: count;
			}

			GameResData ticket = GlobalData.GetGlobalData().sweepTicket;
			GameResData buyTicketCost = GlobalData.GetGlobalData().buy_sweep_cost;
			int num = ItemProxy.instance.GetItemCountByItemID(ticket.id);
			int buyCount = count;
			int needCost = buyTicketCost.count*buyCount;
			int cost = buyCount - num;
			if(cost > 0 )
			{
				string tip = string.Format(Localization.Get("ui.dungeon_detail_view.text_sweep_not_enough_tip"),buyCount);
				ConfirmCostTipsView.Open(new GameResData(BaseResType.Diamond,0,needCost,0), tip,null,()=>{
					ItemController.instance.CLIENT2LOBBY_BuySweepCouponsReq(buyCount);
				});
				return false;
			}
			return true;
		}

		#region update by protocol
		public void UpdateMopUpSuccessByProtocol(int dungeonId,List<DropItem> itemList)
		{
			if(onUpdateMopUpSuccessDelegate != null)
			{
				List<GameResData> rewardList = new List<GameResData>();
				DropItem item;
				for(int i = 0,count = itemList.Count;i<count;i++)
				{
					item = itemList[i];
					GameResData data = new GameResData((BaseResType)item.itemType,item.itemNo,item.itemNum,item.heroStar);
					rewardList.Add(data);
				}
				Dictionary<int ,List<GameResData>> rewardDic = new Dictionary<int, List<GameResData>>();
				rewardDic.Add(1,rewardList);
				onUpdateMopUpSuccessDelegate(dungeonId,rewardDic);
			}

		}
		public void UpdateMopUpSuccessByProtocol(int dungeonId,List<PveMopUpOnceDropProto> itemList)
		{
			if(onUpdateMopUpSuccessDelegate != null)
			{
				Dictionary<int ,List<GameResData>> rewardDic = new Dictionary<int, List<GameResData>>();
				DropItem item;
				for(int j = 0,count2 = itemList.Count;j<count2;j++)
				{
					List<GameResData> rewardList = new List<GameResData>();
					PveMopUpOnceDropProto proto = itemList[j];
					for(int i = 0,count = proto.dropItems.Count;i<count;i++)
					{
						item = proto.dropItems[i];
						GameResData data = new GameResData((BaseResType)item.itemType,item.itemNo,item.itemNum,item.heroStar);
						rewardList.Add(data);
					}
					rewardDic.Add(j+1,rewardList);
				}
				onUpdateMopUpSuccessDelegate(dungeonId,rewardDic);
			}
			
		}

		public void OnBuySweepCouponsSuccess ()
		{
			if (onBuySweepCouponsSuccessDelegate != null)
				onBuySweepCouponsSuccessDelegate();
		}
		#endregion
	}
}

