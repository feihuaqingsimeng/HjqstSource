local t = {}
t.__index = t

----titleStr:标题名  illustrationInfoList元素列表（MList） roleAttrDictionay  (Dictionary) 

function t.New(titleStr,illustrationInfoList,collectNum,roleAttrDictionay)
  local o = {}
  setmetatable(o,t)
  
  o.titleStr = titleStr
  o.illustrationInfoList = illustrationInfoList
  o.collectNum = collectNum
  o.roleAttrDictionay = roleAttrDictionay
  
  return o
end


return t
