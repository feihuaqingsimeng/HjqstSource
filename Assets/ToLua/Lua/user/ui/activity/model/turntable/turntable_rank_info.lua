local t = {}
t.__index = t
--转盘排行榜数据
function t.New(id,name,rank,times)
  local o = {}
  setmetatable(o,t)
  o.id = id
  o.name = name
  o.rank = rank
  o.times = times
  o.rankReward = gamemanager.GetData('turntable_data').GetRankRewardByRank(rank)
  return o
end
function t.NewByLuckyLetteInfoProto(proto)
  local o = {}
  setmetatable(o,t)
  o.id = proto.no
  o.name = proto.name
  o.rank = proto.no
  o.times = proto.times
  o.rankReward = gamemanager.GetData('turntable_data').GetRankRewardByRank(o.rank)
  return o
end
return t