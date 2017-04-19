using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Logic.Enums;
using Logic.UI.DungeonDetail.Model;
using Logic.UI.FightResult.Model;
using Logic.Dungeon.Model;
using Logic.Fight.Model;
using Logic.UI.WorldTree.Model;
using Common.Localization;
using Logic.UI.Tips.View;
using Logic.UI.WorldTree.Controller;
using Logic.FunctionOpen.Model;


namespace Logic.UI.FightResult.View
{
	public class WorldTreeFightResultPanel : PveFightResultPanel 
	{
		protected override void InitShowComponent()
		{
			if(view.isWin)
			{
				view.btnWinBack.gameObject.SetActive(true);
				view.btnWinNext.gameObject.SetActive(true);
				DungeonData currentDungeonData = Logic.Fight.Model.FightProxy.instance.CurrentDungeonData;
				view.btnWinNext.gameObject.SetActive(currentDungeonData.unlockDungeonIDNext1 > 0);

			}else
			{
				view.btnFailedBack.gameObject.SetActive(true);
				view.btnFailedBack.transform.localPosition = Vector3.zero;
			}
		}

		protected override void FightResultQuit(FightResultQuitType type)
		{
			FightResultProxy.instance.GotoMainScene(type, FightResultProxy.instance.QuitWorldTreeCallback);
		}

		#region click event
		//-------------胜利
		protected override void ClickWinBackHandler()
		{
			view.CloseDropItemAction(FightResultQuitType.Go_WorldTree,FightResultQuit);
		}

		protected override void ClickWinNextHandler()
		{
			int nextDungeonDataID = FightProxy.instance.CurrentDungeonData.unlockDungeonIDNext1;
			WorldTreeDungeonInfo nextWorldTreeDungeonInfo = WorldTreeProxy.instance.GetWorldTreeInfoByID(nextDungeonDataID);
			if (WorldTreeProxy.instance.WorldTreeFruit <= 0)
			{
				ConfirmTipsView.Open(Localization.Get("ui.world_tree_preview_view.tips.out_of_fruit_description"), ()=>{
					FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_Action);
				});
				return;
			}
			
			if (nextWorldTreeDungeonInfo.worldTreeDungeonStatus == WorldTreeDungeonStatus.Locked)
			{
				CommonAutoDestroyTipsView.Open(Localization.Get("ui.world_tree_preview_view.tips.dungeon_locked"));
				return;
			}
			if (nextWorldTreeDungeonInfo.worldTreeDungeonStatus == WorldTreeDungeonStatus.Passed)
			{
				CommonAutoDestroyTipsView.Open(Localization.Get("ui.world_tree_preview_view.tips.dungeon_passed"));
                return;
            }
            WorldTreeController.instance.CLIENT2LOBBY_WORLD_TREE_CHALLENGE_REQ(nextWorldTreeDungeonInfo.dungeonID);
            
		}
		//--------------失败
		protected override void ClickFailBackHandler()
		{
			FightResultQuit(FightResultQuitType.Go_WorldTree);
		}

		protected override void  ClickHeroBtnHandler()
		{
			FightResultQuit(FightResultQuitType.Go_Hero);
		}
		
		protected override void  ClickEquipBtnHandler()
		{
			FightResultQuit(FightResultQuitType.Go_Equip);
		}
		
		protected override void ClickShopBtnHandler()
		{
			FightResultQuit(FightResultQuitType.Go_Shop);
		}
		public override void OnAutoGoMainViewHandler()
		{
			if(view.isWin)
				ClickWinBackHandler();
			else
				ClickFailBackHandler();
		}
		#endregion
	}
}

