local t = {}
local name = "player_manual_controller"

local function Start ()
  gamemanager.RegisterCtrl(name, t)
end

function t.OpenPlayerManualView ()
  local player_manual_view = dofile('ui/player_manual/view/player_manual_view')
  player_manual_view.Open()
end

Start()
return t
