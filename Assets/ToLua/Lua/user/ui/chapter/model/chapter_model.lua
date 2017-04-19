local t = {}
t.dungeonStarBoxInfoList = {}
local name = 'chapter_model'
local dungeon_star_data = gamemanager.GetData('dungeon_star_data')
local dungeon_star_box_info = require('ui/chapter/model/dungeon_star_box_info')

t.OnDungeonStarBoxInfosUpdateDelegate = void_delegate.New()

local function Start ()
  t.dungeonStarBoxInfoList = {}
  local dungeonStarDataList = dungeon_star_data.GetDungeonStarDataList()
  for k, v in pairs(dungeonStarDataList) do
    t.dungeonStarBoxInfoList[k] = dungeon_star_box_info.New(k, false)
  end
  table.sort(t.dungeonStarBoxInfoList, t.SortDungeonStarBoxInfo)
  
  gamemanager.RegisterModel(name, t)
end

function t.GetDungeonStarBoxInfoList (dungeonType)
  local dungeonStarInfoList = {}
  for k, v in pairs(t.dungeonStarBoxInfoList) do
    if v.dungeonStarData.dungeon_type == dungeonType then
      dungeonStarInfoList[k] = v
    end
  end
  return dungeonStarInfoList
end

function t.GetDungeonStarBoxInfo (dungeonStarBoxID)
  return t.dungeonStarBoxInfoList[dungeonStarBoxID]
end

function t.DungeonStarBoxHasReceived (dungeonStarBoxID)
  return t.GetDungeonStarBoxInfo(dungeonStarBoxID).hasReceived
end

function t.HasUnreceivedDungeonStarBox (dungeonType)
  local dungeonStarInfoList = t.GetDungeonStarBoxInfoList(dungeonType)
  for k, v in pairs(dungeonStarInfoList) do
    if v:CanReceive() and not v.hasReceived then
      return true
    end
  end
  return false
end

function t.UpdateDungeonStarBoxReceiveStatus (receivedDungeonStarBoxID)
  for k, v in ipairs(receivedDungeonStarBoxID) do
    if t.dungeonStarBoxInfoList[v] ~= nil then
      t.dungeonStarBoxInfoList[v].hasReceived = true
    end
  end
  t.OnDungeonStarBoxInfosUpdateDelegate:Invoke()
  Observers.Facade.Instance:SendNotification('OnDungeonStarBoxInfoUpdate')
end

function t.SortDungeonStarBoxInfo (aDungeonStarBoxInfo, bDungeonStarBoxInfo)
  return aDungeonStarBoxInfo.dungeonStarData.id < bDungeonStarBoxInfo.dungeonStarData.id
end

Start()
return t