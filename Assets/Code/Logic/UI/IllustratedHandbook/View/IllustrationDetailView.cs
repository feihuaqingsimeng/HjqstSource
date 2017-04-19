using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Logic.Role.Model;
using Logic.UI.Hero.View;
using Logic.Enums;
using Logic.Character;
using Logic.Player.Model;
using Logic.Hero.Model;
using Logic.Role;
using Common.Util;
using Logic.Skill.Model;
using Common.ResMgr;
using Common.Localization;
using Logic.Game.Model;
using Logic.UI.CommonTopBar.View;
using Common.Animators;
using Logic.UI.GoodsJump.Model;
using Logic.UI.Description.View;
using Logic.Fight.Model;
using Logic.Fight.Controller;
using Logic.Protocol.Model;
using Logic.Skill;

namespace Logic.UI.IllustratedHandbook.View
{
    public class IllustrationDetailView : MonoBehaviour
    {

        public const string PREFAB_PATH = "ui/illustrated_handbook/illustration_detail_view";

        private CharacterEntity _characterEntity;
        public static IllustrationDetailView Open(RoleInfo info)
        {
            IllustrationDetailView view = UIMgr.instance.Open<IllustrationDetailView>(PREFAB_PATH);
            view.SetRoleInfo(info);
            return view;
        }

        #region ui component
        public Text roleNameText;
        public Text roleLevelText;
        public Image roleTypeImage;
        public Transform panel;
        public Transform roleModelRoot;
        public RoleAttributeView roleAttributePrefab;
        public Transform roleAttributeRoot;
        public Text descriptionText;
        public SkillDesButton[] skillDesButton;
		public List<Image> skillTypeIconImages;
        #endregion

        private RoleInfo _roleInfo;
        private Dictionary<RoleAttributeType, int> _attrDic = new Dictionary<RoleAttributeType, int>();

        void Awake()
        {
            CommonTopBarView view = CommonTopBarView.CreateNewAndAttachTo(panel);
            view.SetAsCommonStyle(Localization.Get("ui.illustration_view.roleFormation"), OnClickCloseBtnHandler, true, true, true, false);
        }

        private void SetRoleInfo(RoleInfo info)
        {
            _roleInfo = info;
            InitAttribute();
            Refresh();
        }
        private void InitAttribute()
        {
            _attrDic.Clear();
            _attrDic.Add(RoleAttributeType.HP, 1);

            RoleType roleType = _roleInfo.heroData.roleType;
            RoleAttackAttributeType roleAttackAttributeType = CharacterUtil.GetRoleAttackAttributeType(roleType);
            if (roleAttackAttributeType == RoleAttackAttributeType.PhysicalAttack)
            {
                _attrDic.Add(RoleAttributeType.NormalAtk, 1);
            }
            else
            {
                _attrDic.Add(RoleAttributeType.MagicAtk, 1);
            }
			_attrDic.Add(RoleAttributeType.Normal_Def, 1);
            _attrDic.Add(RoleAttributeType.Speed, 1);

        }
        private void Refresh()
        {
            RefreshTitle();
            RefreshAttribute();
            RefreshRoleModel();
            RefreshSkill();
            
        }
        private void RefreshTitle()
        {
			roleTypeImage.SetSprite(UIUtil.GetRoleTypeBigIconSprite(_roleInfo.heroData.roleType));
            roleNameText.text = Localization.Get(_roleInfo.heroData.name);
            roleLevelText.text = string.Format("{0}/{1}", _roleInfo.level, GlobalData.GetGlobalData().playerLevelMax);

            descriptionText.text = Localization.Get(_roleInfo.heroData.description);
            RectTransform parent = descriptionText.transform.parent as RectTransform;
            parent.sizeDelta = new Vector2(parent.sizeDelta.x, descriptionText.preferredHeight + 10);
        }
        private void RefreshRoleModel()
        {
            StartCoroutine(RefreshRoleModelCoroutine());
        }

        void DespawnCharacter()
        {
            if (_characterEntity)
                Pool.Controller.PoolController.instance.Despawn(_characterEntity.name, _characterEntity);
            _characterEntity = null;
        }

        private IEnumerator RefreshRoleModelCoroutine()
        {
            DespawnCharacter();
            TransformUtil.ClearChildren(roleModelRoot, true);
            yield return new WaitForSeconds(0.4f);
            PlayerInfo playerInfo = _roleInfo as PlayerInfo;
            HeroInfo heroInfo = _roleInfo as HeroInfo;
            if (playerInfo != null)
            {
                CharacterEntity characterEntity = CharacterEntity.CreatePlayerEntityAsUIElement(playerInfo, roleModelRoot, true, true);
                Action.Controller.ActionController.instance.PlayerAnimAction(characterEntity, AnimatorUtil.VICOTRY_ID);
            }
            else if (heroInfo != null)
            {
                bool canClick = true;
                if (_roleInfo.heroData.hero_type == 4)//boss
                {
                    canClick = false;
                }
                _characterEntity = CharacterEntity.CreateHeroEntityAsUIElement(heroInfo, roleModelRoot, canClick, true);
                if (canClick)
                    Action.Controller.ActionController.instance.PlayerAnimAction(_characterEntity, AnimatorUtil.VICOTRY_ID);
            }
        }
        private void RefreshAttribute()
        {
            TransformUtil.ClearChildren(roleAttributeRoot, true);

            Dictionary<RoleAttributeType, RoleAttribute> roleAttributesDic = RoleUtil.CalcRoleAttributesDic(_roleInfo);

            roleAttributePrefab.gameObject.SetActive(true);
            foreach (var value in _attrDic)
            {
                RoleAttributeView view = Instantiate<RoleAttributeView>(roleAttributePrefab);
                view.transform.SetParent(roleAttributeRoot, false);
                view.Set(roleAttributesDic[value.Key]);
            }
            roleAttributePrefab.gameObject.SetActive(false);

        }
        private void RefreshSkill()
        {
            SkillData skillData1 = SkillData.GetSkillDataById(_roleInfo.heroData.skillId1);
            if (skillData1 != null)
            {
				skillDesButton[0].SetSkillInfo(_roleInfo.heroData.skillId1,_roleInfo.advanceLevel,(int)_roleInfo.heroData.starMin);
				skillDesButton[0].GetComponent<Image>().SetSprite( ResMgr.instance.Load<Sprite>(ResPath.GetSkillIconPath(skillData1.skillIcon)));
				skillTypeIconImages[0].SetSprite( ResMgr.instance.LoadSprite(SkillUtil.GetDesTypeIcon(skillData1)));
				skillTypeIconImages[0].gameObject.SetActive(skillTypeIconImages[0].sprite != null);
            }
            else
            {
                skillDesButton[0].SetSkillInfo(null,0,0);
				skillDesButton[0].GetComponent<Image>().SetSprite(GetDefaultSkillIcon());
				skillTypeIconImages[0].gameObject.SetActive(false);
            }
            SkillData skillData2 = SkillData.GetSkillDataById(_roleInfo.heroData.skillId2);
            if (skillData2 != null)
            {
				skillDesButton[1].SetSkillInfo(_roleInfo.heroData.skillId2,_roleInfo.advanceLevel,(int)_roleInfo.heroData.starMin);
				skillDesButton[1].GetComponent<Image>().SetSprite(ResMgr.instance.Load<Sprite>(ResPath.GetSkillIconPath(skillData2.skillIcon)));
				skillTypeIconImages[1].SetSprite(ResMgr.instance.LoadSprite(SkillUtil.GetDesTypeIcon(skillData2)));
				skillTypeIconImages[1].gameObject.SetActive(skillTypeIconImages[1].sprite != null);
            }
            else
            {
                skillDesButton[1].SetSkillInfo(null,0,0);
				skillDesButton[1].GetComponent<Image>().SetSprite(GetDefaultSkillIcon());
				skillTypeIconImages[1].gameObject.SetActive(false);
            }
            SkillData passiveSkillData = SkillData.GetSkillDataById(_roleInfo.heroData.passiveId1);
            if (passiveSkillData != null)
            {
				skillDesButton[2].SetSkillInfo(_roleInfo.heroData.passiveId1,_roleInfo.advanceLevel,(int)_roleInfo.heroData.starMin);
				skillDesButton[2].GetComponent<Image>().SetSprite(ResMgr.instance.Load<Sprite>(ResPath.GetSkillIconPath(passiveSkillData.skillIcon)));
            }
            else
            {
                skillDesButton[2].SetSkillInfo(null,0,0);
				skillDesButton[2].GetComponent<Image>().SetSprite(GetDefaultSkillIcon());
            }
            PlayerInfo playerInfo = _roleInfo as PlayerInfo;
            if (playerInfo != null)
            {
//                SkillData summonData = SkillData.GetSkillDataById(playerInfo.playerData.summonSkillId);
//                if (summonData != null)
//                {
//					skillDesButton[3].SetSkillInfo(playerInfo.playerData.summonSkillId,_roleInfo.advanceLevel,(int)_roleInfo.heroData.starMin);
//                    skillDesButton[3].GetComponent<Image>().sprite = ResMgr.instance.Load<Sprite>(ResPath.GetSkillIconPath(summonData.skillIcon));
//                }
//                else
//                {
                    skillDesButton[3].SetSkillInfo(null,0,0);
				skillDesButton[3].GetComponent<Image>().SetSprite(GetDefaultSkillIcon());
//                }
            }
            else
            {
                skillDesButton[3].SetSkillInfo(null,0,0);
				skillDesButton[3].GetComponent<Image>().SetSprite(GetDefaultSkillIcon());
            }
        }

        private Sprite GetDefaultSkillIcon()
        {
            return ResMgr.instance.Load<Sprite>(ResPath.GetSkillIconPath("icon_skill_none"));
        }
        private void OnClickCloseBtnHandler()
        {
            UIMgr.instance.Close(PREFAB_PATH);
        }
        public void ClickPathHandler()
        {
            GameResData data = new GameResData(BaseResType.Hero, _roleInfo.modelDataId, 0, _roleInfo.advanceLevel);
            GoodsJump.View.GoodsJumpPathView.Open(data);
        }
        public void ClickSkillDisplayHandler()
        {
            //			Logic.UI.Tips.View.CommonAutoDestroyTipsView.Open("暂未开放");
            //			return;
            Dictionary<RoleAttributeType, RoleAttribute> attrDic = RoleUtil.CalcRoleAttributesDic(_roleInfo);

            if (_roleInfo is PlayerInfo)
            {
                PlayerFightProtoData pfpd = new PlayerFightProtoData();
                pfpd.posIndex = 5;
                pfpd.attr = new HeroAttrProtoData();
                pfpd.attr.hpUp = pfpd.attr.hp = (int)attrDic.GetValue(RoleAttributeType.HP).value;
                pfpd.attr.magic_atk = (int)attrDic.GetValue(RoleAttributeType.MagicAtk).value;
                pfpd.attr.normal_atk = (int)attrDic.GetValue(RoleAttributeType.NormalAtk).value;
				pfpd.attr.normal_def = pfpd.attr.magic_def = (int)attrDic.GetValue(RoleAttributeType.Normal_Def).value;
				//pfpd.attr.magic_def = (int)attrDic.GetValue(RoleAttributeType.m).value;
				pfpd.attr.speed = (int)attrDic.GetValue(RoleAttributeType.Speed).value;
				pfpd.attr.hit = (int)attrDic.GetValue(RoleAttributeType.Hit).value;
				pfpd.attr.dodge = (int)attrDic.GetValue(RoleAttributeType.Dodge).value;
				pfpd.attr.crit = (int)attrDic.GetValue(RoleAttributeType.Crit).value;
				pfpd.attr.anti_crit = (int)attrDic.GetValue(RoleAttributeType.AntiCrit).value;
				pfpd.attr.block = (int)attrDic.GetValue(RoleAttributeType.Block).value;
				pfpd.attr.anti_block = (int)attrDic.GetValue(RoleAttributeType.AntiBlock).value;
				pfpd.attr.counter_atk = (int)attrDic.GetValue(RoleAttributeType.CounterAtk).value;
				pfpd.attr.crit_hurt_add = (int)attrDic.GetValue(RoleAttributeType.CritHurtAdd).value;
				pfpd.attr.crit_hurt_dec = (int)attrDic.GetValue(RoleAttributeType.CritHurtDec).value;
				pfpd.attr.armor = (int)attrDic.GetValue(RoleAttributeType.Armor).value;
				pfpd.attr.damage_add = (int)attrDic.GetValue(RoleAttributeType.DamageAdd).value;
				pfpd.attr.damage_dec = (int)attrDic.GetValue(RoleAttributeType.DamageDec).value;
                //pfpd.attr.hit = 100;
                FightPlayerInfo fightPlayerInfo = new FightPlayerInfo(_roleInfo as PlayerInfo, pfpd);
                FightProxy.instance.SetFightPlayerInfo(fightPlayerInfo);
                FightProxy.instance.SetFightHeroInfoList(new List<FightHeroInfo>());
            }
            else
            {
                HeroFightProtoData hfpd = new HeroFightProtoData();
                hfpd.posIndex = 5;
                hfpd.attr = new HeroAttrProtoData();
                hfpd.attr.hp = hfpd.attr.hpUp = (int)attrDic.GetValue(RoleAttributeType.HP).value;
                hfpd.attr.magic_atk = (int)attrDic.GetValue(RoleAttributeType.MagicAtk).value;
                hfpd.attr.normal_atk = (int)attrDic.GetValue(RoleAttributeType.NormalAtk).value;
				hfpd.attr.normal_def = hfpd.attr.magic_def =(int)attrDic.GetValue(RoleAttributeType.Normal_Def).value;
				//pfpd.attr.magic_def = (int)attrDic.GetValue(RoleAttributeType.m).value;
				hfpd.attr.speed = (int)attrDic.GetValue(RoleAttributeType.Speed).value;
				hfpd.attr.hit = (int)attrDic.GetValue(RoleAttributeType.Hit).value;
				hfpd.attr.dodge = (int)attrDic.GetValue(RoleAttributeType.Dodge).value;
				hfpd.attr.crit = (int)attrDic.GetValue(RoleAttributeType.Crit).value;
				hfpd.attr.anti_crit = (int)attrDic.GetValue(RoleAttributeType.AntiCrit).value;
				hfpd.attr.block = (int)attrDic.GetValue(RoleAttributeType.Block).value;
				hfpd.attr.anti_block = (int)attrDic.GetValue(RoleAttributeType.AntiBlock).value;
				hfpd.attr.counter_atk = (int)attrDic.GetValue(RoleAttributeType.CounterAtk).value;
				hfpd.attr.crit_hurt_add = (int)attrDic.GetValue(RoleAttributeType.CritHurtAdd).value;
				hfpd.attr.crit_hurt_dec = (int)attrDic.GetValue(RoleAttributeType.CritHurtDec).value;
				hfpd.attr.armor = (int)attrDic.GetValue(RoleAttributeType.Armor).value;
				hfpd.attr.damage_add = (int)attrDic.GetValue(RoleAttributeType.DamageAdd).value;
				hfpd.attr.damage_dec = (int)attrDic.GetValue(RoleAttributeType.DamageDec).value;
                //hfpd.attr.hit = 100;
                FightHeroInfo fightHeroInfo = new FightHeroInfo(_roleInfo as HeroInfo, hfpd);
                List<FightHeroInfo> heroInfoList = new List<FightHeroInfo>();
                heroInfoList.Add(fightHeroInfo);
                FightProxy.instance.SetFightHeroInfoList(heroInfoList);
            }
            //enemy
            HeroInfo enemyHero = new HeroInfo(HeroData.HeroDataDictionary.First().Key);
            HeroFightProtoData ehfpd = new HeroFightProtoData();
            ehfpd.posIndex = 5;
            ehfpd.attr = new HeroAttrProtoData();
            ehfpd.attr.hp = ehfpd.attr.hpUp = 100000;
            //			ehfpd.attr.magic_atk = 1;
            //			ehfpd.attr.normal_atk = 1;
            ehfpd.attr.speed = 25;
            FightHeroInfo efightHeroInfo = new FightHeroInfo(enemyHero, ehfpd);
            List<FightHeroInfo> eheroInfoList = new List<FightHeroInfo>();
            eheroInfoList.Add(efightHeroInfo);
            FightProxy.instance.SetEnemyFightHeroInfoList(eheroInfoList);


            UI.UIMgr.instance.Close(Logic.UI.EUISortingLayer.MainUI);
            FightController.instance.fightType = Logic.Enums.FightType.SkillDisplay;
            Logic.UI.LoadGame.Controller.LoadGameController.instance.SetDelayTime(0.5f, LoadFinishedCallback);
        }

        private void LoadFinishedCallback()
        {
            FightController.instance.ReadyFight();
        }

        void OnDestroy()
        {
            DespawnCharacter();
        }
    }
}

