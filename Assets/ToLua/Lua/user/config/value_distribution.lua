-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','dis_5','dis_10','dis_15','dis_20','dis_25','dis_30','dis_35','dis_40','dis_45','dis_50','dis_55','dis_60','dis_65','dis_70','dis_75','dis_80','dis_85','dis_90','dis_95','dis_100'
}

t.t = {
	['1']={'1','0','0','10','10','10','10','10','10','10','10','10','10','10','10','10','10','10','10','10','10'},
	['2']={'2','0','200','150','150','200','200','200','200','100','100','80','80','60','60','40','40','40','40','30','30'},
	['3']={'3','0','320','200','200','200','200','200','200','100','100','80','80','20','20','15','15','15','15','10','10'},
	['4']={'4','0','472','300','300','200','200','120','120','60','60','40','40','18','18','12','12','8','8','6','6'},
	['5']={'5','0','472','300','300','200','200','120','120','60','60','40','40','18','18','12','12','8','8','6','6'}
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
