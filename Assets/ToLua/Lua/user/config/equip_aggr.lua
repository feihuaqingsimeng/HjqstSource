-- AutoGenerate Do not Edit!
local t = {}
local indexs={
'star','exp_provide','off_need_type','off_need_num'
}

t.t = {
    [1]={'1','100','9','10'},
    [2]={'2','150','9','30'},
    [3]={'3','225','9','60'},
    [4]={'4','450','9','120'},
    [5]={'5','900','9','200'},
    [6]={'6','1800','9','300'}
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
