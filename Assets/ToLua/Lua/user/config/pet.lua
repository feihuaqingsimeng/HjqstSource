-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','model','scale','speed','home_rotation','rotation','stay_animation','home_Offset','Offset','head_icon'
}

t.t = {
	['1']={'1','p_pet_00','1;1;1','0.1','0;30;0','0;115;0','Idle','-0.18;-0.18;0','-0.7;-0.5;-0.8','p_pet_01'},
	['2']={'2','p_pet_05','1;1;1','0.1','0;30;0','0;115;0','Idle','-0.18;-0.18;0','-0.7;-0.5;-0.8','p_pet_02'},
	['3']={'3','p_pet_05','1;1;1','0.1','0;30;0','0;115;0','Idle','-0.18;-0.18;0','-0.7;-0.5;-0.8','p_pet_03'},
	['4']={'4','p_pet_04','1;1;1','0.1','0;30;0','0;115;0','Idle','-0.18;-0.18;0','-0.7;-0.5;-0.8','p_pet_04'},
	['5']={'5','p_pet_05','1;1;1','0.1','0;30;0','0;115;0','Idle','-0.18;-0.18;0','-0.7;-0.5;-0.8','p_pet_05'},
	['6']={'6','p_pet_01','1;1;1','0.1','0;30;0','0;115;0','Idle','-0.18;-0.18;0','-0.7;-0.5;-0.8','p_pet_01'}
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
