local t = {}

local function Start()
	local origin = dofile('const')
	t.skill_hurt_a = tonumber(origin.t['skill_hurt_a'][2])
	t.float_hurt_add = tonumber(origin.t['float_hurt_add'][2])
	t.caron_hurt_add = tonumber(origin.t['caron_hurt_add'][2]) 
	t.caron_hurt_max = tonumber(origin.t['caron_hurt_max'][2])
	t.caron_hurt_time = tonumber(origin.t['caron_hurt_time'][2])
	t.ai_skill_round = tonumber(origin.t['ai_skill_round'][2])
end

Start()
return t