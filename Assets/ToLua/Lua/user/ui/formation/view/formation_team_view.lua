local t = {}

local PREFAB_PATH = 'ui/formation/formation_team_view'
--选中角色时回调 参数(roleId,formationIndex)
t.selectRoleCallbackDelegate = void_delegate.New()

local formation_model = gamemanager.GetModel('formation_model')
local auto_destroy_tip_view = require('ui/tips/view/auto_destroy_tip_view')

function t.Init(transformParent,formationTeamType,playerCanLeaveTeam)
  
  t.playerCanLeaveTeam = playerCanLeaveTeam
  t.isRightPanelShowing = true
  t.formationTeamInfo = gamemanager.GetModel('formation_model').GetFormationTeam( formationTeamType)
  
  local gameObject = Common.ResMgr.ResMgr.instance:Load(PREFAB_PATH)
  t.transform = GameObject.Instantiate(gameObject):GetComponent(typeof(Transform))
  t.transform:SetParent(transformParent,false)
  
  t.selectRoleId = 0
  t.oldPower = -1
  t.InitComponent()
  t.BindDelegate()
  
  t.formationGridView = dofile('ui/formation/view/formation_grid_view')
  
  t.formationGridView.Create(t.transform:Find('formation_root'),formationTeamType,false)
  t.formationGridView.selectRoleCallbackDelegate:AddListener(t.selectRoleCallbackFunc)
  t.Refresh()
  
end

function t.Close()
  t.formationGridView.Close()
  t.UnbindDelegate()
end


function t.BindDelegate()
  gamemanager.GetModel('formation_model').FormationChangeDelegate:AddListener(t.Refresh)
  gamemanager.GetModel('formation_model').FormationUpgradeDelegate:AddListener(t.UpdatePower)
  gamemanager.GetModel('formation_model').FormationAdditionAttrActiveDelegate:AddListener(t.UpdatePower)
end
function t.UnbindDelegate()
  gamemanager.GetModel('formation_model').FormationChangeDelegate:RemoveListener(t.Refresh)  
  gamemanager.GetModel('formation_model').FormationUpgradeDelegate:RemoveListener(t.UpdatePower)
  gamemanager.GetModel('formation_model').FormationAdditionAttrActiveDelegate:RemoveListener(t.UpdatePower)
end


function t.InitComponent()
  t.canvas = t.transform:GetComponentInParent(typeof(UnityEngine.Canvas))
  t.numberIncreaseActionPower = t.transform:Find('power_root/text_power'):GetComponent(typeof(NumberIncreaseAction))
  
  local btnCancelSelect = t.transform:Find('btn_cancel_select'):GetComponent(typeof(Button))
  btnCancelSelect.onClick:AddListener(t.ClickCancelSelectBtnHandler)
  
  t.btnChangeFormation = t.transform:Find('btn_formation'):GetComponent(typeof(Button))
  t.btnChangeFormation.onClick:AddListener(t.ClickChangeFormationBtnHandler)
  t.goFormationLock = t.btnChangeFormation.transform:Find('img_lock').gameObject
  --pos
  
  t.btnDetail = t.transform:Find('btn_detial'):GetComponent(typeof(Button))
  t.btnDetail.onClick:AddListener(t.ClickShowDetailHandler)
  t.btnRemove = t.transform:Find('btn_remove_role'):GetComponent(typeof(Button))
  t.btnRemove.onClick:AddListener(t.ClickRemoveBtnHandler)
  t.btnRemove.gameObject:SetActive(false)
  t.transformRightPanel = t.transform:Find('right_panel_root/right_root/right_panel')
  local btnRoleInfo = t.transformRightPanel:Find('btn_roles_info'):GetComponent(typeof(Button))
  btnRoleInfo.onClick:AddListener(t.ClickRoleInfoBtnHandler)
  
  local btnShowHide = t.transformRightPanel:Find('btn_show_hide'):GetComponent(typeof(Button))
  btnShowHide.onClick:AddListener(t.ClickShowHideHandler)
  
  t.transformRightPanelHide = t.transform:Find('right_panel_root/right_root/right_panel_hide_pos')
  t.transformRightPanelShow = t.transform:Find('right_panel_root/right_root/right_panel_show_pos')
  
  t.goCantRemovePlayerTipsRoot = t.transform:Find('cant_remove_player_tips_panel').gameObject
  t.goCantRemovePlayerTipsRoot:SetActive(false)
  
  -- [[ POWER EFFECT ]] --
  t.goEffectShuZhiShuaXinUp = t.transform:Find('power_root/ui_shuzhishuaxin_shang').gameObject
  particle_util.ChangeParticleSortingOrderByCanvas(t.goEffectShuZhiShuaXinUp,t.canvas)
  t.goEffectShuZhiShuaXinUp:SetActive(false)
  
  t.goEffectShuZhiShuaXinDown = t.transform:Find('power_root/ui_shuzhishuaxin_xia').gameObject
  particle_util.ChangeParticleSortingOrderByCanvas(t.goEffectShuZhiShuaXinDown,t.canvas)
  t.goEffectShuZhiShuaXinDown:SetActive(false)
  -- [[ POWER EFFECT ]] --
end

function t.Refresh()
  t.UpdateChangeFormationBtn()
  t.UpdatePower()
end
function t.RefreshAll()
  t.Refresh()
  t.formationGridView.Refresh()
end
--刷新界面
function t.RefreshView(formationTeamType)
  t.formationTeamInfo = gamemanager.GetModel('formation_model').GetFormationTeam( formationTeamType)
  t.formationGridView.RefreshView(formationTeamType)
  t.Refresh()
end
--选中了英雄调用
function t.SelectRole(roleId)
  t.formationGridView.SelectRole(roleId)
end

--战力更新
function t.UpdatePower()
  local power = t.formationTeamInfo:Power()
  print(t.oldPower,power)
  local beforePower = math.max(0,t.oldPower)
  if beforePower ~= power then
    t.numberIncreaseActionPower:Init(beforePower,power,0)
  end
  if t.oldPower ~= -1 and t.oldPower ~= power then
    if power > t.oldPower then
      t.goEffectShuZhiShuaXinUp:SetActive(false)
      t.goEffectShuZhiShuaXinUp:SetActive(true)
    elseif power < t.oldPower then
      t.goEffectShuZhiShuaXinDown:SetActive(false)
      t.goEffectShuZhiShuaXinDown:SetActive(true)
    end
  end
  t.oldPower = power
end
--小阵型图标更新
function t.UpdateChangeFormationBtn()
  local posRoot = t.btnChangeFormation.transform:Find('pos')
  local childCount = posRoot.childCount
  for i = 1,childCount do
    posRoot:GetChild(i-1).gameObject:SetActive(t.formationTeamInfo.formationInfo.formationData.pos[i])
  end
  local isOpen = gamemanager.GetModel('function_open_model').IsFunctionOpen(FunctionOpenType.FormationTraining,false)
  if t.formationTeamInfo.teamType ~= FormationTeamType.pveFirstTeam and t.formationTeamInfo.teamType ~= FormationTeamType.pveSecondTeam and t.formationTeamInfo.teamType ~= FormationTeamType.pveThirdTeam and not isOpen then
    t.goFormationLock:SetActive(true)
  else
    t.goFormationLock:SetActive(false)
  end
end

------------------click event------------------------
function t.selectRoleCallbackFunc(roleId,formationIndex)
  t.selectRoleId = roleId
  LuaInterface.LuaCsTransfer.GetRoleDesButton(t.btnDetail.gameObject, t.selectRoleId,0,false,false)
  local gameModel = gamemanager.GetModel('game_model')
  if roleId == 0 then
    t.btnRemove.gameObject:SetActive(false)
  else
    local isInTeam = formation_model.IsHeroInTeamByType(t.formationTeamInfo.teamType,roleId)
    local isPlayer = gameModel.IsPlayer(roleId)
    
    if isPlayer then
      t.btnRemove.gameObject:SetActive(t.playerCanLeaveTeam and isInTeam)
      if t.playerCanLeaveTeam == false then
        t.goCantRemovePlayerTipsRoot:SetActive(true)
        local gridPos = t.formationGridView.GetGridPosition(formationIndex)
        t.goCantRemovePlayerTipsRoot.transform.position = Vector3(gridPos.x,gridPos.y,t.goCantRemovePlayerTipsRoot.transform.position.z)
        CommonFadeToAnimation.Get(t.goCantRemovePlayerTipsRoot):init(1, 0, 2.5, 0)
      end
    else 
      t.btnRemove.gameObject:SetActive(isInTeam)
    end
  end
  if formationIndex ~= 0 then
    local formationGridBtnPosition = t.formationGridView.btnFormationTable[formationIndex].transform.position
    t.btnRemove.transform.position = Vector3(formationGridBtnPosition.x,formationGridBtnPosition.y-0.1,t.btnRemove.transform.position.z)

  end
  t.selectRoleCallbackDelegate:InvokeTwoParam(roleId,formationIndex)
end

function t.ClickCancelSelectBtnHandler()
  t.formationGridView.SelectRole(0)
end

function t.ClickChangeFormationBtnHandler()
  --Logic.UI.UIMgr.instance:Open('ui/train_formation/train_formation_view',EUISortingLayer.MainUI,UIOpenMode.Overlay)
  gamemanager.GetModel('formation_controller').OpenTrainingFormationView(t.formationTeamInfo.teamType)
end

function t.ClickRemoveBtnHandler()
  if t.formationGridView.selectedRoleId ~= 0 then
    
    t.formationTeamInfo:RemoveHeroByHeroInstanceID(t.formationGridView.selectedRoleId)
    -------------need send protocol to save team------------------
    gamemanager.GetCtrl('formation_controller').TeamChangeReq(t.formationTeamInfo.teamType)
    
    t.formationGridView.Refresh()
  end
end
function t.ClickRoleInfoBtnHandler()
  if t.selectRoleId == 0 then
    gamemanager.GetCtrl('hero_controller').OpenHeroInfoView()
  else
    gamemanager.GetCtrl('hero_controller').OpenHeroInfoView(t.selectRoleId)
  end
end

--右边的按钮显隐
function t.ClickShowHideHandler()
  if t.isRightPanelShowing then 
    LeanTween.moveLocalX(t.transformRightPanel.gameObject,t.transformRightPanelHide.localPosition.x,0.25)
  else 
    LeanTween.moveLocalX(t.transformRightPanel.gameObject,t.transformRightPanelShow.localPosition.x,0.25)
  end
  
  t.isRightPanelShowing = not t.isRightPanelShowing
end

--点击显示英雄详情
function t.ClickShowDetailHandler()
  if t.selectRoleId == 0 then
    auto_destroy_tip_view.Open(LocalizationController.instance:Get('ui.pve_embattle_view.notChoiceHero'))
  else
    LuaInterface.LuaCsTransfer.GetRoleDesButton(t.btnDetail.gameObject, t.selectRoleId,0,false,false)
  end
end

return t