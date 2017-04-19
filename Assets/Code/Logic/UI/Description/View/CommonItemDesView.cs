using UnityEngine;
using System.Collections;
using Logic.Item.Model;
using UnityEngine.UI;
using Common.Localization;
using Logic.UI.CommonItem.View;
using System.Collections.Generic;
using Logic.Enums;
using Logic.UI.GoodsJump.Model;

namespace Logic.UI.Description.View
{
	public class CommonItemDesView : MonoBehaviour {
		
		public const string PREFAB_PATH = "ui/description/common_item_description_view";
		
		public static CommonItemDesView Open(ItemInfo data, Vector3 pos,Vector2 sizeDelta)
		{
			CommonItemDesView view = UIMgr.instance.Open<CommonItemDesView>(PREFAB_PATH, EUISortingLayer.Tips);
			view.SetData(data,pos,sizeDelta);

			return view;
		}

		public Text textTitle;
		public Transform iconRoot;
		public Text textDes;
		public Text textFrom;
		public Text textUse;
		public RectTransform rootPanel;
		public RectTransform bottomRoot;
		private float _defaultContentLineHeight = 25;

		private int _defaultBorderX = 10;

		private ItemInfo _itemInfo;
		private Vector3 _worldPos;
		private Vector2 _sizeDelta ;
		private Vector2 _originSizeDelta;

		void Awake()
		{
			_originSizeDelta = rootPanel.sizeDelta;
			_defaultContentLineHeight = textUse.preferredHeight;
		}

		public void SetData(ItemInfo data,Vector3 worldPos,Vector2 sizeDelta)
		{

			_itemInfo = data;
			_worldPos = worldPos;
			_sizeDelta = sizeDelta;
			StartCoroutine(RefreshCoroutine());

		}
		public void SetPivot(Vector2 pivot)
		{
			rootPanel.pivot = pivot;
		}
		public IEnumerator RefreshCoroutine()
		{
			rootPanel.gameObject.SetActive(false);
			yield return null;

			textTitle.text = Localization.Get(_itemInfo.itemData.name);
			textDes.text = Localization.Get(_itemInfo.itemData.des);
			DropMessageData data = DropMessageData.GetDropMsgDataByResData((int)BaseResType.Item,_itemInfo.itemData.id);
			textFrom.text = data == null ? "" : Localization.Get( data.des);
			CommonItemIcon icon =  CommonItemIcon.Create(iconRoot);
			icon.SetItemInfo(new ItemInfo(0,_itemInfo.itemData.id,0));
			icon.GetComponent<ItemDesButton>().enabled = false;
			RefreshSpecialUse();
			float useDeltaHeight = (string.IsNullOrEmpty(textUse.text) ? 0 : textUse.preferredHeight)-_defaultContentLineHeight;
			float fromDeltaHeight = (string.IsNullOrEmpty(textFrom.text) ? 0 : textFrom.preferredHeight)-_defaultContentLineHeight;
			float deltaHeight = useDeltaHeight + fromDeltaHeight;
			textFrom.transform.localPosition += new Vector3(0, useDeltaHeight);
			rootPanel.sizeDelta = new Vector2(_originSizeDelta.x,_originSizeDelta.y+deltaHeight);

			float screenHalfHeight = UIMgr.instance.designResolution.y/2;
			Vector3 localPosition = transform.InverseTransformPoint(_worldPos);
			float x = 0f;
			float y = localPosition.y+rootPanel.sizeDelta.y/2;
			if(localPosition.x>0)
			{
				x = localPosition.x-_sizeDelta.x/2-rootPanel.sizeDelta.x/2-_defaultBorderX;
			}else{
				x = localPosition.x+_sizeDelta.x/2+rootPanel.sizeDelta.x/2+_defaultBorderX;
			}
			if(y < rootPanel.sizeDelta.y-screenHalfHeight)
			{
				y = rootPanel.sizeDelta.y-screenHalfHeight;
			}
			if(y > screenHalfHeight)
			{
				y = screenHalfHeight;
			}
			localPosition = new Vector3(x,y);
			rootPanel.localPosition = localPosition;
			rootPanel.gameObject.SetActive(true);
		}
		private void RefreshSpecialUse()
		{
			List<RoleType> heroTypeList = _itemInfo.itemData.heroTypeList;
			int count = heroTypeList.Count ;
			if(count == 0 )
			{
				textUse.gameObject.SetActive(false);
			}else
			{
				
				textUse.gameObject.SetActive(true);
				string typeName = string.Empty;
				for(int i = 0;i<count;i++)
				{
					if(i == 0)
						typeName = UIUtil.GetRoleTypeName((RoleType)heroTypeList[i]);
					else 
					typeName = string.Format("{0}、{1}",typeName,UIUtil.GetRoleTypeName((RoleType)heroTypeList[i]));
				}
				textUse.text = string.Format(Localization.Get("ui.common_des_view.use"),typeName);
			}
		}

		public void Close()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
	}
}

