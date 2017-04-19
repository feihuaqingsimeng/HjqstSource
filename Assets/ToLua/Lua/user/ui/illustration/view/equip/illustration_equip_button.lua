local t = {}
t.__index = t

local illustration_model = gamemanager.GetModel('illustration_model')
local common_equip_icon = require('ui/common_icon/common_equip_icon')

function t.BindTransform(transform)
  local o = {}
  setmetatable(o,t)
  o.onClickCallback = nil
  o.transform = transform
  o:InitComponent()
  o.commonLvStr = LocalizationController.instance:Get('common.role_icon.common_lv')
  return o
end

function t:InitComponent()
  self.btnIcon = self.transform:Find('img_bg'):GetComponent(typeof(Button))
  self.btnIcon.onClick:AddListener(function()
      self:ClickEquipButtonHandler()
    end)
   self.roleQualityFrame = self.btnIcon.transform:GetComponent(typeof(Image))
  self.imgHead = self.transform:Find('img_icon'):GetComponent(typeof(Image))
  self.imgType = self.transform:Find('img_role_type'):GetComponent(typeof(Image))
  self.goSelect = self.transform:Find('img_select').gameObject
  self.goMask = self.transform:Find('mask').gameObject
end

function t:SetIllustrationInfo(illustrationInfo)
  self.illustrationInfo = illustrationInfo
  self:Refresh()
end
--点击按钮相应(参数illustrationInfo)
function t:SetOnClickItemCallback(callback)
  self.onClickCallback = callback
end

function t:SetSelect(isSelect)
  self.goSelect:SetActive(isSelect)
end

function t:Refresh()

  self.roleQualityFrame.sprite = ui_util.GetRoleQualityFrameSprite(self.illustrationInfo.equipInfo.data.quality)
  self:ShowMask(not illustration_model.IsGotInIllustration(IllustrationType.equip,self.illustrationInfo.id))
  self.imgHead.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(self.illustrationInfo.equipInfo:EquipIcon())
  self.imgType.sprite = ui_util.GetRoleTypeSmallIconSprite(self.illustrationInfo.equipInfo.data.equipmentRoleType)
end

function t:ShowMask(value)
  self.goMask:SetActive(value)
end
function t:SetActive(value)
  self.transform.gameObject:SetActive(value)
end
--------------------click event---------------

function t:ClickEquipButtonHandler()
  if self.onClickCallback then
    self.onClickCallback(self.illustrationInfo)
  end
end

return t