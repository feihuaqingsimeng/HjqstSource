local t = {}
local item = {}
item.__index = item
t.rankRewardData = ArrayList.New('item') --排行数据
t.costData = ArrayList.New('item') --单抽钻石消费数据
t.tenCostData = ArrayList.New('item') --十抽钻石消费
t.RewardData = ArrayList.New('item')  -- 转盘奖励数据
local game_res_data = require('ui/game/model/game_res_data')

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.type = tonumber(table.type)
  if o.type == 1 or o.type == 0 then
    o.startCostCount = tonumber(table.param1)
    o.endCostCount = tonumber(table.param2)
    o.costResData = game_res_data.NewByString(table.param3)
  elseif o.type == 2 then
    o.sort = tonumber(table.param1)
    o.rewardResData = game_res_data.NewByString(table.param2)
  else
    o.startRank = tonumber(table.param1)
    o.endRank = tonumber(table.param2)
    o.rewardResData = game_res_data.NewByString(table.param3)
  end
	return o
end

--通过转盘次数获取单抽花费
function t.GetOneCostByDrawCount(count)
  for k,v in ipairs(t.costData:GetDatas()) do
    if v.startCostCount <=count and v.endCostCount >= count then
      return v
    end
    
  end
  return nil
end
--通过转盘次数获取十抽花费
function t.GetTenCostByDrawCount(count)
  local can = false
  for k,v in ipairs(t.tenCostData:GetDatas()) do
    if v.startCostCount <=count and v.endCostCount >= count then
      return v
    end
  end
  return nil
end
--十抽打折
function t.GetNextDifferentDataCostByTen(count)
  for k,v in ipairs(t.tenCostData:GetDatas()) do
    if v.startCostCount > count then
      return v
    end
  end
  return nil
end
--转盘奖励数据(gameResData)
function t.GetRewardDatas()
  local rewardTable = {}
  for k,v in pairs(t.RewardData:GetDatas()) do
      rewardTable[v.sort] = v.rewardResData
  end
  return rewardTable
end
--转盘奖励数据(gameResData)
function t.GetRewardDataByIndex(index)
  return t.RewardData:Get(index).rewardResData
end
--转盘排行奖励数据(gameResData)
function t.GetRankRewardByRank(rank)
  
  for k,v in pairs(t.rankRewardData:GetDatas()) do
    if rank >= v.startRank and rank <= v.endRank then
      return v.rewardResData
    end
  end
  return nil
end

local function Start()
	local origin = dofile('roulette')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
    if newItem.type == 0 then
      t.costData:Add(newItem)
    elseif newItem.type == 1 then
      t.tenCostData:Add(newItem)
    elseif newItem.type == 2 then
      t.RewardData:Add(newItem)
    elseif newItem.type == 3 then
      t.rankRewardData:Add(newItem)
    end
	end)
  t.costData:Sort(function(a,b) 
      return a.startCostCount < b.startCostCount 
    end)
  t.tenCostData:Sort(function(a,b) 
      return a.startCostCount < b.startCostCount 
    end)
  t.RewardData:Sort(function(a,b)
      return a.sort < b.sort
    end)
end

Start()
return t