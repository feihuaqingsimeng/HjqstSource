local t = {}
local PREFAB_PATH = 'ui/activity/seven_hilarity_view_box_view'

local activity_model = gamemanager.GetModel('activity_model')
local seven_hilarity_data = gamemanager.GetData('seven_hilarity_data')
local activity_ctrl = gamemanager.GetCtrl('activity_controller')
local game_res_data = require('ui/game/model/game_res_data')
local common_reward_icon = require('ui/common_icon/common_reward_icon')
t.box = 0
function t.Open (bday, btype, bbox)
  t.box = bbox  
  t.day = bday 
  t.type = btype 
  
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay)
  t.transform = gameObject:GetComponent(typeof(Transform))  
  t.InitComponent()
  t.BindDelegate ()
end

function t.BindDelegate ()
  activity_ctrl.OnDlegateGetSevenDayCompleteAwardResp:AddListener(t.Refresh)
end

function t.UnbindDelegate ()
  activity_ctrl.OnDlegateGetSevenDayCompleteAwardResp:RemoveListener(t.Refresh)
end

function t.InitComponent ()
  t.titleText = t.transform:Find('core/text_title'):GetComponent(typeof(Text))
  t.receiveButton = t.transform:Find('core/btn_receive'):GetComponent(typeof(Button))
  t.closeButton = t.transform:Find('core/btn_close'):GetComponent(typeof(Button))
  t.closeTipsText = t.transform:Find('core/text_close_tips'):GetComponent(typeof(Text))

  t.titleText.text = string.format(LocalizationController.instance:Get("event_1012_boxReward"), t.box)
  t.closeButton.onClick:AddListener(t.ClickClose)
  
  t.cantReceiveGameObject = t.transform:Find('core/img_cant_receive').gameObject
  t.alreadyReceivedGameObject = t.transform:Find('core/img_already_received').gameObject
  t.receiveButton = t.transform:Find('core/btn_receive'):GetComponent(typeof(Button))
  t.receiveButton.onClick:AddListener(t.ClickReceive)
  
  t.rewardIconsRoot = t.transform:Find('core/img_frame/reward_icons_root')
  
  local itemList = seven_hilarity_data.GetDataByDayAndType(t.day, t.type)
  local item = itemList[1]
  
  local rewardGameResDataList = game_res_data.ParseGameResDataList(item.reward_data)
  for k, v in pairs(rewardGameResDataList) do
    common_reward_icon.New(t.rewardIconsRoot, v)
  end
  
  t.Refresh()
end

function t.Refresh ()  
  local currentStarCount = activity_model.SevenHilarityCompleteNums
  if currentStarCount < t.box then
    t.cantReceiveGameObject:SetActive(true)
    t.alreadyReceivedGameObject:SetActive(false)
    t.receiveButton.gameObject:SetActive(false)
  else
    if activity_model:IsGetBoxReward(t.box) then
      t.cantReceiveGameObject:SetActive(false)
      t.alreadyReceivedGameObject:SetActive(true)
      t.receiveButton.gameObject:SetActive(false)
    else
      t.cantReceiveGameObject:SetActive(false)
      t.alreadyReceivedGameObject:SetActive(false)
      t.receiveButton.gameObject:SetActive(true)
    end
  end
end

function t.Close ()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
end

-- [[ UI event handlers ]] --
function t.ClickClose ()
  t.Close()
end

function t.ClickReceive ()
  activity_ctrl.ReqGetSevenDayCompleteAwardResp(t.box)
end
-- [[ UI event handlers ]] --

return t