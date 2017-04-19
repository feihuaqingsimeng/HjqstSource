local t = {}
t.__index = t

function t.New(resType,id,count,star)
  local o = {}
  setmetatable(o,t)
  o:Set(resType,id,count,star)
  return o
end

function t.NewByString(gameResString)
  local o = {}
  setmetatable(o,t)
  o:Set(0,0,0,0)
  if gameResString == nil then
    print(ui_util.FormatToRedText('[error]create GameResData ,but gameResString is nil'))
    return
  end
  local str  = string.split(gameResString,':')
  if #str ~= 4 then
    print(ui_util.FormatToRedText('[error]create GameResData ,but str length is not 4,str:'..gameResString))
    return nil
  end
  o:Set(tonumber(str[1]),tonumber(str[2]),tonumber(str[3]),tonumber(str[4]))
  return o
end

function t:Set(resType,id,count,star)
  self.type = resType
  self.id = id
  self.count = count
  self.star = star
end

function t.ParseGameResDataList(gameResDataListStr)
  local resTable = {}
  local strArray = string.split(gameResDataListStr,';')
  for k,v in ipairs(strArray) do
    resTable[k] = t.NewByString(v)
  end
  return resTable
end

function t:ToString()
  return string.format('(%d,%d,%d,%d)',self.type,self.id,self.count,self.star)
end

return t