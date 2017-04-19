local t = {}
t.data = {}
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
  
	o.id = tonumber(table.id)
	o.task = tonumber(table.task)
	o.type = tonumber(table.type)
	o.condition_type = tonumber(table.condition_type)
	o.title = table.title
	o.des = table.des
	o.count = tonumber(table.count)
	o.transfer = tonumber(table.transfer)
	o.reward = table.reward
	o.pre_task = table.pre_task
	o.lv_limit = table.lv_limit
	o.pic = table.pic
	o.title1 = table.title1
	--o.1des = table.1des
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

local function Start()
	local origin = dofile('task')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t