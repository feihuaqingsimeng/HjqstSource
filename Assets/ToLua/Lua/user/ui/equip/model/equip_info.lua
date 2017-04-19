local equip_info = {}
equip_info.__index = equip_info

local equip_attr = require('ui/equip/model/equip_attr')
local gem_attr_data = gamemanager.GetData('gem_attr_data')

function equip_info.New(instanceId,dataId)
  local o = {}  
  setmetatable(o,equip_info)
  o:Init()
  o.id = instanceId
  o.data = gamemanager.GetData ('equip_data').GetDataById(dataId)
  return o
end
function equip_info:Clone()
  return self:NewBySelf()
end
function equip_info:NewBySelf()
  local o = equip_info.New(self.id,self.data.id)
  
  o.baseAttr = equip_attr.NewByEquipAttr(self.baseAttr)
  for k,v in ipairs(self.baseAttrList) do
    o.baseAttrList[k] = equip_attr.NewByEquipAttr(v)
  end
  for k,v in ipairs(self.randomAttrs) do
    o.randomAttrs[k] =  equip_attr.NewByEquipAttr(v)
  end
  for k,v in ipairs(self.gemInsertIds) do
    o.gemInsertIds[k] = v
  end

  for k,v in ipairs(self.starGems) do
    o.starGems[k] = v
  end

  o.ownerId = self.ownerId
  o.star = self.star
  o.strengthenLevel = self.strengthenLevel
  return o
end

function equip_info:NewByEquip(equip)
  local o = {}  
  setmetatable(o,equip_info)
  o:Init()
  o.id = equip.id
  o:Update(equip)
  
  return o
end
function equip_info:Init()
  self.id = 0
  --基础属性 保存equip_attr
  self.baseAttr = {}
  self.baseAttrList = {}
  
  --随机属性 保存 equip_attr
  self.randomAttrs = {}
  --专属属性
  self.specialAttrList = {}
  --宝石镶嵌属性 保存gem_attr_data. id(即temId)
  self.gemInsertIds = {}
  
  --强化宝石基础属性加成百分比
  self.starGems = {}
  
  self.ownerId = 0
  self.star = 0
  self.strengthenLevel = 0
  
end
function equip_info:Update (equip)
  if equip.modelId ~= 0 then
    self.data = gamemanager.GetData ('equip_data').GetDataById(equip.modelId)
  end
   --基础属性
  local length = #equip.baseAttr
  if length ~= 0 then
    self.baseAttrList = {}
    local index = 0
    for k,v in ipairs(equip.baseAttr) do
      self.baseAttrList[k] = equip_attr.NewByEquipAttr(v)
      --print('基础属性:',self.baseAttrList[k].type,self.baseAttrList[k].value,self.baseAttrList[k]:GetName())
      if index == 0 then
        self.baseAttr = equip_attr.NewByEquipAttr(v)
      end
      index = index + 1
    end
    
  end
  --random
  length = #equip.randomAttr
  if length ~= 0 then
    self.randomAttrs = {}
    for k,v in ipairs(equip.randomAttr) do
      v.value = tonumber(string.format('%.4f', v.value))
      
      self.randomAttrs[k] = equip_attr.NewByEquipAttr(v)
      
     -- print('随机属性 attr:',self.randomAttrs[k].type,self.randomAttrs[k].value,self.randomAttrs[k]:GetName(),self.randomAttrs[k]:GetValueString())
    end
    
  end
  --专属属性
  length = #equip.specialAttr
  if length ~= 0 then
    self.specialAttrList = {}
    for k,v in ipairs(equip.specialAttr) do
      v.value = tonumber(string.format('%.4f', v.value))
      
      self.specialAttrList[k] = equip_attr.NewByEquipAttr(v)
      
      --print('专属属性 attr:',self.specialAttrList[k].type,self.specialAttrList[k].value,self.specialAttrList[k]:GetName())
    end
    
  end
  --强化属性加成
  if equip.aggrLv ~= -1 then
    self.strengthenLevel = equip.aggrLv
  end
  
  --宝石镶嵌
  if table.count(self.gemInsertIds) == 0 then
      for i = 1,2  do
        self.gemInsertIds[i] = -1
      end
  end
  length = #equip.inlayGems
  if length ~= 0 then
    local item_model = gamemanager.GetModel('item_model')
    
    for k,v in ipairs(equip.inlayGems) do
     -- print('宝石镶嵌 原有宝石id:',self.gemInsertIds[k],'镶嵌槽:',k,"替换id:",v)
      if self.gemInsertIds[k] > 0 then
        local itemInfo = item_model.GetItemInfoByItemID(self.gemInsertIds[k])
        if itemInfo ~= nil then
          itemInfo:SetUseCount(itemInfo:GetUseCount()-1)
        end
      end
      
      if v > 0 then
        local itemInfo = item_model.GetItemInfoByItemID(v)
        if itemInfo ~= nil then
          itemInfo:SetUseCount(itemInfo:GetUseCount()+1)
        end
      end
      self.gemInsertIds[k] = v 
    end
  end
  
  --升星强化宝石属性
  
  if equip.isUpStarGems then
    length = #equip.starGems 
    self.star = length
    self.starGems = {}
    local total = 0
    for k,v in ipairs(equip.starGems ) do
      self.starGems[k] = v
     -- print('强化宝石百分比',self.starGems[k] )
      total = total + v
    end
  end
  
end
---获取基础属性（总加成）
function equip_info:GetTotalBaseAttr()
 
  return equip_attr.New( self.baseAttr.type, self.baseAttr.value + self:GetStrengthenAttrValue() + self:GetUpStarAddValue())
end

---获取升星的属性值（只计算升星增加的属性）
function equip_info:GetUpStarAddValue()
  local total = 0
  for k,v in ipairs(self.starGems ) do
    total = total + v
  end
  return math.ceil((self.baseAttr.value + self:GetStrengthenAttrValue()) * total/100)
end

---获取强化的属性值（只计算强化增加的属性）

function equip_info:GetStrengthenAttrValue()
  local strengthenData = gamemanager.GetData('equip_strengthen_data').GetDataByStrengthenTypeAndLevel(self.data.strengthen_type,self.strengthenLevel)
  if strengthenData then
    return self.baseAttr.value * self.strengthenLevel * strengthenData.attr_add_a
  end
  return 0
end

--获得所有属性
function equip_info:GetAllAttribute()
  local attrList = {}
  ui_util.CheckSameAttribute(attrList,self:GetTotalBaseAttr())
  for k,v in pairs(self.randomAttrs) do
    ui_util.CheckSameAttribute(attrList,v)
  end
  for k,v in pairs (self.gemInsertIds) do
    local data = gem_attr_data.GetDataById(v)
    if data ~= nil then
      ui_util.CheckSameAttribute(attrList,data.equipAttr)
    end
  end
  ui_util.CheckSameAttribute(attrList,self.enchantingAttr)
  return attrList
end
--强化最大等级
function equip_info:GetMaxStrengthenLevel()
  return gamemanager.GetModel('game_model').accountLevel
end
--是否强化最大等级
function equip_info:IsMaxStrengthenLevel()
  return self.strengthenLevel >= self:GetMaxStrengthenLevel()
end
--装备者
function equip_info:SetOwnerId (ownerId)
  self.ownerId = ownerId
end

function equip_info:EquipIcon()
  return self.data:IconPath()
end
--升星宝石属性
function equip_info:SetStarGems(starGems)
  self.starGems = {}
  self.star = 0
  for k,v in pairs(starGems) do
    self.starGems[k] = v
    self.star = self.star + 1
  end
  
end

--战力
function equip_info:Power()
  return gamemanager.GetModel('equip_model').CalcEquipPower(self)
end

return equip_info