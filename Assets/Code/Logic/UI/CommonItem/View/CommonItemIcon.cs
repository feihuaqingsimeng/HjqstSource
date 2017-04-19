using UnityEngine;
using UnityEngine.UI;
using Common.Localization;
using Common.ResMgr;
using Logic.Enums;
using Logic.Game.Model;
using Logic.Item.Model;
using Logic.UI.Description.View;
using Logic.Audio.Controller;
using System.Collections.Generic;
using Logic.Hero.Model;
using Logic.Equipment.Model;

namespace Logic.UI.CommonItem.View
{
    public class CommonItemIcon : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/common/common_item_icon";

        public static CommonItemIcon Create(Transform parent)
        {
            GameObject go = ResMgr.instance.Load<GameObject>(PREFAB_PATH);
            go = GameObject.Instantiate<GameObject>(go);
            if (parent != null)
            {
                go.transform.SetParent(parent, false);
                go.transform.localPosition = Vector3.zero;
            }
            return go.GetComponent<CommonItemIcon>();
        }

        private ItemInfo _itemInfo;

        public ItemInfo ItemInfo
        {
            get
            {
                return _itemInfo;
            }
        }


        public bool isSelect;


        public delegate void OnClickHandler(CommonItemIcon icon);
        public OnClickHandler onClickHandler;

        #region UI components
        public RectTransform rectTransform;
		public Image itemQualityFrameImage;
        public Image itemIconImage;
		public Image pieceMarkImage;
        public GameObject selectMark;
        public Text textCount;
        public Text textName;

		public Image roleTypeIcon;
		public Transform starsRoot;
		public List<Image> starImages;
        #endregion UI components

        private ItemDesButton _itemDesButton;
        void Awake()
        {
            SetSelect(false);
            _itemDesButton = ItemDesButton.Get(gameObject);
        }

        private void RefreshItemIconImage()
        {
            Sprite sprite = ResMgr.instance.Load<Sprite>(ResPath.GetItemIconPath(_itemInfo.itemData.icon));
            if (sprite != null)
            {
                itemIconImage.SetSprite(sprite);
                //itemIconImage.SetNativeSize();
            }
        }

        public void Refresh()
        {
			itemQualityFrameImage.SetSprite( UIUtil.GetItemQualityFrameSprite(_itemInfo.itemData.itemQuality));

            RefreshItemIconImage();

			if (_itemInfo.itemData.type == (int)ITEM_TYPE.HeroPiece)
			{
				PieceData heroPieceData = PieceData.GetPieceDataByID(_itemInfo.itemData.id);
				HeroInfo heroInfo = new HeroInfo(heroPieceData.heroGameResData);
				itemIconImage.SetSprite(ResMgr.instance.LoadSprite(heroInfo.HeadIcon));

				for (int i = 0, count = starImages.Count; i < count; i++)
				{
					if (i < heroInfo.advanceLevel)
						starImages[i].SetSprite(ResMgr.instance.LoadSprite("sprite/main_ui/icon_star"));
					else
						starImages[i].SetSprite(ResMgr.instance.LoadSprite("sprite/main_ui/icon_star2_big_disable"));
					starImages[i].gameObject.SetActive(i < heroInfo.MaxAdvanceLevel);
				}

				pieceMarkImage.gameObject.SetActive(true);
				roleTypeIcon.SetSprite(UIUtil.GetRoleTypeSmallIconSprite(heroInfo.heroData.roleType));
				roleTypeIcon.gameObject.SetActive(true);
//				starsRoot.gameObject.SetActive(_itemInfo.itemData.star > 0);
			}
			else if (_itemInfo.itemData.type == (int)ITEM_TYPE.EquipPiece)
			{
				PieceData pieceData = PieceData.GetPieceDataByID(_itemInfo.itemData.id);
				EquipmentData equipmentData = EquipmentData.GetEquipmentDataByID(pieceData.heroGameResData.id);
				itemIconImage.SetSprite(ResMgr.instance.LoadSprite(ResPath.GetEquipmentIconPath(equipmentData.icon)));

				pieceMarkImage.gameObject.SetActive(true);
				roleTypeIcon.SetSprite(UIUtil.GetRoleTypeSmallIconSprite(equipmentData.equipmentRoleType));
				roleTypeIcon.gameObject.SetActive(true);
			}
			else
			{
				pieceMarkImage.gameObject.SetActive(false);
				roleTypeIcon.gameObject.SetActive(false);
//				starsRoot.gameObject.SetActive(false);
			}
			starsRoot.gameObject.SetActive(false);

            textCount.text = _itemInfo.count.ToString();
            if (_itemInfo.count >= 1)
                ShowCount();
            else
                HideCount();

            textName.text = _itemInfo.itemData.id.ToString();
            textName.gameObject.SetActive(false);
        }

        public void SetItemInfo(ItemInfo itemInfo)
        {
            _itemInfo = itemInfo;
            Refresh();
            _itemDesButton.SetItemInfo(_itemInfo);
        }

		public void SetEnableItemDesButton (bool enable)
		{
			_itemDesButton.enabled = enable;
		}

        public void SetGameResData(GameResData gameResData)
        {
            int id = gameResData.id;

            BaseResType type = gameResData.type;
            if (type != BaseResType.Hero && type != BaseResType.Equipment && type != BaseResType.Item)
            {
                ItemData data = ItemData.GetBasicResItemByType(type);
                if (data != null)
                    id = data.id;
            }
            else
            {

            }
            SetItemInfo(new ItemInfo(0, id, gameResData.count));
        }

        public void SetAsCostResources(GameResData gameResData)
        {
            SetSelect(false);
            textName.gameObject.SetActive(false);
            //Sprite iconSprite = null;
            int requiredCount = 0;
            int ownCount = 0;

            BaseResType type = gameResData.type;
            int id = gameResData.id;
            if (type != BaseResType.Hero && type != BaseResType.Equipment && type != BaseResType.Item)
            {

                ownCount = GameProxy.instance.BaseResourceDictionary[gameResData.type];
                ItemData data = ItemData.GetBasicResItemByType(type);
                if (data != null)
                    id = data.id;
            }
            else
            {
                ItemInfo itemInfo = ItemProxy.instance.GetItemInfoByItemID(gameResData.id);
                if (itemInfo != null)
                {
                    ownCount = itemInfo.count;
                }
            }
            requiredCount = gameResData.count;
            //			if (gameResData.type == BaseResType.Gold
            //			    || gameResData.type == BaseResType.Diamond
            //			    || gameResData.type == BaseResType.Crystal
            //			    || gameResData.type == BaseResType.Honor)
            //			{
            //				iconSprite = ResMgr.instance.Load<Sprite>(ResPath.GetItemIconPath(((int)gameResData.type).ToString()));
            //				requiredCount = gameResData.count;
            //				ownCount = GameProxy.instance.BaseResourceDictionary[gameResData.type];
            //			}
            //			else if (gameResData.type == BaseResType.Item)
            //			{
            //				ItemData itemData = ItemData.GetItemDataByID(gameResData.id);
            //				iconSprite = ResMgr.instance.Load<Sprite>(ResPath.GetItemIconPath(itemData.icon));
            //				requiredCount = gameResData.count;
            //
            //				ItemInfo itemInfo = ItemProxy.instance.GetItemInfoByItemID(gameResData.id);
            //				if (itemInfo != null)
            //				{
            //					ownCount = itemInfo.count;
            //				}
            //			}
            //			itemIconImage.sprite = iconSprite;
            //			itemIconImage.SetNativeSize();
            SetItemInfo(new ItemInfo(0, id, gameResData.count));
            textCount.text = ownCount >= requiredCount ? UIUtil.FormatToGreenText(requiredCount.ToString()) : UIUtil.FormatToRedText(requiredCount.ToString());

        }

		public void SetAsRequireResource (GameResData gameResData)
		{
			SetSelect(false);
			textName.gameObject.SetActive(false);
			int requiredCount = 0;
			int ownCount = 0;

			BaseResType type = gameResData.type;
			int id = gameResData.id;
			if (type != BaseResType.Hero && type != BaseResType.Equipment && type != BaseResType.Item)
			{
				
				ownCount = GameProxy.instance.BaseResourceDictionary[gameResData.type];
				ItemData data = ItemData.GetBasicResItemByType(type);
				if (data != null)
					id = data.id;
			}
			else
			{
				ItemInfo itemInfo = ItemProxy.instance.GetItemInfoByItemID(gameResData.id);
				if (itemInfo != null)
				{
					ownCount = itemInfo.count;
				}
			}
			requiredCount = gameResData.count;
			SetItemInfo(new ItemInfo(0, id, gameResData.count));
			string countString = string.Format("{0}/{1}", ownCount, requiredCount);
			textCount.text = ownCount >= requiredCount ? UIUtil.FormatToGreenText(countString) : UIUtil.FormatToRedText(countString);
		}

        public void SetSelect(bool select)
        {
            isSelect = select;
            selectMark.SetActive(select);
        }

        public void ShowSelect()
        {
            selectMark.SetActive(true);
        }
        public void HideSelect()
        {
            selectMark.SetActive(false);
        }
        public void ShowCount()
        {
            textCount.gameObject.SetActive(true);
        }
        public void HideCount()
        {
            textCount.gameObject.SetActive(false);
        }

        public void SetMiddlePointAsPivot()
        {
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }

        public void SetBottomCenterAsPivot()
        {
            rectTransform.pivot = new Vector2(0.5f, 0);
        }

        #region ui event handlers
        public void ClickHandler(CommonItemIcon icon)
        {
            if (onClickHandler != null)
            {
                onClickHandler(icon);
                AudioController.instance.PlayAudio(AudioController.SELECT, false);
            }
        }
        #endregion ui event handlers
    }
}