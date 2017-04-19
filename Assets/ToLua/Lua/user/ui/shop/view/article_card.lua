local t = {}
t.__index = t

local commonEquipIcon = require('ui/common_icon/common_equip_icon')
local commonItemIcon = require('ui/common_icon/common_item_icon')

function t.NewByGameObject(gameObject, articleInfo)
  local o = {}
  setmetatable(o, t)
  
  o.transform = gameObject.transform
  o:InitComponent()
  o:SetArticleInfo(articleInfo)
  
  return o
end

function t:InitComponent ()
  self.isTurnOverFinished = false
  self.OnShowCompleteDelegate = void_delegate.New()
  
  self.equipIconTransform = self.transform:Find('common_equipment_icon')
  self.equipIcon = commonEquipIcon.NewByGameObject(self.equipIconTransform.gameObject)
  self.itemIconTransform = self.transform:Find('common_item_icon_lua')
  self.itemIcon = commonItemIcon.NewByGameObject(self.itemIconTransform.gameObject)
end

function t:SetArticleInfo (articleInfo)
  self.articleInfo = articleInfo
  if self.articleInfo.data ~= nil then
    self.equipIcon:SetEquipInfo(articleInfo)
  elseif self.articleInfo.itemData ~= nil then
    self.itemIcon:SetItemInfo(articleInfo)
  end
  self.equipIconTransform.gameObject:SetActive(false)
  self.itemIconTransform.gameObject:SetActive(false)
end

function t:TurnOver ()
  local ltDescr = LeanTween.scaleX(self.transform.gameObject, 0, 0.2)
  ltDescr:setOnComplete(Action(function ()
      self:TurnOverComplete()
    end))
end

function t:TurnOverComplete ()
  if self.articleInfo.data ~= nil then
    self.equipIconTransform.gameObject:SetActive(true)
    self.itemIconTransform.gameObject:SetActive(false)
  elseif self.articleInfo.itemData ~= nil then
    self.equipIconTransform.gameObject:SetActive(false)
    self.itemIconTransform.gameObject:SetActive(true)
  end
  local ltDescr = LeanTween.scaleX(self.transform.gameObject, 1, 0.2)
  ltDescr:setOnComplete(Action(function ()
    self:ShowComplete ()
  end))
end

function t:ShowComplete ()
  self.isTurnOverFinished = true
  self.OnShowCompleteDelegate:Invoke()
end

function t:TurnOverAfter (delay)
    LeanTween.delayedCall(delay, Action(function ()
      self:TurnOver ()
      end))
end

return t