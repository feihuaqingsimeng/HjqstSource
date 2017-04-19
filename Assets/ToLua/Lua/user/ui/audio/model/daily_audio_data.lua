local t = {}
t.data = Dictionary.New('int','ArrayList<item>')
local item = {}
item.__index = item

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.id)
	o.type = tonumber(table.type)
	o.heroid = tonumber(table.heroid)
	o.audioList = string.split(table.audio,';')
	return o
end

function t.GetDataByTypeHeroId(type,heroId)
  local datas = t.data:Get(type)
  if datas == nil then
    print('[daily_audio_data] can not find type:',type)
    return nil
  end
	for k,v in pairs(datas:GetDatas()) do
    if v.heroid == heroId then
      return v
    end
  end
  print('[daily_audio_data ]can not find heroId:',heroId,'in type:',type)
  return nil
end

function t.GetDataByTypeId(type,id)
  local datas = t.data:Get(type)
  if datas == nil then
    return nil
  end
	for k,v in pairs(datas:GetDatas()) do
    if v.id == id then
      return v
    end
  end
  return nil
end

local function Start()
	local origin = dofile('daily_audio')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
    if t.data:Get(newItem.type) == nil then
      t.data:Add(newItem.type,ArrayList.New('item'))
    end
		t.data:Get(newItem.type):Add(newItem)
	end)
end

Start()
return t