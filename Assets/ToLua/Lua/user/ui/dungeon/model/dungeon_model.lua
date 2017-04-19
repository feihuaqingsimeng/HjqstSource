local t = {}
local name = 'dungeon_model'

local dungeon_info = require('ui/dungeon/model/dungeon_info')

--[ Delegates ]--
t.onDungeonInfoUpdateDelegate = void_delegate.New()
t.onPveFightOverDelegate = void_delegate.New()
--[ Delegates ]--

t.lastUnlockEasyDungeonID = 0
t.lastUnlcokNormalDungeonID = 0
t.lastUnlockHardDungeonID = 0

t.allDungeonInfoList = {}
t.easyDungeonInfoList = {}
t.normalDungeonInfoList = {}
t.hardDungeonInfoList = {}

t.bossSpeciesDungeonInfoList = {}

t.pveSelectDungeonType = DungeonType.Invalid

function t.SetPveSelectDungeonType(dungeonType)

  t.pveSelectDungeonType = dungeonType
end

local function Start ()
  gamemanager.RegisterModel(name, t)
  local dungeon_data = gamemanager.GetData('dungeon_data')
  
  local allDungeonDataList = dungeon_data.GetAllDungeonData()
  for k, v in pairs(allDungeonDataList) do
    t.allDungeonInfoList[k] = dungeon_info.New(k)
    
    if v.type == DungeonType.Easy then
      table.insert(t.easyDungeonInfoList, t.allDungeonInfoList[k])
    elseif v.type == DungeonType.Normal then
      table.insert(t.normalDungeonInfoList, t.allDungeonInfoList[k])
    elseif v.type == DungeonType.Hard then
      table.insert(t.hardDungeonInfoList, t.allDungeonInfoList[k])
    elseif v.type == DungeonType.BossSpecies then
      table.insert(t.bossSpeciesDungeonInfoList, t.allDungeonInfoList[k])
    end
  end
  table.sort(t.easyDungeonInfoList, t.CompareHeroInfoByDungeonID)
  table.sort(t.normalDungeonInfoList, t.CompareHeroInfoByDungeonID)
  table.sort(t.hardDungeonInfoList, t.CompareHeroInfoByDungeonID)
  table.sort(t.bossSpeciesDungeonInfoList, t.CompareHeroInfoByDungeonID)
end

function t.GetDungeonInfoListByDungeonType (dungeonType)
  if dungeonType == DungeonType.Easy then
    return t.easyDungeonInfoList
  elseif dungeonType == DungeonType.Normal then
    return t.normalDungeonInfoList
  elseif dungeonType == DungeonType.Hard then
    return t.hardDungeonInfoList
  elseif dungeonType == DungeonType.BossSpecies then
    return t.bossSpeciesDungeonInfoList
  end
  return nil
end

function t.IsDungeonPassed(dungeonId)
  local dungeonInfo = t.GetDungeonInfo (dungeonId)
  if dungeonInfo == nil or dungeonInfo.star > 0 then
    return true
  end
  return false
end

function t.GetDungeonInfo (dungeonID)
  local info = t.allDungeonInfoList[dungeonID]
  if info == nil then
--    Debugger.Log("dungeonInfo is can not find ,  id:"..dungeonID)
  end
  return info
end

function t.GetAllDungeonInfoList ()
  return t.allDungeonInfoList
end

function t.GetDungeonInfoList (dungeonIDList)
  local dungeonInfoList = {}
  for k, v in pairs(dungeonIDList) do
    dungeonInfoList[k] = t.allDungeonInfoList[k]
  end
  return dungeonInfoList
end

function t.GetLastUnlockDungeonID (dungeonType)
  local lastUnlockDungeonID = -1
  if dungeonType == DungeonType.Easy then
    lastUnlockDungeonID = t.lastUnlockEasyDungeonID
  elseif dungeonType == DungeonType.Normal then
    lastUnlockDungeonID = t.lastUnlcokNormalDungeonID
  elseif dungeonType == DungeonType.Hard then
    lastUnlockDungeonID = t.lastUnlockHardDungeonID
  end
  return lastUnlockDungeonID
end

function t.GetTotalStarCountOfChapterOfDungeonType (dungeonType, chapterID)
  local totalStarCount = 0
  local chapter_data = gamemanager.GetData('chapter_data')
  local dungeonIDList = chapter_data.GetDataById(chapterID):GetDungeonIDListOfDungeonType(dungeonType)
  totalStarCount = (#dungeonIDList) * 3
  return totalStarCount
end

function t.GetPlayerGainStarCountOfChapterOfDungeonType (dungeonType, chapterID)
  local playerGainStarCount = 0
  local chapter_data = gamemanager.GetData('chapter_data')
  local dungeonIDList = chapter_data.GetDataById(chapterID):GetDungeonIDListOfDungeonType(dungeonType)
  for k, v in ipairs(dungeonIDList) do
    playerGainStarCount = playerGainStarCount + t.GetDungeonInfo(v).star
  end
  return playerGainStarCount
end

function t.GetTotalStarCountOfDungeonType (dungeonType)
  local totalStarCount = 0
  local dungeonInfoList = nil
  
  if dungeonType == DungeonType.Easy then
    dungeonInfoList = t.easyDungeonInfoList
  elseif dungeonType == DungeonType.Normal then
    dungeonInfoList = t.normalDungeonInfoList
  elseif dungeonType == DungeonType.Hard then
    dungeonInfoList = t.hardDungeonInfoList
  end
  
  if dungeonInfoList ~= nil then
    for k, v in pairs(dungeonInfoList) do
      totalStarCount = totalStarCount + v.star
    end
  end
  return totalStarCount
end

function t.GetMaxStarCountOfDungeonType (dungeonType)
  local maxStarCount = 0
  local dungeonInfoList = nil
  
  if dungeonType == DungeonType.Easy then
    dungeonInfoList = t.easyDungeonInfoList
  elseif dungeonType == DungeonType.Normal then
    dungeonInfoList = t.normalDungeonInfoList
  elseif dungeonType == DungeonType.Hard then
    dungeonInfoList = t.hardDungeonInfoList
  end
  
  if dungeonInfoList ~= nil then
    maxStarCount = maxStarCount + table.count(dungeonInfoList) * 3
  end
  return maxStarCount
end

function t.GetAllDungeonTotalStarCount ()
  return t.GetTotalStarCountOfDungeonType (DungeonType.Easy) + t.GetTotalStarCountOfDungeonType (DungeonType.Normal) + t.GetTotalStarCountOfDungeonType (DungeonType.Hard)
end

function t.GetPreDungeonInfo (dungeonInfo)
  return t.allDungeonInfoList[dungeonInfo.dungeonData.unlock_dungeon_id_pre1]
end

function t.UpdateDungeonInfos (pveInfoResp)
  --t.lastUnlockEasyDungeonID = pveInfoResp.lastUnLockEasyDungeonId
  --t.lastUnlcokNormalDungeonID = pveInfoResp.lastUnLockNormalDungeonId
  --t.lastUnlockHardDungeonID = pveInfoResp.lastUnLockHardDungeonId
  
  print('========================== [STAR CHECK START] ================================')
  print('pveInfoResp.dungeonStars count'..#pveInfoResp.dungeonStars)
  for k, v in ipairs(pveInfoResp.dungeonStars) do
    if v.star ~= nil then
     -- print('v.dungeonId='..v.dungeonId..'->v.star='..v.star)
      t.allDungeonInfoList[v.dungeonId].star = v.star
    end
    
    if v.star > 0 then
      local dungeonInfo = t.GetDungeonInfo(v.dungeonId)
      dungeonInfo.isLock = false
      
      if dungeonInfo.dungeonData.type == DungeonType.Easy
        and dungeonInfo.dungeonData.id > t.lastUnlockEasyDungeonID then
        t.lastUnlockEasyDungeonID = dungeonInfo.dungeonData.id
      elseif dungeonInfo.dungeonData.type == DungeonType.Normal
        and dungeonInfo.dungeonData.id > t.lastUnlcokNormalDungeonID then
        t.lastUnlcokNormalDungeonID = dungeonInfo.dungeonData.id
      elseif dungeonInfo.dungeonData.type == DungeonType.Hard
        and dungeonInfo.dungeonData.id > t.lastUnlockHardDungeonID then
        t.lastUnlockHardDungeonID = dungeonInfo.dungeonData.id
      end
    end
  end
  print('========================== [STAR CHECK END] ================================')
  
  print('========================== [UNLCOK CHECK START] ================================')
  print('pveInfoResp.lastUnLockDungeons count = '..#pveInfoResp.lastUnLockDungeons)
  for k, v in ipairs(pveInfoResp.lastUnLockDungeons) do
    print('unlocked dungeon id = '..v)
    
    local dungeonInfo = t.GetDungeonInfo(v)
    dungeonInfo.isLock = false
    
    if dungeonInfo.dungeonData.type == DungeonType.Easy
      and dungeonInfo.dungeonData.id > t.lastUnlockEasyDungeonID then
      t.lastUnlockEasyDungeonID = dungeonInfo.dungeonData.id
    elseif dungeonInfo.dungeonData.type == DungeonType.Normal
      and dungeonInfo.dungeonData.id > t.lastUnlcokNormalDungeonID then
      t.lastUnlcokNormalDungeonID = dungeonInfo.dungeonData.id
    elseif dungeonInfo.dungeonData.type == DungeonType.Hard
      and dungeonInfo.dungeonData.id > t.lastUnlockHardDungeonID then
      t.lastUnlockHardDungeonID = dungeonInfo.dungeonData.id
    end
  end
  print('========================== [UNLCOK CHECK END] ================================')
  
  for k, v in pairs(t.allDungeonInfoList) do
    if v.star > 0 then
      v.isLock = false
    end
    v.todayChallengedTimes = 0
  end
  
  for k, v in ipairs(pveInfoResp.challengeTimes) do
    t.GetDungeonInfo(v.value1).todayChallengedTimes = v.value2
    print('更新挑战次数',v.value1,v.value2)
  end
  
  for k, v in ipairs(pveInfoResp.dayRefreshTimes) do
    t.GetDungeonInfo(v.value1).dayRefreshTimes = v.value2
  end
  
  if t.onDungeonInfoUpdateDelegate ~= nil then
    t.onDungeonInfoUpdateDelegate:Invoke()
  end
  
  Observers.Facade.Instance:SendNotification('OnUpdateDungeonInfos')
end

function t.OnPveFightOver ()
  if t.onPveFightOverDelegate ~= nil then
    t.onPveFightOverDelegate:Invoke()
  end
end

function t.CompareHeroInfoByDungeonID (aDungeonInfo, bDungeonInfo)
  return aDungeonInfo.dungeonData.id < bDungeonInfo.dungeonData.id
end

Start ()
return t