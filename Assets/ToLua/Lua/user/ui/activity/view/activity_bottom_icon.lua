local t={}
local name = 'activity_bottom_icon'
t.__index = t

local activity_model = gamemanager.GetModel('activity_model')
local event_data = gamemanager.GetData('event_data')
local vipModel = gamemanager.GetModel('vip_model')

t.prefab   = 'ui/activity/bottom_icon'
t.Type = nil
t.transform = nil
t.onClick = nil
t.spriteIcon = nil
t.buttonIcon = nil 
t.textIcon = nil
t.RedPoint = nil

function t.New(parent)
    local o = {}
    setmetatable(o,t)
    ui_util.ClearChildren(parent.transform, true)    
    o.transform = o:InstantiateIcon(parent)    
    o.onClick = void_delegate.New()
    return o
end

function t:Init(btype)
  self.Type = btype
  self:InitComponent(btype)
  self.OnTimerTick = function()
      local type = Mathf.Floor(self.Type/10) * 10
      if type == 100 or type == 70 then self:SetRedPoint(self.Type) end
    end
end

function t:InitComponent(btype)
  if nil == self.transform then do return end end
  self.spriteIcon = self.transform:GetComponent(typeof(Image))
  self.buttonIcon = self.transform:GetComponent(typeof(Button))
  self.textName = self.transform:FindChild('name'):GetComponent(typeof(Text))  
  self.RedPoint = self.transform:FindChild('new')
  
  local eventData = event_data.GetDataByType(btype)
  if nil == eventData then do return end end
  
	self.spriteIcon.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/'..eventData.event_icon)
	self.buttonIcon.onClick:RemoveAllListeners()
  self.buttonIcon.onClick:AddListener(function() self:OnClick() end)  
  self.textName.text = LocalizationController.instance:Get(eventData.event_name)
  
	self.transform.gameObject:SetActive(true)
  self:SetRedPoint(btype)
end

function t:OnClick()
  if nil ~= self.onClick then self.onClick:InvokeOneParam(self) end
end

function t:InstantiateIcon(parent)
  local obj = Instantiate(ResMgr.instance:Load(t.prefab))
  if nil == obj then do return end end
  obj.transform.parent = parent.transform
  obj.transform.localPosition = Vector3(10, 60, 0)
  obj.transform.localScale = Vector3.one
	return obj.transform
end

function t:SetRedPoint(btype)
  self.RedPoint.gameObject:SetActive(activity_model:IsActivityCanAwardByType(btype))
end

return t