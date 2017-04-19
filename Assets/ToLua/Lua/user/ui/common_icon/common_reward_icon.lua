local t = {}
t.__index = t

local function NewHeroIcon(o,parent,gameResData)
  local heroIcon = require('ui/common_icon/common_hero_icon')
    o.commonHeroIcon = heroIcon.New(parent)
    o.commonHeroIcon:SetGameResData(gameResData,false)
    o.commonHeroIcon.onClick:AddListener(function(obj)
        o.onClick:InvokeOneParam(o)
        end)
    o.transform=o.commonHeroIcon.transform
end
local function NewEquipIcon(o,parent,gameResData)
  local equipIcon = require('ui/common_icon/common_equip_icon')
    o.equipIcon = equipIcon.New(parent)
    o.equipIcon:SetGameResData(gameResData)
    o.equipIcon.onClick:AddListener(function(obj)
        o.onClick:InvokeOneParam(o)
        end)
    o.transform=o.equipIcon.transform
end
local function NewItemIcon(o,parent,gameResData)
  local itemIcon = require('ui/common_icon/common_item_icon')
    o.itemIcon = itemIcon.New(parent)
    o.itemIcon:SetGameResData(gameResData)
    o.itemIcon.onClick:AddListener(function(obj)
        o.onClick:InvokeOneParam(o)
        end)
    o.transform=o.itemIcon.transform
end

function t.New(parent,gameResData)
  local o = {}
  setmetatable(o,t)
  o.gameResData = gameResData
  --callback contains one obj
  o.onClick = void_delegate.New()
  local transform = nil
  if gameResData.type == BaseResType.Hero then
   NewHeroIcon(o,parent,gameResData)
  elseif gameResData.type == BaseResType.Equipment then
    NewEquipIcon(o,parent,gameResData)
  else
    NewItemIcon(o,parent,gameResData)
  end
  o:AddDesButton()
  return o
end
function t:SetGameResData(gameResData)
  local needReLoad = false
  if self.commonHeroIcon then
    if gameResData.type == BaseResType.Hero then
      self.commonHeroIcon:SetGameResData(gameResData,false)
      self.gameResData = gameResData
    else
      needReLoad = true
    end
    
  elseif self.equipIcon then
    if gameResData.type == BaseResType.Equipment then
      self.equipIcon:SetGameResData(gameResData)
      self.gameResData = gameResData
    else
      needReLoad = true
    end
    
  elseif self.itemIcon then
    if gameResData.type == BaseResType.Item then
      self.itemIcon:SetGameResData(gameResData)
      self.gameResData = gameResData
    else
      needReLoad = true
    end
  end
  if needReLoad then
    self.commonHeroIcon = nil
    self.equipIcon = nil
    self.itemIcon = nil
    local parent = self.transform.parent
    GameObject.Destroy(self.transform.gameObject)
    if gameResData.type == BaseResType.Hero then
     NewHeroIcon(self,parent,gameResData)
    elseif gameResData.type == BaseResType.Equipment then
      NewEquipIcon(self,parent,gameResData)
    else
      NewItemIcon(self,parent,gameResData)
    end
  end
  self:AddDesButton()
end
function t:SetSelect(isSelect)
  if self.commonHeroIcon then
    self.commonHeroIcon:SetSelect(isSelect)
  elseif self.equipIcon then
    self.equipIcon:SetSelect(isSelect)
  else
    self.itemIcon:SetSelect(isSelect)
  end
end
function t:SetActive(isActive)
  if self.commonHeroIcon then
    self.commonHeroIcon:SetActive(isActive)
  elseif self.equipIcon then
    self.equipIcon:SetActive(isActive)
  else
    self.itemIcon:SetActive(isActive)
  end
end

function t:HideCount()
  if self.itemIcon ~= nil then
    self.itemIcon:ShowCount(false)
  end
end

function t:AddDesButton()
  if self.commonHeroIcon then
    self.commonHeroIcon:AddRoleDesButton()
  elseif self.equipIcon then
    self.equipIcon:AddEquipDesButton()
  end
end

-- function t:AddTipsClick()
--   if self.commonHeroIcon then
--     self.commonHeroIcon:AddTipsClick()
--   elseif self.equipIcon then
--     self.equipIcon:AddTipsClick()
--   elseif self.itemIcon then
--     self.itemIcon:AddTipsClick()
--   end
-- end

return t