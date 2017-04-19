-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','hero1','hero2','hero3','hero4','star','prize','fail_prize','probability','type'
}

t.t = {
	['1']={'1','0','0','0','0','1','202','207','100','1'},
	['2']={'2','0','0','0','0','2','203','208','100','1'},
	['3']={'3','0','0','0','0','3','204','209','100','1'},
	['4']={'4','0','0','0','0','4','205','210','100','1'},
	['5']={'5','0','0','0','0','5','206','211','100','1'},
	['1001']={'1001','8','9','0','0','3','203','209','100','2'}
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
