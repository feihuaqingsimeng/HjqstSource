//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections.Generic;
//using Common.Util;
//using Common.Localization;
//using Logic.Formation.Model;
//using Logic.UI.CommonTopBar.View;
//using Common.UI.Components;
//using Logic.Enums;
//using Logic.Task.Model;
//using Logic.Task;
//using Logic.Formation.Controller;
//using Logic.UI.TrainFormation.Model;
//using Logic.UI.Tips.View;
//using Logic.Game.Model;
//using Logic.Item.Model;
//using Common.ResMgr;
//using Logic.ConsumeTip.Model;
//using System.Collections;
//using Logic.FunctionOpen.Model;
//using System.Text;
//using LuaInterface;
//
//namespace Logic.UI.TrainFormation.View
//{
//    public class TrainFormationView : MonoBehaviour
//    {
//        public const string PREFAB_PATH = "ui/train_formation/train_formation_view";
//        public static TrainFormationView Open(FormationTeamType type)
//        {
//
//            TrainFormationView view = UIMgr.instance.Open<TrainFormationView>(PREFAB_PATH);
//            view.SetData(type);
//            return view;
//        }
//
//
//        #region UI components
//        public GameObject core;
//        public ScrollContentExpand scrollContent;
//        public Transform formationIconRoot;
//        public Text formationNameText;
//        public Text formationLevelText;
//        public Text formationStateText;
//        public Text formationEffectDesText;
//        public Text additionFormationAttrDesText;
//        public Button additionFormationAttrBtn;
//        public Text trainPointText;
//        public Text trainPointRecoverText;
//        public Image trainCostImage;
//        public Text trainCostText;
//        public Image trainCostImage2;
//        public Text trainCostText2;
//        public Text lockTipText;
//
//        public GameObject trainBtnGO;
//        public GameObject useBtnGO;
//        public GameObject unlockConditionGO;
//
//        public FormationButton formationButtonPrefab;
//
//        private CommonTopBarView _commonTopBarView;
//        //		private FormationButton _selectFormationButton;
//        private FormationInfo _selectFormationInfo;
//        #endregion UI components
//
//        private FormationTeamType _teamType;
//        private int _currentInUseId;//当前使用的阵型id
//        void Awake()
//        {
//            BindDelegate();
//            _commonTopBarView = CommonTopBarView.CreateNewAndAttachTo(core.transform);
//            _commonTopBarView.SetAsCommonStyle(Localization.Get("ui.train_formation_view.title"), ClickCloseHandler, true, true, true, true);
//            FormationController.instance.CLIENT2LOBBY_LineupPointSyn_REQ();
//        }
//
//		void Start ()
//		{
//			LeanTween.delayedCall(0.6f, OnViewReady);
//		}
//
//		private void OnViewReady ()
//		{
//			Observers.Facade.Instance.SendNotification(string.Format("{0}::{1}", PREFAB_PATH, "OnViewReady"));
//		}
//
//        void OnDestroy()
//        {
//            UnbindDelegate();
//            TrainFormationProxy.instance.ClearNewFormationTip();
//        }
//        private void BindDelegate()
//        {
//            TrainFormationProxy.instance.onRecoverTimeUpdateDelegate += RefreshTrainPointRecover;
//            TrainFormationProxy.instance.onBuyTrainPointDelegate += RefreshBuyTrainPointByProtocol;
//            TrainFormationProxy.instance.onUpgradeTrainFormationDelegate += RefreshonUpgradeTrainFormationByProtocol;
//            TrainFormationProxy.instance.onAdditionFormationAttrActiveDelegate += RefreshonUpdateAdditionFormationAttrActiveByProtocol;
//        }
//        private void UnbindDelegate()
//        {
//            TrainFormationProxy.instance.onRecoverTimeUpdateDelegate -= RefreshTrainPointRecover;
//            TrainFormationProxy.instance.onBuyTrainPointDelegate -= RefreshBuyTrainPointByProtocol;
//            TrainFormationProxy.instance.onUpgradeTrainFormationDelegate -= RefreshonUpgradeTrainFormationByProtocol;
//            TrainFormationProxy.instance.onAdditionFormationAttrActiveDelegate -= RefreshonUpdateAdditionFormationAttrActiveByProtocol;
//        }
//
//        private void RefreshonUpdateAdditionFormationAttrActiveByProtocol()
//        {
//            RefreshFormation();
//            CommonErrorTipsView.Open(Localization.Get("ui.train_formation_view.addition_formation_active_success_tips"));
//        }
//
//        private void SetData(FormationTeamType type)
//        {
//            _teamType = type;
//            _currentInUseId = FormationProxy.instance.GetFormationTeam(type).formationId;
//            _selectFormationInfo = FormationProxy.instance.GetFormationInfo(_currentInUseId);
//            //GenerateFormationButtons();
//            StartCoroutine(GenerateFormationButtonsCoroutine());
//            Refresh();
//        }
//        private void GenerateFormationButtons()
//        {
//            List<FormationInfo> allFormationInfoList = FormationProxy.instance.GetAllFormationInfoList();
//            int formationInfoCount = allFormationInfoList.Count;
//            scrollContent.Init(formationInfoCount, true);
//        }
//        private IEnumerator GenerateFormationButtonsCoroutine()
//        {
//            yield return null;
//            GenerateFormationButtons();
//        }
//        private void Refresh()
//        {
//            RefreshContentItem();
//            RefreshFormation();
//        }
//        private void RefreshContentItem()
//        {
//            scrollContent.RefreshAllContentItems();
//        }
//        private void RefreshFormation()
//        {
//            FormationInfo info = _selectFormationInfo;
//            FormationState state = info.id == _currentInUseId ? FormationState.InUse : info.formationState;
//            TransformUtil.ClearChildren(formationIconRoot, true);
//            FormationButton b = Instantiate<FormationButton>(formationButtonPrefab);
//            b.gameObject.SetActive(true);
//            b.SetButtonEnable(false);
//            b.transform.SetParent(formationIconRoot, false);
//            b.SetInfo(info, FormationState.NotInUse);
//            b.ShowLevel(false);
//
//            formationNameText.text = Localization.Get(Localization.Get(info.formationData.name));
//            formationLevelText.text = string.Format(Localization.Get("ui.train_formation_view.formation_level_text"), info.level);
//            formationStateText.text = GetFormationStateString(state);
//            //培养条件
//            trainCostImage.sprite = ResMgr.instance.Load<Sprite>(UIUtil.GetBaseResIconPath(info.UpgradeResCost.type));
//            trainCostImage.SetNativeSize();
//            trainCostText.text = info.UpgradeResCost.count.ToString();
//            trainCostText2.text = info.UpgradeTrainPointCost.ToString();
//            //btn
//            bool isLock = state == FormationState.Locked;
//            trainBtnGO.SetActive(!isLock);
//            useBtnGO.SetActive(!isLock);
//            UIUtil.SetGray(useBtnGO, state == FormationState.InUse);
//            unlockConditionGO.SetActive(isLock);
//
//            RefreshEffectAdd();
//            if (isLock)
//                RefreshUnlockCondition();
//        }
//        private void RefreshEffectAdd()
//        {
//            FormationInfo info = _selectFormationInfo;
//            Dictionary<FormationEffectType, float> curDic = info.formationAttrDic;
//            List<FormationAttrData> attrDataList = FormationAttrData.GetFormationDatas(info.id, info.level);
//
//            Dictionary<FormationEffectType, float> nextDic = new FormationInfo(info.id, info.level + 1, info.formationState).formationAttrDic;
//
//            StringBuilder sb = new StringBuilder();
//            for (int i = 0, count = attrDataList.Count; i < count; i++)
//            {
//                FormationAttrData attrData = attrDataList[i];
//                string nextEffectString = string.Empty;
//                string duringTimeString = string.Empty;
//                if (!info.IsMax || info.isFollowAccountLevel)
//                {
//                    nextEffectString = string.Format(Localization.Get("ui.train_formation_view.nextLevelEffect"), nextDic[attrData.effectType] * 100);
//                }
//                if (attrData.time != 0)
//                {
//                    duringTimeString = string.Format(Localization.Get("ui.train_formation_view.duringTime"), attrData.time);
//                }
//                if (attrData.effectType == FormationEffectType.TreatPercent)
//                {
//                    sb.Append(string.Format(Localization.Get("ui.train_formation_view.effect" + (int)attrData.effectType), attrData.interval, info.GetFormationAttrValue(attrData.effectType) * 100, duringTimeString + nextEffectString));
//                }
//                else
//                {
//                    sb.Append(string.Format(Localization.Get("ui.train_formation_view.effect" + (int)attrData.effectType), info.GetFormationAttrValue(attrData.effectType) * 100, duringTimeString + nextEffectString));
//                }
//            }
//            //			foreach(var value in curDic)
//            //			{
//            //				string nextEffectString = string.Empty;
//            //				if(!info.IsMax || info.isFollowAccountLevel)
//            //				{
//            //					nextEffectString = string.Format(Localization.Get("ui.train_formation_view.nextLevelEffect"),nextDic[value.Key]*100);
//            //				}
//            //				effectString += string.Format( Localization.Get("ui.train_formation_view.effect"+(int)value.Key),value.Value*100,nextEffectString);
//            //			}
//            formationEffectDesText.text = sb.ToString();
//            sb.Remove(0, sb.Length);
//            List<FormationAttrData> additionFormationAttrDatas = FormationAttrData.GetFormationDatas(info.id, FormationAttrType.Addition);
//            if (additionFormationAttrDatas.Count > 0)
//            {
//                for (int i = 0, count = additionFormationAttrDatas.Count; i < count; i++)
//                {
//                    FormationAttrData attrData = additionFormationAttrDatas[i];
//                    string duringTimeString = string.Empty;
//                    if (attrData.time != 0)
//                    {
//                        duringTimeString = string.Format(Localization.Get("ui.train_formation_view.duringTime"), attrData.time);
//                    }
//                    if (attrData.effectType == FormationEffectType.TreatPercent)
//                    {
//                        sb.Append(string.Format(Localization.Get("ui.train_formation_view.effect" + (int)attrData.effectType), attrData.interval, info.GetFormationAdditionAttrValue(attrData.effectType) * 100, duringTimeString));
//                    }
//                    else
//                    {
//						sb.Append(string.Format(Localization.Get("ui.train_formation_view.effect" + (int)attrData.effectType), info.GetFormationAdditionAttrValue(attrData.effectType) * 100, duringTimeString));
//                    }
//                }
//                additionFormationAttrDesText.text = sb.ToString();
//                if (info.isActiveAdditionAttr)
//                {
//                    additionFormationAttrDesText.color = Color.green;
//                    additionFormationAttrBtn.gameObject.SetActive(false);
//                }
//                else
//                {
//                    additionFormationAttrDesText.color = Color.red;
//                    additionFormationAttrBtn.gameObject.SetActive(true);
//                    if (info.formationState != FormationState.Locked)
//                    {
//                        additionFormationAttrBtn.enabled = true;
//                        additionFormationAttrBtn.image.SetGray(false);
//                    }
//                    else
//                    {
//                        additionFormationAttrBtn.enabled = false;
//                        additionFormationAttrBtn.image.SetGray(true);
//                    }
//                }
//            }
//            else
//            {
//                additionFormationAttrDesText.gameObject.SetActive(false);
//                additionFormationAttrBtn.gameObject.SetActive(false);
//            }
//            sb = null;
//        }
//        private void RefreshUnlockCondition()
//        {
//            lockTipText.text = string.Empty;
//            FormationInfo info = _selectFormationInfo;
//            int[] conditionIds = info.formationData.formationConditionIds;
//            TaskConditionData data;
//            for (int i = 0, count = conditionIds.Length; i < count; i++)
//            {
//                data = TaskConditionData.GetTaskConditionData(conditionIds[i]);
//                lockTipText.text += TaskUtil.GetTaskConditionDescriptionWithColor(data) + "\n";
//            }
//        }
//        private void RefreshTrainPointRecover()
//        {
//            TrainFormationProxy trainProxy = TrainFormationProxy.instance;
//            trainPointText.text = string.Format("{0}/{1}", trainProxy.trainPoint, trainProxy.trainPointRecoverMax);
//            if (trainProxy.trainPointNextRecoverCountDown == -1)
//            {
//                trainPointRecoverText.gameObject.SetActive(false);
//            }
//            else
//            {
//                trainPointRecoverText.gameObject.SetActive(true);
//                trainPointRecoverText.text = string.Format("({0})", TimeUtil.FormatSecondToMinute(trainProxy.trainPointNextRecoverCountDown / 1000));
//            }
//        }
//        private void RefreshBuyTrainPointByProtocol()
//        {
//
//        }
//        private void RefreshonUpgradeTrainFormationByProtocol()
//        {
//            Refresh();
//        }
//        private string GetFormationStateString(FormationState state)
//        {
//            if (state == FormationState.InUse)
//            {
//                return Localization.Get("ui.train_formation_view.formation_state_use");
//            }
//            else if (state == FormationState.NotInUse)
//            {
//                return Localization.Get("ui.train_formation_view.formation_state_noUse");
//            }
//            else
//            {
//                return Localization.Get("ui.train_formation_view.formation_state_lock");
//            }
//        }
//        private void OpenBuyTrainPointView()
//        {
//            int price = TrainFormationProxy.instance.trainPointPurcase;
//            int remaindTimes = TrainFormationProxy.instance.TrainPointPurcasedMaxTimes - TrainFormationProxy.instance.trainPointPurcasedTimes;
//            if (remaindTimes <= 0)
//            {
//                CommonAutoDestroyTipsView.Open(Localization.Get("ui.train_buy_point_view.buyNotEnough"));
//                return;
//            }
//            if (ConsumeTipProxy.instance.GetConsumeTipEnable(ConsumeTipType.DiamondBuyFormationTrainPoint))
//            {
//                ComfirmBuyTrainPointView.Open(price, GlobalData.GetGlobalData().formationTrainPointBuyNum, remaindTimes, BuyTrainPointComfirmHandler, ConsumeTipType.DiamondBuyFormationTrainPoint);
//
//            }
//            else
//            {
//                BuyTrainPointComfirmHandler();
//            }
//        }
//        private void BuyTrainPointComfirmHandler()
//        {
//            int price = TrainFormationProxy.instance.trainPointPurcase;
//            int ownDiamond = GameProxy.instance.BaseResourceDictionary.GetValue(BaseResType.Diamond);
//            if (price > ownDiamond)
//            {
//                CommonAutoDestroyTipsView.Open(Localization.Get("ui.train_buy_point_view.diamondNotEnough"));
//                return;
//            }
//            FormationController.instance.CLIENT2LOBBY_LineupPointBuy_REQ();
//        }
//
//        #region UI event handlers
//        public void ClickCloseHandler()
//        {
//            UIMgr.instance.Close(PREFAB_PATH);
//        }
//
//        public void ClickFormationButton(FormationButton formationButton)
//        {
//            //_selectFormationButton.SetSelect(false);
//            //_selectFormationButton =  formationButton;
//            //_selectFormationButton.SetSelect(true);
//            _selectFormationInfo = formationButton.FormationInfo;
//            Refresh();
//        }
//
//
//        //培养
//        public void OnClickTrainBtnHandler()
//        {
//            if (_selectFormationInfo.IsMax)
//            {
//                if (_selectFormationInfo.isFollowAccountLevel)
//                    CommonAutoDestroyTipsView.Open(Localization.Get("ui.train_buy_point_view.limitAccountLevel"));
//                else
//                    CommonAutoDestroyTipsView.Open(Localization.Get("ui.train_buy_point_view.limitLevel"));
//                return;
//            }
//            //培养点不足
//            if (_selectFormationInfo.UpgradeTrainPointCost > TrainFormationProxy.instance.trainPoint)
//            {
//                int remaindTimes = TrainFormationProxy.instance.TrainPointPurcasedMaxTimes - TrainFormationProxy.instance.trainPointPurcasedTimes;
//                if (remaindTimes == 0)//购买次数不足
//                {
//                    CommonAutoDestroyTipsView.Open(Localization.Get("ui.train_buy_point_view.trainPointNotEnough"));
//                    return;
//                }
//                OpenBuyTrainPointView();
//                return;
//            }
//            GameResData secondResData = _selectFormationInfo.UpgradeResCost;
//            int mySecondResCount = 0;
//            string resName = "itemid找不到";
//            if (secondResData.type == BaseResType.Item)
//            {
//                mySecondResCount = ItemProxy.instance.GetItemCountByItemID(secondResData.id);
//                ItemData itemdata = ItemData.GetItemDataByID(secondResData.id);
//                resName = Localization.Get(itemdata.name);
//            }
//            else
//            {
//                mySecondResCount = GameProxy.instance.BaseResourceDictionary.GetValue(secondResData.type);
//                ItemData itemdata = ItemData.GetBasicResItemByType(secondResData.type);
//                if (itemdata != null)
//                    resName = Localization.Get(itemdata.name);
//            }
//
//            if (secondResData.count > mySecondResCount)
//            {
//                CommonAutoDestroyTipsView.Open(string.Format(Localization.Get("ui.train_buy_point_view.notEnoughGoods"), resName));
//                return;
//            }
//            FormationController.instance.CLIENT2LOBBY_LineupUpgrade_REQ(_selectFormationInfo.id);
//        }
//        //使用阵型
//        public void OnClickUseBtnHandler()
//        {
//            if (_selectFormationInfo.id == _currentInUseId)
//                return;
//            _currentInUseId = _selectFormationInfo.id;
//
//			LuaTable formationModel = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","formation_model")[0];
//			LuaTable formationTeamInfoLua = (LuaTable)formationModel.GetLuaFunction("GetFormationTeam").Call(_teamType)[0];
//
//            FormationTeamInfo teamInfo = FormationProxy.instance.GetFormationTeam(_teamType);
//            teamInfo.SetFormationId(  _selectFormationInfo.id);
//            List<uint> teamIdList = teamInfo.teamPosDictionary.GetValues();
//            List<FormationPosition> formationPosList = _selectFormationInfo.formationData.GetAllEnabledPos();
//            teamInfo.ClearTeam();
//
//            for (int i = 0, count = teamIdList.Count; i < count; i++)
//            {
//                if (formationPosList.Count > 0)
//                {
//					teamInfo.AddHeroToFormaiton(formationPosList[0], teamIdList[i]);
//                   // teamInfo.teamPosDictionary.Add(formationPosList[0], teamIdList[i]);
//                    formationPosList.RemoveAt(0);
//                }
//            }
//
//            FormationController.instance.CLIENT2LOBBY_TeamChange_REQ(_teamType);
//            Refresh();
//            TrainFormationProxy.instance.UpdateFormationChangedByProtocol();
//        }
//        public void OnClickBuyTrainPointBtnHandler()
//        {
//            OpenBuyTrainPointView();
//        }
//        public void OnResetItemHandler(GameObject go, int index)
//        {
//            FormationButton formationButton = go.GetComponent<FormationButton>();
//            FormationInfo info = FormationProxy.instance.GetAllFormationInfoList()[index];
//            formationButton.SetInfo(info, info.id == _currentInUseId ? FormationState.InUse : info.formationState);
//            bool select = false;
//            if (formationButton.FormationInfo.id == _selectFormationInfo.id)
//                select = true;
//            formationButton.SetSelect(select);
//        }
//
//        public void OnClickAdditionFormationAtrrActiveBtnHandler()
//        {
//            List<FormationAttrData> additionFormationAttrDatas = FormationAttrData.GetFormationDatas(_selectFormationInfo.id, FormationAttrType.Addition);
//            if (additionFormationAttrDatas.Count > 0)
//            {
//                FormationData formationData = _selectFormationInfo.formationData;
//                Tips.View.ConfirmCostTipsView.Open(formationData.costGameResData, Localization.Get("ui.train_formation_view.addition_formation_active_text"),
//                    Localization.Get("ui.train_formation_view.addition_formation_active_tips"), null, AdditionFormationAtrrActiveBtnHandlerOK);
//            }
//        }
//
//        private void AdditionFormationAtrrActiveBtnHandlerOK()
//        {
//            int own = GameProxy.instance.BaseResourceDictionary.GetValue(_selectFormationInfo.formationData.costGameResData.type);
//            if (own < _selectFormationInfo.formationData.costGameResData.count)
//            {
//                CommonErrorTipsView.Open(Localization.Get("ui.train_formation_view.addition_formation_active_resless_tips"));
//                return;
//            }
//            FormationController.instance.LOBBY2CLIENT_LineupAttrActiveReq(_selectFormationInfo.id);
//        }
//        #endregion UI event handlers
//    }
//}