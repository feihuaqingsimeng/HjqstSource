local t = {}
local name = "golden_touch_model"

t.useLogList = ArrayList.New('log_info')
local function Start ()
  gamemanager.RegisterCtrl(name,t)
 
end

function t.GetUseTimes()
  return t.useLogList.Count
end

function t.GetMaxTimes()
  return gamemanager.GetModel('vip_model').vipData.dayGoldHandTimes
end
function t.GetNextVipAndExtraTimes()
  local vip_data = gamemanager.GetData('vip_data')
  local vip_model = gamemanager.GetModel('vip_model')
  local ownTimes = t.GetMaxTimes()
  local maxVip = vip_data.maxLevel
  local times = 0
  for i = vip_model.vipLevel+ 1,maxVip do
    local data = vip_data.GetVIPData(i)
    if data then
      times = data.dayGoldHandTimes
    end
    if times >  ownTimes then
      return i,times-ownTimes 
    end
  end
  return 0,0
end
function t.AddUseLogList(goldHandInfoResp,isClear)
  if isClear then
    t.useLogList:Clear()
  end
  local str 
  for k,v in ipairs(goldHandInfoResp.infos) do
    str = string.split2number(v,',')
    t.AddUseLog(str[1],str[2],str[3])
  end
end

function t.AddUseLog(diamond ,gold,critTimes)
  t.useLogList:Add(require('ui/golden_touch/model/log_info').New(diamond,gold,critTimes))
end

Start()

return t