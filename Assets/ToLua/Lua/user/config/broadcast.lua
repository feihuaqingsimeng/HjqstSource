-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','des','repeat_times','repeat_every_time','des1'
}

t.t = {
	['1001']={'1001','mail.10104.des','5','60','世界boss开启，请大家注意时间参加'},
	['1002']={'1002','mail.10105.des','3','60','世界boss已被玩家N成功击杀，恭喜大家，'},
	['1003']={'1003','mail.10201.des','3','60','世界boss活动已经结束，本次挑战没有人成功击杀'},
	['1004']={'1004','mail.1004.des','3','60','世界boss还有N分钟就要开始了，大家快点充值准备鼓舞吧！'},
	['2001']={'2001','mail.2001.des','1','60','恭喜XXX玩家获得了X星英雄XXX，从此之后实力更进一步(只报道5星和6星）'},
	['2002']={'2002','mail.2002.des','1','60','恭喜XXX玩家将英雄XXX升到了6星，从此之后大杀四方战无不胜'},
	['2003']={'2003','mail.2003.des','1','60','恭喜<color=#ff4800>{0}</color>获得了<color=#e6cea1>橙色品质</color>装备<color=#CD43FFFF>{2}</color>，如虎添翼，战无不胜'}
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
