-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','sheet_num','buy_type','resource','name','pic','des','cost','first_award','buy_award','double_base','mounth_card','sort','ios_is_show','name1','des1'
}

t.t = {
	['100']={'100','13','5','9:0:60:0','shop_diamond.name.31','icon_gem_1','shop_diamond.des.31','100:0:6:0','9:0:60:0','9:0:0:0','1','0','3','1','60','钻石描述'},
	['101']={'101','13','5','9:0:300:0','shop_diamond.name.32','icon_gem_1','shop_diamond.des.32','100:0:30:0','9:0:300:0','9:0:30:0','1','0','4','1','300','钻石描述'},
	['102']={'102','13','5','9:0:1980:0','shop_diamond.name.34','icon_gem_2','shop_diamond.des.34','100:0:198:0','9:0:1980:0','9:0:200:0','1','0','6','1','1980','钻石描述'},
	['103']={'103','13','5','9:0:3280:0','shop_diamond.name.35','icon_gem_2','shop_diamond.des.35','100:0:328:0','9:0:3280:0','9:0:400:0','1','0','7','1','3280','钻石描述'},
	['104']={'104','13','5','9:0:6480:0','shop_diamond.name.36','icon_gem_3','shop_diamond.des.36','100:0:648:0','9:0:6480:0','9:0:700:0','1','0','8','1','6480','钻石描述'},
	['105']={'105','13','5','9:0:100:0','shop_diamond.name.25','icon_shop_yueka','shop_diamond.des.25','100:0:30:0',nil,nil,'0','1','1','1',nil,'月卡'},
	['106']={'106','13','5','9:0:300:0','shop_diamond.name.26','icon_shop_zhizunyueka','shop_diamond.des.26','100:0:60:0',nil,nil,'0','1','2','1',nil,'至尊月卡'},
	['107']={'107','13','5','9:0:980:0','shop_diamond.name.33','icon_gem_2','shop_diamond.des.33','100:0:98:0','9:0:980:0','9:0:98:0','1','0','5','1','980','钻石描述'},
	['108']={'108','13','5','9:0:9980:0','shop_diamond.name.37','icon_gem_3','shop_diamond.des.37','100:0:998:0','9:0:9980:0','9:0:998:0','1','0','9','0','9980','钻石描述'}
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
