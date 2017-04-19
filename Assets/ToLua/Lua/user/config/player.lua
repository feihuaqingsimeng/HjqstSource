-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','hero_id','portrait','avatar','figureImage','offence','defence','task_condition','transfer_item','hopping','summonId','summonSkillId','summonMax','pet_breakthrough_item','pet_id'
}

t.t = {
	['200']={'200','200','image_jobcard_00','1','tyro','4','4',nil,'4:52004:20:0','201;202;203','2013','20131','1000000','4:10200:1:0','1'},
	['201']={'201','201','image_jobcard_01','4','artillery','4','4',nil,'4:52015:20:0','200;202;203','2013','20131','1000000','4:10201:1:0','5'},
	['202']={'202','202','image_jobcard_02','5','gunner','4','2',nil,'4:52026:20:0','200;201;203','2013','20131','1000000','4:10202:1:0','4'},
	['203']={'203','203','image_jobcard_03','6','warrior','4','4',nil,'4:52036:20:0','200;201;202','2013','20131','1000000','4:10200:1:0','6'}
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
