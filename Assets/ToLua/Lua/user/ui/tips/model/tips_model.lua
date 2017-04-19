local t = {}

local name = 'tips_model'

local function Start()
  gamemanager.RegisterModel(name,t)
end

function t.GetTipView(name)
  local view = require('ui/tips/view/'..name)
  return view
end

Start()

return t