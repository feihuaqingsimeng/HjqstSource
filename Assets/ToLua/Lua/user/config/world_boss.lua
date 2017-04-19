-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','rank_min','rank_max','reward_up','max_might','boss_killed','player_damage','rank_num','above_player_lv','player_number','reward','reward_show',
}

t.t = {
	['1']={'1','1','1','1000','0','50','0','0','0','0','8:0:20000:0','8:0:20000:0;4:10133:600:0','reward_up*(1+50*boss_killed/100)*(1+max_might*33/1000000)'},
	['2']={'2','2','2','980','0','50','0','0','0','0','8:0:15000:0','8:0:15000:0;4:10133:400:0','reward_up*(1+50*boss_killed/100)*(1+max_might*33/1000000)'},
	['3']={'3','3','3','960','0','50','0','0','0','0','8:0:10000:0','8:0:10000:0;4:10133:300:0','reward_up*(1+50*boss_killed/100)*(1+max_might*33/1000000)'},
	['4']={'4','4','10','900','0','50','0','0','0','0','8:0:5000:0','8:0:5000:0;4:10133:250:0','reward_up*(1+50*boss_killed/100)*(1+max_might*33/1000000)'},
	['5']={'5','11','30','800','0','50','0','0','0','0','8:0:4000:0','8:0:4000:0;4:10133:210:0','reward_up*(1+50*boss_killed/100)*(1+max_might*33/1000000)'},
	['6']={'6','31','100','600','0','50','0','0','0','0','8:0:3000:0','8:0:3000:0;4:10133:180:0','reward_up*(1+50*boss_killed/100)*(1+max_might*33/1000000)'},
	['7']={'7','101','200','500','0','50','0','0','0','0','8:0:2500:0','8:0:2500:0;4:10133:150:0','reward_up*(1+50*boss_killed/100)*(1+max_might*33/1000000)'},
	['8']={'8','201','500','400','0','50','0','0','0','0','8:0:2000:0','8:0:2000:0;4:10133:120:0','reward_up*(1+50*boss_killed/100)*(1+max_might*33/1000000)'},
	['9']={'9','501','1000','300','0','50','0','0','0','0','8:0:1500:0','8:0:1500:0;4:10133:100:0','reward_up*(1+50*boss_killed/100)*(1+max_might*33/1000000)'},
	['10']={'10','1001','2000','200','0','50','0','0','0','0','8:0:1200:0','8:0:1200:0;4:10133:70:0','reward_up*(1+50*boss_killed/100)*(1+max_might*33/1000000)'},
	['99']={'99','2001','-1','150','0','50','0','0','0','0','8:0:1000:0','8:0:1000:0;4:10133:50:0','reward_up*(1+50*boss_killed/100)*(1+max_might*33/1000000)'},
	['11']={'11','0','0','150','0','50','0','0','0','0','8:0:10000:0','8:0:10000:0;4:10133:150:0','reward_up*(1+boss_killed/100)*(1+max_might*服务器最高战力/1000000)'},
	['21']={'21','0','0','60','0','0','10','0','0','0','8:0:1000:0','8:0:1000:0;4:10133:0:0','reward_up*(1+50*boss_killed/100)*(0.4+玩家输出伤害*player_damage/1000000)'},
	['31']={'31','0','0','0','0','0','0','50','0','0',nil,nil,'reward_up*(1+50*boss_killed/100)*(1+max_might*33/1000000)'},
	['41']={'41','0','0','0','0','0','0','0','1','1',nil,nil,'reward_up*(1+50*boss_killed/100)*(1+max_might*33/1000000)'}
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
