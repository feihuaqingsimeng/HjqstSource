local t = {}
local name = 'chapter_controller'
local auto_destroy_tip_view = require('ui/tips/view/auto_destroy_tip_view')

local chapter_model = gamemanager.GetModel('chapter_model')

local function Start ()
  netmanager.RegisterProtocol(MSG.GetStarsAwardResp, t.GetStarsAwardResp)
  netmanager.RegisterProtocol(MSG.StarsAwardInfoResp, t.StarsAwardInfoResp)
  
  gamemanager.RegisterCtrl(name,t)
end

t.dungeonStarBoxID = 0
function t.GetStarsAwardReq (dungeonStarBoxID)
  local intProto = common_pb.IntProto()
  intProto.value = dungeonStarBoxID
  t.dungeonStarBoxID = dungeonStarBoxID
  netmanager.SendProtocol(MSG.GetStarsAwardReq, intProto)
end

function t.GetStarsAwardResp ()
  --auto_destroy_tip_view.Open(Common.Localization.LocalizationController.instance:Get('ui.dungeon_star_reward_view.receive_success'))
  local infoList = chapter_model.GetDungeonStarBoxInfoList(gamemanager.GetModel('dungeon_model').pveSelectDungeonType)
  local boxInfo = infoList[t.dungeonStarBoxID]
  if boxInfo then
    local game_res_data = require('ui/game/model/game_res_data')
    local rewardGameResDataList = game_res_data.ParseGameResDataList(boxInfo.dungeonStarData.award)
    gamemanager.GetModel('tips_model').GetTipView('common_reward_tips_view').Create(rewardGameResDataList)
  end
end

function t.StarsAwardInfoReq ()
end

function t.StarsAwardInfoResp ()
  local resp = pve_pb.StarsAwardInfoResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  chapter_model.UpdateDungeonStarBoxReceiveStatus(resp.starsAwardNo)
end

function t.OpenBossDungeonListView ()
  if not gamemanager.GetModel('function_open_model').IsFunctionOpen(FunctionOpenType.Boss_List_View, true) then
    return
  end
  
  local bossDungeonListView = dofile('ui/chapter/view/boss_dungeon_list_view')
  bossDungeonListView.Open()
end

function t.OpenDungeonStarRewardView (dungeonType)
  local dungeonStarRewardView = dofile('ui/chapter/view/dungeon_star_reward_view')
  dungeonStarRewardView.Open(dungeonType)
end

function t.OpenDungeonStarBoxDetailView (dungeonStarBoxID)
  local dungeonStarBoxDetailView = dofile('ui/chapter/view/dungeon_star_box_detail_view')
  dungeonStarBoxDetailView.Open(dungeonStarBoxID)
end

Start ()
return t