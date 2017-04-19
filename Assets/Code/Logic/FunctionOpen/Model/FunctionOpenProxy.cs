using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Enums;
using Logic.Game.Model;
using Logic.Dungeon.Model;
using Logic.Hero.Model;
using Logic.UI;
using Logic.Chapter.Model;
using Logic.UI.Tips.View;
using Common.Localization;
using Logic.Role.Model;
using Logic.VIP.Model;
using LuaInterface;
using Logic.UI.SoftGuide.View;

namespace Logic.FunctionOpen.Model
{
    public class FunctionOpenProxy : SingletonMono<FunctionOpenProxy>
    {
		private LuaTable _functioOpenModelLuaTable;
		public LuaTable FunctionOpenModelLuaTable
		{
			get
			{
				if (_functioOpenModelLuaTable == null)
					_functioOpenModelLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "function_open_model")[0];
				return _functioOpenModelLuaTable;
			}
		}

        void Awake()
        {
            instance = this;
			Observers.Facade.Instance.RegisterObserver(Logic.UI.Main.View.MainView.PREFAB_PATH,Observer_MainView_handler);
			Observers.Facade.Instance.RegisterObserver("EnterFight",Observer_EnterFight_handler);
			
			Observers.Facade.Instance.RegisterObserver("CloseUIView",Observer_CloseView_handler);
        }
		void OnDestroy()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver(Logic.UI.Main.View.MainView.PREFAB_PATH,Observer_MainView_handler);
				Observers.Facade.Instance.RemoveObserver("EnterFight",Observer_EnterFight_handler);
				Observers.Facade.Instance.RemoveObserver("CloseUIView",Observer_CloseView_handler);
			}
		}
        public bool IsFunctionOpen(FunctionOpenType type)
        {
            return IsFunctionOpen(type, false);
        }
        public bool IsFunctionOpen(FunctionOpenType type, bool showTip)
        {
			if ( LuaScriptMgr.Instance == null)
				return true;
			object isOpenTable = FunctionOpenModelLuaTable.GetLuaFunction("IsFunctionOpen").Call((int)type,showTip)[0];
			return (bool)isOpenTable;
        }


        public void OpenFunction(FunctionOpenType type, Object data = null)
        {
            OpenFunction(type, data, false, true);
        }
		public void OpenLuaView(FunctionOpenType type,params object[] param)
		{
			if ( LuaScriptMgr.Instance == null)
				return ;
			FunctionOpenModelLuaTable.GetLuaFunction("OpenFunctionByCSharp").Call((int)type,param);
        }
        /// <summary>
        /// Opens the function.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="data">Data. 界面数据需要：Hero_BreakThrough、Hero_Strengthen、Hero_Advance、Dungeon_Detail_View</param>
        /// <param name="loadParent">如果打开的是二级界面， 是否加载一级界面.</param>
        /// <param name="showTip">功能未开放，是否显示提示文字</param>
        public void OpenFunction(FunctionOpenType type, object data, bool loadParent = false, bool showTip = false)
        {
            if (!IsFunctionOpen(type, showTip))
                return ;
            
            if (loadParent)
                UI.UIMgr.instance.Close(Logic.UI.EUISortingLayer.MainUI);

            DoOpenFunction(type, data, loadParent);
        }
        private void DoOpenFunction(FunctionOpenType type, object data = null, bool loadParent = false)
        {

            UIMgr uiMgrInstance = UIMgr.instance;

            switch (type)
            {
                case FunctionOpenType.MainView:
                    Logic.UI.Main.View.MainView.Open();
                    break;
				case FunctionOpenType.MainView_Hero:		//主界面-英雄按钮
                case FunctionOpenType.RoleInfoView:
                    if (loadParent)
                        DoOpenFunction(FunctionOpenType.MainView, data, loadParent);
					OpenLuaView(FunctionOpenType.MainView_Hero);
                    break;
                case FunctionOpenType.PveEmbattleView:
					{
						if (loadParent)
                        	DoOpenFunction(FunctionOpenType.MainView, data, loadParent);
						LuaTable pveEmbattleCtrlLua = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl", "pve_embattle_controller")[0];
						pveEmbattleCtrlLua.GetLuaFunction("OpenPveEmbattleView").Call();
					}       
                    break;
                case FunctionOpenType.Shop_View:	//商店
                    {
                        if (loadParent)
                        	DoOpenFunction(FunctionOpenType.MainView, data, loadParent);
//                        Logic.UI.Shop.View.ShopView shopView = uiMgrInstance.Open<Logic.UI.Shop.View.ShopView>(Logic.UI.Shop.View.ShopView.PREFAB_PATH);
//						Logic.UI.Shop.View.ShopView.Open((int)data);
//                    	if (data != null)
//	                            shopView.SetTogglePanel((int)data);

						LuaTable shopControllerLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl", "shop_controller")[0];
						shopControllerLuaTable.GetLuaFunction("OpenShopView").Call();
                    }
                    break;
                case FunctionOpenType.Shop_Diamond:
                    {
                        if (loadParent)
                            DoOpenFunction(FunctionOpenType.MainView, data, loadParent);
						Logic.UI.Shop.View.ShopView.Open(3);
                    }
                    break;
                case FunctionOpenType.Shop_Action:
                    {
                        if (loadParent)
                            DoOpenFunction(FunctionOpenType.MainView, data, loadParent);
						Logic.UI.Shop.View.ShopView.Open(4);
                    }
                    break;
				case FunctionOpenType.Shop_Gold:
					{
						if (loadParent)
							DoOpenFunction(FunctionOpenType.MainView, data, loadParent);
						Logic.UI.Shop.View.ShopView.Open(5);
					}
					break;
                case FunctionOpenType.MainView_Equipment:	//主界面-装备按钮 是否可见
                case FunctionOpenType.Equipment_View:		//装备界面
                    if (loadParent)
                    {
                        DoOpenFunction(FunctionOpenType.MainView, data, loadParent);
                    }
                    uiMgrInstance.Open(Logic.UI.Equipments.View.EquipmentsBrowseView.PREFAB_PATH);
                    break;
                case FunctionOpenType.Player_Change_Profession://主角-转职
                    {
                        if (loadParent)
                        {
                            DoOpenFunction(FunctionOpenType.RoleInfoView, data, loadParent);
                        }
                        uiMgrInstance.Open(Logic.UI.ChangeProfession.View.ChangeProfessionView.PREFAB_PATH);
                    }
                    break;
				case FunctionOpenType.PlayerWearEquip:		//主角-穿戴
                    {
                        if (loadParent)
                        {
                            DoOpenFunction(FunctionOpenType.RoleInfoView, data, loadParent);
                        }
                        Logic.UI.Equipments.View.RoleEquipmentsView roleEquipmentsView = UIMgr.instance.Open<Logic.UI.Equipments.View.RoleEquipmentsView>(Logic.UI.Equipments.View.RoleEquipmentsView.PREFAB_PATH);
                        roleEquipmentsView.SetPlayerInfo(GameProxy.instance.PlayerInfo);
                    }
                    break;
                case FunctionOpenType.MainView_Formation:		//英雄1队
                    {
                        if (loadParent)
                        {
                            DoOpenFunction(FunctionOpenType.MainView, data, loadParent);
                        }
						LuaTable pveEmbattleCtrlLua = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl", "pve_embattle_controller")[0];
						pveEmbattleCtrlLua.GetLuaFunction("OpenPveEmbattleView").Call();
                    }
                    break;
                case FunctionOpenType.Dungeon_Detail_View:		//副本详情
                    {
						if (data == null)
							return;
						int id = (int)data;

                        if (loadParent)
                        {
                            DoOpenFunction(FunctionOpenType.Dungeon_SelectChapter_View, id, loadParent);
                        }
                        Logic.UI.DungeonDetail.View.DungeonDetailView.Open(id);

                    }
                    break;
				case FunctionOpenType.MainView_PVE:
				case FunctionOpenType.PVE_Normal:
				case FunctionOpenType.PVE_Hard:
                case FunctionOpenType.Dungeon_SelectChapter_View:
                    {
                        if (loadParent)
                        {
                            DoOpenFunction(FunctionOpenType.MainView, null, loadParent);
                        }
                        Logic.UI.SelectChapter.View.SelectChapterView chapter = uiMgrInstance.Open<Logic.UI.SelectChapter.View.SelectChapterView>(Logic.UI.SelectChapter.View.SelectChapterView.PREFAB_PATH);
						int dungeonID;
						if (data == null || (int)data == 0)
						{
							dungeonID = Logic.Dungeon.Model.DungeonProxy.instance.GetLastUnlockDungeonID(Dungeon.Model.DungeonProxy.instance.LastSelectPVEDungeonType);
						}
						else
						{
							dungeonID = (int)data;
						}
						chapter.SetSelectDungeon(dungeonID);
                    }
                    break;
				case FunctionOpenType.MainView_Task:
					{
						if(loadParent)
							DoOpenFunction(FunctionOpenType.MainView, null, loadParent);
						Logic.UI.Task.View.TaskView.Open();
					}
					break;
                case FunctionOpenType.MainView_Mail:					//邮件
                    if (loadParent)
                        DoOpenFunction(FunctionOpenType.MainView, null, loadParent);
                    Logic.UI.Mail.View.MailView.Open();
                    break;
                case FunctionOpenType.FightCenter_Arena:
                    {
                        if (loadParent)
                        {
                            DoOpenFunction(FunctionOpenType.MultpleFight_View, null, loadParent);
                        }
                        Logic.UI.Pvp.View.PvpView.Open();
                    }
                    break;
                case FunctionOpenType.MainView_DailyDungeon:
                    {
                        if (loadParent)
                        {
                            DoOpenFunction(FunctionOpenType.MainView, null, loadParent);
                        }
                        UIMgr.instance.Open(Logic.UI.DailyDungeon.View.DailyDungeonView.PREFAB_PATH);
                    }
                    break;
                case FunctionOpenType.Dungeon_Daily_Detail:
                    {
                        if (loadParent)
                        {
                            DoOpenFunction(FunctionOpenType.MainView_DailyDungeon, null, loadParent);
                        }
                        Logic.UI.DailyDungeon.View.DailyDungeonInfoView.Open(data as Logic.Activity.Model.ActivityInfo);
                    }
                    break;
                case FunctionOpenType.FightCenter_Expedition://远征
                    {
                        if (loadParent)
                        {
                            DoOpenFunction(FunctionOpenType.MultpleFight_View, null, loadParent);
                        }
                        Logic.UI.Expedition.View.ExpeditionView.Open();
                    }

                    break;
                case FunctionOpenType.MainView_BlackMarket://黑市
                    {
                        if (loadParent)
                            DoOpenFunction(FunctionOpenType.MainView, null, loadParent);
						OpenLuaView(FunctionOpenType.MainView_BlackMarket,data); 
                    }
                    break;
				case FunctionOpenType.MainView_FightCenter:
                case FunctionOpenType.MultpleFight_View://pvp、竞技场一级界面
                    {
                        if (loadParent)
                            DoOpenFunction(FunctionOpenType.MainView, null, loadParent);
                        Logic.UI.MultipleFight.View.MultipleFightView.Open();
                    }
                    break;
                case FunctionOpenType.FightCenter_WorldTree:
                    {
                        if (loadParent)
                            DoOpenFunction(FunctionOpenType.MainView, null, loadParent);
                        UIMgr.instance.Open(Logic.UI.WorldTree.View.WorldTreePreviewView.PREFAB_PATH);
                    }
                    break;
                case FunctionOpenType.MainView_WorldBoss:
                    {
                        if (loadParent)
                            DoOpenFunction(FunctionOpenType.MainView, null, loadParent);
                        UIMgr.instance.Open(Logic.UI.WorldBoss.View.WorldBossView.PREFAB_PATH);
                    }
                    break;

                case FunctionOpenType.MainView_illustrate:		//图鉴
                    {
                        if (loadParent)
                            DoOpenFunction(FunctionOpenType.MainView, null, loadParent);
						bool use = false;
						if(data != null)
							use = (bool)data;
						Logic.UI.IllustratedHandbook.View.IllustratedHandbookView.Open(use);
                    }
                    break;
                case FunctionOpenType.MainView_Friend:		//好友
                    {
                        if (loadParent)
                            DoOpenFunction(FunctionOpenType.MainView, null, loadParent);
                        Logic.UI.Friend.View.FriendView.Open();
                    }
                    break;
                case FunctionOpenType.MainView_SignIn:		//签到
                    if (loadParent)
                        DoOpenFunction(FunctionOpenType.MainView, null, loadParent);
                    Logic.UI.SignIn.View.SignInView.Open();
                    break;
				case FunctionOpenType.MainView_Player:	//主界面-主角按钮 是否可见
                case FunctionOpenType.PlayerInfoView:
                    if (loadParent)
                        DoOpenFunction(FunctionOpenType.MainView, null, loadParent);
                    UIMgr.instance.Open(Logic.UI.Player.View.PlayerInfoView.PREFAB_PATH);
                    break;
                case FunctionOpenType.PlayerTraining://使魔成长功能
                    if (loadParent)
                        DoOpenFunction(FunctionOpenType.PlayerInfoView, null, loadParent);
                    Logic.UI.Player.View.PlayerTalentView.Open();
                    break;

				case FunctionOpenType.Boss_List_View:
					{
						if (loadParent)
						{
							DoOpenFunction(FunctionOpenType.MainView_PVE, null, loadParent);
						}
						LuaTable chapterController = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl", "chapter_controller")[0];
						chapterController.GetLuaFunction("OpenBossDungeonListView").Call();
					}			
					break;
				default:
					if (data != null)
						OpenLuaView(type,data);
					else
						OpenLuaView(type);
					break;
            }
        }
		//------------------------------功能开放界面提示----------------------
		public void AddFunctionViewTipId(int id)
		{
			functionOpenTipIdList.Add(id);
			if(isMainViewOpen)
			{
				OpenFunctionViewTip();
			}
		}

		private List<int> functionOpenTipIdList = new List<int>();

		public void OpenFunctionViewTip()
		{
			FunctionOpenView.Open(functionOpenTipIdList);
			functionOpenTipIdList.Clear();
		}
		public void CloseFunctionViewTip()
		{
			functionOpenTipIdList.Clear();
			FunctionOpenView.Close();
		}
		//-------------------------------end-------------------------

		//-----------------------------主界面按钮动画展示----------------------
		private Dictionary<int, bool> functionOpenDicByMainView = new Dictionary<int, bool>();//开放功能id(主界面按钮动画展示用)
		public System.Action<List<int>> FunctionAnimationDelegate;
		public void AddFunctionAnimationTipId(int id)
		{
			bool isTipOver = functionOpenDicByMainView.GetValue(id,false);
			if (!isTipOver)
			{
				functionOpenDicByMainView[id] = false;
			}
		}

		public void OpenMainViewAnimation()
		{
			List<int> tip = new List<int>();
			foreach(var value in functionOpenDicByMainView) 
			{
				if(! value.Value)
				{
					tip.Add(value.Key);
				}
			}
			for(int i = 0,count = tip.Count;i<count;i++){
				functionOpenDicByMainView[tip[i]] = true;
			}
			if (tip.Count > 0 && FunctionAnimationDelegate != null)
			{
				FunctionAnimationDelegate(tip);
			}
		}
		//-------------------------------end-------------------------

		private bool isMainViewOpen = false;
		private bool Observer_MainView_handler(Observers.Interfaces.INotification note)
		{
			if("open".Equals( note.Type))
			{
				OpenFunctionViewTip();
				isMainViewOpen = true;
				isEnterFight = false;
			}else if("close".Equals(note.Type))
			{
				isMainViewOpen = false;
				CloseFunctionViewTip();
			}
			return true;
		}
		private bool Observer_CloseView_handler(Observers.Interfaces.INotification note)
		{
			//Debugger.Log("openCount:{0},enterFight:{1}",UIMgr.instance.GetOpenUICount(),isEnterFight);
			if (UIMgr.instance.GetOpenUICount() == 1 && !isEnterFight)
			{
				OpenMainViewAnimation();
			}
			return true;
		}
		private bool isEnterFight = false;
		private bool Observer_EnterFight_handler(Observers.Interfaces.INotification note)
		{
			isEnterFight = true;
			return true;
		}
    }

}
