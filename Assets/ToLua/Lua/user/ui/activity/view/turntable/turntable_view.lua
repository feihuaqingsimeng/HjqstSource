local t = {}
local PREFAB_PATH = 'ui/activity/turntable/turntable_view'
local name = PREFAB_PATH

local event_data = gamemanager.GetData('event_data')
local game_res_data = require('ui/game/model/game_res_data')
local common_reward_icon = require('ui/common_icon/common_reward_icon')
local turntable_data = gamemanager.GetData('turntable_data')
local turntable_model = gamemanager.GetModel('turntable_model')
local game_model = gamemanager.GetModel('game_model')
local common_reward_tips_view = require('ui/tips/view/common_reward_tips_view')
local activity_controller = gamemanager.GetCtrl('activity_controller')
local consume_tip_model = gamemanager.GetModel('consume_tip_model')
local consumeTipData = gamemanager.GetData('consume_tip_data')

local effect_stay_name = 'effects/prefabs/ui_zhuanpan_stop'
local effect_work_name = 'effects/prefabs/ui_zhuanpan_work'
local effect_end_name = 'effects/prefabs/ui_zhuanpan_end'
local go_effect_stay = nil
local go_effect_work = nil
local go_effect_end = nil

function t.Open()
  if uimanager.GetView(name) then
    Debugger.LogError('view is already open------------------------')
    return 
  end
  uimanager.RegisterView(name,t)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  t.isClickStartAction = false
  t.eventData = gamemanager.GetData('event_data').GetTurntableData()
  t.scrollItems = {}
  t.gainRewardList = {}
  t.isFirstEnter = true
  
  t.BindDelegate()
  t.InitComp()
  t.Init()
  
  --请求转盘信息
  t.coroutineReqTurntableInfo = coroutine.start(t.ReqTurntableInfoCoroutine)
  t.ChangeParticleStatus(0)
end

function t.Close()
  t.transform = nil
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
  coroutine.stop(t.coroutineReqTurntableInfo)
end
function t.BindDelegate()
  activity_controller.OnTurntableInfoUpdateDelegate:AddListener(t.RefreshByProtocol)
  activity_controller.OnturntableDrawSucDelegate:AddListener(t.DrawSucByProtol)
end
function t.UnbindDelegate()
  activity_controller.OnTurntableInfoUpdateDelegate:RemoveListener(t.RefreshByProtocol)
  activity_controller.OnturntableDrawSucDelegate:RemoveListener(t.DrawSucByProtol)
end

function t.InitComp()
  --right
  t.canvas = t.transform:GetComponentInParent(typeof(UnityEngine.Canvas))
  local right = ui_util.FindComp(t.transform,'core/frame/right',Transform)
  t.tranArrow = ui_util.FindComp(right,'img_turntable/img_arrow',Transform)
  t.btnStartLottery = ui_util.FindComp(right,'img_turntable/btn_start',Button)
  t.btnStartLottery.onClick:AddListener(t.ClickStartLotteryHandler)
  t.tranRewardRoot = ui_util.FindComp(right,'img_turntable/award_root',Transform)
  ui_util.FindComp(right,'img_title/btn_rule',Button).onClick:AddListener(t.ClickCheckRuleHandler)
  t.textOpenTime = ui_util.FindComp(right,'img_title/text_open_time',Text)
  t.textFreeCount = ui_util.FindComp(right,'img_title/text_free_count',Text)
  t.textAlreadyBuyCountTip = ui_util.FindComp(right,'text_times_tip',Text)
  t.btnTenDraw = ui_util.FindComp(right,'btn_more_draw',Button)
  t.btnTenDraw.onClick:AddListener(t.ClickTenDrawHandler)
  t.textTenDraw = ui_util.FindComp(t.btnTenDraw.transform,'text_count',Text)
  t.particleRoot = ui_util.FindComp(t.transform,'core/frame/right/img_turntable/particles_root',Transform)
  
  --left
  local left = ui_util.FindComp(t.transform,'core/frame/left',Transform)
  ui_util.FindComp(t.transform,'core/btn_close',Button).onClick:AddListener(t.Close)
  t.scrollContent = ui_util.FindComp(left,'Scroll View/Viewport/Content',ScrollContentExpand)
  t.scrollContent:AddResetItemListener(t.OnResetItemHandler)
end
function t.Init()
  t.turntableCount = 8--转盘奖励数量
  t.arrowOriginRotate = 22.5--初始箭头转角
  t.lotteryChoiceIndex = 1 -- 抽中index
  --local zRotate = t.GetMidRotate(t.lotteryChoiceIndex)
 -- t.tranArrow.localRotation = Vector3(0,0,90)
 
  t.InitReward()
  t.Refresh()
end


--初始化奖励
function t.InitReward()
  local rewardList = turntable_data.GetRewardDatas()
  local r = 140--长度
  local scale = 0.7
  local midPos = Vector3(0,0,0)
  for k,v in ipairs(rewardList) do
    local rotate = t.GetMidRotate(k)
    local pos 
    pos = Vector3(-r*math.sin(rotate/180*3.1415),r*math.cos(rotate/180*3.1415),0)
    local icon = common_reward_icon.New(t.tranRewardRoot,v)
    icon.transform.gameObject.name = k
    icon.transform.localPosition = pos
    icon.transform.localScale = Vector3(scale,scale,1)
  end
  
end

function t.Refresh()
  local time1,time2 = uimanager.DealWithTime(t.eventData.event_timestart, t.eventData.event_timeover)
  t.textOpenTime.text = string.format(LocalizationController.instance:Get('ui.turntable_view.openTime'),time1,time2)
  local freeCount = turntable_model.GetFreeCount()
  if freeCount > 0 then
    t.textFreeCount.text = string.format(LocalizationController.instance:Get('ui.turntable_view.freeTimes'),turntable_model.GetFreeCount())
  else
    t.textFreeCount.text = string.format(LocalizationController.instance:Get('ui.turntable_view.oneDrawCostTip'),turntable_model.GetOneCostDiamond())
  end
  
  local nextNeed,discount = turntable_model.GetNextNeedCountAndDiscount()
  if nextNeed then
    t.textAlreadyBuyCountTip.text = string.format(LocalizationController.instance:Get('ui.turntable_view.discountTip'),turntable_model.drawCount,nextNeed,discount)
  else
    t.textAlreadyBuyCountTip.text = string.format(LocalizationController.instance:Get('ui.turntable_view.discountTip2'),turntable_model.drawCount)
  end
  t.textTenDraw.text = turntable_model.GetTenCostDimaond()
  
end


--获取奖励下标对应的旋转角度degree（逆时针为正）index 从1开始
function t.GetMidRotate(index)
  return -(360/t.turntableCount * (index-1) + t.arrowOriginRotate)
end

--开始转啦
function t.StartTurntableAction(index)
  t.lotteryChoiceIndex = index -- 抽中index
  local rotate = t.GetMidRotate(index) 
  t.zRotate = rotate
  local originRotate =  t.tranArrow.localEulerAngles.z-360 * (math.floor(t.tranArrow.localEulerAngles.z/360) + 1)
  if t.zRotate >= originRotate then
    t.zRotate = t.zRotate - originRotate-360
  else
    t.zRotate = t.zRotate - originRotate
  end
  print('rotate',rotate,'z',t.tranArrow.localEulerAngles.z,'originRotate',originRotate,'chioseIndex',t.lotteryChoiceIndex)
  t.zRotate = t.zRotate - 360*8 + UnityEngine.Random.Range(-99,99)/100*22
  
  local rotate1 = -120--120度加速
  local time1 = 0.3
  t.zRotate = t.zRotate - rotate1
  LeanTween.rotateAroundLocal(t.tranArrow.gameObject,Vector3.forward,rotate1,time1):setEase(LeanTweenType.easeInCirc):setOnComplete(Action(t.accelerateCompleteHandler))
  
end
--加速完成，进入匀速
function t.accelerateCompleteHandler()
  local rotate2 = t.zRotate + 360--留360度进入减速状态
  local time2 = 2
  t.zRotate = t.zRotate - rotate2
  LeanTween.rotateAroundLocal(t.tranArrow.gameObject,Vector3.forward,rotate2,time2):setOnComplete(Action(t.uniformCompleteHandler))
end
--匀速完成，进入减速
function t.uniformCompleteHandler()
  local rotate3 = t.zRotate
  local time3 = 3
  LeanTween.rotateAroundLocal(t.tranArrow.gameObject,Vector3.forward,rotate3,time3):setEase(LeanTweenType.easeOutCirc):setOnComplete(Action(t.TurntableActionCompleteHandler))
end
--动画完成
function t.TurntableActionCompleteHandler()
  
  local dataList = {}
  dataList[1] = turntable_data.GetRewardDataByIndex(t.lotteryChoiceIndex)
  
  t.ChangeParticleStatus(3)
  coroutine.start(function()
      coroutine.wait(1)
      
      if t.transform == nil then return end
      
      t.ChangeParticleStatus(0)
      common_reward_tips_view.Create(t.gainRewardList)
      t.isClickStartAction = false
    end)
  
end


function t.InitScrollContent()
  local value = 0
  t.scrollContent:RefreshCount(turntable_model.turntableRankDictionary.Count, -1)

  t.isFirstEnter = false
end
function t.RefreshScrollContent()
  t.scrollContent:RefreshAllContentItems()
end
--------------------------click event-------------------------
--抽奖
function t.ClickStartLotteryHandler()
  t.ConfirmDraw(false)
end
--规则
function t.ClickCheckRuleHandler()
  LuaCsTransfer.OpenCommonRuleTipsView(LocalizationController.instance:Get('ui.turntable_view.ruleTitle'),LocalizationController.instance:Get('ui.turntable_view.des'))
end
function t.OnResetItemHandler(go,index)
  local item = t.scrollItems[go]
  if item == nil then
    item = require('ui/activity/view/turntable/rank_item').BindTransform(go.transform)
    t.scrollItems[go] = item
  end
  item:SetData(turntable_model.turntableRankDictionary:Get(index+1))
end
--十抽
function t.ClickTenDrawHandler()
  t.ConfirmDraw(true)
end
--抽奖啦
t.clickTenDraw = false
function t.ConfirmDraw(isTen)
  if t.isClickStartAction then
    print('please hold on second')
    return 
  end
  --是否开放
  local eventData = gamemanager.GetData('event_data').GetTurntableData()
    local start1 = System.DateTime.Parse(eventData.event_timestart)
    local end1 = System.DateTime.Parse(eventData.event_timeover)
    local serverTime = TimeController.instance.ServerTime
    start1 = (serverTime-start1).TotalSeconds
    end1 = (end1-serverTime).TotalSeconds
    if start1 >= 0 and end1 >= 0  and eventData.open == '1' then
      --在开放时间内
    else
      --不在开放时间内 
      require('ui/tips/view/common_error_tips_view').Open(LocalizationController.instance:Get('71701'))--活动未开放
      return
    end
  
  t.clickTenDraw = isTen
  local cost = 0
  if isTen then
    cost = turntable_model.GetTenCostDimaond()
  else
    cost = turntable_model.GetOneCostDiamond()
  end
  if (turntable_model.freeCount == 0 or isTen) and not game_model.CheckBaseResEnoughByType(BaseResType.Diamond,cost) then
    return
  end
  local addCount = 1
  if isTen then
    addCount = 10
  end
  if game_model.CheckPackFull(true,true,addCount,addCount) then
    return
  end
  
  if turntable_model.GetFreeCount() == 0 and consume_tip_model.GetConsumeTipEnable(ConsumeTipType.Turntable) then
    require('ui/tips/view/confirm_cost_tip_view').Open(BaseResType.Diamond,cost,LocalizationController.instance:Get('ui.confirm_cost_tip_view.cost_des'), t.ConfirmClickHandler,ConsumeTipType.Turntable)
  else
    t.ConfirmClickHandler()
  end
end
--消费提示确认
function t.ConfirmClickHandler()
  t.isClickStartAction = true
  t.ChangeParticleStatus(1)
  if t.clickTenDraw then
    activity_controller.UseLuckyRouletteTenReq()
  else
    activity_controller.UseLuckyLetteReq()
  end
end

--几秒刷新排行榜
function t.ReqTurntableInfoCoroutine()
  while(true) do
    activity_controller.LuckyLetteInfoReq()
    coroutine.wait(10)
  end
end

function t.ChangeParticleStatus(status)
  GameObject.Destroy(go_effect_stay)
  GameObject.Destroy(go_effect_work)
  GameObject.Destroy(go_effect_end)
  if status == 0 then     --待机
    go_effect_stay = particle_util.CreateParticleByCanvas(effect_stay_name,t.canvas,t.particleRoot)
  elseif status == 1 then -- 转盘转动
    go_effect_work = particle_util.CreateParticleByCanvas(effect_work_name,t.canvas,t.particleRoot)
  else                    --转盘转动后停止转动
    go_effect_end = particle_util.CreateParticleByCanvas(effect_end_name,t.canvas,t.particleRoot)
  end
end
------------------update by protocol --------------
function t.RefreshByProtocol()
  t.InitScrollContent()
  t.Refresh()
end
function t.DrawSucByProtol(rewardIdList)
  activity_controller.LuckyLetteInfoReq()--请求转盘信息
  local count = #rewardIdList
  local rewardList = {}
  for k,v in ipairs(rewardIdList) do
    rewardList[k] = turntable_data.GetRewardDataByIndex(v)
  end
  rewardList = ui_util.CombineGameResList(rewardList)
  if count == 1 then
    t.StartTurntableAction(rewardIdList[1])
    t.gainRewardList = rewardList
  else
    
    t.ChangeParticleStatus(3)
    coroutine.start(function()
        coroutine.wait(1)
        
        if t.transform == nil then return end
        
        t.ChangeParticleStatus(0)
        require('ui/tips/view/common_reward_tips_view').Create(rewardList)
        t.isClickStartAction = false
      end)
    
  end
  
  

end
return t