-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','function_id','des','notice',
}

t.t = {
	['1']={'1','1001','des.1001','钻石单抽英雄卡','商店英雄'},
	['2']={'2','1002','des.1002','钻石十抽英雄卡','商店英雄'},
	['3']={'3','1003','des.1003','荣誉单抽英雄卡','商店英雄'},
	['4']={'4','1014','des.1014','钻石购买体力值',nil},
	['5']={'5','1004','des.1004','钻石购买世界树果实',nil},
	['6']={'6','1005','des.1005','钻石购买竞技场次数',nil},
	['7']={'7','1006','des.1006','钻石购买金币',nil},
	['8']={'8','1007','des.1007','钻石购买黑市道具',nil},
	['9']={'9','1008','des.1008','钻石购买阵型培养点数',nil},
	['10']={'10','1009','des.1009','钻石世界boss鼓舞',nil},
	['11']={'11','1010','des.1010','钻石翻牌提示',nil},
	['12']={'12','1011','des.1011','钻石重置远征次数',nil},
	['13']={'13','1012','des.1012','钻石购买装备格子',nil},
	['14']={'14','1013','des.1013','钻石购买英雄格子',nil},
	['15']={'15','1015','des.1015','金币单抽物品','商店物品'},
	['16']={'16','1016','des.1016','荣誉单抽物品','商店物品'},
	['17']={'17','1017','des.1017','金币十抽物品','商店物品'},
	['18']={'18','1018','des.1018','世界Boss复活',nil},
	['19']={'19','1019','des.1019','公会捐献',nil},
	['21']={'21','1021','des.1021','装备分解',nil},
	['22']={'22','1022','des.1022','探险刷新消费',nil},
	['23']={'23','1023','des.1023','大转盘消费提示',nil}
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
