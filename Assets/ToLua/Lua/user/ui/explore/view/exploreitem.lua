local t={}
local name='exploreitem'

local function Start( ... )
	t.__index=t
end

function t:Start( ... )
end

function t.New(go)
	local r={}
	setmetatable(r,t)
	r:Init(go)
	return r
end

function t:Init(go)
	self.go=go
	transform=go.transform
	hit=require('ui/explore/view/heroitem')
	
	self.lefttype=transform:FindChild('left/title/type'):GetComponent(typeof(Image))
	self.leftname=transform:FindChild('left/title/Text'):GetComponent(typeof(Text))
	self.leftitem=transform:FindChild('left/horizontal/item')
	self.typeimage=transform:FindChild('left/title/type'):GetComponent(typeof(Image))
	self.centername=transform:FindChild('center/title/Text'):GetComponent(typeof(Text))
	self.centeritem=transform:FindChild('center/horizontal/item')
	self.righttime_name=transform:FindChild('right/value/time/title'):GetComponent(typeof(Text))
	self.righttime=transform:FindChild('right/value/time/Text'):GetComponent(typeof(Text))
	self.rightpercent_name=transform:FindChild('right/value/percent/title'):GetComponent(typeof(Text))
	self.rightpercent=transform:FindChild('right/value/percent/Text'):GetComponent(typeof(Text))
	self.rightvaluet=transform:FindChild('right/value')
	self.rightovert=transform:FindChild('right/over')
	self.rightovername=transform:FindChild('right/over/Text'):GetComponent(typeof(Text))
	self.rightoverImage=transform:FindChild('right/over/Image'):GetComponent(typeof(Image))
	self.btn=transform:FindChild('right/Button'):GetComponent(typeof(Image))
	self.btnname=transform:FindChild('right/Button/Text'):GetComponent(typeof(Text))
	self.btnclick=transform:FindChild('right/Button'):GetComponent(typeof(Button)).onClick
	
	self.leftname.text=LuaInterface.LuaCsTransfer.LocalizationGet('explore_team')
	self.centername.text=LuaInterface.LuaCsTransfer.LocalizationGet('explore_award')
	self.percent='0%'
	self.ptview=uimanager.GetView('exploreview')

	self.nulls={}
	local  it = 1
	for i=1,3 do
		it=transform:FindChild('left/horizontal/add'..i):GetComponent(typeof(Button))
		self.nulls[i]=it
		--it.onClick:RemoveAllListeners()
		it.onClick:AddListener(function ()
			t.ClickAdd(self)
		end)
	end
	self.heroitems={}
	for i=1,3 do
		it=hit.New()
		it:Init(transform:FindChild('left/horizontal/item'..i))
		it.heroitem.onClick:RemoveAllListener()
		it.heroitem.onClick:AddListener(t.ClickHero)
		self.heroitems[i]=it
	end

	self.awards={}
	--it=require('ui/common_icon/common_item_icon')
	it=require('ui/explore/view/awarditem')
	for i=1,2 do
		--it.SetTrans(transform:FindChild('center/horizontal/award'..i))
		self.awards[i]=it.New(transform:FindChild('center/horizontal'))
		--self.awards[i].onClick:AddListener(t.)
	end

	self.btnclick:RemoveAllListeners()
	self.btnclick:AddListener(function ( ... )
			t.ClickExplore(self)
	end)
end

function t.ClickHero(itemself)
	print('total time:'..itemself.data.time)
	if itemself.data.status==0 then
		uimanager.GetView('exploreview').ChildOpen(itemself)
	else
		print('invalid')
	end
end
function t:ClickExplore( )
	--print(self.data.status)
	if self.data.status==0 then
		if gamemanager.GetModel('exploremodel').remainTimes <=0 then uimanager.ShowTipsOnly(LuaInterface.LuaCsTransfer.LocalizationGet('explore_invalid')) return end
		if self.data.hero_number == #self.data.heros then
			--print(self.data.id,self.data.heros[1])
			uimanager.GetView('exploreview').reqItem=self
			gamemanager.GetCtrl('explorectrl').ExploreTaskReq(self.data.id,self.data.heros)
		else
			uimanager.ShowTipsOnly(string.format(LuaInterface.LuaCsTransfer.LocalizationGet('explore_teamnotenough'),self.data.hero_number))
		end
	elseif self.data.status==2 or self.data.status==3 then
		uimanager.GetView('exploreview').reqItem=self
		gamemanager.GetCtrl('explorectrl').ExploreTaskRewardReq(self.data.id)
    local button = self.btn.transform:GetComponent(typeof(Button))
    button.interactable = false
	end
end

function t:ClickAdd()
	-- print('clickadd')
	uimanager.GetView('exploreview').ChildOpen(self)
end

function t:Ticked( ... )
	--print(self.overTime-LuaInterface.LuaCsTransfer.GetTime())
	--print('ticked',self.data.id)
	self.righttime.text=LuaInterface.LuaCsTransfer.GetCountDown(self.tick)
	if self.tick ==-1 then
		self:UnTick()
		gamemanager.GetCtrl('explorectrl').ExploreTaskSynReq(self.data.id) 
	end
	self.tick=self.tick-1
end
function t:Tick( ... )
	self.tick=self.overTime-LuaInterface.LuaCsTransfer.GetTime()-1
	--self.tick=self.overTime-LuaInterface.LuaCsTransfer.GetTime()
	table.insert(self.ptview.tickqueue,self)
	self.ptview.Tick()

end

function t:TickAgain( ... )
	-- print('TickAgain',self)
	self.tick=5
	self.righttime.text=LuaInterface.LuaCsTransfer.GetCountDown(self.tick)
	table.insert(self.ptview.tickqueue,self)
	self.ptview.Tick()
end

function t:UnTick( ... )
	self.ptview.UnTick()

	for i=1,#self.ptview.tickqueue do
		if self.ptview.tickqueue[i].data == self.data then
			table.remove(self.ptview.tickqueue,i)
			break
		end
	end
end

function t:SetStatus(status)
	self.data.status=status
	if status == 0 then
		self.btn.sprite=Common.ResMgr.ResMgr.instance:LoadSprite('sprite/main_ui/btn_equip_Square_01')
		self.btnname.text=LuaInterface.LuaCsTransfer.LocalizationGet('explore_begin')
	elseif status == 1 then
		self.btn.sprite=Common.ResMgr.ResMgr.instance:LoadSprite('sprite/main_ui/btn_equip_Square_01')
		self.btnname.text=LuaInterface.LuaCsTransfer.LocalizationGet('explore_working')
	else
		self.btn.sprite=Common.ResMgr.ResMgr.instance:LoadSprite('sprite/main_ui/btn_equip_Square_02')
		self.btnname.text=LuaInterface.LuaCsTransfer.LocalizationGet('explore_pickup')
	end
end

function t:ExploreSucess( ... )
	self.data.status=2
	self.rightvaluet.gameObject:SetActive(false)
	self.rightovert.gameObject:SetActive(true)
  ui_util.SetImageGray(self.rightoverImage,false)
	self.rightovername.text=LuaInterface.LuaCsTransfer.LocalizationGet('explore_success')
  ui_util.SetTextGray(self.rightovername,false)
	self:SetStatus(2)
end

function t:ExploreFailed( ... )
	self.data.status=3
	self.rightvaluet.gameObject:SetActive(false)
	self.rightovert.gameObject:SetActive(true)
  ui_util.SetImageGray(self.rightoverImage,true)
	self.rightovername.text=LuaInterface.LuaCsTransfer.LocalizationGet('explore_failed')
  ui_util.SetTextGray(self.rightovername,true)
	self:SetStatus(3)
end

function t:Setup(data)
	--print('setup:'..#data.heros)
	-- print(data.id)
	self.data=data
	--self.heros=data.heros
	self:SetHeros()
	self.overTime=data.overTime
	self.typeimage.sprite=require('util/ui_util').GetRoleTypeSmallIconSprite(data.herotype)
	if data.status==0 then
		self.rightvaluet.gameObject:SetActive(true)
		self.rightovert.gameObject:SetActive(false)
		self.righttime.text=LuaInterface.LuaCsTransfer.GetCountDown(data.time)
		self:SetStatus(data.status)
	elseif data.status==1 then
		self.rightvaluet.gameObject:SetActive(true)
		self.rightovert.gameObject:SetActive(false)
		self.righttime.text=LuaInterface.LuaCsTransfer.GetCountDown(data.overTime-LuaInterface.LuaCsTransfer.GetTime())
		self:SetStatus(data.status)
	elseif data.status==2 then
		self:ExploreSucess()
	elseif data.status==3 then
		self:ExploreFailed()
	end

	--local rd=require('ui/game/model/game_res_data')
	for i=1,#data.award do
		--print('award:',data.award[i])
		self.awards[i]:Setup(data.award[i])
		--self.awards[i]:SetActive(true)
	end
	for i=#data.award+1,2 do
		self.awards[i]:Show(false)
	end

	if data.status~=1 then
		self:UnTick()
	else
		self:Tick()
--		righttime.text=ParseTime(0)
	end
end

function t:SetHeros()
	num=#self.data.heros
	for i=1,num do
		self.heroitems[i]:Setup(self,self.data.heros[i])
	end
	max=self.data.hero_number
	for i=num+1,3 do
		self.heroitems[i]:Close()
	end
	for i=1,max-num do
		temp=self.nulls[i]
		temp.gameObject:SetActive(true)
	end
	for i=max-num+1,3 do
		self.nulls[i].gameObject:SetActive(false)
	end

	-- if #self.data.heros<max then
	-- 	self.rightpercent.text='0%'
	-- else
		self.rightpercent.text=self.ptview.CalculatePercent(self.data.heros,max)
	-- end
end

Start()
return t