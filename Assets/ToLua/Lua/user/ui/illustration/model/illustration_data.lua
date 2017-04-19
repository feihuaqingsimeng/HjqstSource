local t = {}
local item = {}
t.data = {}
item.__index = item

local game_res_data = require('ui/game/model/game_res_data')
local role_attr = require('ui/role/model/role_attr')

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.type_name = table.type_name
	o.sheet = tonumber(table.sheet)
	o.sheet_name = table.sheet_name
  o.hero_id = tonumber(table.hero_id)
  o.hero_star = 1
  o.illustration_type = tonumber(table.atlas_type)
  o.resData = game_res_data.New(BaseResType.Hero,o.hero_id,0,1)
  o.roleAttrDic = {}
  if table.attr_type ~= nil then
    local attr_type = string.split2number(table.attr_type,';')
    local attr_value = string.split2number(table.attr_value,';')
    for k,v in pairs(attr_type) do
      o.roleAttrDic[v] = role_attr.New(v,attr_value[k])
    end
  end
  o.comat = 0
  if table.comat then
    o.comat = tonumber(table.comat)
  end
  
	return o
end

function t.GetDataById(id)
	return t.data[id]
end

function t.GetDataByHeroId(id)
	for k,v in pairs(t.data) do
    if v.illustration_type == IllustrationType.hero and v.hero_id == id then
      return v
    end
  end
  return nil
end

local function Start()
	local origin = dofile('atlas')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
end

Start()
return t