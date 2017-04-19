using UnityEngine;
using System.Collections;
using Logic.Net.Controller;
using Logic.Character;
using Logic.Skill.Model;
using UnityEngine.UI;
using Common.ResMgr;
using Logic.Enums;
using Common.Components;
using System.Collections.Generic;
using Logic.Audio.Controller;
using Common.Util;
using Logic.Effect.Controller;
using Logic.Skill;
using Logic.Fight.Controller;


namespace Logic.UI.SkillBar.View
{
    public class SkillItemBoxView : MonoBehaviour
    {
        #region ui
        public Image headImage;
        public Image leaderImage;
        public Image HPImage;
        public Image HPBGImg;
        public Image angryImage;
        public Image angryBGImage;
        public Image buffImg;
        public Button aeonBtn;
        public GameObject angryEffectParent;
        public SkillItemView skillItemView1;
        public SkillItemView skillItemView2;
        public GameObject angryGO;
        public Canvas canvas;
        #endregion
        private CharacterEntity _character;
        private Color32 _color = new Color(255f / 255, 56f / 255, 56f / 255, 1);
        private Color _comboColor = new Color(98 / 255f, 98 / 255f, 98 / 255f, 44 / 255f);
        private Color _originalColor = Color.white;
        private bool _isShaking = false;
        private bool _isOrderAeonSkill = false;
        private float _lastTime = 0f;
        private const float INTERVAL = 1f;
        private int _currentIndex = 0;
        private float _buffProgress = 0f;
        //private bool _existContrary = false;
        //private BuffType _lastBuffType = BuffType.None;
        private string _lastIconPath;
        public bool isCanPlayAeon { get; private set; }
        public bool isEnable
        {
            get
            {
                return gameObject.activeInHierarchy;
            }
        }

        private bool init = false;
        public CharacterEntity character
        {
            get
            {
                return _character;
            }
            set
            {
                _character = value;
                //if (value is PlayerEntity)
                //{
                //    leaderImage.gameObject.SetActive(true);
                //    angryGO.SetActive(true);
                //    ResetAngry();
                //    SetAngrySlideSize();
                //}
                //else
                //{
                leaderImage.gameObject.SetActive(false);
                aeonBtn.gameObject.SetActive(false);
                angryGO.SetActive(false);
                isCanPlayAeon = false;
                //}
                SetHPSprite();
                _character.OnHPChange += SetHPSprite;
                init = true;
            }
        }
        public System.Action<uint, SkillItemView> OnSkillClick;

        #region 内部方法
        void Awake()
        {
            skillItemView1.onClickSkillAction += SkillClick;
            skillItemView1.skillItemBoxView = this;
            skillItemView2.onClickSkillAction += SkillClick;
            skillItemView2.skillItemBoxView = this;
        }

        void Update()
        {
            if (init && !character.isDead)
            {
                if (character.tickCD)
                {
                    SetSkill1Status(character.skill1CD);
                    SetSkill2Status(character.skill2CD);
                    UpdateBuffIcon();//版本屏蔽
                }
                UpdateSkillLockState();
            }
        }

        #endregion

        private void UpdateBuffIcon()
        {
            List<string> buffIcons = character.GetBuffIcons();
            if (buffIcons.Count == 0)
            {
                buffImg.gameObject.SetActive(false);
                return;
            }
            if (Time.time - _lastTime >= INTERVAL)
            {
                //变长集合迭代?
                /*BuffType[] buffs = character.buffEffectDic.GetKeyArray();
                int count = buffs.Length;
                _currentIndex = Mathf.RoundToInt(_buffProgress * count);//取上
                if (_currentIndex >= buffs.Length)
                    _currentIndex = 0;
                BuffType buff = buffs[_currentIndex];
                bool contrary = _existContrary;
                if (buff == _lastBuffType)
                {
                    if (_existContrary)
                        _existContrary = false;
                    else
                    {
                        _currentIndex++;
                        if (_currentIndex >= buffs.Length)
                            _currentIndex = 0;
                    }
                    buff = buffs[_currentIndex];
                }
                bool kindness = false;
                if (!contrary)
                {
                    kindness = character.ExistBuff(buff, true);
                    if (kindness)
                        _existContrary = character.ExistBuff(buff, false);
                }
                buffImg.gameObject.SetActive(true);
                string path = Fight.Model.FightProxy.instance.GetIconPath(buff, kindness);
                buffImg.SetSprite(ResMgr.instance.Load<Sprite>("sprite/" + path));
                _lastBuffType = buff;
                _buffProgress = (float)_currentIndex / (float)count;
                buffs = null;
                _lastTime = Time.time;
                 */
                int count = buffIcons.Count;
                _currentIndex = Mathf.RoundToInt(_buffProgress * count);//取上
                if (_currentIndex >= count)
                    _currentIndex = 0;
                string path = buffIcons[_currentIndex];
                if (path == _lastIconPath)
                {
                    _currentIndex++;
                    if (_currentIndex >= count)
                        _currentIndex = 0;
                    path = buffIcons[_currentIndex];
                }
                buffImg.gameObject.SetActive(true);
				buffImg.SetSprite( ResMgr.instance.Load<Sprite>("sprite/" + path));
                _lastIconPath = path;
                _buffProgress = (float)_currentIndex / (float)count;
                _lastTime = Time.time;
            }
        }

        private void SkillClick(SkillItemView skillItemView)
        {
            if (OnSkillClick != null && !character.isDead)
                OnSkillClick(character.characterInfo.instanceID, skillItemView);
            character.canOrderTime = float.MaxValue;
            SetCommonCDState(1f);
            AudioController.instance.PlayAudio(AudioController.SKILL_CLICK, false);
        }

        private void SetAngrySlideSize()
        {
            int count = Logic.Character.Controller.PlayerController.instance.heros.Count;
            float width = 0f;
            switch (count)
            {
                case 1:
                    width = 156f;
                    break;
                case 2:
                    width = 346f;
                    break;
                case 3:
                    width = 536f;
                    break;
                case 4:
                    width = 725f;
                    break;
                case 5:
                    width = 915f;
                    break;
                default:
                    width = 156f;
                    break;
            }
            angryImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            angryBGImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        }

        private void ResetAngry()
        {
            character.characterInfo.angerValue = 0;
            angryImage.fillAmount = 0f;
            angryEffectParent.SetActive(false);
            aeonBtn.image.SetGray(true);
            isCanPlayAeon = false;
        }

        private void ResetAeonSkillOrder()
        {
            if (_isOrderAeonSkill)
            {
                _isOrderAeonSkill = false;
                SetAngry(character.characterInfo.maxAngerValue);
            }
        }

        public void SetAngry(float angry)
        {
            return;
            if (_isOrderAeonSkill) return;
            if (character.isDead) return;
            character.characterInfo.angerValue += angry;
            angryImage.fillAmount = character.characterInfo.angerValue / character.characterInfo.maxAngerValue;
            if (character.characterInfo.angerValue >= character.characterInfo.maxAngerValue)
            {
                isCanPlayAeon = true;
                aeonBtn.image.SetGray(false);
                //angryEffectParent.SetActive(true);
                aeonBtn.gameObject.SetActive(true);
            }
        }

        public void Init()
        {
            //skillItemView1.Show(false);
            //skillItemView2.Show(false);
            init = false;
            _character = null;
            aeonBtn.gameObject.SetActive(false);
            angryGO.SetActive(false);
            buffImg.gameObject.SetActive(false);
            LoadEffects();
        }

        private void LoadEffects()
        {
            //angry comment
            //GameObject ui_effect_07 = ParticleUtil.CreateParticle(string.Format("effects/prefabs/{0}", EffectController.UI_EFFECT_07), canvas);
            //ui_effect_07.transform.SetParent(angryEffectParent.transform, false);
            //ui_effect_07.transform.localPosition = Vector3.zero;
            //SortingOrderChanger sortingOrderChanger_07 = ui_effect_07.GetComponent<SortingOrderChanger>();
            //if (sortingOrderChanger_07)
            //    sortingOrderChanger_07.sortingOrder = canvas.sortingOrder - 1;
            //GameObject ui_effect_08 = ParticleUtil.CreateParticle(string.Format("effects/prefabs/{0}", EffectController.UI_EFFECT_08), canvas);
            //ui_effect_08.transform.SetParent(angryEffectParent.transform, false);
            //ui_effect_08.transform.localPosition = Vector3.zero;
            //SortingOrderChanger sortingOrderChanger_08 = ui_effect_08.GetComponent<SortingOrderChanger>();
            //if (sortingOrderChanger_08)
            //    sortingOrderChanger_08.sortingOrder = canvas.sortingOrder + 1;

            skillItemView1.LoadEffects();
            skillItemView2.LoadEffects();
        }

        public Sprite headSprite
        {
            set
            {
				headImage.SetSprite(value);
            }
        }

        public void SetAeonSkillSprite(string skillSpriteName)
        {
			aeonBtn.image.SetSprite(ResMgr.instance.Load<Sprite>("sprite/skill/" + skillSpriteName));
        }

        public void ResetSkillOrder(uint skillId)
        {
            if (character.characterInfo.skillId1 == skillId)
                skillItemView1.ResetSkillOrder();
            else if (character.characterInfo.skillId2 == skillId)
                skillItemView2.ResetSkillOrder();
            else if (character.characterInfo.aeonSkill == skillId)
                ResetAeonSkillOrder();
            StopCoroutine("UpdateCommonCDCoroutine");
            SetCommonCDState(0f);
        }

        public void ResetSkillOrder()
        {
            if (character.characterInfo.skillId1 != 0)
            {
                skillItemView1.ResetSkillOrder();
                character.ResetSkillOrder(character.characterInfo.skillId1);
            }
            if (character.characterInfo.skillId2 != 0)
            {
                skillItemView2.ResetSkillOrder();
                character.ResetSkillOrder(character.characterInfo.skillId2);
            }
            if (character.characterInfo.aeonSkill != 0)
            {
                ResetAeonSkillOrder();
                character.ResetSkillOrder(character.characterInfo.aeonSkill);
            }
            StopCoroutine("UpdateCommonCDCoroutine");
            SetCommonCDState(0f);
        }

        public void ResetSkillItem()
        {
            if (character.characterInfo.skillId1 != 0)
                skillItemView1.ResetSkillItem();
            if (character.characterInfo.skillId2 != 0)
                skillItemView2.ResetSkillItem();
            SetSkillBoxState(true);
            UpdateHPSprite();
            ResetAngry();
        }

        public void InitSkill()
        {
            if (character.characterInfo.skillInfo1 != null)
            {
                skillItemView1.Show(true);
                skillItemView1.SetSkillInfo(character.characterInfo.skillInfo1);
            }
            else
            {
                skillItemView1.SetDisable();
                skillItemView1.ResetSkillIcon();
                skillItemView1.Show(false);
            }
            if (character.characterInfo.skillInfo2 != null)
            {
                skillItemView2.Show(true);
                skillItemView2.SetSkillInfo(character.characterInfo.skillInfo2);
            }
            else
            {
                skillItemView2.SetDisable();
                skillItemView2.ResetSkillIcon();
                skillItemView2.Show(false);
            }
        }

        private void SetCommonCDState(float progress)
        {
            if (character.characterInfo.skillId1 != 0)
                skillItemView1.SetCommonCDState(progress);
            if (character.characterInfo.skillId2 != 0)
                skillItemView2.SetCommonCDState(progress);
        }

        private void ShowCommonCDImage(bool show)
        {
            if (character.characterInfo.skillId1 != 0)
                skillItemView1.ShowCommonCDImage(show);
            if (character.characterInfo.skillId2 != 0)
                skillItemView2.ShowCommonCDImage(show);
        }

        private void UpdateHPSprite()
        {
            HPImage.fillAmount = (float)character.HP / (float)character.maxHP;
        }

        private void SetHPSprite()
        {
            if (character.HP <= 0)
            {
                SetSkillBoxState(false);
                if (character.characterInfo.skillId1 != 0)
                    skillItemView1.SetDisable();
                if (character.characterInfo.skillId2 != 0)
                    skillItemView2.SetDisable();
                //_character.OnHPChange -= SetHPSprite;
                Logic.UI.SkillBar.Controller.SkillBarController.instance.RemoveDisableSkillItem();
            }
            UpdateHPSprite();
            if (init && HPImage.fillAmount < 1f)
            {
                Shake();
            }
        }

        //private IEnumerator HPSpriteTrigger()
        //{
        //    HPImage.color = _color;
        //    yield return new WaitForSeconds(0.3f);
        //    HPImage.color = Color.white;
        //}

        private IEnumerator UpdateCommonCDCoroutine()
        {
            float delay = 0f;
            delay = character.canOrderTime - Time.time;
            while (Time.time <= character.canOrderTime)
            {
                if (character.tickCD)
                {
                    float progress = delay / FightController.COMMON_CD_TIME;
                    SetCommonCDState(progress);
                    delay = character.canOrderTime - Time.time;
                }
                else
                {
                    character.canOrderTime = delay + Time.time;
                }
                yield return null;
            }
            SetCommonCDState(0f);
        }

        private void SetSkillBoxColor(Color color)
        {
            if (isCanPlayAeon)
                aeonBtn.image.color = color;
            HPImage.color = color;
            HPBGImg.color = color;
            if (character is PlayerEntity)
            {
                Image[] images = angryGO.GetComponentsInChildren<Image>();
                for (int i = 0, count = images.Length; i < count; i++)
                {
                    images[i].color = color;
                }
                leaderImage.color = color;
            }
        }

        private void SetSkillBoxState(bool state)
        {
            if (isCanPlayAeon)
            {
                //aeonBtn.enabled = state;
                //aeonBtn.image.SetGray(!state);
                aeonBtn.gameObject.SetActive(state);
            }
            HPImage.SetGray(!state);
            HPBGImg.SetGray(!state);
            headImage.SetGray(!state);
            if (character is PlayerEntity)
            {
                Image[] images = angryGO.GetComponentsInChildren<Image>();
                for (int i = 0, count = images.Length; i < count; i++)
                {
                    images[i].SetGray(!state);
                }
                leaderImage.SetGray(!state);
                //angryEffectParent.SetActive(state);
            }
            buffImg.gameObject.SetActive(false);
        }

        public void FinishSkill(uint skillID)
        {
            if (character.characterInfo.skillId1 == skillID)
            {
                skillItemView1.FinishSkill();
                ShowCommonCDImage(true);
                StartCoroutine("UpdateCommonCDCoroutine");
            }
            else if (character.characterInfo.skillId2 == skillID)
            {
                skillItemView2.FinishSkill();
                ShowCommonCDImage(true);
                StartCoroutine("UpdateCommonCDCoroutine");
            }
            else if (character.characterInfo.aeonSkill == skillID)
            {
                _isOrderAeonSkill = false;
                aeonBtn.gameObject.SetActive(false);
            }
        }

        private void SetSkill1Status(float cd)
        {
            if (character.characterInfo.skillId1 != 0)
                skillItemView1.SetSkillState(cd);
        }

        private void SetSkill2Status(float cd)
        {
            if (character.characterInfo.skillId2 != 0)
                skillItemView2.SetSkillState(cd);
        }

        private void UpdateSkillLockState()
        {
            if (character.characterInfo.skillId1 != 0)
                skillItemView1.UpdateSkillLockState();
            if (character.characterInfo.skillId2 != 0)
                skillItemView2.UpdateSkillLockState();
        }

        public void DesaltOnCombo(float duration)
        {
            if (character.isDead) return;
            SetSkillBoxColor(_comboColor);
            ShowCommonCDImage(false);
            switch (Fight.Controller.FightController.instance.fightStatus)
            {
                case FightStatus.FloatWaiting:
                    if (character.characterInfo.skillInfo1 != null)
                    {
                        if (SkillUtil.AttackableFloat(character.characterInfo.skillInfo1))
                            PlayComboSkillEffect(skillItemView1, AttackableType.Float, duration);
                        else
                        {
                            skillItemView1.SetImagesColor(_comboColor);
                            skillItemView1.SetSkillImage(true, _comboColor);
                        }
                    }
                    if (character.characterInfo.skillInfo2 != null)
                    {
                        if (SkillUtil.AttackableFloat(character.characterInfo.skillInfo2))
                            PlayComboSkillEffect(skillItemView2, AttackableType.Float, duration);
                        else
                        {
                            skillItemView2.SetImagesColor(_comboColor);
                            skillItemView2.SetSkillImage(true, _comboColor);
                        }
                    }
                    break;
                case FightStatus.TumbleWaiting:
                    if (character.characterInfo.skillInfo1 != null)
                    {
                        if (SkillUtil.AttackableTumble(character.characterInfo.skillInfo1))
                            PlayComboSkillEffect(skillItemView1, AttackableType.Tumble, duration);
                        else
                        {
                            skillItemView1.SetImagesColor(_comboColor);
                            skillItemView1.SetSkillImage(true, _comboColor);
                        }
                    }
                    if (character.characterInfo.skillInfo2 != null)
                    {
                        if (SkillUtil.AttackableTumble(character.characterInfo.skillInfo2))
                            PlayComboSkillEffect(skillItemView2, AttackableType.Tumble, duration);
                        else
                        {
                            skillItemView2.SetImagesColor(_comboColor);
                            skillItemView2.SetSkillImage(true, _comboColor);
                        }
                    }
                    break;
            }
        }

        public void ShowAfterWaitCombo()
        {
            if (character.isDead) return;
            SetSkillBoxColor(_originalColor);

            switch (Fight.Controller.FightController.instance.fightStatus)
            {
                case FightStatus.FloatComboing:
                    if (character.characterInfo.skillInfo1 != null && (character.characterInfo.skillInfo1.skillData.attackableType != AttackableType.Float || character.characterInfo.skillInfo1.skillData.attackableType != AttackableType.FloatAndNormal || character.characterInfo.skillInfo1.skillData.attackableType != AttackableType.All))
                    {
                        skillItemView1.SetImagesColor(_originalColor);
                        skillItemView1.SetSkillImage(false, _originalColor);
                    }
                    if (character.characterInfo.skillInfo2 != null && (character.characterInfo.skillInfo2.skillData.attackableType != AttackableType.Float || character.characterInfo.skillInfo2.skillData.attackableType != AttackableType.FloatAndNormal || character.characterInfo.skillInfo2.skillData.attackableType != AttackableType.All))
                    {
                        skillItemView2.SetImagesColor(_originalColor);
                        skillItemView2.SetSkillImage(false, _originalColor);
                    }
                    break;
                case FightStatus.TumbleComboing:
                    if (character.characterInfo.skillInfo1 != null && (character.characterInfo.skillInfo1.skillData.attackableType != AttackableType.Tumble || character.characterInfo.skillInfo1.skillData.attackableType != AttackableType.TumbleAndNormal || character.characterInfo.skillInfo1.skillData.attackableType != AttackableType.All))
                    {
                        skillItemView1.SetImagesColor(_originalColor);
                        skillItemView1.SetSkillImage(false, _originalColor);
                    }
                    if (character.characterInfo.skillInfo2 != null && (character.characterInfo.skillInfo2.skillData.attackableType != AttackableType.Tumble || character.characterInfo.skillInfo2.skillData.attackableType != AttackableType.TumbleAndNormal || character.characterInfo.skillInfo2.skillData.attackableType != AttackableType.All))
                    {
                        skillItemView2.SetImagesColor(_originalColor);
                        skillItemView2.SetSkillImage(false, _originalColor);
                    }
                    break;
            }
        }

        //连击技能预约超时，显示为不可点击
        public void ShowMaskStartCombo()
        {
            if (character.isDead) return;
            switch (Fight.Controller.FightController.instance.fightStatus)
            {
                case FightStatus.FloatComboing:
                    if (character.characterInfo.skillInfo1 != null)
                    {
                        if (SkillUtil.AttackableFloat(character.characterInfo.skillInfo1))
                        {
                            if (skillItemView1.canOrder)
                                skillItemView1.ShowMask(true);
                            skillItemView1.ShowFloatParticle(false);
                        }
                    }
                    if (character.characterInfo.skillInfo2 != null)
                    {
                        if (SkillUtil.AttackableFloat(character.characterInfo.skillInfo2))
                        {
                            if (skillItemView1.canOrder)
                                skillItemView2.ShowMask(true);
                            skillItemView2.ShowFloatParticle(false);
                        }
                    }
                    break;
                case FightStatus.TumbleComboing:
                    if (character.characterInfo.skillInfo1 != null)
                    {
                        if (SkillUtil.AttackableTumble(character.characterInfo.skillInfo1))
                        {
                            if (skillItemView1.canOrder)
                                skillItemView1.ShowMask(true);
                            skillItemView1.ShowTumbleParticle(false);
                        }
                    }
                    if (character.characterInfo.skillInfo2 != null)
                    {
                        if (SkillUtil.AttackableTumble(character.characterInfo.skillInfo2))
                        {
                            if (skillItemView1.canOrder)
                                skillItemView2.ShowMask(true);
                            skillItemView2.ShowTumbleParticle(false);
                        }
                    }
                    break;
            }
        }

        public void ShowMaskAfterCombo()
        {
            if (character.isDead) return;
            ShowCommonCDImage(true);
            switch (Fight.Controller.FightController.instance.fightStatus)
            {
                case FightStatus.FloatComboing:
                    if (character.characterInfo.skillInfo1 != null)
                    {
                        if (SkillUtil.AttackableFloat(character.characterInfo.skillInfo1))
                        {
                            if (character.characterInfo.skillInfo1.skillData.attackableType == AttackableType.Float)
                                skillItemView1.ShowMask(true);
                            skillItemView1.ShowFloatParticle(false);
                        }
                    }
                    if (character.characterInfo.skillInfo2 != null)
                    {
                        if (SkillUtil.AttackableFloat(character.characterInfo.skillInfo2))
                        {
                            if (character.characterInfo.skillInfo2.skillData.attackableType == AttackableType.Float)
                                skillItemView2.ShowMask(true);
                            skillItemView2.ShowFloatParticle(false);
                        }
                    }
                    break;
                case FightStatus.TumbleComboing:
                    if (character.characterInfo.skillInfo1 != null)
                    {
                        if (SkillUtil.AttackableTumble(character.characterInfo.skillInfo1))
                        {
                            if (character.characterInfo.skillInfo1.skillData.attackableType == AttackableType.Tumble)
                                skillItemView1.ShowMask(true);
                            skillItemView1.ShowTumbleParticle(false);
                        }
                    }
                    if (character.characterInfo.skillInfo2 != null)
                    {
                        if (SkillUtil.AttackableTumble(character.characterInfo.skillInfo2))
                        {
                            if (character.characterInfo.skillInfo2.skillData.attackableType == AttackableType.Tumble)
                                skillItemView2.ShowMask(true);
                            skillItemView2.ShowTumbleParticle(false);
                        }
                    }
                    break;
            }
        }

        private void PlayComboSkillEffect(SkillItemView skillItemView, AttackableType attackableType, float duration)
        {
            skillItemView.ShowMask(false);
            switch (attackableType)
            {
                case AttackableType.Float:
                    skillItemView.ShowFloatParticle(true);
                    break;
                case AttackableType.Tumble:
                    skillItemView.ShowTumbleParticle(true);
                    break;
            }
        }

        public void Shake()
        {
            if (!_isShaking)
            {
                LeanTween.moveLocalX(gameObject, 15, 0.05f)
                    .setEase(LeanTweenType.easeShake)
                        .setLoopOnce()
                        .setRepeat(3)
                        .setIgnoreTimeScale(true)
                        .setOnComplete(() =>
                        {
                            _isShaking = false;
                        });
                _isShaking = true;
            }
        }

        #region 事件
        public void AeonBtnClickHandler()
        {
#if UNITY_EDITOR
            if (!VirtualServerController.instance.fightEidtor)
#endif
                if (character.isDead || character.controled || character.Silence || !isCanPlayAeon || _isOrderAeonSkill) return;
            if (Fight.Controller.FightController.instance.fightStatus != FightStatus.Normal) return;
            SkillBarView skillBarView = UIMgr.instance.Get<SkillBarView>(SkillBarView.PREFAB_PATH);
            Canvas canvas = skillBarView.GetComponent<Canvas>();
            Logic.Effect.Controller.EffectController.instance.PlayUIEffect(Logic.Effect.Controller.EffectController.EFFECT_CLICKSKILL, Vector3.zero, Quaternion.identity, Vector3.one, 2f, canvas.sortingOrder, aeonBtn.transform);
            _isOrderAeonSkill = true;
            Logic.UI.SkillBar.Controller.SkillBarController.instance.OrderAeonSkill(character.characterInfo.instanceID, character.characterInfo.aeonSkill);
            ResetAngry();
            AudioController.instance.PlayAudio(AudioController.SKILL_CLICK, false);
        }
        #endregion
    }
}
