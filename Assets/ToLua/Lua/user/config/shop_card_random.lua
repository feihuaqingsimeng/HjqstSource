-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','sheet_num','buy_type','prize_id','prize_pro_id','first_prize','name','pic','bg_pic','des','cost','free_time','max','discount'
}

t.t = {
	['11']={'11','11','1','1','0','21','ui.shop_hero.name_1','icon_book_1','ui_equip_shop_9','ui.shop_hero.describe_1','9:0:198:0','86400','2','0.9'},
	['12']={'12','11','2','2;2;2;2;2;2;2;2;2;2','4','0','ui.shop_hero.name_2','icon_book_2','ui_equip_shop_10','ui.shop_hero.describe_2','9:0:1860:0','-1','-1','0.9'},
	['13']={'13','11','1','3','0','0','ui.shop_hero.name_3','icon_book_3','ui_equip_shop_11','ui.shop_hero.describe_3','14:0:280:0','-1','-1','1'}
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
