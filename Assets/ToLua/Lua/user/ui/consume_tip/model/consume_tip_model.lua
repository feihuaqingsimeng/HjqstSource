local t = {}
local name = 'consume_tip_model'

t.ConsumeTipTable = {} --ConsumeTipType , int 
t.isInit = false

local function Start()
  gamemanager.RegisterModel(name,t)
  
end

function t.Init()
  if  t.isInit then
    return
  end
  
  local player_model = gamemanager.GetModel('player_model')
  local game_model = gamemanager.GetModel('game_model')

  local consume_tip_data = gamemanager.GetData('consume_tip_data')
  for k,v in pairs(consume_tip_data.data) do
    t.ConsumeTipTable[k] = 1
  end
  
  local s = PlayerPrefs.GetString(string.format("consumeTip%s",game_model.accountName))
  --Debugger.LogError("lua[consumeTip] key:{0},{1}","consumeTip"..game_model.accountName,s)
  if s ~= '' then
    local consumeString = string.split2(s,';',',')
    for k,v in pairs(consumeString) do
      local consumeType = tonumber(v[1])
      if t.ConsumeTipTable[consumeType] then
        t.ConsumeTipTable[consumeType] = tonumber(v[2]) 
      end
    end
  end
  t.isInit = true
end

function t.Save()
  local index = 1
  local s = ''
  for k,v in pairs(t.ConsumeTipTable) do
    if index ~= 1 then
      s = s .. ';'
    end
    s = s .. k .. ',' .. v
    index = index + 1
  end
  local game_model = gamemanager.GetModel('game_model')
  --print('lua[consumeTip] save:',s)
  PlayerPrefs.SetString(string.format("consumeTip%s",game_model.accountName),s);
end

function t.HasConsumeTipKey(consumeTipType)
  t.Init()
  return t.ConsumeTipTable[consumeTipType] ~= nil
end

function t.GetConsumeTipEnable(consumeTipType)
  t.Init()
  if t.ConsumeTipTable[consumeTipType] then
    return t.ConsumeTipTable[consumeTipType] == 1
  end
  return false
end

function t.SetConsumeTipEnable(consumeTipType ,isTip)
  t.Init()
  local value = 0
  if isTip then
    value = 1
  end
  
  if t.ConsumeTipTable[consumeTipType] then
    t.ConsumeTipTable[consumeTipType] = value
  end
  t.Save()
end


Start()
return  t