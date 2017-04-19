local t={}
local name='exploreview'
local ctrl
local item
local container = {}

local using={}
local notuse={}
local model
local hero_model

local function Start( ... )
	uimanager.RegisterView(name,t)
	-- t.Start()
end

function t.Start( ... )
	local asset = LuaInterface.LuaCsTransfer.UIMgrOpen('ui/explore/explore_view',EUISortingLayer.MainUI,UIOpenMode.Replace)
	asset:GetComponent(typeof(UnityEngine.Canvas)).worldCamera=GameObject.Find('root/ui_root/ui camera'):GetComponent(typeof(UnityEngine.Camera))
	local transform=asset:GetComponent(typeof(Transform))
	-- transform:SetParent(GameObject.Find('root/ui_root/ui_2d_root').transform)
	-- transform.localScale=Vector3.one
	-- transform.localPosition=Vector3.zero

	local common_top_bar = require ('ui/common_top_bar/common_top_bar')
 	common_top_bar = common_top_bar:Create(transform)
	--common_top_bar.transform:SetAsFirstSibling()
    common_top_bar:SetAsCommonStyle(LuaInterface.LuaCsTransfer.LocalizationGet('explore_title'),
    	function()
    		t.Close()
    	end,
    	false,true,true,false,false,false,false)

    t.title=common_top_bar.transform:FindChild('text_title'):GetComponent(typeof(Text))

	t.transform=transform
	t.item=dofile('ui/explore/view/exploreitem')

	t.content=transform:FindChild('core/scrollrect/viewport/content'):GetComponent(typeof(Common.UI.Components.ScrollContent))
	t.content.onResetItem:AddListener(t.onResetItem)

	t.times=transform:FindChild('core/bottom/times/Text'):GetComponent(typeof(Text))
	t.timestitle=transform:FindChild('core/bottom/times/title'):GetComponent(typeof(Text))
	transform:FindChild('core/bottom/refresh/Text'):GetComponent(typeof(Text)).text=LuaInterface.LuaCsTransfer.LocalizationGet('public_refresh')
	transform:FindChild('core/bottom/refresh'):GetComponent(typeof(Button)).onClick:AddListener(t.ClickRefresh)
	t.freet=transform:FindChild('core/bottom/free')
	transform:FindChild('core/bottom/free/Text'):GetComponent(typeof(Text)).text=LuaInterface.LuaCsTransfer.LocalizationGet('public_free')
	t.pricet=transform:FindChild('core/bottom/price')
	t.priceicon=transform:FindChild('core/bottom/price/icon'):GetComponent(typeof(Image))
	t.price=transform:FindChild('core/bottom/price/Text'):GetComponent(typeof(Text))
	t.tickqueue={}
	model=gamemanager.GetModel('exploremodel')
	t.priceicon.sprite=Common.ResMgr.ResMgr.instance:LoadSprite(model.priceicon)
	t.price.text=model.price

	model.ExploreTaskRespListener:AddListener(t.ExploreTaskResp)
	model.ExploreListRespListener:AddListener(t.ExploreListResp)
	model.ExploreTaskRewardRespListener:AddListener(t.ExploreTaskRewardResp)
	model.ExploreTaskRefreshRespListener:AddListener(t.ExploreTaskRefreshResp)
	model.ExploreTaskSynRespListener:AddListener(t.ExploreTaskSynResp)
	asset:SetActive(true)
end

function t.onResetItem(go,idx)
	idx=idx+1
	it=t:GenItem(go)
	it:Setup(model.taskList[idx])
end

function t:GenItem(go)
	for i=1,#using do
		if using[i].go==go then
			using[i]:UnTick()
			return using[i]
		end
	end
	it=self.item.New(go)
	table.insert(using,it)
	return it
end

function t.CalculatePercent(heros,maxhero)
	if #heros == 0 or #heros < maxhero then
		return '0%'
	end
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
  local heroInfo = nil
  for k,v in ipairs(heros) do
    print (k,v)
    heroInfo = gamemanager.GetModel('hero_model').GetHeroInfo(v)
    heroLv = heroInfo.level
		lv_D_Value = lv_D_Value + (accountLevel - heroLv)
    
		heroStar = heroInfo.advanceLevel
    star_D_Value = star_D_Value + starTemp - heroStar
    
    heroQuality = heroInfo.heroData.quality
    quality_D_Value = quality_D_Value + qualityTemp - heroQuality
  end
  
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
	--[[if not hero_model then
		hero_model=gamemanager.GetModel('hero_model')
	end

	local herolvs=0
	for i=1,maxhero do
		if #heros>=i then			 
			if hero_model.GetHeroInfo(heros[i]) then
				herolvs=herolvs+hero_model.GetHeroInfo(heros[i]).level
			else
				print('not exist:'..tostring(heros[i]))
			end
		else
			break
		end
	end
	herolvs=50- math.floor((t.userlv* maxhero - herolvs)*.5)
	if herolvs<0 then herolvs=0 end
	-- print('等级影响：',herolvs)
	
	local userstar=math.floor(t.userlv*.1)
	if userstar>6 then userstar=6 end
	userstar=userstar*maxhero
	
	local starlvs=0
	for i=1,maxhero do
		if #heros>=i then			
			starlvs=starlvs+hero_model.GetHeroInfo(heros[i]).advanceLevel
		else
			break
		end
	end
	starlvs = 40-(userstar - starlvs)*5
	if starlvs > 40 then
		starlvs = 40 
	end
	-- starlvs=starlvs*5
	-- if starlvs>40 then starlvs=40 end
	if starlvs<0 then starlvs=0 end
	-- print('星级影响：',starlvs)

	local pt=10+herolvs+starlvs
	if pt<0 then
		pt=0
	elseif pt>100 then
		pt=100
	end]]

	return pt..'%'
end

function t.ClickRefresh( ... )
  --[[
	t.StopTick()
	t.userlv=tonumber(gamemanager.GetModel('game_model').accountLevel)
	gamemanager.GetCtrl('explorectrl').ExploreTaskRefreshReq()
  ]]--
  local costGameResData = require('ui/game/model/game_res_data').NewByString(gamemanager.GetData('global_data').explore_refresh)
  if gamemanager.GetModel('game_model').GetBaseResourceValue(costGameResData.type) < costGameResData.count then
    local confirm_tip_view = require('ui/tips/view/confirm_tip_view')
    confirm_tip_view.Open(LocalizationController.instance:Get('explore_refresh_diamond_not_enough_and_go_to_buy'), t.GoToBuyDiamond)
    return
  end
  
  local consume_tip_model = gamemanager.GetModel('consume_tip_model')
  local consumeTipData = gamemanager.GetData('consume_tip_data').GetDataById(ConsumeTipType.ExploreRefresh)
  local tipStr = LocalizationController.instance:Get(consumeTipData.des)
  local confirm_cost_tip_view = require 'ui/tips/view/confirm_cost_tip_view'
  if consume_tip_model.GetConsumeTipEnable(ConsumeTipType.ExploreRefresh) then
    confirm_cost_tip_view.Open(costGameResData.type, costGameResData.count, tipStr, t.ConfirmRefresh, ConsumeTipType.ExploreRefresh)
  else
    t.ConfirmRefresh ()
  end
end

function t.GoToBuyDiamond ()
  gamemanager.GetModel('function_open_model').OpenFunction(FunctionOpenType.MainView_Shop, 3)
end

function t.ConfirmRefresh ()
  t.StopTick()
	t.userlv=tonumber(gamemanager.GetModel('game_model').accountLevel)
	gamemanager.GetCtrl('explorectrl').ExploreTaskRefreshReq()
end

function t.ExploreOpen()
	t.Start()
	t.ExploreListResp()
end

function t.ChildOpen(item)
	childview=require('ui/explore/view/childview')
	if not childview.opened then
		childview.Init(t.transform:FindChild('core/child'))
	end
	childview.Open(item)
end

function t.ExploreTaskSynResp(proto)
	print('ExploreTaskSynResp:',proto.id,proto.status)
	local flag=false
	for i=1,#using do
		if using[i].data.id==proto.id then
			flag=true
			if proto.status==1 then
				using[i]:TickAgain()
			elseif proto.status==2 then
				using[i]:ExploreSucess()
			elseif proto.status==3 then
				using[i]:ExploreFailed()
			end
			--print('cd ok')
			break
		end
	end
	if not flag then 
		print('无匹配已完成的任务',proto.id)
		for i=1,#using do
			print('已加载的任务id集合: ',using[i].data.id)
		end
	end
end

function t.ExploreTaskRewardResp()
	--print('ExploreTaskRewardResp')
	--table.removevalue(model.taskList,t.reqItem.data)
	require('ui/tips/view/auto_destroy_tip_view').Open(LuaInterface.LuaCsTransfer.LocalizationGet('explore_getaward'))
	
	model.Remove(t.reqItem.data)
	--model.taskList.count=model.taskList.count-1
	using={}
	t.StopTick()

	t.content:Init(#model.taskList,false,0)
end

function t.ExploreTaskResp()
	--print('ExploreTaskResp')
	require('ui/tips/view/auto_destroy_tip_view').Open(LuaInterface.LuaCsTransfer.LocalizationGet('explore_start'))
	model.ExploreTaskRespOK(t.reqItem.data)
	--t.reqItem:SetStatus(1)
	--t.reqItem.tick=t.reqItem.data.time
	--t.reqItem.data.overTime=LuaInterface.LuaCsTransfer.GetTime()+t.reqItem.data.time
	--t.reqItem.overTime=t.reqItem.data.overTime
	--t.reqItem:Tick()
	t.times.text=''..model.remainTimes
	t.timestitle.text=LuaInterface.LuaCsTransfer.LocalizationGet('explore_remain_times')
	for i=1,#using do
		for j=1,#using[i].heroitems do
			using[i].heroitems[j].transform=nil
		end
	end
	using={}
	t.StopTick()
	require('ui/explore/view/childview').SetNil()

	t.content:Init(#model.taskList,true,0)
end

function t.ExploreTaskRefreshResp( ... )
	t.freet.gameObject:SetActive(false)
	t.pricet.gameObject:SetActive(true)
	-- local model=gamemanager.GetModel('exploremodel')
	-- t.priceicon.sprite=model.priceicon
	-- t.price.text=model.price
	
	for i=1,#using do
		for j=1,#using[i].heroitems do
			using[i].heroitems[j].transform=nil
		end
	end
	using={}
	t.StopTick()
	require('ui/explore/view/childview').SetNil()

	t.content:Init(#model.taskList,true,0)
end

function t.ExploreListResp()
	-- model.GetExploringHeros()
	if model.freeRefreshIsUsed then
		t.freet.gameObject:SetActive(false)
		t.pricet.gameObject:SetActive(true)
		-- t.priceicon.sprite=Common.ResMgr.ResMgr.instance:LoadSprite(model.priceicon)
		-- t.price.text=model.price
	else
		t.freet.gameObject:SetActive(true)
		t.pricet.gameObject:SetActive(false)
	end
	--t.list=model.taskList
	
	t.userlv=tonumber(gamemanager.GetModel('game_model').accountLevel)	
	t.times.text=''..model.remainTimes
	t.timestitle.text=LuaInterface.LuaCsTransfer.LocalizationGet('explore_remain_times')
	t.content:Init(#model.taskList,true,0)
end

function t.tick( ... )
	while true do
		if #t.tickqueue<1 then break end
		for i=1,#t.tickqueue do
			if t.tickqueue[i] then
				t.tickqueue[i]:Ticked()
			end
		end
		coroutine.wait(1)
	end
end
function t.Tick( ... )
	if not t.cotick then
		t.cotick=coroutine.start(t.tick)
	end
end
function t.UnTick( ... )
	if not t.cotick or not t.tickqueue then return end
	if #t.tickqueue<1 then
		if t.cotick then
			coroutine.stop(t.cotick)
			t.cotick=nil
		end
	end
end

function t.StopTick( ... )
	t.tickqueue={}
	if t.cotick then
		coroutine.stop(t.cotick)
		t.cotick=nil
	end
end

function t.Close()
	-- for i=1,#using do 
	-- 	it=using[i]
	-- 	it.gameObject:SetActive(false)
	-- 	using[i]=nil
	-- 	table.insert(notuse,it)
	-- end
	t.StopTick()
	t.opened=false
	require('ui/explore/view/childview').Close()

	model.ExploreTaskRespListener:RemoveListener(t.ExploreTaskResp)
	model.ExploreListRespListener:RemoveListener(t.ExploreListResp)
	model.ExploreTaskRewardRespListener:RemoveListener(t.ExploreTaskRewardResp)
	model.ExploreTaskRefreshRespListener:RemoveListener(t.ExploreTaskRefreshResp)
	model.ExploreTaskSynRespListener:RemoveListener(t.ExploreTaskSynResp)
	LuaInterface.LuaCsTransfer.UIMgrClose('ui/explore/explore_view')
end

Start()
return t