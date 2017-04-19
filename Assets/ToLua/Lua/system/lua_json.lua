local t = {}

function t.TableToJsonStr(table)
  local str = '{'
  for k,v in pairs(table) do
    str = str .. string.format('"%s":"%s",',k,v)
  end
  str = string.sub(str,0,-2)
  str = str .. '}'
  return str
end
return t