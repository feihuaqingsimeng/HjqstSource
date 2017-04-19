using UnityEngine;
using UnityEngine.UI;
using Common.Localization;
using Logic.UI.CommonTopBar.View;
using Common.UI.Components;
using Logic.Enums;
using Common.Util;
using Logic.UI.BlackMarket.Model;
using System.Collections.Generic;
using Logic.Game.Model;
using Logic.UI.CommonItem.View;
using Logic.Item.Model;
using Logic.UI.CommonReward.View;
using Logic.UI.CommonEquipment.View;
using Logic.UI.Tips.View;
using Logic.UI.BlackMarket.Controller;
using Common.GameTime.Controller;
using System.Collections;
using Logic.ConsumeTip.Model;
using LuaInterface;


namespace Logic.UI.BlackMarket.View
{
	public class BlackMarketView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/black_market/black_market_view";
		public static BlackMarketView Open(BlackMarketType type = BlackMarketType.BlackMarket_Hero)
		{
//			BlackMarketView view = UIMgr.instance.Open<BlackMarketView>(PREFAB_PATH);
//			BlackMarketProxy.instance.selectType = type;
//			return view;
			LuaTable table = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl", "black_market_controller")[0];
			table.GetLuaFunction("OpenBlackMarketView").Call((int)type);
			return null;
		}
		#region UI components
		public Text exchangeTitleText;
		public Text exchangeTimeText;
		public Text exchangeMaterialText;

		public GameObject core;


		public Toggle togglePrefab;
		public Transform toggleRoot;
		public Transform materialPrefab;
		public Transform materialRoot;
		public BlackMarketGoodsButton currentGoodsPrefab;
		public Transform currentItemIconRoot;
		public ScrollContentExpand itemScrollContent;
		#endregion UI components

		private CommonTopBarView _commonTopBarView;
		private Toggle _currentToggle;
		private bool _isFirstEnter = true;
		void Awake ()
		{ 

			string title = Localization.Get("ui.black_market_view.title");
			_commonTopBarView = CommonTopBarView.CreateNewAndAttachTo(core.transform);
			_commonTopBarView.SetAsCommonStyle(title, ClickCloseHandler, true, true, true, true);
			BindDelegate();

			exchangeTitleText.text = Localization.Get("ui.black_market_view.exchangeTitle");
			exchangeMaterialText.text = Localization.Get("ui.black_market_view.exchangeMaterial");
			currentGoodsPrefab.gameObject.SetActive(false);
			BlackMarketController.instance.CLIENT2LOBBY_BlackMarket_REQ();
			StartCoroutine(UpdateActivityTimeCoroutine());

		}
		private void BindDelegate()
		{
			BlackMarketProxy.instance.onUpdateAllBlackMarketDelegate += InitTableAndRefresh;
			BlackMarketProxy.instance.onUpdateBlackMarketDelegate += Refresh;
			BlackMarketProxy.instance.onUpdatePurchaseGoodsDelegate += OnUpdatePurchaseSuccessByProtocol;
			ItemProxy.instance.onItemInfoListUpdateDelegate += Refresh;
			GameProxy.instance.onBaseResourcesUpdateDelegate += OnBaseResourcesUpdateHandler;
		}
		private void UnBindDelegate()
		{
			BlackMarketProxy.instance.onUpdateAllBlackMarketDelegate -= InitTableAndRefresh;
			BlackMarketProxy.instance.onUpdateBlackMarketDelegate -= Refresh;
			BlackMarketProxy.instance.onUpdatePurchaseGoodsDelegate -= OnUpdatePurchaseSuccessByProtocol;
			ItemProxy.instance.onItemInfoListUpdateDelegate -= Refresh;
			GameProxy.instance.onBaseResourcesUpdateDelegate -= OnBaseResourcesUpdateHandler;
		}
		void OnDestroy()
		{
			UnBindDelegate();
		}
		public void InitTableAndRefresh()
		{
			InitToggles();
			RefreshCurrentItemIcon();
			RefreshMaterial();
		}
		private void InitToggles()
		{
			int start = (int)BlackMarketType.BlackMarket_Min+1;
            
			int end = (int)BlackMarketType.BlackMarket_Max;
            if (!BlackMarketProxy.instance.isLimitActivityOpen)
                end = (int)BlackMarketType.BlackMarket_LimitActivity;
			TransformUtil.ClearChildren(toggleRoot,true);
			togglePrefab.gameObject.SetActive(true);
			for(int i = start;i<end;i++)
			{
				Toggle toggle = Instantiate<Toggle>(togglePrefab);
				toggle.transform.SetParent(toggleRoot,false);
				ToggleContent content = toggle.GetComponent<ToggleContent>();
				content.Set(i,Localization.Get(string.Format("ui.black_market_view.toggle.{0}",i)));
				if(i == (int)BlackMarketProxy.instance.selectType)
				{

					toggle.isOn =  true ;

				}else
				{
					toggle.isOn =  false ;
				}
			}
			togglePrefab.gameObject.SetActive(false);
		}

		private void Refresh()
		{
			RefreshCurrentItemIcon();
			RefreshTable();
			RefreshMaterial();
		}

		private IEnumerator UpdateActivityTimeCoroutine()
		{
			while(true)
			{
				int type = 1;
				Dictionary<int,BlackMarketData> marketDic = BlackMarketData.BlackMarketDataDictionary;
				foreach(var data in marketDic)
				{
					if(data.Value.item_type == (int)BlackMarketProxy.instance.selectType )
					{
						type = data.Value.refresh_type;
						break;
					}
				}
				if(type == 2)
				{
					long remaindTime = BlackMarketProxy.instance.limitActiavityRefreshTime - TimeController.instance.ServerTimeTicksSecond*1000;
					if(remaindTime <= 0)
					{
						BlackMarketController.instance.CLIENT2LOBBY_BlackMarket_REQ();
					}
					exchangeTimeText.text = string.Format( Localization.Get("ui.black_market_view.exchangeTime"), TimeUtil.FormatSecondToHour((int)(remaindTime/1000)));
				}else
				{
					System.TimeSpan span =TimeController.instance.ServerTime.TimeOfDay;
					int remaindTime = 86400 - (int)span.TotalSeconds;
					if(remaindTime <= 0|| remaindTime == 86400)
					{
						BlackMarketController.instance.CLIENT2LOBBY_BlackMarket_REQ();
					}

					exchangeTimeText.text = string.Format( Localization.Get("ui.black_market_view.exchangeTime"), TimeUtil.FormatSecondToHour((int)remaindTime));
				}
				yield return new WaitForSeconds(1);
			}

		}

		private void InitTable()
		{
			List<BlackMarketInfo> infoList = BlackMarketProxy.instance.GetExchangeList();
			int count = infoList.Count;
			BlackMarketProxy.instance.selectBlackMarketInfo = count == 0 ? null : infoList[0];

			itemScrollContent.Init(count,_isFirstEnter,0.2f);
			_isFirstEnter = false;
		}
		public void RefreshTable()
		{
			itemScrollContent.RefreshAllContentItems();
		}
		private void RefreshCurrentItemIcon()
		{
			BlackMarketInfo info = BlackMarketProxy.instance.selectBlackMarketInfo;
			TransformUtil.ClearChildren(currentItemIconRoot,true);

			if(info == null)
			{
				return;
			}

			currentGoodsPrefab.gameObject.SetActive(true);

			BlackMarketGoodsButton btn = Instantiate<BlackMarketGoodsButton>(currentGoodsPrefab);
			btn.transform.SetParent(currentItemIconRoot,false);
			btn.transform.localPosition = Vector3.zero;
			btn.SetData(info,info.itemData.type == BaseResType.Hero,ShowDescriptionType.click);
			currentGoodsPrefab.gameObject.SetActive(false);
		}
		private void RefreshMaterial()
		{
			TransformUtil.ClearChildren(materialRoot,true);

			materialPrefab.gameObject.SetActive(true);
			BlackMarketInfo info = BlackMarketProxy.instance.selectBlackMarketInfo;
			List<GameResData> material = null;
			if(info != null)
			{
				material = info.materials;
			}else
			{
				material = new List<GameResData>();
			}
			int count = material.Count;
			GameResData resData;
			for(int i = 0;i<4;i++)
			{
				Transform tran = Instantiate<Transform>(materialPrefab);
				tran.transform.SetParent(materialRoot,false);
				Text textCount = tran.Find("text_count").GetComponent<Text>();
				Transform material_root = tran.Find("materialRoot");
				if(i<count)
				{
					resData = material[i];
					CommonItemIcon icon = CommonItemIcon.Create(material_root);
					icon.SetGameResData(resData);
					icon.HideCount();
					int ownCount = 0;
					if(resData.type == BaseResType.Item)
					{
						ownCount = ItemProxy.instance.GetItemCountByItemID(resData.id);
					}else
					{
						ownCount = GameProxy.instance.BaseResourceDictionary.GetValue(resData.type);
					}

					string countString = string.Format(Localization.Get( "common.value/max"),ownCount,resData.count);
					textCount.text = ownCount >= resData.count ? UIUtil.FormatToGreenText(countString) : UIUtil.FormatToRedText(countString);
				}else{
					textCount.text = string.Empty;
				}

			}
			materialPrefab.gameObject.SetActive(false);
		}

		public void OnUpdatePurchaseSuccessByProtocol()
		{
			List<GameResData> reward = new List<GameResData>();
			reward.Add(BlackMarketProxy.instance.buyBlackMarketInfo.itemData);
			CommonRewardAutoDestroyTipsView.Open(reward);
		}
		public void  OnBaseResourcesUpdateHandler()
		{
			RefreshMaterial();
		}
		#region UI event handlers

		public void OnClickTableGoodsBtnHandler(BlackMarketGoodsButton btn)
		{
			if(!btn.IsSelect)
			{
				BlackMarketProxy.instance.selectBlackMarketInfo = btn.blackMarketInfo;
				Refresh();
			}
		}
		public void OnClickExchangeBtnHandler()
		{
			BlackMarketInfo info = BlackMarketProxy.instance.selectBlackMarketInfo;
			if(info == null)
			{
				CommonErrorTipsView.Open(Localization.Get("ui.black_market_view.choiceGoods"));
				return;
			}
			if(info.limitLv > GameProxy.instance.AccountLevel)
			{
				CommonErrorTipsView.Open(Localization.Get("ui.black_market_view.levelNotEnough"));
				return;
			}
			if(info.remaindCount == 0)
			{
				CommonErrorTipsView.Open(Localization.Get("ui.black_market_view.exchangeEmpty"));
				return;
			}
			List<GameResData> materials = info.materials;
			GameResData resData;

			bool hasDiamond = false;
			GameResData diamondResData = null;
			for(int i = 0,count = materials.Count;i<count;i++)
			{
				resData = materials[i];
				int ownCount = 0 ;
				if(resData.type == BaseResType.Item)
				{
					ownCount = ItemProxy.instance.GetItemCountByItemID(resData.id);
				}else
				{
					ownCount = GameProxy.instance.BaseResourceDictionary.GetValue(resData.type);
					if(resData.type == BaseResType.Diamond)
					{
						hasDiamond = true;
						diamondResData = resData;
					}
						
				}
				if(ownCount < resData.count)
				{
					CommonErrorTipsView.Open(Localization.Get("ui.black_market_view.notEnoughMaterial"));
					return;
				}
			}
			if(hasDiamond)
			{
				if(ConsumeTipProxy.instance.GetConsumeTipEnable(ConsumeTipType.DiamondBuyBlackmarket))
					ConfirmBuyShopItemTipsView.Open("",diamondResData,ExchangeSureHandler,ConsumeTipType.DiamondBuyBlackmarket);
				else
					ExchangeSureHandler();
			}else
			{
				ExchangeSureHandler();
			}

		}
		private void ExchangeSureHandler()
		{
			BlackMarketInfo info = BlackMarketProxy.instance.selectBlackMarketInfo;
			BlackMarketProxy.instance.buyBlackMarketInfo = info;
			BlackMarketController.instance.CLIENT2LOBBY_BlackMarket_Exchange_REQ(info.id);
		}
		public void ClickCloseHandler ()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}

		public void ClickToggleHandler (Toggle toggle)
		{
			if(_currentToggle == toggle)
				return;
			if (toggle.isOn)
			{
				_currentToggle = toggle;
				ToggleContent content = toggle.GetComponent<ToggleContent>();
				BlackMarketProxy.instance.selectType = (BlackMarketType)content.id;
				InitTable();
				RefreshCurrentItemIcon();
				RefreshMaterial();
			}
		}
		public void OnResetItemContent(GameObject go,int index)
		{
			BlackMarketInfo info = BlackMarketProxy.instance.currentBalckMarketInfoList[index];
			bool isSelect = false;
			if(info == BlackMarketProxy.instance.selectBlackMarketInfo)
			{
				isSelect = true;
			}
			BlackMarketGoodsButton btn = go.GetComponent<BlackMarketGoodsButton>();
			btn.onBlackMarketClickDelegate = OnClickTableGoodsBtnHandler;
			btn.SetData(info);
			btn.SetSelect(isSelect);
		}
		#endregion UI handlers
	}
}
