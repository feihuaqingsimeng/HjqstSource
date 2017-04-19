using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common.Localization;
using Common.Util;
using Common.ResMgr;
using Logic.Game;
using Logic.Game.Model;
using Logic.Player;
using Logic.Player.Model;
using Logic.Equipment.Model;
using Logic.Character;
using Logic.UI.Equipments.View;
using Logic.Enums;
using Logic.Hero.Model;
using Logic.Skill.Model;
using Logic.UI.CommonTopBar.View;
using Logic.UI.Description.View;

namespace Logic.UI.Player.View
{
    public class PlayerView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/player/player_view";
        private CharacterEntity _characterEntity;
        public static PlayerView Open()
        {
            PlayerView view = UIMgr.instance.Open<PlayerView>(PREFAB_PATH);
            return view;
        }
        #region UI components
        public GameObject core;
        private CommonTopBarView _commonTopBarView;

        public Image roleTypeIconImage;
        public Text levelTitleText;
        public Text currentLevelText;
        public Text currentMaxLevelText;
        public Text changeProfessionAndStrengthen;
        public Text accountExpPercentText;
        public Text playerExpPercentText;
        public Text playerNameAndLevelText;
        public Text hpTitleText;
        public Text offenceTitleText;
        public Text defenceTitleText;
        public Text actionPointTitleText;

        public Slider accountExpSlider;
        public Slider playerExpSlider;

        public Text hpText;
        public Text offenceText;
        public Text defenceText;
        public Text speedText;

        public Text critText;
        public Text dodgeText;
        public Text blockText;
        public Text hitText;

        public Text hpAddText;
        public Text defenceAddText;
        public Text attackAddText;
        public Text actionPointAddText;
        public Text critAddText;
        public Text dodgeAddText;
        public Text blockAddText;
        public Text hitAddText;

        public Text activeSkillTitleText;
        public Text passiveSkillTitleText;
        public Text ultimateSummonText;
        public Text summonDetialText;
        public Transform player_model_root_transform;

        private bool _isSkillsPanelShowing = true;
        public GameObject skillsPanelShowHideArrow;
        public GameObject skillsPanelRootGameObject;
        public Transform skillPanelShowPosTransform;
        public Transform skillPanelHidePosTransform;

        public EquipmentIcon weaponIcon;
        public EquipmentIcon armorIcon;
        public EquipmentIcon accessoryIcon;
        public Button[] equipmentButtons;
        public SkillDesButton[] skillDesButton;
        #endregion

        void Awake()
        {
            Init();
            BindDelegate();
        }

        void OnDestroy()
        {
            DespawnCharacter();
            UnbindDelegate();
        }

        private void BindDelegate()
        {
            GameProxy.instance.onAccountInfoUpdateDelegate += OnAccountInfoUpdateHandler;
            PlayerProxy.instance.onPlayerInfoUpdateDelegate += OnPlayerInfoUpdateHandler;
            EquipmentProxy.instance.onEquipmentInfoListUpdateDelegate += OnEquipmentInfoListUpdateHandler;
        }

        private void UnbindDelegate()
        {
            GameProxy.instance.onAccountInfoUpdateDelegate -= OnAccountInfoUpdateHandler;
            PlayerProxy.instance.onPlayerInfoUpdateDelegate -= OnPlayerInfoUpdateHandler;
            EquipmentProxy.instance.onEquipmentInfoListUpdateDelegate -= OnEquipmentInfoListUpdateHandler;
        }

        private void Init()
        {
            PlayerInfo playerInfo = GameProxy.instance.PlayerInfo;
            _commonTopBarView = CommonTopBarView.CreateNewAndAttachTo(core.transform);
//            _commonTopBarView.SetAsRoleViewStyle(playerInfo.name, ClickCloseHandler);
			_commonTopBarView.SetAsRoleViewStyle(GameProxy.instance.AccountName, ClickCloseHandler);

			roleTypeIconImage.SetSprite( UIUtil.GetRoleTypeSmallIconSprite(playerInfo.heroData.roleType));
            levelTitleText.text = Localization.Get("ui.player_view.level_title");
            currentLevelText.text = "15";
            currentMaxLevelText.text = "30";
            changeProfessionAndStrengthen.text = Localization.Get("ui.player_view.change_profession_and_strengthen");
            accountExpPercentText.text = string.Format(Localization.Get("ui.player_view.exp_percent_text"), 65);
            playerExpPercentText.text = string.Format(Localization.Get("ui.player_view.profession_exp_percent"), 76);
            playerNameAndLevelText.text = string.Format(Localization.Get("ui.player_view.profession_name_and_level"), Localization.Get(playerInfo.heroData.name), 16, 30);
            hpTitleText.text = Localization.Get("ui.player_view.hp_title");
            offenceTitleText.text = Localization.Get("ui.player_view.offence_title");
            defenceTitleText.text = Localization.Get("ui.player_view.defence_title");
            actionPointTitleText.text = Localization.Get("ui.player_view.action_point_title");

            activeSkillTitleText.text = Localization.Get("ui.player_view.active_skill_title");
            passiveSkillTitleText.text = Localization.Get("ui.player_view.passive_skill_title");
            ultimateSummonText.text = Localization.Get("ui.player_view.ultimate_summon");
            summonDetialText.text = Localization.Get("ui.player_view.summon_detial");

            RefreshAccountLevelAndExp();
            RefreshPlayerLevelAndAttributes();
            RefreshPlayerModel();
            RefreshEquipments();
            RefreshSkill();
        }
        private void RefreshSkill()
        {
            PlayerInfo playerInfo = GameProxy.instance.PlayerInfo;

            SkillData skillData1 = SkillData.GetSkillDataById(playerInfo.heroData.skillId1);
            if (skillData1 != null)
            {
                skillDesButton[0].gameObject.SetActive(true);
				skillDesButton[0].SetSkillInfo(playerInfo.heroData.skillId1,0,0);
				skillDesButton[0].GetComponent<Image>().SetSprite(ResMgr.instance.Load<Sprite>(ResPath.GetSkillIconPath(skillData1.skillIcon)));
            }
            else
            {
                skillDesButton[0].gameObject.SetActive(false);
            }
            SkillData skillData2 = SkillData.GetSkillDataById(playerInfo.heroData.skillId2);
            if (skillData2 != null)
            {
                skillDesButton[1].gameObject.SetActive(true);
				skillDesButton[1].SetSkillInfo(playerInfo.heroData.skillId2,0,0);
				skillDesButton[1].GetComponent<Image>().SetSprite(ResMgr.instance.Load<Sprite>(ResPath.GetSkillIconPath(skillData2.skillIcon)));
            }
            else
            {
                skillDesButton[1].gameObject.SetActive(false);
            }
            SkillData passiveSkillData = SkillData.GetSkillDataById(playerInfo.heroData.passiveId1);
            if (passiveSkillData != null)
            {
                skillDesButton[2].gameObject.SetActive(true);
				skillDesButton[2].SetSkillInfo(playerInfo.heroData.passiveId1,0,0);
				skillDesButton[2].GetComponent<Image>().SetSprite(ResMgr.instance.Load<Sprite>(ResPath.GetSkillIconPath(passiveSkillData.skillIcon)));
            }
            else
            {
                skillDesButton[2].gameObject.SetActive(false);
            }
            SkillData summonData = SkillData.GetSkillDataById(playerInfo.playerData.summonSkillId);
            if (summonData != null)
            {
                skillDesButton[3].gameObject.SetActive(true);
				skillDesButton[3].SetSkillInfo(playerInfo.playerData.summonSkillId,0,0);
				skillDesButton[3].GetComponent<Image>().SetSprite( ResMgr.instance.Load<Sprite>(ResPath.GetSkillIconPath(summonData.skillIcon)));
            }
            else
            {
                skillDesButton[3].gameObject.SetActive(false);
            }
        }
        private void RefreshAccountLevelAndExp()
        {
            int accountLevel = GameProxy.instance.AccountLevel;
            float accountExpPercentToNextLevel = AccountUtil.GetAccountExpPercentToNextLevel(accountLevel);
            currentLevelText.text = accountLevel.ToString();
            currentMaxLevelText.text = GlobalData.GetGlobalData().accountLevelMax.ToString();
            accountExpSlider.value = accountExpPercentToNextLevel;
            accountExpPercentText.text = string.Format(Localization.Get("common.percent"), (int)(accountExpPercentToNextLevel * 100));
        }

        private void RefreshPlayerLevelAndAttributes()
        {
            PlayerInfo playerInfo = GameProxy.instance.PlayerInfo;
            int playerLevel = playerInfo.level;
            int playerMaxLevel = GlobalData.GetGlobalData().playerLevelMax;
            float playerExpPercentToNextLevel = PlayerUtil.GetPlayerExpPercentToNextLevel(playerInfo);
            playerExpSlider.value = playerExpPercentToNextLevel;
            playerExpPercentText.text = string.Format(Localization.Get("common.percent"), ((int)(playerExpPercentToNextLevel * 100)));
            playerNameAndLevelText.text = string.Format(Localization.Get("ui.player_view.profession_name_and_level"), Localization.Get(playerInfo.heroData.name), playerLevel, playerMaxLevel);

            Dictionary<RoleAttributeType, RoleAttribute> playerAttributeDictionary = PlayerUtil.CalcPlayerAttributesDic(playerInfo);
            Dictionary<RoleAttributeType, RoleAttribute> playerAddAttrByEquipDic = PlayerUtil.CalcPlayerAttributesDicByEquip(playerInfo);

            hpText.text = playerAttributeDictionary[RoleAttributeType.HP].ValueString;


            RoleAttackAttributeType roleAttackAttributeType = CharacterUtil.GetRoleAttackAttributeType(playerInfo.heroData.roleType);
            int offence = 0;
            int attackAdd = 0;
            if (roleAttackAttributeType == RoleAttackAttributeType.PhysicalAttack)
            {
                offence = (int)playerAttributeDictionary[RoleAttributeType.NormalAtk].value;
                attackAdd = GetRoleAttrValue(playerAddAttrByEquipDic.GetValue(RoleAttributeType.NormalAtk));
            }
            else if (roleAttackAttributeType == RoleAttackAttributeType.MagicalAttack)
            {
                offence = (int)playerAttributeDictionary[RoleAttributeType.MagicAtk].value;
                attackAdd = GetRoleAttrValue(playerAddAttrByEquipDic.GetValue(RoleAttributeType.MagicAtk));
            }
            offenceText.text = offence.ToString();

            defenceText.text = playerAttributeDictionary[RoleAttributeType.Normal_Def].ValueString;
            speedText.text = playerAttributeDictionary[RoleAttributeType.Speed].ValueString;

            float crit = playerAttributeDictionary.ContainsKey(RoleAttributeType.Crit) ? playerAttributeDictionary[RoleAttributeType.Crit].value : 0;
            float dodge = playerAttributeDictionary.ContainsKey(RoleAttributeType.Dodge) ? playerAttributeDictionary[RoleAttributeType.Dodge].value : 0;
            float block = playerAttributeDictionary.ContainsKey(RoleAttributeType.Block) ? playerAttributeDictionary[RoleAttributeType.Block].value : 0;
            float hit = playerAttributeDictionary.ContainsKey(RoleAttributeType.Hit) ? playerAttributeDictionary[RoleAttributeType.Hit].value : 0;
            critText.text = string.Format(Localization.Get("common.percent"), crit);
            dodgeText.text = string.Format(Localization.Get("common.percent"), dodge);
            blockText.text = string.Format(Localization.Get("common.percent"), block);
            hitText.text = string.Format(Localization.Get("common.percent"), hit);

            hpAddText.text = playerAddAttrByEquipDic.ContainsKey(RoleAttributeType.HP) ? string.Format("(+{0})", playerAddAttrByEquipDic[RoleAttributeType.HP].ValueString) : string.Empty;
            attackAddText.text = attackAdd == 0 ? string.Empty : string.Format("(+{0})", attackAdd);
            defenceAddText.text = playerAddAttrByEquipDic.ContainsKey(RoleAttributeType.Normal_Def) ? string.Format("(+{0})", playerAddAttrByEquipDic[RoleAttributeType.Normal_Def].ValueString) : string.Empty;
            actionPointAddText.text = playerAddAttrByEquipDic.ContainsKey(RoleAttributeType.Speed) ? string.Format("(+{0})", playerAddAttrByEquipDic[RoleAttributeType.Speed].ValueString) : string.Empty;
            float critAdd = playerAddAttrByEquipDic.ContainsKey(RoleAttributeType.Crit) ? playerAddAttrByEquipDic[RoleAttributeType.Crit].value : 0;
            float dodgeAdd = playerAddAttrByEquipDic.ContainsKey(RoleAttributeType.Dodge) ? playerAddAttrByEquipDic[RoleAttributeType.Dodge].value : 0;
            float blockAdd = playerAddAttrByEquipDic.ContainsKey(RoleAttributeType.Block) ? playerAddAttrByEquipDic[RoleAttributeType.Block].value : 0;
            float hitAdd = playerAddAttrByEquipDic.ContainsKey(RoleAttributeType.Hit) ? playerAddAttrByEquipDic[RoleAttributeType.Hit].value : 0;
            critAddText.text = critAdd == 0 ? string.Empty : string.Format(Localization.Get("common.percent"), string.Format("(+{0})", critAdd));
            dodgeAddText.text = dodgeAdd == 0 ? string.Empty : string.Format(Localization.Get("common.percent"), string.Format("(+{0})", dodgeAdd));
            blockAddText.text = blockAdd == 0 ? string.Empty : string.Format(Localization.Get("common.percent"), string.Format("(+{0})", blockAdd));
            hitAddText.text = hitAdd == 0 ? string.Empty : string.Format(Localization.Get("common.percent"), string.Format("(+{0})", hitAdd));
        }
        private int GetRoleAttrValue(RoleAttribute attr)
        {
            if (attr == null)
                return 0;
            return (int)attr.value;
        }

        private void DespawnCharacter()
        {
            if (_characterEntity)
                Pool.Controller.PoolController.instance.Despawn(_characterEntity.name, _characterEntity);
            _characterEntity = null;
        }

        private void RefreshPlayerModel()
        {
            DespawnCharacter();
            PlayerInfo playerInfo = GameProxy.instance.PlayerInfo;
            TransformUtil.ClearChildren(player_model_root_transform, true);
            _characterEntity = CharacterEntity.CreatePlayerEntityAsUIElement(playerInfo, player_model_root_transform, false, true);
        }

        private void RefreshEquipments()
        {
            PlayerInfo playerInfo = GameProxy.instance.PlayerInfo;

            weaponIcon.gameObject.SetActive(false);
            armorIcon.gameObject.SetActive(false);
            accessoryIcon.gameObject.SetActive(false);

            if (playerInfo.weaponID != 0)
            {
                EquipmentInfo equipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(playerInfo.weaponID);
                weaponIcon.SetEquipmentInfo(equipmentInfo);
                weaponIcon.gameObject.SetActive(true);
            }

            if (playerInfo.armorID != 0)
            {
                EquipmentInfo equipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(playerInfo.armorID);
                armorIcon.SetEquipmentInfo(equipmentInfo);
                armorIcon.gameObject.SetActive(true);
            }

            if (playerInfo.accessoryID != 0)
            {
                EquipmentInfo equipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(playerInfo.accessoryID);
                accessoryIcon.SetEquipmentInfo(equipmentInfo);
                accessoryIcon.gameObject.SetActive(true);
            }
        }

        #region proxy callback handler
        private void OnAccountInfoUpdateHandler()
        {
            RefreshAccountLevelAndExp();
        }

        private void OnPlayerInfoUpdateHandler()
        {
            RefreshPlayerLevelAndAttributes();
            RefreshPlayerModel();
            RefreshEquipments();
            RefreshSkill();
        }

        private void OnEquipmentInfoListUpdateHandler()
        {
            RefreshEquipments();
        }
        #endregion

        #region UI event handlers
        public void ClickCloseHandler()
        {
            UIMgr.instance.Close(PREFAB_PATH);
        }

        public void ClickChangeProfession()
        {
            if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(Logic.Enums.FunctionOpenType.Player_Change_Profession, true))
            {
                return;
            }
            UIMgr.instance.Open(Logic.UI.ChangeProfession.View.ChangeProfessionView.PREFAB_PATH);
        }

        public void ClickSummonBeast()
        {
//            if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(Logic.Enums.FunctionOpenType.Player_Summon_Beast, true))
//            {
//                return;
//            }
        }

        public void ClickShowHideSkillsPanel()
        {
            if (_isSkillsPanelShowing)
            {
                LeanTween.moveLocalY(skillsPanelRootGameObject, skillPanelHidePosTransform.localPosition.y, 0.2f);
                skillsPanelShowHideArrow.transform.localRotation = Quaternion.Euler(0, 0, 180);
            }
            else
            {
                LeanTween.moveLocalY(skillsPanelRootGameObject, skillPanelShowPosTransform.localPosition.y, 0.2f);
                skillsPanelShowHideArrow.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            _isSkillsPanelShowing = !_isSkillsPanelShowing;
        }

        public void ClickWeaponHandler()
        {
            if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.MainView_Equipment, true))
            {
                return;
            }
            RoleEquipmentsView roleEquipmentsView = UIMgr.instance.Open<RoleEquipmentsView>(RoleEquipmentsView.PREFAB_PATH);
            roleEquipmentsView.SetPlayerInfo(GameProxy.instance.PlayerInfo);
            RoleAttackAttributeType roleAttackAttributeType = CharacterUtil.GetRoleAttackAttributeType(GameProxy.instance.PlayerInfo.heroData.roleType);
            if (roleAttackAttributeType == RoleAttackAttributeType.PhysicalAttack)
            {
                roleEquipmentsView.SetCurrentSelectEquipmentType(EquipmentType.PhysicalWeapon);
            }
            else if (roleAttackAttributeType == RoleAttackAttributeType.MagicalAttack)
            {
                roleEquipmentsView.SetCurrentSelectEquipmentType(EquipmentType.MagicalWeapon);
            }
        }

        public void ClickArmorHandler()
        {
            if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.MainView_Equipment, true))
            {
                return;
            }
            RoleEquipmentsView roleEquipmentsView = UIMgr.instance.Open<RoleEquipmentsView>(RoleEquipmentsView.PREFAB_PATH);
            roleEquipmentsView.SetPlayerInfo(GameProxy.instance.PlayerInfo);
            roleEquipmentsView.SetCurrentSelectEquipmentType(EquipmentType.Armor);
        }

        public void ClickAccessoryHandler()
        {
            if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.MainView_Equipment, true))
            {
                return;
            }
            RoleEquipmentsView roleEquipmentsView = UIMgr.instance.Open<RoleEquipmentsView>(RoleEquipmentsView.PREFAB_PATH);
            roleEquipmentsView.SetPlayerInfo(GameProxy.instance.PlayerInfo);
            roleEquipmentsView.SetCurrentSelectEquipmentType(EquipmentType.Accessory);
        }
        #endregion
    }
}
