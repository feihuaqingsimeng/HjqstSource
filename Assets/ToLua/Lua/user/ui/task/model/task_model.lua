local t = {}
local name = 'task_model'

local game_res_data = require('ui/game/model/game_res_data')

local function Start ()
  gamemanager.RegisterModel(name, t)
end

Start()
--获得task条件描述
function t.GetTaskConditionDescriptionWithColor(conditionData)
  local isFinished = t.IsTaskConditionFinished(conditionData)
  if isFinished then
    return string.format('<color=#00ff00>%s</color>',LocalizationController.instance:Get( conditionData.desc))
  else 
    return string.format('<color=#ff0000>%s</color>',LocalizationController.instance:Get( conditionData.desc))
  end
end
--任务有没有完成
function t.IsTaskConditionFinished(conditionData)
  if conditionData.type == TaskType.AccountLevelRequirement then
    return gamemanager.GetModel('game_model').accountLevel  >= tonumber(conditionData.parameter1)
  elseif conditionData.type == TaskType.PassDungeonTimes then
    return gamemanager.GetModel('dungeon_model').GetDungeonInfo(tonumber(conditionData.parameter1)).star > 0
  elseif conditionData.type == TaskType.VIPLevelRequirement then
    return gamemanager.GetModel('vip_model').vipLevel >= tonumber(conditionData.parameter1)
  elseif conditionData.type == TaskType.MultipleHeroLevelRequirement then
    return gamemanager.GetModel('hero_model').GetHeroInfosLevelMoreThanCount(tonumber(conditionData.parameter2)) >= tonumber(conditionData.parameter1)
  elseif conditionData.type == TaskType.MultipleHeroStarRequirement then
    return gamemanager.GetModel('hero_model').GetHeroInfosStarMoreThanCount(tonumber(conditionData.parameter2)) >= tonumber(conditionData.parameter1)
  elseif conditionData.type == TaskType.OwnResouce then
    local resData = game_res_data.NewByString(conditionData.parameter1)
    return gamemanager.GetModel('game_model').GetBaseResourceValue(resData.type) >= resData.count
  end
  return false
end
return t