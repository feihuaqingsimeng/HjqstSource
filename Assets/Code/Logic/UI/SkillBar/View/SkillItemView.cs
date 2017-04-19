using UnityEngine;
using System.Collections;
using Logic.Skill.Model;
using Logic.Effect.Controller;
using UnityEngine.UI;
using Common.ResMgr;
using Logic.Enums;
using Common.Components.Effect;
using Logic.UI.FightTips.View;
using Logic.UI.Description.View;
using Common.Util;
using Logic.Skill;


namespace Logic.UI.SkillBar.View
{
    public class SkillItemView : MonoBehaviour
    {
        private const byte maxLevel = 1;
        private float pointDownTime;
        private const float DELAY = 1;
        public uint skillID;
        #region ui挂载
        public Image skillImage;
        public Image CDImage;
        public Image commonCDImage;
        public Image selectedImage;
        public Image levelImage;
        public Button skillBtn;
        public Image maskImage;
        public Image leftDesImage;
        public Image rightDesImage;
        public Image sortImage;
        public Canvas canvas;
        #endregion

        private GameObject _floatParticleGameObject;
        private GameObject _tumbleParticleGameObject;
        private GameObject _refreshParticleGameObject;

        private Image[] _images;
        private uint currentLevel //当前技能阶数
        {
            get
            {
                return _currentLevel;
            }
            set
            {
                _currentLevel = value;
                if (skillInfo != null)
                    skillInfo.currentLevel = value;
                if (_currentLevel > 0)
                    UpdateLevel(true);
            }
        }
        private uint _currentLevel = 0;
        public System.Action<SkillItemView> onClickSkillAction;
        public SkillItemBoxView skillItemBoxView = null;
        public SkillInfo skillInfo = null;
        private bool _enable = false;

        public bool Enable
        {
            get
            {
                return _enable;
            }
        }
        private bool _canOrder = true;
        public uint orderableLevel
        {
            get
            {
                if (Fight.Controller.FightController.instance.autoFight)
                    return (uint)Mathf.Max(Const.Model.ConstData.GetConstData().aiSkillRound - 1, 0);
                return 0;
            }
        }

        private bool _cancelable = false;
        public bool cancelable
        {
            get
            {
                bool result = _cancelable;
                result &= Logic.Net.Controller.DataMessageHandler.DataMessage_CanCancelSkillOrder(skillItemBoxView.character, skillInfo.skillData.skillId);
                return result;
            }
            set
            {
                _cancelable = value;
            }
        }
        private float _cancelTime = 0f;

        public bool canOrder
        {
            get
            {
                if (!_enable || skillItemBoxView.character.controled || skillItemBoxView.character.Silence) return false;
                bool result = currentLevel > orderableLevel && _canOrder;
                if (!result) return result;
                switch (Fight.Controller.FightController.instance.fightStatus)
                {
                    case FightStatus.Normal:
                        result &= SkillUtil.AttackableNormal(skillInfo);
                        result &= skillItemBoxView.character.canOrderSkill;
                        break;
                    case FightStatus.FloatWaiting:
                        result &= SkillUtil.AttackableFloat(skillInfo);
                        break;
                    case FightStatus.TumbleWaiting:
                        result &= SkillUtil.AttackableTumble(skillInfo);
                        break;
                    case FightStatus.FloatComboing:
                    case FightStatus.TumbleComboing:
                    case FightStatus.GameOver:
                        result = false;
                        break;
                }
                return result;
            }
            set
            {
                _canOrder = value;
            }
        }

        public int Sort
        {
            set
            {
                if (value <= 0)
                {
                    _cancelable = false;
                    sortImage.gameObject.SetActive(false);
                }
                else
                {
                    sortImage.gameObject.SetActive(true);
                    sortImage.SetSprite(ResMgr.instance.Load<Sprite>("sprite/num_skillcd/num_fight_cd_" + value.ToString()));
                    //sortImage.SetNativeSize();
                }
            }
        }

        private void UpdateLevel(bool state)
        {
            levelImage.gameObject.SetActive(false);//版本不显示技能阶数
            //levelImage.gameObject.SetActive(state);
            //if (state)
            //{
            //    levelImage.sprite = ResMgr.instance.Load<Sprite>("sprite/temporary_resources/num_skillCD_" + (currentLevel < maxLevel ? currentLevel.ToString() : "max"));
            //    levelImage.SetNativeSize();
            //}
            //SetSkillSpriteState(!state);
        }

        void Awake()
        {
            _images = GetComponentsInChildren<Image>(true);
            CDImage.fillAmount = 0;
            SetCommonCDState(0f);
            selectedImage.enabled = false;
            ShowMask(false);
            UpdateLevel(false);
            SetSkillSpriteState(true);
            leftDesImage.gameObject.SetActive(false);
            rightDesImage.gameObject.SetActive(false);
            sortImage.gameObject.SetActive(false);
        }

        public void LoadEffects()
        {
            _refreshParticleGameObject = ParticleUtil.CreateParticle(string.Format("effects/prefabs/{0}", EffectController.UI_EFFECT_28), canvas);
            _refreshParticleGameObject.transform.SetParent(transform, false);
            _refreshParticleGameObject.transform.localPosition = Vector3.zero;

            _floatParticleGameObject = ParticleUtil.CreateParticle(string.Format("effects/prefabs/{0}", EffectController.UI_ICON_FLOAT), canvas);
            _floatParticleGameObject.transform.SetParent(transform, false);
            _floatParticleGameObject.transform.localPosition = Vector3.zero;

            _tumbleParticleGameObject = ParticleUtil.CreateParticle(string.Format("effects/prefabs/{0}", EffectController.UI_ICON_TUMBLE), canvas);
            _tumbleParticleGameObject.transform.SetParent(transform, false);
            _tumbleParticleGameObject.transform.localPosition = Vector3.zero;

            ShowFloatParticle(false);
            ShowTumbleParticle(false);
            ShowRefreshParticle(false);
            ShowsSkillLock(false);
        }

        public void ResetSkillIcon()
        {
            skillImage.SetSprite(ResMgr.instance.Load<Sprite>("sprite/main_ui/icon_skill_lock"));
            leftDesImage.gameObject.SetActive(false);
            rightDesImage.gameObject.SetActive(false);
            sortImage.gameObject.SetActive(false);
        }

        public void SetSkillSprite(string spriteName)
        {
            CDImage.fillAmount = 1;
            SetSkillSpriteState(false);
            skillImage.SetSprite(ResMgr.instance.Load<Sprite>("sprite/skill/" + spriteName));
        }

        public void ShowMask(bool show)
        {
            maskImage.gameObject.SetActive(show);
        }

        public void ShowCommonCDImage(bool show)
        {
            commonCDImage.gameObject.SetActive(show);
        }

        public void SetCommonCDState(float progress)
        {
            commonCDImage.fillAmount = progress;
        }

        public void SetCDSkillSprite(string spriteName)
        {
            CDImage.gameObject.SetActive(true);
            //CDImage.sprite = ResMgr.instance.Load<Sprite>("sprite/skill/" + spriteName);
        }

        public void FinishSkill()
        {
            ResetSkillOrder();
            currentLevel = 0;
            UpdateLevel(false);
            CDImage.fillAmount = 1f;
        }

        public void ResetSkillOrder()
        {
            //if (currentLevel > 0)
            //    SetSkillSpriteState(false);
            selectedImage.enabled = false;
            canOrder = true;
            _cancelable = false;
        }

        public void SetSkillInfo(SkillInfo skillInfo)
        {
            if (skillInfo == null) return;
            if (this.skillInfo != null)
                if (this.skillInfo.skillData.skillId == skillInfo.skillData.skillId)//变身时，技能id相同，不处理
                    return;
            _enable = true;
            Sort = 0;
            this.skillInfo = skillInfo;
            skillBtn.enabled = true;
            SkillDesButton skillDesBtn = SkillDesButton.Get(gameObject);
            skillDesBtn.SetSkillInfo(skillInfo, 0, 0, 0.8f);
            SetSkillSprite(skillInfo.skillData.skillIcon);
            SetCDSkillSprite("icon_skill_cover");
            skillID = skillInfo.skillData.skillId;
            InitSkillDes();
        }

        private void InitSkillDes()
        {
            string path = Logic.Skill.SkillUtil.GetDesTypeIcon(skillInfo);
            if (!string.IsNullOrEmpty(path))
            {
                rightDesImage.SetSprite(Common.ResMgr.ResMgr.instance.Load<Sprite>(path));
                rightDesImage.SetNativeSize();
                rightDesImage.gameObject.SetActive(true);
            }
            else
                rightDesImage.gameObject.SetActive(false);
            string path2 = Logic.Skill.SkillUtil.GetDesTypeIcon2(skillInfo);
            if (!string.IsNullOrEmpty(path2))
            {
                leftDesImage.SetSprite(Common.ResMgr.ResMgr.instance.Load<Sprite>(path2));
                leftDesImage.SetNativeSize();
                leftDesImage.gameObject.SetActive(true);
            }
            else
                leftDesImage.gameObject.SetActive(false);


            switch (skillInfo.skillData.attackableType)
            {
                case AttackableType.Float:
                case AttackableType.Tumble:
                    ShowMask(true);
                    break;
            }
        }

        public void ResetSkillItem()
        {
            skillID = skillInfo.skillData.skillId;
            currentLevel = 0;
            UpdateLevel(false);
            _enable = true;
            canOrder = true;
            _cancelable = false;
            skillBtn.enabled = true;
            selectedImage.enabled = false;
            SetSkillSpriteState(false);
            skillItemBoxView.character.canOrderTime = Time.time;
            SetCommonCDState(0f);
            InitSkillDes();
        }


        public void SetDisable()
        {
            _enable = false;
            canOrder = false;
            _cancelable = false;
            CDImage.fillAmount = 0f;
            SetCommonCDState(0f);
            selectedImage.enabled = false;
            UpdateLevel(false);
            SetSkillSpriteState(true);
            skillBtn.enabled = false;
            leftDesImage.SetGray(true);
            rightDesImage.SetGray(true);
            //rightDesImage.gameObject.SetActive(false);
            currentLevel = 0;
            ShowFloatParticle(false);
            ShowTumbleParticle(false);
        }
		public void Show(bool value)
		{
			gameObject.SetActive(value);
		}
        public void SetSkillState(float cd)
        {
            if (!_canOrder) return;
            if (currentLevel == maxLevel)
                return;
            float maxCD = maxLevel * skillInfo.skillData.CD;
            if (cd > maxCD)
                cd = maxCD;
            uint level = (uint)(cd / skillInfo.skillData.CD);
            //if (level >= maxLevel)
            //    level = maxLevel;
            currentLevel = level;
            //CDImage.fillAmount = 1f - (cd - level * skillInfo.skillData.CD) / skillInfo.skillData.CD;
            if (level == maxLevel)
                CDImage.fillAmount = 0;
            else
                CDImage.fillAmount = 1f - (cd - level * skillInfo.skillData.CD) / skillInfo.skillData.CD;
            if (canOrder)
            {
                MechanicsType mechanicsType = Logic.Skill.SkillUtil.GetSkillMechanicsType(skillInfo);
                if (mechanicsType == MechanicsType.Float)
                    ShowFloatParticle(true);
                else if (mechanicsType == MechanicsType.Tumble)
                    ShowTumbleParticle(true);
                else
                    ShowRefreshParticle(true);
            }
        }

        public void UpdateSkillLockState()
        {
            if (skillItemBoxView.character.controled || skillItemBoxView.character.Silence)
                ShowsSkillLock(true);
            else
                ShowsSkillLock(false);
        }

        public void SetImagesColor(Color color)
        {
            for (int i = 0, count = _images.Length; i < count; i++)
            {
                if (_images[i] == skillImage)
                    continue;
                _images[i].color = color;
            }
        }

        public void SetSkillImage(bool isComboWait, Color color)
        {
            if (isComboWait)
            {
                skillImage.SetGray(false);
                skillImage.color = color;
            }
            else
            {
                skillImage.color = color;
                //if (currentLevel == 0)
                //    skillImage.SetGray(true);
            }
        }

        private void SetSkillSpriteState(bool gray)
        {
            skillImage.SetGray(gray);
        }

        public void ShowFloatParticle(bool show)
        {
            if (!canOrder && show) return;
            _floatParticleGameObject.SetActive(show);
            if (show)
            {
                ParticlePlayer pp = _floatParticleGameObject.AddComponent<ParticlePlayer>();
                pp.isLoop = true;
                pp.isUI = true;
                //pp.speed = Game.GameSetting.instance.speed;
            }
            else
            {
                ParticlePlayer pp = _floatParticleGameObject.GetComponent<ParticlePlayer>();
                if (pp)
                    UnityEngine.Object.Destroy(pp);
            }
        }

        public void ShowTumbleParticle(bool show)
        {
            if (!canOrder && show) return;
            _tumbleParticleGameObject.SetActive(show);
            if (show)
            {
                ParticlePlayer pp = _tumbleParticleGameObject.AddComponent<ParticlePlayer>();
                pp.isLoop = true;
                pp.isUI = true;
                //pp.speed = Game.GameSetting.instance.speed;
            }
            else
            {
                ParticlePlayer pp = _tumbleParticleGameObject.GetComponent<ParticlePlayer>();
                if (pp)
                    UnityEngine.Object.Destroy(pp);
            }
        }

        private void ShowRefreshParticle(bool show)
        {
            _refreshParticleGameObject.SetActive(show);
            if (show)
            {
                ParticlePlayer pp = _refreshParticleGameObject.AddComponent<ParticlePlayer>();
                pp.isLoop = true;
                pp.isUI = true;
                //pp.speed = Game.GameSetting.instance.speed;
            }
            else
            {
                ParticlePlayer pp = _refreshParticleGameObject.GetComponent<ParticlePlayer>();
                if (pp)
                    UnityEngine.Object.Destroy(pp);
            }
        }

        private void ShowsSkillLock(bool show)
        {
            skillImage.SetGray(show);
        }

        #region 事件
        public void SkillClickHandler()
        {
            if (canOrder)
            {
                Debugger.Log("order skill");
#if UNITY_EDITOR
                if (!Logic.Net.Controller.VirtualServerController.instance.fightEidtor)
#endif
                    if (!Fight.Controller.FightController.instance.autoFight)
                        if (Time.realtimeSinceStartup - pointDownTime > DELAY)//长按忽略技能释放
                            return;
                switch (Fight.Controller.FightController.instance.fightStatus)
                {
                    case FightStatus.Normal:
                        MechanicsType mechanicsType = Logic.Skill.SkillUtil.GetSkillMechanicsType(skillInfo);//点击浮空和倒地技能，提示特效消失  
                        if (mechanicsType == MechanicsType.Float)
                            ShowFloatParticle(false);
                        else if (mechanicsType == MechanicsType.Tumble)
                            ShowTumbleParticle(false);
                        break;
                    case FightStatus.FloatWaiting:
                        ShowFloatParticle(false);
                        break;
                    case FightStatus.TumbleWaiting:
                        ShowTumbleParticle(false);
                        break;
                }
                ShowRefreshParticle(false);
                selectedImage.enabled = true;
                //SetSkillSpriteState(true);
                SkillBarView skillBarView = UIMgr.instance.Get<SkillBarView>(SkillBarView.PREFAB_PATH);
                Canvas canvas = skillBarView.canvas;
                EffectController.instance.PlayUIEffect(Logic.Effect.Controller.EffectController.EFFECT_CLICKSKILL, Vector3.zero, Quaternion.identity, Vector3.one, 2f, canvas.sortingOrder, transform);
                canOrder = false;
                _cancelable = true;
                _cancelTime = Time.time;
                if (skillItemBoxView.character.characterInfo.skillId1 == skillID && skillID != 0)
                    skillItemBoxView.character.canOrderSkill1 = false;
                else if (skillItemBoxView.character.characterInfo.skillId2 == skillID && skillID != 0)
                    skillItemBoxView.character.canOrderSkill2 = false;
                skillInfo.currentLevel = currentLevel;
                if (onClickSkillAction != null)
                    onClickSkillAction(this);
            }
            else if (cancelable && !Fight.Controller.FightController.instance.isWaitingCombo && !Fight.Controller.FightController.instance.isComboing/*&& Time.time - _cancelTime >= 2f*/)
            {
                _cancelable = false;
                Logic.Net.Controller.DataMessageHandler.DataMessage_ResetSkillOrder(skillItemBoxView.character, skillInfo.skillData.skillId, true);
            }
        }

        public void SkillPointDownHandler()
        {
            pointDownTime = Time.realtimeSinceStartup;
            if (canOrder)
            {
                this.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            }
        }

        public void SkillPointUpHandler()
        {
            this.transform.localScale = Vector3.one;
        }
        #endregion
    }
}