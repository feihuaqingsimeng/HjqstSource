-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','type_name','type_icon','item_type'
}

t.t = {
	['1']={'1','sheet_item_type1','icon_page_gshop','25;3;2;1;20;22;21;23'},
	['2']={'2','sheet_item_type2','icon_page_piece','24;26'},
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
