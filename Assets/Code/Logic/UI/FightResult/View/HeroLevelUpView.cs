using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Logic.Hero.Model;
using Common.ResMgr;
using Logic.Player.Model;
using System.Collections.Generic;
using Logic.Protocol.Model;
using Logic.Game.Model;
using Common.Localization;
using Logic.UI.CommonAnimations;
using Logic.Audio.Controller;

namespace Logic.UI.FightResult.View
{
    public class HeroLevelUpView : MonoBehaviour
    {

        public delegate void ProfessionLevelUpDelegate(int professionLevel);
        public ProfessionLevelUpDelegate OnProfessionLevelUpDelegate;

        public ProfessionLevelUpDelegate OnHeroLevelUpFinishedDelegate;
        #region ui
        public Text text_lv;
        public Image image_icon_bg;
        public Image image_icon;
        public Image image_leader;
        public Image image_upArrow;
        public ExpScaleBar exp_profession_bar;
        public ExpScaleBar exp_hero_bar;

        public Text expAddText;
        #endregion


        private HeroInfo _oldHeroInfo;
        private int _addHeroExp;
        private int _addAcountExp;


        private bool _isLeader;
        private PlayerInfo _oldPlayerInfo;

        private int _oldHeroLevel;
        private int _curHeroLevel;
        private int _nextHeroLevel;

        private int _oldAccountLevel;
        private int _curAccountLevel;
        private int _nextAccountLevel;

        private float _delayTime;


        void Awake()
        {

        }
        public bool needRefresh = false;
        public bool reset;
        void Update()
        {
            if (reset)
            {
                reset = false;
                Init();
            }
            if (needRefresh)
            {
                StartCoroutine(DoHeroLevelUpCoroutine(0));
                needRefresh = false;
            }
        }

        public void SetHeroData(int heroId, int addHeroExp, float delay = 0)
        {
            HeroInfo info = HeroProxy.instance.GetHeroInfo((uint)heroId);
            if (info == null) return;
            HeroInfo temp = new HeroInfo(info.instanceID, info.heroData.id, 0, 0, info.advanceLevel);
            int totalExp = info.TotalLevelUpExp - addHeroExp;

            HeroExpData oldData = HeroExpData.GetHeroExpData(totalExp);
            bool isMax = oldData == null;
            if (isMax)
            {
                temp = info;
            }
            else
            {
                temp.level = HeroExpData.GetHeroExpData(totalExp).lv;
                temp.exp = totalExp - HeroExpData.GetHeroExpDataByLv(temp.level - 1).exp_total;
            }

            //Debugger.Log(string.Format("[HeroView][SetHeroData]heroid: {0}, old exp:{1}, old level: {2}, new exp:{3},  new level :{4}, addExp:{5}",heroId,temp.exp,temp.level,info.exp,info.level,addHeroExp));
            SetHeroData(temp, addHeroExp, delay);
        }

        public void SetHeroData(HeroInfo info, int addHeroExp, float delay)
        {

            _isLeader = false;
            _oldHeroInfo = info;

            _addHeroExp = addHeroExp;
            _oldHeroLevel = info.level;
            _curHeroLevel = info.level;
            _delayTime = delay;

			image_icon.SetSprite(ResMgr.instance.Load<Sprite>(_oldHeroInfo.HeadIcon));
			image_icon_bg.SetSprite(UIUtil.GetRoleQualityFrameSprite(_oldHeroInfo.heroData.roleQuality));
            Init();
        }

        public void SetMainHeroData(int addHeroExp, int addAccountExp, float delay = 0)
        {
            PlayerInfo playerInfo = GameProxy.instance.PlayerInfo;

            PlayerInfo temp = new PlayerInfo(playerInfo.instanceID, playerInfo.playerData.Id, 0, 0, 0, 0, "");

            int expTotal = playerInfo.TotalLevelUpExp - addHeroExp;
            HeroExpData oldData = HeroExpData.GetHeroExpData(expTotal);
            bool isMax = oldData == null;
            if (isMax)
            {
                temp = playerInfo;
            }
            else
            {
                temp.level = HeroExpData.GetHeroExpData(expTotal).lv;
                temp.exp = expTotal - HeroExpData.GetHeroExpDataByLv(temp.level - 1).exp_total;
            }

            //Debugger.Log(string.Format("[HeroView][SetMainHeroData]playerid: {0}, old exp:{1}, old level: {2}, addheroExp:{3},addAccount:{4}",playerInfo.instanceID,temp.exp,temp.level,addHeroExp,addAccountExp));

            SetMainHeroData(temp, addHeroExp, addAccountExp, delay);
        }
        public void SetMainHeroData(PlayerInfo playerInfo, int addHeroExp, int addAcountExp, float delay)
        {
            _isLeader = true;
            _oldPlayerInfo = playerInfo;
            _addAcountExp = addAcountExp;

            _addHeroExp = addHeroExp;
            _oldHeroLevel = playerInfo.level;
            _curHeroLevel = playerInfo.level;

            _delayTime = delay;

			image_icon.SetSprite( ResMgr.instance.Load<Sprite>(playerInfo.HeadIcon));
			image_icon_bg.SetSprite(UIUtil.GetRoleQualityFrameSprite(playerInfo.heroData.roleQuality));
            Init();
        }
        private void Init()
        {
            image_leader.gameObject.SetActive(_isLeader);
            image_upArrow.gameObject.SetActive(false);

            InitBar();
        }
        private void InitAccountBar()
        {
            int level = GameProxy.instance.AccountLevel;
            int exp = GameProxy.instance.AccountExp;
            int oldExp = 0;
            _nextAccountLevel = level;

            //协议已经过来，账号经验已升级，所以需要计算old exp
            AccountExpData accountExpData = AccountExpData.GetAccountExpDataByLv(level - 1);
            int expTotal = 0;
            if (accountExpData != null)
                expTotal = accountExpData.expTotal;
            int oldExpTotal = expTotal + exp - _addAcountExp;
            AccountExpData oldData = AccountExpData.GetAccountExpDataByExp(oldExpTotal);
            bool isMax = oldData == null;
            if (isMax)
            {
                exp_profession_bar.ChangeValue(0);
                _curAccountLevel = _oldAccountLevel = level;
            }
            else
            {
                _curAccountLevel = _oldAccountLevel = oldData.lv;
                oldExp = oldExpTotal - (oldData.expTotal - oldData.exp);
                float percent = (oldExp + 0.0f) / oldData.exp;
                exp_profession_bar.ChangeValue(percent);
            }

        }
        private void InitBar()
        {
            float percent = 0;

            if (_isLeader)
            {

                HeroExpData curHeroExpData = HeroExpData.GetHeroExpDataByLv(_oldPlayerInfo.level);
                HeroExpData next = HeroExpData.GetHeroExpData(_oldPlayerInfo.TotalLevelUpExp, _addHeroExp);
                if (curHeroExpData == null)
                {
                    exp_hero_bar.ChangeValue(0);
                }
                else
                {
                    percent = (_oldPlayerInfo.exp + 0.0f) / curHeroExpData.exp;
                    exp_hero_bar.ChangeValue(percent);
                }

                if (next == null)
                {
                    _nextHeroLevel = GameProxy.instance.PlayerInfo.level;
                }
                else
                {
                    _nextHeroLevel = next.lv;
                }


                exp_profession_bar.ChangeValue(0);
                //account 
                InitAccountBar();
                if (_addAcountExp != 0)
                    StartCoroutine(UpdateAccountBarDelay(_delayTime));
            }
            else
            {
                exp_profession_bar.gameObject.SetActive(false);


                HeroExpData curHeroExpData = HeroExpData.GetHeroExpDataByLv(_oldHeroInfo.level);
                HeroExpData next = HeroExpData.GetHeroExpData(_oldHeroInfo.TotalLevelUpExp, _addHeroExp);
                if (curHeroExpData == null)
                {
                    exp_hero_bar.ChangeValue(0);
                }
                else
                {
                    percent = (_oldHeroInfo.exp + 0.0f) / curHeroExpData.exp;
                    exp_hero_bar.ChangeValue(percent);
                }
                if (next == null)
                {
                    _nextHeroLevel = HeroProxy.instance.GetHeroInfo(_oldHeroInfo.instanceID).level;
                }
                else
                {
                    _nextHeroLevel = next.lv;
                }
            }
            if (expAddText != null)
            {
                expAddText.text = string.Format(Localization.Get("ui.fightResultView.hero_exp_add"), _addHeroExp);
            }
            text_lv.text = string.Format(Localization.Get("common.role_icon.role_lv"), _curHeroLevel);
            if (_addHeroExp != 0)
                StartCoroutine(UpdateHeroBarDelay(_delayTime));
        }
        private IEnumerator UpdateAccountBarDelay(float time)
        {
            yield return new WaitForSeconds(time);
            UpdateAccountBar();
        }
        private IEnumerator UpdateHeroBarDelay(float time)
        {
            yield return new WaitForSeconds(time);
            UpdateHeroBar();
			//AudioController.instance.PlayAudioRepeat(AudioController.addExp_audio);
        }
        private void UpdateAccountBar()
        {

            float duringTime = 1f;
            if (_curAccountLevel == _nextAccountLevel)
            {

                AccountExpData data = AccountExpData.GetAccountExpDataByLv(_curAccountLevel);
                bool isMax = data == null;
                float percent = 0;
                if (!isMax)
                {
                    percent = (GameProxy.instance.AccountExp + 0.0f) / data.exp;
                }
                exp_profession_bar.ChangeValue(percent, duringTime * (percent - exp_profession_bar.Value));


            }
            else
            {
                exp_profession_bar.ChangeValue(1, duringTime).setOnComplete(AccountLevelUp);
            }
        }
        private void UpdateHeroBar()
        {
            float duringTime = 1f;
            if (_curHeroLevel > _nextHeroLevel)
                return;
            if (_curHeroLevel == _nextHeroLevel)
            {
                int exp_need = 1;
                int exp_cur = 0;
                if (_isLeader)
                {

                    HeroExpData expData = HeroExpData.GetHeroExpDataByLv(_nextHeroLevel);
                    bool isMax = expData == null;
                    if (!isMax)
                    {
                        exp_need = expData.exp;
                        exp_cur = expData.exp - (expData.exp_total - (_oldPlayerInfo.TotalLevelUpExp + _addHeroExp));
                    }
                }
                else
                {
                    HeroExpData expData = HeroExpData.GetHeroExpDataByLv(_nextHeroLevel);
                    bool isMax = expData == null;
                    if (!isMax)
                    {
                        exp_need = expData.exp;
                        exp_cur = expData.exp - (expData.exp_total - (_oldHeroInfo.TotalLevelUpExp + _addHeroExp));

                    }

                }

                float percent = (exp_cur + 0.0f) / exp_need;
                exp_hero_bar.ChangeValue(percent, duringTime * (percent - exp_hero_bar.Value)).setOnComplete(DoHeroLevelUpFinished);
            }
            else
            {
                if (_curHeroLevel == _oldHeroLevel)
                    // LeanTween.moveLocalY(image_upArrow.gameObject, 0, 0.3f).setLoopPingPong();

                    exp_hero_bar.ChangeValue(1, duringTime).setOnComplete(HeroLevelUp);
            }
        }
        private void AccountLevelUp()
        {

            StartCoroutine(DoAccountLevelUpCoroutine(0.1f));
        }
        //账号升级
        private IEnumerator DoAccountLevelUpCoroutine(float time)
        {
            yield return new WaitForSeconds(time);

            exp_profession_bar.ChangeValue(0);
            _curAccountLevel++;
            UpdateAccountBar();
            LevelUpTipsView.Open();
            if (OnProfessionLevelUpDelegate != null)
            {
                OnProfessionLevelUpDelegate(_curAccountLevel);

            }
        }
        private void DoHeroLevelUpFinished()
        {
            if (OnHeroLevelUpFinishedDelegate != null)
            {
                OnHeroLevelUpFinishedDelegate(_curHeroLevel);
            }
        }
        private void HeroLevelUp()
        {

            StartCoroutine(DoHeroLevelUpCoroutine(0.1f));
        }
        //角色升级
        private IEnumerator DoHeroLevelUpCoroutine(float time)
        {
            yield return new WaitForSeconds(time);

            exp_hero_bar.ChangeValue(0);
            _curHeroLevel++;
            text_lv.text = string.Format(Localization.Get("common.role_icon.role_lv"), _curHeroLevel);
            LeanTween.scale(text_lv.transform.parent as RectTransform, new Vector3(1.2f, 1.2f, 1), 0.1f).setLoopPingPong(1);

            UpdateHeroBar();
            image_upArrow.gameObject.SetActive(true);
            image_upArrow.transform.localScale = new Vector3(0, 1, 1);
            LeanTween.scaleX(image_upArrow.gameObject, 1, 0.1f);
            CommonFadeToAnimation.Get(image_upArrow.gameObject).init(0, 1, 0.2f);
            yield return new WaitForSeconds(2);
            CommonFadeToAnimation.Get(image_upArrow.gameObject).init(1, 0, 0.5f);
            yield return new WaitForSeconds(1);
            image_upArrow.gameObject.SetActive(false);

        }

    }
}

