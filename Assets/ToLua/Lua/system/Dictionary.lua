local t = {}
t.__index = t

--keyType,valueType 给你自己看的，目前没有实际用途
function t.New(keyType,valueType)
    local o = {}
    setmetatable(o,t)
    o.keyType = keyType
    o.valueType = valueType
    o.Count = 0
    o.data = {}
    return o
end
function t:GetDatas()
  return self.data
end
function t:Add(key,value)
 -- if(self.keyType ~= type(key) or self.valueType ~= type(value)) then
   -- Debugger.LogError('[Dictionary],type error :key and value is not Create type')
   -- return
 -- end
  local d = self.data[key]
  self.data[key] = value
  if not d then
    self.Count = self.Count + 1
  else
    print('[Dictionary] has same key:'..key..',so replace the old value')
  end
end
function t:Remove(key)
  local d = self.data[key]
  self.data[key] = nil
  if d then
    self.Count = self.Count - 1
  end
end

function t:Set(key,value)
  
  local d = self.data[key]
  self.data[key] = value
  if not d then
    self.Count = self.Count + 1
  end
end

function t:Get(key)
  return self.data[key]
end

function t:GetKeysList()
  local keys = {}
  local index = 1
  for k,v in pairs(self.data) do
    keys[index] = k
    index = index + 1
  end
  return keys
end
--自增
function t:GetSortKeysList()
  local ks = self:GetKeysList()
  table.sort(ks,function(a,b)
      return a < b
    end)
  return ks
end

function t:GetValuesList()
  local values = {}
  local index = 1
  for k,v in pairs(self.data) do
    values[index] = v
    index = index + 1
  end
  return values
end

function t:Clear()
  self.Count = 0
  self.data = {}
end
function t:ContainsKey(key)
  return self.data[key] ~= nil
end

function t:ContainsValue(value)
  for k,v in pairs(self.data) do
    if v == value then
      return true
    end
  end
  return false
end
return t