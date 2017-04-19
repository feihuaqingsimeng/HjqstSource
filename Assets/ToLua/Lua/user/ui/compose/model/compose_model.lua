local t = {}
local name = 'compose_model'


t.OnSuccessUpdateDelegate = void_delegate.New()


local function Start()
  gamemanager.RegisterModel(name,t)
end
function t.GetDicCount(tab)
  local index = 0
  for k,v in pairs(tab) do
    index =index+1
  end
  return index
end
function t.GetDicToList(tab)
  local tabList= {}
  for k,v in pairs(tab) do
    table.insert(tabList,v)
  end
  return tabList
end
function t.EquipCompare(a,b)
   if a.data.quality < b.data.quality then
      do return true end
   elseif a.data.quality > b.data.quality then
      do return false end
   else
      return a.id> b.id
   end
end
function t.GetNewSortList(quality,equipList)
  local curEquipInfoList ={}
 -- print("quality:",quality)
  for index= #equipList,1,-1 do   
      if equipList[index].data.quality == quality then
         -- print("index:",index,"equipList[index]:",equipList[index])
          local data = table.remove(equipList,index)
          table.insert(curEquipInfoList,data)
      end
  end
  --print("#curEquipInfoList:",#curEquipInfoList)
  table.sort(curEquipInfoList,t.EquipCompare)
  for index =1,#equipList do
     table.insert(curEquipInfoList,equipList[index])
  end
  return curEquipInfoList
end
function t.GetQualityCount(quality,equipList)
  local num =0
  for index=1,# equipList do
      if equipList[index].data.quality == quality then
        num =num+1
        if num >=5 then
           do return num end
        end
      end
  end
  return num
end
function t.GetQualityEquipInfo(quality,equipList)
  local curEquipInfo =0
  for index=1,# equipList do
      if equipList[index].data.quality == quality then
        table.insert(curEquipInfo,equipList[index])
      end
  end
  return curEquipInfo
end
function t.UpdateComposeSuccess(result,id)
  t.OnSuccessUpdateDelegate:InvokeTwoParam(result,id)
end
Start()
return t