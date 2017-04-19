-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'star','gold','level','rate1','rate2','rate3'
}

t.t = {
	['1']={'1','10','1','33','66','100'},
	['2']={'2','40','1','33','66','100'},
	['3']={'3','160','1','33','66','100'},
	['4']={'4','1280','1','33','66','100'},
	['5']={'5','10240','1','33','66','100'},
	['6']={'6','12040','1','33','66','100'}
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
