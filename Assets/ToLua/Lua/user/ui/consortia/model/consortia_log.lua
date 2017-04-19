local t = {}
t.__index = t

function t.New(type,time,paramsList)
  local o = {}
  setmetatable(o,t)
  
  o.type = type
  o.time =time
  
  o.timeString = TimeUtil.FormatTimeToString(o.time/1000,'yyyy/MM/dd   HH:mm:ss')
  o.paramsList = paramsList
  
  return o
end
function t:GetTimeString()
  return self.timeString
end

function t:GetContent()
  return string.format( LocalizationController.instance:Get('ui.consortia_view.log'..self.type),self.paramsList[1])
end

return t