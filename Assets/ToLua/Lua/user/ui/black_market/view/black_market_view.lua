local t = {}
local PREFAB_PATH = 'ui/black_market/black_market_view1'

local black_market_model = gamemanager.GetModel('black_market_model')
local black_market_controller = gamemanager.GetModel('black_market_controller')
local item_model = gamemanager.GetModel('item_model')
local game_model = gamemanager.GetModel('game_model')
local consume_tip_model = gamemanager.GetModel('consume_tip_model')

local black_market_data = gamemanager.GetData('black_market_data')
local goods_button = require('ui/black_market/view/goods_button')
local common_item_icon = require('ui/common_icon/common_item_icon')
local common_error_tip_view = require('ui/tips/view/common_error_tips_view')
local reward_auto_destroy_tip_view = require('ui/tips/view/reward_auto_destroy_tip_view')
local common_reward_tips_view = require('ui/tips/view/common_reward_tips_view')
local confirm_cost_tip_view = require('ui/tips/view/confirm_cost_tip_view')
local item_data = gamemanager.GetData('item_data')
local global_data = gamemanager.GetData('global_data')

function t.Open(toggleIndex)
  local gameObject = UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  local common_top_bar = require ('ui/common_top_bar/common_top_bar')
  common_top_bar = common_top_bar:Create(t.transform:Find('core'))
  common_top_bar:SetAsCommonStyle(LocalizationController.instance:Get("ui.black_market_view.title"),t.OnClickBackBtnHandler,true,true,true,true,false,false,false)
  
  t.toggles = {}
  t.selectToggleIndex = toggleIndex
  t.marketInfoList = {}
  t.selectBlackMarketInfo = nil
  t.scrollItems = {}
  t.currentItemIcon = nil
  t.isFirstEnter = true
  t.sortMarketType = {} --商店按照global表里排序显示(index,marketType)
   
   
  t.functionOpenTypeTable = {}
  t.functionOpenTypeTable[1] = FunctionOpenType.MainView_BlackMarket
  t.functionOpenTypeTable[2] = FunctionOpenType.BlackMarket_Arena
  t.functionOpenTypeTable[3] = FunctionOpenType.BlackMarket_Expedition
  t.functionOpenTypeTable[4] = FunctionOpenType.BlackMarket_WorldBoss
  t.functionOpenTypeTable[5] = FunctionOpenType.MainView_BlackMarket
  t.functionOpenTypeTable[6] = FunctionOpenType.BlackMarket_PvpRace
  if t.refreshCoroutine then
    coroutine.stop(t.refreshCoroutine)          
  end
  t.refreshCoroutine = nil
   
  t.BindDelegate()
  t.InitComponent()
  
  ---send msg
  black_market_controller.BlackMarketReq()
  t.refreshCoroutine = coroutine.start(t.RefreshTimeCoroutine)
end
function t.Close()
  UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
  if t.refreshCoroutine then
    coroutine.stop(t.refreshCoroutine)
    t.refreshCoroutine = nil
  end
end

function t.BindDelegate()
  black_market_model.onInitBlackMarketDelegate:AddListener(t.InitBlackMarketByProtocol)
  black_market_model.onUpdateBlackMarketDelegate:AddListener(t.UpdateBlackMarketByProtocol)
  black_market_model.onUpdatePurchaseGoodsSuccessDelegate:AddListener(t.OnUpdatePurchaseSuccessByProtocol)
  item_model.updateItemInfoListDelegate:AddListener(t.RefreshAll)
  game_model.onUpdateBaseResourceDelegate:AddListener(t.RefreshAll)
end
function t.UnbindDelegate()
  black_market_model.onInitBlackMarketDelegate:RemoveListener(t.InitBlackMarketByProtocol)
  black_market_model.onUpdateBlackMarketDelegate:RemoveListener(t.UpdateBlackMarketByProtocol)
  black_market_model.onUpdatePurchaseGoodsSuccessDelegate:RemoveListener(t.OnUpdatePurchaseSuccessByProtocol)
  item_model.updateItemInfoListDelegate:RemoveListener(t.RefreshAll)
  game_model.onUpdateBaseResourceDelegate:RemoveListener(t.RefreshAll)
end

function t.InitComponent()
  t.goTogglePrefab = t.transform:Find('core/root/toggle_prefab').gameObject
  t.goTogglePrefab:SetActive(false)
  t.tranToggleRoot = t.transform:Find('core/root/Scroll_toggle/Viewport/Content')
  
  local right_root = t.transform:Find('core/root/img_frame/right_root')
  t.scrollContent = right_root:Find('ItemScrollView/Viewport/Content'):GetComponent(typeof(Common.UI.Components.ScrollContentExpand))
  t.scrollContent:AddResetItemListener(t.ResetItemHandler)
  t.textRefreshTime = right_root:Find('img_bg/text_time'):GetComponent(typeof(Text))
  t.btnRefresh = right_root:Find('img_bg/btn_refresh'):GetComponent(typeof(Button))
  t.btnRefresh.onClick:AddListener(t.ClickRefreshBtnHandler)
  t.imgCurResIcon = ui_util.FindComp(right_root,'img_bg/img_res_icon',Image)
  t.textCurResCount = ui_util.FindComp(right_root,'img_bg/text_res_count',Text)
  --
  
end
function t.InitToggles()
  local go = nil
  local tran = nil
  
 
  for k,v in pairs(global_data.black_market_sort_list) do
    for k2,v2 in pairs(black_market_data.marketTypeList) do
      if v == k2 then
        t.sortMarketType[k] = v
        break
      end
    end
  end
  
  for index,marketType in pairs( t.sortMarketType) do
    go = GameObject.Instantiate(t.goTogglePrefab)
    go:SetActive(true)
    tran = go:GetComponent(typeof(Transform))
    tran:SetParent(t.tranToggleRoot,false)
    
    tran:GetComponent(typeof(ToggleContent)):Set(marketType,LocalizationController.instance:Get(black_market_data.marketTypeList[marketType].name))
    tran:GetComponent(typeof(EventTriggerDelegate)).onClick:AddListener(t.ClickToggleHandler)
    t.toggles[marketType] = tran:GetComponent(typeof(Toggle))
  end
  
  if not gamemanager.GetModel('function_open_model').IsFunctionOpen(t.functionOpenTypeTable[t.selectToggleIndex],true) then
    t.selectToggleIndex = t.sortMarketType[1]
  end
  if t.toggles[t.selectToggleIndex] then
    t.toggles[t.selectToggleIndex].isOn = true
  end
  for k,v in pairs(t.toggles) do
    v.transform:Find('img_icon'):GetComponent(typeof(Image)).sprite = ResMgr.instance:LoadSprite('sprite/main_ui/'..black_market_data.marketTypeList[k].icon)
  end
end
function t.InitScrollContent(resetSelect)
  t.marketInfoList = black_market_model.GetMarketInfoListByType(t.selectToggleIndex)
  if t.marketInfoList == nil then
    Debugger.LogError('not found marketInfoList ,so change to show first toggle ,Toggle id is :'..t.selectToggleIndex)
    t.toggles[t.selectToggleIndex].isOn = false 
    t.selectToggleIndex = 1
    t.toggles[t.selectToggleIndex].isOn = true 
    t.marketInfoList = black_market_model.GetMarketInfoListByType(t.selectToggleIndex)
    
  end
  local count = table.count(t.marketInfoList)
  t.scrollContent:Init(count,t.isFirstEnter,0)
  t.isFirstEnter = false
  
end

function t.RefreshContentItems()
  t.scrollContent:RefreshAllContentItems()
end

function t.RefreshAll()
  t.Refresh(true)
end

function t.Refresh(needRefreshScrollContent)
  if needRefreshScrollContent then
    t.RefreshContentItems()
  end
  t.UpdateRefreshButton()
  local blackMarketInfo = t.marketInfoList[1]
  if blackMarketInfo then
    local mat = blackMarketInfo:GetMaterials()[1]
    local path = ''
    local count = 0
    if mat.type == BaseResType.Item then
      path = item_data.GetDataById(mat.id):IconPath()
      count = item_model.GetItemCountByItemID(mat.id)
    else
      path = ui_util.GetBaseResIconPath(mat.type)
      count = game_model.GetBaseResourceValue(mat.type)
    end
    t.imgCurResIcon.sprite = ResMgr.instance:LoadSprite(path)
    t.textCurResCount.text = count
  end
  
end
function t.UpdateRefreshButton()
  local endTime = black_market_model.GetMarketRefreshTime(t.selectToggleIndex)
  if endTime ~= nil and endTime ~= -1 then
    t.textRefreshTime.gameObject:SetActive(true)
  else
    t.textRefreshTime.gameObject:SetActive(false)
  end
  t.btnRefresh.gameObject:SetActive( black_market_data.marketTypeList[t.selectToggleIndex].manual_refresh ~= 0 )
    
  
end
function t.RefreshTimeCoroutine()
  while true do
    t.RefreshTime()
    coroutine.wait(1)
  end
end
function t.RefreshTime()
    local endTime = 0
    endTime = black_market_model.GetMarketRefreshTime(t.selectToggleIndex) 
    if endTime ~= nil and endTime ~= -1 then
      local remaindTime = math.floor(endTime) - TimeController.instance.ServerTimeTicksSecond*1000
      if remaindTime <= 0 then
        black_market_controller.BlackMarketReq()
        remaindTime  = 0
      end
      t.textRefreshTime.text = string.format( LocalizationController.instance:Get("ui.black_market_view.exchangeTime"), TimeUtil.FormatSecondToHour(math.floor(remaindTime/1000)))
    end
end

----------------------click event------------------------
function t.OnClickBackBtnHandler()
  t.Close()
end
function t.ClickToggleHandler(go)
  local index = 1
  for k,v in pairs(t.toggles) do
    if v.gameObject == go then
      index = k
      break
    end
  end
  if t.selectToggleIndex == index then
    return
  end
  if not gamemanager.GetModel('function_open_model').IsFunctionOpen(t.functionOpenTypeTable[index],true) then
    t.toggles[t.selectToggleIndex].isOn = true
    t.toggles[index].isOn = false
    return
  end
  t.selectToggleIndex = index
  t.InitScrollContent(true)
  t.Refresh(false)
  t.RefreshTime()
end
function t.ResetItemHandler(go,index)
  if not t.scrollItems[go] then
    t.scrollItems[go] = goods_button.BindGameObject(go)
    t.scrollItems[go].onClick:AddListener(t.OnClickBuySureHandler)
  end
  local info = t.marketInfoList[index +1]
  t.scrollItems[go]:SetData(info,t.selectToggleIndex == 1)
end


function t.ClickRefreshBtnHandler()
  local cost = black_market_model.GetRefreshCost(t.selectToggleIndex)
  confirm_cost_tip_view.OpenByThreeParam(BaseResType.Diamond,cost,t.ConfirmClickRefreshHandler)
  
end
function t.ConfirmClickRefreshHandler()
  local cost = black_market_model.GetRefreshCost(t.selectToggleIndex)
  if game_model.GetBaseResourceValue(BaseResType.Diamond) >= cost then
    black_market_controller.RefreshBlackMarketReq(t.selectToggleIndex)
  else
    common_error_tip_view.Open(LocalizationController.instance:Get("ui.black_market_view.DiamondNotEnough"))
  end
end

function t.OnClickBuySureHandler(blackmarketInfo)
  t.selectBlackMarketInfo = blackmarketInfo
  black_market_controller.PurchaseBlackGoodsReq(t.selectToggleIndex,blackmarketInfo.id)
  
  
end

---------------------update by protocol--------------
function t.InitBlackMarketByProtocol()
   t.InitToggles()
  if  black_market_model.GetMarketInfoCount(BlackmarketType.limitActivity) == 0 then
    t.toggles[BlackmarketType.limitActivity].gameObject:SetActive(false)
  end
 
  t.InitScrollContent(true)
  t.Refresh(false)
end
function t.UpdateBlackMarketByProtocol()
  t.InitScrollContent(false)
  t.Refresh(false)
end
function t.OnUpdatePurchaseSuccessByProtocol()
  print('购买成功')
  local material = t.selectBlackMarketInfo:GetMaterials()[1]
  local item = t.selectBlackMarketInfo:GetItemResData()
  local list = {}
  list[1] = item
  common_reward_tips_view.Create(list)
  if material.type == BaseResType.Diamond then
    local cost = t.selectBlackMarketInfo:GetRisePrice()    
    TalkingDataController.instance:TDGAItemOnPurchase("黑市",item.type,item.id,item.count,cost)
  end
end
return t