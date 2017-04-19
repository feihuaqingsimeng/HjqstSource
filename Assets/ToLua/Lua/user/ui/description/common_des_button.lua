local t = {}
t.__index = t
t.pressTime=0.15

function t.New(transform)
  local o = {}
  setmetatable(o,t)
  local event = transform:GetComponent(typeof(EventTriggerDelegate))
  if not event then
    event = transform.gameObject:AddComponent(typeof(EventTriggerDelegate))
  end

  o.onPointerDown=void_delegate.New()
  o.onPointerUp=void_delegate.New()
  o.onPointClick = void_delegate.New()
  event.onDown:RemoveAllListeners()
  event.onUp:RemoveAllListeners()
  event.onClick:RemoveAllListeners()
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
  return o
end

function t:OnPointerDown()
  self.co=coroutine.start(function()
    coroutine.wait(t.pressTime)
    self.onPointerDown:Invoke()
  end)
end

function t:OnPointerUp()
  coroutine.stop(self.co)
  self.onPointerUp:Invoke()
end
function t:OnPointerClick()
  self.onPointClick:Invoke()
end
return t