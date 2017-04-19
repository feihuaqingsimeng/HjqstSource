local otherMineView ={}
local mineModel = gamemanager.GetModel('mine_model')
local mine_role_view = require('ui/mine/view/mine_role_view')
local confirm_error_tip_view = require('ui/tips/view/common_error_tips_view')
local vipData = gamemanager.GetData('vip_data')
local vipModel = gamemanager.GetModel('vip_model')
local PREFAB_PATH = 'ui/mine_battle/other_mine_view'
otherMineView.m_labMineName = nil
otherMineView.m_imgIcon = nil
otherMineView.m_labReward = nil
otherMineView.m_labNum = nil
otherMineView.m_btnClose = nil
otherMineView.m_btnCapture = nil
otherMineView.m_btnChange = nil
otherMineView.m_objOtherRole = nil
otherMineView.m_objRoleParent = nil
otherMineView.m_aryRoleInfo = {}
local m_curMineData =nil

function otherMineView.Open()
  local obj = Logic.UI.UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay)
  otherMineView.transform = obj:GetComponent(typeof(Transform))
  otherMineView.InitComp()
  otherMineView.BindDelegate()
  gamemanager.GetCtrl('mine_controller').GetMineInfoReq(mineModel.m_curMineInfo.mineNo)
  uimanager.RegisterView(PREFAB_PATH,otherMineView)
end

function otherMineView.InitComp() 
  otherMineView.m_labMineName = otherMineView.transform:Find('core/top/text_title'):GetComponent(typeof(Text))
  otherMineView.m_imgIcon = otherMineView.transform:Find('core/top/mine_info/spr_icon'):GetComponent(typeof(Image))
  otherMineView.m_labReward = otherMineView.transform:Find('core/top/mine_info/text_output/text_num'):GetComponent(typeof(Text))
  otherMineView.m_labNum =  otherMineView.transform:Find('core/top/mine_info/text_people/text_num'):GetComponent(typeof(Text))
  otherMineView.m_btnClose = otherMineView.transform:Find('core/top/btn_close'):GetComponent(typeof(Button))
  otherMineView.m_btnCapture = otherMineView.transform:Find('core/top/mine_info/btn_capture'):GetComponent(typeof(Button))
  otherMineView.m_btnChange = otherMineView.transform:Find('core/bottom/btn_change'):GetComponent(typeof(Button))
  otherMineView.m_objOtherRole = otherMineView.transform:Find('core/bottom/battle_info_scroll/Viewport/mine_player_info').gameObject
  otherMineView.m_objRoleParent = otherMineView.transform:Find('core/bottom/battle_info_scroll/Viewport/Content').gameObject
  m_curMineData = mineModel.GetMineData(mineModel.m_curMineInfo.mineNo) 
  otherMineView.m_btnClose.onClick:AddListener(otherMineView.OnClickClose)
  otherMineView.m_btnCapture.onClick:AddListener(otherMineView.OnClickCapture)
  otherMineView.m_btnChange.onClick:AddListener(otherMineView.OnClickChange)
end

function otherMineView.BindDelegate()
  mineModel.OnOtherMineCloseDelegate:AddListener(otherMineView.OnClickClose)
  mineModel.OnOtherMineUpdataDelegate:AddListener(otherMineView.RefreshOtherMineUI)
end

function otherMineView.UnbindDelegate()
  mineModel.OnOtherMineCloseDelegate:RemoveListener(otherMineView.OnClickClose)
  mineModel.OnOtherMineUpdataDelegate:RemoveListener(otherMineView.RefreshOtherMineUI)
end

function otherMineView.RefreshOtherMineUI()
    otherMineView.RefreshAllRole()
    otherMineView.SetCurMineInfo()
end

function otherMineView.RefreshAllRole() 
  print("mineModel.m_aryMineRoles.length:",#mineModel.m_aryMineRoles)
  for key,value in pairs(mineModel.m_aryMineRoles) do
    local roleObj = GameObject.Instantiate(otherMineView.m_objOtherRole.gameObject)
    roleObj.gameObject:SetActive(true)
    roleObj.transform:SetParent(otherMineView.m_objRoleParent.transform,false)
    local roleInfo =mine_role_view.NewByGameObject(roleObj,m_curMineData.time)
    table.insert(otherMineView.m_aryRoleInfo,roleInfo)
    roleInfo.onClick:AddListener(otherMineView.OnClickRoleLook)
    roleInfo:SetRoleInfo(value)
  end
  if #otherMineView.m_aryRoleInfo >= m_curMineData.man_max and m_curMineData.man_max <999 then
    otherMineView.m_btnCapture.gameObject:SetActive(false)
  else
    otherMineView.m_btnCapture.gameObject:SetActive(true)
  end
  otherMineView.m_objOtherRole.gameObject:SetActive(false)
end

function otherMineView.OnClickRoleLook(roleInfo)
   mineModel.m_curEnemyRoleInfo = roleInfo
   gamemanager.GetCtrl('mine_controller').OpenRoleInfoView()
end

function otherMineView.SetCurMineInfo()
  otherMineView.m_labMineName.text = LocalizationController.instance:Get(string.format("ui.mine_view.mine_view_mine_%d",m_curMineData.quality))
  otherMineView.m_imgIcon.sprite = ResMgr.instance:LoadSprite(string.format('sprite/main_ui/image_ore_%d',m_curMineData.quality))
  otherMineView.m_labReward.text  = tostring(m_curMineData.foundation*m_curMineData.time/600)
  if m_curMineData.man_max < 999 then
    otherMineView.m_btnChange.gameObject:SetActive(false)
    otherMineView.m_labNum.text = mineModel.m_curMineInfo.occNum..'/'..m_curMineData.man_max
  else
    otherMineView.m_btnChange.gameObject:SetActive(true)
    otherMineView.m_labNum.text =LocalizationController.instance:Get('ui.mine_view.mine_view_unlimite')
  end
end
function otherMineView.OnClickCapture()
  local currentVIPData = vipData.GetVIPData (vipModel.vipLevel)
  if currentVIPData.plunder_occ-mineModel.m_selfMineInfo.occTime <= 0 then
    confirm_error_tip_view.Open(LocalizationController.instance:Get('ui.mine_view.capture_use_up'))   
    do return end
  end
  gamemanager.GetCtrl('mine_controller').OccMineReq(mineModel.m_curMineInfo.mineNo)
end

function otherMineView.Close()
  otherMineView.OnClickClose() 
end

function otherMineView.OnClickClose() 
  for index,role in pairs(otherMineView.m_aryRoleInfo) do
    role:Close()
  end
  while table.getn(otherMineView.m_aryRoleInfo) > 0 do
     local role = table.remove(otherMineView.m_aryRoleInfo,1)
     GameObject.Destroy(role.transform.gameObject)
  end
  otherMineView.m_aryRoleInfo={}
  otherMineView.UnbindDelegate()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end

function otherMineView.OnClickChange()
  for index,role in pairs(otherMineView.m_aryRoleInfo) do
    role:Close()
  end
  while table.getn(otherMineView.m_aryRoleInfo) > 0 do
     local role = table.remove(otherMineView.m_aryRoleInfo,1)
     GameObject.Destroy(role.transform.gameObject)
  end
  otherMineView.m_aryRoleInfo={}
  gamemanager.GetCtrl('mine_controller').ChangeMineInfoReq(mineModel.m_curMineInfo.mineNo)
end
return otherMineView