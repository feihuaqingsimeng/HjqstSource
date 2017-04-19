local t={}
local name='exploreview'
local ctrl
local item
local container = {}

local using={}
local hero_model

local function Start( ... )
	hero_model=gamemanager.GetModel('hero_model')
end

local function CalculatePercent( ... )
	-- if #t.heros <t.maxhero then
	-- 	t.curitem.percent='0%'
	--     --t.percent.text=string.format(LuaInterface.LuaCsTransfer.LocalizationGet('explore_cur_percent'),'0%')
	--   	t.percenttitle.text=LuaInterface.LuaCsTransfer.LocalizationGet('explore_cur_percent')
	-- 	t.percentvalue.text=''
	--   	return 
	--  end
	if #t.heros == 0 or #t.heros < t.maxhero then
		t.percentvalue.text='0%'
		return
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
  for k,v in pairs(t.heros) do
     print (k,v)
    heroInfo = hero_model.GetHeroInfo(v)
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
  
	t.curitem.percent=pt ..'%'
	--t.percent.text=string.format(LuaInterface.LuaCsTransfer.LocalizationGet('explore_cur_percent'),t.curitem.percent)
	t.percenttitle.text=LuaInterface.LuaCsTransfer.LocalizationGet('explore_cur_percent')
	t.percentvalue.text=t.curitem.percent
end

function t.Init(transform)
	t.opened=true
	t.transform=transform
	t.item=dofile('ui/explore/view/childitem')
	t.contentTrans=transform:FindChild('scrollrect/viewport/content')
	transform:FindChild('close'):GetComponent(typeof(Button)).onClick:AddListener(t.ClickClose)
	transform:FindChild('verify'):GetComponent(typeof(Button)).onClick:AddListener(t.ClickVerify)
	
	t.maxhero1=transform:FindChild('herotitle/layout/title1'):GetComponent(typeof(Text))
	t.heronum=transform:FindChild('herotitle/layout/Text'):GetComponent(typeof(Text))
	t.maxhero2=transform:FindChild('herotitle/layout/title2'):GetComponent(typeof(Text))
	
	--t.percent=transform:FindChild('percenttitle/percent'):GetComponent(typeof(Text))
	
	t.percenttitle=transform:FindChild('percenttitle/title'):GetComponent(typeof(Text))
	t.percentvalue=transform:FindChild('percenttitle/Text'):GetComponent(typeof(Text))

	transform:FindChild('verify/Text'):GetComponent(typeof(Text)).text=LuaInterface.LuaCsTransfer.LocalizationGet('public_verify')
	transform:FindChild('title'):GetComponent(typeof(Text)).text=LuaInterface.LuaCsTransfer.LocalizationGet('explore_title_child')
	t.content=t.contentTrans:GetComponent(typeof(Common.UI.Components.ScrollContent))
	t.content.onResetItem:AddListener(t.onResetItem)
end

function t.ClickClose( ... )
	t.olditem=t.curitem
	t.olddata=t.curitem.data
	t.transform.gameObject:SetActive(false)
end

function t.ClickVerify( ... )
	t.transform.gameObject:SetActive(false)
	t.olddata=t.curitem.data
	t.olditem=t.curitem
	t.olditem.data.heros=t.heros
	t.olditem:SetHeros()
end

function t.onResetItem(go,idx)
	idx=idx+1
	it=t:GenItem(go)
	info=t.infolist[idx]
	it:Setup(info)
	it.heroitem.onClick:RemoveAllListener()
	it.heroitem.onClick:AddListener(t.ClickSelect)
	
	--table.insert(t.herositem,it)

	if t.herotype ~= info.heroData.roleType then
		it:Invalid()
		return
	else
		--print(t.type,info.heroData.roleType)
	end

	if table.contain(t.explored,info.instanceID) then
		it:Explored()
		return
	end
	if table.contain(t.exploring,info.instanceID) then
		it:Exploring()
		return
	end
	if table.contain(t.heros,info.instanceID) then
		it:Select()
		--return
	else
		it:UnSelect()
		--return
	end

	t.herositem[info.instanceID]=it
end

function t:GenItem(go)
	for i=1,#using do
		if using[i].go==go then
			return using[i]
		end
	end

	it=self.item.New(go)
	it:Init(go)
	table.insert(using,it)
	return it
end

function t.ClickSelect(item)
	if item.selected then
		if #t.heros==t.maxhero then
			t.herositem[t.heros[t.maxhero]]:UnSelect()
			table.remove(t.heros,t.maxhero)
			-- table.remove(t.herositem,t.maxhero)
		end
		table.insert(t.heros,item.heroitem.roleInfo.instanceID)
		--table.insert(t.herositem,item)
	else
		table.removevalue(t.heros,item.heroitem.roleInfo.instanceID)
		--table.removevalue(t.herositem,item)
	end
	CalculatePercent()
end

function t.Open(iteme)
	t.herotype=iteme.data.herotype	
	t.explored=gamemanager.GetModel('exploremodel').exploredHeros
	t.exploring=gamemanager.GetModel('exploremodel').exploringHeros
	local herolist=gamemanager.GetModel('hero_model').GetAllHeroInfoList()
	
	t.percentvalue.text=''

	t.infolist={}
	
	-- print('herototal :',table.count(herolist))
	-- for i=1,#t.explored do
	-- 	print('explored:',t.explored[i])
	-- end

	local listcanuse={}
	local count=1
	for k,v in pairs(herolist) do
		if v.heroData.roleType ~= t.herotype then
			-- count=count+1
			-- print('!type not equal')
			table.insert(t.infolist,v)
		else
			if table.contain(t.explored,v.instanceID) or table.contain(t.exploring,v.instanceID) then
				-- print('!type has used')
				-- table.insert(t.infolist,1,v)
			else
				-- print('!type can use')
				table.insert(listcanuse,v)
			end
		end
	end
  local explore_model = gamemanager.GetModel('exploremodel')
	table.sort(listcanuse,function (a,b)
		local p1 = explore_model.CalcExploreSucPercent(a) 
    local p2 = explore_model.CalcExploreSucPercent(b)
    if p1 == p2 then
      p1 = a.heroData.quality
      p2 = b.heroData.quality
    end
    if p1 == p2 then
      p1 = a.advanceLevel
      p2 = b.advanceLevel
    end
    if p1 == p2 then
      p1 = a.level
      p2 = b.level
    end
    return p1 > p2
	end)

	-- print('listcanuse ',#listcanuse)

	local lvstemp={}
	local lvstable={}
	if #listcanuse > 0 then
		local stars = listcanuse[1].advanceLevel
		for i=1,#listcanuse do
			if listcanuse[i].advanceLevel == stars then
				table.insert(lvstable,listcanuse[i])
			else
				table.sort(lvstable,function (a,b)
					return a.level>b.level
				end)
				table.addtable(lvstemp,lvstable)
				lvstable={}

				table.insert(lvstable,listcanuse[i])

				stars=listcanuse[i].advanceLevel
			end
		end
	end
	-- print('listcanuse ',#lvstable)

	-- if #listcanuse == 1 then
	-- 	table.addtable(lvstemp,lvstable)
	-- end
	-- if #listcanuse>1 and #lvstemp <1 then
	-- end
	table.addtable(lvstemp,lvstable)

	local curtemp={}
	table.addtable(curtemp,lvstemp)
	table.addtable(curtemp,t.infolist)
	t.infolist=curtemp

	t.userlv=tonumber(gamemanager.GetModel('game_model').accountLevel)
	t.heros={}
	t.herositem={}
	-- for i=1,#iteme.data.heros do
	-- 	table.insert(t.heros,iteme.data.heros[i])
	-- end
	table.addtable(t.heros,iteme.data.heros)
--	if t.olddata and t.olddata ~= iteme.data then
	if t.olddata then
		--print(t.olddata)
		if t.olddata.status==0 and t.olddata ~= iteme.data then
			t.olddata.heros={}
		else
			--print('same data')
		end
	else
		--print('old is not exist')
	end
	if t.olditem then
		if t.olddata~=iteme.data then
	 		t.olditem:SetHeros() 
		end
	end

	t.curitem=iteme
	t.maxhero=iteme.data.hero_number
	
	t.maxhero1.text=LuaInterface.LuaCsTransfer.LocalizationGet('explore_max_heros1')
	t.heronum.text=string.format('  %s  ',t.maxhero)
	t.maxhero2.text=LuaInterface.LuaCsTransfer.LocalizationGet('explore_max_heros2')
	
	t.content:Init(#t.infolist,true,0)

	-- print('exploringHeros')
	-- for k,v in pairs(t.exploring) do
	-- 	print(v)
	-- end
	-- print('exploredHeros')
	-- for k,v in pairs(t.explored) do
	-- 	print(v)
	-- end

	CalculatePercent()
	t.transform.gameObject:SetActive(true)
end

function t.SetNil( ... )
	if t.olddata and t.olddata.status ==0 then
		t.olddata.heros={}
		t.olddata=nil
	end
	t.olditem=nil
end

function t.Close()
	-- for i=1,#using do 
	-- 	it=using[i]
	-- 	it.gameObject:SetActive(false)
	-- 	using[i]=nil
	-- 	table.insert(notuse,it)
	-- end
	using={}
	t.SetNil()
	t.opened=false
	--self.transform.gameObject:SetActive(false)
	--LuaInterface.LuaCsTransfer.UIMgrOpen('ui/main/main_view',0,0)
end

Start()
return t