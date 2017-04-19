local ranking_info = {}
ranking_info.rankingList = {}

function ranking_info.SetData(list,currRankNo,lastRankNo,combat)
  ranking_info.rankingList = list
  ranking_info.currRankNo = currRankNo
  ranking_info.lastRankNo = lastRankNo
  ranking_info.combat = combat
end

return ranking_info