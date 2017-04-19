local t = {}
local name = 'game_model'

t.accountId = ''
t.accountLevel = 0
t.accountExp = 0
t.accountName = ''
t.accountHeadNo = 0 --账号头像
t.lastServerId = 0  --选择的服务器
t.serverName = '' --server name
t.channelId  = 0    --渠道id
t.platformAccountId = '' --渠道账号
t.playerInfo = nil
t.baseResourceTable = {}
--背包
t.heroCellNum = 0
t.heroCellBuyNum = 0
t.equipCellNum = 0
t.equipCellBuyNum = 0
--pve action
t.pveAction = 0
t.pveActionMax = 0
t.pveActionNextRecoverTime = 0--回复倒计时

--账号等级和经验更新no param
t.UpdateAccountLevelAndExpDelegate = void_delegate.New()
-- 英雄背包购买成功界面更新委托  no param
t.UpdateHeroCellNumDelegate = void_delegate.New()
-- 装备背包购买成功界面更新委托  no param
t.UpdateEquipCellNumDelegate = void_delegate.New()
--基础资源更新 no param
t.onUpdateBaseResourceDelegate = void_delegate.New()
--pve Action 更新 no param
t.onPveActionInfoUpdateDelegate = void_delegate.New()

local function Start ()
  gamemanager.RegisterModel(name, t)
  t.channelId = LuaInterface.LuaCsTransfer.GetChannelID()
  Observers.Facade.Instance:RegisterObserver("ui/main/main_view", t.MainViewOpenHandler)
  Observers.Facade.Instance:RegisterObserver("ui/server_list/server_list_view", t.ServerListViewOpenHandler)
end
--账号经验比例  最大1
function t.AccountExpPercent()
  local data = gamemanager.GetData('account_exp_data').GetDataById(t.accountLevel)
  if data then
    return t.accountExp/data.exp
  end
  return 0
end

--检查背包格子是否够(chu)
function t.CheckPackFull(isCheckHero,isCheckEquip,addHeroCount,addEquipCount)
  if addHeroCount == nil then
    addHeroCount = 0
  end
  if addEquipCount == nil then
    addEquipCount = 0
  end
  if isCheckHero and gamemanager.GetModel('hero_model').GetAllHeroCount() + addHeroCount >= t.heroCellNum then
    require('ui/tips/view/common_error_tips_view').Open(LocalizationController.instance:Get('ui.common_tips.hero_bag__reach_max'))
    return true
  end
  if isCheckEquip and gamemanager.GetModel('equip_model').GetFreeEquipmentCount() + addEquipCount >= t.equipCellNum then
    require('ui/tips/view/common_error_tips_view').Open(LocalizationController.instance:Get('ui.common_tips.equipment_bag_reach_max'))
    return true
  end
  return false
end
--检查基础资源是否充足 (目前只跳转 金币、宝石)，不够跳转页面   
--gameResDataTable为消耗资源[id不限，gameResData]
function t.CheckBaseResEnoughByDataTable(gameResDataTable)
  local flag = true
  for k,v in pairs(gameResDataTable) do
    flag = t.CheckBaseResEnoughByType(v.type,v.count)
    if not flag then
      return flag
    end
  end
  return flag
end
--检查基础资源是否充足 (目前只跳转 金币、宝石)，不够跳转页面   
function t.CheckBaseResEnoughByType(baseResType,costCount)
  
  local own = t.GetBaseResourceValue(baseResType)
  
  local confirm_tip_view = require('ui/tips/view/confirm_tip_view')
  local common_error_tip_view = require('ui/tips/view/common_error_tips_view')
  if baseResType == BaseResType.Gold and costCount > own then
    confirm_tip_view.Open(LocalizationController.instance:Get('ui.base_resource_jump_view.tip.content2'),function()
        --跳转点金手
        gamemanager.GetCtrl('golden_touch_controller').OpenGoldenTouchView()
      end)
    return false
  elseif baseResType == BaseResType.Diamond and costCount > own then
    confirm_tip_view.Open(LocalizationController.instance:Get('ui.base_resource_jump_view.tip.content1'),function()
        gamemanager.GetModel('function_open_model').OpenFunction(FunctionOpenType.MainView_Shop,3)
      end)
    return false
  elseif costCount > own then
    common_error_tip_view.Open(LocalizationController.instance:Get("ui.confirm_buy_item_tips_view.tip.notEnoughTitle"))
    return false
  end
  return true
end


--account id
function t.SetAccountId(accountId)
  t.accountId = accountId
end

--最大等级
function t.GetAccountMaxLevel()
  return gamemanager.GetData('global_data').account_lv_max
end

function t.IsAccountMaxLevel()
  return t.accountLevel >=  t.GetAccountMaxLevel()
end

function t.IsPlayer(instanceID)
  if t.playerInfo == nil then
    return false
  else 
    return instanceID == t.playerInfo.instanceID
  end

end

--设置服务器名字
function t.SetServerName(serverName)
  t.serverName = serverName
end

function t.SetAccountName(name)
  t.accountName = name
end
function t.SetPlatformAccountId(id)
  t.platformAccountId = id
end
function t.SetAccountHeadNo(headNo)
  t.accountHeadNo = headNo
end
function t.SetLastServerId(id)
  t.lastServerId = id
end
function t.SetChannelId(id)
  t.channelId = id
end
--获取货币资源数量
function t.GetBaseResourceValue(baseResType)
  local value = t.baseResourceTable[baseResType]
  if value == nil then return 0 end
  return value
end


function t.UpdateAccountLevelAndExp(accountLevel, accountExp)
  t.accountLevel = accountLevel
  t.accountExp = accountExp
  t.UpdateAccountLevelAndExpDelegateByProtocol()
end

function t.OnBaseResourcesUpdate(baseResourceInfoList)
  local tempCount = 0
  for k,v in ipairs(baseResourceInfoList) do
    tempCount = t.baseResourceTable[v.type]
    t.baseResourceTable[v.type] = v.value
    if v.type == BaseResType.Gold then
      gamemanager.GetModel('hero_model').CheckHasAdvanceBreakthroughHeroByRedPoint()
    end
    if tempCount and tempCount > v.value then
      TalkingDataController.instance:TDGAItemOnUse(0,tempCount-v.value,v.type)
    end
  end
  t.onUpdateBaseResourceDelegate:Invoke()
end
--pve Action 更新 RecoverTime倒计时
function t.OnPveActionInfoUpdate(pveAction,pveActionMax,pveActionNextRecoverTime)
  t.pveAction = pveAction
  t.pveActionMax = pveActionMax
  t.pveActionNextRecoverTime = pveActionNextRecoverTime
  
  t.onPveActionInfoUpdateDelegate:Invoke()
  
--[[if pveAction < pveActionMax and pveActionNextRecoverTime <= 0 then
    coroutine.start(function()
        coroutine.wait(1)
        gamemanager.GetCtrl('game_controller').SynPveActionReq()
      end)( 
  end]]
end
--------------------- get pack cell num ----------------------
function t.GetHeroCellNum ()
  return t.heroCellNum
end

function t.GetHeroCellBuyNum ()
  return t.heroCellBuyNum
end

function t.GetEquipCellNum ()
  return t.equipCellNum
end

function t.GetEquipCellBuyNum ()
  return t.equipCellBuyNum
end


--------------------- get pack cell num ----------------------


-------------------call back-------------------
function t.UpdateAccountLevelAndExpDelegateByProtocol()
  t.UpdateAccountLevelAndExpDelegate:Invoke()
  Observers.Facade.Instance.SendNotification(Observers.Facade.Instance,'game_model_UpdateAccountLevelAndExpDelegateByProtocol',(t.accountLevel..','..t.accountExp))
  
end

function t.UpdateHeroCellNum (heroCellNum, heroCellBuyNum)
  if t.heroCellNum ~= heroCellNum then
    t.heroCellNum = heroCellNum
    t.heroCellBuyNum = heroCellBuyNum
    t.UpdateHeroCellNumDelegate:Invoke()
  end
end

function t.UpdateEquipcellNum (equipCellNum, equipCellBuyNum)
  if t.equipCellNum ~= equipCellNum then
    t.equipCellNum = equipCellNum
    t.equipCellBuyNum = equipCellBuyNum
    t.UpdateEquipCellNumDelegate:Invoke()
  end
end
------------------------界面打开监听---------------------------------
--图鉴只请求一次
t.isRequestIllustration = false

function t.MainViewOpenHandler(note)
  if note.Type == 'open' then
    if gamemanager.GetModel('function_open_model').IsFunctionOpen(FunctionOpenType.MainView_illustrate) and not t.isRequestIllustration then
      gamemanager.GetCtrl('illustration_ctrl').IllustrationReq()
      t.isRequestIllustration = true
    end
  end
  if gamemanager.GetModel('function_open_model').IsFunctionOpen(FunctionOpenType.FightCenter_Expedition) then
    gamemanager.GetModel('expedition_model').CheckDeadHeroAtFormation()
  end
    gamemanager.GetCtrl('activity_controller').RepActivityList()
  
  return true
end

function t.ServerListViewOpenHandler(note)
  if note.Type == 'open' then
    
    coroutine.start(t.HandleServerListViewOpen,note.Body)
      
  end
  return true
end


function t.HandleServerListViewOpen(gameObject)
  coroutine.wait(0.05)
  local transform = gameObject.transform
  local contentTran = ui_util.FindComp(transform,"core/frame/Scroll View/Viewport/Content",Transform)
  if contentTran == nil then
    return true
  end
  --new 
  local newTran = ui_util.FindComp(contentTran,"new",Transform)
  local newServerListTran = ui_util.FindComp(contentTran,"new/new_server_root",Transform)
  local grid = newServerListTran:GetComponent(typeof(GridLayoutGroup))
  local row,mod = math.modf(newServerListTran.childCount /3)
  if mod ~= 0 then
    row = row + 1
  end
  local height = row * grid.cellSize.y + (row-1)* grid.spacing.y
  --print("newTran",newTran.localPosition.y,row,mod,height)
  --full
  local fullTran = ui_util.FindComp(contentTran,"full",Transform)
  fullTran.localPosition = Vector3(fullTran.localPosition.x,newTran.localPosition.y - height - 50,0)
  local fullServerListTran = ui_util.FindComp(contentTran,"full/full_server_root",Transform)
  grid = fullServerListTran:GetComponent(typeof(GridLayoutGroup))
  row,mod = math.modf(fullServerListTran.childCount / 3)
  if mod ~= 0 then
    row = row + 1
  end
  height = row * grid.cellSize.y + (row-1)* grid.spacing.y
 -- print("full",fullTran.localPosition.y,row,mod,newServerListTran.childCount,height)
  --maintain
  local maintainTran = ui_util.FindComp(contentTran,"maintain",Transform)
  maintainTran.localPosition = Vector3(fullTran.localPosition.x,fullTran.localPosition.y - height - 50,0)
  local maintainServerListTran = ui_util.FindComp(contentTran,"maintain/maintain_server_root",Transform)
  grid = maintainServerListTran:GetComponent(typeof(GridLayoutGroup))
  row,mod = math.modf(maintainServerListTran.childCount/ 3)
  if mod ~= 0 then
    row = row + 1
  end
  height = row * grid.cellSize.y + (row-1)* grid.spacing.y
  --print("maintainTran",maintainTran.localPosition.y,row,mod,height)
  contentTran.sizeDelta = Vector2(contentTran.sizeDelta.x,math.abs(maintainTran.localPosition.y) + height + 50)
  
      
end
------------------------界面打开监听---------------------------------

-------------open view-------------------
function t.OpenCdkeyGiftView()
  dofile('ui/cdkey/view/cdkey_gift_view').Open()
end

Start()

return t