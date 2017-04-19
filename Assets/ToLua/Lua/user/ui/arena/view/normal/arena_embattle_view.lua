local t = {}
local PREFAB_PATH = 'ui/pvp/pvp_embattle_view_lua'
local name = PREFAB_PATH

local common_hero_icon = require('ui/common_icon/common_hero_icon')
local hero_model = gamemanager.GetModel('hero_model')
local player_model = gamemanager.GetModel('player_model')
local formation_model = gamemanager.GetModel('formation_model')

function t.Open(isReadyFight)
  uimanager.RegisterView(name,t)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  local common_top_bar = require ('ui/common_top_bar/common_top_bar')
  common_top_bar = common_top_bar:Create(t.transform:Find('core'))
  common_top_bar:SetAsCommonStyle(LocalizationController.instance:Get("ui.pvp_formation_view.titleText"),t.Close,false,false,true,true,true,false,false)
  
  t.formationTeamType = FormationTeamType.pvpTeam
  t.scrollItems = {}
  t.selectInstanceId = 0
  t.isReadyFight = isReadyFight
  
  t.formationTeamView = dofile('ui/formation/view/formation_team_view')
  t.formationTeamView.Init(t.transform:Find('core'),t.formationTeamType,false)
  t.formationTeamView.selectRoleCallbackDelegate:AddListener(t.SelectRoleCallbackHandler)
  t.formationTeamView.transform:SetAsFirstSibling()
  
  t.BindDelegate()
  
  t.InitComponent()
  t.InitScrollContent()
end

function t.Close()
  t.formationTeamView.Close()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegaete()
end

function t.BindDelegate()
  hero_model.onHeroInfoListUpdateDelegate:AddListener(t.onHeroInfoListUpdateByProtocol)
  player_model.OnPlayerInfoUpdateDelegate:AddListener(t.onHeroInfoListUpdateByProtocol)
  Observers.Facade.Instance:RegisterObserver('RANK_ARENA_CHALLENGE_RESP_LUA',t.RANK_ARENA_CHALLENGE_RESP_LUA)
end

function t.UnbindDelegaete()
  hero_model.onHeroInfoListUpdateDelegate:RemoveListener(t.onHeroInfoListUpdateByProtocol)
  player_model.OnPlayerInfoUpdateDelegate:RemoveListener(t.onHeroInfoListUpdateByProtocol)
  Observers.Facade.Instance:RemoveObserver('RANK_ARENA_CHALLENGE_RESP_LUA',t.RANK_ARENA_CHALLENGE_RESP_LUA)
end

function t.InitComponent()
  t.scrollContent = t.transform:Find('core/Scroll View/Viewport/Content'):GetComponent(typeof(Common.UI.Components.ScrollContent))
  t.scrollContent.onResetItem:AddListener(t.OnResetItemHandler)
  t.btnFight = t.transform:Find('core/btn_fight'):GetComponent(typeof(Button))
  t.btnFight.onClick:AddListener(t.ClickFightBtnHandler)
  t.btnFight.gameObject:SetActive(t.isReadyFight)
end
--初始化列表
function t.InitScrollContent()
  local infoList = hero_model.GetAllHeroInfoList()
  local count = 0
  t.heroInfoList = {}
  for k,v in pairs(infoList) do
    count = count + 1
    t.heroInfoList[count] = v
  end
  table.sort(t.heroInfoList,hero_model.CompareHeroByQualityDesc)
  t.scrollContent:Init(count,false,0)
end

--列表刷新
function t.RefreshScrollContent()
  t.scrollContent:RefreshAllContentItems()
end
--------------------click event------------------------------------------------
function t.OnResetItemHandler(go ,index)
  local item = t.scrollItems[go]
  if not item then
    item = common_hero_icon.NewByGameObject(go)
    item.onClick:AddListener(t.ClickScrollItemHandler)
    t.scrollItems[go] = item
  end
  local info = t.heroInfoList[index+1]
  item:SetRoleInfo(info,false)
  item:AddRoleDesButton()
  item:SetInFormation(formation_model.IsHeroInTeamByType(t.formationTeamType,info.instanceID))
  item:SetSelect(t.selectInstanceId == info.instanceID)
end
--点击列表中的icon
function t.ClickScrollItemHandler(heroIcon)
  if t.selectInstanceId == heroIcon.roleInfo.instanceID then
    return
  end
  t.formationTeamView.SelectRole(heroIcon.roleInfo.instanceID)
end

--选择上阵英雄后的回调
function t.SelectRoleCallbackHandler(roleId,formationIndex)
  --print('SelectRoleCallbackHandler',roleId,formationIndex)
  t.selectInstanceId = roleId
  t.RefreshScrollContent()
end
--战斗
function t.ClickFightBtnHandler()
  LuaCsTransfer.SendArenaChanllengReq()
end

----------------------------update by protocol-----------------------------------
function t.onHeroInfoListUpdateByProtocol()
  t.InitScrollContent()
  t.formationTeamView.RefreshAll()
end
function t.RANK_ARENA_CHALLENGE_RESP_LUA(note)
  uimanager.CloseView(name)
  return true
end
return t