-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'heroId','star','exp'
}

t.t = {
	[1]={'98','1','100'},
	[2]={'98','2','150'},
	[3]={'98','3','200'},
	[4]={'98','4','300'},
	[5]={'98','5','400'},
	[6]={'98','6','500'},
	[7]={'99','1','1000'},
	[8]={'99','2','1500'},
	[9]={'99','3','2000'},
	[10]={'99','4','3000'},
	[11]={'99','5','4000'},
	[12]={'99','6','5000'},
	[13]={'100','1','10000'},
	[14]={'100','2','15000'},
	[15]={'100','3','20000'},
	[16]={'100','4','30000'},
	[17]={'100','5','40000'},
	[18]={'100','6','50000'}
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

