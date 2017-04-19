local t = {}
local name = 'chat_info'
t.__index=t

function t.New( ... )
  local r={}
  setmetatable(r,t)
  return r
end

return t