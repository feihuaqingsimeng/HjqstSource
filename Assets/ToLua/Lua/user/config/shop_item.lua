-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','shop_sheet','buy_type','itemid','name','pic','des','cost','free_time','max','discount'
}

t.t = {
	['1']={'1','1','1','1','ui.shop_hero.name_1','icon_book_1','ui.shop_hero.describe_1','9:0:120:0','600','5','1'},
	['2']={'2','1','2','2','ui.shop_hero.name_2','icon_book_2','ui.shop_hero.describe_2','9:0:1200:0','-1','-1','0.8'},
	['3']={'3','1','1','3','ui.shop_hero.name_3','icon_book_3','ui.shop_hero.describe_3','14:0:120:0','300','5','1'},
	['4']={'4','2','1','4','ui.shop_equip.name_1','icon_equip_1','ui.shop_equip.describe_1','9:0:120:0','-1','-1','1'},
	['5']={'5','2','1','5','ui.shop_equip.name_2','icon_equip_2','ui.shop_equip.describe_2','14:0:120:0','-1','-1','1'}
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
