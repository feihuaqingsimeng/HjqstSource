using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Util;
using Common.Localization;
using Logic.Enums;
using Logic.Game.Model;
using Logic.Hero.Model;
using Logic.UI.CommonTopBar.View;
using Logic.UI.ManageHeroes.Model;
using Logic.Player.Model;
using Logic.Character;
using Logic.UI.ManageHeroes.Controller;
using Logic.Role.Model;
using Logic.UI.Tips.View;
using Logic.Hero;
using Logic.UI.TrainFormation.View;
using Common.UI.Components;
using Logic.Role;
using Logic.Formation.Model;
using Logic.UI.TrainFormation.Model;
using Logic.FunctionOpen.Model;
using Common.ResMgr;
using Logic.UI.Description.View;

namespace Logic.UI.PVEEmbattle.View
{
    public class PVEEmbattleView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/pve_embattle/pve_embattle_view";

        public static void Open(System.Action<int> callBack)
        {
            PVEEmbattleView pveEmbattleView = Logic.UI.UIMgr.instance.Open<PVEEmbattleView>(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Replace);
            pveEmbattleView.onBackAction = callBack;
        }

        public System.Action<int> onBackAction;

        private RoleInfo _selectedRoleInfo = null;
        private bool _isRightPanelShowing = true;
        private int _cachedCombatCapability = 0;
        private List<HeroInfo> _cachedHeroInfoList;
        private Dictionary<FormationPosition, uint> _cachedFormationDic = new Dictionary<FormationPosition, uint>();
        private List<CharacterEntity> _characters = new List<CharacterEntity>();


        #region UI components
        public GameObject core;
        private CommonTopBarView _commonTopbarView;

        public Transform rightPanelShowPosTransform;
        public Transform rightPanelHidePosTransform;
        public Transform rightPanelTransform;

        public List<Toggle> teamToggles;
        private Toggle _currentSelectedTeamToggle;

        public Text combatCapabilityText;

        public Button removeRoleButton;
        public Transform cantRemovePlayerTipsRoot;
        public Text removeRoleText;
        public Text cantRemovePlayerTipsText;
        public List<Button> formationBaseButtons;
        public List<Image> formationBaseImages;
        public List<GameObject> formationPositionIndicators;
        public List<Transform> roleModelRoots;
        public List<Image> roleShadows;

        public ScrollContent heroListScrollContent;

        public Image selectedFormationPositionIndicatorImage;
        public FormationButton _formationButton;
        public GameObject commonHeroPrefab;
        #endregion UI components


        private Sprite _formationNormalSprite;
        private Sprite _formationOccupiedSprite;

        void Awake()
        {
            ManageHeroesProxy.instance.onFormationUpdateHandler += OnFormationUpdateHandler;
            HeroProxy.instance.onHeroInfoUpdateDelegate += onHeroInfoUpdateDelegate;
            HeroProxy.instance.onHeroInfoListUpdateDelegate += OnHeroInfoListUpdateDelegate;
            TrainFormationProxy.instance.onFormationChangedDelegate += OnFormationUpdateHandler;
            TrainFormationProxy.instance.onUpgradeTrainFormationDelegate += FormationUpgradeHandler;
            _commonTopbarView = CommonTopBarView.CreateNewAndAttachTo(core.transform);
            _commonTopbarView.SetAsCommonStyle(Localization.Get("ui.pve_embattle_view.title"), ClickCloseHandler, true, true, true, false);

            removeRoleText.text = Localization.Get("ui.pve_embattle_view.remove_role");
            cantRemovePlayerTipsText.text = Localization.Get("ui.pve_embattle_view.cant_remove_player_tips");
            rightPanelTransform.position = rightPanelShowPosTransform.position;
            _formationNormalSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/bg_unit_02");
            _formationOccupiedSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/bg_unit_01");

            Invoke("Init", 0.01f);
            cantRemovePlayerTipsRoot.gameObject.SetActive(false);
            removeRoleButton.gameObject.SetActive(false);
            selectedFormationPositionIndicatorImage.gameObject.SetActive(false);
            commonHeroPrefab.SetActive(false);
        }

        private void Init()
        {
            teamToggles[ManageHeroesProxy.instance.CurrentSelectedFormationNo - 1].isOn = true;
            GenerateHeroButtons();
            //RegenerateInTeamHeroModels();
            //RefreshCombatCapability();
            //SelectRole(null);
        }

        void OnDestroy()
        {
            DespawnCharacter();
            ManageHeroesProxy.instance.onFormationUpdateHandler -= OnFormationUpdateHandler;
            HeroProxy.instance.onHeroInfoUpdateDelegate -= onHeroInfoUpdateDelegate;
            HeroProxy.instance.onHeroInfoListUpdateDelegate -= OnHeroInfoListUpdateDelegate;
            TrainFormationProxy.instance.onFormationChangedDelegate -= OnFormationUpdateHandler;
            TrainFormationProxy.instance.onUpgradeTrainFormationDelegate -= FormationUpgradeHandler;
        }

        private void GenerateHeroButtons()
        {
            _cachedHeroInfoList = HeroProxy.instance.GetAllHeroInfoList();
            _cachedHeroInfoList.Sort(RoleUtil.CompareRoleByQualityDesc);
            heroListScrollContent.Init(_cachedHeroInfoList.Count);
        }

        public void On_HeroListScrollView_ResetItem(GameObject go, int index)
        {
            Logic.UI.CommonHeroIcon.View.CommonHeroIcon commonHeroIcon = go.GetComponent<Logic.UI.CommonHeroIcon.View.CommonHeroIcon>();
            go.name = index.ToString();
            commonHeroIcon.SetRoleInfo(_cachedHeroInfoList[index]);
            RoleDesButton.Get(go).SetRoleInfo(_cachedHeroInfoList[index]);
            commonHeroIcon.SetInFormation(ManageHeroesProxy.instance.IsHeroInFormation(_cachedHeroInfoList[index].instanceID));
            commonHeroIcon.onClickHandler = ClickRoleButtonHandler;
            bool selectedRole = _selectedRoleInfo != null && commonHeroIcon.HeroInfo.instanceID == _selectedRoleInfo.instanceID;
            commonHeroIcon.SetSelect(selectedRole);
        }

        public void On_HeroListScrollView_InitComplete()
        {

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
                FormationPosition formationPosition = (FormationPosition)(i + 1);
                uint roleID = ManageHeroesProxy.instance.GetCharacterInstanceIDAt(formationPosition);
                //uint cachedRoleID = 0;
                //_cachedFormationDic.TryGetValue(formationPosition, out cachedRoleID);

                //if (roleID != cachedRoleID)
                //{
                TransformUtil.ClearChildren(roleModelRoots[i], true);
                if (!ManageHeroesProxy.instance.IsPositionEmpty(formationPosition))
                {
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
                }
                _cachedFormationDic.AddOrReplace(formationPosition, roleID);
                //}
				formationBaseImages[i].SetSprite( roleID > 0 || ManageHeroesProxy.instance.CurrentFormationTeamInfo.formationInfo.formationData.GetPosEnalbe(formationPosition) ? _formationOccupiedSprite : _formationNormalSprite);
                roleShadows[i].gameObject.SetActive(roleID > 0);
            }
        }

        private void RefreshCombatCapability()
        {
            int newCombatCapability = ManageHeroesProxy.instance.CurrentFormationTeamInfo.Power;
            //			SortedDictionary<FormationPosition, uint> currentFormationDic = ManageHeroesProxy.instance.CurrentFormationDictionary;
            //			List<FormationPosition> formationPositions = currentFormationDic.GetKeys();
            //			int formationPositionCount = formationPositions.Count;
            //			for (int i = 0; i < formationPositionCount; i++)
            //			{
            //				uint roleInstanceID = currentFormationDic[formationPositions[i]];
            //				if (GameProxy.instance.IsPlayer(roleInstanceID))
            //				{
            //					newCombatCapability += GameProxy.instance.PlayerInfo.Power;
            //				}
            //				else
            //				{
            //					newCombatCapability += HeroProxy.instance.GetHeroInfo(roleInstanceID).Power;
            //				}
            //			}
            LTDescr ltDescr = LeanTween.value(gameObject, _cachedCombatCapability, newCombatCapability, 0.8f);
            ltDescr.setOnUpdate(UpdateCombatCapability);
            _cachedCombatCapability = newCombatCapability;
        }

        private void UpdateCombatCapability(float combatCapability)
        {
            combatCapabilityText.text = Mathf.RoundToInt(combatCapability).ToString();
        }

        private void ShowFormationPositionIndicators()
        {
            int formationPositionIndicatorCount = formationPositionIndicators.Count;
            for (int i = 0; i < formationPositionIndicatorCount; i++)
            {
                FormationPosition formationPosition = (FormationPosition)(i + 1);
                bool isPositionEmpty = ManageHeroesProxy.instance.CanAddToFormationPosition(formationPosition, _selectedRoleInfo.instanceID);
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
                if (ManageHeroesProxy.instance.IsHeroInFormation(roleInfo.instanceID))
                {
                    FormationPosition formationPosition = ManageHeroesProxy.instance.GetHeroCurrentFormationPosition(roleInfo.instanceID);
                    Vector3 formationBaseButtonPosition = formationBaseButtons[(int)formationPosition - 1].transform.position;

                    bool isPlayer = GameProxy.instance.IsPlayer(roleInfo.instanceID);
                    removeRoleButton.transform.position = new Vector3(formationBaseButtonPosition.x, formationBaseButtonPosition.y, removeRoleButton.transform.position.z);
                    cantRemovePlayerTipsRoot.position = new Vector3(formationBaseButtonPosition.x, formationBaseButtonPosition.y, cantRemovePlayerTipsRoot.position.z);
                    removeRoleButton.gameObject.SetActive(!isPlayer);
                    cantRemovePlayerTipsRoot.gameObject.SetActive(isPlayer);
                    UIUtil.CrossFadeAlpha(cantRemovePlayerTipsRoot.gameObject, 1, 0, 2.5f);

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
                    cantRemovePlayerTipsRoot.gameObject.SetActive(false);
                    selectedFormationPositionIndicatorImage.gameObject.SetActive(false);
                }
            }
            else
            {
                HideAllFormationPositionIndicators();
                removeRoleButton.gameObject.SetActive(false);
                cantRemovePlayerTipsRoot.gameObject.SetActive(false);
                selectedFormationPositionIndicatorImage.gameObject.SetActive(false);
            }
            heroListScrollContent.RefreshAllContentItems();
        }

        private void Refresh()
        {
            SelectRole(null);
            HideAllFormationPositionIndicators();
            RegenerateInTeamHeroModels();
            RefreshCombatCapability();
            RefreshTrainForamtionButton();
        }

        void onHeroInfoUpdateDelegate(uint heroInstanceID)
        {
            Refresh();
        }

        void OnHeroInfoListUpdateDelegate()
        {
            GenerateHeroButtons();
            RegenerateInTeamHeroModels();
            RefreshCombatCapability();
            SelectRole(null);
        }

        public void OnFormationUpdateHandler()
        {

            Refresh();
        }
        private void FormationUpgradeHandler()
        {
            RefreshCombatCapability();
        }
        #region UI event handlers
        public void ClickCloseHandler()
        {
            ManageHeroesController.instance.FinishEmbattle();
            if (onBackAction != null)
                onBackAction((int)ManageHeroesProxy.instance.CurrentSelectFormationTeamType);
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
        public void ClickPlayerBtnHandler()
        {
            FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.PlayerInfoView);
        }
        public void ClickTeamToggleHandler(Toggle clickedTeamToggle)
        {
            if (clickedTeamToggle.isOn)
            {
                if (_currentSelectedTeamToggle == clickedTeamToggle)
                {
                    return;
                }

                int teamIndex = teamToggles.IndexOf(clickedTeamToggle) + 1;
                if (ManageHeroesProxy.instance.IsTeamUnlocked(teamIndex))
                {
                    ManageHeroesProxy.instance.CurrentSelectedFormationNo = teamIndex;
                    _currentSelectedTeamToggle = clickedTeamToggle;
                    Refresh();
                    ManageHeroesController.instance.CLIENT2LOBBY_PVE_TEAM_CHANGE_REQ();
                }
                else
                {
                    CommonErrorTipsView.Open(string.Format(Localization.Get("ui.manage_heroes_view.team_locked_tips"), teamIndex.ToString()));
                    _currentSelectedTeamToggle.isOn = true;
                }
            }
        }
        //特定阵型
        private void RefreshTrainForamtionButton()
        {
            _formationButton.SetInfo(FormationProxy.instance.GetFormationInfo(ManageHeroesProxy.instance.CurrentFormationTeamInfo.formationId), FormationState.NotInUse);
        }
        public void ClickTrainFormation()
        {
            if (!FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.FormationTraining, true))
            {
                return;
            }

            //TrainFormationView.Open(ManageHeroesProxy.instance.CurrentSelectFormationTeamType);
        }

        public void ClickFormationBaseButtonHandler(Button formationBaseButton)
        {
            int formationSlotIndex = formationBaseButtons.IndexOf(formationBaseButton);
            FormationPosition formationPosition = (FormationPosition)(formationSlotIndex + 1);
            bool isPositionEmpty = ManageHeroesProxy.instance.IsPositionEmpty(formationPosition);
            uint roleInstanceID = ManageHeroesProxy.instance.GetCharacterInstanceIDAt(formationPosition);
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
                    if (ManageHeroesProxy.instance.CanAddToFormationPosition(formationPosition, _selectedRoleInfo.instanceID))
                    {
                        ManageHeroesController.instance.AddHeroToFormaiton(formationPosition, _selectedRoleInfo.instanceID);
                        ManageHeroesController.instance.CLIENT2LOBBY_PVE_TEAM_CHANGE_REQ();
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
            SelectRole(commonHeroIcon.HeroInfo);
        }

        public void ClickCancelSelectButton()
        {
            SelectRole(null);
        }

        public void ClickRemoveRoleButtonHandler()
        {
            if (_selectedRoleInfo != null)
            {
                ManageHeroesProxy.instance.RemoveHeroFromFormation(_selectedRoleInfo.instanceID);
                ManageHeroesController.instance.CLIENT2LOBBY_PVE_TEAM_CHANGE_REQ();
            }
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
        #endregion UI event handlers
    }
}