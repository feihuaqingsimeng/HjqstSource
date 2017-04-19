local t = {}
t.datas = {}

local role_attr = require('ui/role/model/role_attr')
local game_res_data = require('ui/game/model/game_res_data')

local function Parse(item)
	item.id = tonumber(item.id)
	item.hero_id = tonumber(item.hero_id)
	item.friend_id=string.split2(item.friend_id,';',':')
  item.friendResData = {}
	for i=1,#item.friend_id do
		for j=1,#item.friend_id[i] do
			item.friend_id[i][j]=tonumber(item.friend_id[i][j])
		end
    item.friendResData[i] = game_res_data.New(BaseResType.Hero,item.friend_id[i][1],0,item.friend_id[i][2])
	end
  item.attr = {}
  item.comat = {}
  local index = 1
	if item.attribute_type1 then
		item.attribute_type1=tonumber(item.attribute_type1)
		item.attribute1=tonumber(item.attribute1)
    item.attr[index] = role_attr.New(item.attribute_type1,item.attribute1)
    item.comat[index] = tonumber(item.comat1)
    index = index + 1
	end
	if item.attribute_type2 then
		item.attribute_type2=tonumber(item.attribute_type2)
		item.attribute2=tonumber(item.attribute2)
    item.attr[index] = role_attr.New(item.attribute_type2,item.attribute2)
    item.comat[index] = tonumber(item.comat2)
	end
end

local function Start()
	local origin = dofile('hero_relationship')
	local item = nil
	local item2=nil
	for k in pairs(origin.t) do
		item=origin.GetItem(k)
		Parse(item)
		if not t.datas[item.hero_id] then t.datas[item.hero_id]={} end
		t.Sort(t.datas[item.hero_id],item)
	end
end

function t.Sort(tb,it)
	if #tb<1 then
		table.insert(tb,it)
	else
		local flag=false
		for i=1,#tb do
			if tb[i].id>it.id then
				flag=true
				table.insert(tb,i,it)
				break
			end
		end
		if not flag then
			table.insert(tb,it)
		end
	end
end

function t.GetDataBydataid(id)
	-- if not t.datas[id] then
	-- 	print('无法获取英雄配表数据,请检查配置文件,模型id: ',id)
	-- end
	return t.datas[id]
end

function t.GetDataById(shipId)
	for k,v in pairs(t.datas) do
		for k1,v1 in pairs(v) do
      if v1.id == shipId then
        return v1
      end
    end
	end
  return nil
end
Start()
return t