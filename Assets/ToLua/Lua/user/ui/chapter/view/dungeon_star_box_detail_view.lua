local t = {}
local PREFAB_PATH = 'ui/dungeon_star_reward/dungeon_star_box_detail_view'

local dungeon_model = gamemanager.GetModel('dungeon_model')
local chapter_model = gamemanager.GetModel('chapter_model')
local chapter_controller = gamemanager.GetCtrl('chapter_controller')
local game_res_data = require('ui/game/model/game_res_data')
local common_reward_icon = require('ui/common_icon/common_reward_icon')

function t.Open (dungeonStarBoxID)
  t.dungeonStarBoxInfo = chapter_model.GetDungeonStarBoxInfo(dungeonStarBoxID)
  
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  t.InitComponent()
  t.BindDelegate ()
end

function t.BindDelegate ()
  chapter_model.OnDungeonStarBoxInfosUpdateDelegate:AddListener(t.Refresh)
end

function t.UnbindDelegate ()
  chapter_model.OnDungeonStarBoxInfosUpdateDelegate:RemoveListener(t.Refresh)
end

function t.InitComponent ()
  t.titleText = t.transform:Find('core/text_title'):GetComponent(typeof(Text))
  t.receiveButton = t.transform:Find('core/btn_receive'):GetComponent(typeof(Button))
  t.closeButton = t.transform:Find('core/btn_close'):GetComponent(typeof(Button))
  t.closeTipsText = t.transform:Find('core/text_close_tips'):GetComponent(typeof(Text))
  
  t.titleText.text = string.format(LocalizationController.instance:Get('ui.dungeon_star_box_detail_view.title'), t.dungeonStarBoxInfo.dungeonStarData.star_number)
  t.closeButton.onClick:AddListener(t.ClickClose)
  
  t.cantReceiveGameObject = t.transform:Find('core/img_cant_receive').gameObject
  t.alreadyReceivedGameObject = t.transform:Find('core/img_already_received').gameObject
  t.receiveButton = t.transform:Find('core/btn_receive'):GetComponent(typeof(Button))
  t.receiveButton.onClick:AddListener(t.ClickReceive)
  
  t.rewardIconsRoot = t.transform:Find('core/img_frame/reward_icons_root')
  local rewardGameResDataList = game_res_data.ParseGameResDataList(t.dungeonStarBoxInfo.dungeonStarData.award)
  for k, v in pairs(rewardGameResDataList) do
    common_reward_icon.New(t.rewardIconsRoot, v)
  end
  
  t.Refresh()
end

function t.Refresh ()
  --local currentStarCount = LuaInterface.LuaCsTransfer.GetTotalStarCountOfDungeonType(t.dungeonStarBoxInfo.dungeonStarData.dungeon_type)
  local currentStarCount = dungeon_model.GetPlayerGainStarCountOfChapterOfDungeonType(t.dungeonStarBoxInfo.dungeonStarData.dungeon_type, t.dungeonStarBoxInfo.dungeonStarData.chapterID)
  if currentStarCount < t.dungeonStarBoxInfo.dungeonStarData.star_number then
    t.cantReceiveGameObject:SetActive(true)
    t.alreadyReceivedGameObject:SetActive(false)
    t.receiveButton.gameObject:SetActive(false)
  else
    if t.dungeonStarBoxInfo.hasReceived then
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
  t.UnbindDelegate ()
end

-- [[ UI event handlers ]] --
function t.ClickClose ()
  t.Close()
end

function t.ClickReceive ()
  chapter_controller.GetStarsAwardReq(t.dungeonStarBoxInfo.dungeonStarData.id)
  t.Close()
end
-- [[ UI event handlers ]] --

return t