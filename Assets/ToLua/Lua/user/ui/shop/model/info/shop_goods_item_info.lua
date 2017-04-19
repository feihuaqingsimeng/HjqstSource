local t = {}
t.__index = t

local gameResData = require('ui/game/model/game_res_data')

function t.New (shopGoodItemData)
  local o = {}
  setmetatable(o, t)
  
  o.shopGoodItemData = shopGoodItemData
  o.remainPurchaseTimes = 0
  return o
end

function t:SetRemainPurchaseTimes (remainPurchaseTimes)
  self.remainPurchaseTimes = remainPurchaseTimes
end

function t:GetShopItemType ()               --商店类型ID，对应不同的页签
  return self.shopGoodItemData.sheet_num
end

function t:GetShopItemID ()
  return self.shopGoodItemData.id
end

function t:GetShopItemName ()
  local shopItemName = ''
  local itemGameResData = gameResData.NewByString(self.shopGoodItemData.item)
  if itemGameResData.type == BaseResType.Item then
    local itemData = gamemanager.GetData('item_data').GetDataById(itemGameResData.id)
    shopItemName = LocalizationController.instance:Get(itemData.name)
  elseif itemGameResData.type == BaseResType.Hero then
    local heroData = gamemanager.GetData('hero_data').GetDataById(itemGameResData.id)
    shopItemName = LocalizationController.instance:Get(heroData.name)
  end
  return shopItemName
end

function t:GetCostResData ()
  return gameResData.NewByString(self.shopGoodItemData.cost)
end

function t:GetCostType ()
  return 1
end

return t