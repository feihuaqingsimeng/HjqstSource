local t = {}
--local PREFAB_PATH = 'ui/equipments/equip_attr_panel'
t.__index = t

local equip_info = require('ui/equip/model/equip_info')

function t.Open(parent,equipInfo)
  --local gameObject = GameObject.Instantiate(Common.ResMgr.ResMgr.instance:Load(PREFAB_PATH))
  
  --local o = t.OpenByGameObject(gameObject)
 -- o.transform:SetParent(parent,false)
 -- o:SetEquipInfo(equipInfo)
 -- return o
end

function t.OpenByGameObject(gameObject)
  local o = {}
  setmetatable(o,t)
  
  o.transform = gameObject:GetComponent(typeof(Transform))
  o.equipInfo = nil  
  o.inheritEquipInfo = nil
  o:InitComponent()
  return o
end

function t:InitComponent()
  self.textPower = self.transform:Find('text_power'):GetComponent(typeof(Text))
  self.textEquipName = self.transform:Find('text_equip_name'):GetComponent(typeof(Text))

  self.tranEquipIcon = self.transform:Find('img_equip_icon')
  self.tranAttrRoot = self.transform:Find('Scroll View/Viewport/Content')
  self.textStarAddValue = self.transform:Find('text_star_add'):GetComponent(typeof(Text))
   --attr
  self.goAttrPrefab = self.transform:Find('attr_item').gameObject
  self.goAttrPrefab:SetActive(false)

  --tip
  self.tipTable = {}
  local tip_root = self.transform:Find('tip_root')
  local childCount = tip_root.childCount
  for i = 1,childCount do
    self.tipTable[i] = {}
    local tran = tip_root:GetChild(i-1)
    self.tipTable[i].root = tran.gameObject
    self.tipTable[i].goShuXingEffect = tran:Find('ui_shuxing').gameObject
    self.tipTable[i].goShuXingEffect:SetActive(false)
  end
  --effect
  self.goZhuangBeiEffect = self.transform:Find('ui_zhuangbei').gameObject
  self.goZhuangBeiEffect:SetActive(false)
end

function t:SetEquipInfo(equipInfo)
  self.equipInfo = equipInfo

  self:SetInheritEquipInfo(self.inheritEquipInfo)
end
--设置继承信息
function t:SetInheritEquipInfo(inheritEquipInfo,isInheritLevel,isInheritStar)
  if inheritEquipInfo then
    self.inheritEquipInfo = self.equipInfo:Clone()
    if isInheritLevel then
      self.inheritEquipInfo.strengthenLevel = inheritEquipInfo.strengthenLevel
    end
    if isInheritStar then
      self.inheritEquipInfo:SetStarGems(inheritEquipInfo.starGems)
    end
    
  end
  self:Refresh()
end
--清除继承痕迹
function t:ClearInheritEquipInfo()
  self.inheritEquipInfo = nil
  self:Refresh()
end

function t:Refresh()
  
  ui_util.ClearChildren(self.tranEquipIcon,true)
  ui_util.ClearChildren(self.tranAttrRoot,true)
  if self.equipInfo == nil then
    self.textPower.text = '0'
    self.textEquipName.text = ''
  else
    self.textPower.text = self.equipInfo:Power()
    self.textEquipName.text = LocalizationController.instance:Get(self.equipInfo.data.name)
    self.textEquipName.color = ui_util.GetEquipQualityColor(self.equipInfo.data)
    
    local common_equip_icon = require('ui/common_icon/common_equip_icon')
    local icon = common_equip_icon.New(self.tranEquipIcon)
    icon:SetEquipInfo(self.equipInfo)
    --继承相关
    self.textStarAddValue.gameObject:SetActive(self.inheritEquipInfo ~= nil)
    if self.inheritEquipInfo then
      if self.inheritEquipInfo.strengthenLevel >= 0 then
        self.textEquipName.text = string.format(LocalizationController.instance:Get('ui.equipment_training_view.inherit.changeNameValue'),self.textEquipName.text,self.equipInfo.strengthenLevel,self.inheritEquipInfo.strengthenLevel)
      end
      
      self.textPower.text = string.format(LocalizationController.instance:Get('ui.equipment_training_view.inherit.changePowerValue'), self.textPower.text,self.inheritEquipInfo:Power())
      local max = 0
      local min = 0
      if self.inheritEquipInfo.star > self.equipInfo.star then
        max = self.inheritEquipInfo.star
        min = max
      else
        max = self.equipInfo.star
        min = self.inheritEquipInfo.star
      end
      icon:SetStar(min,max)
      
      local starAddValue = 0
      for k,v in pairs(self.inheritEquipInfo.starGems) do
        starAddValue = starAddValue + v
      end
      self.textStarAddValue.text = string.format('(%d%%)',starAddValue)
  end
  end
  self:RefreshAttr()
end
function t:RefreshAttr()
  if self.equipInfo == nil then
    return
  end
  self:RefreshMainAttr()
  self:RefreshRandomAttr()
  self:RefreshGemAttr()
end


function t:CreateAttrItem(parent)
  local go = GameObject.Instantiate(self.goAttrPrefab)
  go:SetActive(true)
  local tran = go.transform
  tran:SetParent(parent,false)
  local item = {}
  item.root = tran
  item.textName = tran:Find('text_name'):GetComponent(typeof(Text))  
  item.textTitle = tran:Find('text_title'):GetComponent(typeof(Text))
  item.textNum = tran:Find('text_num'):GetComponent(typeof(Text))
  item.imgIcon = tran:Find('img_icon'):GetComponent(typeof(Image))
  return item
end

function t:RefreshMainAttr()
  local item = self:CreateAttrItem(self.tranAttrRoot)
  item.textTitle.text = LocalizationController.instance:Get('ui.equipment_training_view.base_attr_title')
  local totalBaseAttr = self.equipInfo:GetTotalBaseAttr()
  item.textName.text = totalBaseAttr:GetName()
  item.textNum.text = totalBaseAttr:GetValueString()
  item.imgIcon.gameObject:SetActive(false)
  
  if self.inheritEquipInfo then
    item.textNum.text = string.format(LocalizationController.instance:Get('ui.equipment_training_view.inherit.changeBaseAttrValue'), totalBaseAttr:GetValueString(),self.inheritEquipInfo:GetTotalBaseAttr():GetValueString())
  end
end
function t:RefreshRandomAttr()
  local colors = ui_util.GetEquipRandomColor(self.equipInfo)
  for k,v in ipairs(self.equipInfo.randomAttrs) do
    local item = self:CreateAttrItem(self.tranAttrRoot)
    item.textTitle.text = LocalizationController.instance:Get('ui.equipment_training_view.random_attr_title')..k
    item.textName.text = v:GetName()
    item.textNum.text = v:GetValueString()
    item.textNum.color = colors[k]
    item.textName.color = colors[k]
    item.imgIcon.gameObject:SetActive(false)
  end
  
end

function t:RefreshGemAttr()
    
  local item_data = gamemanager.GetData('item_data')
  local gem_attr_data = gamemanager.GetData('gem_attr_data')
  
  for k,v in ipairs(self.equipInfo.gemInsertIds) do
    local gemAttrData = gem_attr_data.GetDataById(v)
    
    if gemAttrData  then
      
      local itemData = item_data.GetDataById(gemAttrData.id)
      local item = self:CreateAttrItem(self.tranAttrRoot)
      item.textTitle.text = string.format(LocalizationController.instance:Get('common.role_icon.common_lv'),itemData.lv)
      item.textName.text = gemAttrData.equipAttr:GetName()
      item.textNum.text = gemAttrData.equipAttr:GetValueString()
      item.imgIcon.sprite = ResMgr.instance:LoadSprite(itemData:IconPath())
    end
  end
end
--[[function t:RefreshEnchantAttr()
  if self.equipInfo.enchantingAttr == nil then
    self.enchantTable.textName.text = ''
    self.enchantTable.textTitle.text =  LocalizationController.instance:Get('ui.equipment_training_view.none')
    self.enchantTable.textNum.text = ''
    self.enchantTable.imgIcon.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/icon_fumo_01')
    
  else
    local scrollItemData = gamemanager.GetData('item_data').GetDataById(self.equipInfo.enchantingScrollNo)
    self.enchantTable.textName.text = self.equipInfo.enchantingAttr:GetName()
    self.enchantTable.textTitle.text = string.format('Lv %d', scrollItemData.lv)
    self.enchantTable.textNum.text = self.equipInfo.enchantingAttr:GetValueString()
    self.enchantTable.imgIcon.sprite = ResMgr.instance:LoadSprite(scrollItemData:IconPath())
  end
end]]
function t:HideAllTip()
  for k,v in ipairs(self.tipTable) do
    v.root:SetActive(false)
  end
end
function t:ShowTip(tipIndex,isShow,beforeEquipAttr,afterEquipAttr)
 
  local tip = self.tipTable[tipIndex]
  tip.root:SetActive(isShow)
  if isShow == false then
    return
  end
  if afterEquipAttr == nil or (beforeEquipAttr == nil and afterEquipAttr == nil) then
   
    return
  end
  local equip_attr = require('ui/equip/model/equip_attr')
  if beforeEquipAttr == nil then
    beforeEquipAttr = equip_attr.New(afterEquipAttr.type,0)
  end
  if beforeEquipAttr.type ~= afterEquipAttr.type then
    afterEquipAttr = equip_attr.New(beforeEquipAttr.type,0)
  end
end
function t:ShowMainTip(isShow,newEquipAttr)
  self:ShowTip(1,isShow,self.equipInfo.baseAttr, newEquipAttr)
end
function t:ShowRandomTip(isShow,randomIndex ,newEquipAttr)
  self:ShowTip(randomIndex+1,isShow,self.equipInfo.randomAttrs[randomIndex], newEquipAttr)
end

function t:ShowGemTip(isShow,slotIndex,newEquipAttr)
  local gem_attr_data = require('ui/item/model/gem_attr_data')
  local equip_attr = require('ui/equip/model/equip_attr')
  local attrData = nil
  local gemId = self.equipInfo.gemInsertIds[slotIndex]
  if gemId > 0 then
    attrData = gem_attr_data.GetDataById(gemId)
    attrData = gem_attr_data.equipAttr
  end
  self:ShowTip(slotIndex+3,isShow,attrData, newEquipAttr)
end
function t:ShowEnchantTip(isShow,newEquipAttr)
  self:ShowTip(6,isShow,self.equipInfo.enchantingAttr, newEquipAttr)
end
--属性特效展示
function t:ShowShuXingEffect()
  coroutine.start(function()
      --coroutine.wait(0.02)
      for k,v in pairs(self.tipTable) do
        if v.root.activeSelf then
          v.goShuXingEffect:SetActive(false)
          v.goShuXingEffect:SetActive(true)
        end
      end
      coroutine.wait(0.5)
      if self.transform == nil then
        return
      end
      for k,v in pairs(self.tipTable) do
        v.goShuXingEffect:SetActive(false)
      end
    end)

end
---装备icon特效展示
function t:ShowEquipIconEffect()
  coroutine.start(function()
      coroutine.wait(0.02)
    if self.transform == nil then
      return
    end
    self.goZhuangBeiEffect:SetActive(false)
    self.goZhuangBeiEffect:SetActive(true)
    end)
  

end

function t:Close ()
  self.transform = nil
end

return t