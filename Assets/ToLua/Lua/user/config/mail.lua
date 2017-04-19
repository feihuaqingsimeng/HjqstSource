-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','type','pic','name','des','item',
}

t.t = {
	['10001']={'10001','1','sprite/task_icon/1','mail.10001.name','mail.10001.des',nil,'50级奖励'},
	['10002']={'10002','1','sprite/task_icon/1','mail.10002.name','mail.10002.des',nil,'40级奖励'},
	['10003']={'10003','1','sprite/task_icon/1','mail.10003.name','mail.10003.des',nil,'30级奖励'},
	['10004']={'10004','1','sprite/task_icon/1','mail.10004.name','mail.10004.des',nil,'20级奖励'},
	['10005']={'10005','1','sprite/task_icon/1','mail.10005.name','mail.10005.des',nil,'10级奖励'},
	['10101']={'10101','2','sprite/task_icon/8','mail.10101.name','mail.10101.des',nil,'你在世界boss的战斗中，伤害排名第N，特此给予奖励，再接再厉'},
	['10102']={'10102','2','sprite/task_icon/8','mail.10102.name','mail.10102.des',nil,'你在世界boss的战斗中，成功的击杀了boss，特此给予奖励，再接再厉'},
	['10103']={'10103','2','sprite/task_icon/8','mail.10103.name','mail.10103.des',nil,'你在世界boss的战斗中，对Boss造成了N点伤害，特此给予奖励，再接再厉'},
	['10301']={'10301','4','sprite/task_icon/3','mail.10301.name','mail.10301.des',nil,'扫荡奖励'},
	['10401']={'10401','5','sprite/task_icon/3','mail.10401.name','mail.10401.des',nil,'老玩家回归'},
	['10501']={'10501','1','sprite/task_icon/5','mail.10501.name','mail.10501.des','8:0:10000:0;4:10600:5:0','欢迎加入游戏'},
	['10502']={'10502','1','sprite/task_icon/5','mail.10502.name','mail.10502.des','8:0:10000:0;9:0:200:0;4:10601:10:0;4:50981:20:0','内测感恩礼包'},
	['10503']={'10503','1','sprite/task_icon/5','mail.10503.name','mail.10503.des',nil,'双倍返还奖励'},
	['10601']={'10601','6','sprite/task_icon/1','mail.10601.name','mail.10601.des',nil,'竞技场每日排名奖励'},
	['10701']={'10701','7','sprite/task_icon/34','mail.10701.name','mail.10701.des','9:0:88:0;8:0:888:0','月卡'},
	['10702']={'10702','7','sprite/task_icon/35','mail.10702.name','mail.10702.des','9:0:288:0;8:0:2888:0','至尊月卡'},
	['10801']={'10801','8','sprite/task_icon/5','mail.10801.name','mail.10801.des',nil,'转盘邮件'},
	['99999']={'99999','99','sprite/task_icon/6','mail.99999.name','mail.99999.des',nil,'运营专用公告'},
	['99998']={'99998','99','sprite/task_icon/7','mail.99999.name','mail.99999.des',nil,'赔偿'},
	['99997']={'99997','99','sprite/task_icon/5','mail.99999.name','mail.99999.des',nil,'更新'},
	['99996']={'99996','99','sprite/task_icon/8','mail.99999.name','mail.99999.des',nil,'其他活动等'},
	['99995']={'99995','99','sprite/task_icon/5','mail.99999.name','mail.99999.des',nil,nil},
	['99994']={'99994','99','sprite/task_icon/5','mail.99999.name','mail.99999.des',nil,nil},
	['99993']={'99993','99','sprite/task_icon/5','mail.99999.name','mail.99999.des',nil,nil},
	['99992']={'99992','99','sprite/task_icon/5','mail.99999.name','mail.99999.des',nil,nil},
	['99991']={'99991','99','sprite/task_icon/5','mail.99999.name','mail.99999.des',nil,nil},
	['99990']={'99990','99','sprite/task_icon/5','mail.99999.name','mail.99999.des',nil,nil}
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
