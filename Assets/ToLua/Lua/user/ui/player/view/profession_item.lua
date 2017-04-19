local t = {}
t.__index = t

local game_model = gamemanager.GetModel('game_model')
local player_model = gamemanager.GetModel('player_model')
local hero_data = gamemanager.GetData('hero_data')

function t.NewByGameObject (gameObject, playerInfo)
  local o = {}
  setmetatable(o, t)
  
  o.transform = gameObject.transform
  o:InitComponent()
  o:SetPlayerInfo(playerInfo)
  
  return o
end

function t:InitComponent ()
  self.selectedMark = self.transform:Find('selected_mark').gameObject
  self.selectedMark:SetActive(false)
  self.professionNameText = self.transform:Find('img_title/text_profession_name'):GetComponent(typeof(Text))
  self.professionPortraitImage = self.transform:Find('img_profession_portrait'):GetComponent(typeof(Image))
  self.activeStatusText = self.transform:Find('img_activate_status_bg/text_activate_status'):GetComponent(typeof(Text))
end

function t:SetPlayerInfo(playerInfo)
  self.playerInfo = playerInfo
  self:Refresh()
end

function t:Refresh ()
  if player_model.GetPlayerInfoByModelId(self.playerInfo.heroData.id) ~= nil then
    self.playerInfo = player_model.GetPlayerInfoByModelId(self.playerInfo.heroData.id)
  end
  
  self.professionNameText.text = Common.Localization.LocalizationController.instance:Get(self.playerInfo.heroData.name)
  self.professionPortraitImage.sprite = ResMgr.instance:LoadSprite(res_path.GetPlayerPortraitPath(self.playerInfo.playerData.portrait))
  
  if self.playerInfo.instanceID == game_model.playerInfo.instanceID then
    self.activeStatusText.text = Common.Localization.LocalizationController.instance:Get('ui.change_profession_view.is_selected')
  elseif player_model.IsPlayerUnlocked(self.playerInfo.playerData.id) then
    self.activeStatusText.text = Common.Localization.LocalizationController.instance:Get('ui.change_profession_view.is_activated')
  else
    self.activeStatusText.text = Common.Localization.LocalizationController.instance:Get('ui.change_profession_view.is_not_activated')
  end
end

function t:SetSelected (selected)
  self.selectedMark:SetActive(selected)
end

return t