-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','time','title','messinfo','register'
}

t.t = {
	['1']={'1','1899/12/31 12:00:00','push_title_1','push_messinfo_1','1'},
	['2']={'2','1899/12/31 18:00:00','push_title_2','push_messinfo_2','1'},
	['3']={'3','1899/12/31 21:00:00','push_title_3','push_messinfo_3','1'},
	['4']={'4','1899/12/31 20:00:00','push_title_4','push_messinfo_4','1'}
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
