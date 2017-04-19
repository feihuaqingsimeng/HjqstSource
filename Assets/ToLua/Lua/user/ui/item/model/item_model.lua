local t = {}
local name = 'item_model'

local global_data = gamemanager.GetData('global_data')
local item_data = gamemanager.GetData('item_data')
local item_info = require('ui/item/model/item_info')

t.itemsTable = {}
--no param
t.updateItemInfoListDelegate = void_delegate.New()
--经验药水更新
t.updateExpPotionDelegate = void_delegate.New()

local function Start ()
  gamemanager.RegisterModel(name, t)
end

function t.GetItemInfoByInstanceID(instanceID)
  return t.itemsTable[instanceID]
end

function t.GetItemInfoByItemID(itemId)
  for k,v in pairs(t.itemsTable) do
    if v.itemData.id == itemId then
      return v
    end
  end
  return nil
end

function t.GetItemInfoListByItemType(itemType)
  local infoList = {}
  for k,v in pairs(t.itemsTable) do
    if v.itemData.type == itemType and v:Count() > 0 then
      infoList[k] = v
    end
  end
  return infoList
end

function t.GetItemCountByItemID(itemId)
  local info = t.GetItemInfoByItemID(itemId)
  if info == nil then
    return 0
  end
  return info:Count()
end

function t.AddItemInfo(itemInfo, isNew)
  t.itemsTable[itemInfo.instanceId] = itemInfo
  t.itemsTable[itemInfo.instanceId].isNew = isNew
 -- print('add item..............',itemInfo.instanceId,itemInfo.itemData.id,itemInfo.count)
end
function t.RemoveItemInfo(instanceId)
  t.itemsTable[instanceId] = nil
end

function t.GetItemInfoWithoutNilByItemId (itemId)
  local itemInfo = t.GetItemInfoByItemID(itemId)
  if itemInfo == nil then
    itemInfo = item_info.New(0, itemId, 0)
  end
  return itemInfo
end

function t.SortItemInfoByQuality (aItemInfo, bItemInfo)
  aValue = aItemInfo.itemData.quality
  bValue = bItemInfo.itemData.quality
  
  if aValue == bValue then
    aValue = aItemInfo.itemData.id
    bValue = bItemInfo.itemData.id
  end
  
  return aValue < bValue
end

function t.GetItemInfoListByIDList (itemIDList)
  local itemInfoList = {}
  for k, v in ipairs(itemIDList) do
    table.insert(itemInfoList, t.GetItemInfoWithoutNilByItemId(v))
  end
  return itemInfoList
end

function t.GetScrollItemInfo (itemId)
  local scrollItemInfo = t.GetItemInfoByItemID(itemId)
  if scrollItemInfo == nil then
    scrollItemInfo = item_info.New(0, itemId, 0)
  end
  return scrollItemInfo
end

function t.GetScrollItemInfoListByLevel (level)
  local scrollItemInfoList = {}
  local itemData = gamemanager.GetData('item_data').data
  for k, v in pairs(itemData) do
    if v.type == ItemType.Enchant and v.lv == level then
      local itemInfo = t.GetItemInfoByItemID(v.id)
      if itemInfo == nil then
        itemInfo = item_info.New(0, v.id, 0)
      end
      table.insert(scrollItemInfoList, itemInfo)
    end
  end
  return scrollItemInfoList
end

function t.HasNewItem ()
  for k, v in pairs(t.itemsTable) do
    if v.isNew == true then
      return true
    end
  end
  return false
end

function t.HasNewItemOfType (itemType)
    for k, v in pairs(t.itemsTable) do
    if v.itemData.type == itemType and v.isNew == true then
      return true
    end
  end
  return false
end

function t.ClearAllNewItemMark ()
  for k, v in pairs(t.itemsTable) do
    v.isNew =false
  end
  gamemanager.GetModel('red_point_model').RefreshSpecific(RedPointType.RedPoint_New_Item)
end

function t.SetItemAsChecked (itemID)
  local itemInfo = t.GetItemInfoByItemID(itemID)
  if itemInfo ~= nil then
    itemInfo.isNew = false
  end
  gamemanager.GetModel('red_point_model').RefreshSpecific(RedPointType.RedPoint_New_Item)
end

--检测宝石合成材料是否够,返回canCombine, ownCount,materialCount
function t.CheckGemCombine(itemid)
  local itemData = item_data.GetDataById(itemid)
  local gem_synthesis_data = gamemanager.GetData('gem_synthesis_data')
  local curGemSynthesisData = gem_synthesis_data.GetDataById(itemid)
  if curGemSynthesisData.NextId ~= 0 then
    --t.newRootTable.textCombineTip.gameObject:SetActive(true)
    local nextGemSynthesisData = gem_synthesis_data.GetDataById(curGemSynthesisData.NextId)
    local count = 0
    
    if itemData.lv < global_data.max_gen_lv and nextGemSynthesisData ~= nil and #nextGemSynthesisData.Material == 1 then
      count = nextGemSynthesisData.Material[1].count
      local own = t.GetItemCountByItemID(nextGemSynthesisData.Material[1].id)
      return own >= count,own,count
    end
    return false,0,0
  end
  return false,0,0
end

---------------------update by protocol -------------------------
function t.updateItemInfoListByProtocol()
  t.updateItemInfoListDelegate:Invoke()
  gamemanager.GetModel('red_point_model').RefreshSpecific(RedPointType.RedPoint_New_Item)
  gamemanager.GetModel('red_point_model').RefreshSpecific(RedPointType.RedPoint_Small_Exp_Potion)
  gamemanager.GetModel('red_point_model').RefreshSpecific(RedPointType.RedPoint_Middle_Exp_Potion)
  gamemanager.GetModel('red_point_model').RefreshSpecific(RedPointType.RedPoint_Big_Exp_Potion)
end
function t.updateExpPotionByProtocol(id)
  t.updateExpPotionDelegate:InvokeOneParam(id)
end

Start()

return t