local t = {}
local name = 'turntable_model'

local turntable_data = gamemanager.GetData('turntable_data') 

local function Start()
	gamemanager.RegisterModel(name, t)  
end



--转盘排行榜
t.turntableRankDictionary = Dictionary.New('id','turntable_rank_info')
t.drawCount = 0 --转了多少次
t.freeCount = 0--免费次数

function t.InitRankByProtocol(luckyLetteInfoProtoList)
  local turntable_rank_info = require('ui/activity/model/turntable/turntable_rank_info')
  
  t.turntableRankDictionary:Clear()
  local count = #luckyLetteInfoProtoList
  for i = 1,count do
    t.turntableRankDictionary:Add(i,turntable_rank_info.NewByLuckyLetteInfoProto(luckyLetteInfoProtoList[i]))
  end
end
--免费次数
function t.GetFreeCount()
  return t.freeCount
end
--转动次数
function t.GetOneCostDiamond()
  return turntable_data.GetOneCostByDrawCount(t.drawCount+1).costResData.count
end
--十连抽
function t.GetTenCostDimaond()
    return turntable_data.GetTenCostByDrawCount(t.drawCount+1).costResData.count
end

--下次折扣
function t.GetNextNeedCountAndDiscount()
  local firstData = turntable_data.GetTenCostByDrawCount(1)
  local nextCount = t.drawCount
  if t.drawCount == 0 then
    nextCount = 1
  end
  local nextData = turntable_data.GetNextDifferentDataCostByTen(nextCount)

  if nextData == nil then
    return nil,nil
  end
  
  return nextData.startCostCount,math.floor(nextData.costResData.count/firstData.costResData.count * 10)
end

Start()
return t