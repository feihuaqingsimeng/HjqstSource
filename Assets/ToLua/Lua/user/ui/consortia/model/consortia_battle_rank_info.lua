local t = {}
t.__index = t

function t.New(id)
  local o = {}
  setmetatable(o,t)
  o:Set(id,'name'..id,1,1,id,1,0)
  return o
end

function t:Set(id,name,headNo,lv,rank,vectoryCount,point)
  self.id = id
  self.name = name
  self.headNo = headNo
  self.lv = lv
  self.rank = rank
  self.vectoryCount = vectoryCount
  self.point = point
end

return t