local t = {}
local name = 'arena_model'
t.UpdatePvpInfoDelegate = void_delegate.New()
--竞技场可用胜利场数
t.canUseWinTimes = 0

local function Start()
  gamemanager.RegisterModel(name,t)
  local race_rank_info = require('ui/arena/model/arena_race_rank_info')
  t.myRaceRankInfo = race_rank_info.New(1,'你猜猜我是谁',999)
end

function t.SetPvpInfo(pvpInfoResp)
  t.race = pvpInfoResp.point
  t.winTimes = pvpInfoResp.winTimes
  t.loseTimes = pvpInfoResp.loseTimes
  t.rec = {}
  for k,v in ipairs(pvpInfoResp.rec) do
    local strs = string.split(v,',')    
    local item = {}
    item.name = strs[1]
    item.result = tonumber(strs[2])
    item.race = tonumber(strs[3])
    table.insert(t.rec,item)
  end
  
  t.useTimes = pvpInfoResp.useTimes
  t.isOpen = pvpInfoResp.isOpen
  t.time = pvpInfoResp.time / 1000  
  t.UpdatePvpInfoDelegate:Invoke()
end

function t.SetCanUseWinTimes(count)
  t.canUseWinTimes = count
  gamemanager.GetModel('red_point_model').RefreshSpecific(RedPointType.RedPoint_PVP_Challenge_Reward)
end

Start()
return t