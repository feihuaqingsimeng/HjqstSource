-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','rank_min','rank_max','award'
}

t.t = {
	['1']={'1','1','1','19:0:50:0;8:0:80:0;9:0:40:0'},
	['2']={'2','2','5','19:0:50:0;8:0:80:0;9:0:40:0'},
	['3']={'3','6','10','19:0:50:0;8:0:80:0;9:0:40:0'},
	['4']={'4','11','20','19:0:50:0;8:0:80:0;9:0:40:0'},
	['5']={'5','21','50','19:0:50:0;8:0:80:0;9:0:40:0'},
	['6']={'6','51','100','19:0:50:0;8:0:80:0;9:0:40:0'},
	['7']={'7','101','200','19:0:50:0;8:0:80:0;9:0:40:0'},
	['8']={'8','201','1000','19:0:50:0;8:0:80:0;9:0:40:0'},
	['999']={'999','0','0','19:0:100:0'},
	['998']={'998','0','0','19:0:100:0'}
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
