-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','time','award'
}

t.t = {
	['1']={'1','120','8:0:1000:0;9:0:20:0'},
	['2']={'2','300','8:0:2000:0;9:0:20:0'},
	['3']={'3','600','8:0:3000:0;9:0:20:0'},
	['4']={'4','1200','8:0:4000:0;9:0:20:0'},
	['5']={'5','1800','8:0:5000:0;9:0:20:0;4:10500:5:0'}
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
