using UnityEngine;
using UnityEngine.UI;
using Common.ResMgr;
using Common.Util;
using Common.Localization;
using Logic.Enums;
using Logic.Game;
using Logic.Game.Model;
using Logic.Hero.Model;
using Logic.Character;
using Logic.Shop.Model;
using Logic.UI.Shop.Controller;
using Logic.UI.Tips.View;
using Logic.ConsumeTip.Model;

namespace Logic.UI.Shop.View
{
    public class RecruitOneHeroResultView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/recruit_result/recruit_one_hero_result_view";

        private HeroInfo _heroInfo;
        private ShopHeroRandomCardInfo _shopHeroRandomCardInfo;
        private CharacterEntity _characterEntity;

        #region UI components
        public Text textHeroName;
        public GameObject[] heroStars;
        public Transform heroModelRootTransform;
        public Text backText;
        public Image costIconImage;
        public Text againText;
        #endregion UI components

        void Start()
        {
            LTDescr ltDescr = LeanTween.delayedCall(0.6f, OnViewReady);
        }

        private void OnViewReady()
        {
            Observers.Facade.Instance.SendNotification(string.Format("{0}::{1}", PREFAB_PATH, "OnViewReady"));
        }

        public void SetInfo(ShopHeroRandomCardInfo shopHeroCardInfo, HeroInfo heroInfo)
        {
            _shopHeroRandomCardInfo = shopHeroCardInfo;
            _heroInfo = heroInfo;
            ResetHero();
            backText.text = Localization.Get("ui.recruit_one_hero_result_view.back");
			costIconImage.SetSprite(ResMgr.instance.Load<Sprite>(UIUtil.GetBaseResIconPath(_shopHeroRandomCardInfo.ShopCardRandomData.costGameResData.type)));
            costIconImage.SetNativeSize();
            againText.text = string.Format(Localization.Get("ui.recruit_one_hero_result_view.again"), _shopHeroRandomCardInfo.ShopCardRandomData.costGameResData.count.ToString());
        }

        private void ResetHero()
        {
            textHeroName.text = Localization.Get(_heroInfo.heroData.name);
            for (int i = 0; i < heroStars.Length; i++)
            {
                heroStars[i].SetActive(i < _heroInfo.advanceLevel);
            }
            DespawnCharacter();
            TransformUtil.ClearChildren(heroModelRootTransform, true);
            _characterEntity = CharacterEntity.CreateHeroEntityAsUIElement(_heroInfo, heroModelRootTransform, false, true);
        }

        private void DespawnCharacter()
        {
            if (_characterEntity)
                Pool.Controller.PoolController.instance.Despawn(_characterEntity.name, _characterEntity);
            _characterEntity = null;
        }

        void OnDestroy()
        {
            DespawnCharacter();
        }

        #region UI event handlers
        public void ClickAgainHandler()
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
                        if (ConsumeTipProxy.instance.GetConsumeTipEnable(ConsumeTipType.DiamondDrawSingleHero))
                            ConfirmBuyShopItemTipsView.Open(shopItemName, costGameResData, ClickConfirmBuyHandler, ConsumeTipType.DiamondDrawSingleHero);
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

        public void ClickConfirmBuyHandler()
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
            ShopController.instance.CLIENT2LOBBY_PURCHASE_GOODS_REQ(shopID, shopItemID, costType);
        }

        public void ClickBackHander()
        {
            UIMgr.instance.Close(PREFAB_PATH);
        }
        #endregion
    }
}
