-- AutoGenerate Do not Edit!
local t = {}
local indexs={
'aggr_lv','exp_need','gold_need_1','gold_need_2','gold_need_3','gold_need_4','gold_need_5','gold_need_6','base_attr_percent'
}

t.t = {
    [0]={'0','500','13','25','50','100','200','400','140'},
    [1]={'1','1000','19','38','75','150','300','600','180'},
    [2]={'2','2000','25','50','100','200','400','800','220'},
    [3]={'3','4000','31','63','125','250','500','1000','260'},
    [4]={'4','8000','38','75','150','300','600','1200','300'}
}

function t.ForEach(func)
	if not func then return end
	for i,j in pairs(t.t) do
		local r={}
		for k,v in ipairs(indexs) do
			if v ~= nil and v ~= '' then
				r[v]=j[k]
			end
		end
		func(i,r)
	end
end

function t.GetItem(id)
    local item=t[id]
	if item then return item end
	item=t.t[id]
	local result = {}
	for k,v in pairs(indexs) do
		if v ~= nil and v ~= '' then
			result[v]=item[k]
		end
	end
	t[id]=result
	return result
end

return t
