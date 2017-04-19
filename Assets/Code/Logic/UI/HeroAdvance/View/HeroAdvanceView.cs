using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Common.Localization;
using Common.Util;
using Logic.Hero.Model;
using Logic.Hero.Controller;
using Logic.Character;
using Logic.Hero;
using Logic.UI.Hero.View;
using Logic.Hero.Model.Advance;
using Logic.UI.CommonItem.View;
using Common.ResMgr;
using Logic.Item.Model;
using Logic.UI.HeroAdvance.Model;
using Logic.UI.Tips.View;
using Logic.Game.Model;
using Logic.Enums;
using Logic.UI.CommonTopBar.View;
using Logic.UI.CommonReward.View;
using Logic.Role;

namespace Logic.UI.HeroAdvance.View
{
    public class HeroAdvanceView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/hero_advance/hero_advance_view";

        //private HeroInfo _heroInfo;
        private CharacterEntity _characterEntity1, _characterEntity2;
        #region UI components
        public GameObject core;
        private CommonTopBarView _commonTopBarView;

        public Text attributesAfterAdvanceTitleText;
        public Text startAdvanceText;
        public Text needLevelText;
        public Text beforeAdvanceNameText;
        public Text beforeAdvanceLeveText;
        public Image beforeAdvanceRoleTypeImage;
        public Text afterAdvanceNameText;
        public Text afterAdvanceLeveText;
        public Image afterAdvanceRoleTypeImage;

        public GameObject advanceOperationRootGameObject;
        public Text coinText;
        public Transform materialRoot;
        public Text starReachMaxText;
        public Transform attributeRoot;
        public GameObject attributePrefab;

        public Image[] beforeAdvanceStarImages;
        public Image[] afterAdvanceStarImages;

        public Text[] materialCountText;
        public Transform beforeAdvanceModelRootTransform;
        public Transform afterAdvanceModelRootTransform;

        public GameObject centerPartGameObject;
        public GameObject beforeAdvanceHeroRootGameObject;
        public GameObject afterAdvanceHeroRootGameObject;
        public GameObject arrowGameObject;
        #endregion

        void Awake()
        {
            Init();
            BindDelegate();
        }

        void OnDestroy()
        {
            DespawnCharacter(_characterEntity1);
            DespawnCharacter(_characterEntity2);
            UnbindDelegate();
        }

        private void Init()
        {
            string title = Localization.Get("ui.hero_advance_view.hero_advance_title");
            _commonTopBarView = CommonTopBarView.CreateNewAndAttachTo(core.transform);
            _commonTopBarView.SetAsCommonStyle(title, ClickCloseHandler, true, true, true, false);
            attributesAfterAdvanceTitleText.text = Localization.Get("ui.hero_advance_view.attributes_after_advance_title");

            startAdvanceText.text = Localization.Get("ui.hero_advance_view.start_advance_title");
            starReachMaxText.text = Localization.Get("ui.hero_advance_view.star_reach_max");
        }

        private void BindDelegate()
        {
            HeroAdvanceProxy.instance.OnUpdateHeroAdvanceByProtocolDelegate = UpdateHeroAdvanceByProtocol;
        }

        private void UnbindDelegate()
        {
            HeroAdvanceProxy.instance.OnUpdateHeroAdvanceByProtocolDelegate = null;
        }

        public void SetHeroInfo(HeroInfo heroInfo)
        {

            HeroAdvanceProxy.instance.advanceHeroInfo = heroInfo;
            Debugger.Log(string.Format("进阶英雄id:{0},类型:{1},level:{2},strengthenLevel:{3}", heroInfo.instanceID, (int)heroInfo.heroData.roleType, heroInfo.level, heroInfo.strengthenLevel));
            beforeAdvanceNameText.text = RoleUtil.GetRoleNameWithStrengthenLevel(heroInfo);
            HeroInfo mockAfterAdvanceHeroInfo = new HeroInfo(heroInfo.instanceID, heroInfo.heroData.id, heroInfo.breakthroughLevel, heroInfo.strengthenLevel, heroInfo.advanceLevel, heroInfo.level);
            afterAdvanceNameText.text = RoleUtil.GetRoleNameWithStrengthenLevel(heroInfo);
            Refresh();
        }

        private void Refresh()
        {
            ResetHeroModels();
            RefreshAttribute();
            RefreshMoney();
            RefreshMaterial();
        }

        void DespawnCharacter(CharacterEntity characterEntity)
        {
            if (characterEntity)
                Pool.Controller.PoolController.instance.Despawn(characterEntity.name, characterEntity);
        }

        private void ResetHeroModels()
        {
            HeroInfo heroInfo = HeroAdvanceProxy.instance.advanceHeroInfo;

            for (int i = 0; i < beforeAdvanceStarImages.Length; i++)
            {
                beforeAdvanceStarImages[i].gameObject.SetActive(false);
                if (heroInfo.advanceLevel > i)
                {
                    beforeAdvanceStarImages[i].gameObject.SetActive(true);
                }
            }
            beforeAdvanceLeveText.text = heroInfo.level.ToString();
			beforeAdvanceRoleTypeImage.SetSprite(UIUtil.GetRoleTypeSmallIconSprite(heroInfo.heroData.roleType));
            beforeAdvanceRoleTypeImage.SetNativeSize();
            DespawnCharacter(_characterEntity1);
            TransformUtil.ClearChildren(beforeAdvanceModelRootTransform, true);
            _characterEntity1 = CharacterEntity.CreateHeroEntityAsUIElement(heroInfo, beforeAdvanceModelRootTransform, false, true);

            DespawnCharacter(_characterEntity2);
            if (!heroInfo.IsMaxAdvanceLevel)
            {
                HeroInfo mockAfterAdvanceHeroInfo = new HeroInfo(heroInfo.instanceID, heroInfo.heroData.id,
                                                                 heroInfo.breakthroughLevel, heroInfo.strengthenLevel,
                                                                 heroInfo.advanceLevel + 1);

                for (int i = 0; i < afterAdvanceStarImages.Length; i++)
                {
                    afterAdvanceStarImages[i].gameObject.SetActive(false);
                    if (mockAfterAdvanceHeroInfo.advanceLevel > i)
                    {
                        afterAdvanceStarImages[i].gameObject.SetActive(true);
                    }
                }
                afterAdvanceLeveText.text = "1";
				afterAdvanceRoleTypeImage.SetSprite(UIUtil.GetRoleTypeSmallIconSprite(mockAfterAdvanceHeroInfo.heroData.roleType));
                afterAdvanceRoleTypeImage.SetNativeSize();
                TransformUtil.ClearChildren(afterAdvanceModelRootTransform, true);
                _characterEntity2 = CharacterEntity.CreateHeroEntityAsUIElement(mockAfterAdvanceHeroInfo, afterAdvanceModelRootTransform, false, true);
                arrowGameObject.SetActive(true);
            }
            else
            {
                //				centerPartGameObject.SetActive(false);
                afterAdvanceHeroRootGameObject.SetActive(false);
                TransformUtil.ClearChildren(afterAdvanceModelRootTransform, true);
                LeanTween.moveLocalX(beforeAdvanceHeroRootGameObject, 0, 0.2f);
                arrowGameObject.SetActive(false);
            }
        }

        private void RefreshAttribute()
        {
            HeroInfo heroInfo = HeroAdvanceProxy.instance.advanceHeroInfo;
            TransformUtil.ClearChildren(attributeRoot, true);
            List<RoleAttribute> attributeList = HeroUtil.CalcHeroMainAttributesList(heroInfo);
            int count = attributeList.Count;
            if (!heroInfo.IsMaxAdvanceLevel)
            {
                //next
                HeroInfo nextInfo = new HeroInfo(heroInfo.instanceID, heroInfo.heroData.id, heroInfo.breakthroughLevel, heroInfo.strengthenLevel, heroInfo.advanceLevel + 1, heroInfo.level);
                List<RoleAttribute> nextAttributeList = HeroUtil.CalcHeroMainAttributesList(nextInfo);
                attributePrefab.SetActive(true);
                for (int i = 0; i < count; i++)
                {
                    GameObject go = Instantiate<GameObject>(attributePrefab);
                    go.transform.SetParent(attributeRoot, false);
                    RoleAttributeView view = go.GetComponent<RoleAttributeView>();
                    int add = (int)(nextAttributeList[i].value - attributeList[i].value);
                    view.Set(attributeList[i], add, false);

                }
                attributePrefab.SetActive(false);
            }
            else
            {
                attributePrefab.SetActive(true);
                for (int i = 0; i < count; i++)
                {
                    GameObject go = Instantiate<GameObject>(attributePrefab);
                    go.transform.SetParent(attributeRoot, false);
                    RoleAttributeView roleAttributeView = go.GetComponent<RoleAttributeView>();
                    roleAttributeView.Set(attributeList[i], Localization.Get("ui.hero_advance_view.role_attribute_max_remark"));
                }
                attributePrefab.SetActive(false);
            }
        }
        private void RefreshMoney()
        {
            HeroInfo heroInfo = HeroAdvanceProxy.instance.advanceHeroInfo;

            HeroAdvanceData data = HeroAdvanceData.GetHeroAdvanceDataByStar(heroInfo.advanceLevel);
            int needMoney = 0;
            if (data != null)
            {
                needMoney = data.gold;
            }
            int money = GameProxy.instance.BaseResourceDictionary.GetValue(BaseResType.Gold);
            if (money < needMoney)
            {
                coinText.text = UIUtil.FormatToRedText(needMoney.ToString());
            }
            else
            {
                coinText.text = UIUtil.FormatToGreenText(needMoney.ToString());
            }

        }
        private void RefreshMaterial()
        {
            HeroInfo heroInfo = HeroAdvanceProxy.instance.advanceHeroInfo;

            for (int i = 0, count = materialCountText.Length; i < count; i++)
            {
                materialCountText[i].gameObject.SetActive(false);
                CommonRewardIcon icon = materialCountText[i].transform.parent.GetComponentInChildren<CommonRewardIcon>();
                if (icon != null)
                    GameObject.DestroyImmediate(icon.gameObject);
            }

            if (heroInfo.IsMaxAdvanceLevel)
            {

                advanceOperationRootGameObject.SetActive(false);
                starReachMaxText.gameObject.SetActive(true);
                return;
            }

            HeroAdvanceData advanceData = HeroAdvanceData.GetHeroAdvanceDataByStar(heroInfo.advanceLevel);
            if (advanceData == null)
            {
                return;
            }
            ItemInfo itemInfo;
            GameResData resData;
            GridLayoutGroup materialRootGridLayout = materialRoot.GetComponent<GridLayoutGroup>();
            List<GameResData> itemDataList = advanceData.GetItemIdByHeroType(heroInfo.heroData.roleType);
            Transform parent;
            for (int i = 0, dataListCount = itemDataList.Count; i < dataListCount; i++)
            {
                if (i < materialRoot.childCount)
                    parent = materialRoot.GetChild(i);
                else
                    parent = materialRoot;
                resData = itemDataList[i];
                itemInfo = ItemProxy.instance.GetItemInfoByItemID(resData.id);

                CommonRewardIcon item = CommonRewardIcon.Create(parent);
                RectTransform itemTran = item.transform as RectTransform;
                itemTran.localPosition = Vector3.zero;
                float width = itemTran.sizeDelta.x;
                item.SetGameResData(resData);
                float scale = materialRootGridLayout.cellSize.x / width;
                item.transform.localScale = new Vector3(scale, scale, 1);
                item.HideCount();
                item.onClickHandler = ClickMaterialHandler;
                int count = itemInfo == null ? 0 : itemInfo.count;
                string text = string.Format(Localization.Get("common.value/max"), count, resData.count);
                if (count < resData.count)
                {
                    text = UIUtil.FormatToRedText(text);
                }
                else
                {
                    text = UIUtil.FormatToGreenText(text);
                }
                Text countText = parent.Find("text_count").GetComponent<Text>();
                countText.text = text;
                countText.gameObject.SetActive(true);

            }
            string needLvString = string.Format(Localization.Get("ui.hero_advance_view.need_level"), advanceData.lv_limit);
            if (heroInfo.level < advanceData.lv_limit)
            {
                needLevelText.text = UIUtil.FormatToRedText(needLvString);
            }
            else
            {
                needLevelText.text = UIUtil.FormatToColorText(needLvString, "FFAB25");//深黄
            }
            advanceOperationRootGameObject.SetActive(true);
            starReachMaxText.gameObject.SetActive(false);
        }
        #region UI event handlers
        public void ClickCloseHandler()
        {
            UIMgr.instance.Close(PREFAB_PATH);
        }
        public void ClickMaterialHandler(GameObject go)
        {
            //Logic.UI.Item.View.ItemBagView.Open();

            GoodsJump.View.GoodsJumpPathView.Open(go.GetComponent<CommonRewardIcon>().GameResData);
        }
        public void ClickStartAdvanceHandler()
        {

            HeroInfo heroInfo = HeroAdvanceProxy.instance.advanceHeroInfo;

            HeroAdvanceData advanceData = HeroAdvanceData.GetHeroAdvanceDataByStar(heroInfo.advanceLevel);
            List<GameResData> itemDataList = advanceData.GetItemIdByHeroType(heroInfo.heroData.roleType);


            if (heroInfo.level < advanceData.lv_limit)
            {
                CommonErrorTipsView.Open(string.Format(Localization.Get("ui.hero_advance_view.level_not_enough"), advanceData.lv_limit));
                return;
            }
            GameResData resData;
            for (int i = 0, count = itemDataList.Count; i < count; i++)
            {
                resData = itemDataList[i];
                ItemInfo itemInfo = ItemProxy.instance.GetItemInfoByItemID(resData.id);
                if (itemInfo == null || itemInfo.count < resData.count)
                {
                    CommonErrorTipsView.Open(Localization.Get("ui.hero_advance_view.material_not_enough"));
                    return;
                }
            }

            int money = GameProxy.instance.BaseResourceDictionary.GetValue(BaseResType.Gold);
            if (money < advanceData.gold)
            {
                CommonErrorTipsView.Open(Localization.Get("ui.hero_advance_view.gold_not enough"));
                return;
            }

            HeroController.instance.CLIENT2LOBBY_HERO_ADVANCE_REQ((int)heroInfo.instanceID, 0);
        }
        #endregion

        #region proxy callback handlers
        public void UpdateHeroAdvanceByProtocol(bool isSuccess)
        {
            if (!isSuccess)
            {
                CommonErrorTipsView.Open(Localization.Get("ui.hero_advance_view.advance_fail"));
            }
            HeroAdvanceProxy.instance.advanceHeroInfo = HeroProxy.instance.GetHeroInfo(HeroAdvanceProxy.instance.advanceHeroInfo.instanceID);
            Refresh();
        }

        #endregion
    }
}