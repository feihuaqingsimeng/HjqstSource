local attr = {}
attr.__index = attr

function attr.New(roleAttrType,value)
  local o = {}
  setmetatable(o,attr)
  
  o.type = roleAttrType
  o.value = value
  return o
end

function attr.NewByRoleAttr(roleAttr)
  return attr.New(roleAttr.type,roleAttr.value)
end

function attr:GetName()
  return LocalizationController.instance:Get('attribute_des_'..self.type)
end

function attr:GetValueString()
  if self:IsPercent() then
    local v = math.floor(self.value*1000)
    if v %10 == 0 then
      return string.format('%.0f%%',v/10)
    else
      return string.format('%.1f%%',v/10)
    end
    
  else 
    return tostring(math.floor(self.value))
  end
end

function attr:IsPercent()
  local isPercent = true
  if self.type == RoleAttributeType.HP or self.type == RoleAttributeType.NormalAtk or
		self.type == RoleAttributeType.MagicAtk or self.type == RoleAttributeType.Def or
		self.type == RoleAttributeType.Speed then
      isPercent = false
  end
  return isPercent
end

return attr