-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','type','heroid','audio'
}

t.t = {
	['1']={'1','2','0','daily_21;daily_22;daily_23;daily_24'},
	['2']={'2','3','0','daily_31;daily_32;daily_33;daily_34'},
	['3']={'3','4','0','daily_41;daily_42;daily_43;daily_44'},
	['4']={'4','1','131','daily_1311'},
	['5']={'5','1','133','daily_1331'},
	['6']={'6','1','128','daily_1281'},
	['7']={'7','1','105','daily_1051'},
	['8']={'8','1','138','daily_1381'},
	['9']={'9','1','134','daily_1341'},
	['10']={'10','1','49','daily_491'},
	['11']={'11','1','121','daily_1211'},
	['12']={'12','1','200','daily_2001'},
	['13']={'13','1','201','daily_2001'},
	['14']={'14','1','202','daily_2001'},
	['15']={'15','1','11','daily_111'},
	['16']={'16','1','14','daily_141'},
	['17']={'17','1','18','daily_181'},
	['18']={'18','1','23','daily_231'},
	['19']={'19','1','27','daily_271'},
	['20']={'20','1','32','daily_321'},
	['21']={'21','1','35','daily_351'},
	['22']={'22','1','37','daily_371'},
	['23']={'23','1','42','daily_421'},
	['24']={'24','1','101','daily_1011'},
	['25']={'25','1','102','daily_1021'},
	['26']={'26','1','103','daily_1031'},
	['27']={'27','1','104','daily_1041'},
	['28']={'28','1','107','daily_1071'},
	['29']={'29','1','108','daily_1081'},
	['30']={'30','1','109','daily_1091'},
	['31']={'31','1','110','daily_1101'},
	['32']={'32','1','112','daily_1121'},
	['33']={'33','1','115','daily_1151'},
	['34']={'34','1','117','daily_1171'},
	['35']={'35','1','119','daily_1191'},
	['36']={'36','1','120','daily_1201'},
	['37']={'37','1','122','daily_1221'},
	['38']={'38','1','124','daily_1241'},
	['39']={'39','1','125','daily_1251'},
	['40']={'40','1','129','daily_1291'},
	['41']={'41','1','130','daily_1301'},
	['42']={'42','1','137','daily_1371'},
	['43']={'43','1','139','daily_1391'},
	['44']={'44','1','140','daily_1401'},
	['45']={'45','1','141','daily_1411'},
	['46']={'46','1','146','daily_1461'},
	['47']={'47','1','147','daily_1471'},
	['48']={'48','1','148','daily_1481'},
	['49']={'49','1','149','daily_1491'},
	['50']={'50','1','150','daily_1501'},
	['51']={'51','1','151','daily_1511'},
	['52']={'52','1','152','daily_1521'},
	['53']={'53','1','203','daily_2031'}
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