local t = {}
local name = 'pack_controller'

local function Start ()
  gamemanager.RegisterCtrl(name, t)
end

function t.OpenPackView ()
  local packView = dofile 'ui/pack/view/pack_view'
  packView.Open()
end

Start()
return t