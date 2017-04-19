-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'name','value','des'
}

t.t = {
	['skill_hurt_a']={'skill_hurt_a','0.03','技能伤害公式系数。Hurt=basic*(1+(star_now-star_min)*skill_hurt_a)技能层数效果。一层的话，技能层数效果为1'},
	['float_hurt_add']={'float_hurt_add','1','浮空伤害加成'},
	['caron_hurt_add']={'caron_hurt_add','0.01','连击伤害加成系数。连击伤害加成公式：caron_hurt_add*连击数'},
	['caron_hurt_max']={'caron_hurt_max','0.3','连击伤害加成的最大值。连击伤害不会大于这个值'},
	['caron_hurt_time']={'caron_hurt_time','5','连击间隔多少秒，才算连击。单位：秒'},
	['ai_skill_round']={'ai_skill_round','1','AI释放技能时候，第几圈释放技能'}
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
