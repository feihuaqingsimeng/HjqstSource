-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','name','des','p1','p2','p3','p4','p5','p6','p7','p8','p9','max_lv','upgrade_cost','upgrade_formula_a','upgrade_formula_b','upgrade_formula_c','upgrade_base_cost_a','upgrade_base_cost_b','formation_condition','unlock_cost'
}

t.t = {
	['1003']={'1003','formation_name_1003','formation_des_1003','0','1','0','1','1','1','0','1','0','-1','8:0:1000:0','0','0.2','1','0.1','1','4003','9:0:980:0'},
	['1004']={'1004','formation_name_1004','formation_des_1004','1','0','1','1','0','1','0','1','0','-1','8:0:1000:0','0','0.2','1','0.1','1','4004','9:0:980:0'},
	['1005']={'1005','formation_name_1005','formation_des_1005','0','1','0','1','0','1','1','1','0','-1','8:0:1000:0','0','0.2','1','0.1','1','4005','9:0:980:0'},
	['1009']={'1009','formation_name_1009','formation_des_1009','0','1','0','1','1','0','1','0','1','-1','8:0:1000:0','0','0.2','1','0.1','1','4009','9:0:980:0'}
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
