local t = {}
t.__index = t


function t.New(diamond,gold,crit)
  local o = {}
  setmetatable(o,t)
  o.diamond = diamond
  o.gold = gold
  o.crit = crit
  
  return o
end

function t:ToString()
  
  return string.format(LocalizationController.instance:Get('ui.golden_touch_view.usedDiamondTip'),self.diamond,self.gold)
end

return t