using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Common.Localization;
using Logic.UI.Equipments.View;
using Logic.UI.CommonItem.View;
using Logic.Equipment.Model;
using Logic.UI.CommonEquipment.View;
using Common.Util;
using Logic.Enums;
using Logic.UI.GoodsJump.Model;
using Logic.Equipment;
using Logic.Hero.Model;

namespace Logic.UI.Description.View
{
	public class CommonEquipDesView : MonoBehaviour {
		
		public const string PREFAB_PATH = "ui/description/common_equip_description_view";
		
		public static CommonEquipDesView Open(EquipmentInfo data, Vector3 pos,Vector2 sizeDelta)
		{
			CommonEquipDesView view = UIMgr.instance.Open<CommonEquipDesView>(PREFAB_PATH, EUISortingLayer.Tips);
			view.SetData(data,pos,sizeDelta);

			return view;
		}

		public Text textTitle;

		public Text textFrom;
		public Text textUse;
		public RectTransform rootPanel;
		public Transform iconRoot;
		public Text textAttrName;
		public Text textAttrValue;
		public Text textCareerType;
		public Text textCareerTypeValue;
		public Transform storyBottomLineTran;
		public Text textStory;

		private float _defaultContentLineHeight = 25;

		private int _defaultBorderX = 10;

		private EquipmentInfo _equipInfo;
		private Vector3 _worldPos;
		private Vector2 _sizeDelta ;
		private Vector2 _originSizeDelta;

		void Awake()
		{
			_originSizeDelta = rootPanel.sizeDelta;
			_defaultContentLineHeight = textUse.preferredHeight;
		}

		public void SetData(EquipmentInfo data,Vector3 worldPos,Vector2 sizeDelta)
		{

			_equipInfo = data;
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
			yield return null;

			textTitle.text = Localization.Get(_equipInfo.equipmentData.name);
			textStory.text = Localization.Get(_equipInfo.equipmentData.description);
			DropMessageData data = DropMessageData.GetDropMsgDataByResData((int)BaseResType.Equipment,_equipInfo.equipmentData.id);
			textFrom.text = data == null ? "" : Localization.Get( data.des);

			TransformUtil.ClearChildren(iconRoot,true);
			CommonEquipmentIcon icon =  CommonEquipmentIcon.Create(iconRoot);
			icon.SetEquipmentInfo(_equipInfo);
			icon.GetEquipmentDesButton().enabled = false;

			RefreshAttr();
			RefreshSpecialUse();

			float UseDeltaHeight =  (string.IsNullOrEmpty(textUse.text) ? 0 : textUse.preferredHeight)-_defaultContentLineHeight;
			float fromDeltaHeight = (string.IsNullOrEmpty(textFrom.text) ? 0 : textFrom.preferredHeight)-_defaultContentLineHeight;
			float storyDeltaHeight = textStory.preferredHeight-_defaultContentLineHeight;
			storyBottomLineTran.localPosition += new Vector3(0,-storyDeltaHeight);
			textFrom.transform.localPosition += new Vector3(0,UseDeltaHeight,0);
			rootPanel.sizeDelta = new Vector2(_originSizeDelta.x,_originSizeDelta.y+UseDeltaHeight+storyDeltaHeight+fromDeltaHeight);

			float screenHalfHeight = UIMgr.instance.designResolution.y/2;
			Vector3 localPosition = transform.InverseTransformPoint(_worldPos);
			float x = 0f;
			float y = localPosition.y;
			if(localPosition.x>0)
			{
				x = localPosition.x-_sizeDelta.x/2-rootPanel.sizeDelta.x/2-_defaultBorderX;
			}else{
				x = localPosition.x+_sizeDelta.x/2+rootPanel.sizeDelta.x/2+_defaultBorderX;
			}
			if(localPosition.y<rootPanel.sizeDelta.y/2-screenHalfHeight)
			{
				y = rootPanel.sizeDelta.y/2-screenHalfHeight;
			}
			if(localPosition.y>screenHalfHeight-rootPanel.sizeDelta.y/2)
			{
				y = screenHalfHeight - rootPanel.sizeDelta.y/2;
			}
			localPosition = new Vector3(x,y);
			rootPanel.anchoredPosition3D = localPosition;
			rootPanel.gameObject.SetActive(true);
		}
		private void RefreshAttr()
		{

			//attrViewPrefab.gameObject.SetActive(true);
//			List<EquipmentAttribute> attrList = _equipInfo.EquipAttribute;
//			int count = attrList.Count;
//			for(int i = 0;i<count;i++)
//			{
//				EquipAttributeView view = Instantiate<EquipAttributeView>(attrViewPrefab);
//				view.transform.SetParent(attrRoot,false);
//				view.Set(attrList[i]);
//			}
			EquipmentAttribute attr = _equipInfo.MainAttribute;

			textAttrName.text = attr.Name;
			textAttrValue.text = attr.ValueString;
			int special_hero = _equipInfo.equipmentData.special_hero;
			if (special_hero == 0) 
			{
				textCareerType.text = Localization.Get("equip_tips_careerlimit");
				textCareerTypeValue.text = Localization.Get(UIUtil.GetRoleTypeName(_equipInfo.equipmentData.equipmentRoleType));
			}else
			{
				textCareerType.text = Localization.Get("equip_tips_herolimit");
				textCareerTypeValue.text = Localization.Get(Localization.Get(HeroData.GetHeroDataByID(special_hero).name));
			}
		}
		private void RefreshSpecialUse()
		{

		}
		public void Close()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
	}
}

