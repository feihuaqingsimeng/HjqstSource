-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','sheet_num','buy_type','item','name','des','pic','cost','item_num'
}

t.t = {
	['64']={'64','16','8','4:10600:10:0','shop_goods_item.name.63',nil,'icon_package_1','9:0:100:0','-1'},
	['65']={'65','16','8','4:10601:10:0','shop_goods_item.name.64',nil,'icon_package_1','9:0:880:0','-1'},
	['66']={'66','16','8','4:10602:10:0','shop_goods_item.name.65',nil,'icon_package_1','9:0:6800:0','-1'}
}

function t.ForEach(func)
	if not func then return end
	local ky = nil
	local v = nil
	for i,j in pairs(t.t) do
		local r={}
		ky=tonumber(i)
		for k=1,#indexs do
			v=indexs[k]
			if v and v ~= '' then
				r[v]=j[k]
			end
		end

		if ky then func(ky,r) 
		else func(i,r) end
	end
end

function t.GetItem(id)
	id=tostring(id)
    local item=t[id]
	if item then return item end
	item=t.t[id]
	local result = {}
	local v = nil
	for i=1,#indexs do
		v=indexs[i]
		if v and v ~= '' then
			result[v]=item[i]
		end
	end
	t[id]=result
	return result
end

return t
