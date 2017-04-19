-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','shop_type','sheet_name','vip_limit','item_list'
}

t.t = {
	['11']={'11','1','英雄','0','card_random'},
	['13']={'13','1','钻石','0','diamond'},
	['14']={'14','1','行动力','0','limit_item'},
	['15']={'15','1','金币','0','limit_item'},
	['16']={'16','1','其他','0','shop_goods'},
	['21']={'21','2','vip3级开放','3',nil},
	['22']={'22','2','vip4级开放','4',nil},
	['23']={'23','2','vip5级开放','5',nil},
	['24']={'24','2','vip6级开放','6',nil},
	['25']={'25','2','vip7级开放','7',nil},
	['26']={'26','2','vip8级开放','8',nil}
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
