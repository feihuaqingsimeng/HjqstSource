using UnityEngine;
using UnityEngine.UI;
using Common.Util;
using System.Collections.Generic;
using Logic.UI.CommonTopBar.View;
using Logic.Player.Model;
using Logic.Game.Model;
using Logic.Character;
using Common.Animators;
using Logic.Skill.Model;
using Common.ResMgr;
using Logic.UI.Equipments.View;
using Logic.Equipment.Model;
using Logic.Role;
using Logic.Enums;
using Logic.UI.Role.View;
using Common.Localization;
using Logic.Hero.Model;
using Logic.Game;
using Logic.Player;
using Common.UI.Components;
using Logic.Item.Controller;
using Logic.UI.Description.View;
using Logic.UI.CommonEquipment.View;
using LuaInterface;
using Logic.Skill;
using Logic.FunctionOpen.Model;

namespace Logic.UI.Player.View
{
    public class PlayerInfoView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/player/player_info_view";

        private PlayerInfo _playerInfo;
        private CharacterEntity _characterEntity;

        #region UI components
        public GameObject core;
        private CommonTopBarView _commonTopBarView;

        public Transform modelRootTransform;
		public GameObject playerLevelUpFXGameObject;
		public GameObject playerTalentBtnGameObject;
        public Text accountNameText;
        public Slider accountExpSlider;
        public Text accountExpText;
        public Text accountLevelText;

        public Image playerRoleTypeIconImage;
        public Text playerNameText;
        public Slider playerExpSlider;
        public Text playerExpText;
        public Text playerLevelText;

        public Text changeProfessionText;
        public Text petStrengthenText;
        public Text petAdvancText;
        public Text petBreakthroughText;
        public Text playerEquipmentsText;

        public RoleAttributeItem hpItem;
        public RoleAttributeItem offenceItem;
        public RoleAttributeItem defenceItem;
        public RoleAttributeItem speedItem;
        public RoleAttributeItem criticalItem;
        public RoleAttributeItem dodgeItem;
        public RoleAttributeItem blockItem;
        public RoleAttributeItem hitItem;


        public List<SkillDesButton> skillDesButton;
		public List<Image> skillTypeIconImages;
        public RectTransform weaponEquipRoot;
        public RectTransform armorEquipRoot;
        public RectTransform accessoryEquipRoot;

        public MedicineAutoUseButton[] medicineAutoUseButton;
        #endregion UI components

        void Awake()
        {
			playerLevelUpFXGameObject.SetActive(false);

            GameProxy.instance.onAccountInfoUpdateDelegate += OnAccountInfoUpdateHandler;
            PlayerProxy.instance.onPlayerInfoUpdateDelegate += OnPlayerInfoUpdateHandler;
            EquipmentProxy.instance.onEquipmentInfoListUpdateDelegate += OnEquipmentInfoListUpdateHandler;
            PlayerTalentProxy.instance.UpdateAllTalentDelegate += RefreshSkillButtons;
            for (int i = 0, count = medicineAutoUseButton.Length; i < count; i++)
            {
                medicineAutoUseButton[i].onAutoAddExpDelegate += OnClickExpMedicineHandler;
                medicineAutoUseButton[i].onSendProtocolDelegate += OnSendProtocolByUseExpMedicineHandler;
				medicineAutoUseButton[i].onMedicineCompleteDelegate += UpdateMedicineComplete;
            }
        }

        void Start()
        {
            _commonTopBarView = CommonTopBarView.CreateNewAndAttachTo(core.transform);
            _commonTopBarView.SetAsCommonStyle(string.Empty, ClickCloseButtonHandler, false, true, true, true);

            _playerInfo = GameProxy.instance.PlayerInfo;

            changeProfessionText.text = Localization.Get("ui.player_info_view.change_profession");
            petStrengthenText.text = Localization.Get("ui.player_info_view.pet_strengthen");
            petAdvancText.text = Localization.Get("ui.player_info_view.player_advance");
            petBreakthroughText.text = Localization.Get("ui.player_info_view.player_breakthrough");
            playerEquipmentsText.text = Localization.Get("ui.player_info_view.player_equipments");

            hpItem.SetRoleAttributeValueType(RoleAttributeValueType.RealValue);
            offenceItem.SetRoleAttributeValueType(RoleAttributeValueType.RealValue);
            defenceItem.SetRoleAttributeValueType(RoleAttributeValueType.RealValue);
            speedItem.SetRoleAttributeValueType(RoleAttributeValueType.RealValue);
            criticalItem.SetRoleAttributeValueType(RoleAttributeValueType.Percent);
            dodgeItem.SetRoleAttributeValueType(RoleAttributeValueType.Percent);
            blockItem.SetRoleAttributeValueType(RoleAttributeValueType.Percent);
            hitItem.SetRoleAttributeValueType(RoleAttributeValueType.Percent);

            hpItem.SetTitle(Localization.Get("ui.role_info_view.hp_title"));
            offenceItem.SetTitle(Localization.Get("ui.role_info_view.offence_title"));
            defenceItem.SetTitle(Localization.Get("ui.role_info_view.defence_title"));
            speedItem.SetTitle(Localization.Get("ui.role_info_view.speed_title"));
            criticalItem.SetTitle(Localization.Get("ui.role_info_view.critical_title"));
            dodgeItem.SetTitle(Localization.Get("ui.role_info_view.dodge_title"));
            blockItem.SetTitle(Localization.Get("ui.role_info_view.block_title"));
            hitItem.SetTitle(Localization.Get("ui.role_info_view.hit_title"));

            RefreshModel();
            RefreshAccountInfo();
            RefreshPlayerInfo();
            RefreshAttributes();
            RefreshEquipments();
            RefreshSkillButtons();
        }

        void OnDestroy()
        {
            DespawnCharacter();
            GameProxy.instance.onAccountInfoUpdateDelegate -= OnAccountInfoUpdateHandler;
            PlayerProxy.instance.onPlayerInfoUpdateDelegate -= OnPlayerInfoUpdateHandler;
            EquipmentProxy.instance.onEquipmentInfoListUpdateDelegate -= OnEquipmentInfoListUpdateHandler;
            PlayerTalentProxy.instance.UpdateAllTalentDelegate -= RefreshSkillButtons;

            for (int i = 0, count = medicineAutoUseButton.Length; i < count; i++)
            {
                medicineAutoUseButton[i].onAutoAddExpDelegate -= OnClickExpMedicineHandler;
                medicineAutoUseButton[i].onSendProtocolDelegate -= OnSendProtocolByUseExpMedicineHandler;
				medicineAutoUseButton[i].onMedicineCompleteDelegate -= UpdateMedicineComplete;
				
            }
        }

        private void DespawnCharacter()
        {
            if (_characterEntity)
                Pool.Controller.PoolController.instance.Despawn(_characterEntity.name, _characterEntity);
            _characterEntity = null;
        }

        private void RefreshModel()
        {
            DespawnCharacter();
            TransformUtil.ClearChildren(modelRootTransform, true);
            _characterEntity = CharacterEntity.CreatePlayerEntityAsUIElement(_playerInfo, modelRootTransform, true, true);
            Action.Controller.ActionController.instance.PlayerAnimAction(_characterEntity, AnimatorUtil.VICOTRY_ID);
        }

        private void RefreshAccountInfo()
        {
            int accountLevel = GameProxy.instance.AccountLevel;
            float accountExpPercentToNextLevel = AccountUtil.GetAccountExpPercentToNextLevel(accountLevel);

//            accountNameText.text = GameProxy.instance.PlayerInfo.name;
			accountNameText.text = GameProxy.instance.AccountName;
            accountExpSlider.value = accountExpPercentToNextLevel;
            accountExpText.text = string.Format(Localization.Get("common.percent"), (int)(accountExpPercentToNextLevel * 100));
            accountLevelText.text = string.Format(Localization.Get("ui.player_info_view.change_profession.account_level"), accountLevel);
        }

        private void RefreshPlayerInfo()
        {
            int playerLevel = _playerInfo.level;
            int playerMaxLevel = _playerInfo.MaxLevel;
            float playerExpPercentToNextLevel = PlayerUtil.GetPlayerExpPercentToNextLevel(_playerInfo);

			playerRoleTypeIconImage.SetSprite(UIUtil.GetRoleTypeBigIconSprite(_playerInfo.heroData.roleType));
            playerNameText.text = Localization.Get(_playerInfo.heroData.name);
            playerExpSlider.value = playerExpPercentToNextLevel;
            playerExpText.text = string.Format(Localization.Get("common.percent"), (int)(playerExpPercentToNextLevel * 100));
            playerLevelText.text = string.Format("LV {0}/{1}", playerLevel, playerMaxLevel);

			playerTalentBtnGameObject.SetActive(_playerInfo.playerData.pet_id != 1);
			
        }

        private void RefreshAttributes()
        {
            Dictionary<RoleAttributeType, RoleAttribute> roleAttributesDic = RoleUtil.CalcRoleAttributesDic(_playerInfo);
            Dictionary<RoleAttributeType, RoleAttribute> roleAttributeAddByEquipmentDic = RoleUtil.CalcRoleAttributesDicByEquip(_playerInfo);
            RoleAttackAttributeType roleAttackAttributeType = CharacterUtil.GetRoleAttackAttributeType(_playerInfo.heroData.roleType);

            int hpValue = (int)roleAttributesDic[RoleAttributeType.HP].value;
            int offenceValue = 0;
            int defenceValue = 0;
            int speedValue = (int)roleAttributesDic[RoleAttributeType.Speed].value;
            int criticalValue = roleAttributesDic.ContainsKey(RoleAttributeType.Crit) ? (int)roleAttributesDic[RoleAttributeType.Crit].value : 0;
            int dodgeValue = roleAttributesDic.ContainsKey(RoleAttributeType.Dodge) ? (int)roleAttributesDic[RoleAttributeType.Dodge].value : 0;
            int blockValue = roleAttributesDic.ContainsKey(RoleAttributeType.Block) ? (int)roleAttributesDic[RoleAttributeType.Block].value : 0;
            int hitValue = roleAttributesDic.ContainsKey(RoleAttributeType.Hit) ? (int)roleAttributesDic[RoleAttributeType.Hit].value : 0;

            int hpAddValue = roleAttributeAddByEquipmentDic.ContainsKey(RoleAttributeType.HP) ? (int)roleAttributeAddByEquipmentDic[RoleAttributeType.HP].value : 0;
            int offenceAddValue = 0;
            int defenceAddValue = 0;
            int speedAddValue = roleAttributeAddByEquipmentDic.ContainsKey(RoleAttributeType.Speed) ? (int)roleAttributeAddByEquipmentDic[RoleAttributeType.Speed].value : 0;
            int criticalAddValue = roleAttributeAddByEquipmentDic.ContainsKey(RoleAttributeType.Crit) ? (int)roleAttributeAddByEquipmentDic[RoleAttributeType.Crit].value : 0;
            int dodgeAddValue = roleAttributeAddByEquipmentDic.ContainsKey(RoleAttributeType.Dodge) ? (int)roleAttributeAddByEquipmentDic[RoleAttributeType.Dodge].value : 0;
            int blockAddValue = roleAttributeAddByEquipmentDic.ContainsKey(RoleAttributeType.Block) ? (int)roleAttributeAddByEquipmentDic[RoleAttributeType.Block].value : 0;
            int hitAddValue = roleAttributeAddByEquipmentDic.ContainsKey(RoleAttributeType.Hit) ? (int)roleAttributeAddByEquipmentDic[RoleAttributeType.Hit].value : 0;

            if (roleAttackAttributeType == RoleAttackAttributeType.PhysicalAttack)
            {
                offenceValue = (int)roleAttributesDic[RoleAttributeType.NormalAtk].value;
                offenceAddValue = roleAttributeAddByEquipmentDic.ContainsKey(RoleAttributeType.NormalAtk) ? (int)roleAttributeAddByEquipmentDic.GetValue(RoleAttributeType.NormalAtk).value : 0;
            }
            else if (roleAttackAttributeType == RoleAttackAttributeType.MagicalAttack)
            {
                offenceValue = (int)roleAttributesDic[RoleAttributeType.MagicAtk].value;
                offenceAddValue = roleAttributeAddByEquipmentDic.ContainsKey(RoleAttributeType.MagicAtk) ? (int)roleAttributeAddByEquipmentDic.GetValue(RoleAttributeType.MagicAtk).value : 0;
            }
			defenceValue = (int)roleAttributesDic[RoleAttributeType.Normal_Def].value;
			defenceAddValue = roleAttributeAddByEquipmentDic.ContainsKey(RoleAttributeType.Normal_Def) ? (int)roleAttributeAddByEquipmentDic.GetValue(RoleAttributeType.Normal_Def).value : 0;
            hpItem.SetValue(hpValue);
            hpItem.SetAddValue(hpAddValue);
            offenceItem.SetValue(offenceValue);
            offenceItem.SetAddValue(offenceAddValue);
            defenceItem.SetValue(defenceValue);
            defenceItem.SetAddValue(defenceAddValue);
            speedItem.SetValue(speedValue);
            speedItem.SetAddValue(speedAddValue);
            criticalItem.SetValue(criticalValue);
            criticalItem.SetAddValue(criticalAddValue);
            dodgeItem.SetValue(dodgeValue);
            dodgeItem.SetAddValue(dodgeAddValue);
            blockItem.SetValue(blockValue);
            blockItem.SetAddValue(blockAddValue);
            hitItem.SetValue(hitValue);
            hitItem.SetAddValue(hitAddValue);
        }

        private void RefreshSkillButtons()
        {
            SkillData skillData1 = SkillData.GetSkillDataById(_playerInfo.heroData.skillId1);
            if (skillData1 != null)
            {
                skillDesButton[0].SetSkillInfo(_playerInfo.heroData.skillId1,0,0);
				skillDesButton[0].GetComponent<Image>().SetSprite(ResMgr.instance.Load<Sprite>(ResPath.GetSkillIconPath(skillData1.skillIcon)));
				skillTypeIconImages[0].SetSprite(ResMgr.instance.LoadSprite(SkillUtil.GetDesTypeIcon(skillData1)));
				skillTypeIconImages[0].gameObject.SetActive(skillTypeIconImages[0].sprite != null);
            }
            else
            {
				skillDesButton[0].SetSkillInfo(null,0,0);
				skillDesButton[0].GetComponent<Image>().SetSprite(ResMgr.instance.Load<Sprite>(ResPath.GetSkillIconPath("icon_skill_none")));
				skillTypeIconImages[0].gameObject.SetActive(false);
            }
            SkillData skillData2 = SkillData.GetSkillDataById(_playerInfo.heroData.skillId2);
            if (skillData2 != null)
            {
				skillDesButton[1].SetSkillInfo(_playerInfo.heroData.skillId2,0,0);
				skillDesButton[1].GetComponent<Image>().SetSprite( ResMgr.instance.Load<Sprite>(ResPath.GetSkillIconPath(skillData2.skillIcon)));
				skillTypeIconImages[1].SetSprite(ResMgr.instance.LoadSprite(SkillUtil.GetDesTypeIcon(skillData2)));
				skillTypeIconImages[1].gameObject.SetActive(skillTypeIconImages[1].sprite != null);
            }
            else
            {
				skillDesButton[1].SetSkillInfo(null,0,0);
				skillDesButton[1].GetComponent<Image>().SetSprite( ResMgr.instance.Load<Sprite>(ResPath.GetSkillIconPath("icon_skill_none")));
				skillTypeIconImages[1].gameObject.SetActive(false);
            }
            if (_playerInfo.summonEffectId != 0)
            {
                PlayerSkillTalentInfo talentSkillInfo = PlayerTalentProxy.instance.GetCurPlayerSkillInfoByEffectId(_playerInfo.summonEffectId);

                //skillDesButton[2].SetTalenSkillInfo(talentSkillInfo);
                //skillDesButton[2].GetComponent<Image>().sprite = talentSkillInfo.skillIcon;
            }
            else
            {
                SkillData summonData = SkillData.GetSkillDataById(_playerInfo.playerData.summonSkillId);
                if (summonData != null)
                {
					skillDesButton[2].SetSkillInfo(_playerInfo.playerData.summonSkillId,0,0);
					skillDesButton[2].GetComponent<Image>().SetSprite(ResMgr.instance.Load<Sprite>(ResPath.GetSkillIconPath(summonData.skillIcon)));
                }
                else
                {
					skillDesButton[2].SetSkillInfo(null,0,0);
					skillDesButton[2].GetComponent<Image>().SetSprite(ResMgr.instance.Load<Sprite>(ResPath.GetSkillIconPath("icon_skill_none")));
                }
            }
            if (_playerInfo.passiveSkillId != 0)
            {
                PlayerSkillTalentInfo talentSkillInfo = PlayerTalentProxy.instance.GetCurPlayerSkillInfoByEffectId(_playerInfo.passiveSkillId);
                //skillDesButton[3].SetTalenSkillInfo(talentSkillInfo);
                //skillDesButton[3].GetComponent<Image>().sprite = talentSkillInfo.skillIcon;
            }
            else
            {
                SkillData passiveSkillData = SkillData.GetSkillDataById(_playerInfo.heroData.passiveId1);
                if (passiveSkillData != null)
                {
					skillDesButton[3].SetSkillInfo(_playerInfo.heroData.passiveId1,0,0);
					skillDesButton[3].GetComponent<Image>().SetSprite(ResMgr.instance.Load<Sprite>(ResPath.GetSkillIconPath(passiveSkillData.skillIcon)));
                }
                else
                {
					skillDesButton[3].SetSkillInfo(null,0,0);
					skillDesButton[3].GetComponent<Image>().SetSprite(ResMgr.instance.Load<Sprite>(ResPath.GetSkillIconPath("icon_skill_none")));
                }
            }

        }

        private void RefreshEquipments()
        {
            TransformUtil.ClearChildren(weaponEquipRoot, true);
            TransformUtil.ClearChildren(armorEquipRoot, true);
            TransformUtil.ClearChildren(accessoryEquipRoot, true);

            if (_playerInfo.weaponID != 0)
            {
                EquipmentInfo equipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(_playerInfo.weaponID);
                CommonEquipmentIcon icon = CommonEquipmentIcon.Create(weaponEquipRoot);
                icon.SetEquipmentInfo(equipmentInfo);
                icon.ButtonEnable(false);
                icon.GetEquipmentDesButton().enabled = false;
            }

            if (_playerInfo.armorID != 0)
            {
                EquipmentInfo equipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(_playerInfo.armorID);
                CommonEquipmentIcon icon = CommonEquipmentIcon.Create(armorEquipRoot);
                icon.SetEquipmentInfo(equipmentInfo);
                icon.ButtonEnable(false);
                icon.GetEquipmentDesButton().enabled = false;
            }

            if (_playerInfo.accessoryID != 0)
            {
                EquipmentInfo equipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(_playerInfo.accessoryID);
                CommonEquipmentIcon icon = CommonEquipmentIcon.Create(accessoryEquipRoot);
                icon.SetEquipmentInfo(equipmentInfo);
                icon.ButtonEnable(false);
                icon.GetEquipmentDesButton().enabled = false;
            }
        }

        #region proxy callback
        private void OnAccountInfoUpdateHandler()
        {
            RefreshAccountInfo();
        }

        private void OnPlayerInfoUpdateHandler()
        {
            _playerInfo = GameProxy.instance.PlayerInfo;
            RefreshModel();
            RefreshPlayerInfo();
            RefreshAttributes();
            RefreshSkillButtons();
            RefreshEquipments();
        }

        private void OnEquipmentInfoListUpdateHandler()
        {
            RefreshEquipments();
        }
        #endregion proxy callback

        #region exp potion
		//==========================以下是经验药水代码==================
		//经验药水
		private int _selectHeroLevel = 0;
		private int _selectHeroExp = 0;
		private int selectMaxLevel = 0;
        private bool OnClickExpMedicineHandler(int addExp)
        {
			if (_selectHeroLevel == 0)
			{
				_selectHeroLevel = _playerInfo.level;
				_selectHeroExp = _playerInfo.exp;
				selectMaxLevel = _playerInfo.MaxLevel;
			}
			if(_selectHeroLevel >= selectMaxLevel)
				return false;
			if(_selectHeroLevel >= GameProxy.instance.AccountLevel)
				return false;

			int totalExp = addExp + _selectHeroExp;
			int level = _selectHeroLevel;
            HeroExpData expData;
            while (true)
            {
                expData = HeroExpData.GetHeroExpDataByLv(level);
                if (expData == null)
                    return false;
                if (totalExp >= expData.exp)//升级
                {
                    level++;
                    totalExp -= expData.exp;
					if (level == selectMaxLevel)
                    {
                        totalExp = 0;
                        break;
                    }
                }
                else //经验没导致升级
                {
                    break;
                }
            }
			int oldLv = _selectHeroLevel;
			_selectHeroLevel = level;
            _selectHeroExp = totalExp;
            expData = HeroExpData.GetHeroExpDataByLv(level);
            if (expData != null)
                UpdateExpBar(level - oldLv + (totalExp + 0.01f) / expData.exp);
            if (oldLv != level)
			{
                RefreshAttributes();
				playerLevelText.text = string.Format("LV {0}/{1}", _selectHeroLevel, selectMaxLevel);
				playerLevelUpFXGameObject.SetActive(false);
				playerLevelUpFXGameObject.SetActive(true);
			}
            return true;
        }
        private void UpdateExpBar(float endRate)
        {
            int playerLevel = _playerInfo.level;
            int playerMaxLevel = _playerInfo.MaxLevel;
            
            SliderValueChangeAction.Get(playerExpSlider.gameObject).SetValueChangeUpdate(UpdateExpBarValueChange).StartValueChangeActionList(endRate, 0.15f);
        }
        private void UpdateExpBarValueChange(float t)
        {
            int percent = (int)((t - (int)t) * 100);
            playerExpText.text = string.Format(Localization.Get("common.percent"), percent);
        }
        private bool _isSendExpMedicineProtocol = false;
        private void OnSendProtocolByUseExpMedicineHandler(int itemid, int count)
        {
            _isSendExpMedicineProtocol = true;
            ItemController.instance.CLIENT2LOBBY_ExpPotion_REQ((int)_playerInfo.instanceID, itemid, count);
        }
		private void UpdateMedicineComplete()
		{
			_selectHeroExp = 0;
			_selectHeroLevel = 0;
		}

        private void StopExpMedicineAction()
        {
            SliderValueChangeAction.Get(playerExpSlider.gameObject).CancelAction();
        }
        #endregion exo potion

        #region UI event handlers
        public void ClickChangeProfessionButtonHandler()
        {
            if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.Player_Change_Profession, true))
            {
                return;
            }
            Logic.UI.ChangeProfession.View.ChangeProfessionView changeProfessionView = UIMgr.instance.Open<Logic.UI.ChangeProfession.View.ChangeProfessionView>(Logic.UI.ChangeProfession.View.ChangeProfessionView.PREFAB_PATH);
        }

        public void ClickPetStrengthenButtonHandler()
        {
            if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.HeroStrengthen, true))
            {
                return;
            }
            Logic.UI.HeroStrengthen.View.HeroStrengthenView.Open(GameProxy.instance.PlayerInfo);
        }
        public void ClickPetCultivateBtnHandler()
        {
            FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.PlayerTraining, null, false, true);
        }
        public void ClickPetAdvanceButtonHandler()
        {
            if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.HeroAdvance, true))
            {
                return;
            }

            if (_playerInfo.IsMaxAdvanceLevel)
            {
                Logic.UI.Tips.View.CommonAutoDestroyTipsView.Open(Localization.Get("ui.common_tips.hero_star_reach_max"));
                return;
            }

            Logic.UI.HeroAdvance.View.HeroAdvanceView heroAdvanceView = UIMgr.instance.Open<Logic.UI.HeroAdvance.View.HeroAdvanceView>(Logic.UI.HeroAdvance.View.HeroAdvanceView.PREFAB_PATH);
            heroAdvanceView.SetHeroInfo(null);
        }

        public void ClickPetBreakthroughButtonHandler()
        {
            if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.HeroBreakthrough, true))
            {
                return;
            }
//            Logic.UI.HeroBreakthrough.View.HeroBreakthroughView.Open(_playerInfo);
        }

        public void ClickPlayerEquipmentsButtonHandler()
        {
           
			FunctionOpenProxy.instance.OpenLuaView(FunctionOpenType.PlayerWearEquip,_playerInfo.instanceID,true, (int)RoleEquipPos.Weapon);

			//LuaTable equipModelLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","equip_model")[0];
			//equipModelLuaTable.GetLuaFunction("OpenRoleEquipView").Call(_playerInfo.instanceID, true, (int)RoleEquipPos.Weapon);
        }

        public void ClickWeaponButtonHandler()
        {
            if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.MainView_Equipment, true))
            {
                return;
            }
			LuaTable equipModelLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","equip_model")[0];
			equipModelLuaTable.GetLuaFunction("OpenRoleEquipView").Call(_playerInfo.instanceID, true, (int)RoleEquipPos.Weapon);
        }

        public void ClickArmorButtonHandler()
        {
            if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.MainView_Equipment, true))
            {
                return;
            }
			LuaTable equipModelLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","equip_model")[0];
			equipModelLuaTable.GetLuaFunction("OpenRoleEquipView").Call(_playerInfo.instanceID, true, (int)RoleEquipPos.Armor);
        }

        public void ClickAccessoryButtonHandler()
        {
            if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.MainView_Equipment, true))
            {
                return;
            }
			LuaTable equipModelLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","equip_model")[0];
			equipModelLuaTable.GetLuaFunction("OpenRoleEquipView").Call(_playerInfo.instanceID, true, (int)RoleEquipPos.Accessory);
        }

        public void ClickCloseButtonHandler()
        {
            UIMgr.instance.Close(PREFAB_PATH);
        }
        #endregion UI event handlers
    }
}