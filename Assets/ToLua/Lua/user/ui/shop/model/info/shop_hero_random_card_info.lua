local t = {}
t.__index = t

local gameResData = require('ui/game/model/game_res_data')

function t.New (shopCardRandomData)
  local o = {}
  setmetatable(o, t)
  
  o.shopCardRandomData = shopCardRandomData
  o.remainFreeTimes = 0
  o.nextFreeBuyTimeStamp = 0
  return o
end

function t:SetFreeInfo (remainFreeTimes, nextFreeTimeStamp)
  self.remainFreeTimes = remainFreeTimes
  self.nextFreeBuyTimeStamp = nextFreeTimeStamp
end

function t:GetNextFreeBuyCountDownTime ()
  return Common.GameTime.Controller.TimeController.instance:GetDiffTimeWithServerTimeInSecond(self.nextFreeBuyTimeStamp)
end

function t:GetShopItemType ()               --商店类型ID，对应不同的页签
  return self.shopCardRandomData.sheet_num
end

function t:GetShopItemID ()
  return self.shopCardRandomData.id
end

function t:GetShopItemName ()
  return LocalizationController.instance:Get(self.shopCardRandomData.name)
end

function t:GetCostResData ()
  return gameResData.NewByString(self.shopCardRandomData.cost)
end

function t:GetCostType ()
  if self.remainFreeTimes > 0 and self:GetNextFreeBuyCountDownTime() <= 0 then
    return 0      --0 代表免费
  end
  return 1        --1 代表付费
end

return t