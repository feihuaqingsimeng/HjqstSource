local t = {}
t.__index = t

local illustration_model = gamemanager.GetModel('illustration_model')
local common_item_icon = require('ui/common_icon/common_item_icon')

function t.BindTransform(transform)
  local o = {}
  setmetatable(o,t)
  o.onClickCallback = nil
  o.transform = transform
  o:InitComponent()

  return o
end

function t:InitComponent()
  
  self.goSelect = self.transform:Find('img_select').gameObject
  self.goMask = self.transform:Find('mask').gameObject
  self.btnBg = self.transform:Find('img_bg'):GetComponent(typeof(Button))
  self.btnBg.onClick:AddListener(function()
      self:ClickItemButtonHandler()
    end)
  self.roleQualityFrame = self.btnBg.transform:GetComponent(typeof(Image))
  self.imgHead = self.transform:Find('img_head'):GetComponent(typeof(Image))
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
  self.roleQualityFrame.sprite = ui_util.GetRoleQualityFrameSprite(self.illustrationInfo.itemInfo.itemData.quality)
  self:ShowMask(not illustration_model.IsGotInIllustration(IllustrationType.item,self.illustrationInfo.id))
  self.imgHead.sprite = Common.ResMgr.ResMgr.instance:LoadSprite(self.illustrationInfo.itemInfo:ItemIcon())
end

function t:ShowMask(value)
  self.goMask:SetActive(value)
end
function t:SetActive(value)
  self.transform.gameObject:SetActive(value)
end
--------------------click event---------------

function t:ClickItemButtonHandler()
  if self.onClickCallback then
    self.onClickCallback(self.illustrationInfo)
  end
end

return t