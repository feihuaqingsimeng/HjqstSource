using UnityEngine;
using UnityEngine.UI;
using Common.Localization;
using Common.ResMgr;
using Common.UI.Components;
using Logic.Game.Model;
using Logic.Hero.Model;
using Logic.Player.Model;
using Logic.Role.Model;
using Logic.Role;
using Logic.Audio.Controller;

namespace Logic.UI.CommonHeroIcon.View
{
    public class CommonHeroIcon : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/common/common_hero_icon";
        public const string PREFAB_SMALL_PATH = "ui/common/common_hero_small_icon";
        public const string PREFAB_BIG_PATH = "ui/common/common_hero_big_icon";

        public static CommonHeroIcon Create(Transform parent)
        {
            GameObject go = GameObject.Instantiate<GameObject>(ResMgr.instance.Load<GameObject>(PREFAB_PATH));
            if (parent != null)
            {
                go.transform.SetParent(parent, false);
                go.transform.localPosition = Vector3.zero;
            }
            return go.GetComponent<CommonHeroIcon>();
        }
        public static CommonHeroIcon CreateSmallIcon(Transform parent)
        {
            GameObject go = GameObject.Instantiate<GameObject>(ResMgr.instance.Load<GameObject>(PREFAB_SMALL_PATH));
            if (parent != null)
            {
                go.transform.SetParent(parent, false);
                go.transform.localPosition = Vector3.zero;
            }
            return go.GetComponent<CommonHeroIcon>();
        }

        public static CommonHeroIcon CreateBigIcon(Transform parent)
        {
            CommonHeroIcon commonHeroIcon = GameObject.Instantiate<GameObject>(ResMgr.instance.Load<GameObject>(PREFAB_BIG_PATH)).GetComponent<CommonHeroIcon>();
            if (parent != null)
            {
                commonHeroIcon.transform.SetParent(parent, false);
                commonHeroIcon.transform.localPosition = Vector3.zero;
            }
            return commonHeroIcon;
        }

        public GameResData gameResData;

        private HeroInfo _heroInfo;
        public HeroInfo HeroInfo
        {
            get
            {
                return _heroInfo;
            }
        }
        private PlayerInfo _playerInfo;
        public PlayerInfo PlayerInfo
        {
            get
            {
                return _playerInfo;
            }
        }
        private bool _isSelect;
        public bool IsSelect
        {
            get
            {
                return _isSelect;
            }
        }

        private bool _isUsePetHeadIcon;

        public delegate void OnClickHandler(CommonHeroIcon commonHeroIcon);
        public OnClickHandler onClickHandler;

        #region ui components
        public RectTransform rectTransform;
		public Image roleQualityFrame;
        public Image heroIconImage;
        public Text textLevel;
        public Text textStrengthenLevel;
        public Image roleTypeIconImage;
        public GameObject imgSelectMark;
        public GameObject[] starGameObjects;

        public Image inFormationMarkImage;

        #endregion ui components


        private Sprite _starNormalSprite;
        private Sprite _starDefaultSprite;

        void Awake()
        {
            _starNormalSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/icon_star");
            _starDefaultSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/icon_star2_big_disable");

            HideSelect();
            SetInFormation(false);
        }

        private void RefreshHeroImage()
        {
			heroIconImage.SetSprite(ResMgr.instance.Load<Sprite>(_heroInfo.HeadIcon));
        }
        public void SetHeroHeadIcon(string headIcon)
        {
			heroIconImage.SetSprite(ResMgr.instance.Load<Sprite>(headIcon));
        }
        private void RefreshStars()
        {
			/*
            int starCount = starGameObjects.Length;

            if (_starNormalSprite == null || _starDefaultSprite == null)
            {
                for (int i = 0; i < starCount; i++)
                {
                    starGameObjects[i].SetActive(i < _heroInfo.advanceLevel);
                }
            }
            else
            {
                for (int i = 0; i < starCount; i++)
                {
                    starGameObjects[i].GetComponent<Image>().SetSprite( i < _heroInfo.advanceLevel ? _starNormalSprite : _starDefaultSprite);
                    starGameObjects[i].SetActive(i < _heroInfo.MaxAdvanceLevel);
                }
            }
            */

			int starCount = starGameObjects.Length;
			for (int i = 0; i < starCount; i++)
			{
				starGameObjects[i].GetComponent<Image>().SetSprite(_starNormalSprite);
				starGameObjects[i].SetActive(i < _heroInfo.advanceLevel);
			}
        }

        private void RefreshHero()
        {
			if (roleQualityFrame != null)
				roleQualityFrame.SetSprite(UIUtil.GetRoleQualityFrameSprite(_heroInfo.heroData.roleQuality));
            //heroNameText.text = Localization.Get(_heroInfo.heroData.name);
            //heroNameText.color = UIUtil.GetRoleNameColor(_heroInfo);
            RefreshHeroImage();
            RefreshStars();
            if (textLevel != null)
            {
                textLevel.text = string.Format(Localization.Get("common.role_icon.role_lv"), _heroInfo.level);
            }
            if (textStrengthenLevel != null)
            {
                textStrengthenLevel.text = RoleUtil.GetStrengthenAddShowValueString(_heroInfo);
            }
            if (roleTypeIconImage != null)
            {
				roleTypeIconImage.SetSprite(UIUtil.GetRoleTypeSmallIconSprite(_heroInfo.heroData.roleType));
            }

        }
        private void RefreshPlayer()
        {
			if (roleQualityFrame != null)
				roleQualityFrame.SetSprite(UIUtil.GetRoleQualityFrameSprite(_playerInfo.heroData.roleQuality));
			heroIconImage.SetSprite(_isUsePetHeadIcon ? ResMgr.instance.Load<Sprite>(_playerInfo.PetHeadIcon) : ResMgr.instance.Load<Sprite>(_playerInfo.HeadIcon));
            int starCount = starGameObjects.Length;
            for (int i = 0; i < starCount; i++)
            {
                starGameObjects[i].SetActive(i < _playerInfo.advanceLevel);
            }
            if (textLevel != null)
            {
                textLevel.text = string.Format(Localization.Get("common.role_icon.role_lv"), _playerInfo.level);
            }
            if (textStrengthenLevel != null)
            {
                textStrengthenLevel.text = RoleUtil.GetStrengthenAddShowValueString(_playerInfo);
            }
            if (roleTypeIconImage != null)
            {
				roleTypeIconImage.SetSprite(UIUtil.GetRoleTypeSmallIconSprite(_playerInfo.heroData.roleType));
            }


        }
        public void SetHeroInfo(HeroInfo heroInfo)
        {
            _heroInfo = heroInfo;
            RefreshHero();
        }
        public void SetPlayerInfo(PlayerInfo playerInfo)
        {
            _playerInfo = playerInfo;
            RefreshPlayer();
        }
        public void SetRoleInfo(RoleInfo roleInfo)
        {
            _heroInfo = roleInfo as HeroInfo;
            if (_heroInfo != null)
            {
                SetHeroInfo(_heroInfo);
            }
            else
            {
                _playerInfo = roleInfo as PlayerInfo;
                if (_playerInfo != null)
                    SetPlayerInfo(_playerInfo);
            }
        }
        public void UsePetIcon()
        {
//            _isUsePetHeadIcon = true;
//            if (_playerInfo != null)
//                heroIconImage.sprite = ResMgr.instance.Load<Sprite>(_playerInfo.PetHeadIcon);

        }
        public void SetGameResData(GameResData gameResData)
        {
            this.gameResData = gameResData;
            _heroInfo = new HeroInfo(0, gameResData.id, 0, 0, gameResData.star);
            RefreshHero();
        }

        public void SetButtonEnable(bool enable)
        {
            Button b = transform.GetComponent<Button>();
            if (b != null)
                b.enabled = enable;
        }
        public void HideStar()
        {
            starGameObjects[0].transform.parent.gameObject.SetActive(false);
        }
        public void ShowStar()
        {
            starGameObjects[0].transform.parent.gameObject.SetActive(true);
        }
        public void SetInFormation(bool inFormation)
        {
            if (inFormationMarkImage != null)
            {
                inFormationMarkImage.gameObject.SetActive(inFormation);
            }
        }

        public void SetSelect(bool select)
        {
            _isSelect = select;
            if (imgSelectMark != null)
                imgSelectMark.SetActive(select);
        }
        public void HideSelect()
        {
            if (imgSelectMark != null)
                imgSelectMark.SetActive(false);
        }
        public void ShowSelect()
        {
            if (imgSelectMark != null)
                imgSelectMark.SetActive(true);
        }
        public void HideLevel()
        {
            if (textLevel != null)
                textLevel.gameObject.SetActive(false);
            if (textStrengthenLevel != null)
                textStrengthenLevel.gameObject.SetActive(false);
        }
        public void ShowLevel()
        {
            if (textLevel != null)
                textLevel.gameObject.SetActive(true);
            if (textStrengthenLevel != null)
                textStrengthenLevel.gameObject.SetActive(true);
        }
        public void SetMiddlePointAsPivot()
        {
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }

        public void SetBottomCenterAsPivot()
        {
            rectTransform.pivot = new Vector2(0.5f, 0);
        }

        #region UI event handlers
        public void ClickHandler(CommonHeroIcon commonHeroIcon)
        {
            if (onClickHandler != null)
            {
                onClickHandler(commonHeroIcon);
                AudioController.instance.PlayAudio(AudioController.SELECT, false);
            }
        }
        #endregion UI event handlers
    }
}