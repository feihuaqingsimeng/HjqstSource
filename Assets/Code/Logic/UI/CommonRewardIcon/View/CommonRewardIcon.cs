using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Logic.Game.Model;
using Common.ResMgr;
using Logic.Enums;
using Logic.Equipment.Model;
using Logic.Hero.Model;
using Logic.Item.Model;
using Logic.UI.CommonItem.View;
using Logic.UI.CommonEquipment.View;
using Common.Localization;
using Logic.Role.Model;
using Logic.Player.Model;
using UnityEngine.EventSystems;
using Logic.UI.Description.View;
using Logic.Audio.Controller;

namespace Logic.UI.CommonReward.View
{
    public class CommonRewardIcon : MonoBehaviour //,IPointerDownHandler,IPointerUpHandler
    {

        public const string PREFAB_PATH = "ui/common/common_reward_icon";


        public static CommonRewardIcon Create(Transform parent)
        {
            GameObject go = ResMgr.instance.Load<GameObject>(PREFAB_PATH);
            go = GameObject.Instantiate<GameObject>(go);
            go.transform.SetParent(parent, false);
            return go.GetComponent<CommonRewardIcon>();
        }
        private RoleInfo _roleInfo;
        public RoleInfo RoleInfo
        {
            get
            {
                return _roleInfo;
            }
        }
        private UI.CommonHeroIcon.View.CommonHeroIcon _commonHeroIcon;
        public UI.CommonHeroIcon.View.CommonHeroIcon CommonHeroIcon
        {
            get
            {
                return _commonHeroIcon;
            }
        }
        private CommonEquipmentIcon _commonEquipIcon;
        public CommonEquipmentIcon CommonEquipmentIcon
        {
            get
            {
                return _commonEquipIcon;
            }
        }
        private CommonItemIcon _commonItemIcon;
        public CommonItemIcon CommonItemIcon
        {
            get
            {
                return _commonItemIcon;
            }
        }
        private GameResData _GameResData;
        public GameResData GameResData
        {
            get
            {
                return _GameResData;
            }
        }

        public System.Action<GameObject> _onClickHandler;
        public System.Action<GameObject> onClickHandler
        {
            get
            {
                return _onClickHandler;
            }
            set
            {
                _onClickHandler = value;
                switch (_GameResData.type)
                {
                    case BaseResType.Equipment:
                        _commonEquipIcon.onClickHandler = ClickEquipHandler;
                        break;
                    case BaseResType.Item:
                        _commonItemIcon.onClickHandler = ClickItemHandler;
                        break;
                    default:
                        break;
                }

            }
        }

        #region UI components
        public Image heroHeadIcon;
        public GameObject roleRoot;
        public Image roleTypeIconImage;
        public GameObject[] starGameObjects;
        public GameObject selectImage;
        public Text nameText;
        public Text rewardCountText;
        #endregion UI components

        private Sprite _starNormalSprite;
        private Sprite _starDefaultSprite;
        void Awake()
        {

            _starNormalSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/icon_star");
            _starDefaultSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/icon_star2_big_disable");
            if (selectImage == null)
            {
                Transform t = roleRoot.transform.Find("img_select");
                if (t != null)
                    selectImage = t.gameObject;
            }

            SetSelect(false);
            ShowName(false);
            ShowRewardCount(false);
        }

        private void Refresh()
        {
            switch (_GameResData.type)
            {
                case BaseResType.Hero:
                    RefreshHero();
                    break;
                case BaseResType.Equipment:
                    RefreshEquipment();
                    break;
                default:
                    RefreshItem();

                    break;
            }


        }
        public void SetDesButtonEnable(bool value)
        {
			if (_roleInfo != null && GetComponentInChildren<RoleDesButton>() != null)
                GetComponentInChildren<RoleDesButton>().enabled = value;
			else if (_commonEquipIcon != null && GetComponentInChildren<EquipmentDesButton>() != null)
                GetComponentInChildren<EquipmentDesButton>().enabled = value;
			else if (_commonItemIcon != null && GetComponentInChildren<ItemDesButton>() != null)
                GetComponentInChildren<ItemDesButton>().enabled = value;
        }
        public void SetDesButtonType(ShowDescriptionType type)
        {
			if (_roleInfo != null && GetComponentInChildren<RoleDesButton>() != null)
                GetComponentInChildren<RoleDesButton>().SetType(type);
			else if (_commonEquipIcon != null && GetComponentInChildren<EquipmentDesButton>()!=null)
                GetComponentInChildren<EquipmentDesButton>().SetType(type);
			else if (_commonItemIcon != null && GetComponentInChildren<ItemDesButton>() != null)
                GetComponentInChildren<ItemDesButton>().SetType(type);
        }
        public void SetSelect(bool isSelect)
        {
            if (_commonEquipIcon != null)
                _commonEquipIcon.SetSelect(isSelect);
            else if (_commonItemIcon != null)
                _commonItemIcon.SetSelect(isSelect);
            else if (selectImage != null)
                selectImage.SetActive(isSelect);

        }
        public bool IsSelect
        {
            get
            {
                bool isSelect = false;
                if (_commonEquipIcon != null)
                    isSelect = _commonEquipIcon.isSelect;
                else if (_commonItemIcon != null)
                    isSelect = _commonItemIcon.isSelect;
                return isSelect;
            }

        }
        private void RefreshStars()
        {

            int starCount = starGameObjects.Length;
            if (_starNormalSprite == null || _starDefaultSprite == null)
            {
                for (int i = 0; i < starCount; i++)
                {
                    starGameObjects[i].SetActive(i < _GameResData.star);
                }
            }
            else
            {
                for (int i = 0; i < starCount; i++)
                {
					starGameObjects[i].GetComponent<Image>().SetSprite(i < _GameResData.star ? _starNormalSprite : _starDefaultSprite);
                    starGameObjects[i].SetActive(i < _roleInfo.MaxAdvanceLevel);
                }
            }

        }
        private void RefreshHero()
        {
            //            roleRoot.SetActive(true);
            //            if (_roleInfo is PlayerInfo)
            //            {
            //                heroHeadIcon.sprite = ResMgr.instance.Load<Sprite>((_roleInfo as PlayerInfo).PetHeadIcon);
            //            }
            //            else
            //            {
            //                heroHeadIcon.sprite = ResMgr.instance.Load<Sprite>(_roleInfo.HeadIcon);
            //            }
            //            RefreshStars();
            //
            //            if (roleTypeIconImage != null)
            //            {
            //                roleTypeIconImage.sprite = UIUtil.GetRoleTypeSmallIconSprite(_roleInfo.heroData.roleType);
            //                roleTypeIconImage.gameObject.SetActive(true);
            //            }
            //            SetName(Localization.Get(_roleInfo.heroData.name));

            roleRoot.SetActive(false);
            if (_commonHeroIcon == null)
            {
                _commonHeroIcon = UI.CommonHeroIcon.View.CommonHeroIcon.Create(transform);
            }

            if (_roleInfo is PlayerInfo)
            {
                _commonHeroIcon.SetPlayerInfo(_roleInfo as PlayerInfo);
            }
            else
            {
                _commonHeroIcon.SetHeroInfo(_roleInfo as HeroInfo);
            }

			RoleDesButton btn = RoleDesButton.Get(_commonHeroIcon.gameObject);
			btn.SetRoleInfo(_roleInfo);

			if (_commonHeroIcon != null)
			{
				_commonHeroIcon.gameObject.SetActive(true);
			}
			if (_commonEquipIcon != null)
			{
				_commonEquipIcon.gameObject.SetActive(false);
			}
			if (_commonItemIcon != null)
			{
				_commonItemIcon.gameObject.SetActive(false);
			}
        }
        private void RefreshEquipment()
        {
            roleRoot.SetActive(false);

            gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            CommonEquipmentIcon icon = GetComponentInChildren<CommonEquipmentIcon>();
            if (icon == null)
                icon = CommonEquipmentIcon.Create(transform);
            icon.SetGameResData(_GameResData);
            _commonEquipIcon = icon;
            if (roleTypeIconImage != null)
            {
                roleTypeIconImage.gameObject.SetActive(false);
            }
            SetName(Localization.Get(_commonEquipIcon.EquipmentInfo.equipmentData.name));

			if (_commonHeroIcon != null)
			{
				_commonHeroIcon.gameObject.SetActive(false);
			}
			if (_commonEquipIcon != null)
			{
				_commonEquipIcon.gameObject.SetActive(true);
			}
			if (_commonItemIcon != null)
			{
				_commonItemIcon.gameObject.SetActive(false);
			}
        }
        private void SetName(string name)
        {
            if (nameText != null)
            {
                nameText.text = name;
            }
        }
        private void RefreshItem()
        {

            roleRoot.SetActive(false);
            gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            CommonItemIcon icon = GetComponentInChildren<CommonItemIcon>();
            if (icon == null)
                icon = CommonItemIcon.Create(transform);
            icon.SetGameResData(_GameResData);
            _commonItemIcon = icon;
            if (roleTypeIconImage != null)
            {
                roleTypeIconImage.gameObject.SetActive(false);
            }
            SetName(Localization.Get(_commonItemIcon.ItemInfo.itemData.name));

			if (_commonHeroIcon != null)
			{
				_commonHeroIcon.gameObject.SetActive(false);
			}
			if (_commonEquipIcon != null)
			{
				_commonEquipIcon.gameObject.SetActive(false);
			}
			if (_commonItemIcon != null)
			{
				_commonItemIcon.gameObject.SetActive(true);
			}
        }

        public void SetGameResData(GameResData gameResData)
        {
            SetGameResData(gameResData, false);
        }
        public void SetGameResData(GameResData gameResData, bool heroIsPlayer)
        {
            _GameResData = gameResData;

            if (_GameResData.type == BaseResType.Hero)
            {
                if (heroIsPlayer && PlayerData.GetPlayerData((uint)gameResData.id) != null)
                    _roleInfo = new PlayerInfo((uint)0, (uint)gameResData.id, (uint)0, (uint)0, (uint)0, 0, "");
                else
                    _roleInfo = new HeroInfo(0, gameResData.id, 0, 0, gameResData.star);
                

            }

            Refresh();
        }
        public void ShowName(bool flag)
        {
            if (nameText != null)
            {
                nameText.gameObject.SetActive(flag);
            }
        }
        public void HideCount(bool isHide = true)
        {
            if (_commonItemIcon != null)
                _commonItemIcon.HideCount();
        }

        public void ShowRewardCount(bool show)
        {

            if (rewardCountText != null)
            {
                if (show)
                {
                    HideCount(true);
                    if (GameResData != null)
                        rewardCountText.text = string.Format(Localization.Get("common.x_count"), GameResData.count);
                    rewardCountText.gameObject.SetActive(true);
                }
                else
                {
                    HideCount(false);
                    rewardCountText.gameObject.SetActive(false);
                }
            }

        }

        public void SetMiddlePointAsPivot()
        {
            (transform as RectTransform).pivot = new Vector2(0.5f, 0.5f);
        }

        public void SetBottomCenterAsPivot()
        {
            (transform as RectTransform).pivot = new Vector2(0.5f, 0);
        }

        #region ui event handlers
        private void ClickEquipHandler(CommonEquipmentIcon icon)
        {
            ClickHandler();
        }
        private void ClickItemHandler(CommonItemIcon icon)
        {
            ClickHandler();
        }
        public void ClickHandler()
        {
            if (onClickHandler != null)
            {
                onClickHandler(gameObject);
                if (_GameResData.type == BaseResType.Hero)
                    AudioController.instance.PlayAudio(AudioController.SELECT, false);

            }
        }
        public void OnPointerDown(PointerEventData data)
        {
            //Debug.Log("down");
        }
        public void OnPointerUp(PointerEventData data)
        {
            // Debug.Log("up");
        }
        #endregion ui event handlers
    }
}

