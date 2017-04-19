local t = {}
t.__index = t

local player_model = gamemanager.GetModel('player_model')
local game_model = gamemanager.GetModel('game_model')
local consortia_model = gamemanager.GetModel('consortia_model')
local consortia_controller = gamemanager.GetCtrl('consortia_controller')
local friend_controller = gamemanager.GetCtrl('friend_controller')

function t.BindGameObject(gameObject)
  local o = {}
  setmetatable(o,t)
  
  t.transform = gameObject:GetComponent(typeof(Transform))
  o:InitComponent()
  return o
end
function t:InitComponent()
  self.imgIcon = self.transform:Find('icon/img_icon'):GetComponent(typeof(Image))
  self.textLevelInIcon = self.transform:Find('icon/text_lv'):GetComponent(typeof(Text))
  self.textLevel = self.transform:Find('text_lv'):GetComponent(typeof(Text))
  self.textName = self.transform:Find('text_name'):GetComponent(typeof(Text))
  self.textContribute = self.transform:Find('text_contribute'):GetComponent(typeof(Text))
  self.tranJob = self.transform:Find('job')
  self.goPresident = self.tranJob:Find('img_president').gameObject
  self.goMember = self.tranJob:Find('text_member').gameObject
  self.textOnline = self.transform:Find('img_status/text_online'):GetComponent(typeof(Text))
  local btn_root = self.transform:Find('btn_root')
  self.btnKick = btn_root:Find('btn_kick'):GetComponent(typeof(Button))
  self.btnKick.onClick:AddListener(function()
      self:ClickKickHandler()
    end)
  self.btnApplyFriend = btn_root:Find('btn_apply_friend'):GetComponent(typeof(Button))
  self.btnApplyFriend.onClick:AddListener(function()
      self:ClickApplyFriendHandler()
    end)
  self.btnRefuse = btn_root:Find('btn_refuse'):GetComponent(typeof(Button))
  self.btnRefuse.onClick:AddListener(function()
      self:ClickRefuseHandler()
    end)
  self.btnPass = btn_root:Find('btn_pass'):GetComponent(typeof(Button))
  self.btnPass.onClick:AddListener(function()
      self:ClickPassHandler()
    end)
  self.onlineOriginPos = self.textOnline.transform.parent.localPosition
end
--status 1成员 2申请
function t:SetMemberInfo(memberInfo,status)
  self.memberInfo = memberInfo
  self.status = status
  self:Refresh()
end
function t:Refresh()
  self.imgIcon.sprite = ResMgr.instance:LoadSprite(ui_util.ParseHeadIcon(self.memberInfo.headNo))
  self.textLevelInIcon.text = string.format('Lv%d',self.memberInfo.lv)
  self.textLevel.text = string.format('Lv%d',self.memberInfo.lv)
  self.textName.text = self.memberInfo.name
  self.textContribute.text = self.memberInfo.contribute
  if self.memberInfo:IsPresident() then
    self.goPresident:SetActive(true)
    self.goMember:SetActive(false)
  else
    self.goPresident:SetActive(false)
    self.goMember:SetActive(true)
  end
  
  self.textOnline.text = TimeUtil.FormatLastLoginTimeString(self.memberInfo.lastLoginTime)
  if self.status == 1 then--成员
    self.textOnline.transform.parent.localPosition = self.onlineOriginPos
    self.tranJob.gameObject:SetActive(true)
    self.textContribute.gameObject:SetActive(true)
    self.btnKick.gameObject:SetActive(consortia_model.IsConsortiaCreator() and not self.memberInfo:IsPresident())
    self.btnApplyFriend.gameObject:SetActive(not (game_model.accountName == self.memberInfo.name) and not LuaCsTransfer.IsFriend(self.memberInfo.id))
    self.btnRefuse.gameObject:SetActive(false)
    self.btnPass.gameObject:SetActive(false)
  else--申请
    self.textOnline.transform.parent.localPosition = self.tranJob.localPosition
    self.tranJob.gameObject:SetActive(false)
    self.textContribute.gameObject:SetActive(false)
    self.btnKick.gameObject:SetActive(false)
    self.btnApplyFriend.gameObject:SetActive(false)
    self.btnRefuse.gameObject:SetActive(true)
    self.btnPass.gameObject:SetActive(true)
  end
end








---------------------click event------------------------------
--踢出公会
function t:ClickKickHandler()
  consortia_controller.GuildKickReq(self.memberInfo.id)
end
--申请好友
function t:ClickApplyFriendHandler()
  friend_controller.FriendAddReq(self.memberInfo.name)
end
--拒绝
function t:ClickRefuseHandler()
  consortia_controller.GuildAnswerReq(self.memberInfo.id,0)
end
--通过
function t:ClickPassHandler()
  consortia_controller.GuildAnswerReq(self.memberInfo.id,1)
end
return t