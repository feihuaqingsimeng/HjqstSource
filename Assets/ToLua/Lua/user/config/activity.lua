-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','name','des','pic','type','day','reward','item_cost','lv_limit','level1','count','open','miaoshu'
}

t.t = {
	['1005']={'1005','sheet_activity.name.5','sheet_activity.description.5','card_07','5','1;2;3;4;5;6;7',nil,nil,'1','40501;40502;40503;40504;40505;40506;40507;40508;40509;40510;40511;40512','2','0','物防强化副本'},
	['1006']={'1006','sheet_activity.name.6','sheet_activity.description.6','card_03','5','1;2;3;4;5;6;7',nil,nil,'1','40601;40602;40603;40604;40605;40606;40607;40608;40609;40610;40611;40612','2','0','魔防强化副本'},
	['1007']={'1007','sheet_activity.name.7','sheet_activity.description.7','card_08','6','1;2;3;4;5;6;7',nil,nil,'1','40701;40702;40703;40704;40705;40706;40707;40708;40709;40710;40711;40712','2','1','限时伤害副本'},
	['1008']={'1008','sheet_activity.name.8','sheet_activity.description.8','card_01','8','1;2;3;4;5;6;7',nil,nil,'1','40801;40802;40803;40804;40805;40806;40807;40808;40809;40810;40811;40812','2','1','单体技能副本'},
	['1009']={'1009','sheet_activity.name.9','sheet_activity.description.9','card_10','9','1;2;3;4;5;6;7',nil,nil,'1','40901;40902;40903;40904;40905;40906;40907;40908;40909;40910;40911;40912','2','1','范围技能副本'},
	['1010']={'1010','sheet_activity.name.10','sheet_activity.description.10','card_02','10','1;2;3;4;5;6;7',nil,nil,'1','41001;41002;41003;41004;41005;41006;41007;41008;41009;41010;41011;41012','2','0','镜像副本'},
	['1011']={'1011','sheet_activity.name.11','sheet_activity.description.11','card_02','11','1;2;3;4;5;6;7',nil,nil,'1','42001;42002;42003;42004;42005;42006;42007;42008;42009;42010;42011;42012','2','1','浮空伤害副本'}
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
