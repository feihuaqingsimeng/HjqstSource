local t = {}
t.__index = t

function t.New(transform,id)
  local r={}
  setmetatable(r,t)
  r.id=id
  r.transform=transform
  local des=require('ui/description/common_des_button').New(transform)
  des.onPointerDown:AddListener(function()
    r:OnPointerDown()
  end)
  des.onPointerUp:AddListener(function()
    r:OnPointerUp()
  end)
  des.onPointClick:AddListener(function()
      r:onPointClick()
    end)
  return r
end
function t:SetId(id)
  self.id = id
end
function t:IsLongPress(isLongPress)
  self.isLongPress = isLongPress
end

function t:OnPointerDown()
  if self.isLongPress then
    require('ui/description/common_equip_desview').OpenById(self.id,self.transform.position)
  end
end

function t:OnPointerUp()
  if self.isLongPress then
    require('ui/description/common_equip_desview').Close()
  end
end
function t:onPointClick()
  if not self.isLongPress then
    require('ui/description/common_equip_desview').OpenById(self.id,self.transform.position)
  end
end
return t