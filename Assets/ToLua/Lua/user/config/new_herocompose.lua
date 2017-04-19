-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','hero_id','need_cost','need_material'
}

t.t = {
	['1']={'1','2:110:1:4','9:0:800:0','2:107:1:4;3:11001:1:0;4:10321:27:0;4:10322:11:0;4:10303:1:0'},
	['2']={'2','2:112:1:4','9:0:800:0','4:10320:27:0;4:10321:27:0;4:10322:11:0;4:10303:1:0'},
	['3']={'3','2:115:1:4','9:0:800:0','4:10320:27:0;4:10321:27:0;4:10322:11:0;4:10303:1:0'},
	['4']={'4','2:117:1:4','9:0:800:0','4:10320:27:0;4:10321:27:0;4:10322:11:0;4:10303:1:0'}
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
