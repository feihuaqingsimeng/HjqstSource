local t = {}
local name = 'vip_model'

local vipData = gamemanager.GetData('vip_data')

t.OnVIPInfoUpdateDelegate = void_delegate.New()
t.OnDrawVIPBenefitsSuccessDelegate = void_delegate.New()

t.totalRecharge = 0
t.vipLevel = 0
t.dailyGetted = false
t.vipData = nil
t.hasReceivedGiftVIPLevelList = nil

local function Start ()
  gamemanager.RegisterModel(name, t)
end

function t.UpdateVIPInfo (dailyGet, totalRecharge, hasReceivedGiftVIPLevelList)
  t.dailyGetted = dailyGet
  t.totalRecharge = totalRecharge
  t.hasReceivedGiftVIPLevelList = hasReceivedGiftVIPLevelList
  
  for k, v in pairs(vipData.GetAllVIPDataList()) do
    if t.totalRecharge >= v.exp then
      t.vipLevel = v.id
    else
      break
    end
  end
  t.vipData = vipData.GetVIPData(t.vipLevel)
  t.OnVIPInfoUpdateDelegate:Invoke()
  Observers.Facade.Instance:SendNotification('OnVIPInfoUpdate')
  Observers.Facade.Instance:SendNotification('UpdateSoftGuideByLua')
end

function t.GetVipInfo()
  local infostr = ''
  infostr=string.format('%s%s%s',infostr,t.vipLevel,';')
  if t.dailyGetted then
    infostr=string.format('%s%s%s',infostr,1,';')
  else
    infostr=string.format('%s%s%s',infostr,0,';')
  end
  infostr=string.format('%s%s%s',infostr,t.totalRecharge,';')

  local length=#t.hasReceivedGiftVIPLevelList
  for i=1,length do
      infostr=string.format('%s%s',infostr,t.hasReceivedGiftVIPLevelList[i])
      if i ~= length then infostr=infostr..':' end
  end
  return infostr
end

function t.IsVipDailyGiftGetted()
  return t.dailyGetted
end

function t.DrawVIPBenefitsSuccess (vipLevel)
  Observers.Facade.Instance:SendNotification('OnRedPointChange')
  t.OnDrawVIPBenefitsSuccessDelegate:InvokeOneParam(vipLevel)
end

function t.GetRechargeDiamondToNextVIPLevel ()
  local rechargeDiamondToNextVIPLevel = 0
  if not t.vipData:IsMaxLevel() then
    local nextVIPData = t.vipData.GetNextLevelVIPData ()
    rechargeDiamondToNextVIPLevel = (nextVIPData.exp - t.totalRecharge) * 10
  end
  return rechargeDiamondToNextVIPLevel
end

function t.HasReceivedGift (level)
  for k, v in pairs(t.hasReceivedGiftVIPLevelList) do
    if level == v then
      return true
    end
  end
  return false
end

function t.GetFundLevelGift()
  local allVIPDataList = vipData.GetAllVIPDataList()
  for k, v in pairs(allVIPDataList) do
    if v:IsCanBuyFoundLevelGift() then return v.id end
  end
  
  return 0
end

function t.HasUnreceivedGiftBefore (level)
  local allVIPDataList = vipData.GetAllVIPDataList()
  for k, v in pairs(allVIPDataList) do
    if v.id < level and v.id <= t.vipLevel then
      if (not t.HasReceivedGift(v.id)) then
        return true
      end
    end
  end
  return false
end

function t.GetFirstUnreceivedGiftLevel ()
  local allVIPDataList = vipData.GetAllVIPDataList ()
  for k, v in pairs(allVIPDataList) do
    if ((v.id <= t.vipLevel) and (not t.HasReceivedGift(v.id))) then
      return v.id
    end
  end
  return -1
end

Start ()
return t