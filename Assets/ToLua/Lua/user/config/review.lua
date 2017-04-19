-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'review_id','star_1','star_2','star_3','star_4','star_5','star_6','star_7','star_8','star_9','star_10'
}

t.t = {
	['1']={'1','0;1','1;0|2;240','1;0&2;240',nil,nil,nil,nil,nil,nil,nil},
	['2']={'2','4;3370','4;6750','4;10130','4;13510','4;16890','4;20270','4;23650','4;27030','4;30410','4;33790'},
	['3']={'3','4;14068','4;28133','4;42198','4;56263','4;70328','4;84393','4;98458','4;112523','4;126588','4;140653'},
	['4']={'4','4;33150','4;66299','4;99448','4;132597','4;165746','4;198895','4;232044','4;265193','4;298342','4;331491'},
	['5']={'5','4;55559','4;111114','4;166669','4;222224','4;277779','4;333334','4;388889','4;444444','4;499999','4;555554'},
	['11']={'11','0;1','0;1&1;1','0;1&1;0',nil,nil,nil,nil,nil,nil,nil},
	['12']={'12','0;1','0;1&2;90','0;1&2;60',nil,nil,nil,nil,nil,nil,nil},
	['13']={'13','0;1','0;1&3;50','0;1&3;80',nil,nil,nil,nil,nil,nil,nil},
	['14']={'14','0;1','0;1&5;4','0;1&5;8',nil,nil,nil,nil,nil,nil,nil},
	['15']={'15','0;1','4;10000','4;30000',nil,nil,nil,nil,nil,nil,nil},
	['1001']={'1001','4;351','4;696','4;1041','4;1386','4;1731','4;2076','4;2421','4;2766','4;3111','4;3456'},
	['1002']={'1002','4;4392','4;8784','4;13176','4;17568','4;21960','4;26352','4;30744','4;35136','4;39528','4;43920'},
	['1003']={'1003','4;8026','4;16043','4;24060','4;32077','4;40094','4;48111','4;56128','4;64145','4;72162','4;80179'},
	['1004']={'1004','4;11363','4;22718','4;34073','4;45428','4;56783','4;68138','4;79493','4;90848','4;102203','4;113558'},
	['1005']={'1005','4;17753','4;35502','4;53251','4;71000','4;88749','4;106498','4;124247','4;141996','4;159745','4;177494'},
	['1006']={'1006','4;22865','4;45726','4;68587','4;91448','4;114309','4;137170','4;160031','4;182892','4;205753','4;228614'},
	['1007']={'1007','4;31021','4;62035','4;93049','4;124063','4;155077','4;186091','4;217105','4;248119','4;279133','4;310147'},
	['1008']={'1008','4;37409','4;74814','4;112219','4;149624','4;187029','4;224434','4;261839','4;299244','4;336649','4;374054'},
	['1009']={'1009','4;45428','4;90854','4;136280','4;181706','4;227132','4;272558','4;317984','4;363410','4;408836','4;454262'},
	['1010']={'1010','4;54063','4;108120','4;162177','4;216234','4;270291','4;324348','4;378405','4;432462','4;486519','4;540576'},
	['1011']={'1011','4;64641','4;129273','4;193905','4;258537','4;323169','4;387801','4;452433','4;517065','4;581697','4;646329'},
	['1012']={'1012','4;74633','4;149262','4;223891','4;298520','4;373149','4;447778','4;522407','4;597036','4;671665','4;746294'}
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