using UnityEngine;
using System.Collections;
using Logic.Protocol.Model;

namespace Logic.UI.Main.Model
{
	public class MainViewProxy : SingletonMono<MainViewProxy> 
	{

		public System.Action UpdateEnterMainViewDelegate;

		void Awake()
		{
			instance = this;
		}

		void Start()
		{
			Observers.Facade.Instance.RegisterObserver(Logic.UI.MultipleFight.View.MultipleFightView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
			Observers.Facade.Instance.RegisterObserver(Logic.UI.DailyDungeon.View.DailyDungeonView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
			Observers.Facade.Instance.RegisterObserver(Logic.UI.SelectChapter.View.SelectChapterView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
			Observers.Facade.Instance.RegisterObserver(Logic.UI.IllustratedHandbook.View.IllustratedHandbookView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
			Observers.Facade.Instance.RegisterObserver(Logic.UI.BlackMarket.View.BlackMarketView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
			Observers.Facade.Instance.RegisterObserver(Logic.UI.Shop.View.ShopView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
			Observers.Facade.Instance.RegisterObserver(Logic.UI.Equipments.View.EquipmentsBrowseView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
			Observers.Facade.Instance.RegisterObserver(Logic.UI.PVEEmbattle.View.PVEEmbattleView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
//			Observers.Facade.Instance.RegisterObserver(Logic.UI.Role.View.RoleInfoView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
			Observers.Facade.Instance.RegisterObserver(Logic.UI.Player.View.PlayerInfoView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
			Observers.Facade.Instance.RegisterObserver(Logic.UI.Friend.View.FriendView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
			Observers.Facade.Instance.RegisterObserver(Logic.UI.Mail.View.MailView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
			Observers.Facade.Instance.RegisterObserver(Logic.UI.Task.View.TaskView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
			Observers.Facade.Instance.RegisterObserver(Logic.UI.Main.View.MainView.PREFAB_PATH,Observer_MainView_handler);
		}
		
		void OnDestroy()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver(Logic.UI.MultipleFight.View.MultipleFightView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
				Observers.Facade.Instance.RemoveObserver(Logic.UI.DailyDungeon.View.DailyDungeonView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
				Observers.Facade.Instance.RemoveObserver(Logic.UI.SelectChapter.View.SelectChapterView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
				Observers.Facade.Instance.RemoveObserver(Logic.UI.IllustratedHandbook.View.IllustratedHandbookView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
				Observers.Facade.Instance.RemoveObserver(Logic.UI.BlackMarket.View.BlackMarketView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
				Observers.Facade.Instance.RemoveObserver(Logic.UI.Shop.View.ShopView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
				Observers.Facade.Instance.RemoveObserver(Logic.UI.Equipments.View.EquipmentsBrowseView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
				Observers.Facade.Instance.RemoveObserver(Logic.UI.PVEEmbattle.View.PVEEmbattleView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
//				Observers.Facade.Instance.RemoveObserver(Logic.UI.Role.View.RoleInfoView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
				Observers.Facade.Instance.RemoveObserver(Logic.UI.Player.View.PlayerInfoView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
				Observers.Facade.Instance.RemoveObserver(Logic.UI.Friend.View.FriendView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
				Observers.Facade.Instance.RemoveObserver(Logic.UI.Mail.View.MailView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
				Observers.Facade.Instance.RemoveObserver(Logic.UI.Task.View.TaskView.PREFAB_PATH, LOBBY2CLIENT_GOBACK_MAIN_Handler);
				Observers.Facade.Instance.RemoveObserver(Logic.UI.Main.View.MainView.PREFAB_PATH,Observer_MainView_handler);
			}
		}

		private bool LOBBY2CLIENT_GOBACK_MAIN_Handler(Observers.Interfaces.INotification note)
		{
			if("close".Equals(note.Type))
			{
				if(UpdateEnterMainViewDelegate != null)
					UpdateEnterMainViewDelegate();
			}
			return true;
		}
		private bool Observer_MainView_handler(Observers.Interfaces.INotification note)
		{
			if("open".Equals( note.Type))
			{
				//if (FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(Enums.FunctionOpenType.MainView_Activity))
				//LuaInterface.ToLuaPb.SetFromLua((int)MSG.ActivityListReq);
				if (FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(Enums.FunctionOpenType.MainView_Chat))
					LuaInterface.ToLuaPb.SetFromLua((int)MSG.ChatInfoReq);
				if (FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(Enums.FunctionOpenType.MainView_Explore))
					LuaInterface.ToLuaPb.SetFromLua((int)MSG.ExploreListReq);
			}
			return true;
		}
	}
}

