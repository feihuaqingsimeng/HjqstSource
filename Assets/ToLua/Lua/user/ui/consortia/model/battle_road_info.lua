local t = {}
t.__index = t

function t.New(id)
  local o = {}
  setmetatable(o,t)
  o.id = id
  o.myRoad = {}
  if id == 1 then
    o.myRoad[1] = t.CreateRoad('nihao'..id,0,1)
  end
  o.enemyRoad = {}
  if id == 1 then
    o.enemyRoad[1] = t.CreateRoad('nihaoa'..id,0,1)
  end
  return o
end

function t.CreateRoad(name,headNo,lv)
  local o = {}
  o.name = name
  o.headNo = headNo
  o.lv = lv
  return o
end

return t