-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','type','duration','scale','color','change_time','hero_id','back_hero_id','animation_name','back_animation_name','skill_id','back_skill_id','backable'
}

t.t = {
	['1001']={'1001','1','20','0;0;0','255;255;255;0','2','2901','0','0','1002','0','1002','0'},
	['1002']={'1002','1','20','0;0;0','255;255;255;0','2','201','0','0','0','0','0','0'},
	['1003']={'1003','2','20','0;0;0','255;255;255;0','2','0','0','p_artillery_t','p_artillery','29010','2010','1'},
	['1052']={'1052','2','50','0;0;0','255;255;255;0','2','0','0','m_052_artillery_t1','m_052_artillery','51011','511','0'},
	['1053']={'1053','2','150','0;0;0','255;255;255;0','6.7','0','0','m_052_artillery_t1','m_052_artillery','51011;51012;51010','511;512;510','0'},
	['1054']={'1054','2','150','0;0;0','255;255;255;0','4','0','0','m_053_gunner_t1','m_053_gunner','53011;53012;53010','531;532;530','0'},
	['1055']={'1055','2','150','0;0;0','255;255;255;0','5','0','0','m_053_gunner_t2','m_053_gunner_t1','530011;530012;530010','53011;53012;53010','0'},
	['1056']={'1056','2','150','0;0;0','255;255;255;0','5.333','0','0','m_052_artillery_t2','m_052_artillery_t1','510011;510012;510010','51011;51012;51010','0'},
	['1057']={'1057','2','150','0;0;0','255;255;255;0','2','0','0','m_050_warrior_t1','m_050_warrior','5001;5002;5000','501;502;500','0'},
	['1058']={'1058','2','150','0;0;0','255;255;255;0','2','0','0','m_050_warrior_t2','m_050_warrior_t1','50001;50002;50000','5001;5002;5000','0'}
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
