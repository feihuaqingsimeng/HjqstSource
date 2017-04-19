local t = {}
t.data = {}
local item = {}
item.__index = item

t.lastNeedData = nil

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.aggr_lv = tonumber(table.aggr_lv)
	o.exp_need = tonumber(table.exp_need)
  o.exp_total =  tonumber(table.exp_totle)
	o.gold_need = tonumber(table.gold_need)
	o.aggr_value = tonumber(table.aggr_value)
	o.color = tonumber(table.colour)
	o.strengthenAddShowValue = tonumber(table.aggr_des)
  o.accountLv = tonumber(table.player_lv)
	return o
end
function item:NewBySelf()
	local o = {}
	setmetatable(o,item)
	o.aggr_lv = self.aggr_lv
	o.exp_need = self.exp_need
  o.exp_total =  self.exp_total
	o.gold_need = self.gold_need
	o.aggr_value = self.aggr_value
	o.color = self.color
	o.strengthenAddShowValue = self.strengthenAddShowValue
  o.accountLv = self.accountLv
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

function t.MaxLevel()
  return t.maxLevel
end

function t.LastNeedData()
  return t.lastNeedData
end

function t.GetDataByExp(expTotal)
  local exp = 0
  for k, v in pairs(t.data) do
    exp = exp + v.exp_need
    if expTotal < exp then
      return v
    end
  end
  return nil
end
--
function t.GetAccountLevelByStrengthenLevel(strengthenLevel)
  local data = t.GetDataById(strengthenLevel-1)
  if data == nil then
    return 0
  end
  return data.accountLv
end

function t.GetLimitDataByAccountLevel(accountLevel)
  local limit = t.LastNeedData()
  if limit.accountLv <= accountLevel then
    limit = limit:NewBySelf()
    limit.aggr_lv = limit.aggr_lv + 1
    return limit
  end
  for k,v in pairs(t.data) do
    limit = v
    if v.accountLv > accountLevel then
      break
    end
  end
  return limit
end

function t.GetTotalExp(level)
  if level == 0 then
    return 0
  end
  local data = t.data[level-1]
  if data then
    return data.exp_total
  end
  return t.LastNeedData().exp_total + t.LastNeedData().exp_need
end

local function Start()
	local origin = dofile('aggr_need')
  
  
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem

	end)
  table.sort(t.data,function(a,b)
    return a.aggr_lv < b.aggr_lv
  end)

  for k,v in pairs(t.data) do
    t.lastNeedData = v
    t.maxLevel = v.aggr_lv+1
  end
end

Start()
return t