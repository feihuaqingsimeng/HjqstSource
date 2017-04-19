-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'sever_id','server_name','server_ip','server_port','inner','server_state'
}

t.t = {
	['1']={'1','servername1','192.168.3.107','90','1','0'},
	['2']={'2','servername2','192.168.3.114','90','1','0'},
	['1000']={'1000','servername1000','120.132.58.75','90','0','1'}
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
