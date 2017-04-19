using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Localization;
using Common.Util;
using Common.UI.Components;
using Logic.Enums;
using Logic.Hero.Model;
using Logic.Shop.Model;
using Logic.UI.CommonTopBar.View;
using Logic.UI.Shop.Model;
using Logic.UI.Shop.Controller;
using Logic.VIP.Model;
using LuaInterface;

namespace Logic.UI.Shop.View
{
	public class ShopView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/shop/shop_view";

		private bool _isViewReady = false;

		#region UI components
		public GameObject core;
		private CommonTopBarView _commonTopBarView;

		public Text myVIPLevelText;
		public Text myVIPExpText;
		public Slider myVIPExpSlider;

		public Text privilegeText;
		public Text shopTypeText;

		public Toggle[] toggles;
		public GameObject[] togglePanelGameObjects;

		public ShopItemView shopItemViewPrefab;
		public Transform heroRandomShopItemsRoot;
		public Transform equipmentRandomShopItemsRoot;
		public Transform diamondShopItemsRoot;
		public Transform actionShopItemsRoot;
		public Transform goldShopItemsRoot;
		public Transform otherShopItemsRoot;
		#endregion

		public static void Open(int toggelIndex = 0)
		{
			LuaTable shopControllerLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl", "shop_controller")[0];
			shopControllerLuaTable.GetLuaFunction("OpenShopView").Call(toggelIndex);
		}

		void Awake ()
		{
			core.SetActive(false);
			Init();
			EnableTogglePanel(0);
			BindDelegate();
			ShopController.instance.CLIENT2LOBBY_DRAW_CARD_GOODS_REQ();
			ShopController.instance.CLIENT2LOBBY_LIMIT_GOODS_REQ();
			ShopController.instance.CLIENT2LOBBY_OTHER_GOODS_REQ();
		}
		
		private void OnViewReady ()
		{
			Observers.Facade.Instance.SendNotification(string.Format("{0}::{1}", PREFAB_PATH, "OnViewReady"));
		}

		void OnDestroy ()
		{
			UnbindDelegate();
		}

		private void Init ()
		{
			_commonTopBarView = CommonTopBarView.CreateNewAndAttachTo(core.transform);
			string title = Localization.Get("ui.shop_view.shop_view_title");
			_commonTopBarView.SetAsCommonStyle(title, ClickCloseHandler, true, true, true, true);
			_commonTopBarView.transform.SetAsFirstSibling();

			privilegeText.text = Localization.Get("ui.shop_view.privilege");
			shopTypeText.text = Localization.Get("ui.shop_view.noble_shop");
			shopItemViewPrefab.gameObject.SetActive(false);
			RefreshMyVIPInfo();
			RegenerateShopDiamondItems();
		}

		private void BindDelegate ()
		{
			VIPProxy.instance.onVIPInfoUpdateDelegate += OnVIPInfoUpdateHandler;
			ShopProxy.instance.onShopHeroRandomCardInfoListUpdateDelegate += OnShopHeroRandomCardItemInfoListUpdate;
			ShopProxy.instance.onShopEquipmentRandomCardInfoListUpdateDelegate += OnShopEquipmentRandomCardItemInfoListUpdate;
			ShopProxy.instance.onShopDiamondItemInfoListUpdateDelegate += OnShopDiamondItemInfoListUpdate;
			ShopProxy.instance.onShopActionItemInfoListUpdateDelegate += OnShopActionItemInfoListUpdate;
			ShopProxy.instance.onShopGoldItemInfoListUpdateDelegate += OnShopGoldItemInfoListUpdate;
			ShopProxy.instance.onShopGoodsItemInfoListUpdateDelegate += onShopGoodsItemInfoListUpdate;
		}

		private void UnbindDelegate ()
		{
			if (ShopProxy.instance != null)
			{
				VIPProxy.instance.onVIPInfoUpdateDelegate -= OnVIPInfoUpdateHandler;
				ShopProxy.instance.onShopHeroRandomCardInfoListUpdateDelegate -= OnShopHeroRandomCardItemInfoListUpdate;
				ShopProxy.instance.onShopEquipmentRandomCardInfoListUpdateDelegate -= OnShopEquipmentRandomCardItemInfoListUpdate;
				ShopProxy.instance.onShopDiamondItemInfoListUpdateDelegate -= OnShopDiamondItemInfoListUpdate;
				ShopProxy.instance.onShopActionItemInfoListUpdateDelegate -= OnShopActionItemInfoListUpdate;
				ShopProxy.instance.onShopGoldItemInfoListUpdateDelegate -= OnShopGoldItemInfoListUpdate;
				ShopProxy.instance.onShopGoodsItemInfoListUpdateDelegate -= onShopGoodsItemInfoListUpdate;
			}
		}

		private void RefreshMyVIPInfo ()
		{
			VIPData currentVipData = VIPData.GetVIPData(VIPProxy.instance.VIPLevel);
			myVIPLevelText.text = VIPProxy.instance.VIPLevel.ToString();
			if (!currentVipData.IsMaxLevelVIPData())
			{
				VIPData nextVIPData = currentVipData.GetNextLevelVIPData();
				myVIPExpText.text = string.Format(Localization.Get("common.value/max"), VIPProxy.instance.TotalRecharge * 10, nextVIPData.exp * 10);
				myVIPExpSlider.value = (float)VIPProxy.instance.TotalRecharge / nextVIPData.exp;
				myVIPExpSlider.gameObject.SetActive(true);
			}
			else
			{
				myVIPExpText.text = (VIPProxy.instance.TotalRecharge * 10).ToString();
				myVIPExpSlider.gameObject.SetActive(false);
			}
		}

		private void RegenerateShopHeroRandomItems ()
		{
			TransformUtil.ClearChildren(heroRandomShopItemsRoot, true);
			List<ShopHeroRandomCardInfo> shopHeroRandomCardInfoList = ShopProxy.instance.ShopHeroRandomCardInfoDic.GetValues();
			int shopHeroRandomCardInfoCount = shopHeroRandomCardInfoList.Count;
			ShopHeroRandomCardInfo shopHeroRandomCardInfo = null;
			for (int i = 0; i < shopHeroRandomCardInfoCount; i++)
			{
				shopHeroRandomCardInfo = shopHeroRandomCardInfoList[i];
				ShopItemView shopItemView = GameObject.Instantiate<ShopItemView>(shopItemViewPrefab);
				shopItemView.SetShopHeroRandomCardInfo(shopHeroRandomCardInfo);
				shopItemView.name = shopHeroRandomCardInfo.ShopCardRandomData.id.ToString();
				shopItemView.transform.SetParent(heroRandomShopItemsRoot, false);
				shopItemView.gameObject.SetActive(true);
			}
		}

		private void RegenerateShopEquipmentRandomItems ()
		{
			TransformUtil.ClearChildren(equipmentRandomShopItemsRoot, true);
			List<ShopEquipmentRandomCardInfo> shopEquipmentRandomCardInfoList = ShopProxy.instance.ShopEquipmentRandomCardInfoDic.GetValues();
			int shopEquipmentRandomCardInfoCount = shopEquipmentRandomCardInfoList.Count;
			for (int i = 0; i < shopEquipmentRandomCardInfoCount; i++)
			{
				ShopItemView shopItemView = GameObject.Instantiate<ShopItemView>(shopItemViewPrefab);
				shopItemView.SetShopEquipmentRandomCardInfo(shopEquipmentRandomCardInfoList[i]);
				shopItemView.transform.SetParent(equipmentRandomShopItemsRoot, false);
				shopItemView.gameObject.SetActive(true);
			}
		}

		private void RegenerateShopDiamondItems ()
		{
			TransformUtil.ClearChildren(diamondShopItemsRoot, true);
			List<ShopDiamondItemInfo> shopDiamondItemInfoList = ShopProxy.instance.ShopDiamondItemInfoDic.GetValues();
			int shopDiamondItemInfoCount = shopDiamondItemInfoList.Count;
			for (int i = 0; i < shopDiamondItemInfoCount; i++)
			{
				ShopItemView shopItemView = GameObject.Instantiate<ShopItemView>(shopItemViewPrefab);
				shopItemView.SetShopDiamondItemInfo(shopDiamondItemInfoList[i]);
				shopItemView.transform.SetParent(diamondShopItemsRoot, false);
				shopItemView.gameObject.SetActive(true);
			}
		}

		private void RegenerateShopActionItems ()
		{
			TransformUtil.ClearChildren(actionShopItemsRoot, true);
			List<ShopActionItemInfo> shopActionItemInfoList = ShopProxy.instance.ShopActionItemInfoDic.GetValues();
			int shopActionItemInfoCount = shopActionItemInfoList.Count;
			for (int i = 0; i < shopActionItemInfoCount; i++)
			{
				if (shopActionItemInfoList[i].IsOpen)
				{
					ShopItemView shopItemView =  GameObject.Instantiate<ShopItemView>(shopItemViewPrefab);
					shopItemView.SetShopActionItemInfo(shopActionItemInfoList[i]);
					shopItemView.transform.SetParent(actionShopItemsRoot, false);
					shopItemView.gameObject.SetActive(true);
				}
			}
		}

		private void RegenerateShopGoldItems ()
		{
			TransformUtil.ClearChildren(goldShopItemsRoot, true);
			List<ShopGoldItemInfo> shopGoldItemInfoList = ShopProxy.instance.ShopGoldItemInfoDic.GetValues();
			int shopGoldItemInfoCount = shopGoldItemInfoList.Count;
			for (int i = 0; i < shopGoldItemInfoCount; i++)
			{
				if (shopGoldItemInfoList[i].IsOpen)
				{
					ShopItemView shopItemView =  GameObject.Instantiate<ShopItemView>(shopItemViewPrefab);
					shopItemView.SetShopGoldItemInfo(shopGoldItemInfoList[i]);
					shopItemView.transform.SetParent(goldShopItemsRoot, false);
					shopItemView.gameObject.SetActive(true);
				}
			}
		}

		private void RegenerateShopGoodsItems ()
		{
			TransformUtil.ClearChildren(otherShopItemsRoot, true);
			List<ShopGoodsItemInfo> shopGoodsItemInfoList = ShopProxy.instance.ShopGoodsItemInfoDic.GetValues();
			int shopGoodsItemInfoCount = shopGoodsItemInfoList.Count;
			for (int i = 0; i < shopGoodsItemInfoCount; i++)
			{
				ShopItemView shopItemView =  GameObject.Instantiate<ShopItemView>(shopItemViewPrefab);
				shopItemView.SetShopGoodItemInfo(shopGoodsItemInfoList[i]);
				shopItemView.transform.SetParent(otherShopItemsRoot, false);
				shopItemView.gameObject.SetActive(true);
			}
		}

		public void EnableTogglePanel (int panelIndex)
		{
			int togglePanelsCount = togglePanelGameObjects.Length;
			for (int i = 0; i < togglePanelsCount; i++)
			{
				if (i == panelIndex)
				{
					togglePanelGameObjects[i].SetActive(true);
				}
				else
				{
					togglePanelGameObjects[i].SetActive(false);
				}
			}
		}

		public void SetTogglePanel(int panelIndex){

			if(panelIndex>=0 && panelIndex < toggles.Length)
			{
				toggles[panelIndex].isOn = true;
				for(int i = 0,cout = toggles.Length;i<cout;i++)
				{
					if(toggles[i] != toggles[panelIndex])
					{
						toggles[i].isOn = false;
					}
				}
				EnableTogglePanel (panelIndex);
			}
		}

		#region UI event handlers
		public void ClickCloseHandler ()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}

		public void ClickVIPButtonHandler ()
		{
			UIMgr.instance.Open(VIP.View.VIPView.PREFAB_PATH);
		}

		public void ClickToggle (Toggle toggle)
		{
			int index = toggles.IndexOf(toggle);
			EnableTogglePanel (index);
		}
		#endregion

		#region proxy callbacks
		void OnVIPInfoUpdateHandler (int vipLevel, int totalRecharge, List<int> hasReceivedGiftVIPLevelList)
		{
			RefreshMyVIPInfo();
		}
		private void OnShopHeroRandomCardItemInfoListUpdate ()
		{
			RegenerateShopHeroRandomItems();
		}

		private void OnShopEquipmentRandomCardItemInfoListUpdate ()
		{
			RegenerateShopEquipmentRandomItems();
		}

		private void OnShopDiamondItemInfoListUpdate ()
		{
			RegenerateShopDiamondItems();
		}

		private void OnShopActionItemInfoListUpdate ()
		{
			RegenerateShopActionItems();
		}

		private void OnShopGoldItemInfoListUpdate ()
		{
			RegenerateShopGoldItems();
		}

		private void onShopGoodsItemInfoListUpdate ()
		{
			RegenerateShopGoodsItems();
			if (!_isViewReady)
			{
				core.gameObject.SetActive(true);
				OnViewReady();
				_isViewReady = true;
			}
		}
		#endregion
	}
}
