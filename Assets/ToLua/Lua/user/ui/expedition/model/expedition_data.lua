local t = {}
t.data = {}
t.sortedDataList = {}
local item = {}
item.__index = item

t.firstData = nil

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.type = tonumber(table.type)
	o.reward_show = table.reward_show
	o.reward = table.reward
	o.position = table.position
	o.chapter = table.chapter
	o.prefab = table.prefab
	o.comat_min = table.comat_min
	o.comat_max = table.comat_max
	o.pre_id = table.pre_id
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

function t.GetNextExpeditionData(curId)
  local isNext = false
  local nextData = nil
  
  for k,v in pairs(t.sortedDataList) do
    if isNext then
      nextData = v
      break
    end  
    if curId == v.id then
      isNext = true
    end
  end
  return nextData
end

local function Start()
  local minId = 0
  local index = 1
	local origin = dofile('faraway_expedition')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
    t.sortedDataList[index] = newItem
    index = index + 1
    if t.firstData == nil or t.firstData.id > id then
      t.firstData = newItem
    end
    
	end)
  table.sort(t.sortedDataList,function(a,b)
      return a.id < b.id
    end)

end

Start()
return t