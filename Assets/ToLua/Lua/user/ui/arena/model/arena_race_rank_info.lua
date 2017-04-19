local t = {}
t.__index = t
function t.New(rank,name,power)
  local o = {}
  setmetatable(o,t)
  
  o.rank = rank
  o.name = name
  o.power = power
  return o
end

return t