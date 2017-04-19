local role_info_view ={}
local PREFAB_PATH = 'ui/mine_battle/player_info_view'
local common_hero_icon = require('ui/common_icon/common_hero_icon')
local vipData = gamemanager.GetData('vip_data')
local vipModel = gamemanager.GetModel('vip_model')
local confirm_error_tip_view = require('ui/tips/view/common_error_tips_view')
local mineModel = gamemanager.GetModel('mine_model')
role_info_view.m_imgIcon =nil
role_info_view.m_btnCapture= nil
role_info_view.m_btnRob= nil
role_info_view.m_btnClose= nil
role_info_view.m_labLv= nil
role_info_view.m_labName = nil
role_info_view.m_labTime = nil
role_info_view.m_labFight = nil
role_info_view.m_labFormation = nil
role_info_view.m_labGold = nil
role_info_view.m_labFight = nil
role_info_view.m_gridHero = nil
role_info_view.m_roleInfo = nil
local m_curFormationData =nil

function role_info_view.Open()
  uimanager.RegisterView(PREFAB_PATH,role_info_view)
  local obj = Logic.UI.UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay)
  role_info_view.transform = obj:GetComponent(typeof(Transform))
  role_info_view.InitComp()
  role_info_view.BindDelegate()
  role_info_view.m_roleInfo = mineModel.m_curEnemyRoleInfo
  gamemanager.GetCtrl('mine_controller').GetOccRoleInfoReq(mineModel.m_curMineInfo.mineNo,role_info_view.m_roleInfo.roleId)
end

function role_info_view.InitComp() 
  role_info_view.m_imgIcon = role_info_view.transform:Find('core/mine_player_info/icon_bg/img_icon'):GetComponent(typeof(Image))
  role_info_view.m_labLv  = role_info_view.transform:Find('core/mine_player_info/text_lv'):GetComponent(typeof(Text))
  role_info_view.m_labName = role_info_view.transform:Find('core/mine_player_info/text_name'):GetComponent(typeof(Text))
  role_info_view.m_labTime = role_info_view.transform:Find('core/mine_player_info/text_time/text_num'):GetComponent(typeof(Text))
  role_info_view.m_labGold = role_info_view.transform:Find('core/mine_player_info/text_gold/text_num'):GetComponent(typeof(Text)) 
  role_info_view.m_btnCapture = role_info_view.transform:Find('core/mine_player_info/btn_capture'):GetComponent(typeof(Button))
  role_info_view.m_btnRob= role_info_view.transform:Find('core/mine_player_info/btn_rob'):GetComponent(typeof(Button))
  role_info_view.m_btnClose = role_info_view.transform:Find('core/btn_close'):GetComponent(typeof(Button))
  role_info_view.m_labFight = role_info_view.transform:Find('core/hero_list/img_battle/text_num'):GetComponent(typeof(Text)) 
  role_info_view.m_labFormation = role_info_view.transform:Find('core/hero_list/text_formation/text_num'):GetComponent(typeof(Text))
 
  role_info_view.m_gridHero = role_info_view.transform:Find('core/hero_list/grid'):GetComponent(typeof(GridLayoutGroup))   
  role_info_view.m_btnCapture.onClick:AddListener(role_info_view.OnClickCapture)
  role_info_view.m_btnRob.onClick:AddListener(role_info_view.OnClickRob)
  role_info_view.m_btnClose.onClick:AddListener(role_info_view.OnClickClose)
end

function role_info_view.BindDelegate()
  mineModel.OnRoleMineUpdateDelegate:AddListener(role_info_view.RefreshEnemyInfo)
end

function role_info_view.UnbindDelegate()
  mineModel.OnRoleMineUpdateDelegate:RemoveListener(role_info_view.RefreshEnemyInfo)
end

function role_info_view.RefreshEnemyInfo()
  print("mineModel.m_curFormationNo===",mineModel.m_curFormationNo)
  m_curFormationData = gamemanager.GetData('formation_data').GetDataById(mineModel.m_curFormationNo)
  role_info_view.OnRefreshHero()
  role_info_view.SetEnemyRoleInfo()
end

function role_info_view.OnClickCapture()
  local currentVIPData = vipData.GetVIPData (vipModel.vipLevel)
  if currentVIPData.plunder_occ-mineModel.m_selfMineInfo.occTime <= 0 then
    confirm_error_tip_view.Open(LocalizationController.instance:Get('ui.mine_view.capture_use_up'))   
    do return end
  end
  gamemanager.GetCtrl('mine_controller').RobMineReq(mineModel.m_curMineInfo.mineNo,role_info_view.m_roleInfo.roleId)
end
function role_info_view.OnClickRob()
  local currentVIPData = vipData.GetVIPData (vipModel.vipLevel)
  if currentVIPData.plunder_num-mineModel.m_selfMineInfo.plunderTime <= 0 then
    confirm_error_tip_view.Open(LocalizationController.instance:Get('ui.mine_view.rob_use_up'))   
    do return end
  end
  gamemanager.GetCtrl('mine_controller').PlunderMineReq(mineModel.m_curMineInfo.mineNo,role_info_view.m_roleInfo.roleId)
end
function role_info_view.OnClickClose()
  coroutine.stop(role_info_view.UpdateCoroutine)
  role_info_view.UnbindDelegate()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end
function role_info_view.Close()
  role_info_view.OnClickClose()
end
function role_info_view.OnRefreshHero()
    local player = common_hero_icon.New(role_info_view.m_gridHero.transform)
    player:SetRoleInfo(mineModel.m_curRolePlayerInfo,true)
    for index, hero in pairs(mineModel.m_curAryRoleHeroInfo) do
      local he = common_hero_icon.New(role_info_view.m_gridHero.transform)
      he:SetRoleInfo(hero,false)
    end
end
function role_info_view.SetEnemyRoleInfo()
    role_info_view.m_imgIcon.sprite = ResMgr.instance:LoadSprite(ui_util.ParseHeadIcon(role_info_view.m_roleInfo.headNo))
    role_info_view.m_labLv.text = role_info_view.m_roleInfo.roleLv
    role_info_view.m_labName.text = role_info_view.m_roleInfo.roleName
    role_info_view.m_labFight.text = role_info_view.m_roleInfo.fightingPower
    role_info_view.m_labGold.text = mineModel.m_curRoleGold
    role_info_view.m_labFormation.text =LocalizationController.instance:Get(m_curFormationData.name)
    role_info_view.UpdateCoroutine = coroutine.start(role_info_view.UpdateView)
end
function role_info_view.RefreshTime()
  local time = mineModel.m_curRoleProtectTime
  role_info_view.m_labTime.text = TimeUtil.FormatSecondToHour(time)
end
function role_info_view.UpdateView()
  while(true) do
    role_info_view.RefreshTime()
    coroutine.wait(1)
  end
end
return role_info_view
