local t = {}
local PREFAB_PATH = 'ui/train_formation/train_formation_view'

local game_model = gamemanager.GetModel('game_model')
local vip_model = gamemanager.GetModel('vip_model')
local formation_model = gamemanager.GetModel('formation_model')
local formation_controller = gamemanager.GetCtrl('formation_controller')
local formation_button = require('ui/formation/view/training/formation_button')
local confirm_cost_tip_view = require('ui/tips/view/confirm_cost_tip_view')
local auto_destroy_tip_view = require('ui/tips/view/auto_destroy_tip_view')
local confirm_buy_train_point_view = require('ui/formation/view/training/confirm_buy_train_point_view')
local confirm_tip_view = require('ui/tips/view/confirm_tip_view')

function t.Open(formationTeamType)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  local common_top_bar = require ('ui/common_top_bar/common_top_bar')
  common_top_bar = common_top_bar:Create(t.transform:Find('core'))
  common_top_bar:SetAsCommonStyle(LocalizationController.instance:Get("ui.train_formation_view.title"),t.Close,true, true, true, true, false, false,false)
  
  t.scrollItems = {}
  t.formationInfoList = {}
  t.isFirstEnter = true
  t.formationTeamType = formationTeamType
  t.currentInUseId = formation_model.GetFormationTeam(formationTeamType).formationId
  t.selectFormationInfo = formation_model.GetFormationInfo(t.currentInUseId)
  
  t.BindDelegate()
  t.InitComponent()
  t.InitScrollContent()
  t.RefreshFormation()
  t.RefreshTrainPointRecover()
  
  LeanTween.delayedCall(0.6, Action(t.OnViewReady))
end
function t.Close()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
  formation_model.ClearNewFormationTip()
end
function t.BindDelegate()
  formation_model.FormationAdditionAttrActiveDelegate:AddListener(t.FormationAdditionAttrActiveByProtocol)
  formation_model.FormationBuyTrainPointDelegate:AddListener(t.RefreshBuyTrainPointByProtocol)
  formation_model.FormationUpgradeDelegate:AddListener(t.RefreshonUpgradeTrainFormationByProtocol)
  formation_model.onRecoverTimeUpdateDelegate:AddListener(t.RefreshTrainPointRecover)
end
function t.UnbindDelegate()
  formation_model.FormationAdditionAttrActiveDelegate:RemoveListener(t.FormationAdditionAttrActiveByProtocol)
  formation_model.FormationBuyTrainPointDelegate:RemoveListener(t.RefreshBuyTrainPointByProtocol)
  formation_model.FormationUpgradeDelegate:RemoveListener(t.RefreshonUpgradeTrainFormationByProtocol)
  formation_model.onRecoverTimeUpdateDelegate:RemoveListener(t.RefreshTrainPointRecover)
end

function t.OnViewReady()
  Observers.Facade.Instance:SendNotification(string.format("%s::%s", PREFAB_PATH, "OnViewReady"))
end

function t.InitComponent()
  
  t.scrollContent = t.transform:Find('core/formation_list_frame/img_inner_frame/Scroll View/Viewport/Content'):GetComponent('ScrollContentExpand')
  t.scrollContent:AddResetItemListener(t.onResetItemHandler)
  local right_frame = t.transform:Find('core/img_right_frame')
  local formation_root = right_frame:Find('info/img_formation_root')
  t.textTrainPoint = right_frame:Find('img_top_bar/text_train_point'):GetComponent(typeof(Text))
  t.btnAddTrainPoint = right_frame:Find('img_top_bar/btn_add_train_point'):GetComponent(typeof(Button))
  t.btnAddTrainPoint.onClick:AddListener(t.ClickAddTrainPointHandler)
  t.textTrainPointRecover = right_frame:Find('img_top_bar/text_recover'):GetComponent(typeof(Text))
  
  t.goPosList = {}
  for i = 1,9 do
    t.goPosList[i] = formation_root:Find('pos/image'..i).gameObject
  end
  t.textFormationName = right_frame:Find('info/text_formation_name'):GetComponent(typeof(Text))
  t.textFormationLevel = right_frame:Find('info/text_formation_level'):GetComponent(typeof(Text))
  t.textFormationState = right_frame:Find('info/text_formation_state'):GetComponent(typeof(Text))
  --培养条件
  t.btnTraining = right_frame:Find('btn_train'):GetComponent(typeof(Button))
  t.btnTraining.onClick:AddListener(t.ClickTrainHandler)
  t.btnUse = right_frame:Find('btn_use'):GetComponent(typeof(Button))
  t.btnUse.onClick:AddListener(t.ClickUseHandler)
  t.tranUpgradeCondition = t.btnTraining.transform:Find('upgrade_condition')
  t.imgTrainCost = t.tranUpgradeCondition:Find('image_cost1'):GetComponent(typeof(Image))
  t.textTrainCost = t.tranUpgradeCondition:Find('text_cost1'):GetComponent(typeof(Text))
  t.textTrainPointCost =  t.tranUpgradeCondition:Find('text_cost2'):GetComponent(typeof(Text))
  --lock condition
  t.goLockCondition = right_frame:Find('unlock_condition').gameObject
  t.textLockCondition = t.goLockCondition.transform:Find('text_formation_unlock_tip'):GetComponent(typeof(Text))
  --attr
  t.textFormationEffectDes = right_frame:Find('text_formation_effect_des'):GetComponent(typeof(Text))
  t.textFormationAdditionEffectDes = right_frame:Find('text_addition_formation_attr_des'):GetComponent(typeof(Text))
  t.btnFormationAddition = right_frame:Find('btn_addition_formation_attr_active'):GetComponent(typeof(Button))
  t.btnFormationAddition.onClick:AddListener(t.ClickFormationAdditionHandler)
end

function t.InitScrollContent()
  local index = 1
  local count = 0
  t.formationInfoList = {}
  for k,v in pairs(formation_model.formationInfoTable) do
    t.formationInfoList[index] = v
    index = index + 1
    count = count + 1
  end
  table.sort(t.formationInfoList,function(a,b)
      return a.id < b.id
    end)
  t.scrollContent:Init(count,t.isFirstEnter,0)
  t.isFirstEnter = false
end
function t.Refresh()
  t.RefreshScrollContent()
  t.RefreshFormation()
end
function t.RefreshScrollContent()
  t.scrollContent:RefreshAllContentItems()
end
--刷新阵型数据
function t.RefreshFormation()
  local formationState = nil 
  if t.selectFormationInfo.id == t.currentInUseId then
    formationState = FormationState.InUse
  else
    formationState = t.selectFormationInfo.formationState
  end
  for i = 1,9 do
    t.goPosList[i]:SetActive(t.selectFormationInfo.formationData.pos[i])
  end
  t.textFormationName.text = LocalizationController.instance:Get(t.selectFormationInfo.formationData.name)
  t.textFormationLevel.text = string.format(LocalizationController.instance:Get("ui.train_formation_view.formation_level_text"), t.selectFormationInfo.level)
  t.textFormationState.text = t.GetFormationStateString(formationState)
  --培养条件
  t.RefreshCondition()
  --btn
  local isLock = formationState == FormationState.Locked
  t.btnTraining.gameObject:SetActive(not isLock)
  t.btnUse.gameObject:SetActive(not isLock)
  ui_util.SetGrayChildren(t.btnUse.transform,formationState == FormationState.InUse,true)
  t.btnFormationAddition.gameObject:SetActive(t.selectFormationInfo:HasAdditionAttr() and not t.selectFormationInfo.isActiveAdditionAttr)
  ui_util.SetImageGray(t.btnFormationAddition.image,isLock)
  --attr
  t.RefreshEffectAttrAdd()
  --lock condition
  t.goLockCondition:SetActive(isLock)
  if isLock then
    local text = ''
    for k,v in pairs(t.selectFormationInfo.formationData.formationConditionIdsList) do
      local conditionData = gamemanager.GetData('task_condition_data').GetDataById(v)
       text = text .. gamemanager.GetModel('task_model').GetTaskConditionDescriptionWithColor(conditionData) .. '\n'
    end
    t.textLockCondition.text = text
  end
end
--刷新培养条件
function t.RefreshCondition() 
  local UpgradeResCost = t.selectFormationInfo:UpgradeResCost()
  t.imgTrainCost.sprite = ResMgr.instance:LoadSprite(ui_util.GetBaseResIconPath(UpgradeResCost.type))
  t.imgTrainCost:SetNativeSize()
  local ownCost = game_model.GetBaseResourceValue(UpgradeResCost.type)
  if ownCost < UpgradeResCost.count then
    t.textTrainCost.text = ui_util.FormatToRedText(UpgradeResCost.count)
  else
    t.textTrainCost.text = UpgradeResCost.count
  end
  local needCost = t.selectFormationInfo:UpgradeTrainPointCost()
  if formation_model.trainPoint < needCost then
    t.textTrainPointCost.text = ui_util.FormatToRedText(needCost)
  else
    t.textTrainPointCost.text = needCost
  end
end
--阵型加成属性
function t.RefreshEffectAttrAdd()
  local attrDataList = gamemanager.GetData('formation_attr_data').GetFormationDatas(t.selectFormationInfo.id,t.selectFormationInfo.level)
  local tip = ''
  local additionTip = ''
  for k,v in pairs(attrDataList) do
    local nextEffectString = ''
    local duringTimeString = ''
    if (not t.selectFormationInfo:IsMax() or t.selectFormationInfo:isFollowAccountLevel()) and v.type == 1 then
      nextEffectString = string.format(LocalizationController.instance:Get("ui.train_formation_view.nextLevelEffect"),v:GetEffectAttrValue(t.selectFormationInfo.level+1) * 100)
    end
    if v.time ~= 0 then
      duringTimeString = string.format(LocalizationController.instance:Get("ui.train_formation_view.duringTime"), v.time)
    end
    local temp = ''
    if v.effectType == FormationEffectType.TreatPercent then
      temp = string.format(LocalizationController.instance:Get("ui.train_formation_view.effect"..v.effectType),v.interval,v:GetEffectAttrValue(t.selectFormationInfo.level) * 100,duringTimeString..nextEffectString)
    else
      temp = string.format(LocalizationController.instance:Get("ui.train_formation_view.effect"..v.effectType),v:GetEffectAttrValue(t.selectFormationInfo.level) * 100,duringTimeString..nextEffectString)
    end
    if v.type == 2 then--花钱激活属性
      additionTip = temp
    else
      tip = tip .. temp
    end
  end
  t.textFormationEffectDes.text = tip
  if t.selectFormationInfo.isActiveAdditionAttr then
    t.textFormationAdditionEffectDes.text = additionTip
  else
    t.textFormationAdditionEffectDes.text = string.format('<color=#ff0000>%s</color>',additionTip)
  end
  
end

--状态
function t.GetFormationStateString(formationState)
  if formationState == FormationState.InUse then
    return LocalizationController.instance:Get("ui.train_formation_view.formation_state_use")
  elseif formationState == FormationState.NotInUse then
    return LocalizationController.instance:Get("ui.train_formation_view.formation_state_noUse")
  else
    return LocalizationController.instance:Get("ui.train_formation_view.formation_state_lock")
  end
  
end
function t.RefreshTrainPointRecover()
  t.textTrainPoint.text = string.format('%d/%d',formation_model.trainPoint,formation_model.trainPointRecoverMax)
  if formation_model.trainPointNextRecoverCountDown == -1 then
    t.textTrainPointRecover.gameObject:SetActive(false)
  else
    t.textTrainPointRecover.gameObject:SetActive(true)
    t.textTrainPointRecover.text = string.format('(%s)',TimeUtil.FormatSecondToMinute(math.floor(formation_model.trainPointNextRecoverCountDown/1000)))
  end
end
function t.OpenBuyTrainPointView()
  local price = formation_model.GetTrainPointPurcase(formation_model.bringUpPointPurcasedTimes)
  local remainTimes = formation_model.TrainPointPurcasedMaxCount() - formation_model.bringUpPointPurcasedTimes
  if remainTimes <= 0 then
    local nextImproveFormationAddTimesVIPData = nil
    local currentVIPData = vip_model.vipData
    local tempVIPData = currentVIPData
    while (tempVIPData ~= nil) do
      if tempVIPData.formation_add > currentVIPData.formation_add then
        nextImproveFormationAddTimesVIPData = tempVIPData
        break
      end
      tempVIPData = tempVIPData:GetNextLevelVIPData ()
    end
    if nextImproveFormationAddTimesVIPData ~= nil then
      local tipsString = string.format(LocalizationController.instance:Get('ui.train_buy_point_view.buy_times_not_enough_and_go_to_buy_diamond'), nextImproveFormationAddTimesVIPData:ShortName())
      confirm_tip_view.Open(tipsString, t.GoToBuyDiamond)
    else
      auto_destroy_tip_view.Open(LocalizationController.instance:Get('ui.train_buy_point_view.buyNotEnough'))
    end
    return
  end
  --if gamemanager.GetModel('consume_tip_model').GetConsumeTipEnable(ConsumeTipType.DiamondBuyFormationTrainPoint) then
    confirm_buy_train_point_view.Open(price,gamemanager.GetData('global_data').for_coin_buy_num,remainTimes,t.ClickConfirmBuyPoint)
 -- else
  --  t.ClickConfirmBuyPoint()
 -- end
end

function t.GoToBuyDiamond ()
  gamemanager.GetModel('function_open_model').OpenFunction(FunctionOpenType.MainView_Shop, 3)
end

--确认购买
function t.ClickConfirmBuyPoint()
  print('确认购买')
  formation_controller.LineupPointBuyReq()
end
-----------------------click event---------------------

function t.onResetItemHandler(go ,index)
  local item = t.scrollItems[go]
  if item == nil then
    item = formation_button.BindGameObject(go)
    item.onClick:AddListener(t.ClickItemHandler)
    t.scrollItems[go] = item
  end
  local formationInfo = t.formationInfoList[index+1]
  local formationState = nil 
  if formationInfo.id == t.currentInUseId then
    formationState = FormationState.InUse
  else
    formationState = formationInfo.formationState
  end
  item:SetInfo(formationInfo,formationState)
  item:SetSelect(formationInfo.id == t.selectFormationInfo.id)
end

--点击阵型item
function t.ClickItemHandler(formationButton)
  t.selectFormationInfo = formationButton.formationInfo
  t.Refresh()
end

--购买阵型点
function t.ClickAddTrainPointHandler()
  t.OpenBuyTrainPointView()
end
------培养
function t.ClickTrainHandler()
  if t.selectFormationInfo:IsMax() then
    if t.selectFormationInfo:isFollowAccountLevel() then
      auto_destroy_tip_view.Open(LocalizationController.instance:Get("ui.train_buy_point_view.limitAccountLevel"))
    else
      auto_destroy_tip_view.Open(LocalizationController.instance:Get("ui.train_buy_point_view.limitLevel"))
    end
    return
  end
  --培养点不足
  if t.selectFormationInfo:UpgradeTrainPointCost() > formation_model.trainPoint then
    local remindTimes = formation_model.TrainPointPurcasedMaxCount() - formation_model.bringUpPointPurcasedTimes
    if remindTimes == 0 then--购买次数不足
      auto_destroy_tip_view.Open(LocalizationController.instance:Get("ui.train_buy_point_view.trainPointNotEnough"))
      return
    end
    t.OpenBuyTrainPointView()
    return
  end
  local costRes = t.selectFormationInfo:UpgradeResCost()
  if costRes.type == BaseResType.Gold or costRes.type == BaseResType.Diamond then
    if not game_model.CheckBaseResEnoughByType(costRes.type,costRes.count) then
      return 
    end
  end
  formation_controller.LineupUpgradeReq(t.selectFormationInfo.id)
end
--使用阵型
function t.ClickUseHandler()
  if t.selectFormationInfo.id == t.currentInUseId then
    return 
  end
  t.currentInUseId = t.selectFormationInfo.id
  formation_model.GetFormationTeam(t.formationTeamType):ChangeFormationPosByFormationId(t.currentInUseId)
  formation_controller.TeamChangeReq(t.formationTeamType)
  t.Refresh()
end
--激活牛逼附加属性
function t.ClickFormationAdditionHandler()
  if t.selectFormationInfo.formationState ~= FormationState.Locked then
    local unlock_cost = t.selectFormationInfo.formationData.unlock_cost
    confirm_cost_tip_view.Open(unlock_cost.type,unlock_cost.count,LocalizationController.instance:Get("ui.train_formation_view.addition_formation_active_tips"),
      function()
        if game_model.CheckBaseResEnoughByType(unlock_cost.type,unlock_cost.count) then
          formation_controller.LineupAttrActiveReq(t.selectFormationInfo.id)
        end
      end,
      ConsumeTipType.None)
    
  end
  
end
--------------------------------update by protocol ------------------------
function t.FormationAdditionAttrActiveByProtocol()
  t.RefreshFormation()
  auto_destroy_tip_view.Open(LocalizationController.instance:Get("ui.train_formation_view.addition_formation_active_success_tips"))
  --talkingData--
  local unlock_cost = t.selectFormationInfo.formationData.unlock_cost
  TalkingDataController.instance:TDGAItemOnPurchaseByCount("阵型附加属性",1,unlock_cost.count)
  --end--
end

function t.RefreshBuyTrainPointByProtocol()
  t.RefreshTrainPointRecover()
  t.RefreshCondition()
  --talkingData--
  local price = formation_model.GetTrainPointPurcase(formation_model.bringUpPointPurcasedTimes-1)--次数已经加过了，所以要减一
  TalkingDataController.instance:TDGAItemOnPurchase("阵型",BaseResType.FormationTrainPoint,0,gamemanager.GetData('global_data').for_coin_buy_num,price)
  --end--
end
function t.RefreshonUpgradeTrainFormationByProtocol()
  t.Refresh()
  
  --talkingData--
  local UpgradeResCost = t.selectFormationInfo:UpgradeResCost()
  if UpgradeResCost.type == BaseResType.Diamond then
     TalkingDataController.instance:TDGAItemOnPurchaseByCount("阵型升级",1,UpgradeResCost.count)
  end
  --end--
end


return t