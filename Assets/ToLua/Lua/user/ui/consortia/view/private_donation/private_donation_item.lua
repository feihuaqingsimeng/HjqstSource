local t = {}
t.__index = t

local common_item_icon = require ('ui/common_icon/common_item_icon')

function t.BindGameObject(go)
  local o = {}
  setmetatable(o,t)
  o.transform = go:GetComponent(typeof(Transform))
  o:InitComponent()
  return o
end

function t:InitComponent()
  t.playerAvatarImage = self.transform:Find('img_player_avatar_frame/img_player_avatar'):GetComponent(typeof(Image))
  t.playerLevelText = self.transform:Find('text_player_level'):GetComponent(typeof(Text))
  t.playerNameText = self.transform:Find('text_player_name'):GetComponent(typeof(Text))
  t.playerConsortiaTitleText = self.transform:Find('text_player_consortia_title'):GetComponent(typeof(Text))
  t.requirement_root = self.transform:Find('requirement_piece_icon_root')
  t.requirementIcon = common_item_icon.New(t.requirement_root)
  t.myOwnCountText = self.transform:Find('img_own_count_frame/text_own_count'):GetComponent(typeof(Text))
  
  t.donateButton = self.transform:Find('btn_donation'):GetComponent(typeof(Button))
  t.inProgressTextGameObject = self.transform:Find('text_in_progress').gameObject
  t.completeImageGameObject = self.transform:Find('img_complete').gameObject
  t.donatedImageGameObject = self.transform:Find('img_donated').gameObject
end

function t:SetData (privateDonationInfo)
  self:Refresh()
end

function t:Refresh()
  t.playerAvatarImage.sprite = nil
  t.playerLevelText.text = 'Player Level'
  t.playerNameText.text = 'Player Name'
  t.playerConsortiaTitleText.text = 'Consortia Title'
  
  t.myOwnCountText.text = '00'
  
  t.donateButton.gameObject:SetActive(true)
  t.inProgressTextGameObject:SetActive(true)
  t.completeImageGameObject:SetActive(true)
  t.donatedImageGameObject:SetActive(true)
end

return t