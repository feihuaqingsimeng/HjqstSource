local t = {}
t.__index = t

function t.New(talentid, level, exp)
  local o = {}
  setmetatable(o,t)
  
  o:Set(talentid, level, exp)
  o.isCarry = false
  return o

end

function t:Set(talentid, level, exp)
  self.id = talentid
  self.talentData = gamemanager.GetModel('pet_talent_data').GetDataById(talentid)
  self.level = level
  self.exp = exp
 -- print('talent',talentid, level, exp)
end

function t:IsCarry()
  return self.isCarry
end

function t:SetCarry(isCarry)
  self.isCarry = isCarry
end

return t