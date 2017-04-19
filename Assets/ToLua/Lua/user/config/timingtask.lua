-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'no','taskDes','type','period','startDate','startHour','startMin','endDate','endHour','endMin'
}

t.t = {
	['100001']={'100001','排名竞技场结算','1','1440','20141201','21','0','21141201','0','0'},
	['100003']={'100003','黑市全服商品刷新','1','1440','20141201','0','0','21141201','0','0'},
	['100004']={'100004','黑市限时商品刷新','3','360','20141201','0','0','21141201','0','0'},
	['100005']={'100005','总排行榜多久刷新','2','60','20141201','0','0','21141201','0','0'},
	['100006']={'100006','商店刷新时间1','1','86400','20141201','8','0','21141201','0','0'},
	['100007']={'100007','商店刷新时间2','1','86400','20141201','15','0','21141201','0','0'},
	['100008']={'100008','商店刷新时间3','1','86400','20141201','21','0','21141201','0','0'}
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
