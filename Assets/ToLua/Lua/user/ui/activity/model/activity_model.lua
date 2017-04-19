local t = {}
local name = 'activity_model'

local activity_info = require('ui/activity/model/activity_info')
--fix bug  策划说每个服务器的活动表不一样，所以要根据 event+服务器id 组合读取event表，请不要再进入登陆成功之前加载event表
--local event_data = gamemanager.GetData('event_data')

local seven_hilarity_info = require('ui/activity/model/seven_hilarity_info')
local seven_hilarity_data = gamemanager.GetData('seven_hilarity_data')

t.cotick = nil
t.DelegateOnTimeTicker = void_delegate.New()
t.DelegateClose = void_delegate.New()

t.ActivityConfig = 
{
  firstcharge   = { name = "firstcharge",   type = 30,   path = '',                                                          prefabPath ="" },
  levelup       = { name = "levelup",       type = 80,   path = 'ui/activity/view/levelup/activity_levelup_view',            prefabPath ="ui/activity/activitylvup" },
  vip           = { name = "vip",           type = 70,   path = 'ui/activity/view/vip/activity_vip_view',                    prefabPath ="ui/activity/activityvip" },
  present       = { name = "present",       type = 100,  path = 'ui/activity/view/present/activity_present_view',            prefabPath ="ui/activity/activitypresent" },
  everyday      = { name = "everyday",      type = 50,   path = 'ui/activity/view/everyday/activity_everyday_view',          prefabPath ="ui/activity/activityeveryday" },
  continue      = { name = "continue",      type = 40,   path = 'ui/activity/view/continue/activity_continue_view',          prefabPath ="ui/activity/activitycontinuelogin" },
  accuCharge    = { name = "accuCharge",    type = 10,   path = 'ui/activity/view/accumulatecharge/activity_charge_view',    prefabPath ="ui/activity/activitycharge" },
  accuConsume   = { name = "accuConsume",   type = 20,   path = 'ui/activity/view/accumulateconsume/activity_consume_view',  prefabPath ="ui/activity/activityconsume" },
  returngift    = { name = "returngift",    type = 60,   path = 'ui/activity/view/returngift/activity_returngift_view',      prefabPath ="ui/activity/returngift" },
  betaactivity  = { name = "betaactivity",  type = 110,  path = 'ui/activity/view/betaactivity/activity_betaactivity_view',  prefabPath ="ui/activity/betaactivity" },
  fundlevelgift = { name = "fundlevelgift", type = 140,  path = 'ui/activity/view/fundlevel/activity_fundlevel_view',        prefabPath ="ui/activity/activityfundLevelgift" },
  sevenhalirity = { name = "sevenhalirity", type = 160,  path = '',                                                          prefabPath ="" },
}


t.activityList = {} --所有活动的数据

local function Start( ... )
	gamemanager.RegisterModel(name, t)  
end

function t:OpenTimer()
  self.cotick = coroutine.start(t.OnTimerTick)
end

function t.OnTimerTick()
  while true do
    t.DelegateOnTimeTicker:Invoke()
    red_point_model = gamemanager.GetModel('red_point_model')
    if nil ~= red_point_model then red_point_model.RefreshSpecific(RedPointType.RedPoint_Activity) end
    coroutine.wait(1)
	end
end

function t:CloseTimer()
  coroutine.stop(self.cotick)
end

--解析活动数据
function t:ParseActivityList(pb)  
  local list = {}
	for i = 1, #pb.list do table.insert(list, activity_info:New(pb.list[i])) end  
  self.activityList = list
  
  t:OpenTimer()
end

--解析活动状态更新
function t:ParseUpdateResp(pb)
  self:AddActivity(pb.addActivity)
  self:RemoveActivity(pb.delActivity)
  self:UpdataActivity(pb.updateActivitys)
end

--增加活动
function t:AddActivity(info)
  if nil == info then do return end end
  if info.type == 0 then do return end end
  
  table.insert(self.activityList, activity_info:New(info))
end

--移除活动
function t:RemoveActivity(type)
  if nil == type then do return end end
  if type == 0 then do return end end
  
  local activity, index = self:GetActivityByType(type)
  if nil == activity then do return end end
  
  table.remove(activity, index)
end

--更新活动
function t:UpdataActivity(list)
  if nil == list then do return end end
  for i = 1, #list do 
    local bNewActyvity = activity_info:New(list[i])  
    local Oldactivity = self:GetActivityByType(bNewActyvity.type)
    
    if nil ~= Oldactivity then Oldactivity.Update(bNewActyvity) end
  end
end

--对活动进行排序
function t:SortActiveList(list)
  local bList = {}  
  local sort = string.split(require('global').GetItem('event_sort').value,';')
	for i = 1, #sort do
		for j = 1, #list do
			if Mathf.Floor(list[j].type/10) * 10 == tonumber(sort[i]) then table.insert(bList, list[j]) end
		end
	end
  return bList
end

--根据活动大类型获取活动数据
--返回 ActivityInfoProto ，index
function t:GetActivityByType(type)
  for index = 1, #self.activityList do
    if self.activityList[index].type == type then return self.activityList[index], index end
  end
end

--根据索引获取活动数据,排除首充活动
function t:GetActivityExcludeFirstChargeByIndex(index) 
  local list = self:GetActivityListExcludeFirstCharge()
  if nil ~= list[index] then return list[index] end
end

--获取活动列表,排除首充活动
function t:GetActivityListExcludeFirstCharge()
  local tab = {}
  for index = 1, #self.activityList do
    if self.activityList[index].type ~= self.ActivityConfig["firstcharge"]["type"] and 
    self.activityList[index].type ~= self.ActivityConfig["sevenhalirity"]["type"] then table.insert(tab, self.activityList[index]) end
  end  
  return t:SortActiveList(tab)
end

--获取开始的所有活动排在首位的活动
function t:GetFirstActivity()
  local list = self:GetActivityListExcludeFirstCharge()
  for index = 1, #list do
    if list[index].type ~= self.ActivityConfig["firstcharge"]["type"] and 
    list[index].type ~= self.ActivityConfig["sevenhalirity"]["type"] then return list[index] end
  end
end

--获取活动的配置信息
function t:GetActivityConfigByType(btype)
  for k, v in pairs(self.ActivityConfig) do
    if v["type"] == Mathf.Floor(btype/10) * 10 then return v end
  end
end

--获取活动的配置信息
function t:GetActivityConfigByName(bname)
  for k, v in pairs(self.ActivityConfig) do
    if v["name"] == bname then return v end
  end
end

--获取首充Type值
function t:GetFirstChargeType()
  local activityConfigData = self:GetActivityConfigByName("firstcharge")
  if nil == activityConfigData then do return 0 end end
  
  return activityConfigData["type"]
end

--获取七日狂欢Type值
function t:GetSevenHalirityType()
  local activityConfigData = self:GetActivityConfigByName("sevenhalirity")
  if nil == activityConfigData then do return 0 end end
  
  return activityConfigData["type"]
end

--返回累积登录的时间格式（增加一天后的时间）
function t.GetOneDayAdd(timeStr, addday)
  local atime = string.sub(timeStr, 1, 10)
  local btime = string.split(atime, '/')
  local tab = {}
  tab.year = tonumber(btime[1])
  tab.month = tonumber(btime[2])
  tab.day = tonumber(btime[3])
  tab.hour = 0
  tab.min = 0
  tab.sec = 0
  tab.isdst = false
  
  local ctime = os.time(tab)
  ctime = ctime + 86400 * addday  
  local addTab = os.date("*t", ctime)  
  return string.format(LocalizationController.instance:Get("event_1005_days"), addTab.year, addTab.month, addTab.day)
end

--判断是否参加某一个活动
function t:IsActivity(btype)
  local activityData = self:GetActivityByType(btype)
  if nil == activityData then do return false end end
  
  return true
end

--判断商店打折活动是否开启
function t:IsShopDiscountOpen()
  local isOpen = false
  
  for index = 1, #self.activityList do
    if self.activityList[index].type == 130 then isOpen = true end
  end  
  return isOpen
end

--判断活动是否有未领取的奖励
function t:IsActivityCanGetReward()
  local bIsCanAward = false
  for index = 1, #self.activityList do
    if self.activityList[index].type ~= self.ActivityConfig["firstcharge"]["type"] and 
    self.activityList[index].type ~= self.ActivityConfig["sevenhalirity"]["type"] then
      if self:IsActivityCanAwardByType(self.activityList[index].type) then bIsCanAward = true break end
    end
  end
  return bIsCanAward
end

--根据活动的类型判断该类型是否有未领取的奖励
function t:IsActivityCanAwardByType(type)  
  if type == self.ActivityConfig.vip['type'] then do return self:IsVipDailyGiftCanGetAward() end end
  if type == self.ActivityConfig.present['type'] then do return self:IsPresentGiftCanGetAward(type) end end
  
  local bActivity = t:GetActivityByType(type)  
  if nil == bActivity then do return false end end
  
  return bActivity:IsCanAward() 
end

--判断免费体力是否可以领取（该方法要放在时间迭代器中调用）
function t:IsPresentGiftCanGetAward(btype)
  local isCanAward = false 
  local activityData = self:GetActivityByType(btype)
  if nil == activityData then do return end end
  
  for k, v in pairs(activityData.scheduleStus.data) do
    local itemData = gamemanager.GetData('event_data').GetDataBySmallId(activityData.type, k)
    if nil == itemData then do return end end
    
    local IsPresentInTimeofDay = self.IsPresentInTimeofDay(tonumber(itemData.event_param))    
    if IsPresentInTimeofDay then if v == 1 or v == 3 then  isCanAward = true end end
  end  
  return isCanAward
end

--判断VIP日常奖励是否可以领取
function t:IsVipDailyGiftCanGetAward()
  return not gamemanager.GetModel('vip_model').IsVipDailyGiftGetted()
end

--根据活动的子类型判断该活动子类型是否有未领取的奖励
function t:IsActivityCanAwardBySubType(type, subType)
  local bActivity = t:GetActivityByType(type)  
  if nil == bActivity then do return end end
  
  return bActivity:IsCanAwardBySubType(subType)
end

--判断免费领取体力活动是否在一天中的某一个时间段
function t.IsPresentInTimeofDay(Hourfrom)
  local IsInTimeofDay = false  
  local FromTime = Hourfrom * 60 * 60  
  local serverDateTime = TimeController.instance.ServerTime.TimeOfDay.TotalSeconds
  local dif = serverDateTime - FromTime
  --3秒保护期
  IsInTimeofDay = dif >= 3 and dif < tonumber(gamemanager.GetData('global_data').daily_key_time_keep) - 3
  return IsInTimeofDay
end

--判断是否已经参加某一个活动中的子类型活动
function t:IsActivityjoin(btype)
  local activityData = self:GetActivityByType(btype)
  if nil == activityData then do return false end end
  
  local tevent_data = gamemanager.GetData('event_data').GetTypeDatasByType(btype)
  if nil == tevent_data then do return false end end
  
  local IsJoinOne = false
  print("activityData.subType", activityData.subType)
  for i = 1, #tevent_data do if activityData.subType == tonumber(tevent_data[i].event_small_type) then IsJoinOne = true end end
  
  return IsJoinOne
end

--判断某一个子类型活动是否激活
function t:IsActivityjoinBySubType(btype, subType)
  local activityData = self:GetActivityByType(btype)
  if nil == activityData then do return false end end
  
  return subType == activityData.subType
end

---------------------------------------------------------七日狂欢--------------------------------------------
t.SevenHilarityDay = -1                                --开服天数
t.SevenHilarityCompleteNums = 0                       --完成活动数量
t.SevenHilarityBuyedGoodsList = {}                    --已经购买的商品
t.SevenHilarityObtainedTaskList = {}                  --已经接受的任务
t.SevenHilarityCompleteTaskList = {}                  --已经接受的任务
t.SevenHilarityCompleteBoxList = {}                   --已经领取的宝箱奖励

--解析七日活动数据
function t:ParseSevenHilarityInfo(pb)
 self:UpdateDays(pb.days)
 self:UpdateCompleteNums(pb.completeTimes)
 self:UpdateBuyedGoodsList(pb.alreadyBuys)
 self:UpdateObtainedTaskList(pb.hasTask)
 self:UpdateCompleteTaskList(pb.completeTask)
 self:UpdateCompleteBoxList(pb.alreadyGetComplete)
end

function t:UpdateDays(bday)
  self.SevenHilarityDay = bday  
end

function t:UpdateCompleteNums(bnum)
  self.SevenHilarityCompleteNums = bnum
end

function t:UpdateBuyedGoodsList(blist)
  self.SevenHilarityBuyedGoodsList = {}
	for i = 1, #blist do table.insert(self.SevenHilarityBuyedGoodsList, blist[i]) end
end

function t:UpdateObtainedTaskList(blist)
  self.SevenHilarityObtainedTaskList = {}
	for i = 1, #blist do   
    table.insert(self.SevenHilarityObtainedTaskList, seven_hilarity_info:New(blist[i]))
    end
end

function t:UpdateCompleteTaskList(blist)
  self.SevenHilarityCompleteTaskList = {}
	for i = 1, #blist do
    table.insert(self.SevenHilarityCompleteTaskList, seven_hilarity_info:New(blist[i])) end
end

function t:UpdateCompleteBoxList(blist)
  self.SevenHilarityCompleteBoxList = {}
	for i = 1, #blist do table.insert(self.SevenHilarityCompleteBoxList, blist[i]) end
end

function t:GetObtainedTaskListById(taskId)
   for i = 1, #self.SevenHilarityObtainedTaskList do
    if taskId == self.SevenHilarityObtainedTaskList[i].id then return self.SevenHilarityObtainedTaskList[i] end
  end
  
  return nil
end

function t:GetCompleteTaskListById(taskId)
  for i = 1, #self.SevenHilarityCompleteTaskList do
    if taskId == self.SevenHilarityCompleteTaskList[i].id then return self.SevenHilarityCompleteTaskList[i] end
  end
  
  return nil
end

function t:GetSevenHilarityDataByDayAndType(bday, btype)
    local blist = {}    
    local totlaList = seven_hilarity_data.GetDataByDayAndType(bday, btype)

    local flage = 0
    for i = 1, #totlaList do   
      flage = 0 
      
      local obtainTask = self:GetObtainedTaskListById(tonumber(totlaList[i].task))
      if nil ~= obtainTask then 
        table.insert(blist, obtainTask)
        flage = 1
      end
        
      local completeTask = self:GetCompleteTaskListById(tonumber(totlaList[i].task))
      if nil ~= completeTask then 
        table.insert(blist, completeTask)
        flage = 2
      end
      
      if flage < 1 then
        local shdata = seven_hilarity_info:NewData(totlaList[i])
        table.insert(blist, shdata)
      end
    end
    table.sort(blist, function(A, B) return self:SortSevenHilarity(A, B) end)
    return blist
end

function t:SortSevenHilarity(DataA, DataB)
  if nil == DataA or nil == DataA then return false end
  
  local A, B = DataA.id, DataB.id
  
  if DataA.completed and DataA.InActivity and  not DataA.getReward then  A = A + 10000 end
  if DataB.completed and DataB.InActivity and  not DataB.getReward then  B = B + 10000 end 
  
  if not DataA.completed and DataA.InActivity and  not DataA.getReward then  A = A + 100000 end
  if not DataB.completed and DataB.InActivity and  not DataB.getReward then  B = B + 100000 end  
  
  if DataA.getReward and DataA.completed and DataA.InActivity then  A = A + 1000000 end
  if DataB.getReward and DataB.completed and DataB.InActivity then  B = B + 1000000 end  
  
  if not DataA.InActivity  then  A = A + 10000000 end
  if not DataB.InActivity  then  B = B + 10000000 end  
  
  return A < B
end

function t:IsBuyGoods(id)
  for i = 1, #self.SevenHilarityBuyedGoodsList do
    if self.SevenHilarityBuyedGoodsList[i] == id then do return true end end
  end
  
  return false
end

function t:IsGetBoxReward(box)
  for i = 1, #self.SevenHilarityCompleteBoxList do
    if self.SevenHilarityCompleteBoxList[i] == box then do return true end end
  end
  
  return false
end

function t:IsHaveHilarityCanAward()
  for day = 1, self.SevenHilarityDay do
    if self:IsHaveHilarityCanAwardByDay(day) then do return true end end
  end
  return false
end

function t:IsHaveHilarityCanAwardByDay(bday)
  return self:IsSevenHilarityCanAwardByDayAndType(bday, 1) or 
  self:IsSevenHilarityCanAwardByDayAndType(bday, 2) or
  self:IsSevenHilarityCanAwardByDayAndType(bday, 3) or 
  self:IsSevenHilarityCanAwardByDayAndType(bday, 4)
end

function t:IsSevenHilarityCanAwardByDayAndType(bday, btype)
  local isHave = false
  
  if btype < 4 then 
    local list = self:GetSevenHilarityDataByDayAndType(bday, btype)  
    for index = 1, #list do
      local data = list[index]
      if data.InActivity and data.completed and not data.getReward then isHave = true end
    end  
  end
  
  return isHave
end

function t:IsSevenHilarityOverTime()
  if self.SevenHilarityDay > 7 then do return true end end 
  if self.SevenHilarityDay < 1 then do return true end end
  return false
end
----------------------------------------------------------end------------------------------------------------

Start()
return t
