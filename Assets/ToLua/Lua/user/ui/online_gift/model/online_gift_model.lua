local t = {}
local name = 'online_gift_model'
t.GetOnlineGiftRewardSucDelegate = void_delegate.New()

local function Start()
  gamemanager.RegisterModel(name,t)  
   t.lastTime = 0
   t.totalTime = 0
   t.alreadyGetId = 0
end

function t.OnlineGiftSynResp(totalTime,alreadyGetId)
  t.totalTime = totalTime
  t.lastTime = TimeController.instance.ServerTimeTicksSecond
  t.alreadyGetId = alreadyGetId  
end

function t.OnlineGiftResp(id)
  t.GetOnlineGiftRewardSucDelegate:InvokeOneParam(id)
end

function t.GetCurrentTotalTime()
  local currentTotalTime = TimeController.instance.ServerTimeTicksSecond - t.lastTime
  currentTotalTime = currentTotalTime + t.totalTime  
  return currentTotalTime
end

function t.GetEndTime()
  local id = t.GetCurrentGiftId()
  local giftData = gamemanager.GetData('online_gift_data').GetDataById(id)
  local result = 0
  if(giftData ~= nil) then
    result = giftData.time - t.GetCurrentTotalTime()
  end   
  if(result <= 0) then
    result = 0
  end
  return result
end

function t.CanGetOnlineGift(time)  
  return time <= 0
end

function t.GetCurrentGiftId()
  local id = t.alreadyGetId
  if(id == 0) then
    return 1
  end
  local length = gamemanager.GetData('online_gift_data').GetDataLength()
  if(id >= length) then
    return 0
  end
  return (id+1)
end

Start()

return t