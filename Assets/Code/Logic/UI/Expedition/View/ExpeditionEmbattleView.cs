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
using Logic.Hero;
using Logic.UI.Expedition.Model;
using Logic.UI.Expedition.Controller;
using Logic.UI.FightResult.View;
using Logic.Formation.Model;
using Logic.UI.TrainFormation.View;
using Logic.UI.TrainFormation.Model;
using Common.UI.Components;
using Logic.Role;
using Logic.FunctionOpen.Model;
using Common.ResMgr;
using Logic.UI.Description.View;
using LuaInterface;

namespace Logic.UI.Expedition.View
{
    public class ExpeditionEmbattleView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/expedition/expedition_embattle_view";

        public static ExpeditionEmbattleView Open(bool isReadyFight)
        {
            //ExpeditionEmbattleView view = UIMgr.instance.Open<ExpeditionEmbattleView>(PREFAB_PATH);
            //view.SetReadyFight(isReadyFight);
			LuaTable expeditionModelLua = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetCtrl","expedition_controller")[0];
			expeditionModelLua.GetLuaFunction("OpenExpeditionEmbattleView").Call(isReadyFight);
            return null;
        }
        private List<ExpeditionHeroInfo> _cachedExpeditionHeroInfoList;
        private bool _isReadyFight;
        private RoleInfo _selectedRoleInfo = null;
        private bool _isRightPanelShowing = true;

        private ExpeditionHeroButton _selectedCommonHeroIcon = null;


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
        #endregion UI components


        public Sprite _formationNormalSprite;
        public Sprite _formationOccupiedSprite;
        private List<CharacterEntity> _characters = new List<CharacterEntity>();

        void Awake()
        {

            BindDelegate();
            _commonTopbarView = CommonTopBarView.CreateNewAndAttachTo(core.transform);
            _commonTopbarView.SetAsCommonStyle(Localization.Get("ui.expedition_formation_view.titleText"), ClickCloseHandler, true, true, true, false, false, true);

            removeRoleText.text = Localization.Get("ui.expedition_formation_view.remove_from_team");
            rightPanelTransform.position = rightPanelShowPosTransform.position;

            _formationNormalSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/bg_unit_02");
            _formationOccupiedSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/bg_unit_01");

            ExpeditionFormationProxy.instance.CheckDeadHeroAtFormation();

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
            _cachedExpeditionHeroInfoList = ExpeditionFormationProxy.instance.GetEnableExpeditionHeroList();
            _cachedExpeditionHeroInfoList.Sort(CompareHeroInfo);
            scrollContent.Init(_cachedExpeditionHeroInfoList.Count);
        }

        private int CompareHeroInfo(ExpeditionHeroInfo aHeroIcon, ExpeditionHeroInfo bHeroIcon)
        {
            int aValue = GameProxy.instance.IsPlayer(aHeroIcon.roleInfo.instanceID) ? 1 : 0;
            int bValue = GameProxy.instance.IsPlayer(bHeroIcon.roleInfo.instanceID) ? 1 : 0;
            if (aValue - bValue != 0)
                return bValue - aValue;
            return RoleUtil.CompareRoleByQualityDesc(aHeroIcon.roleInfo, bHeroIcon.roleInfo);
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
                if (!ExpeditionFormationProxy.instance.IsPositionEmpty(formationPosition))
                {
                    uint roleID = ExpeditionFormationProxy.instance.GetCharacterInstanceIDAt(formationPosition);
                    if (GameProxy.instance.IsPlayer(roleID))
                    {
                        PlayerInfo playerInfo = GameProxy.instance.PlayerInfo;
                        PlayerEntity playerEntity = CharacterEntity.CreatePlayerEntityAsUIElement(playerInfo, roleModelRoots[i], false, false);
                        _characters.Add(playerEntity);
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
					formationBaseButtons[i].GetComponent<Image>().SetSprite( ExpeditionFormationProxy.instance.FormationTeamInfo.formationInfo.formationData.GetPosEnalbe(formationPosition) ? _formationOccupiedSprite : _formationNormalSprite);
                }
            }
        }
        //特定阵型
        private void RefreshTrainForamtionButton()
        {
            _formationButton.SetInfo(FormationProxy.instance.GetFormationInfo(ExpeditionFormationProxy.instance.FormationTeamInfo.formationId), FormationState.NotInUse);
        }
        private void RefreshCombatCapability()
        {
            int combatCapability = ExpeditionFormationProxy.instance.ExpeditionPower;
            NumberIncreaseAction numberAct = combatCapabilityText.GetComponent<NumberIncreaseAction>();
            combatCapabilityText.GetComponent<NumberIncreaseAction>().Init(numberAct.endNumber, combatCapability);
            //combatCapabilityText.text = combatCapability.ToString();
        }

        private void ShowFormationPositionIndicators()
        {
            int formationPositionIndicatorCount = formationPositionIndicators.Count;
            for (int i = 0; i < formationPositionIndicatorCount; i++)
            {
                FormationPosition formationPosition = (FormationPosition)(i + 1);
                bool isPositionEmpty = ExpeditionFormationProxy.instance.CanAddToFormationPosition(formationPosition, _selectedRoleInfo.instanceID);
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
                if (ExpeditionFormationProxy.instance.IsHeroInFormation(roleInfo.instanceID))
                {
                    FormationPosition formationPosition = ExpeditionFormationProxy.instance.GetHeroCurrentFormationPosition(roleInfo.instanceID);
                    Vector3 formationBaseButtonPosition = formationBaseButtons[(int)formationPosition - 1].transform.position;
                    removeRoleButton.transform.position = new Vector3(formationBaseButtonPosition.x, formationBaseButtonPosition.y, removeRoleButton.transform.position.z);
                    removeRoleButton.gameObject.SetActive(true);

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



        public void ClickFormationBaseButtonHandler(Button formationBaseButton)
        {
            int formationSlotIndex = formationBaseButtons.IndexOf(formationBaseButton);
            FormationPosition formationPosition = (FormationPosition)(formationSlotIndex + 1);
            bool isPositionEmpty = ExpeditionFormationProxy.instance.IsPositionEmpty(formationPosition);
            uint roleInstanceID = ExpeditionFormationProxy.instance.GetCharacterInstanceIDAt(formationPosition);
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
                    if (ExpeditionFormationProxy.instance.CanAddToFormationPosition(formationPosition, _selectedRoleInfo.instanceID))
                    {
                        ExpeditionFormationProxy.instance.AddHeroToFormaiton(formationPosition, _selectedRoleInfo.instanceID);
                        ExpeditionController.instance.CLIENT2LOBBY_Expedition_Formation_Change_REQ();
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
        public void ClickTrainFormation()
        {
			if(!FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.FormationTraining,true))
			{
				return;	
			}
           // TrainFormationView.Open(FormationTeamType.expeditionTeam);
        }
        public void ClickRoleButtonHandler(ExpeditionHeroButton btn)
        {
            if (btn.ExpeditionHeroInfo.IsDead)
            {
                CommonAutoDestroyTipsView.Open(Localization.Get("ui.expedition_formation_view.die_not_add_Team"));
                return;
            }
            _selectedCommonHeroIcon = btn;
            SelectRole(btn.ExpeditionHeroInfo.roleInfo);
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
                ExpeditionFormationProxy.instance.RemoveHeroFromFormation(_selectedRoleInfo.instanceID);
                ExpeditionController.instance.CLIENT2LOBBY_Expedition_Formation_Change_REQ();
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
                int count = ExpeditionFormationProxy.instance.FormationsDictionary.Count;
                if (count == 0)
                {
                    CommonAutoDestroyTipsView.Open(Localization.Get("ui.expedition_formation_view.notEnoughHero"));
                    return;
                }
                ExpeditionController.instance.CLIENT2LOBBY_Expedition_Challenge_REQ(ExpeditionProxy.instance.selectExpeditionDungeonInfo.id);
            }
        }
        public void OnResetItemHandler(GameObject go, int index)
        {
            ExpeditionHeroButton icon = go.GetComponent<ExpeditionHeroButton>();
            ExpeditionHeroInfo info = _cachedExpeditionHeroInfoList[index];
            icon.SetExpeditionHeroInfo(info);
			RoleDesButton.Get(go).SetRoleInfo(info.roleInfo);
            icon.onClickHandler = ClickRoleButtonHandler;
            icon.SetInFormation(ExpeditionFormationProxy.instance.IsHeroInFormation(info.roleInfo.instanceID));
            bool selectedRole = _selectedRoleInfo != null && info.roleInfo.instanceID == _selectedRoleInfo.instanceID;
            icon.SetSelect(selectedRole);
        }
        #endregion UI event handlers
    }
}