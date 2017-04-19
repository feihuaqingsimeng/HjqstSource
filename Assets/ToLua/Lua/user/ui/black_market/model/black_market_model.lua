local t = {}
local name = 'black_market_model'

t.blackMarketInfoTable = {}--id,table{blackMarketInfo}
--no param
t.onUpdateBlackMarketDelegate = void_delegate.New()
t.onInitBlackMarketDelegate = void_delegate.New()
t.onUpdatePurchaseGoodsSuccessDelegate = void_delegate.New()

t.refreshTimeTable = {} --market_type,刷新时间节点ms

local function Start()
  gamemanager.RegisterCtrl(name,t)
end
function t.ClearAll()
  t.blackMarketInfoTable = {}
  t.refreshTimeTable = {}
end
function t.ClearInfoListByType(marketType)
  t.blackMarketInfoTable[marketType] = {}
  t.blackMarketInfoTable[marketType].infos = {}
  t.blackMarketInfoTable[marketType].buyRefreshTimes = 0
end
function t.GetMarketInfoListByType(marketType)
  if t.blackMarketInfoTable[marketType] == nil then
    return nil
  end
  local infoList = {}
  local index = 1
  local infos = t.blackMarketInfoTable[marketType].infos
  for k,v in pairs(infos) do
    if v:GetMarketType() == marketType then
      infoList[index] = v
      index = index + 1
    end
  end
  table.sort(infoList,function(a,b) 
      return a.id < b.id
      end)
  return infoList
end
function t.GetMarketInfoCount(marketType)
  if t.blackMarketInfoTable[marketType] then
    return table.count(t.blackMarketInfoTable[marketType].infos) 
  end
  return 0
end
function t.AddBlackMarketInfo(marketType,blackMarketInfo)
  if not t.blackMarketInfoTable[marketType] then
    t.ClearInfoListByType(marketType)
  end
  t.blackMarketInfoTable[marketType].infos[blackMarketInfo.id] = blackMarketInfo
end

function t.RemoveBlackMarketInfo(marketType,marketId)
  if t.blackMarketInfoTable[marketType] then
    t.blackMarketInfoTable[marketType].infos[marketId] = nil
  end
end
function t.GetblackMarketInfo(marketType,marketId)
  if t.blackMarketInfoTable[marketType] then
    return t.blackMarketInfoTable[marketType].infos[marketId]
  end
  return nil
end
function t.GetMarketRefreshTime(marketType)
  return t.refreshTimeTable[marketType]
end
function t.SetMarketRefreshTime(marketType,time)
  t.refreshTimeTable[marketType] = time
end
function t.GetRefreshCost(marketType)
  local black_market_data = gamemanager.GetData('black_market_data')
  local costs = black_market_data.marketTypeList[marketType].refresh_diamond
  local count = table.count(costs)
  local buyRefreshTimes = t.blackMarketInfoTable[marketType].buyRefreshTimes
  return costs[math.min(buyRefreshTimes+1,count)]
end
function t.SetRefreshCostUseTimes(marketType,times)
  t.blackMarketInfoTable[marketType].buyRefreshTimes = times
end
----------------update by protocol-------------
function t.UpdateBlackMarketByProtocol()
  t.onUpdateBlackMarketDelegate:Invoke()
end
function t.InitBlackMarketByProtocol()
  t.onInitBlackMarketDelegate:Invoke()
end
function t.UpdatePurchaseGoodsSuccessByProtocol()
  t.onUpdatePurchaseGoodsSuccessDelegate:Invoke()
end


Start()
return t