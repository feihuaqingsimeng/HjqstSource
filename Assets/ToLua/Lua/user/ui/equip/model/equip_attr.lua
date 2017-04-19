local attr = {}
attr.__index = attr

function attr.New(equipAttrType,value)
  local o = {}
  setmetatable(o,attr)
  
  o.type = equipAttrType
  o.value = value
  return o
end

function attr.NewByEquipAttr(equipAttr)
  return attr.New(equipAttr.type,equipAttr.value)
end
function attr.NewByEquipAttrDataId(attrDataId)
  local equip_attr_data = require('ui/equip/model/equip_attr_data')
  local attrData = equip_attr_data.GetFirstDataByAttrId(attrDataId)
  return attr.New(attrData.attr_type,attrData.value_min)
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
  if self.type == EquipmentAttributeType.HP or self.type == EquipmentAttributeType.NormalAtk or
		self.type == EquipmentAttributeType.MagicAtk or self.type == EquipmentAttributeType.Def or
		self.type == EquipmentAttributeType.Speed then
      isPercent = false
  end
  return isPercent
end

return attr