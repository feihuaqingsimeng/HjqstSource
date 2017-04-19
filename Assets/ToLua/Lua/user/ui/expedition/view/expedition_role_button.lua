local t = {}
t.__index = t

local common_hero_icon = require('ui/common_icon/common_hero_icon')

function t.BindGameObject(go)
  local o = {}
  setmetatable(o,t)
  o.expeditionRoleInfo = nil
  o.isSelect = false
  o.transform = go.transform
  o.onClick = void_delegate.New()
  
  o:InitComponent()
  
  return o
  
end
function t:InitComponent()
  self.sliderHpBar = self.transform:Find('Slider'):GetComponent(typeof(Slider))
  self.textDie = self.transform:Find('text_die'):GetComponent(typeof(Text))
  self.goSelect = self.transform:Find('img_selected_tag').gameObject
end

function t:SetExpeditionRoleInfo(info)
  local isOldGray = false
  if self.expeditionRoleInfo then
    isOldGray = self.expeditionRoleInfo:IsDead()
  end
  self.expeditionRoleInfo = info
  if self.heroIcon == nil then
    self.heroIcon = common_hero_icon.New(self.transform)
    self.heroIcon.onClick:AddListener(function()
        self.onClick:InvokeOneParam(self)
      end)
    self.heroIcon.transform:SetAsFirstSibling()
  end
  
    self.heroIcon:SetRoleInfo(info.roleInfo,info:IsPlayer())
    self.heroIcon:AddRoleDesButton()
    self.sliderHpBar.value = info.hpRate
    self.textDie.gameObject:SetActive(info:IsDead())
    
    if (isOldGray and not info:IsDead()) or (not isOldGray and info:IsDead()) then
      self:SetChildGray(self.heroIcon.transform,info:IsDead())
    end
    
end
--子节点全部变灰
function t:SetChildGray(transform,isGray)
  local img = transform:GetComponent(typeof(Image))
  if img then
    ui_util.SetImageGray(img,isGray)
  end
  local txt = transform:GetComponent(typeof(Text))
  if txt then
    ui_util.SetTextGray(txt,isGray)
  end
  local count = transform.childCount
  local child = nil
  for i = 0,count-1 do
    child = transform:GetChild(i)
    self:SetChildGray(child,isGray)
  end
end
function t:SetInFormation(isIn)
  self.heroIcon:SetInFormation(isIn)
end
function t:SetSelect(isSelect)
  self.isSelect = isSelect
  self.goSelect:SetActive(isSelect)
end

return t