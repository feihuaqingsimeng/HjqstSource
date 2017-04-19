-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','name','shop_type'
}

t.t = {
	['1']={'1','ui.noble_shop_view.hero_tab_title','1'},
	['2']={'2','ui.noble_shop_view.equipment_tab_title','1'},
	['3']={'3','ui.noble_shop_view.diamond_tab_title','1'},
	['4']={'4','ui.noble_shop_view.action_point_tab_title','1'},
	['5']={'5','ui.noble_shop_view.gold_tab_title','1'},
	['6']={'6','ui.noble_shop_view.others_tab_title','1'}
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
