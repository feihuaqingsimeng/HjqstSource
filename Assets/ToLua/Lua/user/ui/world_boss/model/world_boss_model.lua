local t = {}
local name = 'world_boss_model'

t.isOpen = false--是否开放
t.openTime = 0
t.overTime = 0

--status change ,no param
t.onWorldBossStatusChangedDelegate = void_delegate.New()

local function Start ()
  gamemanager.RegisterModel(name, t)
end
--设置时间
function t.OnWorldBossStatusChanged(isOpen,openTime,overTime)
  t.isOpen = isOpen
  t.openTime = openTime
  t.overTime = overTime
  t.onWorldBossStatusChangedDelegate:Invoke()
end

Start ()
return t