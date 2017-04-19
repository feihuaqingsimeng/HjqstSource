local t = {}
t.__index = t

local common_hero_icon = require('ui/common_icon/common_hero_icon')

function t.BindTransform(transform)
  local o = {}
  setmetatable(o,t)
  
  o.transform = transform
  o:InitComponent()
  o.heroItem = {}
  return o
end

function t:InitComponent()
  self.textName = self.transform:Find('text_relationship_name'):GetComponent(typeof(Text))
  self.attrTable = {}
  for i = 1,2 do
    local root = self.transform:Find('attr'..i)
    self.attrTable[i] = {}
    self.attrTable[i].root = root
    self.attrTable[i].textName = root:Find('text_relationship_attribute_'..i):GetComponent(typeof(Text))
    self.attrTable[i].textValue = root:Find('text_relationship_attribute_value_'..i):GetComponent(typeof(Text))
  end
  self.tranHeroRoot = self.transform:Find('heros_root')
  self.goActive = self.transform:Find('btn_active').gameObject
  self.goActive:SetActive(false)
end

function t:SetRelationShipData(shipData)
  self.shipData = shipData
  self:Refresh()
end

function t:Refresh()
  self.textName.text = LocalizationController.instance:Get(self.shipData.name)
  for k,v in pairs(self.attrTable) do
    local attr = self.shipData.attr[k]
    if attr == nil then
      v.root.gameObject:SetActive(false)
    else
      v.root.gameObject:SetActive(true)
      v.textName.text = attr:GetName()
      v.textValue.text = string.format('+%s',attr:GetValueString())
    end
  end
  for k,v in pairs(self.shipData.friendResData) do
    local item = self.heroItem[k]
    if item == nil then
      item = common_hero_icon.New(self.tranHeroRoot)
      self.heroItem[k] = item
    end
    item.transform.gameObject:SetActive(true)
    item:SetGameResData(v)
    item:AddRoleDesButton(false)
  end
  local count = #self.shipData.friendResData
  local itemCount = #self.heroItem
  --hide
  if count < itemCount then
    for i = count +1 ,itemCount do
      self.heroItem[i].transform.gameObject:SetActive(false)
    end
  end
  
end

return t