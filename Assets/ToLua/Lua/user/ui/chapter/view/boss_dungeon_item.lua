local t = {}
t.__index = t

local lightStarPath = 'sprite/main_ui/icon_star2_big'
local darkStarPath = 'sprite/main_ui/icon_star2_big_disable'

function t.NewByGameObject (gameObject, dungeonInfo)
  local o = {}
  setmetatable(o, t)
  
  o.dungeonInfo = dungeonInfo
  o.transform = gameObject:GetComponent(typeof(Transform))
  o:InitComponent()
  return o
end

function t:InitComponent()
  self.detailButton = self.transform:Find('btn_detail'):GetComponent(typeof(Button))
  self.detailButton.onClick:AddListener(function ()
      self:ClickDetail()
    end)
  
  self.bossIconImage = self.transform:Find('img_boss_icon'):GetComponent(typeof(Image))
  self.bossNameText = self.transform:Find('text_boss_name'):GetComponent(typeof(Text))
  
  self.starImages = {}
  for i = 1, 3 do
    local starImagePath = 'img_stars_root/img_star_'..i
    self.starImages[i] = self.transform:Find(starImagePath):GetComponent(typeof(Image))
  end
  
  self.dungeonNameText = self.transform:Find('text_dungeon_name'):GetComponent(typeof(Text))
  t.costItemIconImage = self.transform:Find('img_cost_item_icon'):GetComponent(typeof(Image))
  self.costTicketCountText = self.transform:Find('text_cost_ticket_count'):GetComponent(typeof(Text))
  
  self.challengeButton = self.transform:Find('btn_challenge'):GetComponent(typeof(Button))
  self.challengeButton.onClick:AddListener(function ()
      self:ClickChallenge()
      end)
  
  self.lockRoot = self.transform:Find('lock_root')
  self.unlock_condition_text = self.transform:Find('lock_root/text_unlock_condition'):GetComponent(typeof(Text))
  
  self:Refresh()
end

function t:SetDungeonInfo (dungeonInfo)
  self.dungeonInfo = dungeonInfo
  self:Refresh()
end

function t:Refresh ()
  local heroInfo = self.dungeonInfo.dungeonData:GetBossHeroInfo()
  if heroInfo == nil then
    return
  end
  
  self.bossIconImage.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(heroInfo:HeadIcon())
  self.bossNameText.text = LocalizationController.instance:Get(heroInfo.heroData.name)
  
  for k, v in ipairs(self.starImages) do
    if k <= self.dungeonInfo.star then
      v.sprite = ResMgr.instance:LoadSprite(lightStarPath)
    else
      v.sprite = ResMgr.instance:LoadSprite(darkStarPath)
    end
    v:SetNativeSize()
  end
  
  self.dungeonNameText.text = LocalizationController.instance:Get(self.dungeonInfo.dungeonData.name)
  local costTicketItemInfo = gamemanager.GetModel('item_model').GetItemInfoWithoutNilByItemId (self.dungeonInfo.dungeonData.item_cost.id)
  t.costItemIconImage.sprite = ResMgr.instance:LoadSprite(costTicketItemInfo.itemData:IconPath())
  LuaInterface.LuaCsTransfer.GetItemDesButton(t.costItemIconImage.gameObject, costTicketItemInfo.itemData.id, true)
  local costTicketCountString = string.format(LocalizationController.instance:Get('ui.boss_dungeon_list_view.cost_action_count'), self.dungeonInfo.dungeonData.item_cost.count)
  if costTicketItemInfo.count >= self.dungeonInfo.dungeonData.item_cost.count then
    costTicketCountString = string.format(LocalizationController.instance:Get('common.green_text_template_lua'), costTicketCountString)
  else
    costTicketCountString = string.format(LocalizationController.instance:Get('common.red_text_template_lua'), costTicketCountString)
  end
  self.costTicketCountText.text = costTicketCountString
  
  self.challengeButton.gameObject:SetActive(not self.dungeonInfo.isLock)
  
  local preDungeonInfo = gamemanager.GetModel('dungeon_model').GetPreDungeonInfo(self.dungeonInfo)
  if preDungeonInfo ~= nil then
    local difficutyStr = LocalizationController.instance:Get('ui.boss_dungeon_list_view.dungeon_difficulty_'..preDungeonInfo.dungeonData.type)
    self.unlock_condition_text.text = string.format(LocalizationController.instance:Get('ui.boss_dungeon_list_view.unlock_condition'), difficutyStr, LocalizationController.instance:Get(preDungeonInfo.dungeonData.dungeon_show))
  end
  self.lockRoot.gameObject:SetActive(self.dungeonInfo.isLock)
end

function t:ClickDetail()
  LuaCsTransfer.OpenDungeonDetailView(self.dungeonInfo.dungeonData.id)
end

function t:ClickChallenge ()
  local costTicketItemInfo = gamemanager.GetModel('item_model').GetItemInfoWithoutNilByItemId (self.dungeonInfo.dungeonData.item_cost.id)
  if costTicketItemInfo.count < self.dungeonInfo.dungeonData.item_cost.count then
    local auto_destroy_tips_view = require('ui/tips/view/auto_destroy_tip_view')
    auto_destroy_tips_view.Open(LocalizationController.instance:Get('ui.boss_dungeon_list_view.cost_item_not_enough_tips'))
    return
  end
  
  --LuaCsTransfer.CLIENT2LOBBY_PVE_FIGHT_REQ(self.dungeonInfo.dungeonData.id)
  LuaCsTransfer.OpenDungeonDetailView(self.dungeonInfo.dungeonData.id)
end

return t