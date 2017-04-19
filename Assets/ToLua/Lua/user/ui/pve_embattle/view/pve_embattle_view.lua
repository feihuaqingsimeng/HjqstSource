local t = {}
local PREFAB_PATH = 'ui/pve_embattle/pve_embattle_view_lua'
local name = PREFAB_PATH

local common_hero_icon = require('ui/common_icon/common_hero_icon')
local hero_model = gamemanager.GetModel('hero_model')
local player_model = gamemanager.GetModel('player_model')
local formation_model = gamemanager.GetModel('formation_model')
local formation_controller = gamemanager.GetCtrl('formation_controller')
local function_open_model = gamemanager.GetModel('function_open_model')
local auto_destroy_tip_view = require("ui/tips/view/auto_destroy_tip_view")


--changeTeamCallbackFunc 传参一个，选中的当前队（1、2还是3队）,isReadyFight:是否有开始战斗按钮,readyFightCallback：点击开始战斗回调,()无参
function t.Open(changeTeamCallbackFunc,isReadyFight,readyFightCallback)
  uimanager.RegisterView(name,t)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
  t.changeTeamCallbackFunc = changeTeamCallbackFunc
  
  local common_top_bar = require ('ui/common_top_bar/common_top_bar')
  t.common_top_bar = common_top_bar:Create(t.transform:Find('core'))
  t.common_top_bar:SetAsCommonStyle(LocalizationController.instance:Get("ui.pve_embattle_view.title"),t.ClickCloseHandler,true,true,true,false,false,false,false)
  
  t.isReadyFight = isReadyFight
  t.readyFightCallback = readyFightCallback
  t.formationTeamType = formation_model.CurrentPVESelectFormationTeamType
  t.scrollItems = {}
  t.selectInstanceId = 0
  t.goToggles = {}
  t.currentToggleIndex = 1
  
  t.formationTeamTypeList = {}
  t.formationTeamTypeList[1] = FormationTeamType.pveFirstTeam
  t.formationTeamTypeList[2] = FormationTeamType.pveSecondTeam
  t.formationTeamTypeList[3] = FormationTeamType.pveThirdTeam
  for k,v in pairs(t.formationTeamTypeList) do
    if t.formationTeamType == v then
      t.currentToggleIndex = k
      break
    end
  end
  
  t.formationTeamView = dofile('ui/formation/view/formation_team_view')
  t.formationTeamView.Init(t.transform:Find('core'),t.formationTeamType,false)
  t.formationTeamView.selectRoleCallbackDelegate:AddListener(t.SelectRoleCallbackHandler)
  t.formationTeamView.transform:SetAsFirstSibling()
  t.BindDelegate()
  
  t.InitComponent()
  t.InitScrollContent()
  t.goToggles[t.currentToggleIndex].transform.parent:SetAsFirstSibling()
  t.goToggles[t.currentToggleIndex]:GetComponent(typeof(Toggle)).isOn = true
  t.ClickToggleHandler(t.goToggles[t.currentToggleIndex])
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
  if t.changeTeamCallbackFunc then
    t.changeTeamCallbackFunc(t.formationTeamType)
  end
end

function t.BindDelegate()
  hero_model.onHeroInfoListUpdateDelegate:AddListener(t.onHeroInfoListUpdateByProtocol)
  player_model.OnPlayerInfoUpdateDelegate:AddListener(t.onHeroInfoListUpdateByProtocol)
end

function t.UnbindDelegaete()
  hero_model.onHeroInfoListUpdateDelegate:RemoveListener(t.onHeroInfoListUpdateByProtocol)
  player_model.OnPlayerInfoUpdateDelegate:RemoveListener(t.onHeroInfoListUpdateByProtocol)
end

function t.InitComponent()
  t.canvas = t.transform:GetComponentInParent(typeof(UnityEngine.Canvas))
  t.scrollContent = t.transform:Find('core/Scroll View/Viewport/Content'):GetComponent(typeof(Common.UI.Components.ScrollContent))
  t.scrollContent.onResetItem:AddListener(t.OnResetItemHandler)
  
  local toggleRoot = t.transform:Find('core/team_toggles_root')
  t.goToggles[1] = toggleRoot:Find('toggle_team_1').gameObject
  t.goToggles[2] = toggleRoot:Find('toggle_team_2').gameObject
  t.goToggles[3] = toggleRoot:Find('toggle_team_3').gameObject
  
  for k,v in pairs(t.goToggles) do
    local toggleDelegate = v:GetComponent(typeof(Common.UI.Components.EventTriggerDelegate))
    toggleDelegate.onClick:RemoveAllListeners()
    toggleDelegate.onClick:AddListener(t.ClickToggleHandler)
  end
  t.btnReadyFight = t.transform:Find('core/btn_fight'):GetComponent(typeof(Button))
  t.btnReadyFight.onClick:AddListener(t.ClickReadyFightBtnHandler)
  t.btnReadyFight.gameObject:SetActive(t.isReadyFight)
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
  table.sort(t.heroInfoList,hero_model.CompareHeroByQualityDescConsiderInFormationHero)
  t.scrollContent:Init(count,false,0)
end

--列表刷新
function t.RefreshScrollContent()
  t.scrollContent:RefreshAllContentItems()
end
--设置common_top_bar 数据
function t.SetCommonTopBarAsCommonStyle(showPveActionItem,showGoldItem,showDiamondItem,showHonorItem,showPVPActionItem,showExpeditionItem,showWorldTreeFruitItem)
  t.common_top_bar:SetAsCommonStyle2(showPveActionItem,showGoldItem,showDiamondItem,showHonorItem,showPVPActionItem,showExpeditionItem,showWorldTreeFruitItem)
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
  --item:AddRoleDesButton()
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

--点击toggle
function t.ClickToggleHandler(go)
  local index = 0
  for k,v in pairs(t.goToggles) do
    if v == go then
      index = k
      break
    end
  end
  if t.currentToggleIndex == index then
    return
  end
  
  if index == 2 and not function_open_model.IsFunctionOpen(FunctionOpenType.HeroSecondTeam,true) then
    t.goToggles[t.currentToggleIndex]:GetComponent(typeof(Toggle)).isOn = true
    t.goToggles[index]:GetComponent(typeof(Toggle)).isOn = false
    return
  elseif index == 3 and not function_open_model.IsFunctionOpen(FunctionOpenType.HeroThirdTeam,true) then
    t.goToggles[t.currentToggleIndex]:GetComponent(typeof(Toggle)).isOn = true
    t.goToggles[index]:GetComponent(typeof(Toggle)).isOn = false
    return 
  end
  t.formationTeamType = t.formationTeamTypeList[index]
  t.currentToggleIndex = index 
  t.formationTeamView.RefreshView(t.formationTeamType)
  formation_model.SetCurrentPVESelectFormationTeamType(t.formationTeamType)--当前队
  formation_controller.TeamChangeReq(t.formationTeamType)
end

--选择上阵英雄后的回调
function t.SelectRoleCallbackHandler(roleId,formationIndex)
  --print('SelectRoleCallbackHandler',roleId,formationIndex)
  t.selectInstanceId = roleId
  table.sort(t.heroInfoList,hero_model.CompareHeroByQualityDescConsiderInFormationHero)
  t.RefreshScrollContent()
end
--点击开始战斗
function t.ClickReadyFightBtnHandler()
  if t.readyFightCallback then
    t.readyFightCallback()
  end
end
----------------------------update by protocol-----------------------------------
function t.onHeroInfoListUpdateByProtocol()
  t.InitScrollContent()
  t.formationTeamView.RefreshAll()
end
function t.ClickCloseHandler()
  uimanager.CloseView(name)
end
return t