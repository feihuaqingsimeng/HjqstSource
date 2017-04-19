-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','quality','prize_id_fail','prize_id_succ','need_equip','need_gold_cost','gold_chance','need_diamond_cost','diamond_chance'
}

t.t = {
	['1']={'1','2','10020','10021','5','8:0:5000:0','80','9:0:50:0','100'},
	['2']={'2','3','10030','10031','5','8:0:50000:0','60','9:0:200:0','100'},
	['3']={'3','4','10040','10041','5','8:0:500000:0','30','9:0:1000:0','60'}
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
