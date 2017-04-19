using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Logic.UI.CommonItem.View;
using Logic.Item.Model;
using System.Collections.Generic;
using Common.Localization;
using Common.Util;

namespace Logic.UI.Item.View
{
	public class ItemBagView : MonoBehaviour 
	{
		public const string PREFAB_PATH = "ui/item/item_bag_view";
		#region ui components
		public GameObject itemBackgroundGO;
		public CommonItemIcon itemIconPrefab;
		public Transform itemRootTransform;
		public Text bagCellText;
		#endregion ui

		public static ItemBagView Open()
		{
			ItemBagView view = UIMgr.instance.Open<ItemBagView>(PREFAB_PATH);
			return view;
		}

		void Awake()
		{
			BindDelegate();

			Init();
		}
		void OnDestroy()
		{
			UnBindDelegate();
		}
		private void BindDelegate ()
		{
			
			ItemProxy.instance.onItemInfoListUpdateDelegate += ItemInfoListUpdate;
		}
		
		private void UnBindDelegate ()
		{
			ItemProxy.instance.onItemInfoListUpdateDelegate -= ItemInfoListUpdate;
		}
		private void Init()
		{
			InitItemBackground();
			InitItemTable();
			RefreshBagCellNum();
		}
		private void InitItemBackground()
		{
			int num = ItemProxy.instance.GetItemCellNum();
			itemBackgroundGO.SetActive(true);
			for(int i = 0;i<num;i++)
			{
				GameObject go = Instantiate<GameObject>(itemBackgroundGO);
				go.transform.SetParent(itemRootTransform,false);
				go.name = i.ToString();
			}
			itemBackgroundGO.SetActive(false);
		}
		private void InitItemTable()
		{
			List<ItemInfo> items = ItemProxy.instance.GetAllItemInfoList();
			items.Sort(ItemProxy.CompareItemInfo);
			int itemCellNum = ItemProxy.instance.GetItemCellNum();
			int count = items.Count;
			itemIconPrefab.gameObject.SetActive(true);
			for(int i = 0;i<count && i<itemCellNum;i++)
			{
				GameObject go = Instantiate<GameObject>(itemIconPrefab.gameObject);
				Transform tran = go.transform;
				tran.SetParent(itemRootTransform.GetChild(i),false);
				tran.localPosition = Vector3.zero;
				go.name = items[i].itemData.id.ToString();
				CommonItemIcon icon = tran.GetComponent<CommonItemIcon>();
				icon.SetItemInfo(items[i]);
			}
			itemIconPrefab.gameObject.SetActive(false);
		}

		private void Refresh()
		{
			RefreshItemBackground();
			RefreshItemTable();
			RefreshBagCellNum();
		}
		private void RefreshBagCellNum()
		{
			bagCellText.text = string.Format(Localization.Get("common.value/max"),ItemProxy.instance.GetAllItemCount(),ItemProxy.instance.GetItemCellNum());
		}
		private void RefreshItemBackground()
		{
			int num = itemRootTransform.childCount;

			for(int i = 0;i<num;i++)
			{
				TransformUtil.ClearChildren(itemRootTransform.GetChild(i),true);
			}

		}
		private void RefreshItemTable()
		{
			List<ItemInfo> items = ItemProxy.instance.GetAllItemInfoList();
			items.Sort(ItemProxy.CompareItemInfo);
			int itemCellNum = ItemProxy.instance.GetItemCellNum();
			int count = items.Count;
			itemIconPrefab.gameObject.SetActive(true);
			for(int i = 0;i<count && i<itemCellNum;i++)
			{
				GameObject go = Instantiate<GameObject>(itemIconPrefab.gameObject);
				Transform tran = go.transform;
				tran.SetParent(itemRootTransform.GetChild(i),false);
				tran.localPosition = Vector3.zero;
				go.name = items[i].itemData.id.ToString();
				CommonItemIcon icon = tran.GetComponent<CommonItemIcon>();
				icon.SetItemInfo(items[i]);
			}
			itemIconPrefab.gameObject.SetActive(false);
		}
		private void ItemInfoListUpdate()
		{
			Refresh();
		}
		#region ui event handler
		public void OnClickBackHandler()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
		#endregion
	}
}

