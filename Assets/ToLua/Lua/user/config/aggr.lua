-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'star','exp_provide','quality'
}

t.t = {
	[1]={'1','20','1'},
	[2]={'2','40','1'},
	[3]={'3','80','1'},
	[4]={'4','120','1'},
	[5]={'5','200','1'},
	[6]={'6','320','1'},
	[7]={'1','40','2'},
	[8]={'2','80','2'},
	[9]={'3','160','2'},
	[10]={'4','240','2'},
	[11]={'5','400','2'},
	[12]={'6','640','2'},
	[13]={'1','60','3'},
	[14]={'2','120','3'},
	[15]={'3','240','3'},
	[16]={'4','360','3'},
	[17]={'5','600','3'},
	[18]={'6','960','3'},
	[19]={'1','100','4'},
	[20]={'2','200','4'},
	[21]={'3','400','4'},
	[22]={'4','600','4'},
	[23]={'5','1000','4'},
	[24]={'6','1600','4'},
	[25]={'1','160','5'},
	[26]={'2','320','5'},
	[27]={'3','640','5'},
	[28]={'4','960','5'},
	[29]={'5','1600','5'},
	[30]={'6','2560','5'}
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

