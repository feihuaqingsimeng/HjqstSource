local t = {}
t.__index = t

local game_model = gamemanager.GetModel('game_model')
local consortia_controller = gamemanager.GetCtrl('consortia_controller')
local battle_road_info = require('ui/consortia/model/battle_road_info')

function t.BindGameObject(go)
  local o = {}
  setmetatable(o,t)
  o.transform = go:GetComponent(typeof(Transform))
  o:InitComponent()
  return o
end

function t:InitComponent()
  self.textTitle = self.transform:Find('text_road'):GetComponent(typeof(Text))
  local my = self.transform:Find('my')
  local enemy = self.transform:Find('enemy')
  self.tranMyTeam = {}
  self.tranEnemyTeam = {}
  for i = 1,5 do
    self.tranMyTeam[i] = {}
    local tran = my:Find('pos'..i)
    self.tranMyTeam[i].root = tran
    tran:GetComponent(typeof(Button)).onClick:AddListener(function()
        self:ClickRoadPosHandler(i)
      end)
    self.tranMyTeam[i].imgIcon = tran:Find('icon'):GetComponent(typeof(Image))
    self.tranMyTeam[i].goAdd = tran:Find('add').gameObject
    --enemy
    self.tranEnemyTeam[i] = {}
    tran = enemy:Find('pos'..i)
    self.tranEnemyTeam[i].root = tran
    self.tranEnemyTeam[i].imgIcon = tran:Find('icon'):GetComponent(typeof(Image))
  end
end
function t:SetData(battleRoadInfo)
  self.battleRoadInfo = battleRoadInfo
  self:Refresh()
end

function t:Refresh()
  self.textTitle.text = string.format(LocalizationController.instance:Get('ui.consortia_view.battle.road'),self.battleRoadInfo.id)
  --my
  for k,v in pairs(self.tranMyTeam) do
    local road = self.battleRoadInfo.myRoad[k]
    if road then
      v.imgIcon.sprite = ResMgr.instance:LoadSprite(ui_util.ParseHeadIcon(road.headNo))
    else
      
    end
    v.imgIcon.gameObject:SetActive(road ~= nil)
    v.goAdd:SetActive(road == nil)
  end
  -- enemy
  for k,v in pairs(self.tranEnemyTeam) do
    local enemyRoad = self.battleRoadInfo.enemyRoad[k]
    if enemyRoad then
      v.imgIcon.sprite = ResMgr.instance:LoadSprite(ui_util.ParseHeadIcon(enemyRoad.headNo))
      v.imgIcon.gameObject:SetActive(true)
    else
      v.imgIcon.gameObject:SetActive(false)
    end
    
  end
end
----------------click event-----------------------
function t:ClickRoadPosHandler(index)
  local road = self.battleRoadInfo.myRoad[index]
  if road and road.name == game_model.accountName then
    self.battleRoadInfo.myRoad[index] = nil
    self:Refresh()
  else
    consortia_controller.OpenConsortiaBattleRoadDetailView(self.battleRoadInfo,function(index2)
        self.battleRoadInfo.myRoad[index2] = battle_road_info.CreateRoad(game_model.accountName,game_model.accountHeadNo,game_model.accountLevel)
        self:Refresh()
      end)
  end
  
end

return t