local t = {}
t.__index = t

local gameResData = require('ui/game/model/game_res_data')

function t.New (shopDiamondData)
  local o = {}
  setmetatable(o, t)
  
  o.shopDiamondData = shopDiamondData
  o.isFirstRecharge = true
  return o
end

function t:GetShopItemType ()               --商店类型ID，对应不同的页签
  return self.shopDiamondData.sheet_num
end

function t:GetShopItemID ()
  return self.shopDiamondData.id
end

function t:GetShopItemName ()
  return LocalizationController.instance:Get(self.shopDiamondData.name)
end

function t:GetCostResData ()
  return gameResData.NewByString(self.shopDiamondData.cost)
end

function t:GetCostType ()
  return 1
end

return t