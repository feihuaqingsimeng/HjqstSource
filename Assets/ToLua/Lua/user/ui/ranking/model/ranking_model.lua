local t = {}
local name = 'ranking_model'
t.UpdateRankingDelegate = void_delegate.New()
t.ranking_info = require 'ui/ranking/model/ranking_info'

local function Start()
  gamemanager.RegisterModel(name,t)
end

function t.SetRankingData(list,currRankNo,lastRankNo,combat)
  t.ranking_info.SetData(list,currRankNo,lastRankNo,combat)
  t.UpdateRankingDelegate:InvokeOneParam(t.ranking_info)
end

Start()
return t