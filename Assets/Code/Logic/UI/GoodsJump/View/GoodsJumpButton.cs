using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Logic.Enums;
using Common.Localization;
using Logic.Dungeon.Model;
using Logic.FunctionOpen.Model;
using Logic.Activity.Model;
using System.Collections.Generic;
using Logic.Shop.Model;
using Logic.UI.BlackMarket.Model;
using Logic.UI.Tips.View;
using LuaInterface;

namespace Logic.UI.GoodsJump.View
{
	public class GoodsJumpButton : MonoBehaviour 
	{
		[NoToLua]
		public Text titleText;
		[NoToLua]
		public GameObject markGO;

		private GoodsJumpType _type = GoodsJumpType.None;
		private int _id;
		private bool _isButtonEnable = true;

		public void Set(int type,int id)
		{
			Set((GoodsJumpType) type,id);
		}
		[NoToLua]
		public void Set(GoodsJumpType type,int id)
		{
			_type = type;
			_id = id;
			if(type == GoodsJumpType.Jump_DailyDungeon){
				titleText.text =string.Format( Localization.Get("ui.goodsJumpPathView.path"+(int)type),Localization.Get( ActivityData.ActivityDataDictionary[_id].name));
			}else if(type ==  GoodsJumpType.Jump_Dungeon){
				DungeonInfo info = DungeonProxy.instance.GetDungeonInfo(id);
				if(info!= null)
				{
					DungeonType dungeonType =  info.dungeonData.dungeonType;
					string difficult  = string.Empty;
					if(dungeonType == DungeonType.Easy)
						difficult = Localization.Get("ui.select_chapter_view.easy_type");
					else if(dungeonType == DungeonType.Normal)
						difficult = Localization.Get("ui.select_chapter_view.normal_type");
					else 
						difficult = Localization.Get("ui.select_chapter_view.hard_type");
					titleText.text = string.Format(Localization.Get("ui.goodsJumpPathView.path1"),difficult,Localization.Get(info.dungeonData.order_name), Localization.Get( info.dungeonData.name));
				}else
				{
					Debugger.LogError("dungeonInfo is not find ; id:"+id);
				}
			}else if(type == GoodsJumpType.Jump_BlackMarket)
			{
				titleText.text = string.Format(Localization.Get("ui.goodsJumpPathView.path8"), Localization.Get(BlackMarketData.GetBlackMarketTitleNameByType(_id)));
			}
			else
			{
				titleText.text = Localization.Get("ui.goodsJumpPathView.path"+(int)type);
			}
			SetButtonEnable(CheckButtonEnable());
		}
		private void SetButtonEnable(bool value)
		{
			_isButtonEnable = value;
			markGO.SetActive(false);
		}
		private bool CheckButtonEnable()
		{
			bool enable = true;
			switch(_type)
			{
			case GoodsJumpType.Jump_Dungeon:		//副本掉落（多个副本id）
				enable = !DungeonProxy.instance.GetDungeonInfo(_id).isLock;
				break;
			case GoodsJumpType.Jump_WorldTree:		//世界树 功能页面
				enable = FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.FightCenter_WorldTree);
				break;
			case GoodsJumpType.Jump_Expedition:	//远征 功能页面
				enable = FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.FightCenter_Expedition);
				break;
			case GoodsJumpType.Jump_DailyDungeon:	//每日副本 类型+页面
				enable = FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.MainView_DailyDungeon) && CheckDailyDungeonOpen();
				break;
			case GoodsJumpType.Jump_Pvp:			//、PVP获得 功能页面
				enable = FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.FightCenter_Arena);
				break;
			case GoodsJumpType.Jump_Shop:			//、商店购买 对应页签
				enable = true;
				break;
			case GoodsJumpType.Jump_Sign :			//、签到获得 功能页面
				enable = FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.MainView_SignIn);
				break;
			case GoodsJumpType.Jump_BlackMarket:	//、黑市获得 对应页签
				enable = FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.MainView_BlackMarket);
				break;
			case GoodsJumpType.Jump_TakeCard :		//、商店抽卡获得 对应页签
				enable = true;
				break;
			case GoodsJumpType.Jump_Explore:     // 探险
				enabled = FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.MainView_Explore);
				break;
			case GoodsJumpType.Jump_ConsortiaShop:  // 公会商店
				enabled = FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.MainView_Consortia);
				break;
			case GoodsJumpType.Jump_Task:		//任务
				enable = FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.MainView_Task);;
				break;
			}
			return enable;
		}
		[NoToLua]
		public void OnClickButtonHandler()
		{

			switch(_type)
			{
			case GoodsJumpType.Jump_Dungeon:		//副本掉落（多个副本id）
				if(!_isButtonEnable)
				{
					CommonAutoDestroyTipsView.Open(Localization.Get("ui.goodsJumpPathView.notUnlock"));
					break;
				}
				FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Dungeon_Detail_View,_id);
				break;
			case GoodsJumpType.Jump_WorldTree:		//世界树 功能页面
				FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.FightCenter_WorldTree,null,false,true);
				break;
			case GoodsJumpType.Jump_Expedition:	//远征 功能页面
				FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.FightCenter_Expedition,null,false,true);
				break;
			case GoodsJumpType.Jump_DailyDungeon:	//每日副本 类型+页面
				HandleDailyDungeon();
				break;
			case GoodsJumpType.Jump_Pvp:			//、PVP获得 功能页面
				FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.FightCenter_Arena,null,false,true);
				break;
			case GoodsJumpType.Jump_Shop:			//、商店购买 对应页签
				HandleShop();
				break;
			case GoodsJumpType.Jump_Sign :			//、签到获得 功能页面
				FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_SignIn,null,false,true);
				break;
			case GoodsJumpType.Jump_BlackMarket:	//、黑市获得 对应页签
				FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_BlackMarket,_id,false,true);
				break;
			case GoodsJumpType.Jump_TakeCard :		//、商店抽卡获得 对应页签
				FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_View,null,false,true);
				break;
			case GoodsJumpType.Jump_Explore:          // 探险
				FunctionOpenProxy.instance.OpenLuaView(FunctionOpenType.MainView_Explore);
				break;
			case GoodsJumpType.Jump_ConsortiaShop:    // 公会商店
				LuaTable consortiaControllerLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl", "consortia_controller")[0];
				consortiaControllerLuaTable.GetLuaFunction("OpenConsortiaView").Call(4);
				break;
			case GoodsJumpType.Jump_Task:			//任务
				Logic.UI.Task.View.TaskView.Open();
				break;
			}
		}
		private bool CheckDailyDungeonOpen()
		{
			List<ActivityInfo> enabledActivityInfoList = ActivityProxy.instance.GetEnabledActivityInfoList();
			bool hasActivity = false;
			ActivityInfo info;
			for(int i = 0,count = enabledActivityInfoList.Count;i<count;i++)
			{
				info = enabledActivityInfoList[i];
				if(info.isOpen && info.ActivityID == _id)
				{
					return true;
				}
			}
			return false;
		}
		private void HandleDailyDungeon()
		{
			List<ActivityInfo> enabledActivityInfoList = ActivityProxy.instance.GetEnabledActivityInfoList();
			bool hasActivity = false;
			ActivityInfo info;
			for(int i = 0,count = enabledActivityInfoList.Count;i<count;i++)
			{
				info = enabledActivityInfoList[i];
				if(info.isOpen && info.ActivityID == _id)
				{
					if(info.remainChallengeTimes > 0)
						FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Dungeon_Daily_Detail,info);
					else
						CommonAutoDestroyTipsView.Open(Localization.Get("ui.goodsJumpPathView.notEnoughChallenge"));
					return;
				}
			}
			CommonAutoDestroyTipsView.Open(Localization.Get("ui.goodsJumpPathView.notOpen"));
		}
		private void HandleShop()
		{
			FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_View,_id,false,true);
		}

	}
}

