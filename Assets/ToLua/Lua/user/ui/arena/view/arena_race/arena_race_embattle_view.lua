local t = {}
local PREFAB_PATH = 'ui/pvp/pvp_race_embattle_view'

local function Start()
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
   local common_top_bar = require ('ui/common_top_bar/common_top_bar')
  common_top_bar = common_top_bar:Create(t.transform:Find('core'))
  common_top_bar:SetAsCommonStyle('formation',t.OnClickBackBtnHandler,false,true,true,false,false,false,false)
  
  t.formationTeamView = dofile('ui/formation/view/formation_team_view')
 
  t.formationTeamView.Init(t.transform:Find('core'),FormationTeamType.pveFirstTeam,false)
  t.formationTeamView.transform:SetAsFirstSibling()
  t.scrollContent = t.transform:Find('core/Scroll View/Viewport/Content'):GetComponent(typeof(Common.UI.Components.ScrollContent))
  t.scrollContent:Init(20,false,0)
end

function t.OnDestroy()
  t.formationTeamView.OnDestroy()
end

-- --------------------click event---------------------------------------
function t.OnClickBackBtnHandler()
  t.OnDestroy()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end

Start()

return t