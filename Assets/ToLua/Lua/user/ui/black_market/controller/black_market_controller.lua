local t = {}
local name = 'black_market_controller'

require 'shopping_pb'
require 'common_pb'


local black_market_info = require('ui/black_market/model/black_market_info')


local function Start()
  netmanager.RegisterProtocol(MSG.BlackMarketResp,t.BlackMarketResp)
  netmanager.RegisterProtocol(MSG.BlackMarketUpdateResp,t.BlackMarketUpdateResp)
  netmanager.RegisterProtocol(MSG.PurchaseBlackGoodsResp,t.PurchaseBlackGoodsResp)
  netmanager.RegisterProtocol(MSG.RefreshBlackMarketResp,t.RefreshBlackMarketResp)
  
  gamemanager.RegisterCtrl(name,t)
end
-----------------req----------------------------
--请求黑市商品信息
function t.BlackMarketReq()
  local req = shopping_pb.BlackMarketReq()
  netmanager.SendProtocol(MSG.BlackMarketReq,req)
end
--请求交换黑市商品
function t.PurchaseBlackGoodsReq(marketType,marketId)
  local req = shopping_pb.PurchaseBlackGoodsReq()
  req.type = marketType
  req.marketId = marketId
  netmanager.SendProtocol(MSG.PurchaseBlackGoodsReq,req)
end
--请求手动刷新黑市商品
function t.RefreshBlackMarketReq(marketType)
  local req = common_pb.IntProto()
  req.value = marketType
  netmanager.SendProtocol(MSG.RefreshBlackMarketReq,req)
end
-----------------resp----------------------------
--响应黑市商品信息
function t.BlackMarketResp()
  print('响应黑市商品信息la')
  local resp = shopping_pb.BlackMarketResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  local black_market_model = gamemanager.GetModel('black_market_model')
  black_market_model.ClearAll()
  for k,v in ipairs(resp.tabs) do
    for k1,v1 in ipairs(v.goods) do
      local info = black_market_info.New(v1.marketId,v1.goodsNo,v1.remainBuyTimes)
      if info ~= nil and info.ruleData ~= nil then
        black_market_model.AddBlackMarketInfo(v.type,info)
      end
    end
    black_market_model.SetMarketRefreshTime(v.type, math.floor(v.nextRefreshTime))
    black_market_model.SetRefreshCostUseTimes(v.type,v.dailyRefreshTimes)
  end
  black_market_model.InitBlackMarketByProtocol()
end
--黑市商品信息更新
function t.BlackMarketUpdateResp()
  local resp = shopping_pb.BlackMarketUpdateResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  local black_market_model = gamemanager.GetModel('black_market_model')
  --刷新时间
  black_market_model.ClearInfoListByType(resp.tab.type)
  black_market_model.SetMarketRefreshTime(resp.tab.type,math.floor(resp.tab.nextRefreshTime))
  black_market_model.SetRefreshCostUseTimes(resp.tab.type,resp.tab.dailyRefreshTimes)
  --
  for k,v in ipairs(resp.tab.goods) do
    local info = black_market_model.GetblackMarketInfo(resp.tab.type,v.marketId)
    if info == nil then
      info = black_market_info.New(v.marketId,v.goodsNo,v.remainBuyTimes)
      if info ~= nil and info.ruleData ~= nil then
        black_market_model.AddBlackMarketInfo(resp.tab.type,info)
      end
    else
      info:Set(v.goodsNo,v.remainBuyTimes)
      if info.ruleData == nil == nil  then
        black_market_model.RemoveBlackMarketInfo(resp.tab.type,v.marketId)
      end
    end
  end
  
  
  black_market_model.UpdateBlackMarketByProtocol()
end

function t.PurchaseBlackGoodsResp()
  local black_market_model = gamemanager.GetModel('black_market_model')
  black_market_model.UpdatePurchaseGoodsSuccessByProtocol()
end
function t.RefreshBlackMarketResp()
  
end
----------------------open view---------------------
function t.OpenBlackMarketView(toggleIndex)
  if not gamemanager.GetModel('function_open_model').IsFunctionOpen(FunctionOpenType.MainView_BlackMarket,true) then
    return
  end
  local view = dofile('ui/black_market/view/black_market_view')
  if not toggleIndex then
    toggleIndex = gamemanager.GetData('global_data').black_market_sort_list[1]
  end
  view.Open(toggleIndex)
end
Start()
return t