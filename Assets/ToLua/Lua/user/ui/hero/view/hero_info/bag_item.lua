local t = {}
t.__index = t

local common_hero_icon = require('ui/common_icon/common_hero_icon')
local hero_model = gamemanager.GetModel('hero_model')

function t.BindGameObject(gameObject)
  local o = {}
  setmetatable(o,t)
  
  o.roleInfo = nil
  o.heroIcon = nil
  
  o.transform = gameObject:GetComponent(typeof(Transform))
  o.onClick = void_delegate.New()
  o:InitComponent()
  
 
  return o
end
function t:InitComponent()
  self.btnExpandBg = self.transform:GetComponent(typeof(Button))
  self.btnExpandBg.onClick:RemoveAllListeners()
  self.btnExpandBg.onClick:AddListener(function (icon)
      self.onClick:InvokeOneParam(self)
    end)
  
   if self.heroIcon == nil then
    self.heroIcon = common_hero_icon.New(self.transform)
    self.heroIcon.transform:SetAsFirstSibling()
    self.heroIcon.onClick:AddListener(function (icon)
      --self.onClick:InvokeOneParam(self)
      self.btnExpandBg.onClick:Invoke()
    end)
  end
  self.goAdd = self.transform:Find('img_add').gameObject
  self.goNewMark = self.transform:Find('img_new_role_mark').gameObject
  self.redPointView = self.transform:Find('img_red'):GetComponent('RedPointView')
end

function t:ResetListener()
  self.onClick:RemoveAllListener()
end

function t:SetRoleInfo(roleInfo, index)
  self.index = index
  self.roleInfo = roleInfo
  self.goAdd:SetActive(false)
  self.goNewMark:SetActive(false)
  if roleInfo == nil then
    self.heroIcon:SetActive(false)
    self.goNewMark:SetActive(false)
    self.redPointView:SetId(0)
  else
    self.heroIcon:SetActive(true)
    self.heroIcon:SetRoleInfo(roleInfo,false)
    self.goNewMark:SetActive(hero_model.IsNewHero(self.roleInfo.instanceID))
    self.redPointView:SetId(self.roleInfo.instanceID)
  end
  
end

function t:SetAsExpandRolesBagButton(index)
  self.index = index
  self.goAdd:SetActive(true)
  self.goNewMark:SetActive(false)
  if self.heroIcon ~= nil then
    self.heroIcon:SetActive(false)
  end
  self.redPointView:SetId(0)
end
function t:SetSelect(isSelect)
  if self.heroIcon  then
    self.heroIcon:SetSelect(isSelect)
  end
end
function t:SetInFormation(isIn)
  if self.heroIcon  then
    self.heroIcon:SetInFormation(isIn)
  end
end


--------------------click event-------------------


return t