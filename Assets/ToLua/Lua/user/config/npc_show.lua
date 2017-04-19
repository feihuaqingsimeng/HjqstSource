-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','npc_name','face1','face2','face3','face4','face5','face_position'
}

t.t = {
	['nv_pu']={'nv_pu','image_npc_01','npc_01_face_1','npc_01_face_2','npc_01_face_3','npc_01_face_4','npc_01_face_5','152.9;430'},
	['p_artillery']={'p_artillery','p_artillery','p_artillery','p_artillery','p_artillery','p_artillery','p_artillery','0;0'},
	['52']={'52','52','52','52','52','52','52','0;0'},
	['203']={'203','203','203','203','203','203','203','0;0'},
	['27']={'27','27','27','27','27','27','27','0;0'},
	['104']={'104','104','104','104','104','104','104','0;0'},
	['121']={'121','121','121','121','121','121','121','0;0'},
	['138']={'138','138','138','138','138','138','138','0;0'},
	['129']={'129','129','129','129','129','129','129','0;0'},
	['200']={'200','200','200','200','200','200','200','0;0'}
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
