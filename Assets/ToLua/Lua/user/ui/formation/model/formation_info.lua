local t = {}
t.__index = t
 
local game_res_data = require('ui/game/model/game_res_data')

 
function t.New(formationDataId,level,formationState)
  local o = {}
  setmetatable(o,t)
  
  o.id = formationDataId
  o.isActiveAdditionAttr = false
  o.formationData = gamemanager.GetData('formation_data').GetDataById(formationDataId)
  if o.formationData == nil then
    Debugger.LogError('-----------------error  formationData is nil id:'..formationDataId)
  end
  o.level = level
  o.formationState = formationState
  return o
  
end
function t.NewByLineupProtoData(lineupProtoData)
  local o = {}
  setmetatable(o,t)
  o.id = lineupProtoData.no
  o.level = lineupProtoData.lv
  o.formationState = FormationState.NotInUse
  o.isActiveAdditionAttr = lineupProtoData.attrIsActive
  o.formationData = gamemanager.GetData('formation_data').GetDataById(lineupProtoData.no)
  return o
end
function t:Update(lineupProtoData)
  self.level = lineupProtoData.lv
  self.formationState = FormationState.NotInUse
end

function t:IsMax()
  local max = self.formationData.max_lv
  if self.formationData.max_lv == -1 then
    max = gamemanager.GetModel('game_model').accountLevel
  end
  return self.level == max 
end
function t:isFollowAccountLevel()
  return self.formationData.max_lv == -1
end
--战力
function t:Power()
    local power = 0
    local attrList = gamemanager.GetData('formation_attr_data').GetFormationDatas(self.formationData.id,self.level)
    for k,v in ipairs(attrList) do
      if v.type == 1 then
        power = power + v.comat_a + (self.level-1) * v.comat_b
      else
        if self.isActiveAdditionAttr then
          power = power + v.comat_a
        end
      end
    end
    return power
end
--是否有附加属性
function t:HasAdditionAttr()
  local attrList = gamemanager.GetData('formation_attr_data').GetFormationDatasByFormationId(self.formationData.id)
  for k,v in pairs(attrList) do
    if v.type == 2 then
      return true
    end
  end
  return false
end
--升级消耗 gameResData
function t:UpgradeResCost()
  local a = self.formationData.upgrade_formula_a*math.pow(self.level+1,2)
  local b = self.formationData.upgrade_formula_b*(self.level + 1)
  local count = math.floor(self.formationData.upgrade_cost.count*(a+b+self.formationData.upgrade_formula_c))
  return game_res_data.New(self.formationData.upgrade_cost.type,self.formationData.upgrade_cost.id,count,self.formationData.upgrade_cost.star)
end
--升级消耗 培养点
function t:UpgradeTrainPointCost()
  return math.floor(self.formationData.upgrade_base_cost_a * (self.level + 1) + self.formationData.upgrade_base_cost_b)
end
return t