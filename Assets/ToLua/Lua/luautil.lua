function print_warning(msg)
	print(string.format('<color=#FFC300>%s</color>',msg))
end

function print_error(msg)
	print(string.format('<color=#FF0B0B>%s</color>',msg))
end

function string.split(str,sp)
	if str==nil or str=='' or sp==nil then
		return nil
	else
		local r={}
		for match in (str..sp):gmatch('(.-)'..sp) do
			table.insert(r,match)
		end
		return r
	end
end

function string.split2(source,sp1,sp2)
	if source==nil or source=='' or sp1 == nil or sp2 == nil then
		return nil
	else
		step1=string.split(source,sp1)
		local r = {}
		for i,j in ipairs(step1) do
			step2=string.split(j,sp2)
			if step2 then
				local x = {}
				for k,l in ipairs(step2) do
					x[k]=l
				end
				r[i]=x
			end
		end
		return r
	end
end
function string.split2number(str,sp)
  
  local s = string.split(str,sp)
  if s == nil then
    print(string.format('split str : %s by sp : %s,but value is nil ',str,sp))
    assert(false)
    return
  end
  local snumber = {}
  for k,v in pairs(s) do
    snumber[k] = tonumber(v)
  end
  return snumber
end
function table.count(t)
  local count = 0
  for k, v in pairs(t) do
    count = count + 1
  end
  return count
end

function table.contain(t,value)
	if not t or #t==0 then return false end
	
	for k,v in pairs(t) do
		if v==value then
			return true
		end
	end
	return false
end

function table.removevalue(t,value)
	if not t or #t==0 then return end
	for k,v in ipairs(t) do
		if v==value then
			table.remove(t,k)
			return
		end
	end
end

function table.addtable(t,at)
	if not at or #at<1 then return end
	for k,v in ipairs(at) do
		table.insert(t,v)
	end
end