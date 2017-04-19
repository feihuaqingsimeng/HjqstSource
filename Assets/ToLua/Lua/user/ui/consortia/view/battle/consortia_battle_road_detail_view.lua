local t = {}
local PREFAB_PATH = 'ui/consortia/battle/consortia_battle_road_detail_view'

local game_model = gamemanager.GetModel('game_model')
local consortia_controller = gamemanager.GetCtrl('consortia_controller')
local battle_road_info = require('ui/consortia/model/battle_road_info')

----------------------------detail item-------------------------
local roadItem = {}
roadItem.__index = roadItem
function roadItem.BindGameObject(go)
  local o = {}
  setmetatable(o,roadItem)
  o.transform = go.transform
  o.textRoadNum = o.transform:Find('text_pos'):GetComponent(typeof(Text))
  o.imgIcon = o.transform:Find('icon_bg/icon'):GetComponent(typeof(Image))
  o.textLevel = o.imgIcon.transform:Find('text_lv'):GetComponent(typeof(Text))
  o.textName = o.transform:Find('text_name'):GetComponent(typeof(Text))

  o.btnFormationUp = o.transform:Find('btn_up'):GetComponent(typeof(Button))
  o.btnFormationUp.onClick:AddListener(function()
      
      o:ClickFormationUpHandler()
    end)
  return o
end
function roadItem:SetData(index,road)
  self.road = road
  self.index = index
  self.textRoadNum.text = self.index
  if road then
    self.textLevel.text = string.format(LocalizationController.instance:Get('common.role_icon.common_lv'),self.road.lv)
    self.imgIcon.sprite = ResMgr.instance:LoadSprite(ui_util.ParseHeadIcon(self.road.headNo))
    self.textName.text = self.road.name
  end
  self.imgIcon.gameObject:SetActive(road ~= nil)
  self.btnFormationUp.gameObject:SetActive(road == nil)
  self.textName.gameObject:SetActive(road ~= nil)
end
function roadItem:ClickFormationUpHandler()
  local index = self.index
  print(index)
  if t.callback then
    t.callback(index)
  end
  t.Close()
end
----------------------------END-------------------------
--callback 点击上阵回调 。参数：点击index
function t.Open(battleRoadInfo,callback)
  local gameObject = UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.Tips,UIOpenMode.Overlay)
  t.transform = gameObject:GetComponent(typeof(Transform))
  t.battleRoadInfo = battleRoadInfo
  t.roadItems = {}
  t.callback = callback
  
  t.InitComponent()
  
  t.textTitle.text = string.format(LocalizationController.instance:Get('ui.consortia_view.battle.road.title'),t.battleRoadInfo.id)
  t.InitRoad()
end
function t.Close()
  UIMgr.instance:Close(PREFAB_PATH)
end
function t.InitComponent()
  t.textTitle = t.transform:Find('core/root/title/text_title'):GetComponent(typeof(Text))
  t.btnClose = t.transform:Find('core/root/title/btn_close'):GetComponent(typeof(Button))
  t.btnClose.onClick:AddListener(t.Close)
  t.tranRoadRoot = t.transform:Find('core/root/road_root')
  t.goRoadPrefab = t.transform:Find('core/root/road_prefab').gameObject
  t.goRoadPrefab:SetActive(false)
end

function t.InitRoad()
  for i = 1 ,5 do
    local go = GameObject.Instantiate(t.goRoadPrefab)
    go:SetActive(true)
    go.transform:SetParent(t.tranRoadRoot,false)
    t.roadItems[i] = roadItem.BindGameObject(go)
    t.roadItems[i]:SetData(i,t.battleRoadInfo.myRoad[i])
  end
end
function t.RefreshRoad()
  for k,v in pairs(t.roadItems) do
    v:SetData(t.battleRoadInfo.myRoad[k],k)
  end
end

-------------------click event---------------------


return t