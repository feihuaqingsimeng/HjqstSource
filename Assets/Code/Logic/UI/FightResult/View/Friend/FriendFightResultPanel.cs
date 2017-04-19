using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Logic.UI.FightResult.View;
using Logic.Game.Model;
using Logic.Enums;
using Logic.UI.FightResult.Model;
using Common.Localization;


namespace Logic.UI.FightResult.View
{
	public class FriendFightResultPanel : FightResultBasePanel 
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
				text_baseRes.text = "0";
			}
			InitShowComponent();
			InitClickEvent();
		}
		protected virtual void InitShowComponent()
		{
			if(view.isWin)
			{
				btn_fullscreen_back.gameObject.SetActive(false);
				StartCoroutine(ShowFullscreenCloseBtnCoroutine(0.9f));
			}else
			{
				btn_fullscreen_back.gameObject.SetActive(true);
			}
		}
		protected virtual void InitClickEvent()
		{
			btn_fullscreen_back.onClick.AddListener(ClickBackBtnHandler);
            view.btnFailedHero.onClick.AddListener(ClickHeroBtnHandler);
            view.btnFailedEquip.onClick.AddListener(ClickEquipBtnHandler);
            view.btnFailedShop.onClick.AddListener(ClickShopBtnHandler);
			view.btnFailedFormation.onClick.AddListener(ClickFormationBtnHandler);
        }
		private IEnumerator ShowFullscreenCloseBtnCoroutine(float delay)
		{
			yield return new WaitForSeconds(delay);
			btn_fullscreen_back.gameObject.SetActive(true);
		}


		protected virtual void FightResultQuit(FightResultQuitType type)
		{
			FightResultProxy.instance.GotoMainScene(type, FightResultProxy.instance.QuitFriendCallback);
        }
		#region click event
		protected virtual void ClickBackBtnHandler()
		{
			FightResultQuit(FightResultQuitType.GO_FRIEND);
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