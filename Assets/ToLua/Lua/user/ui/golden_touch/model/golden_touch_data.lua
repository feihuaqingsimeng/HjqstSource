local t = {}
t.goldenData = {}
t.timesCostData = {}


local GoldenItem = {}
GoldenItem.__index = GoldenItem
--点金手金币花费表结构
function GoldenItem.New(table)
	local o = {}
	setmetatable(o,GoldenItem)
	o.level = tonumber( table.level)
	o.basegold =tonumber( table.basegold)
	o.timesadd = tonumber(table.timesadd)
	return o
end
--点金手钻石花费表结构
local TimesCostItem = {}
TimesCostItem.__index = TimesCostItem
function TimesCostItem.New(table)
	local o = {}
	setmetatable(o,TimesCostItem)
	o.type = tonumber(table.type)
	o.times = tonumber(table.times)
	o.cost = tonumber(table.cost)
	return o
end

--通过账号等级和使用的次数获得金币
function t.GetGoldByAccountLevel(level,times)
	local data = t.goldenData[level]
  if data == nil then
    Debugger.LogError('can not find golden data by account level:'..level)
  else
    return math.floor(data.basegold *(1+ times*data.timesadd))
  end
  return -1
end
--通过使用的次数获得消耗的钻石
function t.GetDiamondCostByTimes(times)

  local length = #t.timesCostData
  if times > length then
    times = length
  end
	return t.timesCostData[times].cost
end

local function Start()
	local origin = dofile('goldenhand')
	origin.ForEach(function(id,table)
		local newItem = GoldenItem.New(table)
		t.goldenData[id] = newItem
	end)

origin = dofile('timescost')
	origin.ForEach(function(id,table)
		local newItem = TimesCostItem.New(table)
    if newItem.type == 1 then
      t.timesCostData[newItem.times] = newItem
    end
		
	end)
end

Start()
return t