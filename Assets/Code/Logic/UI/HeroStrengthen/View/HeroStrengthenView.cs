using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Util;
using Common.Localization;
using Logic.Hero.Model;
using Logic.Hero.Controller;
using Logic.Hero;
using Logic.UI.HeroStrengthen.Model;
using Logic.Game.Model;
using Logic.UI.CommonPopupView;
using Logic.Enums;
using Logic.UI.Tips.View;
using Common.UI.Components;
using Logic.UI.CommonHeroIcon.View;
using Logic.Role.Model;
using Logic.Role;
using Logic.Player.Model;
using Logic.UI.CommonTopBar.View;
using Common.ResMgr;
using System.Collections;

namespace Logic.UI.HeroStrengthen.View
{
    public class HeroStrengthenView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/hero_strengthen/hero_strengthen_view";

        public static HeroStrengthenView Open(RoleInfo info)
        {
            HeroStrengthenView view = UIMgr.instance.Open<HeroStrengthenView>(PREFAB_PATH);
            view.SetRoleInfo(info);
            return view;
        }
        #region UI components

        public GameObject core;
        private CommonTopBarView _commonTopBarView;

        public Button[] selectedMaterialHeroFrameButtons;
        //public HeroButton[] selectedMaterialHeroButtons;
        public Text attributesTitleText;
        public Text currentCriticalChanceTitleText;
        public Text improveCriticalChanceTitleText;
        public Text currentCriticalChangeText;
        public Text strengthenTipsText;
        public Text strengthenLevelDesText;
        public Text strengthenLevelText;
        public Text addExpText;
        public Text strengthenText;
        public Text coinText;
        public Text selectStrengthenMaterialTitleText;
        public Dropdown dropDownSort;

        public Slider currentExpSlider;
        public Slider addExpSlider;
        public Transform heroButtonsRootTransform;
        public Transform strengthenHeroButtonRoot;
        public AttributeView heroAttributeViewPrefab;
        public Transform heroAttributeViewRoot;
        public ScrollContent scrollContent;
        public Text noAvailableStrengthenMaterialText;
        #endregion

        private CommonHeroIcon.View.CommonHeroIcon _strengthenHeroButton;
        private bool _isReachMaxLevel;
        private int strengthenAddLevel;
        private bool _isClickStrengthenBtn;
        private Canvas _rootCanvas;
        void Awake()
        {
            Init();
            BindDelegate();
        }

        void OnDestroy()
        {
            UnbindDelegate();
        }

        private void Init()
        {
            string title = Localization.Get("ui.hero_strengthen_view.hero_strengthen_title");
            _commonTopBarView = CommonTopBarView.CreateNewAndAttachTo(core.transform);
            _commonTopBarView.SetAsCommonStyle(title, ClickCloseHandler, true, true, true, false);

            attributesTitleText.text = Localization.Get("ui.hero_strengthen_view.attributes_title");
            currentCriticalChanceTitleText.text = Localization.Get("ui.hero_strengthen_view.current_critical_chance_title");
            improveCriticalChanceTitleText.text = Localization.Get("ui.hero_strengthen_view.improve_critical_chance_title");
            strengthenTipsText.text = Localization.Get("ui.hero_strengthen_view.strengthen_tips");
            //currentExpTitleText.text = Localization.Get("ui.hero_strengthen_view.current_exp_title");

            strengthenText.text = Localization.Get("ui.hero_strengthen_view.strengthen_text");
            selectStrengthenMaterialTitleText.text = Localization.Get("ui.hero_strengthen_view.select_strengthen_material_title");
            noAvailableStrengthenMaterialText.text = Localization.Get("ui.hero_strengthen_view.no_available_strengthen_material");

            string[] choice = new string[]{
				Localization.Get("ui.hero_strengthen_view.sort_ascending"),
				Localization.Get("ui.hero_strengthen_view.sort_descending")
			};

        }

        private void BindDelegate()
        {
            //HeroProxy.instance.onHeroStrengthenSuccessDelegate += OnStrengthenSuccessHandler;
            HeroStrengthenProxy.instance.onHeroStrengthenSuccessDelegate = OnStrengthenSuccessHandler;

        }

        private void UnbindDelegate()
        {
            //HeroProxy.instance.onHeroStrengthenSuccessDelegate -= OnStrengthenSuccessHandler;
            HeroStrengthenProxy.instance.onHeroStrengthenSuccessDelegate = null;
        }

        //		public void SetHeroInfo ( heroInfo)
        //		{
        //			HeroStrengthenProxy.instance.StrengthenHeroInfo = heroInfo;
        //			HeroStrengthenProxy.instance.ClearMaterials();
        //
        //			Debugger.Log(string.Format("强化英雄id:{0},dataid:{1},level:{2},exp:{3}",heroInfo.instanceID,heroInfo.heroData.id,heroInfo.strengthenLevel,heroInfo.strengthenExp));
        //
        //			_strengthenHeroButton = CommonHeroIcon.View.CommonHeroIcon.Create(strengthenHeroButtonRoot);
        //			_strengthenHeroButton.SetHeroInfo(heroInfo);
        //
        //			RegenerateHeroButtons();
        //			RefreshSelectedMaterialHeroButtons();
        //			RefreshAttribute();
        //
        //		}
        public void SetRoleInfo(RoleInfo roleInfo)
        {
            if (roleInfo.GetType() == typeof(PlayerInfo))
            {
                _commonTopBarView.titleText.text = Localization.Get("ui.hero_strengthen_view.player_strengthen_title");
            }

            HeroStrengthenProxy.instance.StrengthenHeroInfo = roleInfo;
            HeroStrengthenProxy.instance.ClearMaterials();

            Debugger.Log(string.Format("强化英雄id:{0},dataid:{1},level:{2},exp:{3}", roleInfo.instanceID, roleInfo.modelDataId, roleInfo.strengthenLevel, roleInfo.strengthenExp));

            _strengthenHeroButton = CommonHeroIcon.View.CommonHeroIcon.CreateBigIcon(strengthenHeroButtonRoot);

            _strengthenHeroButton.SetRoleInfo(HeroStrengthenProxy.instance.StrengthenHeroInfo);
            _strengthenHeroButton.UsePetIcon();
            RegenerateHeroButtons();
            RefreshSelectedMaterialHeroButtons();
            RefreshAttribute();

        }
        public void RefreshExp(int addExp)
        {
            RoleInfo heroInfo = HeroStrengthenProxy.instance.StrengthenHeroInfo;

            int curent = heroInfo.strengthenExp;
            HeroStrengthenNeedData needData = HeroStrengthenNeedData.GetHeroStrengthenNeedDataByLevel(heroInfo.strengthenLevel);
            float percent = 0;
            float addPercent = 0;
            if (needData != null)
            {
                percent = (curent + 0.0f) / needData.exp_need;
                addPercent = percent + (addExp + 0.0f) / needData.exp_need;

            }
            else
            {
                addExpText.text = "MAX";
            }
            currentExpSlider.value = percent;
            addExpSlider.value = addPercent;

        }
        private void RegenerateHeroButtons()
        {
            List<HeroInfo> allInBagHeroInfoList = HeroStrengthenProxy.instance.GetHeroInfoBySortType();
            int count = allInBagHeroInfoList.Count;
            scrollContent.Init(count);
            noAvailableStrengthenMaterialText.gameObject.SetActive(count < 1);
        }

        private void RefreshHeroButtons()
        {
            scrollContent.RefreshAllContentItems();
            //			CommonHeroIcon.View.CommonHeroIcon [] heroButtons = heroButtonsRootTransform.GetComponentsInChildren<CommonHeroIcon.View.CommonHeroIcon >();
            //			int heroButtonsCount = heroButtons.Length;
            //			List<HeroInfo> materialList = HeroStrengthenProxy.instance.GetSelectedMaterialHeroInfoList();
            //			for (int i = 0; i < heroButtonsCount; i++)
            //			{
            //				if (materialList.Contains(heroButtons[i].HeroInfo))
            //				{
            //					heroButtons[i].SetSelect(true);
            //				}
            //				else
            //				{
            //					heroButtons[i].SetSelect(false);
            //				}
            //			}
        }

        private void RefreshSelectedMaterialHeroButtons()
        {
            HeroInfo[] materials = HeroStrengthenProxy.instance.SelectedMaterialHeroInfos;
            Transform tran;
            for (int i = 0, count = materials.Length; i < count; i++)
            {
                tran = selectedMaterialHeroFrameButtons[i].transform;
                TransformUtil.ClearChildren(tran, true);
                HeroInfo heroInfo = materials[i];
                if (heroInfo != null)
                {
                    CommonHeroIcon.View.CommonHeroIcon heroIcon = CommonHeroIcon.View.CommonHeroIcon.Create(tran);
                    heroIcon.SetHeroInfo(heroInfo);
                    heroIcon.SetButtonEnable(false);
                }
            }
        }
        private void RefreshAttribute()
        {

            HeroInfo info;
            int expTotal = 0;
            float crit = 0;

            HeroInfo[] materials = HeroStrengthenProxy.instance.SelectedMaterialHeroInfos;
            int count = materials.Length;
            int selectMaterialCount = 0;
            for (int i = 0; i < count; i++)
            {
                info = materials[i];
                if (info != null)
                {
                    HeroStrengthenProvideData data = HeroStrengthenProvideData.GetHeroStrengthenProvideDataByID(info.advanceLevel);
                    if (data != null)
                    {
                        expTotal += data.exp_provide;
                        crit += (data.exp_provide * data.crit);

                    }
                    selectMaterialCount++;
                }
            }
            //crit
            if (expTotal != 0)
                crit = crit / expTotal;
            string s;
            if (crit >= 0 && crit < 10)
            {
                s = Localization.Get("ui.hero_strengthen_view.small");
            }
            else if (crit >= 10 && crit < 30)
            {
                s = Localization.Get("ui.hero_strengthen_view.mid"); ;
            }
            else
            {
                s = Localization.Get("ui.hero_strengthen_view.big"); ;
            }

            currentCriticalChangeText.text = s;

            //level
            RoleInfo heroInfo = HeroStrengthenProxy.instance.StrengthenHeroInfo;


            HeroStrengthenNeedData curData = HeroStrengthenNeedData.GetHeroStrengthenNeedDataByLevel(heroInfo.strengthenLevel);
            HeroStrengthenNeedData nextData = HeroStrengthenNeedData.GetHeroStrengthenNeedDataByExp(HeroStrengthenNeedData.GetStrengthenTotalExp(heroInfo.strengthenLevel) + heroInfo.strengthenExp + expTotal);
            bool isMax = curData == null ? true : false;
            _isReachMaxLevel = isMax ? true : (nextData == null ? true : false);
            int addLevel = isMax ? 0 : (nextData == null) ? HeroStrengthenNeedData.LastNeedData().aggr_lv - heroInfo.strengthenLevel + 1 : nextData.aggr_lv - heroInfo.strengthenLevel;
            strengthenAddLevel = addLevel;

            int totalLv = heroInfo.strengthenLevel + addLevel;
            int stLv = RoleUtil.GetStrengthenAddShowValue(totalLv);
            if (stLv == 0)
                strengthenLevelText.text = "";
            else
                strengthenLevelText.text = string.Format("+{0}", stLv);
            strengthenLevelDesText.text = string.Format(Localization.Get("ui.hero_strengthen_view.strengthen_level"), RoleUtil.GetStrengthenLevelColorName(totalLv));
            HeroStrengthenNeedData heroStrengthenNeedData = HeroStrengthenNeedData.GetHeroStrengthenNeedDataByID(totalLv - 1);
            RoleStrengthenStage color = RoleStrengthenStage.White;
            if (heroStrengthenNeedData != null)
            {
                color = heroStrengthenNeedData.roleStrengthenStage;
                strengthenLevelText.color = UIUtil.GetRoleNameColor(color);
                strengthenLevelDesText.color = UIUtil.GetRoleNameColor(color);
            }
            else
            {
                strengthenLevelText.color = UIUtil.GetRoleNameColor(RoleStrengthenStage.White);
                strengthenLevelDesText.color = UIUtil.GetRoleNameColor(RoleStrengthenStage.White);
            }

            //属性
            RefreshMainAttribute(addLevel);

            //money
            float moneyTotal = GetStrengthenMoney(heroInfo.strengthenLevel, heroInfo.strengthenLevel + addLevel, expTotal);

            moneyTotal = moneyTotal * selectMaterialCount;
            if (moneyTotal > GameProxy.instance.BaseResourceDictionary.GetValue(BaseResType.Gold))
            {
                coinText.text = UIUtil.FormatToRedText(((int)moneyTotal).ToString());
            }
            else
            {
                coinText.text = ((int)moneyTotal).ToString();
            }

            RefreshExp(expTotal);
        }
        private float GetStrengthenMoney(int curlevel, int nextLevel, int expTotal)
        {
            RoleInfo heroInfo = HeroStrengthenProxy.instance.StrengthenHeroInfo;
            int expTempTotal = expTotal;
            float moneyTotal = 0;
            float partMoney = 0;
            float addExpPercent = 0;
            float totalExpPercent = 0;
            if (expTotal != 0)
            {
                for (int i = curlevel; i <= nextLevel; i++)
                {
                    HeroStrengthenNeedData needData = HeroStrengthenNeedData.GetHeroStrengthenNeedDataByLevel(i);
                    if (needData == null)
                    {

                        needData = HeroStrengthenNeedData.LastNeedData();
                    }

                    if (i == nextLevel)
                    {
                        partMoney = (expTempTotal + 0.0f) / expTotal * needData.gold_need;
                        moneyTotal += partMoney;
                        addExpPercent = (expTempTotal + 0.0f) / needData.exp_need;
                        totalExpPercent += addExpPercent;
                    }
                    else if (i == curlevel)
                    {
                        int exp = needData.exp_need - heroInfo.strengthenExp;
                        partMoney = (exp + 0.0f) / expTotal * needData.gold_need;
                        moneyTotal += partMoney;
                        expTempTotal -= exp;
                        addExpPercent = (exp + 0.0f) / needData.exp_need;
                        totalExpPercent += addExpPercent;
                    }
                    else
                    {
                        partMoney = (needData.exp_need + 0.0f) / expTotal * needData.gold_need;
                        moneyTotal += partMoney;
                        expTempTotal -= needData.exp_need;
                        totalExpPercent += 1;
                    }
                }

            }
            addExpText.text = string.Format("+{0}% EXP", (int)(totalExpPercent * 100));
            return moneyTotal;
        }
        private void Refresh()
        {
            RefreshAttribute();
            //RegenerateHeroButtons();
            RefreshSelectedMaterialHeroButtons();
            RefreshHeroButtons();
        }

        private void RefreshMainAttribute(int addLevel)
        {

            RoleInfo roleInfo = HeroStrengthenProxy.instance.StrengthenHeroInfo;

            TransformUtil.ClearChildren(heroAttributeViewRoot, true);

            //List<RoleAttribute> mainAttriList = HeroUtil.CalcHeroMainAttributesByHeroType(heroInfo);
            //HeroInfo nextTemp = new HeroInfo(0,heroInfo.heroData.id,heroInfo.breakthroughLevel,heroInfo.strengthenLevel+addLevel,heroInfo.advanceLevel,heroInfo.level);
            List<RoleAttribute> mainAttriList = RoleUtil.CalcRoleMainAttributesList(roleInfo);
            RoleInfo nextTemp = null;
            if ((roleInfo as HeroInfo) != null)
            {
                nextTemp = new HeroInfo(roleInfo.instanceID, roleInfo.modelDataId, roleInfo.breakthroughLevel, roleInfo.strengthenLevel + addLevel, roleInfo.advanceLevel, roleInfo.level);
            }
            else if ((roleInfo as PlayerInfo) != null)
            {
                nextTemp = new PlayerInfo(0, (uint)roleInfo.modelDataId, 0, 0, 0, 0, "");
                nextTemp.breakthroughLevel = roleInfo.breakthroughLevel;
                nextTemp.strengthenLevel = roleInfo.strengthenLevel + addLevel;
                nextTemp.advanceLevel = roleInfo.advanceLevel;
                nextTemp.level = roleInfo.level;
            }

            bool isMaxLevel = HeroStrengthenNeedData.IsMaxLevel(roleInfo.strengthenLevel);
            List<RoleAttribute> nextMainAttriList = RoleUtil.CalcRoleMainAttributesList(nextTemp);
            heroAttributeViewPrefab.gameObject.SetActive(true);
            for (int i = 0, count = mainAttriList.Count; i < count; i++)
            {
                AttributeView view = Instantiate<AttributeView>(heroAttributeViewPrefab);
                Transform tran = view.transform;
                tran.SetParent(heroAttributeViewRoot, false);
                if (isMaxLevel)
                    view.Set(mainAttriList[i]);
                else
                    view.Set(mainAttriList[i], (int)(nextMainAttriList[i].value - mainAttriList[i].value));
            }
            heroAttributeViewPrefab.gameObject.SetActive(false);
        }
        #region proxy callback handler
        public void OnStrengthenSuccessHandler(bool isCrit)
        {
            //	RoleInfo roleInfo = HeroStrengthenProxy.instance.StrengthenHeroInfo;


            //Debugger.Log(string.Format("success2 heroInstanceID:{0} ,iscrit:{1},level:{2},exp{3}",roleInfo.instanceID,isCrit,roleInfo.strengthenLevel,roleInfo.strengthenExp));
            //				if(isCrit){
            //					Logic.UI.Tips.View.CommonErrorTipsView tips = UIMgr.instance.Open<Logic.UI.Tips.View.CommonErrorTipsView>(Logic.UI.Tips.View.CommonErrorTipsView.PREFAB_PATH);
            //					tips.SetTips("强化暴击了！");
            //				}

            StartCoroutine(StrengthenSuccessRefreshCoroutine());

        }
        private IEnumerator StrengthenSuccessRefreshCoroutine()
        {
            GameObject effect = null;
            List<int> materialSlots = new List<int>();
            for (int i = 0, count = HeroStrengthenProxy.instance.SelectedMaterialHeroInfos.Length; i < count; i++)
            {
                if (HeroStrengthenProxy.instance.SelectedMaterialHeroInfos[i] != null)
                    materialSlots.Add(i);
            }
            int materialCount = materialSlots.Count;
            if (_rootCanvas == null)
                _rootCanvas = gameObject.GetComponent<Canvas>();

            RegenerateHeroButtons();
            RefreshHeroButtons();
            yield return null;
            //强化材料特效
            float meterialEffectTime = 0.5f;
            int index = 0;
            for (int i = 0; i < materialCount; i++)
            {
                index = materialSlots[i];
                effect = ParticleUtil.CreateParticle("effects/prefabs/qianghua", _rootCanvas);
                effect.transform.SetParent(selectedMaterialHeroFrameButtons[index].transform.parent, false);
                effect.transform.localPosition = selectedMaterialHeroFrameButtons[index].transform.localPosition;
                GameObject.Destroy(effect, 1);
            }

            yield return new WaitForSeconds(meterialEffectTime);

            RoleInfo roleInfo = HeroStrengthenProxy.instance.StrengthenHeroInfo;
            HeroStrengthenProxy.instance.ClearMaterials();
            _strengthenHeroButton.SetRoleInfo(roleInfo);
            RefreshSelectedMaterialHeroButtons();

            //聚集粒子特效
            float particleMoveEffectTime = 0.6f;
            Vector3 moveLocation = strengthenHeroButtonRoot.transform.localPosition;
            for (int i = 0; i < materialCount; i++)
            {
                index = materialSlots[i];
                int randomNum = Random.Range(1, 3);
                for (int k = 0; k < randomNum; k++)
                {
                    effect = ParticleUtil.CreateParticle("effects/prefabs/dandao", _rootCanvas);
                    effect.transform.SetParent(selectedMaterialHeroFrameButtons[index].transform.parent, false);
                    effect.transform.localPosition = selectedMaterialHeroFrameButtons[index].transform.localPosition;
                    float randomX = Random.Range(-150, 150) + effect.transform.localPosition.x;
                    float randomY = Random.Range(-150, 150) + effect.transform.localPosition.y;
                    Vector3 flyLocation = new Vector3(randomX, randomY, 0);
                    Vector3 v2 = flyLocation;
                    Vector3 v3 = new Vector3((moveLocation.x + v2.x) / 2, (moveLocation.y + v2.y) / 2);
                    Vector3[] v = new Vector3[] { effect.transform.localPosition, v3, v2, moveLocation };
                    LeanTween.moveLocal(effect, v, particleMoveEffectTime).setEase(LeanTweenType.easeInSine).setDelay(0.03f * i);
                    GameObject.Destroy(effect, particleMoveEffectTime + 0.03f * i);
                }
            }
            yield return new WaitForSeconds(particleMoveEffectTime);
            //升级特效
            if (strengthenAddLevel != 0)
            {
                effect = ParticleUtil.CreateParticle("effects/prefabs/shengji", _rootCanvas);

            }
            else
            {
                effect = ParticleUtil.CreateParticle("effects/prefabs/kapaixishou", _rootCanvas);
            }
            effect.transform.SetParent(strengthenHeroButtonRoot.transform.parent, false);
            effect.transform.localPosition = strengthenHeroButtonRoot.transform.localPosition;
            GameObject.Destroy(effect, 1);

            //经验条特效
            effect = ParticleUtil.CreateParticle("effects/prefabs/qianghua_jindutiao", _rootCanvas);
            effect.transform.SetParent(currentExpSlider.transform.parent, false);
            effect.transform.localPosition = currentExpSlider.transform.localPosition;
            GameObject.Destroy(effect, 1);

            RefreshAttribute();
            _isClickStrengthenBtn = false;
        }
        #endregion

        #region UI event handlers
        public void ClickCloseHandler()
        {
            UIMgr.instance.Close(PREFAB_PATH);
        }

        public void ClickSelectedHeroMaterialHandler(Button button)
        {
            if (_isClickStrengthenBtn)
                return;

            int index = selectedMaterialHeroFrameButtons.IndexOf(button);
            HeroStrengthenProxy.instance.SetSelectedMaterialHeroInfo(index, null);

            Refresh();
        }
        public void onChangeSortBtnHandler(int index)
        {
            Debugger.Log("index:" + index);
            int sortIndex = dropDownSort.value;
            HeroSortType type = (HeroSortType)(sortIndex + 1 + (int)HeroSortType.Invalid);
            if (type != HeroStrengthenProxy.instance.currentSortType)
            {
                HeroStrengthenProxy.instance.currentSortType = type;
                RegenerateHeroButtons();
                RefreshHeroButtons();
            }
        }
        public void ClickHeroButtonView(CommonHeroIcon.View.CommonHeroIcon heroButton)
        {
            if (_isClickStrengthenBtn)
                return;
            HeroInfo[] materials = HeroStrengthenProxy.instance.SelectedMaterialHeroInfos;
            if (heroButton.IsSelect)
            {
                HeroStrengthenProxy.instance.SetSelectedMaterialHeroInfo(heroButton.HeroInfo.instanceID, null);

            }
            else
            {
                if (_isReachMaxLevel)
                {
                    CommonAutoDestroyTipsView.Open(Localization.Get("ui.hero_strengthen_view.maxExp"));
                    return;
                }
                HeroStrengthenProxy.instance.SetSelectedMaterialHeroInfo(heroButton.HeroInfo);
            }
            Refresh();
        }

        public void ClickStartStrengthen()
        {
            if (_isClickStrengthenBtn)
                return;

            RoleInfo roleInfo = HeroStrengthenProxy.instance.StrengthenHeroInfo;

            bool isMaxLevel = roleInfo.IsMaxStrengthenLevel;
            if (isMaxLevel)
            {
                ShowSingleTips("强化已达上限");
                return;
            }
            int money = coinText.text.ToInt32();
            Debugger.Log("money;" + money);
            List<int> materialHeroIDList = new List<int>();
            bool hasHighStar = false;
            HeroInfo[] materials = HeroStrengthenProxy.instance.SelectedMaterialHeroInfos;
            int count = materials.Length;

            for (int i = 0; i < count; i++)
            {
                if (materials[i] != null)
                {
                    materialHeroIDList.Add((int)materials[i].instanceID);
                    if (materials[i].advanceLevel >= 4)
                        hasHighStar = true;
                }
            }
            int gold = 0;
            GameProxy.instance.BaseResourceDictionary.TryGetValue(BaseResType.Gold, out gold);
            if (money > gold)
            {
                ShowSingleTips("金币不足，不可强化");
                return;
            }
            if (materialHeroIDList.Count == 0)
            {
                ShowSingleTips("未添加材料，不可强化");
                return;
            }
            if (hasHighStar)
            {
                Debugger.Log("含有4星以上品质的英雄或装备");
            }


            _isClickStrengthenBtn = true;

            if (roleInfo.GetType() == typeof(HeroInfo))
            {
                HeroController.instance.CLIENT2LOBBY_HERO_AGGR_REQ((int)roleInfo.instanceID, materialHeroIDList);

            }
            else if (roleInfo.GetType() == typeof(PlayerInfo))
            {
                Logic.Player.Controller.PlayerController.instance.CLIENT2LOBBY_PLAYER_AGGR_REQ((int)roleInfo.instanceID, materialHeroIDList);
                //				roleInfo.strengthenLevel = roleInfo.strengthenLevel+1;
                //				PlayerProxy.instance.OnPlayerInfoUpdate();
                //				OnStrengthenSuccessHandler((int)roleInfo.instanceID,false);
            }

        }
        public void OnResetScrollItemHandler(GameObject go, int index)
        {
            CommonHeroIcon.View.CommonHeroIcon heroButton = go.GetComponent<CommonHeroIcon.View.CommonHeroIcon>();
            if (heroButton != null)
            {
                HeroInfo info = HeroStrengthenProxy.instance.currentHeroInfoList[index];
                heroButton.SetHeroInfo(info);
                List<HeroInfo> materialList = HeroStrengthenProxy.instance.GetSelectedMaterialHeroInfoList();
                bool isSelect = false;
                if (materialList.Contains(info))
                {
                    heroButton.SetSelect(true);
                }
                else
                {
                    heroButton.SetSelect(false);
                }
                heroButton.onClickHandler = ClickHeroButtonView;
                go.name = index.ToString();
            }
        }
        #endregion
        private void ShowSingleTips(string message)
        {
            CommonErrorTipsView.Open(message);
        }
    }
}
