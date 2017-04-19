using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Localization;
using Common.Util;
using Logic.Game.Model;
using Logic.VIP.Model;
using Logic.VIP.Controller;
using Logic.UI.CommonTopBar.View;
using Logic.UI.CommonReward.View;

namespace Logic.UI.VIP.View
{
	public class VIPView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/vip/vip_view";

		private int _currentVIPLevel = 0;

		#region UI componets
		public GameObject core;
		private CommonTopBarView _commonTopBarView;

		public Text myVIPLevelText;
		public Text myVIPExpText;
		public Slider myVIPExpSlider;

		public RectTransform vipDetailContent;
		public VIPPrivilegeDetailItem vipPrivilegeDetailItemPrefab;

		public Text vipLevelText;
		public Text vipExpText;
		public Text buyDiamondText;
		public Text conditionToNextVIPLevelText;
		public Text VIPDetailTitleText;
		public Button previousVIPDetailButton;
		public Button nextVIPDetailButton;
		public GameObject VIPBenefitsRootGameObject;
		public Text VIPBenefitsTitleText;
		public Text drawVIPBenefitsText;

		public Transform vipBenefitsItemsRoot;
		public Button drawVIPBenefitsButton;
		#endregion UI componnets

		void Awake ()
		{
			VIPProxy.instance.onVIPInfoUpdateDelegate += OnVIPInfoUpdateHandler;
			VIPProxy.instance.onDrawVIPBenefitsSuccessDelegate += OnDrawVIPBenefitsSuccessHandler;

			_commonTopBarView = CommonTopBarView.CreateNewAndAttachTo(core.transform);
			_commonTopBarView.SetAsCommonStyle(Localization.Get("ui.vip_view.title"), ClickCloseHandler, true, true, true, true);
			_commonTopBarView.transform.SetAsFirstSibling();
			vipLevelText.text = "1";
			vipExpText.text = string.Format(Localization.Get("common.value/max"), 125, 999);

			buyDiamondText.text = Localization.Get("ui.vip_view.buy_diamond");
			conditionToNextVIPLevelText.text = Localization.Get("ui.condition_to_next_vip_level");
			VIPDetailTitleText.text = Localization.Get("ui.vip_view.vip_detail_title");
			VIPBenefitsTitleText.text = Localization.Get("ui.vip_view.vip_benefits_title");
			drawVIPBenefitsText.text = Localization.Get("ui.vip_view.draw_vip_benifits");

			List<VIPData> allVIPDataList = VIPData.GetAllVIPDataList();
			for (int i = 0, allVIPDataCount = allVIPDataList.Count; i < allVIPDataCount; i++)
			{
				VIPPrivilegeDetailItem vipPrivilegeDetailItem = GameObject.Instantiate<VIPPrivilegeDetailItem>(vipPrivilegeDetailItemPrefab);
				vipPrivilegeDetailItem.SetVIPData(allVIPDataList[i]);
				vipPrivilegeDetailItem.transform.SetParent(vipDetailContent, false);
			}
			vipPrivilegeDetailItemPrefab.gameObject.SetActive(false);

			RefreshMyVIPInfo();
			_currentVIPLevel = VIPProxy.instance.VIPLevel;
			RefreshVIPDetail();
		}

		void OnDestroy ()
		{
			VIPProxy.instance.onVIPInfoUpdateDelegate -= OnVIPInfoUpdateHandler;
			VIPProxy.instance.onDrawVIPBenefitsSuccessDelegate -= OnDrawVIPBenefitsSuccessHandler;
		}

		private void RefreshMyVIPInfo ()
		{
			VIPData currentVipData = VIPData.GetVIPData(VIPProxy.instance.VIPLevel);
			myVIPLevelText.text = VIPProxy.instance.VIPLevel.ToString();
			int vipExp = VIPProxy.instance.TotalRecharge * 10;
			if (!currentVipData.IsMaxLevelVIPData())
			{
				VIPData nextVIPData = currentVipData.GetNextLevelVIPData();
				myVIPExpText.text = string.Format(Localization.Get("common.value/max"), vipExp, nextVIPData.exp * 10);
				myVIPExpSlider.value = (float)VIPProxy.instance.TotalRecharge / nextVIPData.exp;
				conditionToNextVIPLevelText.text = string.Format(Localization.Get("ui.condition_to_next_vip_level"), nextVIPData.exp - VIPProxy.instance.TotalRecharge, Localization.Get(nextVIPData.name));
				myVIPExpSlider.gameObject.SetActive(true);
				conditionToNextVIPLevelText.gameObject.SetActive(true);
			}
			else
			{
				myVIPExpText.text = vipExp.ToString();
				myVIPExpSlider.gameObject.SetActive(false);
				conditionToNextVIPLevelText.gameObject.SetActive(false);
			}
		}

		private void OnVIPDetailContentXUpdate (float anchoredX)
		{
			vipDetailContent.anchoredPosition = new Vector2(anchoredX, 0);
		}

		private void RefreshVIPDetail ()
		{
			VIPData currentVIPData = VIPData.GetVIPData(_currentVIPLevel);
			VIPDetailTitleText.text = Localization.Get(currentVIPData.name);
			if (currentVIPData.IsMinLevelVIPData())
			{
				previousVIPDetailButton.gameObject.SetActive(false);
				nextVIPDetailButton.gameObject.SetActive(true);
			}
			else if (currentVIPData.IsMaxLevelVIPData())
			{
				previousVIPDetailButton.gameObject.SetActive(true);
				nextVIPDetailButton.gameObject.SetActive(false);
			}
			else
			{
				previousVIPDetailButton.gameObject.SetActive(true);
				nextVIPDetailButton.gameObject.SetActive(true);
			}
			LeanTween.value(vipDetailContent.gameObject, OnVIPDetailContentXUpdate, vipDetailContent.anchoredPosition.x, -_currentVIPLevel * vipPrivilegeDetailItemPrefab.GetComponent<LayoutElement>().preferredWidth, 0.25f);

			TransformUtil.ClearChildren(vipBenefitsItemsRoot, true);
			List<GameResData> benefitItemList = currentVIPData.benefitItemList;
			for (int i = 0, benefitItemCount = benefitItemList.Count; i < benefitItemCount; i++)
			{
				CommonRewardIcon rewardIcon = CommonRewardIcon.Create(vipBenefitsItemsRoot);
				rewardIcon.SetGameResData(benefitItemList[i]);
			}
			VIPBenefitsRootGameObject.gameObject.SetActive(benefitItemList.Count > 0);
			drawVIPBenefitsButton.gameObject.SetActive(!VIPProxy.instance.HasReceivedGiftVIPLevelList.Contains(_currentVIPLevel));
		}

		#region proxy callback
		void OnVIPInfoUpdateHandler (int vipLevel, int totalRecharge, List<int> hasReceivedGiftVIPLevelList)
		{
			RefreshMyVIPInfo();
			RefreshVIPDetail();
		}

		void OnDrawVIPBenefitsSuccessHandler (int vipLevel)
		{
			Logic.UI.Tips.View.CommonAutoDestroyTipsView.Open(string.Format("Draw VIP{0} benefits success", vipLevel));
		}
		#endregion proxy callback

		#region UI event handlers
		public void ClickCloseHandler ()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}

		public void ClickBuyDiamond ()
		{
			FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction(Logic.Enums.FunctionOpenType.Shop_Diamond, null, false, true);
		}

		public void ClickPreviousVIPLevelDetailButtonHandler ()
		{
			_currentVIPLevel = Mathf.Max(0, _currentVIPLevel - 1);
			RefreshVIPDetail();
		}

		public void ClickNextVIPLevelDetailButtonHandler ()
		{
			_currentVIPLevel = Mathf.Min(15, _currentVIPLevel + 1);
			RefreshVIPDetail();
		}

		public void ClickDrawVIPBenefits ()
		{
			if (VIPProxy.instance.VIPLevel < _currentVIPLevel)
			{
				Logic.UI.Tips.View.ConfirmTipsView.Open(Localization.Get("ui.vip_view.vip_level_not_enough_tips"), ClickBuyDiamond);
				return;
			}
//			VIPController.instance.C2S_VIPGiftBagReq(_currentVIPLevel);
		}


		#endregion UI event handlers
	}
}