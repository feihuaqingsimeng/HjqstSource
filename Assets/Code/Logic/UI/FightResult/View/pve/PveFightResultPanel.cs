using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Logic.UI.FightResult.View;
using Logic.Game.Model;
using Logic.UI.FightResult.Model;
using Common.Localization;
using Logic.Enums;
using Logic.UI.DungeonDetail.Model;
using Logic.Dungeon.Model;


namespace Logic.UI.FightResult.View
{
	public class PveFightResultPanel : FightResultBasePanel 
	{
		public Button btn_fullscreen_back ;
		public Text text_baseRes;
		public Text text_baseRes_des;
		private bool isClickStartFight;
		public override void Init()
		{
			Transform viewTransform = view.transform;
			text_baseRes = viewTransform.Find("core/win/mid/text_total_baseRes").GetComponent<Text>();
			text_baseRes_des = viewTransform.Find("core/win/mid/text_total_baseRes_des").GetComponent<Text>();
			text_baseRes_des.text = Localization.Get("ui.fightResultView.text_total_money_des");
			if(view.isWin)
			{
				float heroDelay = (view.starNum + 1) * 0.3f;
				ShowTotalMoney(heroDelay + 0.3f);
				StartCoroutine(view.ShowDropBoxCoroutine(2 + heroDelay));
			}
			InitShowComponent();
			InitClickEvent();
		}
		protected virtual void InitShowComponent()
		{
			DungeonData currentDungeonData = Logic.Fight.Model.FightProxy.instance.CurrentDungeonData;
			if(view.isWin)
			{
				view.btnWinBack.gameObject.SetActive(true);
				if (currentDungeonData.dungeonType == DungeonType.BossSubspecies)
				{
					view.btnWinBack.transform.localPosition = new Vector3(0, view.btnWinBack.transform.localPosition.y, view.btnWinBack.transform.localPosition.z);
					view.btnWinAgain.gameObject.SetActive(false);
				}
				else
				{
					view.btnWinAgain.gameObject.SetActive(true);
				}
				DungeonInfo nextInfo = DungeonProxy.instance.GetDungeonInfo(currentDungeonData.unlockDungeonIDNext1);
				bool visible = false;
				if(nextInfo != null && !nextInfo.isLock)
					visible = true;
				view.btnWinNext.gameObject.SetActive(visible);
				if(!visible)
				{
					view.btnWinAgain.transform.localPosition = view.btnWinNext.transform.localPosition;
				}
			}else
			{
				if (currentDungeonData.dungeonType == DungeonType.BossSubspecies)
				{
					view.btnFailedAgain.gameObject.SetActive(false);
					view.btnFailedBack.transform.localPosition = new Vector3(0, view.btnFailedBack.transform.localPosition.y, view.btnFailedBack.transform.localPosition.z);
				}
				else
				{
					view.btnFailedAgain.gameObject.SetActive(true);
				}
				Vector3 pos = view.btnFailedBack.transform.localPosition;
				view.btnFailedAgain.transform.localPosition = new Vector3(-pos.x,pos.y,pos.z);
				view.btnFailedBack.gameObject.SetActive(true);
			}
		}
		protected virtual void InitClickEvent()
		{
			view.btnWinBack.onClick.AddListener(ClickWinBackHandler);
			view.btnWinAgain.onClick.AddListener(ClickWinAgainHandler);
			view.btnWinNext.onClick.AddListener(ClickWinNextHandler);

			view.btnFailedAgain.onClick.AddListener(ClickFailAgainHandler);
			view.btnFailedBack.onClick.AddListener(ClickFailBackHandler);
			view.btnFailedHero.onClick.AddListener(ClickHeroBtnHandler);
			view.btnFailedEquip.onClick.AddListener(ClickEquipBtnHandler);
			view.btnFailedShop.onClick.AddListener(ClickShopBtnHandler);
			view.btnFailedFormation.onClick.AddListener(ClickFormationBtnHandler);
		}

		private void ShowTotalMoney(float delay = 0)
		{
			text_baseRes.transform.GetComponent<NumberIncreaseAction>().Init(0, FightResultProxy.instance.dropBaseResDictionary.GetValue(BaseResType.Gold), delay);
		}
		private IEnumerator EnableClickFightCoroutine() 
		{
			isClickStartFight = true;
			yield return new WaitForSeconds(3);
			isClickStartFight = false;
		}
        protected virtual void FightResultQuit(FightResultQuitType type)
		{
			FightResultProxy.instance.GotoMainScene(type, FightResultProxy.instance.QuitPveCallback);
		}

		#region click event
		//-------------胜利
		protected virtual void ClickWinBackHandler()
		{
			DungeonData currentDungeonData = Logic.Fight.Model.FightProxy.instance.CurrentDungeonData;
			if (currentDungeonData.dungeonType == DungeonType.BossSubspecies)
			{
				view.CloseDropItemAction(FightResultQuitType.GO_Boss_List, FightResultQuit);
			}
			else
			{
				view.CloseDropItemAction(FightResultQuitType.Go_Map, FightResultQuit);
			}
		}
		protected virtual void ClickWinAgainHandler()
		{
			if (isClickStartFight) 
				return;

            if (DungeonDetailProxy.instance.StartFight())
			{
				StartCoroutine(EnableClickFightCoroutine());
				view.CloseDropItemAction(FightResultQuitType.Fight_Again,FightResultQuit);
			}else
			{
				isClickStartFight = false;
			}
		}
		protected virtual void ClickWinNextHandler()
		{
			view.CloseDropItemAction(FightResultQuitType.Fight_Next_Dungeon,FightResultQuit);
		}
		//--------------失败
		protected virtual void ClickFailBackHandler()
		{
			DungeonData currentDungeonData = Logic.Fight.Model.FightProxy.instance.CurrentDungeonData;
			if (currentDungeonData.dungeonType == DungeonType.BossSubspecies)
			{
				FightResultQuit(FightResultQuitType.GO_Boss_List);
			}
			else
			{
				FightResultQuit(FightResultQuitType.Go_Map);
			}
		}
		protected virtual void ClickFailAgainHandler()
		{
			if (isClickStartFight) 
				return;

			if (DungeonDetailProxy.instance.StartFight())
			{
				StartCoroutine(EnableClickFightCoroutine());
				FightResultQuit(FightResultQuitType.Fight_Again);
			}else
			{
				isClickStartFight = false;
            }
		}
		protected virtual void  ClickHeroBtnHandler()
		{
			FightResultQuit(FightResultQuitType.Go_Hero);
		}

		protected virtual void  ClickEquipBtnHandler()
		{
			FightResultQuit(FightResultQuitType.Go_Equip);
		}

		protected virtual void ClickShopBtnHandler()
		{
			FightResultQuit(FightResultQuitType.Go_Shop);
		}
		protected virtual void ClickFormationBtnHandler()
		{
			FightResultQuit(FightResultQuitType.Go_Formation);
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