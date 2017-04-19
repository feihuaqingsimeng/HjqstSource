-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'positionId','position','rowNum','columnNum'
}

t.t = {
	['1']={'1','-2.3:0:80','1','1'},
	['2']={'2','-2.3:0:75','2','1'},
	['3']={'3','-2.3:0:70','3','1'},
	['4']={'4','-4.4:0:80','1','2'},
	['5']={'5','-4.4:0:75','2','2'},
	['6']={'6','-4.4:0:70','3','2'},
	['7']={'7','-6.5:0:80','1','3'},
	['8']={'8','-6.5:0:75','2','3'},
	['9']={'9','-6.5:0:70','3','3'},
	['101']={'101','2.3:0:80','1','1'},
	['102']={'102','2.3:0:75','2','1'},
	['103']={'103','2.3:0:70','3','1'},
	['104']={'104','4.4:0:80','1','2'},
	['105']={'105','4.4:0:75','2','2'},
	['106']={'106','4.4:0:70','3','2'},
	['107']={'107','6.5:0:80','1','3'},
	['108']={'108','6.5:0:75','2','3'},
	['109']={'109','6.5:0:70','3','3'}
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
