local t = {}
t.__index = t

local common_hero_icon = require('ui/common_icon/common_hero_icon')
local illustration_model = gamemanager.GetModel('illustration_model')
local illustration_ctrl = gamemanager.GetCtrl('illustration_ctrl')

function t.BindTransform(transform)
  local o = {}
  setmetatable(o,t)
  o.transform = transform
  o:InitComponent()
  o.onClickCallback = nil
  o.starNormalSprite = ResMgr.instance:LoadSprite("sprite/main_ui/icon_star")
  o.starDefaultSprite = ResMgr.instance:LoadSprite("sprite/main_ui/icon_star2_big_disable")
  return o
end

function t:InitComponent()
  self.btnIcon = self.transform:Find('icon'):GetComponent(typeof(Button))
  self.btnIcon.onClick:AddListener(function()
      
      self:ClickRoleButtonHandler()
    end)
  self.roleQualityFrame = self.btnIcon.transform:GetComponent(typeof(Image))
  self.imgHead = self.transform:Find('icon/img_head'):GetComponent(typeof(Image))
  self.imgType = self.transform:Find('icon/img_role_type'):GetComponent(typeof(Image))
  --[[self.imgStars = {}
  for i = 1,6 do
    self.imgStars[i] = self.transform:Find('icon/stars_root/img_star_'..i):GetComponent(typeof(Image))
  end]]
  self.goMask = self.transform:Find('mask').gameObject
end

function t:SetIllustrationInfo(illustrationInfo)
  self.illustrationInfo = illustrationInfo
  self.roleInfo = illustrationInfo.roleInfo
  self:Refresh()
end
--点击按钮相应(参数illustrationInfo)
function t:SetOnClickItemCallback(callback)
  self.onClickCallback = callback
end
---
function t:SetSelect(isSelect)
  
end

function t:Refresh()
  self.roleQualityFrame.sprite = ui_util.GetRoleQualityFrameSprite(self.roleInfo.heroData.quality)
  self:ShowMask(not illustration_model.IsHeroGotInIllustration(self.roleInfo.heroData.id))
  self.imgHead.sprite = ResMgr.instance:LoadSprite(self.roleInfo:HeadIcon())
  --[[for i = 1,6 do
    self.imgStars[i].gameObject:SetActive(i<=self.roleInfo:MaxAdvanceLevel())
    if i<= self.roleInfo.advanceLevel then
      self.imgStars[i].sprite = self.starNormalSprite
    else
      self.imgStars[i].sprite = self.starDefaultSprite
    end
  end]]
  
  self.imgType.sprite = ui_util.GetRoleTypeSmallIconSprite(self.roleInfo.heroData.roleType)
end

function t:ShowMask(value)
  self.goMask:SetActive(value)
end
function t:SetActive(value)
  self.transform.gameObject:SetActive(value)
end
--------------------click event---------------

function t:ClickRoleButtonHandler()
  illustration_ctrl.OpenIllustrationHeroDetailView(self.roleInfo)
  illustration_model.selectRoleInfo = self.roleInfo
  if self.onClickCallback then
    self.onClickCallback(self.illustrationInfo)
  end
end

return t