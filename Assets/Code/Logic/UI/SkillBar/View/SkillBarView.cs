using UnityEngine;
using UnityEngine.UI;
using Logic.Character.Controller;
using Logic.Character;
using Logic.Hero.Model;
using System.Collections.Generic;
using Logic.Fight.Controller;
using Common.Util;
using Logic.Player.Model;
using Common.ResMgr;
using Logic.UI.SkillBar.Controller;
namespace Logic.UI.SkillBar.View
{
    public class SkillBarView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/skillbar/skill_bar_view";
        #region ui
        public GameObject core;
        public GameObject skillBoxGO;
        public GameObject skillRoot;
        public Vector3 originalPos = new Vector3(0, -210, 0);
        public Vector3 newPos = Vector3.zero;
        public Canvas canvas;
        public Image maskImg;
        #endregion

        public List<SkillItemBoxView> skillItemBoxViewList;
        private Dictionary<string, SkillItemView> skillItemViewDic = new Dictionary<string, SkillItemView>();

        public static SkillBarView Open()
        {
            SkillBarView skillBarView = UIMgr.instance.Open<SkillBarView>(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay);
            return skillBarView;
        }

        void Awake()
        {
            core.transform.localPosition = originalPos;
        }
        // Use this for initialization
        void Start()
        {
            InitSkill();
        }

        public bool showSkillBar
        {
            set
            {
                if (value)
                {
                    LeanTween.moveLocal(core, newPos, 0.6f).setEase(LeanTweenType.easeInOutCubic);
                }
                else
                {
                    LeanTween.moveLocal(core, originalPos, 0.6f).setEase(LeanTweenType.easeInOutCubic);
                }
            }
        }

        public bool showMask
        {
            set
            {
                maskImg.gameObject.SetActive(value);
            }
        }

        private void InitSkill()
        {
            skillBoxGO.SetActive(false);
            List<HeroEntity> heros = PlayerController.instance.heros;
            heros.Sort(CharacterUtil.PlayerFirst);
            for (int i = 0, count = heros.Count; i < count; i++)
            {
                CharacterEntity character = heros[i];
                SkillItemBoxView skillItemBoxView = CreateSkillItemBoxView();
                skillItemBoxView.gameObject.name = character.characterName;
                if (character is PlayerEntity)
                {
                    PlayerEntity player = character as PlayerEntity;
                    PlayerInfo playerInfo = PlayerController.instance.GetPlayerInfo();
#if UNITY_EDITOR
                    if (FightController.imitate)
                        playerInfo = PlayerController.instance.imitatePlayerInfoDic.First().Value;
#endif
                    player.skillItemBoxView = skillItemBoxView;
                    skillItemBoxView.headSprite = ResMgr.instance.Load<Sprite>(playerInfo.HeadIcon);
                }
                else if (character is HeroEntity)
                {
                    HeroEntity hero = character as HeroEntity;
                    HeroInfo heroInfo = PlayerController.instance.GetHeroInfo(character.characterInfo.instanceID);
#if UNITY_EDITOR
                    if (FightController.imitate)
                        heroInfo = PlayerController.instance.GetImitateHeroInfo((int)character.characterInfo.instanceID);
#endif
                    hero.skillItemBoxView = skillItemBoxView;
                    skillItemBoxView.headSprite = ResMgr.instance.Load<Sprite>(heroInfo.HeadIcon);
                }
                else
                {
                    Debugger.LogError("these is not this type: " + character.GetType());
                }
#if UNITY_EDITOR
                if (FightController.imitate)
                    showMask = false;
#endif
                skillItemBoxView.OnSkillClick = null;
                skillItemBoxView.OnSkillClick += OnClickSkill;
                skillItemBoxView.character = character;
                skillItemBoxView.InitSkill();
                if (character.characterInfo.skillInfo1 != null)
                {
                    //skillItemBoxView.skillItemView1.Show(true);
                    //skillItemBoxView.skillItemView1.SetSkillInfo(character.characterInfo.skillInfo1);
                    skillItemViewDic.TryAdd(StringUtil.ConcatNumber(character.characterInfo.instanceID, character.characterInfo.skillId1), skillItemBoxView.skillItemView1);
                }
                if (character.characterInfo.skillInfo2 != null)
                {
                    //skillItemBoxView.skillItemView2.Show(true);
                    //skillItemBoxView.skillItemView2.SetSkillInfo(character.characterInfo.skillInfo2);
                    skillItemViewDic.TryAdd(StringUtil.ConcatNumber(character.characterInfo.instanceID, character.characterInfo.skillId2), skillItemBoxView.skillItemView2);
                }
                if (character.characterInfo.aeonSkillInfo != null)
                {
                    skillItemBoxView.SetAeonSkillSprite(character.characterInfo.aeonSkillInfo.skillData.skillIcon);
                }
                skillItemBoxViewList.Add(skillItemBoxView);
            }
            showSkillBar = true;
        }

        private void OnClickSkill(uint userID, SkillItemView skillItemView)
        {
            SkillBarController.instance.OrderPlayerSkill(userID, skillItemView);
        }

        private SkillItemBoxView CreateSkillItemBoxView()
        {
            GameObject go = Object.Instantiate(skillBoxGO) as GameObject;
            go.SetActive(true);
            SkillItemBoxView skillItemBoxView = go.GetComponent<SkillItemBoxView>();
            skillItemBoxView.Init();
            skillItemBoxView.transform.SetParent(skillRoot.transform, false);
            return skillItemBoxView;
        }

        public void FinishSkill(uint userID, uint skillID)
        {
            SkillItemBoxView skillItemBoxView = FindSkillItemViewByUserId(userID);
            if (skillItemBoxView)
            {
                skillItemBoxView.FinishSkill(skillID);
            }
        }

        public void SetAngry(float angry)
        {
            SkillItemBoxView skillItemBoxView = FindSkillItemViewOfPlayer();
            if (skillItemBoxView != null)
                skillItemBoxView.SetAngry(angry);
        }

        public SkillItemView FindSkillItem(string baseAndSkillId)
        {
            if (skillItemViewDic.ContainsKey(baseAndSkillId))
                return skillItemViewDic[baseAndSkillId];
            Debugger.Log("找不到角色技能ID" + baseAndSkillId);
            return null;
        }

        public void ResetSkillOrder(uint id, uint skillId)
        {
            for (int i = 0, count = skillItemBoxViewList.Count; i < count; i++)
            {
                SkillItemBoxView skillItemBoxView = skillItemBoxViewList[i];
                if (skillItemBoxView.isEnable && skillItemBoxView.character.characterInfo.instanceID == id)
                    skillItemBoxView.ResetSkillOrder(skillId);
            }
        }

        public void ResetSkillOrder()
        {
            for (int i = 0, count = skillItemBoxViewList.Count; i < count; i++)
            {
                SkillItemBoxView skillItemBoxView = skillItemBoxViewList[i];
                if (skillItemBoxView.isEnable)
                    skillItemBoxView.ResetSkillOrder();
            }
        }

        public void DesaltOnCombo(float duration)
        {
            for (int i = 0, count = skillItemBoxViewList.Count; i < count; i++)
            {
                skillItemBoxViewList[i].DesaltOnCombo(duration);
            }
            Logic.UI.SkillBanner.View.SkillBannerView skillBannerView = Logic.UI.SkillBanner.View.SkillBannerView.Open();
            skillBannerView.ShowParticle(true);
        }

        public void ShowAfterWaitCombo()
        {
            for (int i = 0, count = skillItemBoxViewList.Count; i < count; i++)
            {
                skillItemBoxViewList[i].ShowAfterWaitCombo();
            }
            Logic.UI.SkillBanner.View.SkillBannerView skillBannerView = Logic.UI.SkillBanner.View.SkillBannerView.Open();
            skillBannerView.ShowParticle(false);
        }

        public void ShowMaskStartCombo()
        {
            for (int i = 0, count = skillItemBoxViewList.Count; i < count; i++)
            {
                skillItemBoxViewList[i].ShowMaskStartCombo();
            }
        }

        public void ShowMaskAfterCombo()
        {
            for (int i = 0, count = skillItemBoxViewList.Count; i < count; i++)
            {
                skillItemBoxViewList[i].ShowMaskAfterCombo();
            }
        }

        private void ResetSkillItem()
        {
            for (int i = 0, count = skillItemBoxViewList.Count; i < count; i++)
            {
                SkillItemBoxView skillItemBoxView = skillItemBoxViewList[i];
                if (skillItemBoxView.isEnable)
                    skillItemBoxView.ResetSkillItem();
            }
        }

        private SkillItemBoxView FindSkillItemViewByUserId(uint userID)
        {
            for (int i = 0, count = skillItemBoxViewList.Count; i < count; i++)
            {
                SkillItemBoxView skillItemBoxView = skillItemBoxViewList[i];
                if (!skillItemBoxView.isEnable) continue;
                if (skillItemBoxView.character != null)
                {
                    if (skillItemBoxView.character.characterInfo.instanceID == userID)
                        return skillItemBoxView;
                }
            }
            return null;
        }

        private SkillItemBoxView FindSkillItemViewBySkillId(uint skillID)
        {
            for (int i = 0, count = skillItemBoxViewList.Count; i < count; i++)
            {
                SkillItemBoxView skillItemBoxView = skillItemBoxViewList[i];
                if (!skillItemBoxView.isEnable) continue;
                if (skillItemBoxView.character != null)
                {
                    if (skillItemBoxView.character.characterInfo.skillId1 == skillID || skillItemBoxView.character.characterInfo.skillId2 == skillID)
                        return skillItemBoxView;
                }
            }
            return null;
        }

        public SkillItemBoxView FindSkillItemViewOfPlayer()
        {
            for (int i = 0, count = skillItemBoxViewList.Count; i < count; i++)
            {
                SkillItemBoxView skillItemBoxView = skillItemBoxViewList[i];
                if (!skillItemBoxView.isEnable) continue;
                if (skillItemBoxView.character is PlayerEntity)
                    return skillItemBoxView;
            }
            return null;
        }

        #region 事件


        #endregion
    }
}
