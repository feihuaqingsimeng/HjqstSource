local t = {}
t.__index = t

local skill_data = gamemanager.GetData('skill_data')

function t.BindTransform(transform)
  local o = {}
  setmetatable(o,t)
  local event = transform:GetComponent(typeof(EventTriggerDelegate))
  if not event then
    event = transform.gameObject:AddComponent(typeof(EventTriggerDelegate))
  end
  event.onDown:AddListener(function(go)
      o:OnPointerDown()
    end)
  event.onUp:AddListener(function(go)
      o:OnPointerUp()
    end)
  event.onClick:AddListener(function(go)
      o:OnPointerClick()
    end)
  o.transform = transform
  o.skill_des_view = nil
  o.isLongPress = false
  return o
end

function t:SetSkillId(skillId,starLevel,starMinLevel,delay)
  self.skillData = skill_data.GetDataById(skillId)
  self.starLevel = starLevel
  self.starMinLevel = starMinLevel
  if delay == nil then
    self.delay = 0
  else
    self.delay = delay
  end
end

function t:IsLongPress(isLongPress)
  self.isLongPress = isLongPress
end

function t:OnPointerDown()
  if not self.skillData then
    return
  end
  if not self.isLongPress then
    return 
  end
  if  self.delay > 0 then
    LeanTween.cancel(self.transform.gameObject)
    LeanTween.delayedCall(self.transform.gameObject,self.delay,Action(function() self:OpenSkillDesView() end))
  else
    self:OpenSkillDesView()
  end
end

function t:OpenSkillDesView()
  self.skill_des_view = require('ui/description/skill_des_view')
  self.skill_des_view.Open(self.skillData.id,self.starLevel,self.starMinLevel,self.transform.position,self.transform.sizeDelta)
end

function t:OnPointerUp()
  if not self.isLongPress then
    return 
  end
  LeanTween.cancel(self.transform.gameObject)
  if self.skill_des_view then
    self.skill_des_view.Close()
    self.skill_des_view = nil
  end
end
function t:OnPointerClick()
  if self.isLongPress then
    return 
  end
  if not self.skillData then
    return
  end
  self:OpenSkillDesView()
end
return t