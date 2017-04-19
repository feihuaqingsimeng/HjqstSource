local t = {}
t.data = {}
local item = {}
item.__index = item
local game_res_data = require('ui/game/model/game_res_data')

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.item = game_res_data.NewByString(table.item)
	o.des = table.des
	o.jump_type1 = tonumber(table.jump_type1)
	o.jump_page1 = tonumber(table.jump_page1)
	o.jump_type2 = tonumber(table.jump_type2)
	o.jump_page2 = tonumber(table.jump_page2)
	o.jump_type3 = tonumber(table.jump_type3)
	o.jump_page3 = tonumber(table.jump_page3)
	o.jump_type4 = tonumber(table.jump_type4)
	o.jump_page4 = tonumber(table.jump_page4)
	o.des1 = table.des1
	o.desc2 = table.desc2
	o.des3 = table.des3
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

function t.GetDataByMockGameResData (mockGameResData)
  for k, v in pairs(t.data) do
    if v.item.type == mockGameResData.type
    and v.item.id == mockGameResData.id then
      return v
    end
  end
end

local function Start()
	local origin = dofile('drop_message')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t