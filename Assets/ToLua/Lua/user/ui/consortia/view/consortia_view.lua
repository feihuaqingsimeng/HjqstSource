local t = {}
local PREFAB_PATH = 'ui/consortia/consortia_view'

local consortia_controller = gamemanager.GetCtrl('consortia_controller')
local consortia_model = gamemanager.GetModel('consortia_model')
local common_errors_tip_view = require('ui/tips/view/common_error_tips_view')
local auto_destroy_tip_view = require('ui/tips/view/auto_destroy_tip_view')

function t.Open (defaultToggleIndex)
  local gameObject = UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
    
  local common_top_bar = require ('ui/common_top_bar/common_top_bar')
  common_top_bar = common_top_bar:Create(t.transform:Find('core'))
  --common_top_bar.transform:SetAsFirstSibling()
  common_top_bar:SetAsCommonStyle(LocalizationController.instance:Get('ui.consortia_view.title'),t.OnClickCloseButtonHandler,true,true,true,true,false,false,false)
  
  t.selectToggleIndex = 0
  t.subPanel = nil
  t.secondSubPanel = nil
  
  t.BindDelegate()
  t.InitComponent()
  
  if defaultToggleIndex == nil then
    defaultToggleIndex = 1
  end
  
  t.goToggles[defaultToggleIndex]:GetComponent(typeof(Toggle)).isOn = true
  t.ClickToggleHandler(t.goToggles[defaultToggleIndex])
end
function t.Close()
  
  t.CloseSecondSubPanel()
  t.CloseSubPanel()
  UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
end
function t.BindDelegate()
  consortia_model.onUpdateExitSuccessDelegate:AddListener(t.Close)
  consortia_model.onUpdateKickedOutDelegate:AddListener(t.onUpdateKickedOutByProtocol)

end
function t.UnbindDelegate()
  consortia_model.onUpdateExitSuccessDelegate:RemoveListener(t.Close)
  consortia_model.onUpdateKickedOutDelegate:RemoveListener(t.onUpdateKickedOutByProtocol)

end
function t.InitComponent()
  
  t.rootPanel = t.transform:Find('core/root_panel')
  local right_root = t.transform:Find('core/scroll_view/viewport/content')
  t.goToggles = {}
  t.goToggles[1] = right_root:Find('Toggle_consortia_info').gameObject
  t.goToggles[2] = right_root:Find('Toggle_manage').gameObject
  
  t.goToggles[3] = right_root:Find('Toggle_donate').gameObject
  t.goToggles[4] = right_root:Find('Toggle_shop').gameObject
  t.goToggles[5] = right_root:Find('Toggle_private_donation').gameObject
  t.goToggles[6] = right_root:Find('Toggle_fight').gameObject
  for k,v in ipairs(t.goToggles) do
    v:GetComponent(typeof(EventTriggerDelegate)).onClick:AddListener(t.ClickToggleHandler)
  end
  
end
function t.CloseSubPanel()
   if t.subPanel then
    t.subPanel.Close()
    t.subPanel = nil
  end
end
function t.OpenBattleSecondPanel()
  t.CloseSubPanel()
  t.CloseSecondSubPanel()
  t.secondSubPanel = dofile('ui/consortia/view/battle/consortia_battle_panel')
  t.secondSubPanel.Open(t.rootPanel)
end
function t.CloseSecondSubPanel()
  if t.secondSubPanel then
    t.secondSubPanel.Close()
    t.secondSubPanel = nil
  end
end
-------------click event----------------------------
function t.OnClickCloseButtonHandler ()
  if t.secondSubPanel then
    print('关闭二级panel')
    t.CloseSecondSubPanel()
    t.OpenSubPanel(t.selectToggleIndex)
  else
    t.Close()
  end
end
function t.ClickToggleHandler(go)
  local index = 1
  for k,v in ipairs(t.goToggles) do
    if v == go then
      index = k
      break
    end
  end
  if t.selectToggleIndex == index then
    return
  end
  --[[
  if index == 5 then
    auto_destroy_tip_view.Open('功能暂未开放')
    t.goToggles[t.selectToggleIndex]:GetComponent(typeof(Toggle)).isOn = true
    t.goToggles[index]:GetComponent(typeof(Toggle)).isOn = false
    return
  end
  ]]--
  t.selectToggleIndex = index 
  
  t.CloseSubPanel()
  t.CloseSecondSubPanel()
  t.OpenSubPanel(index)
end

function t.OpenSubPanel(index)
  if index == 1 then--公会信息
    t.subPanel = dofile('ui/consortia/view/info/consortia_info_panel')
    t.subPanel.Open(t.rootPanel)
  elseif index == 2 then--成员管理
    t.subPanel = dofile('ui/consortia/view/member/consortia_member_panel')
    t.subPanel.Open(t.rootPanel)
  elseif index == 3 then--公会捐献
    t.subPanel = dofile('ui/consortia/view/donate/consortia_donate_panel')
    t.subPanel.Open(t.rootPanel)
  elseif index == 4 then--公会商店
    t.subPanel = dofile('ui/consortia/view/shop/consortia_shop_panel')
    t.subPanel.Open(t.rootPanel)
  elseif index == 5 then--公会求赠
    t.subPanel = dofile('ui/consortia/view/private_donation/private_donation_panel')
    t.subPanel.Open(t.rootPanel)
  elseif index == 6 then--公会对抗
    t.subPanel = dofile('ui/consortia/view/battle/consortia_battle_intro_panel')
    t.subPanel.Open(t.rootPanel,t)
  end
end

--------------------------update by protocol------------------
function t.onUpdateKickedOutByProtocol()
  common_errors_tip_view.Open(LocalizationController.instance:Get('ui.create_consortia_view.kickedOutTip'))
  t.Close()
  consortia_controller.GuildReq()
  
end
return t
