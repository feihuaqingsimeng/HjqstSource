local t = {}
t.__index = t

--hpRate 0~1
function t.New(instanceID,hpRate)
  local o = {}
  setmetatable(o,t)
  
  o.instanceID = instanceID
  o.hpRate = hpRate
  if o:IsPlayer() then
    o.roleInfo = gamemanager.GetModel('game_model').playerInfo
  else
    o.roleInfo = gamemanager.GetModel('hero_model').GetHeroInfo(instanceID)
  end
  
  return o
end

function t:IsDead()
  return self.hpRate <= 0
end

function t:IsPlayer()
  return gamemanager.GetModel('game_model').IsPlayer(self.instanceID)
end

return t