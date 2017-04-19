using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Util;
using Common.Localization;
using Logic.Enums;
using Logic.Game.Model;
using Logic.Hero.Model;
using Logic.UI.CommonTopBar.View;
using Logic.Player.Model;
using Logic.Character;
using Logic.Role.Model;
using Logic.UI.Tips.View;
using Logic.UI.Pvp.Model;
using Logic.UI.Pvp.Controller;
using Logic.Hero;
using Logic.UI.FightResult.View;
using Common.UI.Components;
using Logic.UI.TrainFormation.View;
using Logic.Formation.Model;
using Logic.UI.TrainFormation.Model;
using Logic.Role;
using Logic.FunctionOpen.Model;
using Common.ResMgr;
using Logic.UI.Description.View;
using LuaInterface;

namespace Logic.UI.Pvp.View
{
    public class PVPEmbattleView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/pvp/pvp_embattle_view";

        public static PVPEmbattleView Open(bool isReadyFight)
        {
            //PVPEmbattleView view = UIMgr.instance.Open<PVPEmbattleView>(PREFAB_PATH);
           // view.SetReadyFight(isReadyFight);
			LuaTable arenaCtrlLua = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl","arena_controller")[0];
			arenaCtrlLua.GetLuaFunction("OpenArenaEmbattleView").Call(isReadyFight);
            return null;
        }
        private List<HeroInfo> _cachedHeroInfoList;
        private bool _isReadyFight;
        private RoleInfo _selectedRoleInfo = null;
        private bool _isRightPanelShowing = true;

        private Logic.UI.CommonHeroIcon.View.CommonHeroIcon _selectedCommonHeroIcon = null;



        #region UI components
        public GameObject core;
        private CommonTopBarView _commonTopbarView;

        public Transform rightPanelShowPosTransform;
        public Transform rightPanelHidePosTransform;
        public Transform rightPanelTransform;

        public Text combatCapabilityText;
        public Text fightText;
        public Button fightButton;
        public Button removeRoleButton;
        public Text removeRoleText;
        public List<Button> formationBaseButtons;
        public List<GameObject> formationPositionIndicators;
        public List<Transform> roleModelRoots;
        public List<Image> roleShadows;

        public Transform heroButtonsRoot;
        public ScrollContentExpand scrollContent;

        public Image selectedFormationPositionIndicatorImage;
        public FormationButton _formationButton;

		public Image trainFormationLockImage;
        #endregion UI components

        private Sprite _formationNormalSprite;
        private Sprite _formationOccupiedSprite;
        private List<CharacterEntity> _characters = new List<CharacterEntity>();
        void Awake()
        {

            BindDelegate();
            _commonTopbarView = CommonTopBarView.CreateNewAndAttachTo(core.transform);
            _commonTopbarView.SetAsCommonStyle(Localization.Get("ui.pvp_formation_view.titleText"), ClickCloseHandler, false, false, true, true, true);

            removeRoleText.text = Localization.Get("ui.pvp_formation_view.remove_from_team");
            rightPanelTransform.position = rightPanelShowPosTransform.position;

            _formationNormalSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/bg_unit_02");
            _formationOccupiedSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/bg_unit_01");
			trainFormationLockImage.gameObject.SetActive(!FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.FormationTraining));
        }
        void Start()
        {
            GenerateHeroButtons();
            Refresh();
        }

        void OnDestroy()
        {
            DespawnCharacter();
            UnbindDelegate();
        }

        private void BindDelegate()
        {
            HeroProxy.instance.onHeroInfoListUpdateDelegate += OnHeroInfoListUpdateHandler;
            PlayerProxy.instance.onPlayerInfoUpdateDelegate += OnHeroInfoListUpdateHandler;
            TrainFormationProxy.instance.onFormationChangedDelegate += OnFormationUpdateHandler;
            TrainFormationProxy.instance.onUpgradeTrainFormationDelegate += FormationUpgradeHandler;

        }

        private void UnbindDelegate()
        {
            HeroProxy.instance.onHeroInfoListUpdateDelegate -= OnHeroInfoListUpdateHandler;
            PlayerProxy.instance.onPlayerInfoUpdateDelegate -= OnHeroInfoListUpdateHandler;
            TrainFormationProxy.instance.onFormationChangedDelegate -= OnFormationUpdateHandler;
            TrainFormationProxy.instance.onUpgradeTrainFormationDelegate -= FormationUpgradeHandler;
        }

        private void SetReadyFight(bool isReadyFight)
        {
            _isReadyFight = isReadyFight;
            fightButton.gameObject.SetActive(_isReadyFight);
        }

        private void GenerateHeroButtons()
        {
            _cachedHeroInfoList = HeroProxy.instance.GetAllHeroInfoList();
            _cachedHeroInfoList.Sort(RoleUtil.CompareRoleByQualityDesc);
            scrollContent.Init(_cachedHeroInfoList.Count);
        }

        private void DespawnCharacter()
        {
            int count = _characters.Count;
            for (int index = 0; index < count; index++)
            {
                CharacterEntity character = _characters[index];
                if (character)
                    Pool.Controller.PoolController.instance.Despawn(character.name, character);
            }
            _characters.Clear();
        }

        private void RegenerateInTeamHeroModels()
        {
            DespawnCharacter();
            for (int i = 0; i < 9; i++)
            {
                TransformUtil.ClearChildren(roleModelRoots[i], true);
                FormationPosition formationPosition = (FormationPosition)(i + 1);
                if (!PvpFormationProxy.instance.IsPositionEmpty(formationPosition))
                {
                    uint roleID = PvpFormationProxy.instance.GetCharacterInstanceIDAt(formationPosition);
                    if (GameProxy.instance.IsPlayer(roleID))
                    {
                        PlayerInfo playerInfo = GameProxy.instance.PlayerInfo;
                        CharacterEntity characterEntity = CharacterEntity.CreatePlayerEntityAsUIElement(playerInfo, roleModelRoots[i], false, false);
                        _characters.Add(characterEntity);
                    }
                    else
                    {
                        HeroInfo heroInfo = HeroProxy.instance.GetHeroInfo(roleID);
                        HeroEntity heroEntity = CharacterEntity.CreateHeroEntityAsUIElement(heroInfo, roleModelRoots[i], false, false);
                        _characters.Add(heroEntity);
                    }
                    roleShadows[i].gameObject.SetActive(true);
					formationBaseButtons[i].GetComponent<Image>().SetSprite(_formationOccupiedSprite);
                }
                else
                {
                    roleShadows[i].gameObject.SetActive(false);
					formationBaseButtons[i].GetComponent<Image>().SetSprite( PvpFormationProxy.instance.FormationTeamInfo.formationInfo.formationData.GetPosEnalbe(formationPosition) ? _formationOccupiedSprite : _formationNormalSprite);
                }
            }
        }
        //特定阵型
        private void RefreshTrainForamtionButton()
        {
            _formationButton.SetInfo(FormationProxy.instance.GetFormationInfo(PvpFormationProxy.instance.FormationTeamInfo.formationId), FormationState.NotInUse);
        }
        private void RefreshCombatCapability()
        {
            int combatCapability = PvpFormationProxy.instance.GetPower();
            NumberIncreaseAction numberAct = combatCapabilityText.GetComponent<NumberIncreaseAction>();
            combatCapabilityText.GetComponent<NumberIncreaseAction>().Init(numberAct.endNumber, combatCapability);
        }

        private void ShowFormationPositionIndicators()
        {
            int formationPositionIndicatorCount = formationPositionIndicators.Count;
            for (int i = 0; i < formationPositionIndicatorCount; i++)
            {
                FormationPosition formationPosition = (FormationPosition)(i + 1);
                bool isPositionEmpty = PvpFormationProxy.instance.CanAddToFormationPosition(formationPosition, _selectedRoleInfo.instanceID);
                formationPositionIndicators[i].SetActive(isPositionEmpty);
                formationPositionIndicators[i].transform.SetAsLastSibling();
            }
        }

        private void HideAllFormationPositionIndicators()
        {
            int formationPositionIndicatorCount = formationPositionIndicators.Count;
            for (int i = 0; i < formationPositionIndicatorCount; i++)
            {
                formationPositionIndicators[i].SetActive(false);
            }
        }

        private void SelectRole(RoleInfo roleInfo)
        {
            _selectedRoleInfo = roleInfo;
            if (_selectedRoleInfo != null)
            {
                ShowFormationPositionIndicators();
                if (PvpFormationProxy.instance.IsHeroInFormation(roleInfo.instanceID))
                {
                    FormationPosition formationPosition = PvpFormationProxy.instance.GetHeroCurrentFormationPosition(roleInfo.instanceID);
                    Vector3 formationBaseButtonPosition = formationBaseButtons[(int)formationPosition - 1].transform.position;
                    removeRoleButton.transform.position = new Vector3(formationBaseButtonPosition.x, formationBaseButtonPosition.y, removeRoleButton.transform.position.z);
                    removeRoleButton.gameObject.SetActive(_selectedRoleInfo.instanceID != GameProxy.instance.PlayerInfo.instanceID);

                    Vector3 indicatorLocalPosition = selectedFormationPositionIndicatorImage.transform.parent.InverseTransformPoint(formationBaseButtons[(int)formationPosition - 1].transform.position);
                    indicatorLocalPosition = new Vector3(indicatorLocalPosition.x, indicatorLocalPosition.y + 230, -800);
                    selectedFormationPositionIndicatorImage.transform.localPosition = indicatorLocalPosition;
                    selectedFormationPositionIndicatorImage.gameObject.SetActive(true);
                    Vector3[] path = new Vector3[4];
                    path[0] = indicatorLocalPosition + new Vector3(0, 0, 0);
                    path[1] = indicatorLocalPosition + new Vector3(0, 30, 0);
                    path[2] = indicatorLocalPosition + new Vector3(0, 0, 0);
                    path[3] = indicatorLocalPosition + new Vector3(0, 0, 0);

                    LTDescr ltDescr = LeanTween.moveLocal(selectedFormationPositionIndicatorImage.gameObject, path, 0.6f);
                    ltDescr.setRepeat(-1);
                }
                else
                {
                    removeRoleButton.gameObject.SetActive(false);
                    selectedFormationPositionIndicatorImage.gameObject.SetActive(false);
                }
            }
            else
            {
                HideAllFormationPositionIndicators();
                removeRoleButton.gameObject.SetActive(false);
                selectedFormationPositionIndicatorImage.gameObject.SetActive(false);
                _selectedCommonHeroIcon = null;
            }
            scrollContent.RefreshAllContentItems();
        }

        private void Refresh()
        {
            SelectRole(null);
            HideAllFormationPositionIndicators();
            RegenerateInTeamHeroModels();
            RefreshCombatCapability();
            RefreshTrainForamtionButton();
        }

        public void OnFormationUpdateHandler()
        {
            Refresh();
        }
        private void FormationUpgradeHandler()
        {
            RefreshCombatCapability();
        }
        public void OnHeroInfoListUpdateHandler()
        {

            GenerateHeroButtons();
            Refresh();
        }
        #region UI event handlers
        public void ClickCloseHandler()
        {
            //PvpFormationProxy.instance.FinishEmbattle();
            UIMgr.instance.Close(PREFAB_PATH);
        }

        public void ClickShowHideRightPanelButtonHandler()
        {
            if (_isRightPanelShowing)
            {
                LeanTween.move(rightPanelTransform.gameObject, rightPanelHidePosTransform.position, 0.25f);
            }
            else
            {
                LeanTween.move(rightPanelTransform.gameObject, rightPanelShowPosTransform.position, 0.25f);
            }
            _isRightPanelShowing = !_isRightPanelShowing;
        }

        public void ClickTrainFormation()
        {
			if(!FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.FormationTraining,true))
			{
				return;	
			}
            //TrainFormationView.Open(FormationTeamType.pvpTeam);
        }

        public void ClickFormationBaseButtonHandler(Button formationBaseButton)
        {
            int formationSlotIndex = formationBaseButtons.IndexOf(formationBaseButton);
            FormationPosition formationPosition = (FormationPosition)(formationSlotIndex + 1);
            bool isPositionEmpty = PvpFormationProxy.instance.IsPositionEmpty(formationPosition);
            uint roleInstanceID = PvpFormationProxy.instance.GetCharacterInstanceIDAt(formationPosition);
            RoleInfo roleInfo = null;
            if (GameProxy.instance.IsPlayer(roleInstanceID))
            {
                roleInfo = GameProxy.instance.PlayerInfo;
            }
            else
            {
                roleInfo = HeroProxy.instance.GetHeroInfo(roleInstanceID);
            }

            if (_selectedRoleInfo != null)
            {
                if (!isPositionEmpty &&
                    _selectedRoleInfo.instanceID == roleInstanceID)
                {
                    SelectRole(null);
                }
                else
                {
                    if (PvpFormationProxy.instance.CanAddToFormationPosition(formationPosition, _selectedRoleInfo.instanceID))
                    {
                        PvpFormationProxy.instance.AddHeroToFormaiton(formationPosition, _selectedRoleInfo.instanceID);
                        PvpController.instance.CLIENT2LOBBY_RANK_ARENA_TEAM_CHANGE_REQ();
                        Refresh();
                    }
                }
            }
            else
            {
                if (!isPositionEmpty)
                {
                    SelectRole(roleInfo);
                }
            }
            Debugger.Log(formationBaseButton.name);
        }

        public void ClickRoleButtonHandler(Logic.UI.CommonHeroIcon.View.CommonHeroIcon commonHeroIcon)
        {
            _selectedCommonHeroIcon = commonHeroIcon;
            SelectRole(commonHeroIcon.HeroInfo);
            //			Debugger.Log(string.Format("You clicked {0}", Localization.Get(_selectedRoleInfo.id.name)));
        }

        public void ClickCancelSelectButton()
        {
            _selectedCommonHeroIcon = null;
            SelectRole(null);
        }

        public void ClickRemoveRoleButtonHandler()
        {
            if (_selectedRoleInfo != null)
            {
                PvpFormationProxy.instance.RemoveHeroFromFormation(_selectedRoleInfo.instanceID);
                PvpController.instance.CLIENT2LOBBY_RANK_ARENA_TEAM_CHANGE_REQ();
                Refresh();
            }
        }
		public void ClickPlayerBtnHandler()
		{
			FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.PlayerInfoView);
		}
        public void ClickRolesInfoButtonHandler()
        {
			FunctionOpenProxy.instance.OpenLuaView(FunctionOpenType.MainView_Hero);
        }

        public void ClickEquipmentsButtonHandler()
        {
            if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.MainView_Equipment, true))
            {
                return;
            }
            UIMgr.instance.Open(Logic.UI.Equipments.View.EquipmentsBrowseView.PREFAB_PATH);
        }
        public void ClickReadyFightBtnHandler()
        {
            if (_isReadyFight)
            {
                if (PvpProxy.instance.PvpInfo.remainChallengeTimes == 0)
                {
                    CommonErrorTipsView.Open(Localization.Get("ui.pvp_formation_view.notEnoughChallengeTimes"));
                    return;
                }
                PvpFighterInfo challengeFighter = PvpProxy.instance.ChallengeFighter;
                PvpController.instance.CLIENT2LOBBY_RANK_ARENA_CHANLLENGE_REQ(challengeFighter);
            }
        }
        public void OnResetItemHandler(GameObject go, int index)
        {
            Logic.UI.CommonHeroIcon.View.CommonHeroIcon icon = go.GetComponent<Logic.UI.CommonHeroIcon.View.CommonHeroIcon>();
            go.name = index.ToString();
            HeroInfo info = _cachedHeroInfoList[index];
			RoleDesButton.Get(go).SetRoleInfo(info);
            icon.SetRoleInfo(info);
            icon.onClickHandler = ClickRoleButtonHandler;
            icon.SetInFormation(PvpFormationProxy.instance.IsHeroInFormation(info.instanceID));
            bool selectedRole = _selectedRoleInfo != null && icon.HeroInfo.instanceID == _selectedRoleInfo.instanceID;
            icon.SetSelect(selectedRole);
        }
        #endregion UI event handlers
    }
}