using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Util;
using Common.Localization;
using Logic.Character;
using Logic.Game.Model;
using Logic.Player.Model;
using Logic.Player.Controller;
using Logic.Player;
using Logic.Enums;
using Logic.Hero.Model;
using Common.ResMgr;
using Logic.Skill.Model;
using Logic.UI.CommonTopBar.View;
using Logic.UI.Description.View;

namespace Logic.UI.ChangeProfession.View
{
    public class ChangeProfessionView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/change_profession/change_profession_view";

        private PlayerData _selectedPlayerData;
        private ProfessionButton _selectedProfessionButton;
        private CharacterEntity _characterEntity;

        #region UI components
        public GameObject core;
        private CommonTopBarView _commonTopBarView;

        public Text changeProfessionText;
        public Button changeProfessionButton;
        public Text professionNameText;
        public Text playerLevelText;
        public Text playerLevelMaxText;
        public Slider playerExpSlider;
        public Text playerExpPercentText;
        public Text professionStrenthText;

        public Text hpTitleText;
        public Text attackTitleText;
        public Text defenceTitleText;
        public Text speedTitleText;
        public Text critTitleText;
        public Text dodgeTitleText;
        public Text blockTitleText;
        public Text hitTitleText;

        public Text hpText;
        public Text attackText;
        public Text defenceText;
        public Text speedText;
        public Text critText;
        public Text dodgeText;
        public Text blockText;
        public Text hitText;

        public Text activeSkillsTitleText;
        public Text passiveSkillsTitleText;

        public Text selectProfessionTitleText;

        public Transform professionButtonsRootTransform;
        public ProfessionButton professionButtonPrefab;
        public GameObject selectedProfessionGlowGameObject;

        private bool _isSkillsPanelShowing = true;
        public GameObject skillsPanelShowHideArrow;
        public GameObject skillsPanelRootGameObject;
        public GameObject strengthenBtnGO;
        public Transform skillPanelShowPosTransform;
        public Transform skillPanelHidePosTransform;
        public Transform roleModelRootTransform;
        public SkillDesButton[] skillDesButton;
        #endregion



        void Start()
        {
            Init();
            BindDelegate();
        }

        void OnDestroy()
        {
            DespawnCharacter();
            UnbindDelegate();
        }

        void Update()
        {
            if (_selectedProfessionButton != null)
            {
                RefreshSelectedProfessionButtonIndicator();
            }
        }

        private void BindDelegate()
        {
            PlayerProxy.instance.onPlayerInfoUpdateDelegate += OnPlayerInfoUpdateHandler;
        }

        private void UnbindDelegate()
        {
            PlayerProxy.instance.onPlayerInfoUpdateDelegate -= OnPlayerInfoUpdateHandler;
        }

        private void Init()
        {
            string title = Localization.Get("ui.change_profession_view.change_profession_title");
            _commonTopBarView = CommonTopBarView.CreateNewAndAttachTo(core.transform);
            _commonTopBarView.SetAsCommonStyle(title, ClickBackHandler, true, true, true, true);

            changeProfessionText.text = Localization.Get("ui.change_profession_view.change_profession");
            professionStrenthText.text = Localization.Get("ui.change_profession_view.profession_strenthen");
            hpTitleText.text = Localization.Get("ui.change_profession_view.health_point");
            attackTitleText.text = Localization.Get("ui.change_profession_view.attack");
            defenceTitleText.text = Localization.Get("ui.change_profession_view.defense");
            speedTitleText.text = Localization.Get("ui.change_profession_view.speed");
            critTitleText.text = Localization.Get("ui.change_profession_view.critial");
            dodgeTitleText.text = Localization.Get("ui.change_profession_view.dodge");
            blockTitleText.text = Localization.Get("ui.change_profession_view.block");
            hitTitleText.text = Localization.Get("ui.change_profession_view.hit");
            activeSkillsTitleText.text = Localization.Get("ui.change_profession_view.active_skills");
            passiveSkillsTitleText.text = Localization.Get("ui.change_profession_view.passive_skills");
            selectProfessionTitleText.text = Localization.Get("ui.change_profession_view.select_profession_title");

            _selectedPlayerData = GameProxy.instance.PlayerInfo.playerData;
            RegenerateProfessionButtons();
            Refresh();
        }

        private void RegenerateProfessionButtons()
        {
            ProfessionButton[] professionButtons = professionButtonsRootTransform.GetComponentsInChildren<ProfessionButton>();
            int professionButtonCount = professionButtons.Length;
            for (int i = 0; i < professionButtonCount; i++)
            {
                if (professionButtons[i] != professionButtonPrefab)
                {
                    DestroyImmediate(professionButtons[i].gameObject);
                }
            }

            professionButtonPrefab.gameObject.SetActive(true);
            ProfessionButton currentPlayerProfessionButton = GameObject.Instantiate<ProfessionButton>(professionButtonPrefab);
            currentPlayerProfessionButton.SetPlayerData(GameProxy.instance.PlayerInfo.playerData);
            currentPlayerProfessionButton.transform.SetParent(professionButtonsRootTransform, false);

            List<PlayerData> changeProfessionPlayerList = PlayerData.GetChangeProfessionPlayerDataList(GameProxy.instance.PlayerInfo.playerData);
            int changeProfessionPlayerListCount = changeProfessionPlayerList.Count;
            for (int i = 0; i < changeProfessionPlayerListCount; i++)
            {
                ProfessionButton professionButton = GameObject.Instantiate<ProfessionButton>(professionButtonPrefab);
                professionButton.SetPlayerData(changeProfessionPlayerList[i]);
                professionButton.transform.SetParent(professionButtonsRootTransform, false);
            }
            professionButtonPrefab.gameObject.SetActive(false);
            professionButtonsRootTransform.localPosition = new Vector3(0, professionButtonsRootTransform.localPosition.y, professionButtonsRootTransform.localPosition.z);

            _selectedProfessionButton = currentPlayerProfessionButton;
        }

        private void ReorderProfessionButtons()
        {
            _selectedProfessionButton.transform.SetAsFirstSibling();
        }

        private void RefreshSelectedProfessionButtonIndicator()
        {
            selectedProfessionGlowGameObject.transform.position = _selectedProfessionButton.transform.position;
            selectedProfessionGlowGameObject.transform.SetAsLastSibling();
        }

        private void RefreshPlayerLevelAndAttributes(PlayerInfo playerInfo)
        {
            int playerLevel = playerInfo.level;
            int playerMaxLevel = GlobalData.GetGlobalData().playerLevelMax;
            float playerExpPercentToNextLevel = PlayerUtil.GetPlayerExpPercentToNextLevel(playerInfo);
            playerLevelText.text = playerLevel.ToString();
            playerLevelMaxText.text = playerMaxLevel.ToString(); ;
            playerExpSlider.value = playerExpPercentToNextLevel;
            playerExpPercentText.text = string.Format(Localization.Get("common.percent"), ((int)(playerExpPercentToNextLevel * 100)));

            Dictionary<RoleAttributeType, RoleAttribute> playerAttributeDictionary = PlayerUtil.CalcPlayerAttributesDic(playerInfo);
            hpText.text = playerAttributeDictionary[RoleAttributeType.HP].ValueString;

            RoleAttackAttributeType roleAttackAttributeType = CharacterUtil.GetRoleAttackAttributeType(playerInfo.heroData.roleType);
            int offence = 0;
            if (roleAttackAttributeType == RoleAttackAttributeType.PhysicalAttack)
            {
                offence = (int)playerAttributeDictionary[RoleAttributeType.NormalAtk].value;
            }
            else if (roleAttackAttributeType == RoleAttackAttributeType.MagicalAttack)
            {
                offence = (int)playerAttributeDictionary[RoleAttributeType.MagicAtk].value;
            }
            attackText.text = offence.ToString();

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
        }

        private void DespawnCharacter()
        {
            if (_characterEntity)
                Pool.Controller.PoolController.instance.Despawn(_characterEntity.name, _characterEntity);
            _characterEntity = null;
        }

        private void RefreshPlayerModel(PlayerInfo playerInfo)
        {
            DespawnCharacter();
            TransformUtil.ClearChildren(roleModelRootTransform, true);
            professionNameText.text = Localization.Get(playerInfo.heroData.name);
            _characterEntity = CharacterEntity.CreatePlayerEntityAsUIElement(playerInfo, roleModelRootTransform, false, true);
        }

        private void Refresh()
        {
            PlayerInfo selectedPlayerInfo = null;
            if (PlayerProxy.instance.IsPlayerUnlocked((int)_selectedPlayerData.Id))
            {
                selectedPlayerInfo = PlayerProxy.instance.GetPlayerDataCorrespondingPlayerInfo((int)_selectedPlayerData.Id);
                changeProfessionText.text = Localization.Get("ui.change_profession_view.change_profession");
                //				strengthenBtnGO.SetActive(true);
            }
            else
            {
                //				strengthenBtnGO.SetActive(false);
                PlayerInfo playerInfo = GameProxy.instance.PlayerInfo;
                uint mockPlayerInstanceID = 0;
                selectedPlayerInfo = new PlayerInfo(mockPlayerInstanceID, _selectedPlayerData.Id, playerInfo.hairCutIndex, playerInfo.hairColorIndex, playerInfo.faceIndex, playerInfo.skinIndex, GameProxy.instance.AccountName);
                changeProfessionText.text = Localization.Get("ui.change_profession_view.transfer_profession");
            }


            bool shouldShowChangeProfessionButton = _selectedPlayerData.Id != GameProxy.instance.PlayerInfo.playerData.Id;
            changeProfessionButton.gameObject.SetActive(shouldShowChangeProfessionButton);

            RefreshPlayerLevelAndAttributes(selectedPlayerInfo);
            RefreshPlayerModel(selectedPlayerInfo);
            RefreshSkill(selectedPlayerInfo);
        }
        private void RefreshSkill(PlayerInfo playerInfo)
        {

            SkillData skillData1 = SkillData.GetSkillDataById(playerInfo.heroData.skillId1);
            if (skillData1 != null)
            {
                skillDesButton[0].gameObject.SetActive(true);
                skillDesButton[0].SetSkillInfo(playerInfo.heroData.skillId1, playerInfo.advanceLevel, (int)playerInfo.heroData.starMin);
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
                skillDesButton[1].SetSkillInfo(playerInfo.heroData.skillId2, playerInfo.advanceLevel, (int)playerInfo.heroData.starMin);
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
                skillDesButton[2].SetSkillInfo(playerInfo.heroData.passiveId1, playerInfo.advanceLevel, (int)playerInfo.heroData.starMin);
				skillDesButton[2].GetComponent<Image>().SetSprite(ResMgr.instance.Load<Sprite>(ResPath.GetSkillIconPath(passiveSkillData.skillIcon)));
            }
            else
            {
                skillDesButton[2].gameObject.SetActive(false);
            }
        }
        private void OnPlayerInfoUpdateHandler()
        {
            RefreshPlayerModel(GameProxy.instance.PlayerInfo);
            RegenerateProfessionButtons();
            Refresh();
        }

        #region UI event handlers
        public void ClickBackHandler()
        {
            UIMgr.instance.Close(PREFAB_PATH);
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

        public void ClickProfessionButton(ProfessionButton professionButton)
        {
            _selectedPlayerData = professionButton.PlayerData;
            _selectedProfessionButton = professionButton;
            RefreshSelectedProfessionButtonIndicator();
            Refresh();
            //			if(!PlayerProxy.instance.IsPlayerUnlocked((int)_selectedPlayerData.Id))
            //			{
            //				ActivateProfessionView activateProfessionView = UIMgr.instance.Open<ActivateProfessionView>(ActivateProfessionView.PREFAB_PATH);
            //				activateProfessionView.SetPlayerData(_selectedPlayerData);
            //			}
        }

        public void ClickChangeProfessionButtonHandler()
        {
            if (PlayerProxy.instance.IsPlayerUnlocked((int)_selectedPlayerData.Id))
            {
                PlayerInfo playerInfo = PlayerProxy.instance.GetPlayerDataCorrespondingPlayerInfo((int)_selectedPlayerData.Id);
                PlayerController.instance.CLIENT2LOBBY_PLAYER_CHANGE_REQ((int)playerInfo.instanceID);
            }
            else
            {
                ActivateProfessionView.Open(_selectedPlayerData);
            }
        }
        public void ClickChangeProfessionStrengthenButtonHandler()
        {
            if (PlayerProxy.instance.IsPlayerUnlocked((int)_selectedPlayerData.Id))
            {
                PlayerInfo selectedPlayerInfo = PlayerProxy.instance.GetPlayerDataCorrespondingPlayerInfo((int)_selectedPlayerData.Id);
                Logic.UI.HeroStrengthen.View.HeroStrengthenView.Open(selectedPlayerInfo);
            }

        }
        #endregion
    }
}