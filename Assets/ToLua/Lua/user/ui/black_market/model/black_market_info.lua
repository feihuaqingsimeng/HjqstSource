local t = {}
t.__index = t

local black_market_data = gamemanager.GetData('black_market_data')
local black_market_rule_data = gamemanager.GetData('black_market_rule_data')
local item_data = gamemanager.GetData('item_data')
local equip_data = gamemanager.GetData('equip_data')
local hero_data = gamemanager.GetData('hero_data')

function t.New(marketDataId, ruleDataId, remainCount)
  local o = {}
  setmetatable(o,t)
  
  o.id = marketDataId
  o.marketData = black_market_data.GetDataById(marketDataId)
  if o.marketData == nil then
    print(ui_util.FormatToRedText('[BlackMarketInfo]marketData is null ,id:'..marketDataId))
    return nil
  end
  o:Set(ruleDataId, remainCount)
  return o
end

function t:Set(ruleDataId,remainCount)
  self.ruleData = black_market_rule_data.GetDataById(ruleDataId)
  if self.ruleData == nil then
    print(ui_util.FormatToRedText('[BlackMarketInfo]ruleData is null ,marketid:'..self.id..',ruleid:'..ruleDataId))
    return
  end
  if self.ruleData.item.type == BaseResType.Hero then
    if hero_data.GetDataById(self.ruleData.item.id) == nil then
      print(ui_util.FormatToRedText('[BlackMarketInfo]ruleData里的item找不到 ,marketid:'..self.id..',ruleid:'..ruleDataId..',heroid:'..self.ruleData.item.id))
      self.ruleData = nil
      return
    end
    
  elseif self.ruleData.item.type == BaseResType.Equipment then
     if equip_data.GetDataById(self.ruleData.item.id) == nil then
      print(ui_util.FormatToRedText('[BlackMarketInfo]ruleData里的item找不到 ,marketid:'..self.id..',ruleid:'..ruleDataId..',equipid:'..self.ruleData.item.id))
      self.ruleData = nil
      return
    end
  elseif self.ruleData.item.type == BaseResType.Item then
    if item_data.GetDataById(self.ruleData.item.id) == nil then
      print(ui_util.FormatToRedText('[BlackMarketInfo]ruleData里的item找不到 ,marketid:'..self.id..',ruleid:'..ruleDataId..',itemid:'..self.ruleData.item.id))
      self.ruleData = nil
      return
    end
  end
  
  self.remaindCount = remainCount
end

function t:GetMarketType()
  return self.marketData.market_type
end
function t:GetItemResData()
  return self.ruleData.item
end
function t:GetMaterials()
  return self.ruleData.materials
end
function t:GetLimitLevel()
  return self.marketData.limit_lv
end

function t:GetGoodsName()
  local item = self.ruleData.item
  if(item.type == BaseResType.Hero) then
    return LocalizationController.instance:Get(hero_data.GetDataById(item.id).name)
  elseif item.type == BaseResType.Equipment then
    return LocalizationController.instance:Get(equip_data.GetDataById(item.id).name)
  else
    return LocalizationController.instance:Get(item_data.GetDataById(item.id).name)
  end
end

function t:GetGoodsDescription()
  local item = self.ruleData.item
  if(item.type == BaseResType.Hero) then
    return LocalizationController.instance:Get(hero_data.GetDataById(item.id).description)
  elseif item.type == BaseResType.Equipment then
    return LocalizationController.instance:Get(equip_data.GetDataById(item.id).description)
  else
    return LocalizationController.instance:Get(item_data.GetDataById(item.id).des)
  end
end

function t:GetRisePrice()
  local alreadyExchangeCount = self.marketData.limit_num - self.remaindCount
  local global_data = gamemanager.GetData('global_data')   
  local price = 0
  local index = 1
  if(alreadyExchangeCount >= #global_data.blackmarket_double) then
    index = #global_data.blackmarket_double
  else
    index = alreadyExchangeCount+1
  end
  
  price = math.floor(global_data.blackmarket_double[index] * self.ruleData.materials[1].count)
  return price
end

function t:GetRemindCountString()
  if self.marketData.limit_type == 1 then
    return string.format(LocalizationController.instance:Get("ui.black_market_view.remainCount"),self.remaindCount)
  elseif self.marketData.limit_type == 2 then
    return string.format(LocalizationController.instance:Get("ui.black_market_view.serverRemainCount"),self.remaindCount)
  end
  return ''
end

return t