-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'gold','diamond','crystal','honor','arenaPoint','sociatyPoint','pvpAction','hero_lim_num','equip_lim_num','base_dungeon_id','init_format','init_hero','head_no','pocket_hero','init_item'
}

t.t = {
	['0']={'0','0','0','0','30','30','0','150','50','10101','1003',nil,'2003',nil,nil}
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
