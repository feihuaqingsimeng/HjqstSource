local t = {}
local name = 'pve_embattle_controller'

local function Start()
  gamemanager.RegisterCtrl(name,t)
end

--------------------open view--------------------------
--changeTeamCallbackFunc 传参一个，选中的当前队（1、2还是3队）
function t.OpenPveEmbattleView(changeTeamCallbackFunc,isReadyFight,readyFightCallback)
  local view = dofile('ui/pve_embattle/view/pve_embattle_view')
  view.Open(changeTeamCallbackFunc,isReadyFight,readyFightCallback)
  return view
end

Start()
return t