local enemy_role_info ={}
enemy_role_info.__index =enemy_role_info



function enemy_role_info:NewMineRole(roleInfo)
  local o = {}  
  setmetatable(o,enemy_role_info)
  o:Init()
  o.roleId = roleInfo.roleId
  o:Update(roleInfo) 
  return o
end
function enemy_role_info:Init()
  enemy_role_info.roleId =0
  enemy_role_info.roleName = 0
  enemy_role_info.fightingPower =0
  enemy_role_info.endTime = 0
  enemy_role_info.headNo = 0
  enemy_role_info.roleLv = 0
  enemy_role_info.player =nil
  enemy_role_info.heros =nil
end

function enemy_role_info:Update(roleInfo)  
 self.roleId =roleInfo.roleId
 self.roleName = roleInfo.roleName
 self.fightingPower =roleInfo.fightingPower
 self.endTime = math.floor(roleInfo.endTime/1000)
 self.headNo = roleInfo.headNo
 self.roleLv = roleInfo.roleLv
end
return enemy_role_info