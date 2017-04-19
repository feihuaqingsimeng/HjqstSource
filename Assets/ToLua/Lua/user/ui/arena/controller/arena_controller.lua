local t = {}
local name = 'arena_controller'
require 'pvp_pb'
require 'common_pb'

t.GainArenaChallengeRewardSucDelegate = void_delegate.New()

local function Start()
  netmanager.RegisterProtocol(MSG.GetPvpInfoResp,t.GetPvpInfoResp)
  netmanager.RegisterProtocol(MSG.GetWinTimesAwardResp,t.GetWinTimesAwardResp)
  --netmanager.RegisterProtocol(MSG.PointPvpChallengeResp,t.PointPvpChallengeResp)
  --netmanager.RegisterProtocol(MSG.PointPvpSettleResp,t.PointPvpSettleResp)
  gamemanager.RegisterCtrl(name,t)
end

--请求积分赛信息
function t.GetPvpInfoReq()
  netmanager.SendProtocol(MSG.GetPvpInfoReq,nil)
end

--请求匹配积分赛
function t.PointPvpChallengeReq()
  netmanager.SendProtocol(MSG.PointPvpChallengeReq,nil)
end

--请求积分赛结算
--function t.PointPvpSettleReq(result)
--  local intproto = common_pb.IntProto()
--  intproto.value = result
--  netmanager.SendProtocol(MSG.PointPvpSettleReq,intproto)
--end

--响应积分赛信息
function t.GetPvpInfoResp()
  local pvpInfoResp = pvp_pb.GetPvpInfoResp()
  pvpInfoResp:ParseFromString(netmanager.GetProtocolData())
  gamemanager.GetModel('arena_model').SetPvpInfo(pvpInfoResp)  
end

--[[
--响应匹配积分赛
function t.PointPvpChallengeResp()
  if not pvp_pb then require 'pvp_pb' end
  local pointPvpChallengeResp = pvp_pb.PointPvpChallengeResp()
  pointPvpChallengeResp:ParseFromString(netmanager.GetProtocolData())
  
  
end

--响应积分赛结算
function t.PointPvpSettleResp()
  if not pvp_pb then require 'pvp_pb' end
  local pointPvpSettleResp = pvp_pb.PointPvpSettleResp()
  pointPvpSettleResp:ParseFromString(netmanager.GetProtocolData())
  
  
end
--]]
--请求领取胜利宝箱
function t.GetWinTimesAwardReq()
  netmanager.SendProtocol(MSG.GetWinTimesAwardReq,nil)
end
---竞技场响应领取胜利宝箱
function t.GetWinTimesAwardResp()
  print('竞技场响应领取胜利宝箱')
  local resp = common_pb.IntProto()
  resp:ParseFromString(netmanager.GetProtocolData())
  gamemanager.GetModel('arena_model').canUseWinTimes = resp.value
  t.GainArenaChallengeRewardSucDelegate:Invoke()
  
end
--------------open View------------------------
function t.IsOpenRace()
	local dateTime = TimeController.instance.ServerTime
	local global_data = gamemanager.GetData('global_data')
	local isOpen = false
  for k,v in pairs(global_data.point_pvp_start_week) do
    if(dateTime.DayOfWeek:ToInt() == v) then
      isOpen = true
      break
    end
  end
  return isOpen
end

function t.OpenRaceIntroView()
  local isOpen = t.IsOpenRace()
  if(isOpen) then
    local pvpRaceIntroView = dofile('ui/arena/view/arena_race/arena_race_intro_view')
    pvpRaceIntroView.Open()
    t.GetPvpInfoReq()
  else
    print('not open day')
    local common_error_tip_view = require('ui/tips/view/common_error_tips_view')
    common_error_tip_view.Open(LocalizationController.instance:Get('ui.pvp_race_intro_view.not_open_time_tips'))  
  end
end

function t.OpenRaceView()
  local arenaRaceView = dofile('ui/arena/view/arena_race/arena_race_view')
  arenaRaceView.Open()
end

function t.OpenRaceRankView()
  dofile('ui/arena/view/arena_race/arena_race_rank_view')
end

function t.OpenRaceEmbattleView(callback)
  --dofile('ui/arena/view/arena_race/arena_race_embattle_view')
  gamemanager.GetCtrl('pve_embattle_controller').OpenPveEmbattleView(callback)

end
--竞技场阵型界面
function t.OpenArenaEmbattleView(isReadyFight)
  --local view = dofile('ui/arena/view/normal/arena_embattle_view')
  --print('OpenArenaEmbattleView',isReadyFight)
  --view.Open(isReadyFight)
  local view = gamemanager.GetCtrl('pve_embattle_controller').OpenPveEmbattleView(nil,isReadyFight,function()
       LuaCsTransfer.SendArenaChanllengReq()
    end)
  view.SetCommonTopBarAsCommonStyle(false,false,true,true,true,false,false)
end
----竞技场领取胜利宝箱界面
function t.OpenChallengeRewardView()
  local view = dofile('ui/arena/view/normal/arena_challenge_reward_view')
  view.Open()
end
---竞技场排名变化界面
function t.OpenRankChangeTipView(curRank,lastRank)
  local view = require('ui/arena/view/normal/arena_rank_change_tip_view')
  view.Open(curRank,lastRank)
end
Start()

return t