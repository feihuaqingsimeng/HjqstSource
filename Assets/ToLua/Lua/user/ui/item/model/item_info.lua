local t = {}
t.__index = t

t.instanceId = 0
t.itemData = nil
t.count = 0 --数量 (要获得道具数量 建议调用 方法Count(),不要用属性count)
t.usedCount = 0--被使用的数量，如宝石镶嵌到装备上
t.isNew = false --是否是新道具
function t.New(instanceID, itemDataid, count)
  local o = {}
  setmetatable(o,t)
  
  o.instanceId = instanceID
  o.itemData = gamemanager.GetData('item_data').GetDataById(itemDataid)
  o.count = count
  o.usedCount = 0--被使用的数量，如宝石镶嵌到装备上
  return o
end

function t.NewByItemProtoData(itemProtoData)
  local o = {}
  setmetatable(o,t)
  o:Update(itemProtoData)
  return o
end
function t:Update(itemProtoData)
  self.instanceId = itemProtoData.id
  if itemProtoData.modelId ~= 0 then
    self.itemData = gamemanager.GetData('item_data').GetDataById(itemProtoData.modelId)
  end
  self.count = itemProtoData.num
end
function t:SetUseCount(count)
  self.usedCount = count
end
function t:GetUseCount()
  return self.usedCount
end
--实际的数量
function t:Count()
  return self.count-self.usedCount
end
function t:ItemIcon()
  return self.itemData:IconPath()
end

return t