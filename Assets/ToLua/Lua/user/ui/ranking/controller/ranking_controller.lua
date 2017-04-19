local t = {}
local name = 'ranking_controller'

require 'rankList_pb'

local function Start()
  netmanager.RegisterProtocol(MSG.RankListResp,t.GetRankListResp)
  gamemanager.RegisterCtrl(name,t)
end

function t.GetRankListResp()
  if not rankList_pb then require 'rankList_pb' end
  local rankListResp = rankList_pb.RankListResp()
  rankListResp:ParseFromString(netmanager.GetProtocolData())
  local rankingModel =  gamemanager.GetModel('ranking_model')
  print('currentNo:'..rankListResp.currRankNo..'lastNo:'..rankListResp.lastRankNo)
  rankingModel.SetRankingData(rankListResp.list,rankListResp.currRankNo,rankListResp.lastRankNo,rankListResp.combat)
end 

function t.RankListReq(rankType)
  local intproto = common_pb.IntProto()
  intproto.value = rankType
  netmanager.SendProtocol(MSG.RankListReq, intproto)
end


function t.OpenRankingView(rankType)
  local rankingView = require('ui/ranking/view/ranking_view')
  rankingView.Open(rankType)
end

Start()
return t