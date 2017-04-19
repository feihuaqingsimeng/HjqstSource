local t = {}
local name = "hero_controller"
local hero_info = require ('ui/hero/model/hero_info')

require 'hero_pb'

local function_open_model = gamemanager.GetModel('function_open_model')

function t.GetEmptyTable ()
  emptyTable = {}
  return emptyTable
end

local function Start ()
  netmanager.RegisterProtocol(MSG.GetAllHeroResp, t.GetAllHeroResp)
  netmanager.RegisterProtocol(MSG.HeroUpdateResp, t.HeroUpdateResp)
  netmanager.RegisterProtocol(MSG.HeroAggrResp, t.HeroAggrResp)
  netmanager.RegisterProtocol(MSG.HeroBreakResp, t.HeroBreakResp)
  netmanager.RegisterProtocol(MSG.HeroAdvanceResp, t.HeroAdvanceResp)
  netmanager.RegisterProtocol(MSG.HeroRelationResp, t.HeroRelationShipResp)
  netmanager.RegisterProtocol(MSG.HeroComposeResp, t.HeroComposeResp)
  netmanager.RegisterProtocol(MSG.HeroPieceComposeResp, t.HeroPieceComposeResp)
  netmanager.RegisterProtocol(MSG.HeroDeComposeResp, t.HeroDeComposeResp)

  netmanager.RegisterProtocol(MSG.HeroLockResp, t.HeroLockResp)
  netmanager.RegisterProtocol(MSG.HeroUnLockResp, t.HeroUnLockResp)
  netmanager.RegisterProtocol(MSG.SendHeroLockId, t.SendHeroLockId)
  gamemanager.RegisterCtrl(name,t)
end

function t.GetAllHeroResp ()
  local getAllHeroResp = hero_pb.GetAllHeroResp()
  getAllHeroResp:ParseFromString(netmanager.GetProtocolData())
  
  for k, v in ipairs(getAllHeroResp.heros) do
    local heroInfo = hero_info:NewByHeroProtoData(v)
    gamemanager.GetModel('hero_model').AddHero(heroInfo, false)
    gamemanager.GetModel('hero_model').UpdateHeroEquipments(false,v.id, v.wearEquips)
  end
  
  gamemanager.GetModel('hero_model').updateLockedHeroes (getAllHeroResp.lockHeros)
  gamemanager.GetModel('hero_model').CheckHasRelationshipByRedPoint()
end

function t.HeroUpdateResp ()
  local heroUpdateResp = hero_pb.HeroUpdateResp()
  heroUpdateResp:ParseFromString(netmanager.GetProtocolData())
  
  local hero_model = gamemanager.GetModel('hero_model')
  -- add heroes
  local hasAdd = false
  for k, v in ipairs(heroUpdateResp.addHeros) do
    local heroInfo = hero_info:NewByHeroProtoData(v)
    hero_model.AddHero(heroInfo, true)
    hero_model.UpdateHeroEquipments(false,v.id, v.wearEquips)
    hasAdd = true
  end
  
  -- remove heroes
  local hasRemove = false
  for k, v in ipairs(heroUpdateResp.delHeros) do
    TalkingDataController.instance:TDGAItemOnUse(hero_model.GetHeroInfo(v).heroData.id,1,BaseResType.Hero)
    hero_model.RemoveHero(v)
    hasRemove = true
    
  end
  
  -- update heroes
  for k, v in ipairs(heroUpdateResp.updateHeros) do
    local updateHeroInfo = gamemanager.GetModel('hero_model').GetHeroInfo(v.id)
    local oldlv = updateHeroInfo.level
    updateHeroInfo:SetHeroInfo(v)
    
    hero_model.UpdateHeroEquipments (false,v.id, v.wearEquips)
    hero_model.OnUpdateHero(updateHeroInfo.instanceID)
    local afterExp = updateHeroInfo.exp
  end
  
  hero_model.OnUpdateHeroInfoList()
  if hasAdd then
    hero_model.OnNewHeroMarksChanged()
  end
  if hasAdd or hasRemove then
    hero_model.CheckHasRelationshipByRedPoint()
  end
  hero_model.CheckHasAdvanceBreakthroughHeroByRedPoint()

end

function t.HeroRelationShipResp ()
  gamemanager.GetModel('hero_model').HeroRelationShipResp()
  gamemanager.GetModel('hero_model').CheckHasRelationshipByRedPoint()
end

function t.HeroRelationShipReq(req)
  local Req = hero_pb.HeroRelationReq()
  Req.id = req.id
  Req.heroId = req.heroId
  Req.activeFlag = req.activeFlag
  for i=1,#req.friends do
    Req.friends:append(req.friends[i])
  end
  netmanager.SendProtocol(MSG.HeroRelationReq, Req)
end

function t.HeroAggrReq (aggredId, consumeIds)
  local heroAggrReq = hero_pb.HeroAggrReq()
  
  heroAggrReq.aggredId = aggredId
  for k, v in pairs(consumeIds) do
    heroAggrReq.consumeIds:append(v)
  end
  netmanager.SendProtocol(MSG.HeroAggrReq, heroAggrReq)
end

function t.HeroAggrResp ()
  local resp = hero_pb.HeroAggrResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  --gamemanager.GetModel('hero_strengthen_model').UpdateStrengthenSuccess(heroAggrResp.isCrit)
  gamemanager.GetModel('hero_model').UpdateHeroStrengthenSuccessByProtocol(resp.isCrit)
end

function t.HeroBreakReq (breakedId, costIndex)
  local heroBreakReq = hero_pb.HeroBreakReq()
  
  t.LastBreakthroughHeroInstanceID = breakedId
  heroBreakReq.breakedId = breakedId
  heroBreakReq.costIndex = costIndex
  netmanager.SendProtocol(MSG.HeroBreakReq, heroBreakReq)
end

function t.HeroBreakResp ()
  local heroBreakResp = hero_pb.HeroBreakResp()
  heroBreakResp:ParseFromString(netmanager.GetProtocolData())
  
  gamemanager.GetModel('hero_model').UpdateHeroBreakthroughSuccessByProtocol(t.LastBreakthroughHeroInstanceID)
end

function t.HeroAdvanceReq (heroId, materialType)
  local heroAdvanceReq = hero_pb.HeroAdvanceReq()
  
  t.LastAdvanceHeroInstanceID = heroId
  heroAdvanceReq.heroId = heroId
  heroAdvanceReq.materialType = materialType
  netmanager.SendProtocol(MSG.HeroAdvanceReq, heroAdvanceReq)
end

function t.HeroAdvanceResp ()
  local heroAdvanceResp = hero_pb.HeroAdvanceResp()
  heroAdvanceResp:ParseFromString(netmanager.GetProtocolData())
  
  local success = false
  if heroAdvanceResp.result == 1 then
    success = true
    gamemanager.GetModel('hero_model').UpdateHeroAdvanceSuccessByProtocol(t.LastAdvanceHeroInstanceID)
  end
end
--英雄合成
function t.HeroComposeReq(composeId,materialGameResDataList)
  local req = hero_pb.HeroComposeReq()
  req.composeNo = composeId
  for k ,v in pairs(materialGameResDataList) do 
    local heroComposeProtoData = req.composeData:add()
    heroComposeProtoData.type = v.type
    heroComposeProtoData.id = v.id
  end
  netmanager.SendProtocol(MSG.HeroComposeReq, req)
end

function t.HeroComposeResp()
  local resp = hero_pb.HeroComposeResp()
  resp:ParseFromString(netmanager.GetProtocolData())
  
  gamemanager.GetModel('hero_model').UpdateHeroComposeSuccessByProtocol(resp.newHero)
end

-- [HERO PIECE COMPOSE] --
function t.HeroPieceComposeReq (heroPieceID)
  local intProto = common_pb.IntProto()
  intProto.value = heroPieceID
  netmanager.SendProtocol(MSG.HeroPieceComposeReq, intProto)
end

function t.HeroPieceComposeResp ()
  local intProto = common_pb.IntProto()
  intProto:ParseFromString(netmanager.GetProtocolData())
  
  local heroPieceData = gamemanager.GetData('piece_data').GetDataById(intProto.value)
  local heroGameResData = heroPieceData:GetHeroGameResData()
  
  local heroInfo = hero_info:NewByGameResData(heroGameResData)
  --gamemanager.GetModel('fight_result_model').OpenFightResultHeroDisplayView(heroInfo.heroData.id, heroInfo.advanceLevel)
  t.ShowNewHero(heroInfo.heroData.id, heroInfo.advanceLevel, nil, false, true)
end
-- [HERO PIECE COMPOSE] --

-- [HERO DECOMPOSE] --
function t.HeroDeComposeReq (heroInstanceID,isUseDiamond)
  local intProto = common_pb.DoubleIntProto()
  intProto.value1 = heroInstanceID
  if isUseDiamond then
    intProto.value2 = 1
  else
    intProto.value2 = 0
  end
  
  netmanager.SendProtocol(MSG.HeroDeComposeReq, intProto)
end

function t.HeroDeComposeResp ()
  local stringProto = common_pb.StringProto()
  stringProto:ParseFromString(netmanager.GetProtocolData())

  local game_res_data = require 'ui/game/model/game_res_data'
  local resList = game_res_data.ParseGameResDataList(stringProto.value)
  
  --local get_hero_piece_tip_view = require 'ui/tips/view/get_hero_piece_tip_view'
  --get_hero_piece_tip_view.Open(heroPieceGameResData)
  local common_reward_tips_view = require('ui/tips/view/common_reward_tips_view')
  common_reward_tips_view.Create(resList)
end
-- [HERO DECOMPOSE] --

-- [HERO LOCK] --
function t.HeroLockReq (heroInstanceID)
  local intProto = common_pb.IntProto()
  intProto.value = heroInstanceID
  netmanager.SendProtocol(MSG.HeroLockReq, intProto)
end

function t.HeroLockResp ()
  gamemanager.GetModel('hero_model').OnLockHeroSuccess()
end

function t.HeroUnLockReq (heroInstanceID)
  local intProto = common_pb.IntProto()
  intProto.value = heroInstanceID
  netmanager.SendProtocol(MSG.HeroUnLockReq, intProto)
end

function t.HeroUnLockResp ()
  gamemanager.GetModel('hero_model').OnUnlockHeroSuccess()
end

function t.SendHeroLockId ()
  local resp = hero_pb.SendHeroLockId()
  resp:ParseFromString(netmanager.GetProtocolData())
  gamemanager.GetModel('hero_model').updateLockedHeroes (resp.lockHeros)
  
  gamemanager.GetModel('hero_model').OnLockedHeroChanged()
end
-- [HERO LOCK] --

------------------open view--------------
function t.OpenHeroStrengthenView(heroInstanceId)
  if function_open_model.IsFunctionOpen(FunctionOpenType.HeroStrengthen,true) then
    local view = dofile('ui/hero/view/strengthen/hero_strengthen_view')
    view.Open(heroInstanceId)
  end
  
end
function t.OpenHeroAdvanceView(heroInstanceId)
  if function_open_model.IsFunctionOpen(FunctionOpenType.HeroAdvance,true) then
    local view = dofile('ui/hero/view/advance/hero_advance_view')
    view.Open(heroInstanceId)
  end
end

function t.OpenHeroBreakthroughView(heroInstanceId)
  local isOpen = false
  local isPlayer = gamemanager.GetModel('game_model').IsPlayer(heroInstanceId)
  if isPlayer then
    isOpen = function_open_model.IsFunctionOpen(FunctionOpenType.PlayerBreakthrough,true)
  else
    isOpen = function_open_model.IsFunctionOpen(FunctionOpenType.HeroBreakthrough,true)
  end
  
  
  if isOpen then
    local view = dofile('ui/hero/view/break_through/break_through_view')
    view.Open(heroInstanceId)
  end
end

function t.OpenHeroInfoView(defaultSelectedRoleInstanceID)
  if function_open_model.IsFunctionOpen(FunctionOpenType.MainView_Hero,true) then
    local view = dofile('ui/hero/view/hero_info/hero_info_view')
    view.Open(defaultSelectedRoleInstanceID)
  end
end

function t.OpenHeroRelationShipView(info)
  local view = dofile('ui/hero/view/relationship/hero_relationship_view')
  view.Open(info)
end
function t.OpenHeroComposeView()
  local view = dofile('ui/hero/view/compose/hero_compose_view')
  view.Open()
end

function t.OpenHeroDecomposeView (heroInfo)
  local heroDecomposeView = dofile('ui/hero/view/decompose/hero_decompose_view')
  heroDecomposeView.Open(heroInfo)
end

function t.OpenChangeProfessionView ()
  if function_open_model.IsFunctionOpen(FunctionOpenType.PlayerChoice,true) then
    local changeProfessionView = dofile('ui/player/view/change_profession_view')
    changeProfessionView.Open()
  end
end

function t.ShowNewHero (modelId, star, onViewCloseHandler, showPreheatEffect, isCompose)
  --[[
  print('hero id:'..modelId..', hero star:'..star)
  if  gamemanager.GetModel('illustration_model').IsHeroCheck(modelId) then
    if isCompose then
      local auto_destroy_tip_view = require 'ui/tips/view/auto_destroy_tip_view'
      local heroData = gamemanager.GetData('hero_data').GetDataById(modelId)
      local ui_util = require 'util/ui_util'
      local heroNameWithColor = ui_util.FormatStringWithinQualityColor (heroData.quality, LocalizationController.instance:Get(heroData.name))
      auto_destroy_tip_view.Open(string.format(LocalizationController.instance:Get('common.tips.hero_piece_compose_success'), heroNameWithColor))
    end
  else
    local view = dofile('ui/fight_result/view/fight_result_hero_display_view')
    view.Open(modelId, star, onViewCloseHandler, showPreheatEffect)
    return true
  end
  return false
  ]]--
  
  local fight_result_hero_display_view = require('ui/fight_result/view/fight_result_hero_display_view')
  local heroData = gamemanager.GetData('hero_data').GetDataById(modelId)
  if isCompose then
    if heroData.quality >= Quality.Purple then
      fight_result_hero_display_view.Open(modelId, star, onViewCloseHandler, showPreheatEffect)
      return true
    else
      local auto_destroy_tip_view = require 'ui/tips/view/auto_destroy_tip_view'
      local heroData = gamemanager.GetData('hero_data').GetDataById(modelId)
      local ui_util = require 'util/ui_util'
      local heroNameWithColor = ui_util.FormatStringWithinQualityColor (heroData.quality, LocalizationController.instance:Get(heroData.name))
      auto_destroy_tip_view.Open(string.format(LocalizationController.instance:Get('common.tips.hero_piece_compose_success'), heroNameWithColor))
      return false
    end
  else
    if heroData.quality >= Quality.Purple then
      fight_result_hero_display_view.Open(modelId, star, onViewCloseHandler, showPreheatEffect)
      return true
    end
  end
  return false
end

Start ()
return t