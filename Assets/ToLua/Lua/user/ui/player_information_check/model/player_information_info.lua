local t = {}
t.__index = t

local role_check_info = require('ui/player_information_check/model/role_check_info')
local formation_info = require('ui/formation/model/formation_info')

function t:Reset()
  self.roleCheckInfoDictionary = Dictionary.New('id','roleCheckInfo')
  self.formationInfo = nil
end

--[参考 好友查看协议]
function t.NewByRoleDetInfoProto(data)
  local o = {}
  setmetatable(o,t)
  o:Reset()
  local playerCheckInfo = role_check_info.NewByRolePlayerProtoData(data.player,data.hairCutId,  data.hairColorId, data.faceId,data.skinId)
  o.roleCheckInfoDictionary:Add(playerCheckInfo.id,playerCheckInfo)
  for k,v in ipairs(data.heros) do
    local heroCheckInfo = role_check_info.NewByRoleHeroProtoData(v)
    o.roleCheckInfoDictionary:Add(heroCheckInfo.id,heroCheckInfo)
  end
  
  o.formationInfo = formation_info.NewByLineupProtoData(data.lineup)
 o.level = data.lv
 o.name = data.name
 o.headIconPath = ui_util.ParseHeadIcon(data.headNo)
  return o
end

function t:GetRoleCheckInfo(id)
  return self.roleCheckInfoDictionary:Get(id)
end
--阵型站位[pos,id]
function t:GetTeamPosDictionary()
  --
  local dic = Dictionary.New('number','number')
  for k,v in pairs (self.roleCheckInfoDictionary:GetDatas()) do
    dic:Add(v.posIndex,v.id)
  end
  return dic
end
function t:Power()
  local power = 0
  local gameModel = gamemanager.GetModel('game_model')
  local heroModel = gamemanager.GetModel('hero_model')
  for k,v in pairs(self.roleCheckInfoDictionary:GetDatas()) do
    power = power + v:Power()
  end
  if self.formationInfo ~= nil then
    power = power*(self.formationInfo:Power() + 1)
  end
  return math.floor(power)
end
return t