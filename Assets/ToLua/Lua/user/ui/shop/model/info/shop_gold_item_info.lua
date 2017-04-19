local t = {}
t.__index = t

local gameResData = require('ui/game/model/game_res_data')

function t.New (shopLimitItemData)
  local o = {}
  setmetatable(o, t)

  o.shopLimitItemData = shopLimitItemData
  o.isOpen = false
  o.remainPurchaseTimes = 0
  
  return o
end

function t:SetInfo (isOpen, remainPurchaseTimes)
  self.isOpen = isOpen
  self.remainPurchaseTimes = remainPurchaseTimes
end

function t:GetShopItemType ()               --商店类型ID，对应不同的页签
  return self.shopLimitItemData.sheet_num
end

function t:GetShopItemID ()
  return self.shopLimitItemData.id
end

function t:GetShopItemName ()
  return LocalizationController.instance:Get(self.shopLimitItemData.name)
end

function t:GetCostResData ()
  return gameResData.NewByString(self.shopLimitItemData.cost)
end

function t:GetCostType ()
  return 1
end

return t