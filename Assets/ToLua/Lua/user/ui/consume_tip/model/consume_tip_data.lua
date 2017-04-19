local t = {}
t.data = {}
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.function_id)
	o.des = table.des
	o.notice = table.notice
	return o
end
--id 为 function_id 
function t.GetDataById(id)
	return t.data[id]
end

local function Start()
	local origin = dofile('consumer_tips')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[newItem.id] = newItem
	end)
end

Start()
return t