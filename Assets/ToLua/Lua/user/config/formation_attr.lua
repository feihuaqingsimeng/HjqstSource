-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','type','comat_a','comat_b','time','counts','interval','target','effect_type','effect_base','effect_upgrade','unlock_lv'
}

t.t = {
	[1]={'1003','1','0.02','0.001','0','0','0','1','7','0.01','0.0005468','0'},
	[2]={'1004','1','0.02','0.001','0','0','0','1','9','0.015','0.0008406','0'},
	[3]={'1005','1','0.02','0.001','0','0','0','1','6','0.008','0.0004202','0'},
	[4]={'1009','1','0.02','0.001','0','0','0','1','15','0.02','0.0012152','0'},
	[5]={'1003','2','0.0312','0.001','0','0','0','1','7','0.05','0.000853008','0'},
	[6]={'1004','2','0.0312','0.001','0','0','0','1','9','0.07','0.001311336','0'},
	[7]={'1005','2','0.0312','0.001','0','0','0','1','6','0.03','0.000655512','0'},
	[8]={'1009','2','0.0312','0.001','0','0','0','1','15','0.1','0.001895712','0'}
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

