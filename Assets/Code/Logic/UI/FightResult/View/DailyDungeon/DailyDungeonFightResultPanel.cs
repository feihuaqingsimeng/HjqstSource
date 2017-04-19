using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Logic.UI.FightResult.View;
using Logic.Game.Model;
using Logic.Enums;
using Logic.UI.FightResult.Model;
using Common.Localization;
using Logic.Activity.Model;


namespace Logic.UI.FightResult.View
{
	public class DailyDungeonFightResultPanel : FightResultBasePanel 
	{
		public Button btn_fullscreen_back ;
		public Text text_baseRes;
		public Text text_baseRes_des;

		public override void Init()
		{
			Transform viewTransform = view.transform;
			btn_fullscreen_back = viewTransform.Find("core/extra_root/btn_fullscreen_back").GetComponent<Button>();
			text_baseRes = viewTransform.Find("core/win/mid/text_total_baseRes").GetComponent<Text>();
			text_baseRes_des = viewTransform.Find("core/win/mid/text_total_baseRes_des").GetComponent<Text>();
			text_baseRes_des.text = Localization.Get("ui.fightResultView.text_total_money_des");
			if(view.isWin)
			{
				float heroDelay = (view.starNum+1) * 0.3f;
				ShowTotalMoney(heroDelay + 0.3f);
				StartCoroutine(view.ShowDropBoxCoroutine(2 + heroDelay));
			}
			InitShowComponent();
			InitClickEvent();
		}
		protected virtual void InitShowComponent()
		{
			if(view.isWin)
			{
				view.btnWinBack.gameObject.SetActive(ActivityProxy.instance.fixedRewardGameResDataList.Count > 0);
				view.btnWinBack.transform.localPosition = new Vector3(0, view.btnWinBack.transform.localPosition.y, view.btnWinBack.transform.localPosition.z);
				btn_fullscreen_back.gameObject.SetActive(false);
				if (! view.hasReward)
				{
					StartCoroutine(ShowFullscreenCloseBtnCoroutine(0.9f));
				}
			}else
			{
				btn_fullscreen_back.gameObject.SetActive(true);
			}
		}
		protected virtual void InitClickEvent()
		{
			btn_fullscreen_back.onClick.AddListener(ClickBackBtnHandler);
			view.btnWinBack.onClick.AddListener(ClickBackBtnHandler);
            view.btnFailedHero.onClick.AddListener(ClickHeroBtnHandler);
            view.btnFailedEquip.onClick.AddListener(ClickEquipBtnHandler);
            view.btnFailedShop.onClick.AddListener(ClickShopBtnHandler);
			view.btnFailedFormation.onClick.AddListener(ClickFormationBtnHandler);
			view.onBoxOpenCallback += ClickTreasureBoxHandler;
        }
		private IEnumerator ShowFullscreenCloseBtnCoroutine(float delay)
		{
			yield return new WaitForSeconds(delay);
			btn_fullscreen_back.gameObject.SetActive(true);
		}

		private void ShowTotalMoney(float delay = 0)
		{
			text_baseRes.transform.GetComponent<NumberIncreaseAction>().Init(0, FightResultProxy.instance.dropBaseResDictionary.GetValue(BaseResType.Gold), delay);
		}
		protected virtual void FightResultQuit(FightResultQuitType type)
		{
			FightResultProxy.instance.GotoMainScene(type, FightResultProxy.instance.QuitActivityCallback);
        }
		#region click event

		private void ClickTreasureBoxHandler()
		{
//			DailyDungeon.View.DailyDungeonWinView.Open();
		}

		protected virtual void ClickBackBtnHandler()
		{
			FightResultQuit(FightResultQuitType.Go_Activity);
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
			ClickBackBtnHandler();
		}
		#endregion
	}
}