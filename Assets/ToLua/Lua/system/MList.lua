local list = {}
list.__index = list

---注意：下标从0开始
--valueType 给你自己看的，目前没有实际用途
function list.New(valueType)
  local o = {}
  setmetatable(o,list)
  o.Count = 0
  o.valueType = valueType
  o.data = {}
  return o
end
function list:Clone()
  local l = list.New(self.valueType)
  l:AddList(self.data)
  return l
end

function list:GetDatas()
  return self.data
end

function list:Add(value)
  self.data[self.Count] = value
  self.Count = self.Count + 1
end
--添加整个列表,必须是MList
function list:AddList(mList)
  local count = self.Count + mList.Count
  for i = 0 ,mList.Count-1 do
    self.data[i+self.Count] = mList:GetValue(i)
  end
  
  self.Count = count
end

function list:Set(index,value)
  if not self:CheckSize(index) then
    return 
  end
  self.data[index] = value
end

function list:Remove(value)
  local index = self:GetIndex(value)
  self:RemoveAtIndex(index)
end

function list:RemoveAtIndex(index)
  if not self:CheckSize(index) then
    return nil
  end
  
  self.data[index] = nil
  
  for i = index + 1 ,self.Count-1 do
    self.data[i-1] =self.data[i]
  end
  self.Count = self.Count - 1 
end
--获取第一个value所在的index,否则返回-1
function list:GetIndex(value)
  for i = 0 , self.Count-1 do
    if self.data[i] == value then
      return i
    end
  end
  return -1
end
function list:Get(index)
  if not self:CheckSize(index) then
    return nil
  end
  return self.data[index] 
end
function list:CheckSize(index)
  if index < 0 or index >= self.Count then
    print(string.format('<color=#ff0000>index out of range in array size:%s,index:%s</color>',self.Count,index))
    assert(false)
    return false
  end
  return true
end
---插值
function list:Insert(index,value)
  if index < 0 then
    index = 0
  elseif index >= self.Count then
    index = self.Count
  end
  
  self.Count = self.Count + 1
  for i = self.Count,index,-1 do
    self.data[i] = self.data[i-1]
  end
  self.data[index] = value
end
--清除数据
function list:Clear()
  self.Count = 0
  self.data = {}
end
--排序
function list:Sort(func)
  table.sort(self.data,func)
end


return list