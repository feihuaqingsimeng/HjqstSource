local t={}

local function Start( ... )
  t.data={}
  local shop=dofile('gulid_shop')
  shop.ForEach(function(id,table)
    t.DealWith(id,table)
  end)
end

function t.DealWith(id,table)
  local r={}
  r.id=tonumber(table.id)
  r.type=tonumber(table.type)
  r.lv=tonumber(table.gulid_lv)
  r.item=table.item
  r.cost=string.split(table.cost,':')
  t.data[r.id]=r
end

function t.GetDataById(id)
  return t.data[id]
end

Start()
return t