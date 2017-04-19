local t = {}
local name = 'fight_result_model'

local function Start()
  gamemanager.RegisterModel(name, t)
end

-------------------openView-------------
function t.OpenFightResultHeroDisplayView(modelId, star, onViewCloseHandler, showPreheatEffect)
  local view = dofile('ui/fight_result/view/fight_result_hero_display_view')
  view.Open(modelId, star, onViewCloseHandler, showPreheatEffect)
end
Start()
return t