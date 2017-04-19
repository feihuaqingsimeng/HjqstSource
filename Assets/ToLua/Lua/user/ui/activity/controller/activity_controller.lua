local t={}

local name= 'activity_controller'
local activity_model = gamemanager.GetModel('activity_model')
local auto_destroy_tips_view = require('ui/tips/view/auto_destroy_tip_view')

require 'activity_pb'
require 'common_pb'

t.OnDlegateActivityListResp   = void_delegate.New()
t.OnDlegategActivityJoinResp  = void_delegate.New()
t.OnDlegateActivityRewardResp = void_delegate.New()
t.OnDlegateActivityUpdateResp = void_delegate.New()
t.OnDlegateActivityVipDailyGifReceive = void_delegate.New()
t.OnDlegateSevenDayInfoResp = void_delegate.New()
t.OnDlegateBuySevenDayGoodsResp = void_delegate.New()
t.OnDlegateGetSevenDayCompleteAwardResp = void_delegate.New()
t.OnDlegateGetSevenDayTaskAwardResp = void_delegate.New()
t.OnTurntableInfoUpdateDelegate = void_delegate.New()--转盘更新
t.OnturntableDrawSucDelegate = void_delegate.New()--转盘成功转
t.view = nil

t.IsGetedActivityResp = false
local function Start( ... )
  
	netmanager.RegisterProtocol(MSG.ActivityListResp,t.OnMsgActivityListResp) 
	netmanager.RegisterProtocol(MSG.ActivityJoinResp,t.OnMsgActivityJoinResp)
	netmanager.RegisterProtocol(MSG.ActivityRewardResp,t.OnMsgActivityRewardResp)
	netmanager.RegisterProtocol(MSG.ActivityUpdateResp,t.OnMsgActivityUpdateResp)	
	netmanager.RegisterProtocol(MSG.VipDailyGiftResp, t.OnMsgVipDailyGiftResp)

  netmanager.RegisterProtocol(MSG.SevenDayInfoResp, t.OnMsgSevenDayInfoResp) 
  netmanager.RegisterProtocol(MSG.BuySevenDayGoodsResp, t.OnMsgBuySevenDayGoodsResp) 
  netmanager.RegisterProtocol(MSG.GetSevenDayCompleteAwardResp, t.OnMsgGetSevenDayCompleteAwardResp) 
  netmanager.RegisterProtocol(MSG.GetSevenDayTaskAwardResp, t.OnMsgGetSevenDayTaskAwardResp)
  --转盘
  netmanager.RegisterProtocol(MSG.LuckyLetteInfoResp, t.LuckyLetteInfoResp)
  netmanager.RegisterProtocol(MSG.UseLuckyLetteResp, t.UseLuckyLetteResp)
  netmanager.RegisterProtocol(MSG.UseLuckyRouletteTenResp, t.UseLuckyRouletteTenResp)
  netmanager.RegisterProtocol(MSG.GiftCodeUseResp, t.GiftCodeUseResp)
  
  gamemanager.RegisterCtrl(name, t)
end

--打开活动界面
function t.OpenActivity()
  local bTime = TimeController.instance.ServerTimeTicksMillisecond
  t.view = dofile('ui/activity/view/activity_main_view') 
  t.view:Open() 
end

--打开首充界面
function t.OpenFirstCharge()
  t.view = dofile('ui/activity/view/first_charge_view')
  t.view:Open()
end

--打开七日狂欢界面
function t.OpenSevenHilarity()
  t.view = dofile('ui/activity/view/seven_hilarity/seven_hilarity_view')
  t.view:Open()
end
----------------------------------Resp-------------------------------------------
--响应活动列表
function t.OnMsgActivityListResp()
  local resp = activity_pb.ActivityListResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  activity_model:ParseActivityList(resp)
  t.OnDlegateActivityListResp:Invoke()
  
  if not IsGetedActivityResp then
    t.IsGetedActivityResp = true
    
    local btype = activity_model:GetSevenHalirityType()    
    local activityData = activity_model:GetActivityByType(btype)
    if nil ~= activityData then t.ReqSevenDayInfoResp() end    
  end
  --[[local str = '相应活动列表:'
  for k,v in ipairs(resp.list) do
    str = str ..(v.type..','..v.subType..','..v.conditionValue..';')
  end]]
end

--响应领取活动奖励
function t.OnMsgActivityRewardResp()
  t.OnDlegateActivityRewardResp:Invoke()
end

--响应更新活动
function t.OnMsgActivityUpdateResp()
  local resp = activity_pb.ActivityUpdateResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  activity_model:ParseUpdateResp(resp)
  t.OnDlegateActivityUpdateResp:Invoke()
end

--响应领取vip每日礼包
function t.OnMsgVipDailyGiftResp()
  t.OnDlegateActivityVipDailyGifReceive:Invoke()
end

--响应参加活动
function t.OnMsgActivityJoinResp()
  t.OnDlegategActivityJoinResp:Invoke()
end

--响应七日活动信息
function t.OnMsgSevenDayInfoResp()
  local resp = activity_pb.SevenDayInfoResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  activity_model:ParseSevenHilarityInfo(resp)
  t.OnDlegateSevenDayInfoResp:Invoke()
end

--响应购买七日打折商品
function t.OnMsgBuySevenDayGoodsResp()
  t.OnDlegateBuySevenDayGoodsResp:Invoke()
end

--响应领取完成度奖励
function t.OnMsgGetSevenDayCompleteAwardResp()
  t.OnDlegateGetSevenDayCompleteAwardResp:Invoke()
end

--响应领取七日任务奖励
function t.OnMsgGetSevenDayTaskAwardResp()
  t.OnDlegateGetSevenDayTaskAwardResp:Invoke()
end
----------------------------------Req---------------------------------------------
--请求活动列表
function t.RepActivityList()
  netmanager.SendProtocol(MSG.ActivityListReq, nil)
end

--请求领取活动奖励
function t.ReqActivityReward(type, id, subType)
  local pb = activity_pb.ActivityRewardReq()
	pb.type = type
	pb.scheduleIndex  = id
  --pb.subType = subType
  t.view:RecordCurActivity(type, id)
  netmanager.SendProtocol(MSG.ActivityRewardReq, pb)
end

--请求领取vip每日礼包
function t.ReqActivityVipDailyGift()
  netmanager.SendProtocol(MSG.VipDailyGiftReq, nil)
end

--请求参加活动
function t.ReqActivityJoin(type, subType)
  local pb = activity_pb.ActivityJoinReq()
	pb.type = type	
  pb.subType = subType  
  netmanager.SendProtocol(MSG.ActivityJoinReq, pb)
end

--请求七日活动信息
function t.ReqSevenDayInfoResp()
  netmanager.SendProtocol(MSG.SevenDayInfoReq, nil)  
end

--请求购买七日打折商品
function t.ReqBuySevenDayGoodsResp(bday, CsvId, id)
  local req = common_pb.DoubleIntProto()
  req.value1 = bday
  req.value2 = CsvId  
  
  t.view:RecordCurActivity(id)
  netmanager.SendProtocol(MSG.BuySevenDayGoodsReq, req)   
end

--请求领取完成度奖励
function t.ReqGetSevenDayCompleteAwardResp(bprogress)
  local req = common_pb.IntProto()
  req.value = bprogress
  netmanager.SendProtocol(MSG.GetSevenDayCompleteAwardReq, req)  
end

--请求领取七日任务奖励
function t.ReqGetSevenDayTaskAwardResp(bid)
  local req = common_pb.IntProto()
  req.value = bid
  
  t.view:RecordCurActivity(bid)
  netmanager.SendProtocol(MSG.GetSevenDayTaskAwardReq, req)
end
--0x1625请求转盘信息
function t.LuckyLetteInfoReq()
  netmanager.SendProtocol(MSG.LuckyLetteInfoReq, nil)
end
--响应转盘信息
function t.LuckyLetteInfoResp()
  local resp = activity_pb.LuckyLetteInfoResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  local turntable_model = gamemanager.GetModel('turntable_model')
  turntable_model.freeCount = 0
  if resp.canFree then
    turntable_model.freeCount = 1
  end
  turntable_model.drawCount = resp.times
  turntable_model.InitRankByProtocol(resp.rankInfo)
  
  t.OnTurntableInfoUpdateDelegate:Invoke()
end
--请求使用转盘
function t.UseLuckyLetteReq()
  netmanager.SendProtocol(MSG.UseLuckyLetteReq, nil)
end
--响应使用转盘
function t.UseLuckyLetteResp()
  print('响应使用转盘')
  local resp = common_pb.IntProto()
  resp:ParseFromString(netmanager.GetProtocolData())
  local reward = {}
  reward[1] = resp.value
  t.OnturntableDrawSucDelegate:InvokeOneParam(reward)

end
--请求使用转盘多次
function t.UseLuckyRouletteTenReq()
  netmanager.SendProtocol(MSG.UseLuckyRouletteTenReq, nil)
end
--响应使用转盘多次
function t.UseLuckyRouletteTenResp()
  print('响应使用十转盘')
  local resp = activity_pb.UseLuckyRouletteTenResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  t.OnturntableDrawSucDelegate:InvokeOneParam(resp.awardsId)
end
--请求使用礼包码
function t.GiftCodeUseReq(value)
  local req = common_pb.StringProto()
  req.value = value
  netmanager.SendProtocol(MSG.GiftCodeUseReq, req)
end
--响应使礼包码
function t.GiftCodeUseResp()
  local resp = common_pb.StringProto()
  resp:ParseFromString(netmanager.GetProtocolData())
  local rewardList = require('ui/game/model/game_res_data').ParseGameResDataList(resp.value)
  require('ui/tips/view/common_reward_tips_view').Create(rewardList)
end


Start()
return t