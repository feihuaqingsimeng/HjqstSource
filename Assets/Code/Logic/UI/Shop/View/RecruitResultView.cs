using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.ResMgr;
using Logic.Hero.Model;
using Logic.Shop.Model;
using Logic.UI.Shop.Controller;
using Logic.Game.Model;
using Common.Localization;
using Logic.Game;
using Logic.UI.Tips.View;
using Logic.ConsumeTip.Model;
using Logic.Enums;

namespace Logic.UI.Shop.View
{
	public class RecruitResultView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/recruit_result/recruit_result_view";

		private ShopHeroRandomCardInfo _shopHeroRandomCardInfo;

		#region UI components
		public Transform[] recruitHeroItemsRoot;
		public Transform giftRecruitHeroItemRoot;
		public RecuitHeroItem recuitItemPrefab;
		public float turnOverInterval = 0.25f;
		public Text backText;
		public Image costIconImage;
		public Text againText;
		#endregion UI components
		public bool reset = false;

		private RecuitHeroItem[] _recruitHeroItems;
		private RecuitHeroItem _giftRecruitHeroItem;

		public Logic.UI.CommonAnimations.CommonScaleUpAnimation bottomButtonAnimation;

		void Start ()
		{
			LTDescr ltDescr = LeanTween.delayedCall(3, OnViewReady);
		}

		private void OnViewReady ()
		{
			Observers.Facade.Instance.SendNotification(string.Format("{0}::{1}", PREFAB_PATH, "OnViewReady"));
		}

		void Update()
		{
			if(reset)
			{
				reset = false;
				Reset();
				int i = 0;
				for (i = 0; i < 10; i++)
				{
					_recruitHeroItems[i].TurnOverAfter(i * turnOverInterval);
				}
				_giftRecruitHeroItem.TurnOverAfter(i * turnOverInterval);
			}
		}

		public void Reset ()
		{
			int recruitHeroItemCount = _recruitHeroItems.Length;
			for (int i = 0;i < recruitHeroItemCount; i++)
			{
				_recruitHeroItems[i].Reset();
			}
			_giftRecruitHeroItem.Reset();
		}

		public void SetNewHeroInfoList (ShopHeroRandomCardInfo shopHeroRandomCardInfo, List<HeroInfo> newHeroInfoList, HeroInfo giftHeroInfo)
		{
			int i;
			if(_recruitHeroItems == null)
			{
				int len = recruitHeroItemsRoot.Length;
				_recruitHeroItems = new RecuitHeroItem[len];
				Transform itemRoot;
				RecuitHeroItem item;
				recuitItemPrefab.gameObject.SetActive(true);
				//normal
				for(i = 0;i<len;i++)
				{
					itemRoot = recruitHeroItemsRoot[i];
					item = Instantiate<RecuitHeroItem>(recuitItemPrefab);
					item.transform.SetParent(itemRoot,false);
					item.transform.localPosition = Vector3.zero;
					_recruitHeroItems[i] = item;
				}
				//gift
				item = Instantiate<RecuitHeroItem>(recuitItemPrefab);
				item.transform.SetParent(giftRecruitHeroItemRoot,false);
				item.transform.localPosition = Vector3.zero;
				_giftRecruitHeroItem = item;

				recuitItemPrefab.gameObject.SetActive(false);
			}
			//reset
			Reset();
			_shopHeroRandomCardInfo = shopHeroRandomCardInfo;

			for (i = 0; i < 10; i++)
			{
				HeroInfo heroInfo = newHeroInfoList[i];
				_recruitHeroItems[i].SetHeroInfo(heroInfo);
				_recruitHeroItems[i].TurnOverAfter(i * turnOverInterval);
			}

			_giftRecruitHeroItem.SetHeroInfo(giftHeroInfo);
			_giftRecruitHeroItem.TurnOverAfter(i * turnOverInterval);

			backText.text = Localization.Get("ui.recruit_result_view.back");
			costIconImage.SetSprite( ResMgr.instance.Load<Sprite>(UIUtil.GetBaseResIconPath(_shopHeroRandomCardInfo.ShopCardRandomData.costGameResData.type)));
			againText.text = string.Format(Localization.Get("ui.recruit_result_view.again"), _shopHeroRandomCardInfo.ShopCardRandomData.costGameResData.count);;
		}

		#region UI event handlers
		public void ClickAgainHandler ()
		{
			int shopID = 0;
			int shopItemID = 0;
			string shopItemName = string.Empty;
			GameResData costGameResData = null;
			int costType = 1;
			if (_shopHeroRandomCardInfo != null)
			{
				shopID = _shopHeroRandomCardInfo.ShopCardRandomData.shopID;
				shopItemID = _shopHeroRandomCardInfo.ShopCardRandomData.id;
				shopItemName = Localization.Get(_shopHeroRandomCardInfo.ShopCardRandomData.name);
				costGameResData = _shopHeroRandomCardInfo.ShopCardRandomData.costGameResData;
				if (_shopHeroRandomCardInfo.RemainFreeTimes > 0 && _shopHeroRandomCardInfo.NextFreeBuyCountDownTime <= 0)
				{
					costType = 0;
				}
			}
			
			if (costType == 0)   //免费
			{
				ShopController.instance.CLIENT2LOBBY_PURCHASE_GOODS_REQ(shopID, shopItemID, costType);
			}
			else if (costType == 1)   //非免费
			{
				if (GameResUtil.IsBaseRes(costGameResData.type))
				{
					if (GameProxy.instance.BaseResourceDictionary[costGameResData.type] < costGameResData.count)
					{
						CommonErrorTipsView.Open(string.Format(Localization.Get("ui.common_tips.not_enough_resource"), GameResUtil.GetBaseResName(costGameResData.type)));
						return;
					}
					
					if (costGameResData.type == Logic.Enums.BaseResType.Diamond)
					{
						if(ConsumeTipProxy.instance.GetConsumeTipEnable(ConsumeTipType.DiamondDrawTenHeroes))
							ConfirmBuyShopItemTipsView.Open(shopItemName, costGameResData, ClickConfirmBuyHandler,ConsumeTipType.DiamondDrawTenHeroes);
						else 
							ClickConfirmBuyHandler();
					}
					else
					{
						ShopController.instance.CLIENT2LOBBY_PURCHASE_GOODS_REQ(shopID, shopItemID, costType);
					}
				}
				else
				{
					ShopController.instance.CLIENT2LOBBY_PURCHASE_GOODS_REQ(shopID, shopItemID, costType);
				}
			}
		}

		public void ClickConfirmBuyHandler ()
		{
			int shopID = 0;
			int shopItemID = 0;
			GameResData costGameResData = null;
			int costType = 1;
			if (_shopHeroRandomCardInfo != null)
			{
				shopID = _shopHeroRandomCardInfo.ShopCardRandomData.shopID;
				shopItemID = _shopHeroRandomCardInfo.ShopCardRandomData.id;
				costGameResData = _shopHeroRandomCardInfo.ShopCardRandomData.costGameResData;
				if (_shopHeroRandomCardInfo.RemainFreeTimes > 0 && _shopHeroRandomCardInfo.NextFreeBuyCountDownTime <= 0)
				{
					costType = 0;
				}
			}
			bottomButtonAnimation.Execute();
			ShopController.instance.CLIENT2LOBBY_PURCHASE_GOODS_REQ(shopID, shopItemID, costType);
		}

		public void ClickCloseHandler ()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}		
		#endregion
	}
}