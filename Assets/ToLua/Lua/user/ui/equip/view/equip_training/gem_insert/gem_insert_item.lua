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
end

function t:CreateItem(itemTransform)
  local item = {}
  item.transform = itemTransform
  item.textValue = itemTransform:Find('bg/text_value'):GetComponent(typeof(Text))
  item.commonItemIcon = common_item_icon.New(itemTransform)
  item.commonItemIcon.onClick:AddListener(self.clickItemFunc)
  return item
end
--ArrayList<itemInfo>
function t:SetInfoList(infoList)
  self.infoList = infoList
  self:Refresh()
end

function t:Refresh()
  local gem_attr_data = gamemanager.GetData('gem_attr_data')
  
  local attrData = gem_attr_data.GetDataById(self.infoList:Get(1).itemData.id)
  self.textTitle.text = attrData.equipAttr:GetName()
  local item = nil
  for k,v in pairs(self.infoList:GetDatas()) do
    --create
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
  --hide
  local itemCount = #self.items
  local infosCount = self.infoList.Count
  if itemCount > infosCount then
    for i = infosCount+1,itemCount do
      self.items[i].transform.gameObject:SetActive(false)
    end
  end
  --size
  if infosCount <= 4 then
    self.transform.sizeDelta = self.originSizeDelta
  else
    local row = math.floor(infosCount/4)
    if infosCount % 4 == 0 then
      row = row - 1
    end
    local h = (self.grid.cellSize.y + self.grid.spacing.y)*row
    self.transform.sizeDelta = Vector2(self.originSizeDelta.x,self.originSizeDelta.y+h)
  end
  
end
function t:HideAllSelect()
  for k,v in pairs( self.items) do
    v.commonItemIcon:SetSelect(false)
  end
end

function t:GetCommonItemIcon(instanceId)
  if instanceId <= 0 then
    return nil
  end
  for k,v in pairs( self.items) do
    if v.commonItemIcon.itemInfo.instanceId == instanceId then
      return v.commonItemIcon
    end
  end
  return nil
end


return t