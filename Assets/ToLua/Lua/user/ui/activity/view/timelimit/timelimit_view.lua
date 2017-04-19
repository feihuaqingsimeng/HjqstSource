local t={}
local name='activitytimelimitview'
local item
local data
-- local item

local function GenItem()
	-- body
	length=#notuseItems
	if length>0 then
		obj=notuseItems[length]
		notuseItems[length]=nil 
	else
		obj=Instantiate(item)
		obj:SetParent(item.parent)
		obj.localScale=Vector3(1,1,1)
	end
	obj.gameObject:SetActive(true)
	table.insert(usingItems,obj)
	return obj
end

local function GenItemCommon()
	local objcmn=1
	length=#notuseItems
	if length>0 then
		objcmn=notuseItems[length]
		notuseItems[length]=nil 
	else
		objcmn=dofile('ui/activity/view/accumulatecharge/accuchargeitem_item')
	end
	table.insert(usingItems,objcmn)
	return objcmn
end

local function GotoMarket( ... )
	-- body
	--print('跳转充值')
	--LuaInterface.LuaCsTransfer.UIMgrOpen('ui/shop/shop_view',0,0)
	--LuaInterface.LuaCsTransfer.UIMgrOpen('ui/shop/shop_view',0,0)
	gamemanager.GetCtrl('black_market_controller').OpenBlackMarketView(5)
end

local function GotoAward( ... )
	-- body
	print('领取奖励')
	gamemanager.GetCtrl('activityctrl').ReqReward(data.event_type,data.small_id,data.event_award)
end

local function ShowGoods(goods)
	-- body
	for i=1,#goods do
		--local it=GenItem()	
		--local ait=dofile('ui/activity/view/firstcharge/fstcharge_item')
		--local ait=dofile('ui/activity/view/accumulatecharge/accuchargeitem_item')
		local ait = GenItemCommon()
		ait.Setup(t.transform:FindChild('center'),goods[i])		
	end
end

function t.Show(parentv,datas,info)
	t.parentv=parentv
	if not t.transform then
		-- t.transform=parentv.transform:FindChild('core/timelimit')
		local transform=Instantiate(ResMgr.instance:Load('ui/activity/timelimit')).transform
		transform:SetParent(parentv.coretransform)
		transform:SetSiblingIndex(0)
		transform.localPosition=Vector3(0,0,0)
		transform.localScale=Vector3.one
		t.transform=transform
		-- item=t.transform:FindChild('center/award')
	end
	data=datas
	parentv.UpdateName(LocalizationController.instance:Get(data.event_name))
	parentv.ShowView(t)

	t.id=info.type
	t.transform:FindChild('title/Text'):GetComponent(typeof(Text)).text=LocalizationController.instance:Get(data.event_name)
	t.transform:FindChild('time/Text'):GetComponent(typeof(Text)).text=LocalizationController.instance:Get('event_public_time')..':'
	t.transform:FindChild('desc/Text'):GetComponent(typeof(Text)).text=LocalizationController.instance:Get('event_public_describe')..':'
	
	local time1,time2=uimanager.DealWithTime(data.event_timestart,data.event_timeover)
	if time2 then
		t.transform:FindChild('time'):GetComponent(typeof(Text)).text=time1..'~'..time2
	else
		t.transform:FindChild('time'):GetComponent(typeof(Text)).text=time1
	end
	t.transform:FindChild('desc'):GetComponent(typeof(Text)).text=LocalizationController.instance:Get(data.event_describe)

	t.transform:FindChild('goto/Text'):GetComponent(typeof(Text)).text=LocalizationController.instance:Get('event_1009_btn')
	local ck=t.transform:FindChild('goto'):GetComponent(typeof(Button)).onClick
	ck:RemoveAllListeners()
	ck:AddListener(GotoMarket)

	if data.event_award then
		ShowGoods(data.event_award)		
	else
		print('no goods')
	end

	--print(tostring(item))
end

function t.Setup(who)
	-- body
	ctrl=who
end


function t:Open()
	--print('open fstcharge')
		
	t.parentv.UpdateName(LocalizationController.instance:Get(data.event_name))
	self.showed=true
	self.transform.gameObject:SetActive(true)
end

function t:Close()
	-- body
	self.showed=false
	self.transform.gameObject:SetActive(false)
end

return t