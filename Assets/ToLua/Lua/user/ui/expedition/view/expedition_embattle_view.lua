local t = {}
local PREFAB_PATH = 'ui/expedition/expedition_embattle_view_lua'
local name = PREFAB_PATH

local common_hero_icon = require('ui/common_icon/common_hero_icon')
local hero_model = gamemanager.GetModel('hero_model')
local player_model = gamemanager.GetModel('player_model')
local formation_controller = gamemanager.GetCtrl('formation_controller')
local formation_model = gamemanager.GetModel('formation_model')
local expedition_model = gamemanager.GetModel('expedition_model')
local expedition_controller = gamemanager.GetCtrl('expedition_controller')
local expedition_role_button = require('ui/expedition/view/expedition_role_button')
local auto_destroy_tip_view = require("ui/tips/view/auto_destroy_tip_view")

function t.Open(isReadyFight)
  uimanager.RegisterView(name,t)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  local common_top_bar = require ('ui/common_top_bar/common_top_bar')
  common_top_bar = common_top_bar:Create(t.transform:Find('core'))
  common_top_bar:SetAsCommonStyle(LocalizationController.instance:Get("ui.expedition_formation_view.titleText"),t.ClickCloseHandler,true, true, true, false, false, true,false)
  
  t.formationTeamType = FormationTeamType.expeditionTeam
  t.scrollItems = {}
  t.selectInstanceId = 0
  t.isReadyFight = isReadyFight
  
  
  t.formationTeamView = dofile('ui/formation/view/formation_team_view')
  t.formationTeamView.Init(t.transform:Find('core'),t.formationTeamType,true)
  t.formationTeamView.selectRoleCallbackDelegate:AddListener(t.SelectRoleCallbackHandler)
  t.formationTeamView.transform:SetAsFirstSibling()
  
  t.BindDelegate()
  
  t.InitComponent()
  t.InitScrollContent()
   -- 特效
  coroutine.start(function()
      coroutine.wait(0.2)
      particle_util.CreateParticle('effects/formationEffect/Effects/liuxingbeijing',t.canvas.sortingLayerName,t.canvas.sortingOrder-100,t.transform:Find('bg_core'))
    end)
end

function t.Close()
  t.formationTeamView.Close()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegaete()
end

function t.BindDelegate()
  hero_model.onHeroInfoListUpdateDelegate:AddListener(t.onHeroInfoListUpdateByProtocol)
  player_model.OnPlayerInfoUpdateDelegate:AddListener(t.onHeroInfoListUpdateByProtocol)
  Observers.Facade.Instance:RegisterObserver('Expedition_Challenge_RESP_lua',t.Expedition_Challenge_RESP)
end

function t.UnbindDelegaete()
  hero_model.onHeroInfoListUpdateDelegate:RemoveListener(t.onHeroInfoListUpdateByProtocol)
  player_model.OnPlayerInfoUpdateDelegate:RemoveListener(t.onHeroInfoListUpdateByProtocol)
  Observers.Facade.Instance:RemoveObserver('Expedition_Challenge_RESP_lua',t.Expedition_Challenge_RESP)
end

function t.InitComponent()
  t.canvas = t.transform:GetComponentInParent(typeof(UnityEngine.Canvas))
  t.scrollContent = t.transform:Find('core/Scroll View/Viewport/Content'):GetComponent(typeof(Common.UI.Components.ScrollContent))
  t.scrollContent.onResetItem:AddListener(t.OnResetItemHandler)
  t.btnFight = t.transform:Find('core/btn_fight'):GetComponent(typeof(Button))
  t.btnFight.onClick:AddListener(t.ClickFightBtnHandler)
  t.btnFight.gameObject:SetActive(t.isReadyFight)
end
--初始化列表
function t.InitScrollContent()
  t.heroInfoList = expedition_model.GetEnableExpeditionHeroList()
  
  table.sort(t.heroInfoList,function(a,b)
      local aNum = 0
      local bNum = 0
      if a:IsPlayer() then
        aNum = 1
      end
      if b:IsPlayer() then
        bNum = 1
      end
      if aNum ~= bNum then
        return aNum > bNum
      end
      return hero_model.CompareHeroByQualityDesc(a.roleInfo,b.roleInfo)
    end)
  local count = #t.heroInfoList
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
    item = expedition_role_button.BindGameObject(go)
    item.onClick:AddListener(t.ClickScrollItemHandler)
    t.scrollItems[go] = item
  end
  local info = t.heroInfoList[index+1]
  item:SetExpeditionRoleInfo(info)
  item:SetInFormation(formation_model.IsHeroInTeamByType(t.formationTeamType,info.instanceID))
  item:SetSelect(t.selectInstanceId == info.instanceID)
end
--点击列表中的icon
function t.ClickScrollItemHandler(expeidtionRoleButton)
  if expeidtionRoleButton.expeditionRoleInfo:IsDead() then
    auto_destroy_tip_view.Open(LocalizationController.instance:Get('ui.expedition_formation_view.die_not_add_Team'))
    return 
  end
  
  local id = expeidtionRoleButton.expeditionRoleInfo.instanceID
  if t.selectInstanceId == id then
    return
  end
  t.formationTeamView.SelectRole(id)
end

--选择上阵英雄后的回调
function t.SelectRoleCallbackHandler(roleId,formationIndex)
  t.selectInstanceId = roleId
  t.RefreshScrollContent()
end
--战斗
function t.ClickFightBtnHandler()
 -- LuaCsTransfer.SendArenaChanllengReq()
 if t.isReadyFight then
    local teamInfo = formation_model.GetFormationTeam( t.formationTeamType)
    local count = teamInfo:Count()
    if count == 0 then
      auto_destroy_tip_view.Open(LocalizationController.instance:Get("ui.expedition_formation_view.notEnoughHero"))
      return
    end
  end
  expedition_controller.ExpeditionChallengeReq(expedition_model.currentExpeditionDungeonId)
end

----------------------------update by protocol-----------------------------------
function t.onHeroInfoListUpdateByProtocol()
  t.InitScrollContent()
  t.formationTeamView.RefreshAll()
end
function t.ClickCloseHandler()
  uimanager.CloseView(name)
end
function t.Expedition_Challenge_RESP(note)
  print('Expedition_Challenge_RESP')
  t.ClickCloseHandler()
  return true
end

return t