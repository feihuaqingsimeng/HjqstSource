local t = {}
t.__index = t

local common_item_icon = require('ui/common_icon/common_item_icon')

function t.New(gameObject,clickItemFunc)
  local o = {}
  setmetatable(o,t)
  
  o.transform = gameObject:GetComponent(typeof(Transform))
  o.clickItemFunc = clickItemFunc
  o:InitComponent()
  return o
end

function t:InitComponent()
  --[=====[
  self.textTitle = self.transform:Find('bg/text_title'):GetComponent(typeof(Text))
  self.tranGrid = self.transform:Find('grid')
  self.grid = self.tranGrid:GetComponent(typeof(GridLayoutGroup))
  self.originSizeDelta = self.transform.sizeDelta
  self.goItemPrefab = self.transform:Find('item').gameObject
  self.goItemPrefab:SetActive(false)
  self.items = {}
  local count = self.tranGrid.childCount
  for i = 1,count do
    self.items[i] = self:CreateItem(self.tranGrid:GetChild(i-1))
  end
  ]=====]
  
  self.levelNameText = self.transform:Find('text_scroll_level_name'):GetComponent(typeof(Text))
  self.gridTransform = self.transform:Find('grid')
  self.grid = self.gridTransform:GetComponent(typeof(GridLayoutGroup))
  self.originSizeDelta = self.transform.sizeDelta
  
  self.scrollItemPrefab = self.transform:Find('scroll_item').gameObject
  self.scrollItemPrefab:SetActive(false)
  self.scrollItems = {}
  local count = self.gridTransform.childCount
  for i = 1, count do
    self.scrollItems[i] = self:CreateItem(self.gridTransform:GetChild(i - 1))
  end
end

function t:CreateItem(itemTransform)
  --[=====[
  local item = {}
  item.transform = itemTransform
  item.textValue = itemTransform:Find('bg/text_value'):GetComponent(typeof(Text))
  item.commonItemIcon = common_item_icon.New(itemTransform)
  item.commonItemIcon.onClick:AddListener(self.clickItemFunc)
  return item
  ]=====]
  
  local scrollItem = {}
  scrollItem.transform = itemTransform
  scrollItem.attrNameText = itemTransform:Find('text_attr_name'):GetComponent(typeof(Text))
  scrollItem.attrValueText = itemTransform:Find('text_attr_value'):GetComponent(typeof(Text))
  
  itemIconTransform = itemTransform:Find('common_item_icon')
  scrollItem.commonItemIcon = common_item_icon.New(itemIconTransform)
  scrollItem.commonItemIcon.onClick:AddListener(self.clickItemFunc)
  return scrollItem
end

--[=====[
function t:SetInfoList(infoList)
  self.infoList = infoList
  self:Refresh()
end
]=====]

function t:SetScrollLevel(scrollLevel)
  self.scrollLevel = scrollLevel
  self.levelNameText.text = string.format(LocalizationController.instance:Get("ui.equipment_training_view.equipment_enchanting_panel.scroll_of_level"), scrollLevel)
  self:Refresh()
end

function t:Refresh()
  --[=====[
  local gem_attr_data = gamemanager.GetData('gem_attr_data')
  
  local attrData = gem_attr_data.GetDataById(self.infoList[1].itemData.id)
  self.textTitle.text = attrData.equipAttr:GetName()
  local item = nil
  for k,v in pairs(self.infoList) do
    if self.items[k] == nil then
      local itemClone = GameObject.Instantiate(self.goItemPrefab).transform
      itemClone:SetParent(self.tranGrid,false)
      itemClone.gameObject:SetActive(true)
      self.items[k] = self:CreateItem(itemClone)
    end
    item = self.items[k]
    item.transform.gameObject:SetActive(true)
    attrData = gem_attr_data.GetDataById(v.itemData.id)
    item.textValue.text = string.format('+%s',attrData.equipAttr:GetValueString())
    item.commonItemIcon:SetItemInfo(v)
  end
  local itemCount = #self.items
  local infosCount = #self.infoList
  if itemCount > infosCount then
    for i = infosCount+1,itemCount do
      self.items[i].transform.gameObject:SetActive(false)
    end
  end
  if infosCount <= 4 then
    self.transform.sizeDelta = self.originSizeDelta
  else
    local row = math.floor(infosCount/4)
    local h = (self.grid.cellSize.y + self.grid.spacing.y)*row
    self.transform.sizeDelta = Vector2(self.originSizeDelta.x,self.originSizeDelta.y+h)
  end
  ]=====]
  
  local scrollItemInfos = gamemanager.GetModel('item_model').GetScrollItemInfoListByLevel (self.scrollLevel)
  local gem_attr_data = gamemanager.GetData('gem_attr_data')
  local attrData = nil
  local scrollItem = nil
  
  for k, v in pairs(scrollItemInfos) do
    if self.scrollItems[k] == nil then
      local newScrollItem = GameObject.Instantiate(self.scrollItemPrefab).transform
      newScrollItem:SetParent(self.gridTransform, false)
      newScrollItem.gameObject:SetActive(true)
      self.scrollItems[k] = self:CreateItem(newScrollItem)
    end
    scrollItem = self.scrollItems[k]
    scrollItem.transform.gameObject:SetActive(true)
    attrData = gem_attr_data.GetDataById(v.itemData.id)
    scrollItem.attrNameText.text = attrData.equipAttr:GetName()
    scrollItem.attrValueText.text = string.format('+%s',attrData.equipAttr:GetValueString())
    scrollItem.commonItemIcon:SetItemInfo(v)
    scrollItem.commonItemIcon:ShowCount(true)
  end
  
  local itemsCount = #self.scrollItems
  local infosCount = #scrollItemInfos
  if itemsCount > infosCount then
    for i = infosCount + 1, itemsCount do
      self.items[i].transform.gameObject:SetActive(false)
    end
  end
  if infosCount <= 4 then
    self.transform.sizeDelta = self.originSizeDelta
  else
    local row = math.floor(infosCount / 4)
    if infosCount % 4 == 0 then
      row = row - 1
    end
    local h = (self.grid.cellSize.y + self.grid.spacing.y)*row
    self.transform.sizeDelta = Vector2(self.originSizeDelta.x,self.originSizeDelta.y+h)
  end
end


return t