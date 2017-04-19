-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','quality','foundation','man_max','time','protect_time'
}

t.t = {
	['1']={'1','1','200','999','43200','1800'},
	['2']={'2','2','400','5','28800','1800'},
	['3']={'3','2','400','5','28800','1800'},
	['4']={'4','2','400','5','28800','1800'},
	['5']={'5','2','400','5','28800','1800'},
	['6']={'6','2','400','5','28800','1800'},
	['7']={'7','3','600','3','21600','1800'},
	['8']={'8','3','600','3','21600','1800'},
	['9']={'9','3','600','3','21600','1800'},
	['10']={'10','3','600','3','21600','1800'},
	['11']={'11','3','600','3','21600','1800'},
	['12']={'12','4','800','2','18000','1800'},
	['13']={'13','4','800','2','18000','1800'},
	['14']={'14','4','800','2','18000','1800'},
	['15']={'15','4','800','2','18000','1800'},
	['16']={'16','4','800','2','18000','1800'},
	['17']={'17','5','1200','1','14400','1800'},
	['18']={'18','5','1200','1','14400','1800'},
	['19']={'19','5','1200','1','14400','1800'},
	['20']={'20','5','1200','1','14400','1800'}
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
