local t = {}
t.__index = t



function t.New()
  local o = {}
  setmetatable(o,t)
  o.delegate = {}
  return o
  
end

function t:Invoke()
  for k,v in pairs(self.delegate) do
    v()
  end
end
function t:InvokeOneParam(obj)
  for k,v in pairs(self.delegate) do
    v(obj)
  end
end
function t:InvokeTwoParam(obj1,obj2)
  for k,v in pairs(self.delegate) do
    v(obj1,obj2)
  end
end

function t:AddListener(func)
  for k,v in pairs(self.delegate) do
    if k == func then 
      Debugger.LogError('重复添加监听')
      Debugger.LogError(debug.traceback())
    end
  end
  self.delegate[func] = func
end

function t:RemoveListener(key)
  for k,v in pairs(self.delegate) do 
    if k == key then
      self.delegate[key] = nil
      return
    end
  end
end

function t:RemoveAllListener()
  self.delegate = {}
end

return t