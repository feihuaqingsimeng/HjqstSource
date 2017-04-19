local t={}
local name='exploremodel'

function t.GetExploringHeros()
	-- for i,v in ipairs(t.exploringHeros) do
	-- 	print(i,v)
	-- end
	return t.exploringHeros
end

function t.GetUnfinishExploringHeroes ()
  return t.unfinishedExploringHeroes
end

local function Start( ... )
	t.data=dofile('config/explore')
	price=string.split(require('config/global').GetItem('explore_refresh').value,':')
	t.price=price[3]
	t.priceicon=LuaInterface.LuaCsTransfer.GetIcon(tonumber(price[1]),0)
	t.ExploreTaskRewardRespListener=void_delegate.New()
	t.ExploreTaskRespListener=void_delegate.New()
	t.ExploreTaskRefreshRespListener=void_delegate.New()
	t.ExploreListRespListener=void_delegate.New()
	t.ExploreTaskSynRespListener=void_delegate.New()
	gamemanager.RegisterModel(name,t)
end

local function ParseProto(info)
	if info.id<1 then print('error:id:'..info.id) end
	if not t.data.GetItem(info.id) then print('error:check config file has no id'..info.id) end
	local r={}
	r.id=info.id
	r.status=info.status
	r.heros=info.heros
	r.overTime=math.floor(info.overTime/1000)
	r.herotype=tonumber(t.data.GetItem(info.id).hero_type)
	r.time=tonumber(t.data.GetItem(info.id).time)
	r.award=string.split(t.data.GetItem(info.id).award,';')
	r.hero_number=tonumber(t.data.GetItem(info.id).hero_number)
	if r.status ~= 0 then
    table.addtable(t.unfinishedExploringHeroes, r.heros)
  end
  
  if r.status == 1 then
    table.addtable(t.exploringHeros,r.heros)
  end
	return r
end

function t.ExploreTaskSynResp(pb)
	print('----------ExploreTaskSynResp-------------')
	for i=1,#t.taskList do
		if t.taskList[i].id==pb.id then
			t.taskList[i].status=pb.status
			break
		end
	end
	t.ExploreTaskSynRespListener:InvokeOneParam(pb)
end

function t.ExploreTaskRewardResp(lastRequestExploreRewardTaskID)
	print('----------Explore TaskRewardResp-------------')
  for i=1,#t.taskList do
		if t.taskList[i].id == lastRequestExploreRewardTaskID then
      for k, v in pairs(t.taskList[i].heros) do
        for k2, v2 in pairs(t.unfinishedExploringHeroes) do
          if v == v2 then
            table.remove(t.unfinishedExploringHeroes, k2)
          end
        end
      end
			break
		end
	end
	t.ExploreTaskRewardRespListener:Invoke()
end

function t.ExploreTaskResp( ... )
	print('----------Explore TaskResp-------------')
	t.remainTimes=t.remainTimes-1
	t.ExploreTaskRespListener:Invoke()
end
function t.ExploreTaskRespOK(data)
	for i=1,#t.taskList do
		if t.taskList[i].id == data.id then 
			local task=t.taskList[i]
			task.status=1
			task.overTime=LuaInterface.LuaCsTransfer.GetTime()+task.time

			table.addtable(t.exploringHeros,data.heros)
      table.addtable(t.unfinishedExploringHeroes,data.heros)

			table.remove(t.taskList,i)
			table.insert(t.taskList,task)	
			break
		end
	end
end

function t.ExploreTaskRefreshResp(pb)
	-- print('----------Explore TaskRefreshResp-------------:'..#pb.refreshTasks)
	-- print('----------Explore taskList-------------:'..#t.taskList)

	local  taskListTemp = {}
	for i=1,#t.taskList do
		if t.taskList[i].status > 0 then 
			table.insert(taskListTemp,t.taskList[i])			
		end
	end

	t.taskList = taskListTemp

	for i=1,#pb.refreshTasks do
		--print(pb.refreshTasks[i].id)
		table.insert(t.taskList,ParseProto(pb.refreshTasks[i]))
	end
	t.SortCustom()

	print('----------Explore ListResp-------------:'..#t.taskList)

	--t.taskList.count=#t.taskList
	t.ExploreTaskRefreshRespListener:Invoke()
end

function t.ExploreListResp(pb)
	length=#pb.exploreTasks
	print('----------Explore ListResp-------------:'..length)
	t.taskList={}
	--t.taskList.count=length
	t.exploringHeros={}
	t.exploredHeros={}
  t.unfinishedExploringHeroes = {}
	for i=1,length do
		t.taskList[i]=ParseProto(pb.exploreTasks[i])
		--table.addtable(t.exploringHeros,pb.exploreTasks[i])
	end
	-- for i=1,#pb.exploredHeros do
	-- 	print('已经探险过的英雄',pb.exploredHeros[i])
	-- end

	table.addtable(t.exploredHeros,pb.exploredHeros)

	t.SortCustom()
	t.remainTimes=pb.remainTimes
	t.maxTimes=pb.maxTimes
	t.freeRefreshIsUsed=pb.freeRefreshIsUsed
	-- gamemanager.GetCtrl('explorectrl').ExploreListRespFromModel()
	t.ExploreListRespListener:Invoke()
end

function t.SortCustom()
	local explored={}
	local exploring={}
	local notexplore={}
	local task=nil
	for i=1,#t.taskList do
		task=t.taskList[i]
		if task.status==0 then
			table.insert(notexplore,task)
		elseif task.status==1 then
			table.insert(exploring,task)
		else
			table.insert(explored,task)
		end
	end

	table.sort(explored,function (a,b)
		return a.id<b.id
	end)
	--add to list
	for i=1,#explored do
		table.insert(t.exploredHeros,explored[i].id)
	end

	table.sort(exploring,function (a,b)
		return a.overTime<b.overTime
	end)
	table.sort(notexplore,function (a,b)
		return a.id<b.id
	end)

	t.taskList={}
	table.addtable(t.taskList,explored)
	table.addtable(t.taskList,notexplore)
	table.addtable(t.taskList,exploring)
end

function t.Remove(data)
	table.removevalue(t.taskList,data)
end

function t.SetStatus(id,status)
	for i=1,#t.taskList do
		if t.taskList[i].id==id then
			local task=t.taskList[i]
			task.status=status
			task.overTime=LuaInterface.LuaCsTransfer.GetTime()+task.time
			table.remove(t.taskList,i)
			table.insert(t.taskList,task)
			break
		end
	end
end

function t.GetData(typeid)
	-- body
	--id=info.type
	if typeid == 30 then
		return	GetData_FirstCharge(typeid)
	elseif typeid == 60 then
		return GetData_ReturnGifts(typeid)
	else
		return GetData_Common(typeid)
	end
end

function t.HasUnhandledTask ()
  if t.taskList ~= nil then
    for i=1, #t.taskList do
      if t.taskList[i].status == 2 or t.taskList[i].status == 3 then  
        return true
      end
    end
  end
  return false
end
--探险成功率
function t.CalcExploreSucPercent( heroInfo)

  --基础值, 等级基础值, 等级保底值, 星级基础值
  local baseRate = 10
  local lvBaseRate = 30
  local lvMiniRate = 0 
  local starBaseRate = 30
  local qualityBaseRate = 30
  --英雄等级,等级,等级差，星级差
  local  heroLv
  local heroStar
  local heroQuality
  local lv_D_Value = 0
  local star_D_Value = 0
  local quality_D_Value = 0
  --账号等级
	local accountLevel = gamemanager.GetModel('game_model').accountLevel
  local starTemp = math.min(6,math.floor(accountLevel / 10))
  local qualityTemp = math.min(5,math.floor(accountLevel / 10))
  heroLv = heroInfo.level
  lv_D_Value = lv_D_Value + (accountLevel - heroLv)
  
  heroStar = heroInfo.advanceLevel
  star_D_Value = star_D_Value + starTemp - heroStar
  
  heroQuality = heroInfo.heroData.quality
  quality_D_Value = quality_D_Value + qualityTemp - heroQuality

  
  local lvRate = lvBaseRate - (lv_D_Value / 2)
  lvRate = math.max(0,lvRate)
  lvRate = math.min(lvBaseRate,lvRate)

  local starRate = starBaseRate - (star_D_Value * 5)
  starRate = math.max(0,starRate)
  starRate = math.min(starBaseRate,starRate)

  local qualityRate = qualityBaseRate - (quality_D_Value * 5)
  qualityRate = math.max(0,qualityRate)
  qualityRate = math.min(qualityBaseRate,qualityRate)
  
  local fianlRate = baseRate + lvRate + starRate + qualityRate
  local pt = math.min(100,fianlRate)
  return pt
end



Start()
return t
